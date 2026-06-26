namespace Hegel.Interop

open System.Runtime.InteropServices

/// P/Invoke surface for libhegel, declared with F# `extern` / `[<DllImport>]`.
/// Handles are opaque pointers (`nativeint`); strings returned by the engine are
/// `const char*` and must be copied immediately.
///
/// F# has no `[LibraryImport]` source generator (that is a C#-only feature), so
/// the binding uses classic `[<DllImport>]` P/Invoke with the cdecl calling
/// convention that hegel-rust's C ABI exposes.
///
/// This is a representative subset; the full ABI (settings setters, generate,
/// spans, collections, mark_complete, run results, failures) is tracked in
/// `docs/ARCHITECTURE.md` §3 and filled in alongside the run loop.
module internal NativeMethods =

    [<DllImport("hegel", EntryPoint = "hegel_version", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint Version()

    [<DllImport("hegel", EntryPoint = "hegel_settings_new", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint SettingsNew()

    [<DllImport("hegel", EntryPoint = "hegel_settings_free", CallingConvention = CallingConvention.Cdecl)>]
    extern void SettingsFree(nativeint settings)

    [<DllImport("hegel", EntryPoint = "hegel_run_start", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint RunStart(nativeint settings)

    [<DllImport("hegel", EntryPoint = "hegel_run_free", CallingConvention = CallingConvention.Cdecl)>]
    extern void RunFree(nativeint run)

    [<DllImport("hegel", EntryPoint = "hegel_next_test_case", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint NextTestCase(nativeint run)

    [<DllImport("hegel", EntryPoint = "hegel_run_result", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint RunResult(nativeint run)

    [<DllImport("hegel", EntryPoint = "hegel_last_error_message", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint LastErrorMessage()
