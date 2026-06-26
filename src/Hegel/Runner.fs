namespace Hegel

open System

/// Drives a single property run: builds native settings from `Settings`, loops
/// `hegel_next_test_case`, invokes the body per case, maps outcomes to statuses,
/// and rethrows the minimal counterexample on failure. See `docs/ARCHITECTURE.md` §4.
module internal Runner =

    let run (settings: Settings) (body: TestCase -> unit) : unit =
        ArgumentNullException.ThrowIfNull settings
        ArgumentNullException.ThrowIfNull body

        // Ensure the native engine is loaded before any run.
        Engine.Shared |> ignore

        failwith "Not implemented"
