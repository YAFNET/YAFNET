/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Pages.help
{
    #region Using

    using System;
    using System.Collections.Generic;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Pattern;
    using YAF.Classes.Utils;

    #endregion

    /// <summary>
    /// Summary description for main.
    /// </summary>
    public partial class index : ForumPage
    {
        #region Constants and Fields

        ///<summary>
        ///  List with the Help Content
        ///</summary>
        public List<YafHelpContent> helpContents = new List<YafHelpContent>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "index" /> class.
        /// </summary>
        public index()
            : base(null)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.DoSearch.Click += this.DoSearch_Click;
            base.OnInit(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.LoadHelpContent();

            if (!this.IsPostBack)
            {
                this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(this.PageContext.Localization.GetText("subtitle"), YafBuildLink.GetLink(ForumPages.help_index));

                this.DoSearch.Text = this.GetText("SEARCH", "BTNSEARCH");

                if (this.Request.QueryString.GetFirstOrDefault("faq").IsSet())
                {
                    this.BindData();
                }
                else
                {
                    // Load Index and Search
                    this.SearchHolder.Visible = true;

                    this.SubTitle.Text = this.GetText("subtitle");
                    this.HelpContent.Text = this.GetText("welcome");
                }
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            string FaqPage = this.Request.QueryString.GetFirstOrDefault("faq");

            switch (FaqPage)
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
                        var HelpContentList = this.helpContents.FindAll(check => check.HelpPage.Equals(FaqPage));

                        if (HelpContentList.Count > 0)
                        {
                            this.SearchHolder.Visible = false;
                            this.HelpList.Visible = true;
                            this.HelpList.DataSource = HelpContentList;
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
        /// The do search_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="ApplicationException">
        /// </exception>
        private void DoSearch_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (string.IsNullOrEmpty(this.search.Text))
            {
                return;
            }

            if (this.search.Text.Length <= 3)
            {
                this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("SEARCHLONGER"));

                return;
            }

            IList<string> highlightWords = new List<string> { this.search.Text };

            List<YafHelpContent> searchlist =
              this.helpContents.FindAll(
                check =>
                check.HelpContent.ToLower().Contains(this.search.Text.ToLower()) ||
                check.HelpTitle.ToLower().Contains(this.search.Text.ToLower()));

            foreach (YafHelpContent item in searchlist)
            {
                item.HelpContent = YafFormatMessage.SurroundWordList(
                  item.HelpContent, highlightWords, @"<span class=""highlight"">", @"</span>");
                item.HelpTitle = YafFormatMessage.SurroundWordList(
                  item.HelpTitle, highlightWords, @"<span class=""highlight"">", @"</span>");
            }

            if (searchlist.Count.Equals(0))
            {
                this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("NORESULTS"));

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

             this.helpContents.Add(new YafHelpContent
              {
                  HelpPage = "recover",
                  HelpTitle = this.GetText("RECOVERTITLE"),
                  HelpContent = string.Format(this.GetText("RECOVERCONTENT"), YafBuildLink.GetLink(ForumPages.recoverpassword))
              });

             if (!this.PageContext.BoardSettings.DisableRegistrations && !Config.IsAnyPortal)
             {
                 this.helpContents.Add(new YafHelpContent
                 {
                     HelpPage = "registration",
                     HelpTitle = this.GetText("REGISTRATIONTITLE"),
                     HelpContent = string.Format(this.GetText("REGISTRATIONCONTENT"), YafBuildLink.GetLink(ForumPages.recoverpassword))
                 });
             }
            
            // TODO : Show Only when reg ist enabled
            IList<string> HelpPageNames = new List<string>
                                              {
                                                  "anounce",
                                                  "forums",
                                                  "searching",
                                                  "display",
                                                  "threadopt",
                                                  "threadopt",
                                                  "memberslist",
                                                  "pm",
                                                  "rss",
                                                  "publicprofile",
                                                  "mysettings",
                                                  "mypics",
                                                  "buddies",
                                                  "myalbums",
                                                  "posting",
                                                  "replying",
                                                  "editdelete",
                                                  "polls",
                                                  "attachments",
                                                  "smilies",
                                                  "threadstatus",
                                                  "modsadmins"
                                              };

            foreach (string HelpPageName in HelpPageNames)
            {
                this.helpContents.Add(new YafHelpContent
                {
                    HelpPage = HelpPageName,
                    HelpTitle = this.GetText("{0}TITLE".FormatWith(HelpPageName.ToUpper())),
                    HelpContent = this.GetText("{0}CONTENT".FormatWith(HelpPageName.ToUpper()))
                });
            }
        }

        #endregion

        /// <summary>
        /// Class that Can store the Help Content
        /// </summary>
        public class YafHelpContent
        {
            #region Constants and Fields

            /// <summary>
            ///   The Content of the Help page
            /// </summary>
            public string HelpContent { get; set; }

            /// <summary>
            ///   The Help page Name
            /// </summary>
            public string HelpPage { get; set; }

            /// <summary>
            ///   The Title of the Help page
            /// </summary>
            public string HelpTitle { get; set; }

            #endregion
        }
    }
}