/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2023 Ingo Herbote
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
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Types.Attributes;
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
    public static string TaskName { get; } = "DigestSendTask";

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
        this.SendDigest();
    }

    /// <summary>
    /// Determines whether is time to send digest for board.
    /// </summary>
    /// <param name="boardSettings">The board settings.</param>
    /// <returns>
    /// The is time to send digest for board.
    /// </returns>
    private static bool IsTimeToSendDigestForBoard([NotNull] BoardSettings boardSettings)
    {
        CodeContracts.VerifyNotNull(boardSettings);

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

        // && DateTime.Now < DateTime.Today.AddHours(5))
        // we're good to send -- update latest send so no duplication...
        boardSettings.LastDigestSend = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        boardSettings.ForceDigestSend = false;

        BoardContext.Current.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        return true;
    }

    /// <summary>
    /// The send digest.
    /// </summary>
    private void SendDigest()
    {
        try
        {
            var boards = this.GetRepository<Board>().GetAll();

            boards.ForEach(
                board =>
                    {
                        var boardSettings = this.Get<BoardSettingsService>().LoadBoardSettings(board.ID, board);

                        if (!IsTimeToSendDigestForBoard(boardSettings))
                        {
                            return;
                        }

                        // get users with digest enabled...
                        var usersWithDigest = this.GetRepository<User>().Get(
                            u => u.BoardID == board.ID && (u.Flags & 2) == 2 && (u.Flags & 4) != 4
                                 && (u.Flags & 32) != 32 && u.DailyDigest);

                        if (usersWithDigest.Any())
                        {
                            // start sending...
                            this.SendDigestToUsers(usersWithDigest, boardSettings);
                        }
                        else
                        {
                            this.Get<ILogger<DigestSendTask>>().Info("no user found");
                        }
                    });
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
    private void SendDigestToUsers(IEnumerable<User> usersWithDigest, BoardSettings boardSettings)
    {
        var mailMessages = new List<MimeMessage>();

        var boardEmail = new MailboxAddress(boardSettings.Name, boardSettings.ForumEmail);

        usersWithDigest.AsParallel().ForAll(
            user =>
                {
                    try
                    {
                        var url = this.Get<IDigestService>().GetDigestUrl(user.ID, boardSettings, false);

                        var digestHtml = this.Get<IDigestService>().GetDigestHtmlAsync(url).Result;

                        if (digestHtml.IsNotSet())
                        {
                            return;
                        }

                        if (user.ProviderUserKey == null)
                        {
                            return;
                        }

                        var subject = Regex.Match(digestHtml, "<title>(.*?)</title>", RegexOptions.Singleline)
                            .Groups[1].Value.Trim();

                        // send the digest...
                        mailMessages.Add(this.Get<IDigestService>().CreateDigestMessage(
                            subject.Trim(),
                            digestHtml,
                            boardEmail,
                            user.Email,
                            user.DisplayOrUserName()));
                    }
                    catch (Exception e)
                    {
                        this.Get<ILogger<DigestSendTask>>().Error(e, $"Error In Creating Digest for PageUser {user.ID}");
                    }
                });

        this.Get<IMailService>().SendAll(mailMessages);

        this.Get<ILogger<DigestSendTask>>().Info($"Digest send to {mailMessages.Count} user(s)");
    }
}