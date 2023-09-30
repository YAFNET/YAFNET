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
using System.Net.Mail;

using YAF.Types.Constants;
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
    private static bool IsTimeToSendDigestForBoard(BoardSettings boardSettings)
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
                            this.Get<ILoggerService>().Info("no user found");
                        }
                    });
        }
        catch (Exception ex)
        {
            this.Get<ILoggerService>().Error(ex, $"Error In {TaskName} Task");
        }
    }

    /// <summary>
    /// Sends the digest to users.
    /// </summary>
    /// <param name="usersWithDigest">
    /// The users with digest.
    /// </param>
    /// <param name="boardSettings">
    /// The Board Settings.
    /// </param>
    private void SendDigestToUsers(IEnumerable<User> usersWithDigest, BoardSettings boardSettings)
    {
        var currentContext = HttpContext.Current;

        var mailMessages = new List<MailMessage>();

        var boardEmail = new MailAddress(boardSettings.ForumEmail, boardSettings.Name);

        usersWithDigest.AsParallel().ForAll(
            user =>
                {
                    HttpContext.Current = currentContext;

                    try
                    {
                        if (user.ProviderUserKey == null)
                        {
                            return;
                        }

                        if (user.UserFlags.IsGuest)
                        {
                            return;
                        }

                        var digestMail = this.Get<IDigest>().CreateDigest(
                            user,
                            boardEmail,
                            user.Email,
                            user.DisplayOrUserName());

                        // send the digest...
                        mailMessages.Add(digestMail);
                    }
                    catch (Exception e)
                    {
                        this.Get<ILoggerService>().Error(e, $"Error In Creating Digest for User {user.ID}");
                    }
                    finally
                    {
                        HttpContext.Current = null;
                    }
                });

        this.Get<IMailService>().SendAll(mailMessages);

        this.Get<ILoggerService>().Log(
            $"Digest send to {mailMessages.Count} user(s)",
            EventLogTypes.Information,
            null,
            "Digest Send Task");
    }
}