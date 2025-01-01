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

namespace YAF.Types.Objects;

using System.Collections;
using System.Xml.Serialization;

using YAF.Types.Constants;
using YAF.Types.Extensions;

/// <summary>
/// The site Map. XML
/// </summary>
[XmlRoot("urlset", Namespace = "https://www.sitemaps.org/schemas/sitemap/0.9")]
public class SiteMap
{
    /// <summary>
    /// The map.
    /// </summary>
    private readonly ArrayList map;

    /// <summary>
    /// Initializes a new instance of the <see cref="SiteMap"/> class.
    /// </summary>
    public SiteMap()
    {
        this.map = [];
    }

    /// <summary>
    /// Gets or sets the locations.
    /// </summary>
    [XmlElement("url")]
    public UrlLocation[] Locations
    {
        get
        {
            var items = new UrlLocation[this.map.Count];
            this.map.CopyTo(items);
            return items;
        }

        set
        {
            if (value is null)
            {
                return;
            }

            var items = value;
            this.map.Clear();

            items.ForEach(item => this.map.Add(item));
        }
    }

    /// <summary>
    /// Add Url to List
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public int Add(UrlLocation item)
    {
        return this.map.Add(item);
    }
}