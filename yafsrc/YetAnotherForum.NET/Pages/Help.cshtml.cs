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
using YAF.Types.Interfaces.Services;
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
    /// Gets the examples of the BB code help, by group (each one with its live output).
    /// </summary>
    public List<BBCodeHelpGroup> BBCodeGroups { get; private set; } = [];

    /// <summary>
    /// Gets the other custom BB codes of the board (name and usage), they are not shown live (e.g. embedded media).
    /// </summary>
    public List<BBCodeHelpExample> CustomBBCodes { get; private set; } = [];

    /// <summary>
    /// Gets the achievements of the board (the help topic of the achievements lists them).
    /// </summary>
    public List<AchievementInfo> Achievements { get; private set; } = [];

    /// <summary>
    /// The BB codes of the help with their group, and the custom BB code they need (null for the built-in codes).
    /// The texts are on the language page HELP_BBCODES: [CODE]_TITLE, [CODE]_DESC and [CODE]_EXAMPLE.
    /// </summary>
    private static readonly (string Group, string Code, string CustomCode)[] BBCodeExamples =
    [
        ("TEXT", "B", null), ("TEXT", "I", null), ("TEXT", "U", null), ("TEXT", "S", null), ("TEXT", "H", null),
        ("TEXT", "COLOR", null), ("TEXT", "SIZE", null), ("TEXT", "FONT", null),
        ("LAYOUT", "ALIGN", null), ("LAYOUT", "INDENT", null), ("LAYOUT", "NOTE", null), ("LAYOUT", "HR", null),
        ("LAYOUT", "TABLE", null),
        ("LISTS", "LIST", null), ("LISTS", "LIST_ORDERED", null),
        ("LINKS", "URL", null), ("LINKS", "URL_TEXT", null), ("LINKS", "EMAIL", null), ("LINKS", "IMG", null),
        ("LINKS", "IMG_TEXT", null),
        ("QUOTES", "QUOTE", null), ("QUOTES", "QUOTE_NAME", null), ("QUOTES", "CODE", null),
        ("QUOTES", "CODE_LANGUAGE", null), ("QUOTES", "NOPARSE", null),
        ("SPECIAL", "USERLINK", "USERLINK"), ("SPECIAL", "SPOILER", "SPOILER"), ("SPECIAL", "HIDE", "HIDE"),
        ("SPECIAL", "HIDE_REPLY", "HIDE-REPLY"), ("SPECIAL", "HIDE_REPLY_THANKS", "HIDE-REPLY-THANKS"),
        ("SPECIAL", "HIDE_THANKS", "HIDE-THANKS"), ("SPECIAL", "HIDE_POSTS", "HIDE-POSTS"),
        ("SPECIAL", "GROUP_HIDE", "GROUP-HIDE"), ("SPECIAL", "EMOJI", null)
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpModel"/> class.
    /// </summary>
    public HelpModel()
        : base("HELP_INDEX", ForumPages.Help)
    {
    }

    /// <summary>
    /// Gets the language tag of the title of a help page (index and search results have the same title).
    /// </summary>
    /// <param name="faq">The help page.</param>
    /// <returns>The language tag.</returns>
    public static string TitleTag(string faq)
    {
        return faq.IsNotSet() || faq is "index" or "search" ? "SEARCHHELPTITLE" : $"{faq}TITLE";
    }

    /// <summary>
    /// Shows the help page.
    /// </summary>
    /// <param name="faq">The FAQ.</param>
    /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
    public async Task<IActionResult> OnGetAsync(string faq = null)
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

        // unknown help pages (every request would log a missing translation and reload the language file)
        if (helpContent.Count == 0 && faq is not "index" and not "search")
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.PageBoardContext.PageLinks.AddLink(this.GetText("HELP_INDEX", TitleTag(faq)), string.Empty);

        this.HelpContents = helpContent;

        if (faq == "bbcodes")
        {
            await this.LoadBBCodeExamplesAsync();
        }

        if (faq == "achievements" && this.PageBoardContext.BoardSettings.EnableAchievements)
        {
            this.Achievements = await this.Get<IAchievementService>().GetAchievementInfosAsync(
                                    this.PageBoardContext.PageBoardID,
                                    this.PageBoardContext.IsGuest ? null : this.PageBoardContext.PageUserID,
                                    false);
        }

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
        // the search shows the help as well
        if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowHelpTo))
        {
            return Task.FromResult(this.Get<ILinkBuilder>().AccessDenied());
        }

        if (this.Input.Search.IsNotSet())
        {
            return Task.FromResult<IActionResult>(this.Page());
        }

        this.LoadHelpContent();

        if (this.Input.Search.Length <= 3)
        {
            return Task.FromResult<IActionResult>(this.PageBoardContext.Notify(this.GetText("SEARCHLONGER"), MessageTypes.danger));
        }

        // the titles are text: they are encoded before the words are marked (the view showed the marks as text)
        var highlightWords = new List<string> { this.HtmlEncode(this.Input.Search) };

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
                    this.HtmlEncode(item.Title),
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
                // the labels of the forum and the links to other help topics and pages are tokens
                // the help is html, GetText would replace the BB codes of the examples (e.g. [b])
                var helpContent = HelpTextHelper.ReplaceTokens(
                    this.Get<ILocalization>().GetRawText("HELP_INDEX", $"{helpPage}CONTENT") ?? this.GetText($"{helpPage}CONTENT"),
                    (page, tag) => this.GetText(page, tag),
                    topic => this.Get<ILinkBuilder>().GetLink(ForumPages.Help, new { faq = topic }),
                    page => Enum.TryParse<ForumPages>(page, out var forumPage)
                                ? this.Get<ILinkBuilder>().GetLink(forumPage)
                                : null);

                this.helpContents.Add(
                    new HelpContent
                    {
                        HelpPage = helpPage, Title = this.GetText($"{helpPage}TITLE"), Content = helpContent
                    });
            });
    }
    /// <summary>
    /// Renders the examples of the BB code help with the BB codes of the board (the same output as in a post).
    /// </summary>
    private async Task LoadBBCodeExamplesAsync()
    {
        var customCodes = (await this.Get<IBBCodeService>().GetCustomBBCodesAsync()).ToList();

        var logo = $"{this.Get<BoardInfo>().ForumBaseUrl}/{this.Get<BoardFolders>().Logos}/YAFLogo.svg";

        foreach (var (group, code, customCode) in BBCodeExamples)
        {
            // a custom BB code the administrator has deleted is not explained
            if (customCode != null && !customCodes.Exists(c => c.Name.Equals(customCode, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            // the example is BB code, GetText would replace [b] and [noparse]
            var example = (this.Get<ILocalization>().GetRawText("HELP_BBCODES", $"{code}_EXAMPLE") ?? string.Empty)
                .Replace("{logo}", logo)
                .Replace("{user}", this.PageBoardContext.PageUser.Name);

            var helpGroup = this.BBCodeGroups.Find(g => g.Group == group);

            if (helpGroup == null)
            {
                helpGroup = new BBCodeHelpGroup { Group = group, Title = this.GetText("HELP_BBCODES", $"GROUP_{group}") };
                this.BBCodeGroups.Add(helpGroup);
            }

            helpGroup.Examples.Add(
                new BBCodeHelpExample
                {
                    Title = this.GetText("HELP_BBCODES", $"{code}_TITLE"),
                    Description = this.GetText("HELP_BBCODES", $"{code}_DESC"),
                    Example = example,
                    // prepared like the text of a post when it is saved
                    Output = await this.Get<IFormatMessage>().FormatMessageWithAllBBCodesAsync(
                             HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(example)),
                             0)
                });
        }

        // the other custom BB codes (embedded media, attachments, ...) with their usage
        this.CustomBBCodes =
        [
            .. customCodes
                .Where(c => !Array.Exists(BBCodeExamples, e => c.Name.Equals(e.CustomCode, StringComparison.OrdinalIgnoreCase)))
                .Select(c => new BBCodeHelpExample
                             {
                                 Title = c.Name,
                                 Example = this.Get<IBBCodeService>().LocalizeCustomBBCodeElement(c.Description ?? string.Empty)
                             })
        ];
    }

    /// <summary>
    /// A group of the BB code help.
    /// </summary>
    public class BBCodeHelpGroup
    {
        /// <summary>
        /// Gets or sets the group (the language tag GROUP_[group]).
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the examples.
        /// </summary>
        public List<BBCodeHelpExample> Examples { get; } = [];
    }

    /// <summary>
    /// A BB code of the help.
    /// </summary>
    public class BBCodeHelpExample
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description (html).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the example (BB code, text).
        /// </summary>
        public string Example { get; set; }

        /// <summary>
        /// Gets or sets the output of the example (html), null for the BB codes that are not shown live.
        /// </summary>
        public string Output { get; set; }
    }
}
