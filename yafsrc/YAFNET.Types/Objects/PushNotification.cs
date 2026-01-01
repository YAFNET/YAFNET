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

using Newtonsoft.Json;

namespace YAF.Types.Objects;

/// <summary>
/// Browser Push Notification Object
/// </summary>
public class PushNotification
{
    /// <summary>
    /// A string representing the body text of the notification, which is displayed below the title. The default is the empty string.
    /// </summary>
    [JsonProperty("body")]
    public string Body { get; set; }

    /// <summary>
    /// Defines a title for the notification, which is shown at the top of the notification window.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    /// A string containing the URL of an image to be displayed in the notification.
    /// </summary>
    [JsonProperty("image")]
    public string Image { get; set; }

    /// <summary>
    /// A string containing the URL of an icon to be displayed in the notification.
    /// </summary>
    [JsonProperty("icon")]
    public string Icon { get; set; }

    /// <summary>
    /// A string containing the URL of the image used to represent the notification when there isn't enough space to display the notification itself; for example, the Android Notification Bar.
    /// On Android devices, the badge should accommodate devices up to 4x resolution, about 96x96px, and the image will be automatically masked.
    /// </summary>
    [JsonProperty("badge")]
    public string Badge { get; set; }

    /// <summary>
    /// Gets or sets the unread messages count.
    /// </summary>
    /// <value>
    /// The unread messages count.
    /// </value>
    [JsonProperty("unreadCount")]
    public int UnreadCount { get; set; }

    /// <summary>
    /// Must be unspecified or an empty array. actions is only supported for persistent notifications fired from a service worker
    /// </summary>
    [JsonProperty("data")]
    public PushNotificationAction Actions { get; set; }
}

/// <summary>
/// Must be unspecified or an empty array. actions is only supported for persistent notifications fired from a service worker
/// </summary>
public class PushNotificationAction
{
    /// <summary>
    /// A string containing the URL of an icon to display with the action.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// A string identifying a user action to be displayed on the notification.
    /// </summary>
    [JsonProperty("action")]
    public string Action { get; set; }

    /// <summary>
    /// A string containing action text to be shown to the user.
    /// </summary>
    [JsonProperty("close")]
    public string Close { get; set; }
}