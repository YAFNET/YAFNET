using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Util;

/// <summary>
/// 
/// </summary>
public class HmacSha256
{
    private readonly HMac _hmac;

    /// <summary>
    /// Initializes a new instance of the <see cref="HmacSha256"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    public HmacSha256(byte[] key)
    {
        this._hmac = new HMac(new Sha256Digest());
        this._hmac.Init(new KeyParameter(key));
    }

    /// <summary>
    /// Computes the hash.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public byte[] ComputeHash(byte[] value)
    {
        var resBuf = new byte[this._hmac.GetMacSize()];
        this._hmac.BlockUpdate(value, 0, value.Length);
        this._hmac.DoFinal(resBuf, 0);

        return resBuf;
    }
}