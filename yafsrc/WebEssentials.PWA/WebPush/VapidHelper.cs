using System;
using System.Collections.Generic;

using Org.BouncyCastle.Crypto.Parameters;

using WebEssentials.AspNetCore.Pwa.WebPush.Model;
using WebEssentials.AspNetCore.Pwa.WebPush.Util;

namespace WebEssentials.AspNetCore.Pwa.WebPush;

/// <summary>
/// Vapid helper class
/// </summary>
public static class VapidHelper
{
    /// <summary>
    ///     Generate vapid keys
    /// </summary>
    public static VapidDetails GenerateVapidKeys()
    {
        var results = new VapidDetails();

        var keys = ECKeyHelper.GenerateKeys();
        var publicKey = ((ECPublicKeyParameters)keys.Public).Q.GetEncoded(false);
        var privateKey = ((ECPrivateKeyParameters)keys.Private).D.ToByteArrayUnsigned();

        results.PublicKey = UrlBase64.Encode(publicKey);
        results.PrivateKey = UrlBase64.Encode(ByteArrayPadLeft(privateKey, 32));

        return results;
    }

    /// <summary>
    ///     This method takes the required VAPID parameters and returns the required
    ///     header to be added to a Web Push Protocol Request.
    /// </summary>
    /// <param name="audience">This must be the origin of the push service.</param>
    /// <param name="subject">This should be a URL or a 'mailto:' email address</param>
    /// <param name="publicKey">The VAPID public key as a base64 encoded string</param>
    /// <param name="privateKey">The VAPID private key as a base64 encoded string</param>
    /// <param name="expiration">The expiration of the VAPID JWT.</param>
    /// <returns>A dictionary of header key/value pairs.</returns>
    public static Dictionary<string, string> GetVapidHeaders(string audience, string subject, string publicKey,
        string privateKey, long expiration = -1)
    {
        ValidateAudience(audience);
        ValidateSubject(subject);
        ValidatePublicKey(publicKey);
        ValidatePrivateKey(privateKey);

        var decodedPrivateKey = UrlBase64.Decode(privateKey);

        if (expiration == -1)
        {
            expiration = UnixTimeNow() + 43200;
        }
        else
        {
            ValidateExpiration(expiration);
        }


        var header = new Dictionary<string, object> { { "typ", "JWT" }, { "alg", "ES256" } };

        var jwtPayload = new Dictionary<string, object> { { "aud", audience }, { "exp", expiration }, { "sub", subject } };

        var signingKey = ECKeyHelper.GetPrivateKey(decodedPrivateKey);

        var signer = new JwsSigner(signingKey);
        var token = signer.GenerateSignature(header, jwtPayload);

        var results = new Dictionary<string, string>
        {
            {"Authorization", "WebPush " + token}, {"Crypto-Key", "p256ecdsa=" + publicKey}
        };

        return results;
    }

    /// <summary>
    /// Validates the audience.
    /// </summary>
    /// <param name="audience">The audience.</param>
    /// <exception cref="ArgumentException">
    /// No audience could be generated for VAPID.
    /// or
    /// The audience value must be a string containing the origin of a push service. " + audience
    /// or
    /// VAPID audience is not a url.
    /// </exception>
    public static void ValidateAudience(string audience)
    {
        if (string.IsNullOrEmpty(audience))
        {
            throw new ArgumentException("No audience could be generated for VAPID.");
        }

        if (audience.Length == 0)
        {
            throw new ArgumentException(
                "The audience value must be a string containing the origin of a push service. " + audience);
        }

        if (!Uri.IsWellFormedUriString(audience, UriKind.Absolute))
        {
            throw new ArgumentException("VAPID audience is not a url.");
        }
    }

    /// <summary>
    /// Validates the subject.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <exception cref="ArgumentException">
    /// A subject is required
    /// or
    /// The subject value must be a string containing a url or mailto: address.
    /// or
    /// Subject is not a valid URL or mailto address
    /// </exception>
    public static void ValidateSubject(string subject)
    {
        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentException("A subject is required");
        }

        if (subject.Length == 0)
        {
            throw new ArgumentException("The subject value must be a string containing a url or mailto: address.");
        }

        if (!subject.StartsWith("mailto:") && !Uri.IsWellFormedUriString(subject, UriKind.Absolute))
        {
            throw new ArgumentException("Subject is not a valid URL or mailto address");
        }
    }

    /// <summary>
    /// Validates the public key.
    /// </summary>
    /// <param name="publicKey">The public key.</param>
    /// <exception cref="ArgumentException">
    /// Valid public key not set
    /// or
    /// Vapid public key must be 65 characters long when decoded
    /// </exception>
    public static void ValidatePublicKey(string publicKey)
    {
        if (string.IsNullOrEmpty(publicKey))
        {
            throw new ArgumentException("Valid public key not set");
        }

        var decodedPublicKey = UrlBase64.Decode(publicKey);

        if (decodedPublicKey.Length != 65)
        {
            throw new ArgumentException("Vapid public key must be 65 characters long when decoded");
        }
    }

    /// <summary>
    /// Validates the private key.
    /// </summary>
    /// <param name="privateKey">The private key.</param>
    /// <exception cref="ArgumentException">
    /// Valid private key not set
    /// or
    /// Vapid private key should be 32 bytes long when decoded.
    /// </exception>
    public static void ValidatePrivateKey(string privateKey)
    {
        if (string.IsNullOrEmpty(privateKey))
        {
            throw new ArgumentException("Valid private key not set");
        }

        var decodedPrivateKey = UrlBase64.Decode(privateKey);

        if (decodedPrivateKey.Length != 32)
        {
            throw new ArgumentException("Vapid private key should be 32 bytes long when decoded.");
        }
    }

    /// <summary>
    /// Validates the expiration.
    /// </summary>
    /// <param name="expiration">The expiration.</param>
    /// <exception cref="ArgumentException">Vapid expiration must be a unix timestamp in the future</exception>
    private static void ValidateExpiration(long expiration)
    {
        if (expiration <= UnixTimeNow())
        {
            throw new ArgumentException("Vapid expiration must be a unix timestamp in the future");
        }
    }

    /// <summary>
    /// Unixes the time now.
    /// </summary>
    /// <returns></returns>
    private static long UnixTimeNow()
    {
        var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
        return (long)timeSpan.TotalSeconds;
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
}