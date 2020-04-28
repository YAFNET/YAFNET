/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Tasks
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The digest send task.
    /// </summary>
    public class DigestSendTask : LongBackgroundTask
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DigestSendTask" /> class.
        /// </summary>
        public DigestSendTask()
        {
            this.RunPeriodMs = 300 * 1000;
            this.StartDelayMs = 30 * 1000;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets TaskName.
        /// </summary>
        public static string TaskName { get; } = "DigestSendTask";

        #endregion

        #region Public Methods

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            this.SendDigest();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether is time to send digest for board.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <returns>
        /// The is time to send digest for board.
        /// </returns>
        private static bool IsTimeToSendDigestForBoard([NotNull] LoadBoardSettings boardSettings)
        {
            CodeContracts.VerifyNotNull(boardSettings, "boardSettings");

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

            boardSettings.SaveRegistry();

            // reload all settings from the DB
            BoardContext.Current.BoardSettings = null;

            return true;
        }

        /// <summary>
        /// The send digest.
        /// </summary>
        private void SendDigest()
        {
            try
            {
                var boardIds = this.GetRepository<Board>().ListTyped().Select(b => b.ID);

                boardIds.ForEach(
                    boardId =>
                        {
                            var boardSettings = new LoadBoardSettings(boardId);

                            if (!IsTimeToSendDigestForBoard(boardSettings))
                            {
                                return;
                            }

                            // get users with digest enabled...
                            var usersWithDigest = this.GetRepository<User>()
                                .FindUserTyped(false, boardId, dailyDigest: true).Where(
                                    x => x.IsGuest != null && !x.IsGuest.Value && (x.IsApproved ?? false));

                            var typedUserFinds = usersWithDigest as IList<User> ?? usersWithDigest.ToList();

                            if (typedUserFinds.Any())
                            {
                                // start sending...
                                this.SendDigestToUsers(typedUserFinds, boardSettings);
                            }
                            else
                            {
                                this.Get<ILogger>().Info("no user found");
                            }
                        });
            }
            catch (Exception ex)
            {
                this.Get<ILogger>().Error(ex, $"Error In {TaskName} Task");
            }
        }

        /// <summary>
        /// Sends the digest to users.
        /// </summary>
        /// <param name="usersWithDigest">The users with digest.</param>
        /// <param name="boardSettings">The board settings.</param>
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
                            var digestHtml = this.Get<IDigest>().GetDigestHtml(user.ID, boardSettings);

                            if (digestHtml.IsNotSet())
                            {
                                return;
                            }

                            if (user.ProviderUserKey == null)
                            {
                                return;
                            }

                            var membershipUser = UserMembershipHelper.GetUser(user.Name);

                            if (membershipUser == null || membershipUser.Email.IsNotSet())
                            {
                                return;
                            }

                            var subject = Regex.Match(digestHtml, "<title>(.*?)</title>", RegexOptions.Singleline)
                                .Groups[1].Value.Trim();

                            // send the digest...
                            mailMessages.Add(this.Get<IDigest>().CreateDigestMessage(
                                subject.Trim(),
                                digestHtml,
                                boardEmail,
                                membershipUser.Email,
                                user.DisplayName));
                        }
                        catch (Exception e)
                        {
                            this.Get<ILogger>().Error(e, $"Error In Creating Digest for User {user.ID}");
                        }
                        finally
                        {
                            HttpContext.Current = null;
                        }
                    });

            this.Get<ISendMail>().SendAll(mailMessages);

            this.Get<ILogger>().Log(
                $"Digest send to {mailMessages.Count} user(s)",
                EventLogTypes.Information,
                null,
                "Digest Send Task");
        }

        #endregion
    }
}