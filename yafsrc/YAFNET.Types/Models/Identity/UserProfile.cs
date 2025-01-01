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

namespace YAF.Types.Models.Identity;

/// <summary>
/// The profile info.
/// </summary>
public class ProfileInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileInfo"/> class.
    /// </summary>
    public ProfileInfo()
    {
        this.Birthday = DateTime.MinValue.AddYears(1902);
    }

    /// <summary>
    /// Gets or sets Birthday.
    /// </summary>
    public DateTime Birthday { get; set; }

    /// <summary>
    /// Gets or sets Blog.
    /// </summary>
    public string Blog { get; set; }

    /// <summary>
    /// Gets or sets Gender.
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// Gets or sets Google Id
    /// </summary>
    public string GoogleId { get; set; }

    /// <summary>
    /// Gets or sets Homepage.
    /// </summary>
    public string Homepage { get; set; }

    /// <summary>
    /// Gets or sets Facebook.
    /// </summary>
   public string Facebook { get; set; }

    /// <summary>
    /// Gets or sets Facebook.
    /// </summary>
    public string FacebookId { get; set; }

    /// <summary>
    /// Gets or sets Interests.
    /// </summary>
    public string Interests { get; set; }

    /// <summary>
    /// Gets or sets Location.
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets Country.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets Region or State(US).
    /// </summary>
    public string Region { get; set; }

    /// <summary>
    /// Gets or sets a City.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets Occupation.
    /// </summary>
    public string Occupation { get; set; }

    /// <summary>
    /// Gets or sets RealName.
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// Gets or sets Skype.
    /// </summary>
    public string Skype { get; set; }

    /// <summary>
    /// Gets or sets XMPP.
    /// </summary>
    public string XMPP { get; set; }
}