/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Xml.Serialization;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Renders the Help Menu on the Help Pages.
    /// </summary>
    public class HelpMenu : BasePanel
    {
        #region Methods

        /// <summary>
        /// The List with the Help Navigation Items
        /// </summary>
        private List<YafHelpNavigation> helpNavList = new List<YafHelpNavigation>();

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            // LoadList 
            /*var navItemGeneralTopics = new YafHelpNavigation
                {
                    HelpCategory = "GENERALTOPICS",
                    HelpPages =
                        new List<YafHelpNavigationPage>
                        {
                            new YafHelpNavigationPage { HelpPage = "FORUMS" },
                            new YafHelpNavigationPage { HelpPage = "REGISTRATION" },
                            new YafHelpNavigationPage { HelpPage = "SEARCHING" },
                            new YafHelpNavigationPage { HelpPage = "ANOUNCE" },
                            new YafHelpNavigationPage { HelpPage = "DISPLAY" },
                            new YafHelpNavigationPage { HelpPage = "NEWPOSTS" },
                            new YafHelpNavigationPage { HelpPage = "THREADOPT" },
                            new YafHelpNavigationPage { HelpPage = "RECOVER" },
                            new YafHelpNavigationPage { HelpPage = "MEMBERSLIST" },
                            new YafHelpNavigationPage { HelpPage = "POPUPS" },
                            new YafHelpNavigationPage { HelpPage = "PM" },
                            new YafHelpNavigationPage { HelpPage = "RSS" }
                        }
                };

            this.helpNavList.Add(navItemGeneralTopics);

            var navItemProfileTopics = new YafHelpNavigation
                {
                    HelpCategory = "PROFILETOPICS",
                    HelpPages =
                        new List<YafHelpNavigationPage>
                        {
                            new YafHelpNavigationPage { HelpPage = "MYSETTINGS" },
                            new YafHelpNavigationPage { HelpPage = "MESSENGER" },
                            new YafHelpNavigationPage { HelpPage = "PUBLICPROFILE" },
                            new YafHelpNavigationPage { HelpPage = "EDITPROFILE" },
                            new YafHelpNavigationPage { HelpPage = "THANKS" },
                            new YafHelpNavigationPage { HelpPage = "BUDDIES" },
                            new YafHelpNavigationPage { HelpPage = "MYALBUMS" },
                            new YafHelpNavigationPage { HelpPage = "MYPICS" },
                            new YafHelpNavigationPage { HelpPage = "MAILSETTINGS" },
                            new YafHelpNavigationPage { HelpPage = "SUBSCRIPTIONS" }
                        }
                };

            this.helpNavList.Add(navItemProfileTopics);

            var navItemReadPostTopics = new YafHelpNavigation
                {
                    HelpCategory = "READPOSTTOPICS",
                    HelpPages =
                        new List<YafHelpNavigationPage>
                        {
                            new YafHelpNavigationPage { HelpPage = "POSTING" },
                            new YafHelpNavigationPage { HelpPage = "REPLYING" },
                            new YafHelpNavigationPage { HelpPage = "EDITDELETE" },
                            new YafHelpNavigationPage { HelpPage = "POLLS" },
                            new YafHelpNavigationPage { HelpPage = "ATTACHMENTS" },
                            new YafHelpNavigationPage { HelpPage = "SMILIES" },
                            new YafHelpNavigationPage { HelpPage = "MODSADMINS" },
                        }
                };

            this.helpNavList.Add(navItemReadPostTopics);

            */

            var serializer = new XmlSerializer(typeof(List<YafHelpNavigation>));

            var xmlFilePath =
                HttpContext.Current.Server.MapPath(
                    "{0}resources/{1}".FormatWith(YafForumInfo.ForumServerFileRoot, "HelpMenuList.xml"));

            if (File.Exists(xmlFilePath))
            {
                var reader = new StreamReader(xmlFilePath);

                this.helpNavList = (List<YafHelpNavigation>)serializer.Deserialize(reader);

                reader.Close();
            }

            var html = new StringBuilder(2000);

            string faqPage = "index";

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq").IsSet())
            {
                faqPage = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq");
            }

            // Header
            html.Append(@"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafhelpmenu"">");

            html.Append(@"<tr><td class=""header1"">{0}</td></tr>".FormatWith(this.GetText("HELP_INDEX", "NAVIGATION")));

            html.Append(@"<tr class=""header2""><td>{0}</td></tr>".FormatWith(this.GetText("HELP_INDEX", "INDEX")));

            html.AppendFormat(@"<tr><td class=""post""><ul id=""yafhelpindex"">");

            string selectedStyle = string.Empty;

            if (faqPage.Equals("index"))
            {
                selectedStyle = @"style=""color:red;""";
            }

            html.AppendFormat(
                @"<li><a href=""{0}"" {2} title=""{1}"">{1}</a></li>",
                YafBuildLink.GetLink(ForumPages.help_index, "faq=index"),
                this.GetText("HELP_INDEX", "SEARCHHELP"),
                selectedStyle);

            html.AppendFormat(@"</ul></td></tr>");

            foreach (var category in this.helpNavList)
            {
                html.AppendFormat(
                        @"<tr class=""header2""><td>{0}</td></tr>", this.GetText("HELP_INDEX", category.HelpCategory));

                html.AppendFormat(@"<tr><td class=""post""><ul id=""yafhelp{0}"">", category.HelpCategory.ToLower());

                foreach (var helpPage in category.HelpPages)
                {
                    selectedStyle = string.Empty;

                    if (helpPage.HelpPage.ToLower().Equals(faqPage))
                    {
                        selectedStyle = @"style=""color:red;""";
                    }

                    if (helpPage.HelpPage.Equals("REGISTRATION"))
                    {
                        if (!this.Get<YafBoardSettings>().DisableRegistrations && !Config.IsAnyPortal)
                        {
                            html.AppendFormat(
                                @"<li><a href=""{0}"" {2} title=""{1}"">{1}</a></li>",
                                YafBuildLink.GetLink(ForumPages.help_index, "faq={0}".FormatWith(helpPage.HelpPage.ToLower())),
                                this.GetText("HELP_INDEX", "{0}TITLE".FormatWith(helpPage.HelpPage)),
                                selectedStyle);
                        }
                    }
                    else
                    {
                        html.AppendFormat(
                       @"<li><a href=""{0}"" {2} title=""{1}"">{1}</a></li>",
                       YafBuildLink.GetLink(ForumPages.help_index, "faq={0}".FormatWith(helpPage.HelpPage.ToLower())),
                       this.GetText("HELP_INDEX", "{0}TITLE".FormatWith(helpPage.HelpPage)),
                       selectedStyle);
                    }
                }

                html.AppendFormat(@"</ul></td></tr>");
            }

            html.Append(@"</table>");

            writer.BeginRender();

            // render the contents of the help menu....
            writer.WriteLine(@"<div id=""{0}"">".FormatWith(this.ClientID));
            writer.WriteLine(@"<table class=""adminContainer""><tr>");
            writer.WriteLine(@"<td class=""adminMenu"" valign=""top"">");

            writer.Write(html.ToString());

            writer.WriteLine(@"</td>");

            // contents of the help pages...
            writer.WriteLine(@"<td class=""helpContent"">");

            this.RenderChildren(writer);

            writer.WriteLine(@"</td></tr></table>");
            writer.WriteLine("</div>");

            writer.EndRender();
        }

        #endregion

        /// <summary>
        /// The Yaf Help Navigation Class
        /// </summary>
        public class YafHelpNavigation
        {
            #region Properties

            /// <summary>
            ///   Gets or sets The Category of the Help Category
            /// </summary>
            public string HelpCategory { get; set; }

            /// <summary>
            ///   Gets or sets The Help pages
            /// </summary>
            public List<YafHelpNavigationPage> HelpPages { get; set; }

            #endregion
        }

        /// <summary>
        /// Class for the Help Pages inside a Category
        /// </summary>
        public class YafHelpNavigationPage
        {
            #region Properties

            /// <summary>
            ///   Gets or sets The Help page Name
            /// </summary>
            public string HelpPage { get; set; }

            #endregion
        }
    }
}