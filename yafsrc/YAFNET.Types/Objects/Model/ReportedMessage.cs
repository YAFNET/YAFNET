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

namespace YAF.Types.Objects.Model;

/// <summary>
/// The reported message.
/// </summary>
public class ReportedMessage
{
    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    /// <value>The message identifier.</value>
    public int MessageID { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the resolved by.
    /// </summary>
    public int? ResolvedBy { get; set; }

    /// <summary>
    /// Gets or sets the resolved date.
    /// </summary>
    public DateTime? ResolvedDate { get; set; }

    /// <summary>
    /// Gets or sets the resolved.
    /// </summary>
    public bool? Resolved { get; set; }

    /// <summary>
    /// Gets or sets the original message.
    /// </summary>
    /// <value>The original message.</value>
    public string OriginalMessage { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>The flags.</value>
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is moderator changed.
    /// </summary>
    /// <value><c>null</c> if [is moderator changed] contains no value, <c>true</c> if [is moderator changed]; otherwise, <c>false</c>.</value>
    public bool? IsModeratorChanged { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    /// <value>The display name of the user.</value>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the number of reports.
    /// </summary>
    /// <value>The number of reports.</value>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    /// <value>The suspended.</value>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    /// <value>The user style.</value>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the posted.
    /// </summary>
    /// <value>The posted.</value>
    public DateTime Posted { get; set; }

    /// <summary>
    /// Gets or sets the topic identifier.
    /// </summary>
    /// <value>The topic identifier.</value>
    public int TopicID { get; set; }

    /// <summary>
    /// Gets or sets the name of the topic.
    /// </summary>
    /// <value>The name of the topic.</value>
    public string TopicName { get; set; }

    /// <summary>
    /// Gets or sets the number of reports.
    /// </summary>
    /// <value>The number of reports.</value>
    public int NumberOfReports { get; set; }
}