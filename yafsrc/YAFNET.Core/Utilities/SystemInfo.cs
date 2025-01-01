/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

/// <summary>
/// The class gets common system info. Used in data layers other than MSSQL.
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
    public static string Processors => Environment.ProcessorCount.ToString();

    /// <summary>
    /// Gets the version string.
    /// </summary>
    /// <value>
    /// The version string.
    /// </value>
    public static string VersionString => Environment.OSVersion.VersionString;
}