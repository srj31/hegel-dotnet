# hegel-dotnet — Architecture & Design

This is the design document for `hegel-dotnet`, a .NET client for the
[Hegel](https://hegel.dev) property-based testing protocol. It records the
architecture, the decisions behind it, and the implementation roadmap. The
repository is currently a **scaffold**: this document describes the target
design; most code is stubbed.

## 1. The three-layer system

Hegel separates the *engine* from the *language client*:

1. **`hegel-core`** (Python) — the property-testing brain: data generation and
   shrinking, built on Hypothesis. It can run as a subprocess server speaking a
   framed-CBOR packet protocol over stdio.
2. **`hegel-rust`** — the protocol client to `hegel-core`, wrapped behind a **C
   ABI** (`hegel.h`) and compiled into a native shared library, **`libhegel`**
   (`.so` / `.dylib` / `.dll`), published as GitHub release binaries.
3. **`hegel-dotnet`** (this repo) — binds `libhegel` over the C ABI and exchanges
   CBOR values. We replicate layer 3 only; the stdio packet protocol is internal
   to how Rust talks to Python and is hidden behind the C ABI.

### Why FFI rather than the subprocess protocol

The two reference clients diverge: **hegel-java** uses FFI into `libhegel`;
**hegel-typescript** spawns the Python server. We chose **FFI** because it
yields a self-contained NuGet with no runtime dependency on Python — natives are
bundled per-RID, the standard .NET native-packaging pattern. The cost is
shipping platform binaries and writing P/Invoke marshalling. F# has no
`[LibraryImport]` source generator (a C#-only feature), so the binding uses
classic `[<DllImport>]` / `extern` declarations against the cdecl C ABI.

## 2. Project layout

```
src/Hegel/            Core library (no test-framework dependency)
  Interop/            Abi (ABI constants), NativeMethods ([<DllImport>]/extern), LibraryLoader
  Cbor/               CborCodec (schema/value framing), Wtf8 (CBOR tag-91 decode)
  Generator.fs        IGenerator<'T> + TestCase (mutually recursive, so one file)
  Generators/         concrete generators (IntegerGenerator, BooleanGenerator)
  Gen.fs              Gen module (factories + map/filter/bind) and the gen { } CE
  Engine, Runner, IDataSource, LiveDataSource
  Settings, Database (DU), Enums (Verbosity/Mode/Phase/HealthCheck), Exceptions
src/Hegel.Xunit/      [<Property>] attribute (xUnit integration)
tests/Hegel.Tests/    xUnit behavioral suite
tests/Hegel.Conformance/  Per-generator console binaries for the Python conformance driver
```

## 3. Native binding (the FFI surface)

`NativeMethods` declares the `libhegel` C ABI via F# `extern` declarations
attributed with `[<DllImport("hegel", CallingConvention = CallingConvention.Cdecl)>]`.
`LibraryLoader` resolves the native library at first use, in order:

1. `$HEGEL_LIBHEGEL_PATH` — explicit override (an existing file).
2. The OS loader search path (`LD_LIBRARY_PATH` / `DYLD_LIBRARY_PATH` / `PATH`).
3. The bundled native shipped in the NuGet at `runtimes/<rid>/native/libhegel.<ext>`,
   resolved automatically by the .NET runtime — no manual unpacking.

A `NativeLibrary.SetDllImportResolver` hook implements (1)/(2); (3) is the
default runtime behavior.

### The C ABI (ported from hegel-java's bindings)

Handles are opaque pointers (`nint`). Strings returned by the engine are
`const char*` that **must be copied immediately**. The full surface:

- **Settings lifecycle:** `hegel_settings_new/_free`, and setters
  `_mode/_test_cases/_verbosity/_seed/_derandomize/_report_multiple_failures/`
  `_database/_database_key/_phases/_suppress_health_check`.
- **Run lifecycle:** `hegel_run_start(settings) -> run`, `hegel_next_test_case(run) -> tc`
  (NULL = error or end), `hegel_run_result(run) -> result`, `hegel_run_free`.
- **Per-test-case draws** (return `int32` `hegel_error_t`): `hegel_generate(tc, schema, len, out_ptr, out_len)`,
  `hegel_start_span/_stop_span`, `hegel_new_collection/_collection_more/_collection_reject`,
  `hegel_target`, `hegel_mark_complete`, `hegel_test_case_is_final_replay`.
- **Results / diagnostics:** `hegel_run_result_passed/_failure_count/_failure`,
  `hegel_failure_panic_message/_diagnostic/_origin`, `hegel_last_error_message`, `hegel_version`.

### ABI constants (`Abi.fs`)

Ported verbatim — these must match the C ABI exactly:

- `hegel_error_t`: `OK=0, E_STOP_TEST=-1, E_ASSUME=-2, E_BACKEND=-3, E_INVALID_HANDLE=-4, E_INVALID_ARG=-5, E_ALREADY_COMPLETE=-6, E_NOT_COMPLETE=-7, E_INTERNAL=-8`.
- Phases bitmask: `EXPLICIT=1, REUSE=2, GENERATE=4, TARGET=8, SHRINK=16, ALL=31`.
- Health-check suppress bitmask: `FILTER_TOO_MUCH=1, TOO_SLOW=2, TEST_CASES_TOO_LARGE=4, LARGE_INITIAL_TEST_CASE=8`.
- Span labels (1–17): `LIST, LIST_ELEMENT, SET, SET_ELEMENT, MAP, MAP_ENTRY, TUPLE, ONE_OF, OPTIONAL, FIXED_DICT, FLAT_MAP, FILTER, MAPPED, SAMPLED_FROM, ENUM_VARIANT, STATEFUL, COMPOSITE`.
- `hegel_mode_t`: `TEST_RUN=0, SINGLE_TEST_CASE=1`. `hegel_verbosity_t`: `QUIET=0, NORMAL=1, VERBOSE=2, DEBUG=3`.
- `hegel_status_t` (mark_complete): `VALID=0, INVALID=1, OVERRUN=2, INTERESTING=3`. `UNBOUNDED = UINT64_MAX`.

## 4. The run loop

`Property.Check(body, settings)` → `Runner`:

1. `settings = hegel_settings_new()`; apply each `Settings` field via the setters.
2. `run = hegel_run_start(settings)` (NULL ⇒ `HegelException` from `hegel_last_error_message`).
3. Loop `tc = hegel_next_test_case(run)` until NULL. For each case, build a
   `TestCase` over a `LiveDataSource(tc)` and invoke the user `body`:
   - `tc.Draw(generator)` → basic generators make one `hegel_generate(schema)`
     call; composite generators open a span and draw sub-values.
   - Map the outcome to a status: normal → `VALID`; `AssumeRejected` → `INVALID`;
     `StopTest` → `OVERRUN`; any other exception → `INTERESTING` (capture origin).
   - `hegel_mark_complete(tc, status, origin)`.
4. `result = hegel_run_result(run)`; if not passed, rethrow the minimal
   counterexample's exception (or aggregate when `ReportMultipleFailures`).

**Shrinking** and the **example database** are entirely engine-side; the client
only keeps calling `hegel_next_test_case` and passes a database directory + key.
The minimal example is replayed with `is_final_replay == true`, at which point
each top-level `Draw` prints `label = value;`.

## 5. CBOR data model

Schemas are CBOR maps `{"type": "<name>", ...params}`; values come back as CBOR.
`CborCodec` uses `System.Formats.Cbor`. Notes:

- Integers can exceed 64 bits — decode to `System.Numerics.BigInteger`. The
  conformance suite exercises BIGINT bounds only if `BigInteger` generators are
  exposed; the fixed-width `int`/`long` generators use INT32/INT64 bounds.
- **CBOR tag 91 = WTF-8 string** (`Wtf8.fs`): decode the tagged byte string as
  UTF-8 with surrogate passthrough so lone surrogates survive. Decode-only.
- String lengths are counted in Unicode **codepoints**, not UTF-16 units.

## 6. Test-framework integration

The core library has no test-framework dependency. `Hegel.Xunit` provides
`[Property]` — an xUnit attribute paired with a test-case runner that drives the
whole property loop and reports a **single** pass/fail node (the FsCheck model).
NUnit / MSTest adapters can target the same `Runner` seam later.

Authoring style (v1): the `TestCase`-parameter form, matching every existing
Hegel client —

```fsharp
open Hegel
open Hegel.Xunit

[<Property>]
let additionCommutes (tc: TestCase) =
    let x = tc.Draw(Gen.integers ())
    let y = tc.Draw(Gen.integers ())
    Assert.Equal(x + y, y + x)
```

An argument-injection style (`[<Property>] let add (x: int) (y: int) = …`) is a
possible fast-follow on top of the same engine.

## 7. Conformance

`hegel-core` ships a cross-language conformance harness that spawns a client
**binary per generator** and validates a stream of JSONL metrics. `tests/Hegel.Conformance`
will host those console binaries (one per generator type), driven by the Python
`hegel.conformance` runner, certifying protocol parity. In addition, `Hegel.Tests`
holds an in-process xUnit behavioral suite (the hegel-java approach).

## 8. Implementation roadmap

1. **Interop:** finish `NativeMethods` (full ABI), `LibraryLoader`, marshalling
   helpers (UTF-8 in/out, the `hegel_generate` out-buffer, settings string lifetimes).
2. **CBOR:** `CborCodec` encode (schemas) + decode (values), `Wtf8`, BigInteger.
3. **Run loop:** `Runner`, `TestCase`, `LiveDataSource`, status mapping, failure reporting.
4. **Primitives:** `Integers/Longs/Booleans/Floats/Doubles/Text/Binary` + `Just/SampledFrom`.
5. **Combinators:** `Map/Filter/FlatMap` (basic vs composite split), `OneOf/Optional`.
6. **Collections & tuples:** `Lists/Sets/Maps`, value-tuple generators.
7. **Spans / advanced:** composite, deferred (recursion), `Target`, formats (emails/uuids/regex), temporal.
8. **Integration & conformance:** `[Property]` runner, conformance binaries, docs.

Native packaging (`build/fetch-natives.py` → `runtimes/<rid>/native/`) is wired
in parallel, covering `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`,
`osx-x64`, `osx-arm64`.
