// hegel-dotnet conformance binary.
//
// The cross-language conformance harness in hegel-core (`hegel.conformance`)
// spawns one binary per generator with the generator params as argv.[0] and
// metric-file paths in environment variables (CONFORMANCE_METRICS_FILE,
// CONFORMANCE_TEST_CASES, ...). The binary runs a Hegel test and writes one JSON
// metrics object per generated value. See docs/ARCHITECTURE.md §7.
//
// STUB: no conformance generators are implemented yet.

module Hegel.Conformance.Program

[<EntryPoint>]
let main (argv: string[]) : int =
    if argv.Length = 0 then
        eprintfn "hegel-conformance: STUB — usage: hegel-conformance <params-json>"
        0
    else
        eprintfn "hegel-conformance: STUB — received %d arg(s); generators not implemented yet." argv.Length
        0
