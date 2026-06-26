namespace Hegel.Xunit

open System
open Xunit
open Hegel

/// Marks an xUnit test method as a Hegel property. The method takes a single
/// `TestCase` parameter; the engine runs it many times and shrinks any failure,
/// surfacing the whole run as one pass/fail node in the test tree.
///
/// Scaffold: this currently extends `FactAttribute` and does not yet drive the
/// Hegel run loop. The real integration pairs it with a test-case runner (the
/// FsCheck model). See `docs/ARCHITECTURE.md` §6.
[<Sealed>]
[<AttributeUsage(AttributeTargets.Method, AllowMultiple = false)>]
type PropertyAttribute() =
    inherit FactAttribute()

    /// Number of test cases to run. Defaults to the engine default (100).
    member val TestCases: int64 = 100L with get, set

    /// Fixed RNG seed; leave unset to let the engine choose.
    member val Seed: int64 = 0L with get, set

    /// Whether `Seed` was explicitly set.
    member val HasSeed: bool = false with get, set

    /// Map this attribute onto a `Settings` instance.
    member internal this.ToSettings() : Settings =
        { Settings.Default with
            TestCases = this.TestCases
            Seed = (if this.HasSeed then Some this.Seed else None) }
