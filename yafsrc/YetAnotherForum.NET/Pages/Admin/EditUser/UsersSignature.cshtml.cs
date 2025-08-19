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

using System.Threading.Tasks;

namespace YAF.Pages.Admin.EditUser;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using YAF.Core.Context;
using YAF.Core.Model;
using YAF.Pages.Profile;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersSignatureModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersSignatureModel : AdminPage
{
    /// <summary>
    /// Gets or sets the signature.
    /// </summary>
    [BindProperty]
    public string Signature { get; set; }

    /// <summary>
    ///   Gets or sets the number of characters which is allowed in user signature.
    /// </summary>
    [BindProperty]
    public int AllowedNumberOfCharacters { get; set; }

    /// <summary>
    ///   Gets or sets the string with allowed BBCodes info.
    /// </summary>
    [BindProperty]
    public string AllowedBbcodes { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersSignatureInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersSignatureModel"/> class.
    /// </summary>
    public UsersSignatureModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnGetAsync(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input = new UsersSignatureInputModel {
            UserId = userId
        };

        return await this.BindDataAsync(userId, true);
    }

    /// <summary>
    /// Preview Signature
    /// </summary>
    public IActionResult OnPostPreview()
    {
        this.ValidateSignature();

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new {u = this.Input.UserId, tab = "View5"});
    }

    /// <summary>
    /// Save the Signature.
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] is not
            Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = this.Input.UserId
                });
        }

        if (this.Signature.Length > 0)
        {
            if (this.ValidateSignature())
            {
                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new {u = this.Input.UserId, tab = "View5"});
            }

            if (this.Signature.Length <= this.AllowedNumberOfCharacters)
            {
                if (user.Item1.NumPosts <
                    this.PageBoardContext.BoardSettings.IgnoreSpamWordCheckPostCount)
                {
                    // Check for spam
                    if (this.Get<ISpamWordCheck>().CheckForSpamWord(this.Signature, out var result))
                    {
                        switch (this.PageBoardContext.BoardSettings.BotHandlingOnRegister)
                        {
                            // Log and Send Message to Admins
                            case 1:
                                this.Get<ILogger<EditSignatureModel>>().SpamBotDetected(
                                    this.Input.UserId,
                                    $"""
                                     Internal Spam Word Check detected a SPAM BOT: (user name : '{user.Item1.Name}', user id : '{this.Input.UserId}') 
                                     after the user included a spam word in his/her signature: {result}
                                     """);
                                break;
                            case 2:
                            {
                                this.Get<ILogger<EditSignatureModel>>().SpamBotDetected(
                                    this.Input.UserId,
                                    $"""
                                     Internal Spam Word Check detected a SPAM BOT: (
                                                                                            user name : '{user.Item1.Name}',
                                                                                            user id : '{this.Input.UserId}')
                                                                                      after the user included a spam word in his/her signature: {result}, user was deleted and the name, email and IP Address are banned.
                                     """);

                                break;
                            }
                        }
                    }
                }

                await this.GetRepository<User>().SaveSignatureAsync(
                    this.Input.UserId,
                    this.Signature);
            }
            else
            {
                this.PageBoardContext.SessionNotify(
                    this.GetTextFormatted("SIGNATURE_MAX", this.AllowedNumberOfCharacters),
                    MessageTypes.warning);
            }
        }
        else
        {
            await this.GetRepository<User>().SaveSignatureAsync(this.Input.UserId, null);
        }

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Admin_EditUser,
            new { u = this.Input.UserId, tab = "View5" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private async Task<IActionResult> BindDataAsync(int userId, bool loadSignature)
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] is not Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = userId
                });
        }

        if (loadSignature)
        {
            this.Signature = user.Item1.Signature;
        }

        var data = await this.GetRepository<User>().SignatureDataAsync(
            userId,
            this.PageBoardContext.PageBoardID);

        if (data is null)
        {
            return this.Page();
        }

        this.AllowedBbcodes = (string)data.UsrSigBBCodes.Trim().Trim(',').Trim();

        this.AllowedNumberOfCharacters = (int)data.UsrSigChars;

        return this.Page();
    }

    /// <summary>
    /// The validate signature.
    /// </summary>
    private bool ValidateSignature()
    {
        var body = this.Signature;

        // find forbidden BBCodes in signature
        var detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this.AllowedBbcodes, ',');

        if (this.AllowedBbcodes.IndexOf("ALL", StringComparison.Ordinal) < 0)
        {
            if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
            {
                this.PageBoardContext.SessionNotify(
                    this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode),
                    MessageTypes.warning);

                return false;
            }

            if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
            {
                this.PageBoardContext.SessionNotify(this.GetText("BBCODE_FORBIDDEN"), MessageTypes.warning);

                return false;
            }
        }

        if (this.Signature.Length <= this.AllowedNumberOfCharacters)
        {
            this.Signature = this.Get<IBadWordReplace>().Replace(body);

            return true;
        }

        this.PageBoardContext.SessionNotify(
            this.GetTextFormatted("SIGNATURE_MAX", this.AllowedNumberOfCharacters),
            MessageTypes.warning);

        return false;
    }
}