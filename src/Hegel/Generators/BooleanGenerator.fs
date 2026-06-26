namespace Hegel

/// Generates `bool` values.
[<Sealed>]
type BooleanGenerator() =
    interface IGenerator<bool> with
        member _.Draw(testCase: TestCase) : bool = failwith "Not implemented"
