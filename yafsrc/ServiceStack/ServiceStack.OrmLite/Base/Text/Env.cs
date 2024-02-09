// ***********************************************************************
// <copyright file="Env.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Base.Text;

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
    }

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
    /// Gets a value indicating whether this instance is uwp.
    /// </summary>
    /// <value><c>true</c> if this instance is uwp; otherwise, <c>false</c>.</value>
    public static bool IsUWP { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is net framework.
    /// </summary>
    /// <value><c>true</c> if this instance is net framework; otherwise, <c>false</c>.</value>
    public static bool IsNetFramework { get; set; }

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
    /// The get release date.
    /// </summary>
    /// <returns>
    /// The <see cref="DateTime"/>.
    /// </returns>
    public static DateTime GetReleaseDate() => new(2001, 01, 01);

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
}