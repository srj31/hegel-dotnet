namespace Hegel

open System

/// Generates 32-bit integers within optional inclusive bounds.
[<Sealed>]
type IntegerGenerator private (lower: int, upper: int) =

    /// Create a generator over the full `int` range.
    new() = IntegerGenerator(Int32.MinValue, Int32.MaxValue)

    /// Set the inclusive lower bound.
    member _.Min(min: int) = IntegerGenerator(min, upper)

    /// Set the inclusive upper bound.
    member _.Max(max: int) = IntegerGenerator(lower, max)

    interface IGenerator<int> with
        member _.Draw(testCase: TestCase) : int = failwith "Not implemented"
