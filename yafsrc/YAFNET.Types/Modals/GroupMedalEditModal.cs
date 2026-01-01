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

using Microsoft.AspNetCore.Mvc.Rendering;

namespace YAF.Types.Modals;

/// <summary>
/// Class GroupMedalEditModal.
/// </summary>
public class GroupMedalEditModal
{
    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    /// <value>The group identifier.</value>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    /// <value>The name of the group.</value>
    public string GroupName { get; set; }

    /// <summary>
    /// Gets or sets the medal identifier.
    /// </summary>
    /// <value>The medal identifier.</value>
    public int MedalId { get; set; }

    /// <summary>
    /// Gets or sets the group message.
    /// </summary>
    /// <value>The group message.</value>
    public string GroupMessage { get; set; }

    /// <summary>
    /// Gets or sets the group sort order.
    /// </summary>
    /// <value>The group sort order.</value>
    public int GroupSortOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [group hide].
    /// </summary>
    /// <value><c>true</c> if [group hide]; otherwise, <c>false</c>.</value>
    public bool GroupHide { get; set; }

    /// <summary>
    /// Gets or sets the group list.
    /// </summary>
    /// <value>The group list.</value>
    public SelectList GroupList { get; set; }
}