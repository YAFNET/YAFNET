/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.BoardSettings;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Core.Services.Localization;
    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    /// The Email Digest Control.
    /// </summary>
    public partial class EmailDigest : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The forum data.
        /// </summary>
        private List<SimpleForum> forumData;

        /// <summary>
        ///   The language file.
        /// </summary>
        private string languageFile;

        /// <summary>
        ///   The YAF localization.
        /// </summary>
        private ILocalization localization;

        /// <summary>
        ///   Numbers of hours to compute digest for...
        /// </summary>
        private int topicHours = -24;

        /// <summary>
        /// The show errors
        /// </summary>
        private bool? showErrors;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets ActiveTopics.
        /// </summary>
        [NotNull]
        public IEnumerable<IGrouping<SimpleForum, SimpleTopic>> ActiveTopics
        {
            get
            {
                // flatten...
                var topicsFlattened = this.forumData.SelectMany(x => x.Topics);

                return topicsFlattened.Where(
                    t => t.LastPostDate > DateTime.Now.AddHours(this.topicHours) &&
                         t.CreatedDate < DateTime.Now.AddHours(this.topicHours)).GroupBy(x => x.Forum);
            }
        }

        /// <summary>
        ///   Gets or sets BoardID.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        ///   Gets or sets Current User.
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the board settings.
        /// </summary>
        /// <value>
        /// The board settings.
        /// </value>
        public BoardSettings BoardSettings { get; set; }

        /// <summary>
        ///   Gets NewTopics.
        /// </summary>
        [NotNull]
        public IEnumerable<IGrouping<SimpleForum, SimpleTopic>> NewTopics
        {
            get
            {
                // flatten...
                var topicsFlattened = this.forumData.SelectMany(x => x.Topics);

                return topicsFlattened.Where(t => t.CreatedDate > DateTime.Now.AddHours(this.topicHours))
                    .OrderByDescending(x => x.LastPostDate).GroupBy(x => x.Forum);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show errors].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show errors]; otherwise, <c>false</c>.
        /// </value>
        protected bool ShowErrors
        {
            get
            {
                if (this.showErrors.HasValue)
                {
                    return this.showErrors.Value;
                }

                var showError = false;
                if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("showerror").IsSet())
                {
                    showError = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("showerror").ToType<bool>();
                }

                this.showErrors = showError;

                return this.showErrors.Value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the localized text.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetText([NotNull] string tag)
        {
            if (this.languageFile.IsSet() && this.localization == null)
            {
                this.localization = new Localization();
                this.localization.LoadTranslation(this.languageFile);
            }
            else if (this.localization == null)
            {
                this.localization = this.Get<ILocalization>();
            }

            return this.localization.GetText("DIGEST", tag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the message formatted and truncated.
        /// </summary>
        /// <param name="lastMessage">The last message.</param>
        /// <param name="maxlength">The max length.</param>
        /// <returns>
        /// The get message formatted and truncated.
        /// </returns>
        protected string GetMessageFormattedAndTruncated([NotNull] string lastMessage, int maxlength)
        {
            return BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(lastMessage)))
                .RemoveMultipleWhitespace().Truncate(maxlength);
        }

        /// <summary>
        /// The output error.
        /// </summary>
        /// <param name="errorString">
        /// The error string.
        /// </param>
        protected void OutputError([NotNull] string errorString)
        {
            this.Get<HttpResponseBase>().Write(
                $"<h1>{errorString}</h1>");
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.BoardID == 0)
            {
                this.BoardID = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("BoardID").ToType<int>();
            }

            if (HttpContext.Current != null)
            {
                this.BoardSettings = this.PageBoardContext.BoardSettings.BoardID.Equals(this.BoardID)
                    ? this.PageBoardContext.BoardSettings
                    : new LoadBoardSettings(this.BoardID);
            }
            else
            {
                this.BoardSettings = new LoadBoardSettings(this.BoardID);
            }

            this.Get<StartupInitializeDb>().Run();

            var token = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("token");

            if (token.IsNotSet() || !token.Equals(this.BoardSettings.WebServiceToken))
            {
                if (this.ShowErrors)
                {
                    this.OutputError(
                        "Invalid Web Service Token. Please go into your host settings and save them committing a unique web service token to the database.");
                }

                this.Get<HttpResponseBase>().End();
                return;
            }

            if (Config.ForceScriptName.IsNotSet())
            {
                // fail... ForceScriptName required for Digest.
                if (this.ShowErrors)
                {
                    this.OutputError(
                        @"Cannot generate digest unless YAF.ForceScriptName AppSetting is specified in your app.config (default). Please specify the full page name for YAF.NET -- usually ""default.aspx"".");
                }

                this.Get<HttpResponseBase>().End();
                return;
            }

            if (this.CurrentUser == null)
            {
                var userId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("UserID").Value;
                this.CurrentUser = this.GetRepository<User>().GetById(userId);
            }

            // get topic hours...
            this.topicHours = -this.Get<BoardSettings>().DigestSendEveryXHours;

            this.forumData = this.Get<DataBroker>().GetSimpleForumTopic(
                this.PageBoardContext.PageBoardID,
                this.CurrentUser.ID,
                DateTime.Now.AddHours(this.topicHours),
                9999);

            if (!this.NewTopics.Any() && !this.ActiveTopics.Any())
            {
                if (this.ShowErrors)
                {
                    this.OutputError($"No topics for the last {this.BoardSettings.DigestSendEveryXHours} hours.");
                }

                this.Get<HttpResponseBase>().End();
                return;
            }

            this.languageFile = UserHelper.GetUserLanguageFile(this.CurrentUser);

            if (this.languageFile.IsNotSet())
            {
                this.languageFile = this.BoardSettings.Language;
            }

            var inlineCss = File.ReadAllText(
                HttpContext.Current.Server
                    .MapPath(this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")));

            this.YafHead.Controls.Add(
                ControlHelper.MakeCssControl(inlineCss));

            var logoUrl =
                $"{BoardInfo.ForumClientFileRoot}{this.Get<BoardFolders>().Logos}/{this.Get<BoardSettings>().ForumLogo}";

            this.Logo.ImageUrl = $"{this.Get<BoardSettings>().BaseUrlMask}{logoUrl}";

            var subject = string.Format(this.GetText("SUBJECT"), this.BoardSettings.Name);

            if (subject.IsSet())
            {
                this.YafHead.Title = subject;
            }

            this.NewTopicsForumsRepeater.DataSource = this.NewTopics;
            this.NewTopicsForumsRepeater.DataBind();

            this.ActiveTopicsForumsRepeater.DataSource = this.ActiveTopics;
            this.ActiveTopicsForumsRepeater.DataBind();
        }

        /// <summary>
        /// The new topics forums repeater_ on item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void NewTopicsForumsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!e.Item.ItemType.Equals(ListItemType.Item))
            {
                return;
            }

            var item = (IGrouping<SimpleForum, SimpleTopic>)e.Item.DataItem;

            var topics = item.OrderByDescending(x => x.LastPostDate);

            var newTopicsRepeater = e.Item.FindControlAs<Repeater>("NewTopicsRepeater");
            newTopicsRepeater.DataSource = topics;
            newTopicsRepeater.DataBind();
        }

        /// <summary>
        /// The active topics forums repeater_ on item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ActiveTopicsForumsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!e.Item.ItemType.Equals(ListItemType.Item))
            {
                return;
            }

            var item = (IGrouping<SimpleForum, SimpleTopic>)e.Item.DataItem;

            var topics = item.OrderByDescending(x => x.LastPostDate);

            var activeTopicsRepeater = e.Item.FindControlAs<Repeater>("ActiveTopicsRepeater");
            activeTopicsRepeater.DataSource = topics;
            activeTopicsRepeater.DataBind();
        }

        #endregion
    }
}