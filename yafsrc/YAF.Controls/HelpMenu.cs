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
    using YAF.Types.Extensions;
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
            var serializer = new XmlSerializer(typeof(List<YafHelpNavigation>));

            var xmlFilePath =
                HttpContext.Current.Server.MapPath(
                    "{0}Resources/{1}".FormatWith(YafForumInfo.ForumServerFileRoot, "HelpMenuList.xml"));

            if (File.Exists(xmlFilePath))
            {
                var reader = new StreamReader(xmlFilePath);

                this.helpNavList = (List<YafHelpNavigation>)serializer.Deserialize(reader);

                reader.Close();
            }

            var html = new StringBuilder(2000);

            var faqPage = "index";

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq").IsSet())
            {
                faqPage = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq");
            }

            // Header
            html.Append(@"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafhelpmenu"">");

            html.Append(@"<tr><td class=""header1"">{0}</td></tr>".FormatWith(this.GetText("HELP_INDEX", "NAVIGATION")));

            html.Append(@"<tr class=""header2""><td>{0}</td></tr>".FormatWith(this.GetText("HELP_INDEX", "INDEX")));

            html.AppendFormat(@"<tr><td class=""post""><ul id=""yafhelpindex"">");

            var selectedStyle = string.Empty;

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