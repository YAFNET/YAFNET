// ***********************************************************************
// <copyright file="Env.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/// <summary>
/// Class Env.
/// </summary>
public static class Env
{
    /// <summary>
    /// Initializes static members of the <see cref="Env" /> class.
    /// </summary>
    /// <exception cref="System.ArgumentException">PclExport.Instance needs to be initialized</exception>
    static Env()
    {

        if (PclExport.Instance == null)
            throw new ArgumentException("PclExport.Instance needs to be initialized");

#if NETCORE
            IsNetStandard = true;
            try
            {
                IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                IsOSX  = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

                var fxDesc = RuntimeInformation.FrameworkDescription;
                IsMono = fxDesc.Contains("Mono");
                IsNetCore = fxDesc.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception) {} //throws PlatformNotSupportedException in AWS lambda
            IsUnix = IsOSX || IsLinux;
            HasMultiplePlatformTargets = true;
            IsUWP = IsRunningAsUwp();
#elif NETFX
        IsNetFramework = true;
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
                IsWindows = true;
                break;
        }

        var platform = (int)Environment.OSVersion.Platform;
        IsUnix = platform is 4 or 6 or 128;

        if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
            IsOSX = true;
        var osType = File.Exists(@"/proc/sys/kernel/ostype")
                         ? File.ReadAllText(@"/proc/sys/kernel/ostype")
                         : null;
        IsLinux = osType?.IndexOf("Linux", StringComparison.OrdinalIgnoreCase) >= 0;
        try
        {
            IsMono = AssemblyUtils.FindType("Mono.Runtime") != null;
        }
        catch (Exception)
        {
            IsMono = false;
        }

        SupportsDynamic = true;
#endif

#if NETCORE
            IsNetStandard = false;
            IsNetCore = true;
            SupportsDynamic = true;
#endif

#if NET6_0
            IsNet6 = true;
#endif

        if (!IsUWP)
        {
            try
            {
                IsAndroid = AssemblyUtils.FindType("Android.Manifest") != null;
                if (IsOSX && IsMono)
                {
                    var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();

                    // iOS detection no longer trustworthy so assuming iOS based on some current heuristics. TODO: improve iOS detection
                    IsIOS = runtimeDir.StartsWith("/private/var") || runtimeDir.Contains("/CoreSimulator/Devices/");
                }
            }
            catch (Exception)
            {
                IsIOS = false;
            }
        }

        SupportsExpressions = true;
        SupportsEmit = !(IsUWP || IsIOS);

        if (!SupportsEmit)
        {
            ReflectionOptimizer.Instance = ExpressionReflectionOptimizer.Provider;
        }

        VersionString = ServiceStackVersion.ToString(CultureInfo.InvariantCulture);

        UpdateServerUserAgent();
    }

    internal static void UpdateServerUserAgent()
    {
        ServerUserAgent =
            $"ServiceStack/{VersionString} {PclExport.Instance.PlatformName}{(IsLinux ? "/Linux" : IsOSX ? "/OSX" : IsUnix ? "/Unix" : IsWindows ? "/Windows" : "/UnknownOS")}{(IsIOS ? "/iOS" : IsAndroid ? "/Android" : IsUWP ? "/UWP" : "")}{(IsNet6 ? "/net6" : IsNetFramework ? "netfx" : "")}/{LicenseUtils.Info}";
    }

    /// <summary>
    /// Gets or sets the version string.
    /// </summary>
    /// <value>The version string.</value>
    public static string VersionString { get; set; }

    /// <summary>
    /// The service stack version
    /// </summary>
    public static decimal ServiceStackVersion = 6.51m;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is linux.
    /// </summary>
    /// <value><c>true</c> if this instance is linux; otherwise, <c>false</c>.</value>
    public static bool IsLinux { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is osx.
    /// </summary>
    /// <value><c>true</c> if this instance is osx; otherwise, <c>false</c>.</value>
    public static bool IsOSX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is unix.
    /// </summary>
    /// <value><c>true</c> if this instance is unix; otherwise, <c>false</c>.</value>
    public static bool IsUnix { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is windows.
    /// </summary>
    /// <value><c>true</c> if this instance is windows; otherwise, <c>false</c>.</value>
    public static bool IsWindows { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is mono.
    /// </summary>
    /// <value><c>true</c> if this instance is mono; otherwise, <c>false</c>.</value>
    public static bool IsMono { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is ios.
    /// </summary>
    /// <value><c>true</c> if this instance is ios; otherwise, <c>false</c>.</value>
    public static bool IsIOS { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is android.
    /// </summary>
    /// <value><c>true</c> if this instance is android; otherwise, <c>false</c>.</value>
    public static bool IsAndroid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is net native.
    /// </summary>
    /// <value><c>true</c> if this instance is net native; otherwise, <c>false</c>.</value>
    public static bool IsNetNative { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is uwp.
    /// </summary>
    /// <value><c>true</c> if this instance is uwp; otherwise, <c>false</c>.</value>
    public static bool IsUWP { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is net standard.
    /// </summary>
    /// <value><c>true</c> if this instance is net standard; otherwise, <c>false</c>.</value>
    public static bool IsNetStandard { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is net framework.
    /// </summary>
    /// <value><c>true</c> if this instance is net framework; otherwise, <c>false</c>.</value>
    public static bool IsNetFramework { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is net core.
    /// </summary>
    /// <value><c>true</c> if this instance is net core; otherwise, <c>false</c>.</value>
    public static bool IsNetCore { get; set; }

    public static bool IsNet6 { get; set; }

    /// <summary>
    /// Gets a value indicating whether [supports expressions].
    /// </summary>
    /// <value><c>true</c> if [supports expressions]; otherwise, <c>false</c>.</value>
    public static bool SupportsExpressions { get; }

    /// <summary>
    /// Gets a value indicating whether [supports emit].
    /// </summary>
    /// <value><c>true</c> if [supports emit]; otherwise, <c>false</c>.</value>
    public static bool SupportsEmit { get; }

    /// <summary>
    /// Gets a value indicating whether [supports dynamic].
    /// </summary>
    /// <value><c>true</c> if [supports dynamic]; otherwise, <c>false</c>.</value>
    public static bool SupportsDynamic { get; }

    /// <summary>
    /// The strict mode
    /// </summary>
    private static bool strictMode;

    /// <summary>
    /// Gets or sets a value indicating whether [strict mode].
    /// </summary>
    /// <value><c>true</c> if [strict mode]; otherwise, <c>false</c>.</value>
    public static bool StrictMode
    {
        get => strictMode;
        set => Config.Instance.ThrowOnError = strictMode = value;
    }

    /// <summary>
    /// Gets or sets the server user agent.
    /// </summary>
    /// <value>The server user agent.</value>
    public static string ServerUserAgent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance has multiple platform targets.
    /// </summary>
    /// <value><c>true</c> if this instance has multiple platform targets; otherwise, <c>false</c>.</value>
    public static bool HasMultiplePlatformTargets { get; set; }

    /// <summary>
    /// The get release date.
    /// </summary>
    /// <returns>
    /// The <see cref="DateTime"/>.
    /// </returns>
    public static DateTime GetReleaseDate() => new(2001, 01, 01);

#if NETCORE
        private static bool IsRunningAsUwp()
        {
            try
            {
                IsNetNative = RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);
                return IsInAppContainer || IsNetNative;
            }
            catch (Exception) {}
            return false;
        }

        private static bool IsWindows7OrLower
        {
            get
            {
                int versionMajor = Environment.OSVersion.Version.Major;
                int versionMinor = Environment.OSVersion.Version.Minor;
                double version = versionMajor + (double)versionMinor / 10;
                return version <= 6.1;
            }
        }

        // From: https://github.com/dotnet/corefx/blob/master/src/CoreFx.Private.TestUtilities/src/System/PlatformDetection.Windows.cs
        private static int s_isInAppContainer = -1;
        private static bool IsInAppContainer
        {
            // This actually checks whether code is running in a modern app.
            // Currently this is the only situation where we run in app container.
            // If we want to distinguish the two cases in future,
            // EnvironmentHelpers.IsAppContainerProcess in desktop code shows how to check for the AC token.
            get
            {
                if (s_isInAppContainer != -1)
                    return s_isInAppContainer == 1;

                if (!IsWindows || IsWindows7OrLower)
                {
                    s_isInAppContainer = 0;
                    return false;
                }

                byte[] buffer = TypeConstants.EmptyByteArray;
                uint bufferSize = 0;
                try
                {
                    int result = GetCurrentApplicationUserModelId(ref bufferSize, buffer);
                    switch (result)
                    {
                        case 15703: // APPMODEL_ERROR_NO_APPLICATION
                            s_isInAppContainer = 0;
                            break;
                        case 0:     // ERROR_SUCCESS
                        case 122:   // ERROR_INSUFFICIENT_BUFFER
                                    // Success is actually insufficient buffer as we're really only looking for
                                    // not NO_APPLICATION and we're not actually giving a buffer here. The
                                    // API will always return NO_APPLICATION if we're not running under a
                                    // WinRT process, no matter what size the buffer is.
                            s_isInAppContainer = 1;
                            break;
                        default:
                            throw new InvalidOperationException($"Failed to get AppId, result was {result}.");
                    }
                }
                catch (Exception e)
                {
                    // We could catch this here, being friendly with older portable surface area should we
                    // desire to use this method elsewhere.
                    if (e.GetType().FullName.Equals("System.EntryPointNotFoundException", StringComparison.Ordinal))
                    {
                        // API doesn't exist, likely pre Win8
                        s_isInAppContainer = 0;
                    }
                    else
                    {
                        throw;
                    }
                }

                return s_isInAppContainer == 1;
            }
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern int GetCurrentApplicationUserModelId(ref uint applicationUserModelIdLength, byte[] applicationUserModelId);
#endif

    /// <summary>
    /// The continue on captured context
    /// </summary>
    public const bool ContinueOnCapturedContext = false;

    /// <summary>
    /// Configurations the await.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>ConfiguredTaskAwaitable.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable ConfigAwait(this Task task) =>
        task.ConfigureAwait(ContinueOnCapturedContext);

    /// <summary>
    /// Configurations the await.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task">The task.</param>
    /// <returns>ConfiguredTaskAwaitable&lt;T&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable<T> ConfigAwait<T>(this Task<T> task) =>
        task.ConfigureAwait(ContinueOnCapturedContext);

    /// <summary>
    /// Only .ConfigAwait(false) in .NET Core as loses HttpContext.Current in NETFX/ASP.NET
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable ConfigAwaitNetCore(this Task task) =>
#if NETCORE
            task.ConfigureAwait(false);
#else
        task.ConfigureAwait(true);
#endif

    /// <summary>
    /// Only .ConfigAwait(false) in .NET Core as loses HttpContext.Current in NETFX/ASP.NET
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConfiguredTaskAwaitable<T> ConfigAwaitNetCore<T>(this Task<T> task) =>
#if NETCORE
            task.ConfigureAwait(false);
#else
        task.ConfigureAwait(true);
#endif

#if NETCORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfiguredValueTaskAwaitable ConfigAwait(this ValueTask task) => 
            task.ConfigureAwait(ContinueOnCapturedContext);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfiguredValueTaskAwaitable<T> ConfigAwait<T>(this ValueTask<T> task) => 
            task.ConfigureAwait(ContinueOnCapturedContext);
#endif
}