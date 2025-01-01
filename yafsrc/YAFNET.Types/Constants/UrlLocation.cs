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

namespace YAF.Types.Constants;

using System.Xml.Serialization;

/// <summary>
/// The URL location.
/// </summary>
public class UrlLocation
{
    /// <summary>
    /// The change frequencies.
    /// </summary>
    public enum ChangeFrequencies
    {
        /// <summary>
        /// The always.
        /// </summary>
        always,

        /// <summary>
        /// The hourly.
        /// </summary>
        hourly,

        /// <summary>
        /// The daily.
        /// </summary>
        daily,

        /// <summary>
        /// The weekly.
        /// </summary>
        weekly,

        /// <summary>
        /// The monthly.
        /// </summary>
        monthly,

        /// <summary>
        /// The yearly.
        /// </summary>
        yearly,

        /// <summary>
        /// The never.
        /// </summary>
        never
    }

    /// <summary>
    /// Gets or sets the url.
    /// </summary>
    [XmlElement("loc")]
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the change frequency.
    /// </summary>
    [XmlElement("changefreq")]
    public ChangeFrequencies? ChangeFrequency { get; set; }

    /// <summary>
    /// Gets or sets the last modified.
    /// </summary>
    [XmlElement("lastmod")]
    public string LastModified { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    [XmlElement("priority")]
    public double? Priority { get; set; }

    /// <summary>
    /// The should serialize priority.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool ShouldSerializePriority()
    {
        return this.Priority.HasValue;
    }

    /// <summary>
    /// The should serialize change frequency.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool ShouldSerializeChangeFrequency()
    {
        return this.ChangeFrequency.HasValue;
    }
}