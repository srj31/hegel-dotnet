namespace Hegel

open System
open System.Runtime.InteropServices
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
    member _.Version() : string =
        // `hegel_version` writes an engine-owned `char*` into `outVersion`; copy it
        // into a managed string and always free the context.
        let ctx = NativeMethods.ContextNew()

        try
            let mutable outVersion = IntPtr.Zero
            let rc = NativeMethods.Version(ctx, &outVersion)

            if rc <> Abi.Result.Ok then
                invalidOp $"hegel_version failed with result code {rc}"
            elif outVersion = IntPtr.Zero then
                invalidOp "hegel_version returned a null version pointer"
            else
                Marshal.PtrToStringUTF8 outVersion
        finally
            NativeMethods.ContextFree ctx |> ignore
