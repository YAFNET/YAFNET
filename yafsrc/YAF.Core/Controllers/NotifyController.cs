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

namespace YAF.Core.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.UI.WebControls;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The Notifications controller.
    /// </summary>
    [RoutePrefix("api")]
    public class NotifyController : ApiController, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        #endregion

        /// <summary>
        /// Gets the paged attachments.
        /// </summary>
        /// <param name="pagedResults">
        /// The paged Results.
        /// </param>
        /// <returns>
        /// Returns the Attachment List as Grid Data Set
        /// </returns>
        [Route("Notify/GetNotifications")]
        [HttpPost]
        public IHttpActionResult GetNotifications(PagedResults pagedResults)
        {
            var userId = pagedResults.UserId;
            var pageSize = pagedResults.PageSize;
            var pageNumber = pagedResults.PageNumber;

            var activities = this.GetRepository<Activity>().GetPaged(
                a => a.UserID == userId && a.FromUserID.HasValue && a.Notification,
                pageNumber,
                pageSize);

            var attachmentItems = new List<AttachmentItem>();

            activities.ForEach(
                activity =>
                    {
                        var messageHolder = new PlaceHolder();
                        var iconLabel = new Label { CssClass = "fa-stack" };

                        var message = string.Empty;
                        var icon = string.Empty;

                        var topicLink = new HyperLink
                        {
                            NavigateUrl =
                                                    BuildLink.GetLink(
                                                        ForumPages.Posts,
                                                        "m={0}#post{0}",
                                                        activity.MessageID.Value),
                            Text =
                                $"<i class=\"fas fa-comment fa-fw mr-1\"></i>{this.GetRepository<Topic>().GetById(activity.TopicID.Value).TopicName}"
                        };

                        var name = this.Get<IUserDisplayName>().GetName(activity.FromUserID.Value);

                        if (activity.ActivityFlags.ReceivedThanks)
                        {
                            icon = "heart";
                            message = this.Get<ILocalization>().GetTextFormatted(
                                "RECEIVED_THANKS_MSG",
                                name,
                                topicLink.RenderToString());
                        }

                        if (activity.ActivityFlags.WasMentioned)
                        {
                            icon = "at";
                            message = this.Get<ILocalization>().GetTextFormatted(
                                "WAS_MENTIONED_MSG",
                                name,
                                topicLink.RenderToString());
                        }

                        if (activity.ActivityFlags.WasQuoted)
                        {
                           icon = "quote-left";
                            message = this.Get<ILocalization>().GetTextFormatted(
                                "WAS_QUOTED_MSG",
                                name,
                                topicLink.RenderToString());
                        }

                        var notify = activity.Notification ? "text-success" : "text-secondary";

                        iconLabel.Text = $@"<i class=""fas fa-circle fa-stack-2x {notify}""></i>
                                            <i class=""fas fa-{icon} fa-stack-1x fa-inverse""></i>";

                        messageHolder.Controls.Add(iconLabel);

                        messageHolder.Controls.Add(new Literal { Text = message });

                        var attachment = new AttachmentItem
                                             {
                                                 FileName = messageHolder.RenderToString()
                                             };

                        attachmentItems.Add(attachment);
                    });

            return this.Ok(
                new GridDataSet
                    {
                        PageNumber = pageNumber,
                        TotalRecords =
                            activities.Any()
                                ? this.GetRepository<Activity>().Count(a => a.UserID == userId && a.FromUserID.HasValue && a.Notification)
                                    .ToType<int>()
                                : 0,
                        PageSize = pageSize,
                        AttachmentList = attachmentItems
                    });
        }
    }
}