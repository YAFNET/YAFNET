/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2024 Ingo Herbote
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

namespace YAF.Web.Controls;

using System.Runtime.Remoting.Contexts;
using System.Xml.Serialization;

using YAF.Types.Objects;

/// <summary>
/// Renders the Help Menu on the Help Pages.
/// </summary>
public class HelpMenu : BasePanel
{
    /// <summary>
    /// The List with the Help Navigation Items
    /// </summary>
    private List<HelpNavigation> helpNavList = [];

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        var serializer = new XmlSerializer(typeof(List<HelpNavigation>));

        var xmlFilePath = this.Get<HttpContextBase>().Server.MapPath(
            $"{BoardInfo.ForumServerFileRoot}Resources/HelpMenuList.xml");

        if (File.Exists(xmlFilePath))
        {
            var reader = new StreamReader(xmlFilePath);

            this.helpNavList = (List<HelpNavigation>)serializer.Deserialize(reader);

            reader.Close();
        }

        var html = new StringBuilder();
        var htmlDropDown = new StringBuilder();

        htmlDropDown.Append("""<div class="dropdown d-lg-none d-grid gap-2">""");

        htmlDropDown.Append(
            """<button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">""");

        htmlDropDown.AppendFormat(@"{0}</button>", this.GetText("HELP_INDEX", "INDEX"));

        htmlDropDown.Append(
            """<div class="dropdown-menu scrollable-dropdown" aria-labelledby="dropdownMenuButton">""");

        var faqPage = "index";

        if (this.Get<HttpRequestBase>().QueryString.Exists("faq"))
        {
            faqPage = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("faq");
        }

        // Index / Search
        var indexContent =
            $"""<a href="{this.Get<LinkBuilder>().GetLink(ForumPages.Help, new { faq = "index" })}">{this.Get<ILocalization>().GetText("HELP_INDEX", "INDEX")} / {this.Get<ILocalization>().GetText("BTNSEARCH")}</a>""";

        htmlDropDown.AppendFormat(
            """<a href="{1}" class="dropdown-item">{0}</a>""",
            this.GetText("BTNSEARCH"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Help, new { faq = "index" }));

        html.Append("""<div class="accordion">""");

        // Render Index Item
        html.Append("""<div class="accordion-item">""");

        html.AppendFormat(
            """
            <h2 class="accordion-header"><button class="accordion-button{3}" data-bs-toggle="collapse" aria-expanded="{2}" data-bs-target="#{1}" type="button">
                                          {0}
                                      </button></h2>
            """
            ,
            this.Get<ILocalization>().GetText("HELP_INDEX", "INDEX"),
            "index",
            faqPage.Equals("index"),
            faqPage.Equals("index") ? "" : " collapsed");

        html.AppendFormat(
            faqPage.Equals("index")
                ? """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse show" id="index">"""
                : """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse" id="index">""");

        html.AppendFormat(faqPage.Equals("index") ? "<li class=\"fw-bold\">{0}</li>" : "<li>{0}</li>", indexContent);

        html.Append("</ul></div>");

        this.helpNavList.ForEach(
            category =>
                {
                    var expanded = category.HelpPages.Exists(p => p.HelpPage.ToLower().Equals(faqPage));

                    html.Append("""<div class="accordion-item">""");

                    html.AppendFormat(
                        """
                        <h2 class="accordion-header"><button class="accordion-button{3}" data-bs-toggle="collapse" aria-expanded="{2}" data-bs-target="#{1}" type="button">
                                                      {0}
                                                  </button></h2>
                        """,
                        this.Get<ILocalization>().GetText("HELP_INDEX", category.HelpCategory),
                        category.HelpCategory,
                        expanded.ToString().ToLower(),
                        expanded ? "" : " collapsed");

                    htmlDropDown.AppendFormat(
                        """<h6 class="dropdown-header">{0}</h6>""",
                        this.GetText("HELP_INDEX", category.HelpCategory));

                    html.AppendFormat(
                        expanded
                            ? """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse show" id="{0}">"""
                            : """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse" id="{0}">""",
                        category.HelpCategory);

                    category.HelpPages.ForEach(
                        helpPage =>
                            {
                                var selectedStyle = string.Empty;

                                if (helpPage.HelpPage.ToLower().Equals(faqPage))
                                {
                                    selectedStyle = " fw-bold";
                                }

                                if (helpPage.HelpPage.Equals("REGISTRATION"))
                                {
                                    if (this.PageBoardContext.BoardSettings.DisableRegistrations || Config.IsAnyPortal)
                                    {
                                        return;
                                    }

                                    html.AppendFormat(
                                        """<li><a href="{0}" title="{1}" class="{2}">{1}</a></li>""",
                                        this.Get<LinkBuilder>().GetLink(
                                            ForumPages.Help,
                                            new { faq = helpPage.HelpPage.ToLower() }),
                                        this.GetText("HELP_INDEX", $"{helpPage.HelpPage}TITLE"),
                                        selectedStyle);

                                    htmlDropDown.AppendFormat(
                                        """<a href="{0}" class="dropdown-item">{1}</a>""",
                                        this.Get<LinkBuilder>().GetLink(
                                            ForumPages.Help,
                                            new { faq = helpPage.HelpPage.ToLower() }),
                                        this.GetText("HELP_INDEX", $"{helpPage.HelpPage}TITLE"));
                                }
                                else
                                {
                                    html.AppendFormat(
                                        """<li><a href="{0}" title="{1}" class="{2}">{1}</a></li>""",
                                        this.Get<LinkBuilder>().GetLink(
                                            ForumPages.Help,
                                            new { faq = helpPage.HelpPage.ToLower() }),
                                        this.GetText("HELP_INDEX", $"{helpPage.HelpPage}TITLE"),
                                        selectedStyle);

                                    htmlDropDown.AppendFormat(
                                        """<a href="{0}" class="dropdown-item">{1}</a>""",
                                        this.Get<LinkBuilder>().GetLink(
                                            ForumPages.Help,
                                            new { faq = helpPage.HelpPage.ToLower() }),
                                        this.GetText("HELP_INDEX", $"{helpPage.HelpPage}TITLE"));
                                }
                            });

                    html.Append(@"</ul></div>");
                });

        htmlDropDown.Append(@"</div></div>");

        writer.BeginRender();

        // render the contents of the help menu....
        writer.WriteLine(
            """<div class="col-md-3 d-none d-lg-block">""");

        writer.Write(html.ToString());

        writer.WriteLine(@"</div>");

        writer.WriteLine("</div>");

        // Write Mobile Drop down
        writer.WriteLine(htmlDropDown.ToString());

        // contents of the help pages...
        writer.WriteLine("""<div class="col flex-grow-1 ms-lg-3">""");

        this.RenderChildren(writer);

        writer.EndRender();
    }
}