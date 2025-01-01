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

using System.ComponentModel.DataAnnotations;

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class MailInputModel
{
    /// <summary>
    /// Gets or sets the test from email.
    /// </summary>
    /// <value>The test from email.</value>
    [Required]
    public string TestFromEmail { get; set; }

    /// <summary>
    /// Gets or sets the test to email.
    /// </summary>
    /// <value>The test to email.</value>
    [Required]
    public string TestToEmail { get; set; }

    /// <summary>
    /// Converts to listitem.
    /// </summary>
    /// <value>To list item.</value>
    public int ToListItem { get; set; }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>The subject.</value>
    [Required]
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    /// <value>The body.</value>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the test subject.
    /// </summary>
    /// <value>The test subject.</value>
    [Required]
    public string TestSubject { get; set; }

    /// <summary>
    /// Gets or sets the test body.
    /// </summary>
    /// <value>The test body.</value>
    [Required]
    public string TestBody { get; set; }
}