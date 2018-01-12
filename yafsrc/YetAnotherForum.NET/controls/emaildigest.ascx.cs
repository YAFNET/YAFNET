/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Data.Profiling;
    using YAF.Core.Services;
    using YAF.Core.Services.Localization;
    using YAF.Core.Services.Startup;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The email_digest.
    /// </summary>
    public partial class emaildigest : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _combined user data.
        /// </summary>
        private CombinedUserDataHelper _combinedUserData;

        /// <summary>
        ///   The _forum data.
        /// </summary>
        private List<SimpleForum> _forumData;

        /// <summary>
        ///   The _language file.
        /// </summary>
        private string _languageFile;

        /// <summary>
        ///   The YAF localization.
        /// </summary>
        private ILocalization _localization;

        /// <summary>
        ///   The _theme.
        /// </summary>
        private YafTheme _theme;

        /// <summary>
        ///   Numbers of hours to compute digest for...
        /// </summary>
        private int _topicHours = -24;

        /// <summary>
        /// The _show errors
        /// </summary>
        private bool? _showErrors;

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
                var topicsFlattened = this._forumData.SelectMany(x => x.Topics);

                return
                    topicsFlattened.Where(
                        t =>
                        t.LastPostDate > DateTime.Now.AddHours(this._topicHours)
                        && t.CreatedDate < DateTime.Now.AddHours(this._topicHours)).GroupBy(x => x.Forum);
            }
        }

        /// <summary>
        ///   Gets or sets BoardID.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        ///   Gets or sets CurrentUserID.
        /// </summary>
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Gets or sets the board settings.
        /// </summary>
        /// <value>
        /// The board settings.
        /// </value>
        public YafBoardSettings BoardSettings { get; set; }

        /// <summary>
        ///   Gets NewTopics.
        /// </summary>
        [NotNull]
        public IEnumerable<IGrouping<SimpleForum, SimpleTopic>> NewTopics
        {
            get
            {
                // flatten...
                var topicsFlattened = this._forumData.SelectMany(x => x.Topics);

                return
                    topicsFlattened.Where(t => t.CreatedDate > DateTime.Now.AddHours(this._topicHours))
                        .GroupBy(x => x.Forum);
            }
        }

        /// <summary>
        ///   Gets UserData.
        /// </summary>
        [NotNull]
        public CombinedUserDataHelper UserData
        {
            get
            {
                return this._combinedUserData
                       ?? (this._combinedUserData = new CombinedUserDataHelper(this.CurrentUserID));
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
                if (this._showErrors.HasValue)
                {
                    return this._showErrors.Value;
                }

                var showErrors = false;
                if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("showerror").IsSet())
                {
                    showErrors = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("showerror").ToType<bool>();
                }

                this._showErrors = showErrors;

                return this._showErrors.Value;
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
            if (this._languageFile.IsSet() && this._localization == null)
            {
                this._localization = new YafLocalization();
                this._localization.LoadTranslation(this._languageFile);
            }
            else if (this._localization == null)
            {
                this._localization = this.Get<ILocalization>();
            }

            return this._localization.GetText("DIGEST", tag);
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
            return
                BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(lastMessage)))
                    .RemoveMultipleWhitespace()
                    .Truncate(maxlength);
        }

        /// <summary>
        /// The output error.
        /// </summary>
        /// <param name="errorString">
        /// The error string.
        /// </param>
        protected void OutputError([NotNull] string errorString)
        {
            this.Response.Write(
                "<!DOCTYPE html><html><head><title>Error</title></head><body><h1>{0}</h1></body></html>".FormatWith(
                    errorString));
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
                if (YafContext.Current.BoardSettings.BoardID.Equals(this.BoardID))
                {
                    this.BoardSettings = YafContext.Current.BoardSettings;
                }
            }
            else
            {
                this.BoardSettings = new YafLoadBoardSettings(this.BoardID);
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

                this.Response.End();
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

                this.Response.End();
                return;
            }

            if (this.CurrentUserID == 0)
            {
                this.CurrentUserID = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("UserID").ToType<int>();
            }

            // get topic hours...
            this._topicHours = -this.BoardSettings.DigestSendEveryXHours;

            this._forumData = this.Get<YafDbBroker>()
                .GetSimpleForumTopic(this.BoardID, this.CurrentUserID, DateTime.Now.AddHours(this._topicHours), 9999);

            if (!this.NewTopics.Any() && !this.ActiveTopics.Any())
            {
                if (this.ShowErrors)
                {
                    this.OutputError(
                        "No topics for the last {0} hours.".FormatWith(this.BoardSettings.DigestSendEveryXHours));

                    //this.Response.Write(GetDebug());
                }

                this.Response.End();
                return;
            }

            this._languageFile = UserHelper.GetUserLanguageFile(
                this.CurrentUserID,
                this.BoardID,
                this.BoardSettings.AllowUserLanguage);

            this._theme =
                new YafTheme(
                    UserHelper.GetUserThemeFile(
                        this.CurrentUserID,
                        this.BoardID,
                        this.BoardSettings.AllowUserTheme,
                        this.BoardSettings.Theme));

            var subject = this.GetText("SUBJECT").FormatWith(this.BoardSettings.Name);

            var digestHead = this._theme.GetItem("THEME", "DIGESTHEAD", null);

            if (digestHead.IsSet())
            {
                this.YafHead.Controls.Add(new LiteralControl(digestHead));
            }

            if (subject.IsSet())
            {
                this.YafHead.Title = subject;
            }
        }

        #endregion

        /// <summary>
        /// Gets the debug information.
        /// </summary>
        /// <returns>Returns the String with the debug information</returns>
        protected string GetDebug()
        {
            var debugInfo = new StringBuilder();

#if DEBUG
            if (!this.ShowErrors)
            {
                return debugInfo.ToString();
            }

            debugInfo.Append(@"<div class=""small"">");
            debugInfo.AppendFormat(
                @"<br /><br /><b>{0}</b> SQL Queries: <b>{1:N3}</b> Seconds (<b>{2:N2}%</b> of Total Page Load Time).<br />{3}",
                QueryCounter.Count,
                QueryCounter.Duration,
                (100 * QueryCounter.Duration) / this.Get<IStopWatch>().Duration,
                QueryCounter.Commands);

            debugInfo.Append(@"</div>");
#endif

            return debugInfo.ToString();
        }
    }
}