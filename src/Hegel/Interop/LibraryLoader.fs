namespace Hegel.Interop

open System
open System.IO
open System.Reflection
open System.Runtime.InteropServices

/// Resolves the native `libhegel` on first use. Resolution order:
///
/// 1. `$HEGEL_LIBHEGEL_PATH` — explicit override (an existing file).
/// 2. The OS loader search path (`LD_LIBRARY_PATH` / `DYLD_LIBRARY_PATH` / `PATH`).
/// 3. The bundled native shipped at `runtimes/&lt;rid&gt;/native/libhegel.&lt;ext&gt;`
///    (resolved automatically by the .NET runtime).
module internal LibraryLoader =

    [<Literal>]
    let private LibraryName = "hegel"

    let private resolve
        (libraryName: string)
        (assembly: Assembly)
        (searchPath: Nullable<DllImportSearchPath>)
        : nativeint =
        if libraryName <> LibraryName then
            IntPtr.Zero
        else
            let overridePath = Environment.GetEnvironmentVariable "HEGEL_LIBHEGEL_PATH"

            if not (String.IsNullOrEmpty overridePath) && File.Exists overridePath then
                NativeLibrary.Load overridePath
            else
                // Fall back to default runtime resolution (OS search path + the
                // bundled runtimes/<rid>/native/ asset). Zero lets the runtime try.
                match NativeLibrary.TryLoad(libraryName, assembly, searchPath) with
                | true, handle -> handle
                | false, _ -> IntPtr.Zero

    /// Registers the import resolver exactly once (the `lazy` makes it idempotent
    /// and thread-safe).
    let private registration =
        lazy
            NativeLibrary.SetDllImportResolver(
                Assembly.GetExecutingAssembly(),
                DllImportResolver(fun name asm path -> resolve name asm path)
            )

    /// Ensure the import resolver is registered before any native call.
    let ensureLoaded () = registration.Force()
