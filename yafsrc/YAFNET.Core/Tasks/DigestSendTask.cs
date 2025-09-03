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

namespace YAF.Core.Tasks;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Types.Models;

/// <summary>
/// The digest send task.
/// </summary>
public class DigestSendTask : LongBackgroundTask
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "DigestSendTask" /> class.
    /// </summary>
    public DigestSendTask()
    {
        this.RunPeriodMs = 300 * 1000;
        this.StartDelayMs = 30 * 1000;
    }

    /// <summary>
    ///   Gets TaskName.
    /// </summary>
    public static string TaskName => "DigestSendTask";

    /// <summary>
    /// The run once.
    /// </summary>
    public override Task RunOnceAsync()
    {
        return this.SendDigestAsync();
    }

    /// <summary>
    /// Determines whether is time to send digest for board.
    /// </summary>
    /// <param name="boardSettings">The board settings.</param>
    /// <returns>
    /// The is time to send digest for board.
    /// </returns>
    private static bool IsTimeToSendDigestForBoard(BoardSettings boardSettings)
    {
        ArgumentNullException.ThrowIfNull(boardSettings);

        if (!boardSettings.AllowDigestEmail)
        {
            return false;
        }

        var lastSend = DateTime.MinValue;
        var sendEveryXHours = boardSettings.DigestSendEveryXHours;

        if (boardSettings.LastDigestSend.IsSet())
        {
            try
            {
                lastSend = Convert.ToDateTime(boardSettings.LastDigestSend, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                lastSend = DateTime.MinValue;
            }
        }

#if (DEBUG)
        // haven't sent in X hours or more and it's 12 to 5 am.
        var sendDigest = lastSend < DateTime.Now.AddHours(-sendEveryXHours);
#else
        // haven't sent in X hours or more and it's 12 to 5 am.
        var sendDigest = lastSend < DateTime.Now.AddHours(-sendEveryXHours)
                         && DateTime.Now < DateTime.Today.AddHours(6);
#endif
        if (!sendDigest && !boardSettings.ForceDigestSend)
        {
            return false;
        }

        // we're good to send -- update latest send so no duplication...
        boardSettings.LastDigestSend = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        boardSettings.ForceDigestSend = false;

        BoardContext.Current.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        return true;
    }

    /// <summary>
    /// The send digest.
    /// </summary>
    private async Task SendDigestAsync()
    {
        try
        {
            var boards = await this.GetRepository<Board>().GetAllAsync();

            foreach (var board in boards)
            {
                var boardSettings = this.Get<BoardSettingsService>().LoadBoardSettings(board.ID, board);

                if (!IsTimeToSendDigestForBoard(boardSettings))
                {
                    return;
                }

                // get users with digest enabled...
                var usersWithDigest = await this.GetRepository<User>().GetAsync(
                    u => u.BoardID == board.ID && (u.Flags & 2) == 2 && (u.Flags & 4) != 4
                         && (u.Flags & 32) != 32 && u.DailyDigest);

                if (usersWithDigest.HasItems())
                {
                    // start sending...
                    await this.SendDigestToUsersAsync(usersWithDigest, boardSettings);
                }
                else
                {
                    this.Get<ILogger<DigestSendTask>>().Info("no user found");
                }
            }
        }
        catch (Exception ex)
        {
            this.Get<ILogger<DigestSendTask>>().Error(ex, $"Error In {TaskName} Task");
        }
    }

    /// <summary>
    /// Sends the digest to users.
    /// </summary>
    /// <param name="usersWithDigest">The users with digest.</param>
    /// <param name="boardSettings">The board settings.</param>
    private async Task SendDigestToUsersAsync(IEnumerable<User> usersWithDigest, BoardSettings boardSettings)
    {
        var mailMessages = new List<MimeMessage>();

        var boardEmail = new MailboxAddress(boardSettings.Name, boardSettings.ForumEmail);

        foreach (var user in usersWithDigest)
        {
            try
            {
                if (user.ProviderUserKey == null)
                {
                    return;
                }

                var digestMail = this.Get<IDigestService>().CreateDigest(
                    user,
                    boardEmail,
                    user.Email,
                    user.DisplayOrUserName());

                mailMessages.Add(digestMail);
            }
            catch (Exception e)
            {
                this.Get<ILogger<DigestSendTask>>().Error(e, $"Error In Creating Digest for PageUser {user.ID}");
            }
        }

        await this.Get<IMailService>().SendAllAsync(mailMessages);

        this.Get<ILogger<DigestSendTask>>().Info($"Digest send to {mailMessages.Count} user(s)");
    }
}