namespace Hegel.Cbor

open System

/// Decoding for CBOR tag 91 (WTF-8 strings). The engine wraps string values so
/// lone surrogates survive transport; .NET's UTF-16 `string` can hold them, so we
/// decode the tagged byte string as UTF-8 with surrogate passthrough. Decode-only
/// — never produced by this client.
module internal Wtf8 =

    /// The CBOR semantic tag identifying a WTF-8 string.
    [<Literal>]
    let Tag = 91

    /// Decode WTF-8 bytes into a string, preserving lone surrogates.
    let decode (wtf8: ReadOnlySpan<byte>) : string = failwith "Not implemented"
