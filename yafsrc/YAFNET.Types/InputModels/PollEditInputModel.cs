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
public class PollEditInputModel
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is closed bound CheckBox.
    /// </summary>
    /// <value><c>true</c> if this instance is closed bound CheckBox; otherwise, <c>false</c>.</value>
    public bool IsClosedBoundCheckBox { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow multiple choices CheckBox].
    /// </summary>
    /// <value><c>true</c> if [allow multiple choices CheckBox]; otherwise, <c>false</c>.</value>
    public bool AllowMultipleChoicesCheckBox { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show voters CheckBox].
    /// </summary>
    /// <value><c>true</c> if [show voters CheckBox]; otherwise, <c>false</c>.</value>
    public bool ShowVotersCheckBox { get; set; }

    /// <summary>
    /// Gets or sets the question.
    /// </summary>
    /// <value>The question.</value>
    [BindProperty, MaxLength(255)]
    public string Question { get; set; }

    /// <summary>
    /// Gets or sets the question object path.
    /// </summary>
    /// <value>The question object path.</value>
    [BindProperty, MaxLength(255)]
    public string QuestionObjectPath { get; set; }

    /// <summary>
    /// Gets or sets the poll expire.
    /// </summary>
    /// <value>The poll expire.</value>
    [BindProperty, MaxLength(10)]
    public string PollExpire { get; set; }

    /// <summary>
    /// Gets or sets the choices.
    /// </summary>
    /// <value>The choices.</value>
    public List<Choice> Choices { get; set; }
}