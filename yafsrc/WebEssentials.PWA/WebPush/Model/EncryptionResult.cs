using WebEssentials.AspNetCore.Pwa.WebPush.Util;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

// @LogicSoftware
// Originally From: https://github.com/LogicSoftware/WebPushEncryption/blob/master/src/EncryptionResult.cs
public class EncryptionResult
{
    /// <summary>
    /// Gets or sets the public key.
    /// </summary>
    /// <value>
    /// The public key.
    /// </value>
    public byte[] PublicKey { get; set; }

    /// <summary>
    /// Gets or sets the payload.
    /// </summary>
    /// <value>
    /// The payload.
    /// </value>
    public byte[] Payload { get; set; }

    /// <summary>
    /// Gets or sets the salt.
    /// </summary>
    /// <value>
    /// The salt.
    /// </value>
    public byte[] Salt { get; set; }

    /// <summary>
    /// Base64s the encode public key.
    /// </summary>
    /// <returns></returns>
    public string Base64EncodePublicKey()
    {
        return UrlBase64.Encode(this.PublicKey);
    }

    /// <summary>
    /// Base64s the encode salt.
    /// </summary>
    /// <returns></returns>
    public string Base64EncodeSalt()
    {
        return UrlBase64.Encode(this.Salt);
    }
}