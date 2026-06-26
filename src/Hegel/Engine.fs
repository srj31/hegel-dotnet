namespace Hegel

open Hegel.Interop

/// Process-wide handle to the loaded native engine. Lazily loads `libhegel` on
/// first use; native run/settings handles are created per run, so no state leaks
/// between tests.
[<Sealed>]
type Engine private () =
    do LibraryLoader.ensureLoaded ()

    static let shared = lazy (Engine())

    /// The shared engine instance.
    static member Shared = shared.Value

    /// The engine version reported by the native library.
    member _.Version() : string = failwith "Not implemented"
