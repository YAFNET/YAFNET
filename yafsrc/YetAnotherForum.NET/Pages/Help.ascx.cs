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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml.Serialization;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Help Index.
    /// </summary>
    public partial class Help : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///  List with the Help Content
        /// </summary>
        private readonly List<HelpContent> helpContents = new();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Help" /> class.
        /// </summary>
        public Help()
            : base("HELP")
        {
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

            if (!this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ShowHelpTo))
            {
                this.Get<LinkBuilder>().AccessDenied();
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
                this.GetText("SUBTITLE"), this.Get<LinkBuilder>().GetLink(ForumPages.Help));

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
                        $"{faqPage}TITLE"),
                    string.Empty);

                this.BindData();
            }
            else
            {
                this.PageLinks.AddLink(this.GetText("HELP_INDEX", "SEARCHHELPTITLE"), string.Empty);

                // Load Index and Search
                this.SearchHolder.Visible = true;

                this.SubTitle.Text = this.GetText("subtitle");
                this.HelpContent.Text = this.GetText("welcome");
            }
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
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
            if (this.search.Text.IsNotSet())
            {
                return;
            }

            if (this.search.Text.Length <= 3)
            {
                this.PageContext.AddLoadMessage(this.GetText("SEARCHLONGER"), MessageTypes.danger);

                return;
            }

            var highlightWords = new List<string> { this.search.Text };

            var searchList =
              this.helpContents.FindAll(
                check =>
                check.Content.ToLower().Contains(this.search.Text.ToLower()) ||
                check.Title.ToLower().Contains(this.search.Text.ToLower()));

            searchList.ForEach(
                item =>
                {
                    item.Content = this.Get<IFormatMessage>().SurroundWordList(
                        item.Content, highlightWords, "<mark>", "</mark>");
                    item.Title = this.Get<IFormatMessage>().SurroundWordList(
                        item.Title, highlightWords, "<mark>", "</mark>");
                });

            if (searchList.Count.Equals(0))
            {
                this.PageContext.AddLoadMessage(this.GetText("NORESULTS"), MessageTypes.warning);

                return;
            }

            this.HelpList.DataSource = searchList;
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

            var helpNavigation = new List<HelpNavigation>();

            var serializer = new XmlSerializer(typeof(List<HelpNavigation>));

            var xmlFilePath =
                this.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}Resources/HelpMenuList.xml");

            if (File.Exists(xmlFilePath))
            {
                var reader = new StreamReader(xmlFilePath);

                helpNavigation = (List<HelpNavigation>)serializer.Deserialize(reader);

                reader.Close();
            }

            foreach (var helpPage in helpNavigation.SelectMany(category => category.HelpPages))
            {
                string helpContent = helpPage.HelpPage switch {
                    "RECOVER" => this.GetTextFormatted(
                        $"{helpPage.HelpPage}CONTENT",
                        this.Get<LinkBuilder>().GetLink(ForumPages.Account_ForgotPassword)),
                    "BBCODES" => this.GetTextFormatted($"{helpPage.HelpPage}CONTENT", BoardInfo.ForumBaseUrl),
                    "POSTING" => this.GetTextFormatted(
                        $"{helpPage.HelpPage}CONTENT",
                        this.Get<LinkBuilder>().GetLink(ForumPages.Help, "faq=bbcodes")),
                    _ => this.GetText($"{helpPage.HelpPage}CONTENT")
                };

                this.helpContents.Add(
                    new HelpContent
                    {
                        HelpPage = helpPage.HelpPage,
                        Title = this.GetText($"{helpPage.HelpPage}TITLE"),
                        Content = helpContent
                    });
            }
        }

        #endregion
    }
}