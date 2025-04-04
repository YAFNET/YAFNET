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

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using YAF.Types.Models;

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class EditProfileInputModel
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>The display name.</value>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the name of the real.
    /// </summary>
    /// <value>The name of the real.</value>
    public string RealName { get; set; }

    /// <summary>
    /// Gets or sets the birthday.
    /// </summary>
    /// <value>The birthday.</value>
    [DataType(DataType.Date)]
    public string Birthday { get; set; }

    /// <summary>
    /// Gets or sets the occupation.
    /// </summary>
    /// <value>The occupation.</value>
    [BindProperty, MaxLength(400)]
    public string Occupation { get; set; }

    /// <summary>
    /// Gets or sets the interests.
    /// </summary>
    /// <value>The interests.</value>
    [BindProperty, MaxLength(4000)]
    public string Interests { get; set; }

    /// <summary>
    /// Gets or sets the gender.
    /// </summary>
    /// <value>The gender.</value>
    public string Gender { get; set; }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>The location.</value>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    /// <value>The country.</value>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    /// <value>The region.</value>
    public string Region { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    /// <value>The city.</value>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the home page.
    /// </summary>
    /// <value>The home page.</value>
    [DataType(DataType.Url)]
    public string HomePage { get; set; }

    /// <summary>
    /// Gets or sets the blog.
    /// </summary>
    /// <value>The blog.</value>
    [DataType(DataType.Url)]
    public string Blog { get; set; }

    /// <summary>
    /// Gets or sets the XMPP.
    /// </summary>
    /// <value>The XMPP.</value>
    public string Xmpp { get; set; }

    /// <summary>
    /// Gets or sets the facebook.
    /// </summary>
    /// <value>The facebook.</value>
    public string Facebook { get; set; }

    /// <summary>
    /// Gets or sets the custom profile.
    /// </summary>
    /// <value>The custom profile.</value>
    public List<ProfileDefinition> CustomProfile { get; set; }
}