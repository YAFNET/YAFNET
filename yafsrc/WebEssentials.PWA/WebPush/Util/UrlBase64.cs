using System;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Util;

static internal class UrlBase64
{
    /// <summary>
    ///     Decodes a url-safe base64 string into bytes
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static byte[] Decode(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');

        while (base64.Length % 4 != 0)
        {
            base64 += "=";
        }

        return Convert.FromBase64String(base64);
    }

    /// <summary>
    ///     Encodes bytes into url-safe base64 string
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Encode(byte[] data)
    {
        return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}