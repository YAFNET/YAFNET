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

namespace YAF.Types.Objects;

/// <summary>
/// The Attachment Item
/// </summary>
public class AttachmentItem
{
    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>
    /// The name of the file.
    /// </value>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the on click.
    /// </summary>
    /// <value>
    /// The on click.
    /// </value>
    public string OnClick { get; set; }

    /// <summary>
    /// Gets or sets the data URL.
    /// </summary>
    /// <value>
    /// The data URL.
    /// </value>
    public string DataURL { get; set; }

    /// <summary>
    /// Gets or sets the icon image.
    /// </summary>
    /// <value>
    /// The icon image.
    /// </value>
    public string IconImage { get; set; }
}