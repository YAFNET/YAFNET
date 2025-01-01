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

using System.Collections.Generic;

using YAF.Types.Models;

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class SubscriptionsInputModel
{
    /// <summary>
    /// Gets or sets the notification type.
    /// </summary>
    /// <value>The type of the notification.</value>
    public string NotificationType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether daily digest enabled.
    /// </summary>
    /// <value><c>true</c> if [daily digest enabled]; otherwise, <c>false</c>.</value>
    public bool DailyDigestEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether show subscribe list.
    /// </summary>
    /// <value><c>true</c> if [show subscribe list]; otherwise, <c>false</c>.</value>
    public bool ShowSubscribeList { get; set; }

    /// <summary>
    /// Gets or sets the forums.
    /// </summary>
    /// <value>The forums.</value>
    public List<WatchForum> Forums { get; set; }

    /// <summary>
    /// Gets or sets the topics.
    /// </summary>
    /// <value>The topics.</value>
    public List<WatchTopic> Topics { get; set; }
}