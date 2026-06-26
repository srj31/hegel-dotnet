# Changelog

All notable changes to this project are documented here. The format is based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project aims
to adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

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
