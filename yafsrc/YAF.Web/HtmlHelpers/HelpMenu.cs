/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
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

namespace YAF.Web.HtmlHelpers;

using Microsoft.Extensions.Configuration;

/// <summary>
/// The Help Menu html helper.
/// </summary>
public static class HelpMenuHtmlHelper
{
    /// <summary>
    /// The help menu.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent HelpMenu(this IHtmlHelper htmlHelper)
    {
        var context = BoardContext.Current;

        List<HelpNavigation> helpNavList = new ();

        var content = new HtmlContentBuilder();

        var config = context.Get<IConfiguration>();

        config.GetSection(nameof(HelpNavigation)).Bind(helpNavList);

        var html = new StringBuilder();
        var htmlDropDown = new StringBuilder();

        htmlDropDown.Append(@"<div class=""dropdown d-lg-none d-grid gap-2"">");

        htmlDropDown.Append(
            @"<button class=""btn btn-secondary dropdown-toggle"" type=""button"" id=""dropdownMenuButton"" data-bs-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">");

        htmlDropDown.AppendFormat(@"{0}</button>", context.Get<ILocalization>().GetText("HELP_INDEX", "INDEX"));

        htmlDropDown.Append(
            @"<div class=""dropdown-menu scrollable-dropdown"" aria-labelledby=""dropdownMenuButton"">");

        var faqPage = "index";

        if (htmlHelper.ViewContext.HttpContext.Request.RouteValues.ContainsKey("faq"))
        {
            faqPage = htmlHelper.ViewContext.HttpContext.Request.RouteValues["faq"].ToString();
        }

        // Index / Search
        var indexTag = new TagBuilder("h6");

        var indexContent =
            $@"<a href=""{context.Get<LinkBuilder>().GetLink(ForumPages.Help, new { faq = "index" })}"">{context.Get<ILocalization>().GetText("HELP_INDEX", "INDEX")} / {context.Get<ILocalization>().GetText("BTNSEARCH")}</a>";

        indexTag.AddCssClass("h6 pt-4 pb-3 mb-4 border-bottom text-uppercase");

        if (faqPage.Equals("index"))
        {
            indexTag.AddCssClass("fw-bold");
        }

        indexTag.InnerHtml.AppendHtml(indexContent);

        htmlDropDown.AppendFormat(
            @"<a href=""{1}"" class=""dropdown-item"">{0}</a>",
            context.Get<ILocalization>().GetText("BTNSEARCH"),
            context.Get<LinkBuilder>().GetLink(ForumPages.Help, new { faq = "index" }));

        html.Append(@"<nav><div class=""accordion accordion-flush"">");

        helpNavList.ForEach(
            category =>
                {
                    var expanded = category.HelpPages.Any(p => p.ToLower().Equals(faqPage));

                    html.Append(@"<div class=""accordion-item"">");

                    html.AppendFormat(
                        @"<h2 class=""accordion-header""><button class=""h2 accordion-button collapsed ps-0"" data-bs-toggle=""collapse"" aria-expanded=""{2}"" data-bs-target=""#{1}"" type=""button"">
                              {0}
                          </button></h2>",
                        context.Get<ILocalization>().GetText("HELP_INDEX", category.HelpCategory),
                        category.HelpCategory,
                        expanded.ToString().ToLower());

                    htmlDropDown.AppendFormat(
                        @"<h6 class=""dropdown-header"">{0}</h6>",
                        context.Get<ILocalization>().GetText("HELP_INDEX", category.HelpCategory));

                    html.AppendFormat(
                        expanded
                            ? @"<ul class=""list-unstyled ps-3 accordion-collapse collapse show"" id=""{0}"">"
                            : @"<ul class=""list-unstyled ps-3 accordion-collapse collapse"" id=""{0}"">",
                        category.HelpCategory);

                    category.HelpPages.ForEach(
                        helpPage =>
                            {
                                var selectedStyle = helpPage.ToLower().Equals(faqPage) ? "fw-bold" : string.Empty;

                                var link = context.Get<LinkBuilder>().GetLink(
                                    ForumPages.Help,
                                    new { faq = helpPage.ToLower() });

                                if (helpPage.Equals("REGISTRATION"))
                                {
                                    if (context.BoardSettings.DisableRegistrations)
                                    {
                                        return;
                                    }

                                    html.AppendFormat(
                                        @"<li><a href=""{0}"" title=""{1}"" class=""{2}"">{1}</a></li>",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"),
                                        selectedStyle);

                                    htmlDropDown.AppendFormat(
                                        @"<a href=""{0}"" class=""dropdown-item"">{1}</a>",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"));
                                }
                                else
                                {
                                    html.AppendFormat(
                                        @"<li><a href=""{0}"" title=""{1}"" class=""{2}"">{1}</a></li>",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"),
                                        selectedStyle);

                                    htmlDropDown.AppendFormat(
                                        @"<a href=""{0}"" class=""dropdown-item"">{1}</a>",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"));
                                }
                            });

                    html.Append(@"</ul></div>");
                });

        html.Append("</div></nav>");

        htmlDropDown.Append(@"</div></div>");

        // render the contents of the help menu....
        content.AppendHtml(
            @"<div class=""col-md-3 d-none d-lg-block bg-light sidebar""><div class=""sidebar-sticky"">");

        content.AppendHtml(indexTag);

        content.AppendHtml(html.ToString());

        content.AppendHtml(@"</div></div>");

        // Write Mobile Drop down
        content.AppendHtml(htmlDropDown.ToString());

        return content;
    }
}