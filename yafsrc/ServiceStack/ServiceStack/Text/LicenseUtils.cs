// ***********************************************************************
// <copyright file="LicenseUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using ServiceStack.Text;
using ServiceStack.Text.Common;

/// <summary>
/// Class LicenseException.
/// Implements the <see cref="System.Exception" />
/// </summary>
/// <seealso cref="System.Exception" />
public class LicenseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LicenseException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseException" /> class.
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
    /// <summary>The free individual</summary>
    FreeIndividual,
    /// <summary>The free open source</summary>
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
    /// The orm lite indie
    /// </summary>
    OrmLiteIndie,
    /// <summary>
    /// The orm lite business
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
    /// The aws business
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
    /// The orm lite site
    /// </summary>
    OrmLiteSite,
}

/// <summary>
/// Enum LicenseFeature
/// </summary>
[Flags]
public enum LicenseFeature : long
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0,
    /// <summary>
    /// All
    /// </summary>
    All = Premium | Text | Client | Common | Redis | OrmLite | ServiceStack | Server | Razor | Admin | Aws,
    /// <summary>
    /// The redis sku
    /// </summary>
    RedisSku = Redis | Text,
    /// <summary>
    /// The orm lite sku
    /// </summary>
    OrmLiteSku = OrmLite | Text,
    /// <summary>
    /// The aws sku
    /// </summary>
    AwsSku = Aws | Text,
    /// <summary>
    /// The free
    /// </summary>
    Free = None,
    /// <summary>
    /// The premium
    /// </summary>
    Premium = 1 << 0,
    /// <summary>
    /// The text
    /// </summary>
    Text = 1 << 1,
    /// <summary>
    /// The client
    /// </summary>
    Client = 1 << 2,
    /// <summary>
    /// The common
    /// </summary>
    Common = 1 << 3,
    /// <summary>
    /// The redis
    /// </summary>
    Redis = 1 << 4,
    /// <summary>
    /// The orm lite
    /// </summary>
    OrmLite = 1 << 5,
    /// <summary>
    /// The service stack
    /// </summary>
    ServiceStack = 1 << 6,
    /// <summary>
    /// The server
    /// </summary>
    Server = 1 << 7,
    /// <summary>
    /// The razor
    /// </summary>
    Razor = 1 << 8,
    /// <summary>
    /// The admin
    /// </summary>
    Admin = 1 << 9,
    /// <summary>
    /// The aws
    /// </summary>
    Aws = 1 << 10,
}

/// <summary>
/// Enum LicenseMeta
/// </summary>
[Flags]
public enum LicenseMeta : long
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0,
    /// <summary>
    /// The subscription
    /// </summary>
    Subscription = 1 << 0,
    /// <summary>
    /// The cores
    /// </summary>
    Cores = 1 << 1,
}

/// <summary>
/// Enum QuotaType
/// </summary>
public enum QuotaType
{
    /// <summary>
    /// The operations
    /// </summary>
    Operations,      //ServiceStack
    /// <summary>
    /// The types
    /// </summary>
    Types,           //Text, Redis
    /// <summary>
    /// The fields
    /// </summary>
    Fields,          //ServiceStack, Text, Redis, OrmLite
    /// <summary>
    /// The requests per hour
    /// </summary>
    RequestsPerHour, //Redis
    /// <summary>
    /// The tables
    /// </summary>
    Tables,          //OrmLite, Aws
    /// <summary>
    /// The premium feature
    /// </summary>
    PremiumFeature,  //AdminUI, Advanced Redis APIs, etc
}

/// <summary>
/// Public Code API to register commercial license for ServiceStack.
/// </summary>
public static class Licensing
{
    /// <summary>
    /// Registers the license.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    public static void RegisterLicense(string licenseKeyText)
    {
        LicenseUtils.RegisterLicense(licenseKeyText);
    }
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
    /// Gets or sets the meta.
    /// </summary>
    /// <value>The meta.</value>
    public long Meta { get; set; }
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
public static class LicenseUtils
{
    /// <summary>
    /// The runtime public key
    /// </summary>
    public const string RuntimePublicKey = "<RSAKeyValue><Modulus>nkqwkUAcuIlVzzOPENcQ+g5ALCe4LyzzWv59E4a7LuOM1Nb+hlNlnx2oBinIkvh09EyaxIX2PmaY0KtyDRIh+PoItkKeJe/TydIbK/bLa0+0Axuwa0MFShE6HdJo/dynpODm64+Sg1XfhICyfsBBSxuJMiVKjlMDIxu9kDg7vEs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    /// <summary>
    /// The license public key
    /// </summary>
    public const string LicensePublicKey = "<RSAKeyValue><Modulus>w2fTTfr2SrGCclwLUkrbH0XsIUpZDJ1Kei2YUwYGmIn5AUyCPLTUv3obDBUBFJKLQ61Khs7dDkXlzuJr5tkGQ0zS0PYsmBPAtszuTum+FAYRH4Wdhmlfqu1Z03gkCIo1i11TmamN5432uswwFCVH60JU3CpaN97Ehru39LA1X9E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    /// <summary>
    /// The contact details
    /// </summary>
    private const string ContactDetails = " Please see servicestack.net or contact team@servicestack.net for more details.";

    /// <summary>
    /// Initializes static members of the <see cref="LicenseUtils"/> class.
    /// </summary>
    static LicenseUtils()
    {
        const string ossLicenseKey = "1001-e1JlZjoxMDAxLE5hbWU6VGVzdCBCdXNpbmVzcyxUeXBlOkJ1c2luZXNzLEhhc2g6UHVNTVRPclhvT2ZIbjQ5MG5LZE1mUTd5RUMzQnBucTFEbTE3TDczVEF4QUNMT1FhNXJMOWkzVjFGL2ZkVTE3Q2pDNENqTkQyUktRWmhvUVBhYTBiekJGUUZ3ZE5aZHFDYm9hL3lydGlwUHI5K1JsaTBYbzNsUC85cjVJNHE5QVhldDN6QkE4aTlvdldrdTgyTk1relY2eis2dFFqTThYN2lmc0JveHgycFdjPSxFeHBpcnk6MjAxMy0wMS0wMX0=";

        RegisterLicense(ossLicenseKey);
    }

    /// <summary>
    /// Gets a value indicating whether this instance has initialize.
    /// </summary>
    /// <value><c>true</c> if this instance has initialize; otherwise, <c>false</c>.</value>
    public static bool HasInit { get; private set; }
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public static void Init()
    {
        HasInit = true; //Dummy method to init static constructor
    }

    /// <summary>
    /// Class ErrorMessages.
    /// </summary>
    public static class ErrorMessages
    {
        /// <summary>
        /// The upgrade instructions
        /// </summary>
        private const string UpgradeInstructions = " Please see https://servicestack.net to upgrade to a commercial license or visit https://github.com/ServiceStackV3/ServiceStackV3 to revert back to the free ServiceStack v3.";
        /// <summary>
        /// The exceeded redis types
        /// </summary>
        internal const string ExceededRedisTypes = "The free-quota limit on '{0} Redis Types' has been reached." + UpgradeInstructions;
        /// <summary>
        /// The exceeded redis requests
        /// </summary>
        internal const string ExceededRedisRequests = "The free-quota limit on '{0} Redis requests per hour' has been reached." + UpgradeInstructions;
        /// <summary>
        /// The exceeded orm lite tables
        /// </summary>
        internal const string ExceededOrmLiteTables = "The free-quota limit on '{0} OrmLite Tables' has been reached." + UpgradeInstructions;
        /// <summary>
        /// The exceeded aws tables
        /// </summary>
        internal const string ExceededAwsTables = "The free-quota limit on '{0} AWS Tables' has been reached." + UpgradeInstructions;
        /// <summary>
        /// The exceeded service stack operations
        /// </summary>
        internal const string ExceededServiceStackOperations = "The free-quota limit on '{0} ServiceStack Operations' has been reached." + UpgradeInstructions;
        /// <summary>
        /// The exceeded admin UI
        /// </summary>
        internal const string ExceededAdminUi = "The Admin UI is a commercial-only premium feature." + UpgradeInstructions;
        /// <summary>
        /// The exceeded premium feature
        /// </summary>
        internal const string ExceededPremiumFeature = "Unauthorized use of a commercial-only premium feature." + UpgradeInstructions;
        /// <summary>
        /// The unauthorized access request
        /// </summary>
        public const string UnauthorizedAccessRequest = "Unauthorized access request of a licensed feature.";
    }

    /// <summary>
    /// Class FreeQuotas.
    /// </summary>
    public static class FreeQuotas
    {
        /// <summary>
        /// The service stack operations
        /// </summary>
        public const int ServiceStackOperations = 10;
        /// <summary>
        /// The type fields
        /// </summary>
        public const int TypeFields = 20;
        /// <summary>
        /// The redis types
        /// </summary>
        public const int RedisTypes = 20;
        /// <summary>
        /// The redis request per hour
        /// </summary>
        public const int RedisRequestPerHour = 6000;
        /// <summary>
        /// The orm lite tables
        /// </summary>
        public const int OrmLiteTables = 10;
        /// <summary>
        /// The aws tables
        /// </summary>
        public const int AwsTables = 10;
        /// <summary>
        /// The premium feature
        /// </summary>
        public const int PremiumFeature = 0;
    }

    /// <summary>
    /// Asserts the evaluation license.
    /// </summary>
    /// <exception cref="ServiceStack.LicenseException">The evaluation license for this software has expired. " +
    ///                     "See https://servicestack.net to upgrade to a valid license.</exception>
    public static void AssertEvaluationLicense()
    {
        if (DateTime.UtcNow > new DateTime(2013, 12, 31))
            throw new LicenseException("The evaluation license for this software has expired. " +
                                       "See https://servicestack.net to upgrade to a valid license.").Trace();
    }

    /// <summary>
    /// The revoked subs
    /// </summary>
    private static readonly int[] revokedSubs = { 4018, 4019, 4041, 4331, 4581 };

    /// <summary>
    /// Class __ActivatedLicense.
    /// </summary>
    private class __ActivatedLicense
    {
        /// <summary>
        /// The license key
        /// </summary>
        internal readonly LicenseKey LicenseKey;
        /// <summary>
        /// Initializes a new instance of the <see cref="__ActivatedLicense"/> class.
        /// </summary>
        /// <param name="licenseKey">The license key.</param>
        internal __ActivatedLicense(LicenseKey licenseKey)
        {
            LicenseKey = licenseKey;
        }
    }

    /// <summary>
    /// Gets the license warning message.
    /// </summary>
    /// <value>The license warning message.</value>
    public static string LicenseWarningMessage { get; private set; }

    /// <summary>
    /// Gets the license warning message.
    /// </summary>
    /// <returns>System.String.</returns>
    private static string GetLicenseWarningMessage()
    {
        var key = __activatedLicense?.LicenseKey;
        if (key == null)
            return null;

        if (DateTime.UtcNow > key.Expiry)
        {
            var licenseMeta = key.Meta;
            if ((licenseMeta & (long)LicenseMeta.Subscription) != 0)
                return $"This Annual Subscription expired on '{key.Expiry:d}', please update your License Key with this years subscription.";
        }

        return null;
    }

    /// <summary>
    /// The activated license
    /// </summary>
    private static __ActivatedLicense __activatedLicense;

    /// <summary>
    /// The lock object
    /// </summary>
    private static readonly object lockObj = new();

    /// <summary>
    /// Registers the license.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    public static void RegisterLicense(string licenseKeyText)
    {
        JsConfig.InitStatics();

        if (__activatedLicense != null) //Skip multiple license registrations. Use RemoveLicense() to reset.
            return;

        string subId = null;
        var hold = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            if (IsFreeLicenseKey(licenseKeyText))
            {
                ValidateFreeLicenseKey(licenseKeyText);
                return;
            }

            var parts = licenseKeyText.SplitOnFirst('-');
            subId = parts[0];

            if (int.TryParse(subId, out var subIdInt) && revokedSubs.Contains(subIdInt))
                throw new LicenseException("This subscription has been revoked. " + ContactDetails);

            var key = VerifyLicenseKeyText(licenseKeyText);
            ValidateLicenseKey(key);
        }
        catch (PlatformNotSupportedException)
        {
            // Allow usage in environments like dotnet script
            __activatedLicense = new __ActivatedLicense(new LicenseKey { Type = LicenseType.Indie });
        }
        catch (Exception ex)
        {
            //bubble unrelated project Exceptions
            if (ex is FileNotFoundException or FileLoadException or BadImageFormatException or NotSupportedException)
            {
                throw;
            }

            if (ex is LicenseException)
                throw;

            var msg = "This license is invalid." + ContactDetails;
            if (!string.IsNullOrEmpty(subId))
                msg += $" The id for this license is '{subId}'";

            lock (lockObj)
            {
                try
                {
                    var key = PclExport.Instance.VerifyLicenseKeyTextFallback(licenseKeyText);
                    ValidateLicenseKey(key);
                }
                catch (Exception exFallback)
                {
                    if (exFallback is FileNotFoundException || exFallback is FileLoadException || exFallback is BadImageFormatException)
                        throw;

                    throw new LicenseException(msg, exFallback).Trace();
                }
            }

            throw new LicenseException(msg, ex).Trace();
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = hold;
        }
    }

    /// <summary>
    /// Validates the license key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <exception cref="ServiceStack.LicenseException">This license has expired on {key.Expiry:d} and is not valid for use with this release."
    ///                                            + ContactDetails</exception>
    /// <exception cref="ServiceStack.LicenseException">This trial license has expired on {key.Expiry:d}." + ContactDetails</exception>
    private static void ValidateLicenseKey(LicenseKey key)
    {
        var releaseDate = Env.GetReleaseDate();
        if (releaseDate > key.Expiry)
            throw new LicenseException($"This license has expired on {key.Expiry:d} and is not valid for use with this release."
                                       + ContactDetails).Trace();

        if (key.Type == LicenseType.Trial && DateTime.UtcNow > key.Expiry)
            throw new LicenseException($"This trial license has expired on {key.Expiry:d}." + ContactDetails).Trace();

        __activatedLicense = new __ActivatedLicense(key);

        LicenseWarningMessage = GetLicenseWarningMessage();
        if (LicenseWarningMessage != null)
            Console.WriteLine(LicenseWarningMessage);
    }

    private const string IndividualPrefix = "Individual (c) ";
    private const string OpenSourcePrefix = "OSS ";

    private static bool IsFreeLicenseKey(string licenseText) =>
        licenseText.StartsWith(IndividualPrefix) || licenseText.StartsWith(OpenSourcePrefix);

    private static void ValidateFreeLicenseKey(string licenseText)
    {
        if (!IsFreeLicenseKey(licenseText))
            throw new NotSupportedException("Not a free License Key");

        var envKey = Environment.GetEnvironmentVariable("SERVICESTACK_LICENSE");
        if (envKey == licenseText)
            throw new LicenseException("Cannot use SERVICESTACK_LICENSE Environment variable with free License Keys, " +
                                       "please use Licensing.RegisterLicense() in source code.");

        LicenseKey key = null;
        if (licenseText.StartsWith(IndividualPrefix))
        {
            key = VerifyIndividualLicense(licenseText);
            if (key == null)
                throw new LicenseException("Individual License Key is invalid.");
        }
        else if (licenseText.StartsWith(OpenSourcePrefix))
        {
            key = VerifyOpenSourceLicense(licenseText);
            if (key == null)
                throw new LicenseException("Open Source License Key is invalid.");
        }
        else
        {
            throw new NotSupportedException("Not a free License Key");
        }

        var releaseDate = Env.GetReleaseDate();
        if (releaseDate > key.Expiry)
            throw new LicenseException($"This license has expired on {key.Expiry:d} and is not valid for use with this release.\n"
                                       + "Check https://servicestack.net/free for eligible renewals.").Trace();

        __activatedLicense = new __ActivatedLicense(key);
    }

    internal static string Info => __activatedLicense?.LicenseKey == null
                                       ? "NO"
                                       : __activatedLicense.LicenseKey.Type switch
                                           {
                                               LicenseType.Free => "FR",
                                               LicenseType.FreeIndividual => "FI",
                                               LicenseType.FreeOpenSource => "FO",
                                               LicenseType.Indie => "IN",
                                               LicenseType.Business => "BU",
                                               LicenseType.Enterprise => "EN",
                                               LicenseType.TextIndie => "TI",
                                               LicenseType.TextBusiness => "TB",
                                               LicenseType.OrmLiteIndie => "OI",
                                               LicenseType.OrmLiteBusiness => "OB",
                                               LicenseType.RedisIndie => "RI",
                                               LicenseType.RedisBusiness => "RB",
                                               LicenseType.AwsIndie => "AI",
                                               LicenseType.AwsBusiness => "AB",
                                               LicenseType.Trial => "TR",
                                               LicenseType.Site => "SI",
                                               LicenseType.TextSite => "TS",
                                               LicenseType.RedisSite => "RS",
                                               LicenseType.OrmLiteSite => "OS",
                                               _ => "UN",
                                           };

    private static LicenseKey VerifyIndividualLicense(string licenseKey)
    {
        if (licenseKey == null)
            return null;
        if (licenseKey.Length < 100)
            return null;
        if (!licenseKey.StartsWith(IndividualPrefix))
            return null;
        var keyText = licenseKey.LastLeftPart(' ');
        var keySign = licenseKey.LastRightPart(' ');
        if (keySign.Length < 48)
            return null;

        try
        {
            var rsa = System.Security.Cryptography.RSA.Create();
            rsa.FromXml(LicensePublicKey);

#if !NET7_0_OR_GREATER
            var verified = ((System.Security.Cryptography.RSACryptoServiceProvider)rsa)
                .VerifyData(keyText.ToUtf8Bytes(), "SHA256", Convert.FromBase64String(keySign));
#else
            var verified = rsa.VerifyData(keyText.ToUtf8Bytes(), 
                    Convert.FromBase64String(keySign), 
                    System.Security.Cryptography.HashAlgorithmName.SHA256, 
                    System.Security.Cryptography.RSASignaturePadding.Pkcs1);
#endif
            if (verified)
            {
                var yearStr = keyText.Substring(IndividualPrefix.Length).LeftPart(' ');
                if (yearStr.Length == 4 && int.TryParse(yearStr, out var year))
                {
                    return new LicenseKey
                               {
                                   Expiry = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                   Hash = keySign,
                                   Name = keyText,
                                   Type = LicenseType.FreeIndividual,
                               };
                }
            }
        }
        catch { }

        return null;
    }

    private static LicenseKey VerifyOpenSourceLicense(string licenseKey)
    {
        if (licenseKey == null)
            return null;
        if (licenseKey.Length < 100)
            return null;
        if (!licenseKey.StartsWith(OpenSourcePrefix))
            return null;
        var keyText = licenseKey.LastLeftPart(' ');
        var keySign = licenseKey.LastRightPart(' ');
        if (keySign.Length < 48)
            return null;

        try
        {
            var rsa = System.Security.Cryptography.RSA.Create();
            rsa.FromXml(LicensePublicKey);

#if !NET7_0_OR_GREATER
            var verified = ((System.Security.Cryptography.RSACryptoServiceProvider)rsa)
                .VerifyData(keyText.ToUtf8Bytes(), "SHA256", Convert.FromBase64String(keySign));
#else
            var verified = rsa.VerifyData(keyText.ToUtf8Bytes(), 
                    Convert.FromBase64String(keySign), 
                    System.Security.Cryptography.HashAlgorithmName.SHA256, 
                    System.Security.Cryptography.RSASignaturePadding.Pkcs1);
#endif
            if (verified)
            {
                var yearStr = keyText.Substring(OpenSourcePrefix.Length).RightPart(' ').LeftPart(' ');
                if (yearStr.Length == 4 && int.TryParse(yearStr, out var year))
                {
                    return new LicenseKey
                               {
                                   Expiry = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                   Hash = keySign,
                                   Name = keyText,
                                   Type = LicenseType.FreeOpenSource,
                               };
                }
            }
        }
        catch { }

        return null;
    }

    /// <summary>
    /// Removes the license.
    /// </summary>
    public static void RemoveLicense()
    {
        __activatedLicense = null;
    }

    /// <summary>
    /// Activateds the license features.
    /// </summary>
    /// <returns>LicenseFeature.</returns>
    public static LicenseFeature ActivatedLicenseFeatures()
    {
        return __activatedLicense?.LicenseKey.GetLicensedFeatures() ?? LicenseFeature.None;
    }

    /// <summary>
    /// Approveds the usage.
    /// </summary>
    /// <param name="licenseFeature">The license feature.</param>
    /// <param name="requestedFeature">The requested feature.</param>
    /// <param name="allowedUsage">The allowed usage.</param>
    /// <param name="actualUsage">The actual usage.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ServiceStack.LicenseException"></exception>
    public static void ApprovedUsage(LicenseFeature licenseFeature, LicenseFeature requestedFeature,
                                     int allowedUsage, int actualUsage, string message)
    {
        var hasFeature = (requestedFeature & licenseFeature) == requestedFeature;
        if (hasFeature)
            return;

        if (actualUsage > allowedUsage)
            throw new LicenseException(message.Fmt(allowedUsage)).Trace();
    }

    /// <summary>
    /// Determines whether [has licensed feature] [the specified feature].
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <returns><c>true</c> if [has licensed feature] [the specified feature]; otherwise, <c>false</c>.</returns>
    public static bool HasLicensedFeature(LicenseFeature feature)
    {
        var licensedFeatures = ActivatedLicenseFeatures();
        return (feature & licensedFeatures) == feature;
    }

    /// <summary>
    /// Asserts the valid usage.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <param name="quotaType">Type of the quota.</param>
    /// <param name="count">The count.</param>
    /// <exception cref="ServiceStack.LicenseException">Unknown Quota Usage: {0}, {1}".Fmt(feature, quotaType)</exception>
    public static void AssertValidUsage(LicenseFeature feature, QuotaType quotaType, int count)
    {
        var licensedFeatures = ActivatedLicenseFeatures();
        if ((LicenseFeature.All & licensedFeatures) == LicenseFeature.All) //Standard Usage
            return;

        //Free Quotas
        switch (feature)
        {
            case LicenseFeature.Redis:
                switch (quotaType)
                {
                    case QuotaType.Types:
                        ApprovedUsage(licensedFeatures, feature, FreeQuotas.RedisTypes, count, ErrorMessages.ExceededRedisTypes);
                        return;
                    case QuotaType.RequestsPerHour:
                        ApprovedUsage(licensedFeatures, feature, FreeQuotas.RedisRequestPerHour, count, ErrorMessages.ExceededRedisRequests);
                        return;
                }
                break;

            case LicenseFeature.OrmLite:
                switch (quotaType)
                {
                    case QuotaType.Tables:
                        ApprovedUsage(licensedFeatures, feature, FreeQuotas.OrmLiteTables, count, ErrorMessages.ExceededOrmLiteTables);
                        return;
                }
                break;

            case LicenseFeature.Aws:
                switch (quotaType)
                {
                    case QuotaType.Tables:
                        ApprovedUsage(licensedFeatures, feature, FreeQuotas.AwsTables, count, ErrorMessages.ExceededAwsTables);
                        return;
                }
                break;

            case LicenseFeature.ServiceStack:
                switch (quotaType)
                {
                    case QuotaType.Operations:
                        ApprovedUsage(licensedFeatures, feature, FreeQuotas.ServiceStackOperations, count, ErrorMessages.ExceededServiceStackOperations);
                        return;
                }
                break;

            case LicenseFeature.Admin:
                switch (quotaType)
                {
                    case QuotaType.PremiumFeature:
                        ApprovedUsage(licensedFeatures, feature, FreeQuotas.PremiumFeature, count, ErrorMessages.ExceededAdminUi);
                        return;
                }
                break;

            case LicenseFeature.Premium:
                switch (quotaType)
                {
                    case QuotaType.PremiumFeature:
                        ApprovedUsage(licensedFeatures, feature, FreeQuotas.PremiumFeature, count, ErrorMessages.ExceededPremiumFeature);
                        return;
                }
                break;
        }

        throw new LicenseException("Unknown Quota Usage: {0}, {1}".Fmt(feature, quotaType)).Trace();
    }

    /// <summary>
    /// Gets the licensed features.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>LicenseFeature.</returns>
    /// <exception cref="System.ArgumentException">Unknown License Type: " + key.Type</exception>
    public static LicenseFeature GetLicensedFeatures(this LicenseKey key)
    {
        switch (key.Type)
        {
            case LicenseType.Free:
                return LicenseFeature.Free;

            case LicenseType.Indie:
            case LicenseType.Business:
            case LicenseType.Enterprise:
            case LicenseType.Trial:
            case LicenseType.Site:
                return LicenseFeature.All;

            case LicenseType.TextIndie:
            case LicenseType.TextBusiness:
            case LicenseType.TextSite:
                return LicenseFeature.Text;

            case LicenseType.OrmLiteIndie:
            case LicenseType.OrmLiteBusiness:
            case LicenseType.OrmLiteSite:
                return LicenseFeature.OrmLiteSku;

            case LicenseType.AwsIndie:
            case LicenseType.AwsBusiness:
                return LicenseFeature.AwsSku;

            case LicenseType.RedisIndie:
            case LicenseType.RedisBusiness:
            case LicenseType.RedisSite:
                return LicenseFeature.RedisSku;
        }
        throw new ArgumentException("Unknown License Type: " + key.Type).Trace();
    }

    /// <summary>
    /// Converts to licensekey.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    /// <returns>LicenseKey.</returns>
    /// <exception cref="ServiceStack.LicenseException">The license '{0}' is not assigned to CustomerId '{1}'.".Fmt(base64, refId)</exception>
    public static LicenseKey ToLicenseKey(this string licenseKeyText)
    {
        licenseKeyText = Regex.Replace(licenseKeyText, @"\s+", "");
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
                throw new LicenseException("The license '{0}' is not assigned to CustomerId '{1}'.".Fmt(base64, refId)).Trace();

            return key;
        }
        finally
        {
            JsConfig<DateTime>.DeSerializeFn = hold;
            JsConfig<DateTime>.RawDeserializeFn = holdRaw;
        }
    }

    /// <summary>
    /// Converts to licensekeyfallback.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    /// <returns>LicenseKey.</returns>
    /// <exception cref="ServiceStack.LicenseException">The license '{base64}' is not assigned to CustomerId '{refId}'.</exception>
    public static LicenseKey ToLicenseKeyFallback(this string licenseKeyText)
    {
        licenseKeyText = Regex.Replace(licenseKeyText, @"\s+", "");
        var parts = licenseKeyText.SplitOnFirst('-');
        var refId = parts[0];
        var base64 = parts[1];
        var jsv = Convert.FromBase64String(base64).FromUtf8Bytes();

        var map = jsv.FromJsv<Dictionary<string, string>>();
        var key = new LicenseKey
                      {
                          Ref = map.Get("Ref"),
                          Name = map.Get("Name"),
                          Type = (LicenseType)Enum.Parse(typeof(LicenseType), map.Get("Type"), ignoreCase: true),
                          Hash = map.Get("Hash"),
                          Expiry = DateTimeSerializer.ParseManual(map.Get("Expiry"), DateTimeKind.Utc).GetValueOrDefault(),
                      };

        if (key.Ref != refId)
            throw new LicenseException($"The license '{base64}' is not assigned to CustomerId '{refId}'.").Trace();

        return key;
    }

    /// <summary>
    /// Gets the hash key to sign.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public static string GetHashKeyToSign(this LicenseKey key)
    {
        return $"{key.Ref}:{key.Name}:{key.Expiry:yyyy-MM-dd}:{key.Type}";
    }

    /// <summary>
    /// Gets the inner most exception.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns>Exception.</returns>
    public static Exception GetInnerMostException(this Exception ex)
    {
        //Extract true exception from static initializers (e.g. LicenseException)
        while (ex.InnerException != null)
        {
            ex = ex.InnerException;
        }
        return ex;
    }

    //License Utils
    /// <summary>
    /// Verifies the signed hash.
    /// </summary>
    /// <param name="DataToVerify">The data to verify.</param>
    /// <param name="SignedData">The signed data.</param>
    /// <param name="Key">The key.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, System.Security.Cryptography.RSAParameters Key)
    {
        try
        {
            var RSAalg = new System.Security.Cryptography.RSACryptoServiceProvider(2048);
            RSAalg.ImportParameters(Key);
            return RSAalg.VerifySha1Data(DataToVerify, SignedData);

        }
        catch (System.Security.Cryptography.CryptographicException ex)
        {
            Tracer.Instance.WriteError(ex);
            return false;
        }
    }

    /// <summary>
    /// Verifies the license key text.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    /// <returns>LicenseKey.</returns>
    /// <exception cref="System.ArgumentException">licenseKeyText</exception>
    public static LicenseKey VerifyLicenseKeyText(string licenseKeyText)
    {
#if NETFX || NET7_0_OR_GREATER
        LicenseKey key;
        try
        {
            if (!licenseKeyText.VerifyLicenseKeyText(out key))
                throw new ArgumentException("licenseKeyText");
        }
        catch (Exception)
        {
            if (!VerifyLicenseKeyTextFallback(licenseKeyText, out key))
                throw;
        }
        return key;
#else
            return licenseKeyText.ToLicenseKey();
#endif
    }

    /// <summary>
    /// Froms the XML.
    /// </summary>
    /// <param name="rsa">The RSA.</param>
    /// <param name="xml">The XML.</param>
    private static void FromXml(this System.Security.Cryptography.RSA rsa, string xml)
    {
#if NETFX
        rsa.FromXmlString(xml);
#else
            //throws PlatformNotSupportedException
            var csp = ExtractFromXml(xml);
            rsa.ImportParameters(csp);
#endif
    }

#if !NET48
        private static System.Security.Cryptography.RSAParameters ExtractFromXml(string xml)
        {
            var csp = new System.Security.Cryptography.RSAParameters();
            using (var reader = System.Xml.XmlReader.Create(new StringReader(xml)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType != System.Xml.XmlNodeType.Element)
                        continue;

                    var elName = reader.Name;
                    if (elName == "RSAKeyValue")
                        continue;

                    do
                    {
                        reader.Read();
                    } while (reader.NodeType != System.Xml.XmlNodeType.Text && reader.NodeType != System.Xml.XmlNodeType.EndElement);

                    if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
                        continue;

                    var value = reader.Value;
                    switch (elName)
                    {
                        case "Modulus":
                            csp.Modulus = Convert.FromBase64String(value);
                            break;
                        case "Exponent":
                            csp.Exponent = Convert.FromBase64String(value);
                            break;
                        case "P":
                            csp.P = Convert.FromBase64String(value);
                            break;
                        case "Q":
                            csp.Q = Convert.FromBase64String(value);
                            break;
                        case "DP":
                            csp.DP = Convert.FromBase64String(value);
                            break;
                        case "DQ":
                            csp.DQ = Convert.FromBase64String(value);
                            break;
                        case "InverseQ":
                            csp.InverseQ = Convert.FromBase64String(value);
                            break;
                        case "D":
                            csp.D = Convert.FromBase64String(value);
                            break;
                    }
                }

                return csp;
            }
        }
#endif

    /// <summary>
    /// Verifies the license key text.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool VerifyLicenseKeyText(this string licenseKeyText, out LicenseKey key)
    {
        var publicRsaProvider = new System.Security.Cryptography.RSACryptoServiceProvider(2048);
        publicRsaProvider.FromXml(LicenseUtils.LicensePublicKey);
        var publicKeyParams = publicRsaProvider.ExportParameters(false);

        key = licenseKeyText.ToLicenseKey();
        var originalData = key.GetHashKeyToSign().ToUtf8Bytes();
        var signedData = Convert.FromBase64String(key.Hash);

        return VerifySignedHash(originalData, signedData, publicKeyParams);
    }

    /// <summary>
    /// Verifies the license key text fallback.
    /// </summary>
    /// <param name="licenseKeyText">The license key text.</param>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="System.Exception">Could not import LicensePublicKey</exception>
    /// <exception cref="System.Exception">Could not deserialize LicenseKeyText Manually</exception>
    /// <exception cref="System.Exception">Could not convert HashKey to UTF-8</exception>
    /// <exception cref="System.Exception">Could not convert key.Hash from Base64</exception>
    /// <exception cref="System.Exception">Could not Verify License Key ({originalData.Length}, {signedData.Length})</exception>
    public static bool VerifyLicenseKeyTextFallback(this string licenseKeyText, out LicenseKey key)
    {
        System.Security.Cryptography.RSAParameters publicKeyParams;
        try
        {
            var publicRsaProvider = new System.Security.Cryptography.RSACryptoServiceProvider(2048);
            publicRsaProvider.FromXml(LicenseUtils.LicensePublicKey);
            publicKeyParams = publicRsaProvider.ExportParameters(false);
        }
        catch (Exception ex)
        {
            throw new Exception("Could not import LicensePublicKey", ex);
        }

        try
        {
            key = licenseKeyText.ToLicenseKeyFallback();
        }
        catch (Exception ex)
        {
            throw new Exception("Could not deserialize LicenseKeyText Manually", ex);
        }

        byte[] originalData;
        byte[] signedData;

        try
        {
            originalData = key.GetHashKeyToSign().ToUtf8Bytes();
        }
        catch (Exception ex)
        {
            throw new Exception("Could not convert HashKey to UTF-8", ex);
        }

        try
        {
            signedData = Convert.FromBase64String(key.Hash);
        }
        catch (Exception ex)
        {
            throw new Exception("Could not convert key.Hash from Base64", ex);
        }

        try
        {
            return VerifySignedHash(originalData, signedData, publicKeyParams);
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not Verify License Key ({originalData.Length}, {signedData.Length})", ex);
        }
    }

    /// <summary>
    /// Verifies the sha1 data.
    /// </summary>
    /// <param name="RSAalg">The rs aalg.</param>
    /// <param name="unsignedData">The unsigned data.</param>
    /// <param name="encryptedData">The encrypted data.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool VerifySha1Data(this System.Security.Cryptography.RSACryptoServiceProvider RSAalg, byte[] unsignedData, byte[] encryptedData)
    {
        using var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        return RSAalg.VerifyData(unsignedData, sha, encryptedData);
    }
}