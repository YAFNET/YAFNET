﻿/* Yet Another Forum.NET
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

namespace YAF.Types.Modals;

/// <summary>
/// Class InstallModal.
/// </summary>
public class InstallModal
{
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the connection information.
    /// </summary>
    /// <value>The connection information.</value>
    public string ConnectionInfo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [connection success].
    /// </summary>
    /// <value><c>true</c> if [connection success]; otherwise, <c>false</c>.</value>
    public bool ConnectionSuccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [test email success].
    /// </summary>
    /// <value><c>true</c> if [test email success]; otherwise, <c>false</c>.</value>
    public bool TestEmailSuccess { get; set; }

    /// <summary>
    /// Gets or sets the test email information.
    /// </summary>
    /// <value>The test email information.</value>
    public string TestEmailInfo { get; set; }
}