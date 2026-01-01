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
public class EditAccessMaskInputModel
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
    /// Gets or sets the sort order.
    /// </summary>
    /// <value>The sort order.</value>
    public short SortOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [read access].
    /// </summary>
    /// <value><c>true</c> if [read access]; otherwise, <c>false</c>.</value>
    public bool ReadAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [post access].
    /// </summary>
    /// <value><c>true</c> if [post access]; otherwise, <c>false</c>.</value>
    public bool PostAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [reply access].
    /// </summary>
    /// <value><c>true</c> if [reply access]; otherwise, <c>false</c>.</value>
    public bool ReplyAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [priority access].
    /// </summary>
    /// <value><c>true</c> if [priority access]; otherwise, <c>false</c>.</value>
    public bool PriorityAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [poll access].
    /// </summary>
    /// <value><c>true</c> if [poll access]; otherwise, <c>false</c>.</value>
    public bool PollAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [vote access].
    /// </summary>
    /// <value><c>true</c> if [vote access]; otherwise, <c>false</c>.</value>
    public bool VoteAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [moderator access].
    /// </summary>
    /// <value><c>true</c> if [moderator access]; otherwise, <c>false</c>.</value>
    public bool ModeratorAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [edit access].
    /// </summary>
    /// <value><c>true</c> if [edit access]; otherwise, <c>false</c>.</value>
    public bool EditAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [delete access].
    /// </summary>
    /// <value><c>true</c> if [delete access]; otherwise, <c>false</c>.</value>
    public bool DeleteAccess { get; set; }
}