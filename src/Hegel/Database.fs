namespace Hegel

/// Tri-state configuration for the example database: *unset* (engine default),
/// *disabled*, or a *path* to a directory. The engine persists minimal
/// counterexamples here and replays them in `Phase.Reuse`.
[<RequireQualifiedAccess>]
type Database =
    /// Leave the engine default (a local `.hegel` directory, disabled in CI).
    | Unset
    /// Disable example persistence entirely.
    | Disabled
    /// Persist examples under the given directory.
    | Path of directory: string
