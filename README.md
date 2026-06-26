# hegel-dotnet

A .NET client for the [Hegel](https://hegel.dev) property-based testing protocol —
property-based testing for F#, built on [Hypothesis](https://github.com/hypothesisworks/hypothesis).

> [!WARNING]
> **Status: scaffold / work in progress.** This repository currently contains the
> project structure, the public API surface, and the native-interop ABI constants —
> but the implementation is stubbed (most methods throw `NotImplementedException`).
> It does not run property tests yet. See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
> for the design and the implementation roadmap.

## How it works

Hegel is a three-layer system:

1. **`hegel-core`** (Python) — the generation + shrinking engine, built on Hypothesis.
2. **`hegel-rust`** — wraps the engine behind a C ABI, compiled to a native
   `libhegel` shared library and published as release binaries.
3. **`hegel-dotnet`** (this repo) — binds `libhegel` via P/Invoke
   (`[<DllImport>]` / `extern`) and exchanges CBOR-encoded values across the C
   ABI. No Python or subprocess is required at runtime; the native library is
   bundled in the NuGet package per [RID](https://learn.microsoft.com/dotnet/core/rid-catalog).

This is the same model as [hegel-java](https://github.com/hegeldev/hegel-java).
(The [TypeScript client](https://github.com/hegeldev/hegel-typescript) instead
spawns the Python server and speaks the stdio packet protocol — `hegel-dotnet`
deliberately takes the in-process FFI route, which keeps it a self-contained,
dependency-free NuGet.)

## Packages

| Package | What it is |
| --- | --- |
| `Hegel` | Core library: native binding, engine run-loop, generators, `Settings`. No test-framework dependency. |
| `Hegel.Xunit` | xUnit integration: the `[Property]` attribute. (NUnit / MSTest adapters can follow the same seam.) |

## Development

The dev environment is pinned with Nix flakes — the flake provides the .NET 10
SDK, so you don't need a matching SDK installed globally.

```bash
nix develop            # enter a shell with the pinned .NET 10 SDK (+ just, python3)
dotnet build           # build the solution
dotnet test            # run the test suite
```

With [direnv](https://direnv.net) + `nix-direnv`, the `.envrc` (`use flake`)
auto-enters the shell on `cd`.

Common tasks are in the [`justfile`](justfile): `just build`, `just test`,
`just format`, `just fetch-natives`.

## Goal: an idiomatic F# property-testing library

`hegel-dotnet` aims to be the "ideal" F# Hegel client — following F# conventions
throughout: `[<DllImport>]` / `extern` P/Invoke, RID-based native packaging,
Central Package Management, discriminated unions for closed configurations
(`Database`), records + `with` for immutable `Settings`, F# `option` instead of
nullable fields, module functions (`Gen.integers`, `Gen.map`/`filter`/`bind`),
and a `gen { }` computation expression for sequencing dependent draws.

## License

MIT — see [LICENSE](LICENSE). This is an independent client; see [NOTICE](NOTICE)
for upstream attribution to the Hegel project and Hypothesis.
