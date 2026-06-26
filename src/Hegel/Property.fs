namespace Hegel

/// Entry point for running a Hegel property programmatically (outside a test
/// attribute), e.g. when a setting must be a runtime value.
[<RequireQualifiedAccess>]
module Property =

    /// Run `body` as a property with the given settings.
    let checkWith (settings: Settings) (body: TestCase -> unit) : unit = Runner.run settings body

    /// Run `body` as a property with the default settings.
    let check (body: TestCase -> unit) : unit = checkWith Settings.Default body
