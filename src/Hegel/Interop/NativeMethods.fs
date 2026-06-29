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

    // Every entry point except `hegel_context_new` takes a `hegel_context_t*` first
    // and returns `hegel_result_t` (`Abi.Result`); `hegel_context_new` never returns NULL.
    [<DllImport("libhegel", EntryPoint = "hegel_context_new", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint ContextNew()

    [<DllImport("libhegel", EntryPoint = "hegel_context_free", CallingConvention = CallingConvention.Cdecl)>]
    extern Abi.Result ContextFree(nativeint ctx)

    /// `outVersion` receives an engine-owned `char*`, marshalled as `nativeint` (not
    /// `string`) so the runtime never frees it; `Engine.Version` copies it out.
    [<DllImport("libhegel", EntryPoint = "hegel_version", CallingConvention = CallingConvention.Cdecl)>]
    extern Abi.Result Version(nativeint ctx, nativeint& outVersion)

    // NOTE: the declarations below are still the pre-context ABI (no ctx arg, wrong
    // return types). Unused until M1, where they'll be corrected. See ARCHITECTURE §3.
    [<DllImport("libhegel", EntryPoint = "hegel_settings_new", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint SettingsNew()

    [<DllImport("libhegel", EntryPoint = "hegel_settings_free", CallingConvention = CallingConvention.Cdecl)>]
    extern void SettingsFree(nativeint settings)

    [<DllImport("libhegel", EntryPoint = "hegel_run_start", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint RunStart(nativeint settings)

    [<DllImport("libhegel", EntryPoint = "hegel_run_free", CallingConvention = CallingConvention.Cdecl)>]
    extern void RunFree(nativeint run)

    [<DllImport("libhegel", EntryPoint = "hegel_next_test_case", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint NextTestCase(nativeint run)

    [<DllImport("libhegel", EntryPoint = "hegel_run_result", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint RunResult(nativeint run)

    [<DllImport("libhegel", EntryPoint = "hegel_last_error_message", CallingConvention = CallingConvention.Cdecl)>]
    extern nativeint LastErrorMessage()
