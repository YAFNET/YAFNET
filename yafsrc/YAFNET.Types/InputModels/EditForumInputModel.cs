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

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class EditForumInputModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="EditForumInputModel"/> is copy.
    /// </summary>
    /// <value><c>true</c> if copy; otherwise, <c>false</c>.</value>
    public bool Copy { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    /// <value>The category identifier.</value>
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the parent identifier.
    /// </summary>
    /// <value>The parent identifier.</value>
    public int? ParentID { get; set; }

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
    /// Gets or sets the sort order.
    /// </summary>
    /// <value>The sort order.</value>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the remote URL.
    /// </summary>
    /// <value>The remote URL.</value>
    public string RemoteURL { get; set; }

    /// <summary>
    /// Gets or sets the theme URL.
    /// </summary>
    /// <value>The theme URL.</value>
    public string ThemeURL { get; set; }

    /// <summary>
    /// Gets or sets the image URL.
    /// </summary>
    /// <value>The image URL.</value>
    public string ImageURL { get; set; }

    /// <summary>
    /// Gets or sets the styles.
    /// </summary>
    /// <value>The styles.</value>
    public string Styles { get; set; }

    /// <summary>
    /// Gets or sets the moderated post count.
    /// </summary>
    /// <value>The moderated post count.</value>
    public int ModeratedPostCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether [moderate all posts].
    /// </summary>
    /// <value><c>true</c> if [moderate all posts]; otherwise, <c>false</c>.</value>
    public bool ModerateAllPosts { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [moderate new topic only].
    /// </summary>
    /// <value><c>true</c> if [moderate new topic only]; otherwise, <c>false</c>.</value>
    public bool ModerateNewTopicOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="EditForumInputModel"/> is moderated.
    /// </summary>
    /// <value><c>true</c> if moderated; otherwise, <c>false</c>.</value>
    public bool Moderated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is test.
    /// </summary>
    /// <value><c>true</c> if this instance is test; otherwise, <c>false</c>.</value>
    public bool IsTest { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="EditForumInputModel"/> is locked.
    /// </summary>
    /// <value><c>true</c> if locked; otherwise, <c>false</c>.</value>
    public bool Locked { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [hide no access].
    /// </summary>
    /// <value><c>true</c> if [hide no access]; otherwise, <c>false</c>.</value>
    public bool HideNoAccess { get; set; }
}