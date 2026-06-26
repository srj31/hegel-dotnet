namespace Hegel.Cbor

open System
open System.Collections.Generic
open System.Formats.Cbor

/// Encodes Hegel schemas (CBOR maps like `{"type":"integer","min_value":…}`) and
/// decodes generated values, using the built-in `CborReader` / `CborWriter`.
/// Integers decode to `System.Numerics.BigInteger` (engine ints can exceed 64
/// bits); strings carrying CBOR tag 91 are decoded via `Wtf8`.
module internal CborCodec =

    /// Decode a CBOR value produced by the engine into a CLR object.
    let decodeValue (cbor: ReadOnlyMemory<byte>) : obj =
        let reader = CborReader(cbor, CborConformanceMode.Lax)
        ignore reader // TODO: full value decode (ints→BigInteger, tag-91 WTF-8, arrays, maps).
        failwith "Not implemented"

    /// Encode a schema attribute set to CBOR bytes for `hegel_generate`.
    let encodeSchema (schema: IReadOnlyDictionary<string, obj>) : byte[] =
        let writer = CborWriter(CborConformanceMode.Lax)
        ignore (writer, schema) // TODO: write the {"type": …, …params} map.
        failwith "Not implemented"
