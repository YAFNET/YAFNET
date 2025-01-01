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
public class EditGroupInputModel
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
    /// Gets or sets a value indicating whether this instance is guest x.
    /// </summary>
    /// <value><c>true</c> if this instance is guest x; otherwise, <c>false</c>.</value>
    public bool IsGuestX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is start x.
    /// </summary>
    /// <value><c>true</c> if this instance is start x; otherwise, <c>false</c>.</value>
    public bool IsStartX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is moderator x.
    /// </summary>
    /// <value><c>true</c> if this instance is moderator x; otherwise, <c>false</c>.</value>
    public bool IsModeratorX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is admin x.
    /// </summary>
    /// <value><c>true</c> if this instance is admin x; otherwise, <c>false</c>.</value>
    public bool IsAdminX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [upload access].
    /// </summary>
    /// <value><c>true</c> if [upload access]; otherwise, <c>false</c>.</value>
    public bool UploadAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [download access].
    /// </summary>
    /// <value><c>true</c> if [download access]; otherwise, <c>false</c>.</value>
    public bool DownloadAccess { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
    public short Priority { get; set; }

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

    /// <summary>
    /// Creates new accessmaskid.
    /// </summary>
    /// <value>The new access mask identifier.</value>
    public int NewAccessMaskID { get; set; }
}