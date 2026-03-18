using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Util;

/// <summary>
/// 
/// </summary>
internal class JwsSigner
{
    /// <summary>
    /// The private key
    /// </summary>
    private readonly ECPrivateKeyParameters _privateKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwsSigner"/> class.
    /// </summary>
    /// <param name="privateKey">The private key.</param>
    public JwsSigner(ECPrivateKeyParameters privateKey)
    {
        this._privateKey = privateKey;
    }

    /// <summary>
    ///     Generates a Jws Signature.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    public string GenerateSignature(Dictionary<string, object> header, Dictionary<string, object> payload)
    {
        var securedInput = SecureInput(header, payload);
        var message = Encoding.UTF8.GetBytes(securedInput);

        var hashedMessage = Sha256Hash(message);

        var signer = new ECDsaSigner();
        signer.Init(true, this._privateKey);
        var results = signer.GenerateSignature(hashedMessage);

        // Concated to create signature
        var a = results[0].ToByteArrayUnsigned();
        var b = results[1].ToByteArrayUnsigned();

        // a,b are required to be exactly the same length of bytes
        if (a.Length != b.Length)
        {
            var largestLength = Math.Max(a.Length, b.Length);
            a = ByteArrayPadLeft(a, largestLength);
            b = ByteArrayPadLeft(b, largestLength);
        }

        var signature = UrlBase64.Encode(a.Concat(b).ToArray());
        return $"{securedInput}.{signature}";
    }

    /// <summary>
    /// Secures the input.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="payload">The payload.</param>
    /// <returns></returns>
    private static string SecureInput(Dictionary<string, object> header, Dictionary<string, object> payload)
    {
        var encodeHeader = UrlBase64.Encode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header)));
        var encodePayload = UrlBase64.Encode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));

        return $"{encodeHeader}.{encodePayload}";
    }

    /// <summary>
    /// Bytes the array pad left.
    /// </summary>
    /// <param name="src">The source.</param>
    /// <param name="size">The size.</param>
    /// <returns></returns>
    private static byte[] ByteArrayPadLeft(byte[] src, int size)
    {
        var dst = new byte[size];
        var startAt = dst.Length - src.Length;
        Array.Copy(src, 0, dst, startAt, src.Length);
        return dst;
    }

    /// <summary>
    /// Sha256s the hash.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    private static byte[] Sha256Hash(byte[] message)
    {
        var sha256Digest = new Sha256Digest();
        sha256Digest.BlockUpdate(message, 0, message.Length);
        var hash = new byte[sha256Digest.GetDigestSize()];
        sha256Digest.DoFinal(hash, 0);
        return hash;
    }
}