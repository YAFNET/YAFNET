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

using System;

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;

namespace YAF.UI.Chat;

/// <summary>
/// A class which represents the Private Message Table.
/// </summary>
[Serializable]
public class ChatMessage : IEntity, IHaveID
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [Alias("PMessageID")]
    [AutoIncrement]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the board id.
    /// </summary>
    [Required]
    public int BoardId { get; set; }

    /// <summary>
    /// Gets or sets the created.
    /// </summary>
    [Required]
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    [CustomField(OrmLiteVariables.MaxTextUnicode)]
    [Required]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the avatar URL.
    /// </summary>
    /// <value>The avatar URL.</value>
    [Ignore]
    public string AvatarUrl { get; set; }

    /// <summary>
    /// Gets or sets the date time.
    /// </summary>
    /// <value>The date time.</value>
    [Ignore]
    public string DateTime =>
        BoardContext.Current.Get<BoardSettings>().ShowRelativeTime
            ? this.Created.ToRelativeTime()
            : BoardContext.Current.Get<IDateTimeService>().Format(DateTimeFormat.Both, this.Created);
}