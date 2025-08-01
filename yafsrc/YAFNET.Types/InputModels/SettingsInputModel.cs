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

namespace YAF.Types.InputModels;

/// <summary>
/// Class SettingsModel.
/// </summary>
public class SettingsInputModel
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the forum email.
    /// </summary>
    /// <value>The forum email.</value>
    public string ForumEmail { get; set; }

    /// <summary>
    /// Gets or sets the board logo.
    /// </summary>
    /// <value>The board logo.</value>
    public string BoardLogo { get; set; }

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    /// <value>The theme.</value>
    public string Theme { get; set; }

    /// <summary>
    /// Gets or sets the show topic.
    /// </summary>
    /// <value>The show topic.</value>
    public int ShowTopic { get; set; }

    /// <summary>
    /// Gets or sets the culture.
    /// </summary>
    /// <value>The culture.</value>
    public string Culture { get; set; }

    /// <summary>
    /// Gets or sets the default notification setting.
    /// </summary>
    /// <value>The default notification setting.</value>
    public int DefaultNotificationSetting { get; set; }

    /// <summary>
    /// Gets or sets the default state of the collapsible panel.
    /// </summary>
    /// <value>The default state of the collapsible panel.</value>
    public int DefaultCollapsiblePanelState { get; set; }

    /// <summary>
    /// Gets or sets the default size of the page.
    /// </summary>
    /// <value>The default size of the page.</value>
    public int DefaultPageSize { get; set; }

    /// <summary>
    /// Gets or sets the notification on user register email list.
    /// </summary>
    /// <value>The notification on user register email list.</value>
    public string NotificationOnUserRegisterEmailList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [email moderators on moderated post].
    /// </summary>
    /// <value><c>true</c> if [email moderators on moderated post]; otherwise, <c>false</c>.</value>
    public bool EmailModeratorsOnModeratedPost { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [email moderators on reported post].
    /// </summary>
    /// <value><c>true</c> if [email moderators on reported post]; otherwise, <c>false</c>.</value>
    public bool EmailModeratorsOnReportedPost { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow digest email].
    /// </summary>
    /// <value><c>true</c> if [allow digest email]; otherwise, <c>false</c>.</value>
    public bool AllowDigestEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [default send digest email].
    /// </summary>
    /// <value><c>true</c> if [default send digest email]; otherwise, <c>false</c>.</value>
    public bool DefaultSendDigestEmail { get; set; }

    /// <summary>
    /// Gets or sets the digest send every x hours.
    /// </summary>
    /// <value>The digest send every x hours.</value>
    public int DigestSendEveryXHours { get; set; }

    /// <summary>
    /// Gets or sets the forum default access mask.
    /// </summary>
    /// <value>The forum default access mask.</value>
    public int ForumDefaultAccessMask { get; set; }
}