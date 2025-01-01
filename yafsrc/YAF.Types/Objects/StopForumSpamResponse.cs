/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

namespace YAF.Types.Objects;

using System.Runtime.Serialization;

/// <summary>
/// StopForumSpam.com JSON Response Class
/// </summary>
[DataContract]
public class StopForumSpamResponse
{
    /// <summary>
    /// Gets or sets the success string.
    /// </summary>
    /// <value>
    /// The success string.
    /// </value>
    [DataMember(Name = "success")]
    public string SuccessString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [success].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [success]; otherwise, <c>false</c>.
    /// </value>
    public bool Success
    {
        get => this.SuccessString == "1";

        set => this.SuccessString = value ? "1" : "0";
    }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    [DataMember(Name = "username")]
    public UserName UserName { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    [DataMember(Name = "email")]
    public Email Email { get; set; }

    /// <summary>
    /// Gets or sets the IP address.
    /// </summary>
    /// <value>
    /// The IP address.
    /// </value>
    [DataMember(Name = "ip")]
    public IP IPAddress { get; set; }
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
    [DataMember(Name = "frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Gets or sets the appears string.
    /// </summary>
    /// <value>
    /// The appears string.
    /// </value>
    [DataMember(Name = "appears")]
    public string AppearsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [appears].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
    /// </value>
    public bool Appears
    {
        get => this.AppearsString == "1";

        set => this.AppearsString = value ? "1" : "0";
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
    [DataMember(Name = "frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Gets or sets the appears string.
    /// </summary>
    /// <value>
    /// The appears string.
    /// </value>
    [DataMember(Name = "appears")]
    public string AppearsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [appears].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
    /// </value>
    public bool Appears
    {
        get => this.AppearsString == "1";

        set => this.AppearsString = value ? "1" : "0";
    }
}

/// <summary>
/// IP Address Namespace
/// </summary>
[DataContract(Namespace = "ip")]
public class IP
{
    /// <summary>
    /// Gets or sets the frequency.
    /// </summary>
    /// <value>
    /// The frequency.
    /// </value>
    [DataMember(Name = "frequency")]
    public int Frequency { get; set; }

    /// <summary>
    /// Gets or sets the appears string.
    /// </summary>
    /// <value>
    /// The appears string.
    /// </value>
    [DataMember(Name = "appears")]
    public string AppearsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [appears].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [appears]; otherwise, <c>false</c>.
    /// </value>
    public bool Appears
    {
        get => this.AppearsString == "1";

        set => this.AppearsString = value ? "1" : "0";
    }
}