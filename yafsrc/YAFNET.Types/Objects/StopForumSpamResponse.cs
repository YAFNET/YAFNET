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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>
/// StopForumSpam.com JSON Response Class
/// </summary>
public class StopForumSpamResponse
{
    /// <summary>
    /// Gets or sets the success string.
    /// </summary>
    /// <value>
    /// The success string.
    /// </value>
    [JsonPropertyName("success")]
    public int SuccessString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [success].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [success]; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool Success
    {
        get => this.SuccessString == 1;

        set => this.SuccessString = value ? 1 : 0;
    }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    [JsonPropertyName("username")]
    public UserName UserName { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    [JsonPropertyName("email")]
    public Email Email { get; set; }

    /// <summary>
    /// Gets or sets the IP address.
    /// </summary>
    /// <value>
    /// The IP address.
    /// </value>
    [JsonPropertyName("ip")]
    public Ip IpAddress { get; set; }
}

/// <summary>
/// User Name Namespace
/// </summary>
[DataContract(Namespace = "username")]
public class UserName
{
    /// <summary>
    /// Gets or sets the frequency.
    /// </summary>
    /// <value>
    /// The frequency.
    /// </value>
    [JsonPropertyName("frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Gets or sets the appears string.
    /// </summary>
    /// <value>
    /// The appears string.
    /// </value>
    [JsonPropertyName("appears")]
    public int AppearsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [appears].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool Appears
    {
        get => this.AppearsString == 1;

        set => this.AppearsString = value ? 1 : 0;
    }
}

/// <summary>
/// Email Namespace
/// </summary>
[DataContract(Namespace = "email")]
public class Email
{
    /// <summary>
    /// Gets or sets the frequency.
    /// </summary>
    /// <value>
    /// The frequency.
    /// </value>
    [JsonPropertyName("frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Gets or sets the appears string.
    /// </summary>
    /// <value>
    /// The appears string.
    /// </value>
    [JsonPropertyName("appears")]
    public int AppearsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [appears].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool Appears
    {
        get => this.AppearsString == 1;

        set => this.AppearsString = value ? 1 : 0;
    }
}

/// <summary>
/// IP Address Namespace
/// </summary>
[DataContract(Namespace = "ip")]
public class Ip
{
    /// <summary>
    /// Gets or sets the frequency.
    /// </summary>
    /// <value>
    /// The frequency.
    /// </value>
    [JsonPropertyName("frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Gets or sets the appears string.
    /// </summary>
    /// <value>
    /// The appears string.
    /// </value>
    [JsonPropertyName("appears")]
    public int AppearsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [appears].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
    /// </value>
    [JsonIgnore]
    public bool Appears
    {
        get => this.AppearsString == 1;

        set => this.AppearsString = value ? 1 : 0;
    }
}