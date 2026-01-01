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

namespace YAF.Types.Objects.Model;

/// <summary>
/// The paged event log.
/// </summary>
public class PagedEventLog
{
   /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the event time.
    /// </summary>
    public DateTime EventTime { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>The flags.</value>
    public int Flags { get; set; }

    /// <summary>
    /// Gets the user flags.
    /// </summary>
    /// <value>The user flags.</value>
    public UserFlags UserFlags {
        get => new(this.Flags);
    }

    /// <summary>
    /// Gets or sets the total rows.
    /// </summary>
    public int TotalRows { get; set; }
}