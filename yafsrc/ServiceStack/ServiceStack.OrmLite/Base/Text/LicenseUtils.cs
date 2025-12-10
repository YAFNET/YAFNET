// ***********************************************************************
// <copyright file="LicenseUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class LicenseException.
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
public class LicenseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LicenseException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public LicenseException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Enum LicenseType
/// </summary>
public enum LicenseType
{
    /// <summary>
    /// The free
    /// </summary>
    Free,

    /// <summary>
    /// The free individual/
    /// </summary>
    FreeIndividual,

    /// <summary>
    /// The free open source
    /// </summary>
    FreeOpenSource,

    /// <summary>
    /// The indie
    /// </summary>
    Indie,

    /// <summary>
    /// The business
    /// </summary>
    Business,

    /// <summary>
    /// The enterprise
    /// </summary>
    Enterprise,

    /// <summary>
    /// The text indie
    /// </summary>
    TextIndie,

    /// <summary>
    /// The text business
    /// </summary>
    TextBusiness,

    /// <summary>
    /// The ormlite indie
    /// </summary>
    OrmLiteIndie,

    /// <summary>
    /// The ormlite business
    /// </summary>
    OrmLiteBusiness,

    /// <summary>
    /// The redis indie
    /// </summary>
    RedisIndie,

    /// <summary>
    /// The redis business
    /// </summary>
    RedisBusiness,

    /// <summary>
    /// The aws indie
    /// </summary>
    AwsIndie,

    /// <summary>
    /// The aws business/
    /// </summary>
    AwsBusiness,

    /// <summary>
    /// The trial
    /// </summary>
    Trial,

    /// <summary>
    /// The site
    /// </summary>
    Site,

    /// <summary>
    /// The text site
    /// </summary>
    TextSite,

    /// <summary>
    /// The redis site
    /// </summary>
    RedisSite,

    /// <summary>
    /// The ormlite site
    /// </summary>
    OrmLiteSite
}

/// <summary>
/// Class LicenseKey.
/// </summary>
public class LicenseKey
{
    /// <summary>
    /// Gets or sets the reference.
    /// </summary>
    /// <value>The reference.</value>
    public string Ref { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public LicenseType Type { get; set; }

    /// <summary>
    /// Gets or sets the hash.
    /// </summary>
    /// <value>The hash.</value>
    public string Hash { get; set; }

    /// <summary>
    /// Gets or sets the expiry.
    /// </summary>
    /// <value>The expiry.</value>
    public DateTime Expiry { get; set; }
}

/// <summary>
/// Internal Utilities to verify licensing
/// </summary>
public static partial class LicenseUtils
{
    /// <summary>
    /// The runtime public key
    /// </summary>
    public const string RuntimePublicKey =
        "<RSAKeyValue><Modulus>nkqwkUAcuIlVzzOPENcQ+g5ALCe4LyzzWv59E4a7LuOM1Nb+hlNlnx2oBinIkvh09EyaxIX2PmaY0KtyDRIh+PoItkKeJe/TydIbK/bLa0+0Axuwa0MFShE6HdJo/dynpODm64+Sg1XfhICyfsBBSxuJMiVKjlMDIxu9kDg7vEs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    /// <summary>
    /// The license public key
    /// </summary>
    public const string LicensePublicKey =
        "<RSAKeyValue><Modulus>w2fTTfr2SrGCclwLUkrbH0XsIUpZDJ1Kei2YUwYGmIn5AUyCPLTUv3obDBUBFJKLQ61Khs7dDkXlzuJr5tkGQ0zS0PYsmBPAtszuTum+FAYRH4Wdhmlfqu1Z03gkCIo1i11TmamN5432uswwFCVH60JU3CpaN97Ehru39LA1X9E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    private const string ContactDetails =
        " Please see servicestack.net or contact team@servicestack.net for more details.";

    private readonly static Lock LockObj = new();

    static LicenseUtils()
    {
        const string ossLicenseKey =
            "1001-e1JlZjoxMDAxLE5hbWU6VGVzdCBCdXNpbmVzcyxUeXBlOkJ1c2luZXNzLEhhc2g6UHVNTVRPclhvT2ZIbjQ5MG5LZE1mUTd5RUMzQnBucTFEbTE3TDczVEF4QUNMT1FhNXJMOWkzVjFGL2ZkVTE3Q2pDNENqTkQyUktRWmhvUVBhYTBiekJGUUZ3ZE5aZHFDYm9hL3lydGlwUHI5K1JsaTBYbzNsUC85cjVJNHE5QVhldDN6QkE4aTlvdldrdTgyTk1relY2eis2dFFqTThYN2lmc0JveHgycFdjPSxFeHBpcnk6MjAxMy0wMS0wMX0=";

        RegisterLicense(ossLicenseKey);
    }


    private readonly static int[] RevokedSubs = [4018, 4019, 4041, 4331, 4581];

    /// <summary>
    /// Class __ActivatedLicense.
    /// </summary>
    private class ActivatedLicense
    {
        static internal ActivatedLicense Get { get; private set; }

        static internal void SetActivatedLicense(ActivatedLicense licence)
        {
            Get = licence;
        }
    }

    /// <summary>
    /// Registers the license.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    public static void RegisterLicense(string licenseKeyText)
    {
        JsConfig.InitStatics();

        if (ActivatedLicense.Get != null) //Skip multiple license registrations. Use RemoveLicense() to reset.
        {
            return;
        }

        var hold = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            licenseKeyText = licenseKeyText?.Trim();
            if (string.IsNullOrEmpty(licenseKeyText))
            {
                throw new ArgumentNullException(nameof(licenseKeyText));
            }

            var subId = licenseKeyText.LeftPart('-');
            if (!int.TryParse(subId, out var subIdInt))
            {
                if (!licenseKeyText.StartsWith("TRIAL"))
                {
                    throw new LicenseException($"This license is invalid.{ContactDetails}");
                }
            }

            if (RevokedSubs.Contains(subIdInt))
            {
                throw new LicenseException($"This subscription has been revoked. {ContactDetails}");
            }

            var key = VerifyLicenseKeyText(licenseKeyText);
            ValidateLicenseKey(key);
        }
        catch (PlatformNotSupportedException)
        {
            // Allow usage in environments like dotnet script
            ActivatedLicense.SetActivatedLicense(
                new ActivatedLicense());
        }
        catch (Exception ex)
        {
            //bubble unrelated project Exceptions
            switch (ex)
            {
                case FileNotFoundException or FileLoadException or BadImageFormatException or NotSupportedException
                    or System.Net.Http.HttpRequestException
                    or WebException or TaskCanceledException or LicenseException:
                    throw;
            }

            var msg = ex.Message;

            lock (LockObj)
            {
                try
                {
                    var key = PclExport.Instance.VerifyLicenseKeyTextFallback(licenseKeyText);
                    ValidateLicenseKey(key);
                }
                catch (Exception exFallback)
                {
                    Tracer.Instance.WriteWarning(ex.ToString());

                    if (exFallback is FileNotFoundException or FileLoadException or BadImageFormatException)
                    {
                        throw;
                    }

                    throw new LicenseException(msg, exFallback).Trace();
                }
            }

            throw new LicenseException(msg, ex).Trace();
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = hold;
        }

        return;

        static void ValidateLicenseKey(LicenseKey key)
        {
            var releaseDate = Env.GetReleaseDate();
            if (releaseDate > key.Expiry)
            {
                throw new LicenseException(
                    $"This license has expired on {key.Expiry:d} and is not valid for use with this release.{ContactDetails}").Trace();
            }

            if (key.Type == LicenseType.Trial && DateTime.UtcNow > key.Expiry)
            {
                throw new LicenseException($"This trial license has expired on {key.Expiry:d}.{ContactDetails}")
                    .Trace();
            }

            ActivatedLicense.SetActivatedLicense(new ActivatedLicense());
        }
    }

    /// <summary>
    /// Asserts the valid usage.
    /// </summary>
    public static void AssertValidUsage()
    {
    }

    /// <param name="licenseKeyText">The license key text.</param>
    extension(string licenseKeyText)
    {
        /// <summary>
        /// Converts to license key.
        /// </summary>
        /// <returns>ServiceStack.OrmLite.Base.Text.LicenseKey.</returns>
        public LicenseKey ToLicenseKey()
        {
            licenseKeyText = LicenseKeyRegex().Replace(licenseKeyText, "");
            var parts = licenseKeyText.SplitOnFirst('-');
            var refId = parts[0];
            var base64 = parts[1];
            var jsv = Convert.FromBase64String(base64).FromUtf8Bytes();

            var hold = JsConfig<DateTime>.DeSerializeFn;
            var holdRaw = JsConfig<DateTime>.RawDeserializeFn;

            try
            {
                JsConfig<DateTime>.DeSerializeFn = null;
                JsConfig<DateTime>.RawDeserializeFn = null;

                var key = jsv.FromJsv<LicenseKey>();

                if (key.Ref != refId)
                {
                    throw new LicenseException("The license '{0}' is not assigned to CustomerId '{1}'.".Fmt(base64, refId))
                        .Trace();
                }

                return key;
            }
            finally
            {
                JsConfig<DateTime>.DeSerializeFn = hold;
                JsConfig<DateTime>.RawDeserializeFn = holdRaw;
            }
        }

        /// <summary>
        /// Converts to license key fallback.
        /// </summary>
        /// <returns>ServiceStack.OrmLite.Base.Text.LicenseKey.</returns>
        public LicenseKey ToLicenseKeyFallback()
        {
            licenseKeyText = LicenseKeyRegex().Replace(licenseKeyText, "");
            var parts = licenseKeyText.SplitOnFirst('-');
            var refId = parts[0];
            var base64 = parts[1];
            var jsv = Convert.FromBase64String(base64).FromUtf8Bytes();

            var map = jsv.FromJsv<Dictionary<string, string>>();
            var key = new LicenseKey
            {
                Ref = map.Get("Ref"),
                Name = map.Get("Name"),
                Type = Enum.Parse<LicenseType>(map.Get("Type"), ignoreCase: true),
                Hash = map.Get("Hash"),
                Expiry = DateTimeSerializer.ParseManual(map.Get("Expiry"), DateTimeKind.Utc).GetValueOrDefault()
            };

            if (key.Ref != refId)
            {
                throw new LicenseException($"The license '{base64}' is not assigned to CustomerId '{refId}'.").Trace();
            }

            return key;
        }
    }

    /// <summary>
    /// Verifies the license key text.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    /// <returns>ServiceStack.OrmLite.Base.Text.LicenseKey.</returns>
    public static LicenseKey VerifyLicenseKeyText(string licenseKeyText)
    {
        return licenseKeyText.ToLicenseKey();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex LicenseKeyRegex();
}