/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Utilities;

using System;

using Microsoft.Win32;

/// <summary>
/// The class gets common system info. Used in data layers other than MSSQL. Created by vzrus 2010
/// </summary>
public static class SystemInfo
{
    /// <summary>
    /// Gets the number of memory bytes currently thought to be allocated.
    /// </summary>
    public static long AllocatedMemory => GC.GetTotalMemory(false);

    /// <summary>
    /// Gets the amount of physical memory mapped to the process context.
    /// </summary>
    public static long MappedMemory => Environment.WorkingSet;

    /// <summary>
    /// Gets Processors.
    /// </summary>
    [NotNull]
    public static string Processors => Environment.ProcessorCount.ToString();

    /// <summary>
    /// Gets Runtime String.
    /// </summary>
    /// <value>
    /// The runtime string.
    /// </value>
    [NotNull]
    public static string RuntimeString
    {
        get
        {
            try
            {
                const string SubKey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

                using var openSubKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                    .OpenSubKey(SubKey);
                return openSubKey?.GetValue("Release") != null
                           ? $"Framework Version: {CheckFor45PlusVersion(openSubKey.GetValue("Release").ToType<int>())}"
                           : Environment.Version.ToString();
            }
            catch (Exception)
            {
                return Environment.Version.ToString();
            }
        }
    }

    /// <summary>
    /// Gets the version string.
    /// </summary>
    /// <value>
    /// The version string.
    /// </value>
    public static string VersionString => Environment.OSVersion.VersionString;

    /// <summary>
    /// Checking the version using >= will enable forward compatibility.
    /// </summary>
    /// <param name="releaseKey">The release key.</param>
    /// <returns>Returns the version string.</returns>
    private static string CheckFor45PlusVersion(int releaseKey)
    {
        return releaseKey switch {
                >= 528040 => "4.8 or later",
                >= 461808 => "4.7.2",
                >= 461308 => "4.7.1",
                >= 460798 => "4.7",
                >= 394802 => "4.6.2",
                >= 394254 => "4.6.1",
                >= 393295 => "4.6",
                >= 379893 => "4.5.2",
                >= 378675 => "4.5.1",
                _ => releaseKey >= 378389 ? "4.5" : "No 4.5 or later version detected"
            };
    }
}