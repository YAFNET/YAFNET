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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using YAF.Core.Extensions;
using YAF.Types.Extensions;

/// <summary>
/// Information control displaying feedback information to users.
/// </summary>
public class InfoModel : ForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfoModel"/> class.
    /// </summary>
    public InfoModel()
        : base("INFO", ForumPages.Info)
    {
        this.PageBoardContext.Globals.IsSuspendCheckEnabled = false;
    }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the info label.
    /// </summary>
    public string InfoLabel { get; private set; }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="info">
    /// The info.
    /// </param>
    /// <param name="message">
    /// The info message
    /// </param>
    public IActionResult OnGet(string info = null, string message = null)
    {
        // try to get info message code from parameter
        try
        {
            // compare it converted to enumeration
            switch ((InfoMessage)info.ToType<int>())
            {
                case InfoMessage.Moderated: // Moderated
                    this.Title = this.GetText("title_moderated");
                    this.InfoLabel = this.GetText("moderated");
                    break;
                case InfoMessage.Suspended: // Suspended
                    this.Title = this.GetText("title_suspended");

                    if (this.PageBoardContext.SuspendedReason.IsSet())
                    {
                        this.InfoLabel =
                            $"{this.GetTextFormatted("SUSPENDED", this.Get<IDateTimeService>().GetUserDateTime(this.PageBoardContext.SuspendedUntil))}{this.GetTextFormatted("SUSPENDED_REASON", this.PageBoardContext.SuspendedReason)}";
                    }
                    else
                    {
                        this.InfoLabel = this.GetTextFormatted(
                            "SUSPENDED",
                            this.Get<IDateTimeService>().GetUserDateTime(this.PageBoardContext.SuspendedUntil));
                    }

                    break;
                case InfoMessage.RegistrationEmail: // Registration email
                    this.Title = this.GetText("title_registration");
                    this.InfoLabel = this.GetText("registration");
                    break;
                case InfoMessage.AccessDenied: // Access Denied
                    this.Title = this.GetText("title_accessdenied");
                    this.InfoLabel = this.GetText("accessdenied");
                    break;
                case InfoMessage.Disabled: // Disabled feature
                    this.Title = this.GetText("TITLE_ACCESSDENIED");
                    this.InfoLabel = this.GetText("DISABLED");
                    break;
                case InfoMessage.Invalid: // Invalid argument!
                    this.Title = this.GetText("TITLE_INVALID");
                    this.InfoLabel = this.GetText("INVALID");
                    break;
                case InfoMessage.Failure: // some sort of failure
                    this.Title = this.GetText("TITLE_FAILURE");
                    this.InfoLabel = this.GetText("FAILURE");
                    break;
                case InfoMessage.HostAdminPermissionsAreRequired: // some sort of failure
                    this.Title = this.GetText("TITLE_HOSTADMINPERMISSIONSREQUIRED");
                    this.InfoLabel = this.GetText("HOSTADMINPERMISSIONSREQUIRED");
                    break;
            }

            // information title text
            this.PageBoardContext.PageLinks.AddLink(this.Title);
        }
        catch (Exception)
        {
            // get title for exception message
            this.Title = this.GetText("title_exception");

            // exception message
            this.InfoLabel =
                $"{this.GetText("exception")} <strong>{this.PageBoardContext.PageUser.DisplayOrUserName()}</strong>.";
        }

        if (message.IsSet())
        {
            this.InfoLabel = message;
        }

        return this.Page();
    }
}