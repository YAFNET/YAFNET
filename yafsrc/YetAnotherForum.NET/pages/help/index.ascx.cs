/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
    using System.Collections.Generic;
    using System;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Utils;

    /// <summary>
    /// Summary description for main.
    /// </summary>
    public partial class index : ForumPage
    {
        ///<summary>
        /// List with the Help Content
        ///</summary>
        public List<YafHelpContent> helpContents = new List<YafHelpContent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="index"/> class.
        /// </summary>
        public index()
            : base(null)
        {
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
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadHelpContent();

            if (!IsPostBack)
            {
                this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(this.GetText("subtitle"), YafBuildLink.GetLink(ForumPages.help_index));

                if (this.Request.QueryString.GetFirstOrDefault("faq").IsSet())
                {
                    this.BindData();
                }
                else
                {
                    // Load Index and Search
                    SearchHolder.Visible = true;

                    SubTitle.Text = this.GetText("subtitle");
                    HelpContent.Text = this.GetText("welcome");
                }
            }
        }

        /// <summary>
        /// Load the Complete Help Pages From the language File.
        /// </summary>
        private void LoadHelpContent()
        {
            if (!helpContents.Count.Equals(0)) return;

            YafHelpContent itemRecover = new YafHelpContent
                                             {
                                                 HelpPage = "recover",
                                                 HelpTitle = this.GetText("RECOVERTITLE"),
                                                 HelpContent = this.GetText("RECOVERCONTENT")
                                             };

            YafHelpContent itemAnounce = new YafHelpContent
                                             {
                                                 HelpPage = "anounce",
                                                 HelpTitle = this.GetText("ANOUNCETITLE"),
                                                 HelpContent = this.GetText("ANOUNCECONTENT")
                                             };

            helpContents.Add(itemRecover);

            helpContents.Add(itemAnounce);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            string sFaqPage = this.Request.QueryString.GetFirstOrDefault("faq");

            switch (sFaqPage)
            {
                case "index":
                    {
                        // Load Index and Search
                        SearchHolder.Visible = true;
                        HelpList.Visible = false;

                        SubTitle.Text = this.GetText("subtitle");
                        HelpContent.Text = this.GetText("welcome");
                    }
                    break;
                case "recover":
                    {
                        // Load Lost Password
                        SearchHolder.Visible = false;
                        HelpList.Visible = true;

                        HelpList.DataSource = ListTableHelper.ListToDataTable(helpContents.FindAll(check => check.HelpPage.Equals(sFaqPage)));
                    }
                    break;
                case "anounce":
                    {
                        // Load Lost Password
                        SearchHolder.Visible = false;
                        HelpList.Visible = true;

                        HelpList.DataSource = ListTableHelper.ListToDataTable(helpContents.FindAll(check => check.HelpPage.Equals(sFaqPage)));
                    }
                    break;
                default:
                    {
                        // Load Index and Search
                        SearchHolder.Visible = true;
                        HelpList.Visible = false;

                        SubTitle.Text = this.GetText("subtitle");
                        HelpContent.Text = this.GetText("welcome");
                    }
                    break;
            }

            //HelpList.DataSource = ListTableHelper.ListToDataTable(helpContents);
            HelpList.DataBind();

            DataBind();
        }

        

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.DoSearch.Click += DoSearch_Click;
            base.OnInit(e);
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
        private void DoSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(search.Text)) return;

            if (search.Text.Length <= 3)
            {
                // TODO : Show Message
                return;
            }

            IList<string> HighlightWords = new List<string> {search.Text};

            List<YafHelpContent> searchlist =
                helpContents.FindAll(
                    check =>
                    check.HelpContent.ToLower().Contains(search.Text.ToLower()) ||
                    check.HelpTitle.ToLower().Contains(search.Text.ToLower()));


            foreach (YafHelpContent item in searchlist)
            {
                item.HelpContent = YafFormatMessage.SurroundWordList(item.HelpContent, HighlightWords, @"<span class=""highlight"">",
                                                       @"</span>");
                item.HelpTitle = YafFormatMessage.SurroundWordList(item.HelpTitle, HighlightWords, @"<span class=""highlight"">",
                                                       @"</span>");
            }

            if (searchlist.Count.Equals(0))
            {
                // TODO : Show Message
                return;
            }


            HelpList.DataSource = ListTableHelper.ListToDataTable(searchlist);
            HelpList.DataBind();

            SearchHolder.Visible = false;
            HelpList.Visible = true;
        }

        /// <summary>
        /// Class that Can store the Help Content
        /// </summary>
        public class YafHelpContent
        {
            /// <summary>
            /// The Help page Name
            /// </summary>
            public string HelpPage;
            /// <summary>
            /// The Title of the Help page
            /// </summary>
            public string HelpTitle;
            /// <summary>
            /// The Content of the Help page
            /// </summary>
            public string HelpContent;
        }
    }
}