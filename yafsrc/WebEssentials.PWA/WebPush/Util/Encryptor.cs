using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

using WebEssentials.AspNetCore.Pwa.WebPush.Model;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Util;

/// <summary>
/// @LogicSoftware
/// Originally from https://github.com/LogicSoftware/WebPushEncryption/blob/master/src/Encryptor.cs
/// </summary>
static internal class Encryptor
{
    /// <summary>
    /// Encrypts the specified user key.
    /// </summary>
    /// <param name="userKey">The user key.</param>
    /// <param name="userSecret">The user secret.</param>
    /// <param name="payload">The payload.</param>
    /// <returns></returns>
    public static EncryptionResult Encrypt(string userKey, string userSecret, string payload)
    {
        var userKeyBytes = UrlBase64.Decode(userKey);
        var userSecretBytes = UrlBase64.Decode(userSecret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        return Encrypt(userKeyBytes, userSecretBytes, payloadBytes);
    }

    /// <summary>
    /// Encrypts the specified user key.
    /// </summary>
    /// <param name="userKey">The user key.</param>
    /// <param name="userSecret">The user secret.</param>
    /// <param name="payload">The payload.</param>
    /// <returns></returns>
    public static EncryptionResult Encrypt(byte[] userKey, byte[] userSecret, byte[] payload)
    {
        var salt = GenerateSalt(16);
        var serverKeyPair = ECKeyHelper.GenerateKeys();

        var ecdhAgreement = AgreementUtilities.GetBasicAgreement("ECDH");
        ecdhAgreement.Init(serverKeyPair.Private);

        var userPublicKey = ECKeyHelper.GetPublicKey(userKey);

        var key = ecdhAgreement.CalculateAgreement(userPublicKey).ToByteArrayUnsigned();
        var serverPublicKey = ((ECPublicKeyParameters)serverKeyPair.Public).Q.GetEncoded(false);

        var prk = HKDF(userSecret, key, "Content-Encoding: auth\0"u8.ToArray(), 32);
        var cek = HKDF(salt, prk, CreateInfoChunk("aesgcm", userKey, serverPublicKey), 16);
        var nonce = HKDF(salt, prk, CreateInfoChunk("nonce", userKey, serverPublicKey), 12);

        var input = AddPaddingToInput(payload);
        var encryptedMessage = EncryptAes(nonce, cek, input);

        return new EncryptionResult
        {
            Salt = salt,
            Payload = encryptedMessage,
            PublicKey = serverPublicKey
        };
    }

    /// <summary>
    /// Generates the salt.
    /// </summary>
    /// <param name="length">The length.</param>
    /// <returns></returns>
    private static byte[] GenerateSalt(int length)
    {
        var salt = new byte[length];
        var random = new SecureRandom();
        random.NextBytes(salt);
        return salt;
    }

    /// <summary>
    /// Adds the padding to input.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    private static byte[] AddPaddingToInput(byte[] data)
    {
        var input = new byte[0 + 2 + data.Length];
        Buffer.BlockCopy(ConvertInt(0), 0, input, 0, 2);
        Buffer.BlockCopy(data, 0, input, 0 + 2, data.Length);
        return input;
    }

    /// <summary>
    /// Encrypts the aes.
    /// </summary>
    /// <param name="nonce">The nonce.</param>
    /// <param name="cek">The cek.</param>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    private static byte[] EncryptAes(byte[] nonce, byte[] cek, byte[] message)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(cek), 128, nonce);
        cipher.Init(true, parameters);

        //Generate Cipher Text With Auth Tag
        var cipherText = new byte[cipher.GetOutputSize(message.Length)];
        var len = cipher.ProcessBytes(message, 0, message.Length, cipherText, 0);
        cipher.DoFinal(cipherText, len);

        return cipherText;
    }

    /// <summary>
    /// HKDFs the second step.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="info">The information.</param>
    /// <param name="length">The length.</param>
    /// <returns></returns>
    public static byte[] HKDFSecondStep(byte[] key, byte[] info, int length)
    {
        var hmac = new HmacSha256(key);
        var infoAndOne = info.Concat(new byte[] { 0x01 }).ToArray();
        var result = hmac.ComputeHash(infoAndOne);

        if (result.Length > length)
        {
            Array.Resize(ref result, length);
        }

        return result;
    }

    /// <summary>
    /// HKDFs the specified salt.
    /// </summary>
    /// <param name="salt">The salt.</param>
    /// <param name="prk">The PRK.</param>
    /// <param name="info">The information.</param>
    /// <param name="length">The length.</param>
    /// <returns></returns>
    public static byte[] HKDF(byte[] salt, byte[] prk, byte[] info, int length)
    {
        var hmac = new HmacSha256(salt);
        var key = hmac.ComputeHash(prk);

        return HKDFSecondStep(key, info, length);
    }

    /// <summary>
    /// Converts the int.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns></returns>
    public static byte[] ConvertInt(int number)
    {
        var output = BitConverter.GetBytes(Convert.ToUInt16(number));
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(output);
        }

        return output;
    }

    /// <summary>
    /// Creates the information chunk.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="recipientPublicKey">The recipient public key.</param>
    /// <param name="senderPublicKey">The sender public key.</param>
    /// <returns></returns>
    public static byte[] CreateInfoChunk(string type, byte[] recipientPublicKey, byte[] senderPublicKey)
    {
        var output = new List<byte>();
        output.AddRange(Encoding.UTF8.GetBytes($"Content-Encoding: {type}\0P-256\0"));
        output.AddRange(ConvertInt(recipientPublicKey.Length));
        output.AddRange(recipientPublicKey);
        output.AddRange(ConvertInt(senderPublicKey.Length));
        output.AddRange(senderPublicKey);
        return output.ToArray();
    }
}