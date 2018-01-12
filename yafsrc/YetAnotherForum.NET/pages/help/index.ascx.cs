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

namespace YAF.Pages.help
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml.Serialization;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Help Index.
    /// </summary>
    public partial class index : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///  List with the Help Content
        /// </summary>
        private readonly List<YafHelpContent> helpContents = new List<YafHelpContent>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "index" /> class.
        /// </summary>
        public index()
            : base(null)
        {
        }

        /// <summary>
        /// Gets PageName.
        /// </summary>
        public override string PageName
        {
            get
            {
                return "help_index";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.DoSearch.Click += this.DoSearch_Click;
            base.OnInit(e);

            if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowHelpTo))
            {
                YafBuildLink.AccessDenied();
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.LoadHelpContent();

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("SUBTITLE"), YafBuildLink.GetLink(ForumPages.help_index));

            this.DoSearch.Text = this.GetText("SEARCH", "BTNSEARCH");

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq").IsSet())
            {
                var faqPage = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq");

                if (faqPage.Equals("index"))
                {
                    faqPage = "SEARCHHELP";
                }

                this.PageLinks.AddLink(
                    this.GetText(
                        "HELP_INDEX",
                        "{0}TITLE".FormatWith(faqPage)),
                    string.Empty);

                this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("SUBTITLE"),
                this.GetText("HELP_INDEX", "{0}TITLE".FormatWith(faqPage)));

                this.BindData();
            }
            else
            {
                this.PageLinks.AddLink(this.GetText("HELP_INDEX", "SEARCHHELPTITLE"), string.Empty);

                this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("SUBTITLE"),
                this.GetText("HELP_INDEX", "SEARCHHELPTITLE"));

                // Load Index and Search
                this.SearchHolder.Visible = true;

                this.SubTitle.Text = this.GetText("subtitle");
                this.HelpContent.Text = this.GetText("welcome");
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            var faqPage = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq");

            switch (faqPage)
            {
                case "index":
                    {
                        // Load Index and Search
                        this.SearchHolder.Visible = true;
                        this.HelpList.Visible = false;

                        this.SubTitle.Text = this.GetText("subtitle");
                        this.HelpContent.Text = this.GetText("welcome");
                    }

                    break;
                default:
                    {
                        var helpContentList = this.helpContents.FindAll(check => check.HelpPage.ToLower().Equals(faqPage));

                        if (helpContentList.Count > 0)
                        {
                            this.SearchHolder.Visible = false;
                            this.HelpList.Visible = true;
                            this.HelpList.DataSource = helpContentList;
                        }
                        else
                        {
                            // Load Index and Search
                            this.SearchHolder.Visible = true;
                            this.HelpList.Visible = false;

                            this.SubTitle.Text = this.GetText("subtitle");
                            this.HelpContent.Text = this.GetText("welcome");
                        }
                    }

                    break;
            }

            this.HelpList.DataBind();

            this.DataBind();
        }

        /// <summary>
        /// Search for Help Content
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DoSearch_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (string.IsNullOrEmpty(this.search.Text))
            {
                return;
            }

            if (this.search.Text.Length <= 3)
            {
                this.PageContext.AddLoadMessage(this.GetText("SEARCHLONGER"), MessageTypes.danger);

                return;
            }

            var highlightWords = new List<string> { this.search.Text };

            var searchlist =
              this.helpContents.FindAll(
                check =>
                check.HelpContent.ToLower().Contains(this.search.Text.ToLower()) ||
                check.HelpTitle.ToLower().Contains(this.search.Text.ToLower()));

            foreach (var item in searchlist)
            {
                item.HelpContent = this.Get<IFormatMessage>().SurroundWordList(
                  item.HelpContent, highlightWords, @"<span class=""highlight"">", @"</span>");
                item.HelpTitle = this.Get<IFormatMessage>().SurroundWordList(
                  item.HelpTitle, highlightWords, @"<span class=""highlight"">", @"</span>");
            }

            if (searchlist.Count.Equals(0))
            {
                this.PageContext.AddLoadMessage(this.GetText("NORESULTS"), MessageTypes.warning);

                return;
            }

            this.HelpList.DataSource = searchlist;
            this.HelpList.DataBind();

            this.SearchHolder.Visible = false;
            this.HelpList.Visible = true;
        }

        /// <summary>
        /// Load the Complete Help Pages From the language File.
        /// </summary>
        private void LoadHelpContent()
        {
            if (!this.helpContents.Count.Equals(0))
            {
                return;
            }

            // vzrus tip: some features can be disabled and users shouldn't normally see them in help.
            // The list can include some limitations based on host settings when features are enabled.  
            // tha_watcha: actually not really needed content describes that things can be disabled.
            var helpNavigation = new List<HelpMenu.YafHelpNavigation>();

            var serializer = new XmlSerializer(typeof(List<HelpMenu.YafHelpNavigation>));

            var xmlFilePath =
                HttpContext.Current.Server.MapPath(
                    "{0}Resources/{1}".FormatWith(YafForumInfo.ForumServerFileRoot, "HelpMenuList.xml"));

            if (File.Exists(xmlFilePath))
            {
                var reader = new StreamReader(xmlFilePath);

                helpNavigation = (List<HelpMenu.YafHelpNavigation>)serializer.Deserialize(reader);

                reader.Close();
            }

            foreach (var helpPage in helpNavigation.SelectMany(category => category.HelpPages))
            {
                string helpContent;

                switch (helpPage.HelpPage)
                {
                    case "RECOVER":
                        {
                            helpContent = this.GetTextFormatted(
                                "{0}CONTENT".FormatWith(helpPage.HelpPage),
                                YafBuildLink.GetLink(ForumPages.recoverpassword));
                        }

                        break;
                    case "BBCODES":
                        {
                            helpContent = this.GetTextFormatted(
                                "{0}CONTENT".FormatWith(helpPage.HelpPage),
                                YafForumInfo.ForumBaseUrl);
                        }

                        break;
                    case "POSTING":
                        {
                            helpContent = this.GetTextFormatted(
                                "{0}CONTENT".FormatWith(helpPage.HelpPage),
                                YafBuildLink.GetLink(ForumPages.help_index, "faq=bbcodes"));
                        }

                        break;
                    default:
                        {
                            helpContent = this.GetText("{0}CONTENT".FormatWith(helpPage.HelpPage));
                        }

                        break;
                }
                
                this.helpContents.Add(
                    new YafHelpContent
                        {
                            HelpPage = helpPage.HelpPage,
                            HelpTitle = this.GetText("{0}TITLE".FormatWith(helpPage.HelpPage)),
                            HelpContent = helpContent
                        });
            }
        }

        #endregion

        /// <summary>
        /// Class that Can store the Help Content
        /// </summary>
        public class YafHelpContent
        {
            #region Properties

            /// <summary>
            ///   Gets or sets The Content of the Help page
            /// </summary>
            public string HelpContent { get; set; }

            /// <summary>
            ///   Gets or sets The Help page Name
            /// </summary>
            public string HelpPage { get; set; }

            /// <summary>
            ///   Gets or sets The Title of the Help page
            /// </summary>
            public string HelpTitle { get; set; }

            #endregion
        }
    }
}