/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

        var content = new HtmlContentBuilder();

        var helpNavList = BoardContext.Current.Get<IDataCache>().GetOrSet(
            "HelpNavigation",
            StaticDataHelper.LoadHelpMenuJson);

        var html = new StringBuilder();
        var htmlDropDown = new StringBuilder();


        htmlDropDown.Append("""<div class="dropdown d-lg-none d-grid gap-2">""");

        htmlDropDown.Append(
            """<button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">""");

        htmlDropDown.Append($"{context.Get<ILocalization>().GetText("HELP_INDEX", "INDEX")}</button>");

        htmlDropDown.Append(
            """<div class="dropdown-menu scrollable-dropdown" aria-labelledby="dropdownMenuButton">""");

        var faqPage = "index";

        if (htmlHelper.ViewContext.HttpContext.Request.RouteValues.TryGetValue("faq", out var routeValue))
        {
            faqPage = routeValue.ToString();
        }

        // Index / Search
        var indexContent =
            $"""<a href="{context.Get<ILinkBuilder>().GetLink(ForumPages.Help, new { faq = "index" })}">{context.Get<ILocalization>().GetText("HELP_INDEX", "INDEX")} / {context.Get<ILocalization>().GetText("BTNSEARCH")}</a>""";

        htmlDropDown.AppendFormat(
            """<a href="{1}" class="dropdown-item">{0}</a>""",
            context.Get<ILocalization>().GetText("BTNSEARCH"),
            context.Get<ILinkBuilder>().GetLink(ForumPages.Help, new { faq = "index" }));

        html.Append("""<nav><div class="accordion">""");

        // Render Index Item
        html.Append("""<div class="accordion-item">""");

        html.AppendFormat(
            """
            <h2 class="accordion-header"><button class="accordion-button{3}" data-bs-toggle="collapse" aria-expanded="{2}" data-bs-target="#{1}" type="button">
                                          {0}
                                      </button></h2>
            """,
            context.Get<ILocalization>().GetText("HELP_INDEX", "INDEX"),
            "index",
            faqPage.Equals("index"),
            faqPage.Equals("index") ? "" : " collapsed");

        html.AppendFormat(
            faqPage.Equals("index")
                ? """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse show" id="index">"""
                : """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse" id="index">""");

        html.AppendFormat(faqPage.Equals("index") ? "<li class=\"fw-bold\">{0}</li>" : "<li>{0}</li>", indexContent);

        html.Append("</ul></div>");

        helpNavList.ForEach(
            category =>
                {
                    var expanded = category.HelpPages.Exists(p => p.ToLower().Equals(faqPage));

                    html.Append("""<div class="accordion-item">""");

                    html.AppendFormat(
                        """
                        <h2 class="accordion-header"><button class="accordion-button{3}" data-bs-toggle="collapse" aria-expanded="{2}" data-bs-target="#{1}" type="button">
                                                      {0}
                                                  </button></h2>
                        """,
                        context.Get<ILocalization>().GetText("HELP_INDEX", category.HelpCategory),
                        category.HelpCategory,
                        expanded.ToString().ToLower(),
                        expanded ? "" : " collapsed");

                    htmlDropDown.Append(
                        $"""<h6 class="dropdown-header">{context.Get<ILocalization>().GetText("HELP_INDEX", category.HelpCategory)}</h6>""");

                    html.AppendFormat(
                        expanded
                            ? """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse show" id="{0}">"""
                            : """<ul class="list-unstyled pb-0 accordion-body accordion-collapse collapse" id="{0}">""",
                        category.HelpCategory);

                    category.HelpPages.ForEach(
                        helpPage =>
                            {
                                var selectedStyle = helpPage.ToLower().Equals(faqPage) ? "fw-bold" : string.Empty;

                                var link = context.Get<ILinkBuilder>().GetLink(
                                    ForumPages.Help,
                                    new { faq = helpPage.ToLower() });

                                if (helpPage.Equals("REGISTRATION"))
                                {
                                    if (context.BoardSettings.DisableRegistrations)
                                    {
                                        return;
                                    }

                                    html.AppendFormat(
                                        """<li><a href="{0}" title="{1}" class="{2}">{1}</a></li>""",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"),
                                        selectedStyle);

                                    htmlDropDown.AppendFormat(
                                        """<a href="{0}" class="dropdown-item">{1}</a>""",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"));
                                }
                                else
                                {
                                    html.AppendFormat(
                                        """<li><a href="{0}" title="{1}" class="{2}">{1}</a></li>""",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"),
                                        selectedStyle);

                                    htmlDropDown.AppendFormat(
                                        """<a href="{0}" class="dropdown-item">{1}</a>""",
                                        link,
                                        context.Get<ILocalization>().GetText("HELP_INDEX", $"{helpPage}TITLE"));
                                }
                            });

                    html.Append("</ul></div>");
                });

        html.Append("</div></nav>");

        htmlDropDown.Append("</div></div>");

        // render the contents of the help menu....
        content.AppendHtml(
            """<div class="col-md-3 d-none d-lg-block">""");

        content.AppendHtml(html.ToString());

        content.AppendHtml("</div>");

        // Write Mobile Drop down
        content.AppendHtml(htmlDropDown.ToString());

        return content;
    }
}