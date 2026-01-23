using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Util;

public class HmacSha256
{
    private readonly HMac _hmac;

    public HmacSha256(byte[] key)
    {
        this._hmac = new HMac(new Sha256Digest());
        this._hmac.Init(new KeyParameter(key));
    }

    public byte[] ComputeHash(byte[] value)
    {
        var resBuf = new byte[this._hmac.GetMacSize()];
        this._hmac.BlockUpdate(value, 0, value.Length);
        this._hmac.DoFinal(resBuf, 0);

        return resBuf;
    }
}