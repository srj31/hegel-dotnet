# Changelog

All notable changes to this project are documented here. The format is based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project aims
to adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- **M0 — native engine binding is live.** `Engine.Version()` calls the real
  `libhegel` over P/Invoke and returns the engine version (verified `0.23.1`), proven
  by a test (`tests/Hegel.Tests/GeneratorTests.fs`). The host-RID native is copied
  into build output so the suite runs without `HEGEL_LIBHEGEL_PATH`.
- Initial F# repository scaffold: `Hegel.slnx` solution, `Hegel` core +
  `Hegel.Xunit` integration projects, test + conformance projects.
- Tooling: Nix flake dev shell (pinned .NET 10 SDK), Central Package Management,
  `Directory.Build.props`, `.editorconfig`, Fantomas (`.config/dotnet-tools.json`),
  GitHub Actions CI (cross-platform build/test + Fantomas format check).
- `docs/ARCHITECTURE.md` — the three-layer design, FFI ABI map, and run-loop.
- Native-interop ABI constants (`Abi`) ported verbatim from the C ABI: error
  codes, phase / health-check bitmasks, span labels, mode / verbosity / status.
  The P/Invoke surface uses F# `extern` / `[<DllImport>]` (cdecl).
- Public API surface stubs in idiomatic F#: `Settings` (record), `Database`
  (discriminated union), generators with the `Gen` module and a `gen { }`
  computation expression, `TestCase`, exceptions, the `[<Property>]` attribute.
  Implementations are not yet wired up (`failwith "Not implemented"`).

### Changed

- **ABI correction.** The live v0.23.1 C ABI (`hegel-c/include/hegel.h`) is
  context-based: every entry point takes a `hegel_context_t*` first and returns a
  `hegel_result_t`. Bound `hegel_version` correctly and added `hegel_context_new`/
  `_free` in `NativeMethods`; the remaining settings/run declarations are still
  pre-context and flagged in-source for correction in M1.
