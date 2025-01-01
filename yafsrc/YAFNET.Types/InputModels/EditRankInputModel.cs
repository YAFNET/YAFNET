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

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class EditRankInputModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is start.
    /// </summary>
    /// <value><c>true</c> if this instance is start; otherwise, <c>false</c>.</value>
    public bool IsStart { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is ladder.
    /// </summary>
    /// <value><c>true</c> if this instance is ladder; otherwise, <c>false</c>.</value>
    public bool IsLadder { get; set; }

    /// <summary>
    /// Gets or sets the rank priority.
    /// </summary>
    /// <value>The rank priority.</value>
    public short RankPriority { get; set; }

    /// <summary>
    /// Gets or sets the minimum posts.
    /// </summary>
    /// <value>The minimum posts.</value>
    public int MinPosts { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the usr sig chars.
    /// </summary>
    /// <value>The usr sig chars.</value>
    public int UsrSigChars { get; set; } = 128;

    /// <summary>
    /// Gets or sets the usr sig bb codes.
    /// </summary>
    /// <value>The usr sig bb codes.</value>
    public string UsrSigBBCodes { get; set; }

    /// <summary>
    /// Gets or sets the usr albums.
    /// </summary>
    /// <value>The usr albums.</value>
    public int UsrAlbums { get; set; }

    /// <summary>
    /// Gets or sets the usr album images.
    /// </summary>
    /// <value>The usr album images.</value>
    public int UsrAlbumImages { get; set; }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    /// <value>The style.</value>
    public string Style { get; set; }
}