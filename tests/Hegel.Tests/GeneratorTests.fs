/// Scaffold-level tests: confirm the public surface wires together and the
/// project builds. Behavioural generator tests (using the real engine) land with
/// the implementation — see docs/ARCHITECTURE.md §8.
module Hegel.Tests.GeneratorTests

open Xunit
open Hegel

[<Fact>]
let ``Integers exposes fluent bounds`` () =
    let gen = Gen.integers().Min(-5).Max(5) :> IGenerator<int>
    Assert.NotNull gen

[<Fact>]
let ``Settings with-expression derives variant`` () =
    let settings =
        { Settings.Default with
            TestCases = 500L }

    Assert.Equal(500L, settings.TestCases)
