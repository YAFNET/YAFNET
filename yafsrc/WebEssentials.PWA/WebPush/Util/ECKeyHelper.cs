using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Util;

static internal class ECKeyHelper
{
    public static ECPrivateKeyParameters GetPrivateKey(byte[] privateKey)
    {
        Asn1Object version = new DerInteger(1);
        Asn1Object derEncodedKey = new DerOctetString(privateKey);
        Asn1Object keyTypeParameters = new DerTaggedObject(0, new DerObjectIdentifier("1.2.840.10045.3.1.7"));

        Asn1Object derSequence = new DerSequence(version, derEncodedKey, keyTypeParameters);

        var base64EncodedDerSequence = Convert.ToBase64String(derSequence.GetDerEncoded());
        var pemKey = "-----BEGIN EC PRIVATE KEY-----\n";
        pemKey += base64EncodedDerSequence;
        pemKey += "\n-----END EC PRIVATE KEY----";

        var reader = new StringReader(pemKey);
        var pemReader = new PemReader(reader);
        var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();

        return (ECPrivateKeyParameters)keyPair.Private;
    }

    public static ECPublicKeyParameters GetPublicKey(byte[] publicKey)
    {
        Asn1Object keyTypeParameters = new DerSequence(new DerObjectIdentifier("1.2.840.10045.2.1"),
            new DerObjectIdentifier("1.2.840.10045.3.1.7"));
        Asn1Object derEncodedKey = new DerBitString(publicKey);

        Asn1Object derSequence = new DerSequence(keyTypeParameters, derEncodedKey);

        var base64EncodedDerSequence = Convert.ToBase64String(derSequence.GetDerEncoded());
        var pemKey = "-----BEGIN PUBLIC KEY-----\n";
        pemKey += base64EncodedDerSequence;
        pemKey += "\n-----END PUBLIC KEY-----";

        var reader = new StringReader(pemKey);
        var pemReader = new PemReader(reader);
        var keyPair = pemReader.ReadObject();
        return (ECPublicKeyParameters)keyPair;
    }

    public static AsymmetricCipherKeyPair GenerateKeys()
    {
        var ecParameters = NistNamedCurves.GetByName("P-256");
        var ecSpec = new ECDomainParameters(ecParameters.Curve, ecParameters.G, ecParameters.N, ecParameters.H,
            ecParameters.GetSeed());
        var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDH");
        keyPairGenerator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));

        return keyPairGenerator.GenerateKeyPair();
    }
}