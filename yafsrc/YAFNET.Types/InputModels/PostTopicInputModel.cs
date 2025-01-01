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

using System.ComponentModel.DataAnnotations;

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class PostTopicInputModel
{
    /// <summary>
    /// Gets or sets the topic subject.
    /// </summary>
    /// <value>The topic subject.</value>
    [BindProperty, MaxLength(100)]
    public string TopicSubject { get; set; }

    /// <summary>
    /// Gets or sets the topic description.
    /// </summary>
    /// <value>The topic description.</value>
    public string TopicDescription { get; set; }

    /// <summary>
    /// Gets or sets from.
    /// </summary>
    /// <value>From.</value>
    public string From { get; set; }

    /// <summary>
    /// Gets or sets the topic styles.
    /// </summary>
    /// <value>The topic styles.</value>
    public string TopicStyles { get; set; }

    /// <summary>
    /// Gets or sets the tags value.
    /// </summary>
    /// <value>The tags value.</value>
    public string TagsValue { get; set; }

    /// <summary>
    /// Gets or sets the editor.
    /// </summary>
    /// <value>The editor.</value>
    public string Editor { get; set; }

    /// <summary>
    /// Gets or sets the preview message.
    /// </summary>
    /// <value>The preview message.</value>
    public string PreviewMessage { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [add poll].
    /// </summary>
    /// <value><c>true</c> if [add poll]; otherwise, <c>false</c>.</value>
    public bool AddPoll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PostTopicInputModel"/> is persistent.
    /// </summary>
    /// <value><c>true</c> if persistent; otherwise, <c>false</c>.</value>
    public bool Persistent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [topic watch].
    /// </summary>
    /// <value><c>true</c> if [topic watch]; otherwise, <c>false</c>.</value>
    public bool TopicWatch { get; set; }
}