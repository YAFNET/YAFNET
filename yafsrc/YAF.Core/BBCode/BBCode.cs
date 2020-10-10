/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.BBCode
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Compilation;
    using System.Web.UI;

    using YAF.Configuration;
    using YAF.Core.BBCode.ReplaceRules;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Yaf BBCode Class to Format Message From BB Code to HTML and Reverse.
    /// </summary>
    public class BBCode : IBBCode, IHaveServiceLocator
    {
        /* Ederon : 6/16/2007 - conventions */

        #region Constants and Fields

        /// <summary>
        ///   The _options.
        /// </summary>
        private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

        /// <summary>
        ///   The _rgx bb code localization tag.
        /// </summary>
        private const string _RgxBBCodeLocalizationTag =
            @"\[localization=(?<tag>[^\]]*)\](?<inner>(.+?))\[/localization\]";

        /// <summary>
        ///   The _rgx noparse.
        /// </summary>
        private static readonly Regex _rgxNoParse = new Regex(
            @"\[noparse\](?<inner>(.*?))\[/noparse\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx bold.
        /// </summary>
        private static readonly Regex _rgxBold = new Regex(
            @"\[B\](?<inner>(.*?))\[/B\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx br.
        /// </summary>

        // private const string _RgxBr = "[\r]?\n(?!.*<[^>]+>.*)"; // "[\r]?\n";
        private const string _RgxBr = "\r\n";

        /// <summary>
        ///   The _rgx color.
        /// </summary>
        private const string _RgxColor = @"\[color=(?<color>(\#?[-a-z0-9]*))\](?<inner>(.*?))\[/color\]";

        /// <summary>
        ///   The _rgx font.
        /// </summary>
        private const string _RgxFont = @"\[font=(?<font>([-a-z0-9, ]*))\](?<inner>(.*?))\[/font\]";

        /// <summary>
        ///   The _rgx Highlighted.
        /// </summary>
        private const string _RgxHighlighted = @"\[h\](?<inner>(.*?))\[/h\]";

        /// <summary>
        /// The _rgx easy quote.
        /// </summary>
        // private const string _RgxEasyQuote = @"(\>)\s(?<inner>(.*?))$";

        /// <summary>
        ///   The _rgx email 1.
        /// </summary>
        private static readonly Regex _rgxEmail1 = new Regex(
            @"\[email[^\]]*\](?<inner>([^""\r\n\]\[]+?))\[/email\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx email 2.
        /// </summary>
        private static readonly Regex _rgxEmail2 = new Regex(
            @"\[email=(?<email>[^\]]*)\](?<inner>([^""\r\n\]\[]+?))\[/email\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx code 1.
        /// </summary>
        private static readonly Regex _rgxCode1 = new Regex(@"\[code\](?<inner>(.*?))\[/code\]\r\n|\[code\](?<inner>(.*?))\[/code\]", Options);

        /// <summary>
        ///   The regex code with language string.
        /// </summary>
        private static readonly Regex _regexCodeWithLanguage = new Regex(
            @"\[code=(?<language>[^\]]*)\](?<inner>(.*?))\[/code\]\r\n|\[code=(?<language>[^\]]*)\](?<inner>(.*?))\[/code\]",
            Options);

        /// <summary>
        ///   The _rgx hr.
        /// </summary>
        private const string _RgxHr = "^[-][-][-][-][-]*[\r]?[\n]";

        /// <summary>
        ///   The _rgx img.
        /// </summary>
        private static readonly Regex _rgxImg = new Regex(
            @"\[img\](?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>((?!.+logout)[^""\r\n\]\[]+?\.((googleusercontent[^\[]*)|(jpg[^\[]*)|(jpeg[^\[]*)|(bmp[^\[]*)|(png[^\[]*)|(gif[^\[]*)|(tif[^\[]*)|(ashx[^\[]*)|(php[^\[]*)|(aspx[^\[]*))))\[/img\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx img Title.
        /// </summary>
        private static readonly Regex _rgxImgEmptyTitle = new Regex(
            @"\[img=(?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>((?!.+logout)[^""\r\n\]\[]+?\.((googleusercontent[^\[]*)|(jpg[^\]\[/img\]]*)|(jpeg[^\[\[/img\]]*)|(bmp[^\[\[/img\]]*)|(png[^\]\[/img\]]*)|(gif[^\]\[/img\]]*)|(tif[^\]\[/img\]]*)|(ashx[^\]\[/img\]]*)|(php[^\]\[/img\]]*)|(aspx[^\]\[/img\]]*))))\]\[/img\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx img Title.
        /// </summary>
        private static readonly Regex _rgxImgTitle = new Regex(
            @"\[img=(?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>((?!.+logout)[^""\r\n\]\[]+?\.((googleusercontent[^\[]*)|(jpg[^\]]*)|(jpeg[^\]]*)|(bmp[^\]]*)|(png[^\]]*)|(gif[^\]]*)|(tif[^\]]*)|(ashx[^\]]*)|(php[^\]]*)|(aspx[^\]]*))))\](?<description>[^\[]*)\[/img\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx italic.
        /// </summary>
        private const string _RgxItalic = @"\[I\](?<inner>(.*?))\[/I\]";

        /// <summary>
        ///   The _rgx center.
        /// </summary>
        private const string _RgxCenter = @"\[center\](?<inner>(.*?))\[/center\]";

        /// <summary>
        ///   The _rgx left.
        /// </summary>
        private const string _RgxLeft = @"\[left\](?<inner>(.*?))\[/left\]";

        /// <summary>
        ///   The _rgx right.
        /// </summary>
        private const string _RgxRight = @"\[right\](?<inner>(.*?))\[/right\]";

        /// <summary>
        ///   The _rgx right.
        /// </summary>
        private const string _RgxIndent = @"\[indent\](?<inner>(.*?))\[/indent\]";

        /// <summary>
        ///   The _rgx bullet.
        /// </summary>
        private const string _RgxBullet = @"\[\*\]";

        /// <summary>
        ///   The _rgx list 1.
        /// </summary>
        private const string _RgxList1 = @"\[list\](?<inner>(.*?))\[/list\]";

        /// <summary>
        ///   The List Number Regex
        /// </summary>
        private const string _RgxListNumber = @"\[list=1\](?<inner>(.*?))\[/list\]";

        /// <summary>
        ///   The List Lower Roman Regex
        /// </summary>
        private const string _RgxListLowerRoman = @"\[list=i\](?<inner>(.*?))\[/list\]";

        /// <summary>
        /// The List Upper Roman Regex
        /// </summary>
        private const string _RgxListUpperRoman = @"\[list=I\](?<inner>(.*?))\[/list\]";

        /// <summary>
        ///   The List Lower Alphabet Regex
        /// </summary>
        private const string _RgxListLowerAlpha = @"\[list=a\](?<inner>(.*?))\[/list\]";

        /// <summary>
        /// The List Upper Alphabet Regex
        /// </summary>
        private const string _RgxListUpperAlpha = @"\[list=A\](?<inner>(.*?))\[/list\]";

        /// <summary>
        /// The open quote regex
        /// </summary>
        private const string OpenQuoteRegex = @"\[quote\]";

        /// <summary>
        /// The close quote regex
        /// </summary>
        private const string CloseQuoteRegex = @"\[/quote\]";

        /// <summary>
        /// The open quote user id regex
        /// </summary>
        private const string OpenQuoteUserIdRegex = @"\[quote=(?<quote>(.*?))]";

        /// <summary>
        ///   The _rgx size.
        /// </summary>
        // private const string _RgxSize = @"\[size=(?<size>([1-9]))\](?<inner>(.*?))\[/size\]";

        /// <summary>
        ///   The _rgx strike.
        /// </summary>
        private const string _RgxStrike = @"\[S\](?<inner>(.*?))\[/S\]";

        /// <summary>
        ///   The _rgx topic.
        /// </summary>
        private const string _RgxTopic = @"\[topic=(?<topic>[0-9]*)\](?<inner>(.*?))\[/topic\]";

        /// <summary>
        ///   The _rgx post.
        /// </summary>
        private const string _RgxPost = @"\[post=(?<post>[0-9]*)\](?<inner>(.*?))\[/post\]";

        /// <summary>
        ///   The _rgx underline.
        /// </summary>
        private const string _RgxUnderline = @"\[U\](?<inner>(.*?))\[/U\]";

        /// <summary>
        ///   The _rgx url 1.
        /// </summary>
        private static readonly Regex _rgxModalUrl1 = new Regex(
            @"\[modalurl](?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>([^javascript:])(.+?))\[/modalurl\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx url 2.
        /// </summary>
        private static readonly Regex _rgxModalUrl2 = new Regex(
            @"\[modalurl\=(?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))(?<url>(?<inner>([^javascript:])([^""\r\n\]\[]*?))\](.+?))\[/modalurl\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx url 1.
        /// </summary>
        private static readonly Regex _rgxUrl1 = new Regex(
            @"\[url\](?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://)|(mailto:))?(?<inner>([^javascript:])(.+?))\[/url\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx url 2.
        /// </summary>
        private static readonly Regex _rgxUrl2 = new Regex(
            @"\[url\=(?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))?(?<url>([^javascript:])([^""\r\n\]\[]*?))\](?<inner>(.+?))\[/url\]",
            Options | RegexOptions.Compiled);

        /// <summary>
        /// The URL Regex
        /// </summary>
        private static readonly Regex _RgxUrl3 = new Regex(
            @"^(?!.*youtu).*(?<before>^|[ ]|\[[A-Za-z0-9]\]|\[\*\]|[A-Za-z0-9])(?<!"")(?<!href="")(?<!src="")(?<inner>(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// The URL Regex
        /// </summary>
        private static readonly Regex _RgxUrl4 = new Regex(
            @"^(?!.*youtu).*(?<before>^|[ ]|\[[A-Za-z0-9]\]|\[\*\]|[A-Za-z0-9])(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=+;,:#~/(/)$]*[^.<|^.\[])?)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);


        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCode"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="processReplaceRulesFactory">
        /// The process replace rules factory.
        /// </param>
        public BBCode(
            IServiceLocator serviceLocator,
            Func<IEnumerable<bool>, IProcessReplaceRules> processReplaceRulesFactory)
        {
            this.ServiceLocator = serviceLocator;
            this.ProcessReplaceRulesFactory = processReplaceRulesFactory;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets ProcessReplaceRulesFactory.
        /// </summary>
        public Func<IEnumerable<bool>, IProcessReplaceRules> ProcessReplaceRulesFactory { get; set; }

        /// <summary>
        ///   Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        /// <summary>
        /// Gets CustomBBCode.
        /// </summary>
        protected IDictionary<Types.Models.BBCode, Regex> CustomBBCode
        {
            get
            {
                return this.Get<IObjectStore>().GetOrSet(
                    "CustomBBCodeRegExDictionary",
                    () =>
                        {
                            var bbcodeTable = this.Get<DataBroker>().GetCustomBBCode();
                            return bbcodeTable
                                .Where(b => (b.UseModule ?? false) && b.ModuleClass.IsSet() && b.SearchRegex.IsSet())
                                .ToDictionary(codeRow => codeRow, codeRow => new Regex(codeRow.SearchRegex, Options));
                        });
            }
        }

        #region Implemented Interfaces

        #region IBBCode

        /// <summary>
        /// Formats the message with Custom BBCodes
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="flags">
        /// The Message flags.
        /// </param>
        /// <param name="displayUserId">
        /// The display user id.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// Returns the formatted Message.
        /// </returns>
        public string FormatMessageWithCustomBBCode(string message, [NotNull] MessageFlags flags, int? displayUserId, int? messageId)
        {
            var workingMessage = message;

            // handle custom bbcodes row by row...
            this.CustomBBCode.ForEach(
                keyPair =>
                {
                    var codeRow = keyPair.Key;

                    Match match;

                    do
                    {
                        match = keyPair.Value.Match(workingMessage);

                        if (!match.Success)
                        {
                            continue;
                        }

                        var sb = new StringBuilder();

                        var paramDic = new Dictionary<string, string> { { "inner", match.Groups["inner"].Value } };

                        if (codeRow.Variables.IsSet() && codeRow.Variables.Split(';').Any())
                        {
                            var vars = codeRow.Variables.Split(';');

                            vars.Where(v => match.Groups[v] != null).ForEach(
                                v => paramDic.Add(v, match.Groups[v].Value));
                        }

                        sb.Append(workingMessage.Substring(0, match.Groups[0].Index));

                        // create/render the control...
                        var module = BuildManager.GetType(codeRow.ModuleClass, true, false);
                        var customModule = (BBCodeControl)Activator.CreateInstance(module);

                        // assign parameters...
                        customModule.CurrentMessageFlags = flags;
                        customModule.DisplayUserID = displayUserId;
                        customModule.MessageID = messageId;
                        customModule.Parameters = paramDic;

                        // render this control...
                        sb.Append(customModule.RenderToString());

                        sb.Append(workingMessage.Substring(match.Groups[0].Index + match.Groups[0].Length));

                        workingMessage = sb.ToString();
                    }
                    while (match.Success);
                });

            return workingMessage;
        }

        /// <summary>
        /// Converts a message containing YafBBCode to HTML appropriate for editing in a rich text editor.
        /// </summary>
        /// <remarks>
        /// YafBBCode quotes are not converted to HTML.  "[quote]...[/quote]" will remain in the string
        ///   returned, as to appear in plain text in rich text editors.
        /// </remarks>
        /// <param name="message">
        /// String containing the body of the message to convert
        /// </param>
        /// <returns>
        /// The converted text
        /// </returns>
        public string ConvertBBCodeToHtmlForEdit(string message)
        {
            // get the rules engine from the creator...
            var ruleEngine = this.ProcessReplaceRulesFactory(
                new[] { false, true, this.Get<BoardSettings>().UseNoFollowLinks, true });

            if (!ruleEngine.HasRules)
            {
                // NOTE : Do not convert BBQuotes, BBCodes and Custom BBCodes to HTML when editing -- "[quote]...[/quote]", and [code]..[/code] will remain in plaintext in the rich text editor
                this.CreateBBCodeRules(
                    ruleEngine,
                    false,
                    true,
                    this.Get<BoardSettings>().UseNoFollowLinks,
                    false /*convertBBQuotes*/);
            }

            ruleEngine.Process(ref message);

            return message;
        }

        /// <summary>
        /// Converts a message containing HTML to YAF BBCode for editing in a rich BBCode editor.
        /// </summary>
        /// <param name="message">
        /// String containing the body of the message to convert
        /// </param>
        /// <returns>
        /// The converted text
        /// </returns>
        public string ConvertHtmltoBBCodeForEdit([NotNull] string message)
        {
            const bool DoFormatting = true;
            const bool TargetBlankOverride = false;
            const bool ForBBCodeEditing = true;

            // get the rules engine from the creator...
            var ruleEngine = this.ProcessReplaceRulesFactory(
                new[]
                    {
                        DoFormatting, TargetBlankOverride, this.Get<BoardSettings>().UseNoFollowLinks,
                        ForBBCodeEditing
                    });

            if (!ruleEngine.HasRules)
            {
                this.CreateHtmlRules(ruleEngine);
            }

            ruleEngine.Process(ref message);

            return message;
        }

        /// <summary>
        /// Creates the rules that convert <see cref="BBCode" /> to HTML
        /// </summary>
        /// <param name="ruleEngine">The rule Engine.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <param name="doFormatting">The do Formatting.</param>
        /// <param name="targetBlankOverride">The target Blank Override.</param>
        /// <param name="useNoFollow">The use No Follow.</param>
        /// <param name="convertBBQuotes">The convert BB Quotes.</param>
        public void CreateBBCodeRules(
            IProcessReplaceRules ruleEngine,
            bool isHtml,
            bool doFormatting,
            bool targetBlankOverride,
            bool useNoFollow,
            bool convertBBQuotes)
        {
            var target = this.Get<BoardSettings>().BlankLinks || targetBlankOverride
                             ? "target=\"_blank\""
                             : string.Empty;

            var nofollow = useNoFollow ? "rel=\"nofollow\"" : string.Empty;

            var classModal = string.Empty;

            // pull localized strings
            var localQuoteStr = this.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTE");

            // handle font sizes -- this rule class internally handles the "size" variable
            ruleEngine.AddRule(
                new FontSizeRegexReplaceRule(
                    @"\[size=(?<size>(.*?))\](?<inner>(.*?))\[/size\]",
                    @"<span style=""font-size:${size}"">${inner}</span>",
                    Options));

            if (doFormatting)
            {
                ruleEngine.AddRule(new CodeRegexReplaceRule(_rgxNoParse, @"${inner}"));

                ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxBold, "<strong>${inner}</strong>"));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxStrike, "<s>${inner}</s>", Options));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxItalic, "<em>${inner}</em>", Options));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxUnderline, "<u>${inner}</u>", Options));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxHighlighted, @"<mark>${inner}</mark>", Options));

                // e-mails
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxEmail2,
                        "<a href=\"mailto:${email}\">${inner}&nbsp;<i class=\"fa fa-external-link-alt fa-fw\"></i></a>",
                        new[] { "email" }));

                ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxEmail1, @"<a href=""mailto:${inner}"">${inner}</a>"));

                // urls
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxUrl2,
                        "<a {0} {1} href=\"${http}${url}\" title=\"${http}${url}\">${inner}&nbsp;<i class=\"fa fa-external-link-alt fa-fw\"></i></a>"
                            .Replace("{0}", target).Replace("{1}", nofollow),
                        new[] { "url", "http" },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            }) { RuleRank = 10 });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxUrl1,
                        "<a {0} {1} href=\"${http}${inner}\" title=\"${http}${inner}\">${http}${innertrunc}&nbsp;<i class=\"fa fa-external-link-alt fa-fw\"></i></a>"
                            .Replace("{0}", target).Replace("{1}", nofollow),
                        new[] { "http" },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            },
                        50) { RuleRank = 11 });

                // urls
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _RgxUrl3,
                        "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}&nbsp;<i class=\"fa fa-external-link-alt fa-fw\"></i></a>"
                            .Replace("{0}", target).Replace("{1}", nofollow),
                        new[] { "before" },
                        new[] { string.Empty },
                        50) { RuleRank = 12 });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _RgxUrl4,
                        "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}&nbsp;<i class=\"fa fa-external-link-alt fa-fw\"></i></a>"
                            .Replace("{0}", target).Replace("{1}", nofollow),
                        new[] { "before" },
                        new[] { string.Empty },
                        50) { RuleRank = 13 });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxModalUrl2,
                        "<a {0} {1} {2} href=\"#\" data-href=\"${http}${url}\" title=\"${http}${url}\">${inner}</a>"
                            .Replace("{0}", target).Replace("{1}", nofollow).Replace("{2}", classModal),
                        new[] { "url", "http" },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            }));

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxModalUrl1,
                        "<a {0} {1} {2} href=\"#\" data-href=\"${http}${inner}\" title=\"${http}${inner}\">${http}${innertrunc}</a>"
                            .Replace("{0}", target).Replace("{1}", nofollow).Replace("{2}", classModal),
                        new[] { "http" },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            },
                        50));

                // font
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _RgxFont,
                        "<span style=\"font-family:${font}\">${inner}</span>",
                        Options,
                        new[] { "font" }));

                // color
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _RgxColor,
                        "<span style=\"color:${color}\">${inner}</span>",
                        Options,
                        new[] { "color" }));

                // lists
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxList1, "<ul>${inner}</ul>", Options));

                /*ruleEngine.AddRule(
                                    new VariableRegexReplaceRule(_rgxList2, "<ol type=\"${type}\">${inner}</ol>", _options, new[] { "type" }));*/
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(
                        _RgxListNumber,
                        "<ol style=\"list-style-type:number\">${inner}</ol>",
                        RegexOptions.Singleline));
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(
                        _RgxListLowerRoman,
                        "<ol style=\"list-style-type:lower-roman\">${inner}</ol>",
                        RegexOptions.Singleline));
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(
                        _RgxListUpperRoman,
                        "<ol style=\"list-style-type:upper-roman\">${inner}</ol>",
                        RegexOptions.Singleline));
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(
                        _RgxListLowerAlpha,
                        "<ol style=\"list-style-type:lower-alpha\">${inner}</ol>",
                        RegexOptions.Singleline));
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(
                        _RgxListUpperAlpha,
                        "<ol style=\"list-style-type:upper-alpha\">${inner}</ol>",
                        RegexOptions.Singleline));

                // bullets
                ruleEngine.AddRule(new SingleRegexReplaceRule(_RgxBullet, "<li>", Options));

                // alignment
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(_RgxCenter, "<div align=\"center\">${inner}</div>", Options));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxLeft, "<div align=\"left\">${inner}</div>", Options));
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(_RgxRight, "<div align=\"right\">${inner}</div>", Options));

                // indent
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(_RgxIndent, "<div style=\"margin-left:40px\">${inner}</div>", Options));

                // image
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxImg,
                        "<img src=\"${http}${inner}\" alt=\"UserPostedImage\" class=\"img-user-posted img-thumbnail\" style=\"max-width:auto;max-height:${height}px;\" />",
                        new[] { "http", "height" },
                        new[] { "http://", this.Get<BoardSettings>().ImageThumbnailMaxHeight.ToString() })
                        {
                            RuleRank = 70
                        });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxImgEmptyTitle,
                        "<img src=\"${http}${inner}\" alt=\"UserPostedImage\" class=\"img-user-posted img-thumbnail\" style=\"max-width:auto;max-height:${height}px;\" />",
                        new[] { "http", "height" },
                        new[] { "http://", this.Get<BoardSettings>().ImageThumbnailMaxHeight.ToString() })
                        {
                            RuleRank = 71
                        });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxImgTitle,
                        "<img src=\"${http}${inner}\" alt=\"${description}\" title=\"${description}\" class=\"img-user-posted img-thumbnail\" style=\"max-width:auto;max-height:${height}px;\" />",
                        new[] { "http", "description", "height" },
                        new[]
                            {
                                "http://", string.Empty, this.Get<BoardSettings>().ImageThumbnailMaxHeight.ToString()
                            }) { RuleRank = 72 });

                // basic hr and br rules
                var hrRule = new SingleRegexReplaceRule(
                    _RgxHr,
                    "<hr />",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

                ruleEngine.AddRule(new SingleRegexReplaceRule(@"\[br\]", "</p>", Options));

                // Multiline, since ^ must match beginning of line
                var brRule = new SingleRegexReplaceRule(
                                 _RgxBr,
                                 "<br />",
                                 RegexOptions.IgnoreCase | RegexOptions.Multiline) { RuleRank = hrRule.RuleRank + 1 };

                // Ensure the newline rule is processed after the HR rule, otherwise the newline characters in the HR regex will never match
                ruleEngine.AddRule(hrRule);

                ruleEngine.AddRule(!isHtml ? brRule : new SingleRegexReplaceRule(@"\r\n", "<p>", Options));
            }

            if (convertBBQuotes)
            {
                // add rule for code block type with syntax highlighting
                ruleEngine.AddRule(
                    new SyntaxHighlightedCodeRegexReplaceRule(
                        _regexCodeWithLanguage,
                        @"<div class=""code"">${inner}</div>")
                        {
                            RuleRank = 2
                        });

                // handle custom YafBBCode
                this.AddCustomBBCodeRules(ruleEngine);

                // add rule for code block type with no syntax highlighting
                ruleEngine.AddRule(
                    new SyntaxHighlightedCodeRegexReplaceRule(_rgxCode1, @"<div class=""code"">${inner}</div>"));

                ruleEngine.AddRule(
                    new QuoteRegexReplaceRule(
                        OpenQuoteUserIdRegex,
                        @"<blockquote class=""blockquote blockquote-custom pt-3 px-1 pb-1 mb-4 border border-secondary rounded"">
                                         <div class=""blockquote-custom-icon bg-secondary"">
                                             <i class=""fa fa-quote-left fa-sm text-white""></i>
                                         </div>${quote}",
                        Options));

                // simple open quote tag
                var simpleOpenQuoteReplace =
                    $@"<blockquote class=""blockquote blockquote-custom pt-3 px-1 pb-1 mb-4 border border-secondary rounded"">
                          <div class=""blockquote-custom-icon bg-secondary"">
                              <i class=""fa fa-quote-left fa-sm text-white""></i>
                          </div>
                          <footer class=""blockquote-footer""><cite>{localQuoteStr}</cite></footer>
                          <p class=""mb-0"">";

                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(OpenQuoteRegex, simpleOpenQuoteReplace, Options) { RuleRank = 62 });

                // and finally the closing quote tag
                ruleEngine.AddRule(
                    new SingleRegexReplaceRule(CloseQuoteRegex, "</p></blockquote>", Options) { RuleRank = 63 });
            }

            // post and topic rules...
            ruleEngine.AddRule(
                new PostTopicRegexReplaceRule(
                    _RgxPost,
                    @"<a href=""${post}"" title=""${inner}"">${inner}</a>",
                    Options));

            ruleEngine.AddRule(
                new PostTopicRegexReplaceRule(
                    _RgxTopic,
                    @"<a href=""${topic}"" title=""${inner}"">${inner}</a>",
                    Options));
        }

        /// <summary>
        /// Creates the rules that convert HTML to <see cref="BBCode"/>
        /// </summary>
        /// <param name="ruleEngine">
        /// The rule Engine.
        /// </param>
        public void CreateHtmlRules(IProcessReplaceRules ruleEngine)
        {
            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"center\">(?<inner>(.*?))</div>",
                    "[center]${inner}[/center]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"left\">(?<inner>(.*?))</div>",
                    "[left]${inner}[/left]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"right\">(?<inner>(.*?))</div>",
                    "[right]${inner}[/right]",
                    Options));

            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: center;\">(?<inner>(.*?))</p>",
                    "[center]${inner}[/center]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: left;\">(?<inner>(.*?))</p>",
                    "[left]${inner}[/left]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: right;\">(?<inner>(.*?))</p>",
                    "[right]${inner}[/right]",
                    Options));

            // handle font sizes -- this rule class internally handles the "size" variable
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<span style=""font-size:(?<size>(.*?))px;"">(?<inner>(.*?))</span>",
                    "[size=${size}]${inner}[/size]",
                    Options,
                    new[]
                        {
                            "size"
                        })
                    {
                        RuleRank = 10
                    });

            // font
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<span style=""font-family: (?<font>(.*?));"">(?<inner>(.*?))</span>",
                    "[font=${font}]${inner}[/font]",
                    Options,
                    new[] { "font" }));

            // color
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<span style=""color: (?<color>(\#?[-a-z0-9]*));"">(?<inner>(.*?))</span>",
                    "[color=${color}]${inner}[/color]",
                    Options,
                    new[] { "color" }));

            // lists
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule("<ul>(?<inner>(.*?))</ul>", "[list]${inner}[/list]", Options));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol type=\"1\">(?<inner>(.*?))</ol>",
                    "[list=1]${inner}[/list]",
                    RegexOptions.Singleline));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule("<ol>(?<inner>(.*?))</ol>", "[list=i]${inner}[/list]", Options));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol style=\"list-style-type:number\">(?<inner>(.*?))</ol>",
                    "[list=1]${inner}[/list]",
                    RegexOptions.Singleline));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol type=\"a\">(?<inner>(.*?))</ol>",
                    "[list=a]${inner}[/list]",
                    RegexOptions.Singleline));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol type=\"A\">(?<inner>(.*?))</ol>",
                    "[list=A]${inner}[/list]",
                    RegexOptions.Singleline));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol style=\"list-style-type:lower-alpha\">(?<inner>(.*?))</ol>",
                    "[list=a]${inner}[/list]",
                    RegexOptions.Singleline));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol style=\"list-style-type:upper-alpha\">(?<inner>(.*?))</ol>",
                    "[list=A]${inner}[/list]",
                    RegexOptions.Singleline));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol type=\"i\">(?<inner>(.*?))</ol>",
                    "[list=i]${inner}[/list]",
                    RegexOptions.Singleline));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol type=\"I\">(?<inner>(.*?))</ol>",
                    "[list=I]${inner}[/list]",
                    RegexOptions.Singleline));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol style=\"list-style-type:lower-roman\">(?<inner>(.*?))</ol>",
                    "[list=i]${inner}[/list]",
                    RegexOptions.Singleline));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol style=\"list-style-type:upper-roman\">(?<inner>(.*?))</ol>",
                    "[list=I]${inner}[/list]",
                    RegexOptions.Singleline));

            // bullets
            ruleEngine.AddRule(new SingleRegexReplaceRule("<li>", "[*]", Options));

            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"center\">(?<inner>(.*?))</div>",
                    "[center]${inner}[/center]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"left\">(?<inner>(.*?))</div>",
                    "[left]${inner}[/left]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"right\">(?<inner>(.*?))</div>",
                    "[right]${inner}[/right]",
                    Options));

            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: center;\">(?<inner>(.*?))</p>",
                    "[center]${inner}[/center]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: left;\">(?<inner>(.*?))</p>",
                    "[left]${inner}[/left]",
                    Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: right;\">(?<inner>(.*?))</p>",
                    "[right]${inner}[/right]",
                    Options));

            // Indent text
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div style=\"margin-left:40px\">(?<inner>(.*?))</div>",
                    "[indent]${inner}[/indent]",
                    Options));

            ruleEngine.AddRule(new SingleRegexReplaceRule("<b>", "[b]", Options) { RuleRank = 4 });
            ruleEngine.AddRule(new SingleRegexReplaceRule("</b>", "[/b]", Options) { RuleRank = 5 });

            ruleEngine.AddRule(new SingleRegexReplaceRule("<strong>", "[b]", Options) { RuleRank = 3 });
            ruleEngine.AddRule(new SingleRegexReplaceRule("</strong>", "[/b]", Options) { RuleRank = 4 });

            ruleEngine.AddRule(new SingleRegexReplaceRule("<s>", "[s]", Options));
            ruleEngine.AddRule(new SingleRegexReplaceRule("</s>", "[/s]", Options));

            ruleEngine.AddRule(new SingleRegexReplaceRule("<em>", "[i]", Options));
            ruleEngine.AddRule(new SingleRegexReplaceRule("</em>", "[/i]", Options));

            ruleEngine.AddRule(new SingleRegexReplaceRule("<u>", "[u]", Options));
            ruleEngine.AddRule(new SingleRegexReplaceRule("</u>", "[/u]", Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    @"<span style=""text-decoration: underline;"">(?<inner>(.*?))</span>",
                    "[u]${inner}[/u]",
                    Options));

            // urls
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<a.*?href=""(?<inner>(.*?))"".*?>(?<description>(.*?))</a>",
                    "[url=${inner}]${description}[/url]",
                    Options,
                    new[]
                        {
                            "description"
                        }) { RuleRank = 2 });

            // e-mails
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<a.*?href=""mailto:(?<email>(.*?))"".*?>(?<inner>(.*?))</a>",
                    "[email=${email}]${inner}[/email]",
                    Options,
                    new[]
                        {
                            "email"
                        }) { RuleRank = 1 });

            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<img.*?src=""(?<inner>(.*?))"".*?alt=""(?<description>(.*?))"".*?/>",
                    "[img=${inner}]${description}[/img]",
                    Options,
                    new[] { "description" }));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    @"<span class=""highlight"">(?<inner>(.*?))</span>",
                    "[h]${inner}[/h]",
                    Options));

            // CODE Tags
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<div class=""code"">.*?<div class=""innercode"">.*?<pre class=""brush:(?<language>(.*?));"">(?<inner>(.*?))</pre>.*?</div>",
                    "[code=${language}]${inner}[/code]",
                    Options,
                    new[]
                        {
                            "language"
                        }) { RuleRank = 97 });

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div class=\"code\">.*?<div class=\"innercode\">(?<inner>(.*?))</div>",
                    "[code]${inner}[/code]",
                    Options)
                    {
                        RuleRank = 98
                    });

            ruleEngine.AddRule(new SimpleRegexReplaceRule("<br />", "\r\n", Options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule("<br>", "\r\n", Options));

            // Format paragraphs
            ruleEngine.AddRule(new SimpleRegexReplaceRule("<p>", "\r\n\r\n", Options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule("</p>", "[br]", Options));


            ruleEngine.AddRule(new SimpleRegexReplaceRule("&nbsp;", string.Empty, Options));

            // remove remaining tags
            ruleEngine.AddRule(new SimpleRegexReplaceRule("<[^>]+>", string.Empty, Options) { RuleRank = 100 });
        }

        /// <summary>
        /// Handles localization for a Custom YafBBCode Elements using
        ///   the code [localization=tag]default[/localization]
        /// </summary>
        /// <param name="strToLocalize">
        /// The string to localize.
        /// </param>
        /// <returns>
        /// The localize custom bb code element.
        /// </returns>
        public string LocalizeCustomBBCodeElement(string strToLocalize)
        {
            var regExSearch = new Regex(_RgxBBCodeLocalizationTag, Options);

            var sb = new StringBuilder(strToLocalize);

            var m = regExSearch.Match(strToLocalize);
            while (m.Success)
            {
                // get the localization tag...
                var tagValue = m.Groups["tag"].Value;
                var defaultValue = m.Groups["inner"].Value;

                // remove old code...
                sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

                // insert localized value...
                var localValue = defaultValue;

                if (this.Get<ILocalization>().GetTextExists("BBCODEMODULE", tagValue))
                {
                    localValue = this.Get<ILocalization>().GetText("BBCODEMODULE", tagValue);
                }

                sb.Insert(m.Groups[0].Index, localValue);
                m = regExSearch.Match(sb.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string containing YafBBCode to the equivalent HTML string.
        /// </summary>
        /// <param name="inputString">
        /// Input string containing YafBBCode to convert to HTML
        /// </param>
        /// <param name="doFormatting">
        /// The do Formatting.
        /// </param>
        /// <param name="targetBlankOverride">
        /// The target Blank Override.
        /// </param>
        /// <returns>
        /// The make html.
        /// </returns>
        public string MakeHtml(string inputString, bool isHtml, bool doFormatting, bool targetBlankOverride)
        {
            var ruleEngine = this.ProcessReplaceRulesFactory(
                new[] { doFormatting, targetBlankOverride, this.Get<BoardSettings>().UseNoFollowLinks });

            if (!ruleEngine.HasRules)
            {
                this.CreateBBCodeRules(
                    ruleEngine,
                    isHtml,
                    doFormatting,
                    targetBlankOverride,
                    this.Get<BoardSettings>().UseNoFollowLinks);
            }

            ruleEngine.Process(ref inputString);

            return inputString;
        }

        /// <summary>
        /// Helper function that dandles registering "custom bbcode" javascript (if there is any)
        ///   for all the custom YafBBCode.
        /// </summary>
        /// <param name="currentPage">
        /// The current Page.
        /// </param>
        /// <param name="currentType">
        /// The current Type.
        /// </param>
        public void RegisterCustomBBCodePageElements(Page currentPage, Type currentType)
        {
            this.RegisterCustomBBCodePageElements(currentPage, currentType, null);
        }

        /// <summary>
        /// Helper function that dandles registering "custom bbcode" javascript (if there is any)
        ///   for all the custom YafBBCode. Defining editorID make the system also show "editor js" (if any).
        /// </summary>
        /// <param name="currentPage">
        /// The current Page.
        /// </param>
        /// <param name="currentType">
        /// The current Type.
        /// </param>
        /// <param name="editorID">
        /// The editor ID.
        /// </param>
        public void RegisterCustomBBCodePageElements(Page currentPage, Type currentType, string editorID)
        {
            var bbCodeTable = this.Get<DataBroker>().GetCustomBBCode();
            const string ScriptID = "custombbcode";
            var jsScriptBuilder = new StringBuilder();
            var cssBuilder = new StringBuilder();

            jsScriptBuilder.Append("\r\n");
            cssBuilder.Append("\r\n");

            bbCodeTable.ForEach(
                row =>
                    {
                        string displayScript = null;
                        string editScript = null;

                        if (row.DisplayJS.IsSet())
                        {
                            displayScript = this.LocalizeCustomBBCodeElement(row.DisplayJS.Trim());
                        }

                        if (editorID.IsSet() && row.EditJS.IsSet())
                        {
                            editScript = this.LocalizeCustomBBCodeElement(row.EditJS.Trim());

                            // replace any instances of editor ID in the javascript in case the ID is needed
                            editScript = editScript.Replace("{editorid}", editorID);
                        }

                        if (displayScript.IsSet() || editScript.IsSet())
                        {
                            jsScriptBuilder.AppendLine($"{displayScript}\r\n{editScript}");
                        }

                        // see if there is any CSS associated with this YafBBCode
                        if (row.DisplayCSS.IsSet() && row.DisplayCSS.IsSet())
                        {
                            // yes, add it into the builder
                            cssBuilder.AppendLine(this.LocalizeCustomBBCodeElement(row.DisplayCSS.Trim()));
                        }
                    });

            if (jsScriptBuilder.ToString().Trim().Length > 0)
            {
                BoardContext.Current.PageElements.RegisterJsBlock(
                    currentPage,
                    $"{ScriptID}_script",
                    jsScriptBuilder.ToString());
            }

            if (cssBuilder.ToString().Trim().Length > 0)
            {
                // register the CSS from all custom bbcode...
                BoardContext.Current.PageElements.RegisterCssBlock($"{ScriptID}_css", cssBuilder.ToString());
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Applies Custom BBCode Rules from the BBCode table
        /// </summary>
        /// <param name="rulesEngine">
        /// The rules Engine.
        /// </param>
        protected void AddCustomBBCodeRules(IProcessReplaceRules rulesEngine)
        {
            var bbcodeTable = this.Get<DataBroker>().GetCustomBBCode();

            // handle custom bbcodes row by row...
            bbcodeTable.Where(codeRow => !(codeRow.UseModule ?? false) && codeRow.SearchRegex.IsSet()).ForEach(
                codeRow =>
                    {
                        if (codeRow.Variables.IsSet())
                        {
                            // handle variables...
                            var variables = codeRow.Variables.Split(';');

                            var rule = new VariableRegexReplaceRule(
                                           codeRow.SearchRegex,
                                           codeRow.ReplaceRegex,
                                           Options,
                                           variables) { RuleRank = 50 };

                            rulesEngine.AddRule(rule);
                        }
                        else
                        {
                            // just standard replace...
                            var rule = new SimpleRegexReplaceRule(codeRow.SearchRegex, codeRow.ReplaceRegex, Options)
                                           {
                                               RuleRank = 50
                                           };

                            rulesEngine.AddRule(rule);
                        }
                    });
        }

        #endregion
    }
}