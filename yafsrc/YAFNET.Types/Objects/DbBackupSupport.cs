/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Objects;

/// <summary>
/// Whether a database backup can be made from the application, and why not.
/// </summary>
public class DbBackupSupport
{
    /// <summary>
    /// Gets a value indicating whether a database backup can be made.
    /// </summary>
    public bool IsSupported { get; init; }

    /// <summary>
    /// Gets the reason why the backup is not available, or a note about a supported backup (e.g. a check
    /// that could not be made and is made by the first backup).
    /// </summary>
    public string Reason { get; init; }

    /// <summary>
    /// The backup is available.
    /// </summary>
    /// <param name="note">An optional note.</param>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    public static DbBackupSupport Supported(string note = null)
    {
        return new DbBackupSupport { IsSupported = true, Reason = note };
    }

    /// <summary>
    /// The backup is not available.
    /// </summary>
    /// <param name="reason">The reason.</param>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    public static DbBackupSupport NotSupported(string reason)
    {
        return new DbBackupSupport { IsSupported = false, Reason = reason };
    }
}
