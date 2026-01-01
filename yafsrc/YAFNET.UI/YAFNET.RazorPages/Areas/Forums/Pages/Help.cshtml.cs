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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Objects;

/// <summary>
/// The Help Index.
/// </summary>
public class HelpModel : ForumPage
{
    private List<HelpContent> helpContents { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public HelpInputModel Input { get; set; }

    [BindProperty]
    public List<HelpContent> HelpContents { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpModel"/> class.
    /// </summary>
    public HelpModel()
        : base("HELP_INDEX", ForumPages.Help)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="faq">The FAQ.</param>
    /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
    public IActionResult OnGet(string faq = null)
    {
        if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowHelpTo))
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("SUBTITLE"), this.Get<ILinkBuilder>().GetLink(ForumPages.Help));

        this.LoadHelpContent();

        if (faq.IsNotSet())
        {
            faq = "index";
        }

        var helpContent = this.helpContents.FindAll(check => check.HelpPage.ToLower().Equals(faq));

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("HELP_INDEX", faq != "index" ? $"{faq}TITLE" : "SEARCHHELPTITLE"),
            string.Empty);

        this.HelpContents = [];

        this.HelpContents = helpContent;

        return this.Page();
    }

    /// <summary>
    /// The on post register async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<IActionResult> OnPostAsync()
    {
        if (this.Input.Search.IsNotSet())
        {
            return Task.FromResult<IActionResult>(this.Page());
        }

        this.LoadHelpContent();

        if (this.Input.Search.Length <= 3)
        {
            return Task.FromResult<IActionResult>(this.PageBoardContext.Notify(this.GetText("SEARCHLONGER"), MessageTypes.danger));
        }

        var highlightWords = new List<string> { this.Input.Search };

        var searchList = this.helpContents.FindAll(
            check => check.Content.Contains(this.Input.Search, StringComparison.CurrentCultureIgnoreCase) ||
                     check.Title.Contains(this.Input.Search, StringComparison.CurrentCultureIgnoreCase));

        searchList.ForEach(
            item =>
            {
                item.Content = this.Get<IFormatMessage>().SurroundWordList(
                    item.Content,
                    highlightWords,
                    "<mark>",
                    "</mark>");
                item.Title = this.Get<IFormatMessage>().SurroundWordList(
                    item.Title,
                    highlightWords,
                    "<mark>",
                    "</mark>");
            });

        if (searchList.Count.Equals(0))
        {
            return Task.FromResult<IActionResult>(this.PageBoardContext.Notify(this.GetText("NORESULTS"), MessageTypes.warning));
        }

        this.HelpContents = searchList;

        return Task.FromResult<IActionResult>(this.Page());
    }

    /// <summary>
    /// Load the Complete Help Pages From the language File.
    /// </summary>
    private void LoadHelpContent()
    {
        var helpNavigation = this.Get<IDataCache>().GetOrSet(
            "HelpNavigation",
            StaticDataHelper.LoadHelpMenuJson);

        this.helpContents = [];

        helpNavigation.SelectMany(category => category.HelpPages).ForEach(
            helpPage =>
            {
                var helpContent = helpPage switch
                {
                    "RECOVER" => this.GetTextFormatted(
                        $"{helpPage}CONTENT",
                        this.Get<ILinkBuilder>().GetLink(ForumPages.Account_ForgotPassword)),
                    "BBCODES" => this.GetTextFormatted($"{helpPage}CONTENT", this.Get<BoardInfo>().ForumBaseUrl),
                    "POSTING" => this.GetTextFormatted(
                        $"{helpPage}CONTENT",
                        this.Get<ILinkBuilder>().GetLink(ForumPages.Help, "faq=bbcodes")),
                    _ => this.GetText($"{helpPage}CONTENT")
                };

                this.helpContents.Add(
                    new HelpContent
                    {
                        HelpPage = helpPage, Title = this.GetText($"{helpPage}TITLE"), Content = helpContent
                    });
            });
    }
}