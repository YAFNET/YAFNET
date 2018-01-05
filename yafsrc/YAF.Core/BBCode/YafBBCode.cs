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

namespace YAF.Core.BBCode
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core.BBCode.ReplaceRules;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Yaf BBCode Class to Format Message From BB Code to HTML and Reverse.
    /// </summary>
    public class YafBBCode : IBBCode, IHaveServiceLocator
    {
        /* Ederon : 6/16/2007 - conventions */
        #region Constants and Fields

        /// <summary>
        ///   The _options.
        /// </summary>
        private const RegexOptions _Options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

        /// <summary>
        ///   The _rgx bb code localization tag.
        /// </summary>
        private const string _RgxBBCodeLocalizationTag =
            @"\[localization=(?<tag>[^\]]*)\](?<inner>(.+?))\[/localization\]";

        /// <summary>
        ///   The _rgx noparse.
        /// </summary>
        private static readonly Regex _rgxNoParse = new Regex(
            @"\[noparse\](?<inner>(.*?))\[/noparse\]", _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx bold.
        /// </summary>
        private static readonly Regex _rgxBold = new Regex(
            @"\[B\](?<inner>(.*?))\[/B\]", _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx br.
        /// </summary>
   //     private const string _RgxBr = "[\r]?\n(?!.*<[^>]+>.*)"; // "[\r]?\n";

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
        //private const string _RgxEasyQuote = @"(\>)\s(?<inner>(.*?))$";

        /// <summary>
        ///   The _rgx email 1.
        /// </summary>
        private static readonly Regex _rgxEmail1 = new Regex(
            @"\[email[^\]]*\](?<inner>([^""\r\n\]\[]+?))\[/email\]", _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx email 2.
        /// </summary>
        private static readonly Regex _rgxEmail2 =
            new Regex(
                @"\[email=(?<email>[^\]]*)\](?<inner>([^""\r\n\]\[]+?))\[/email\]", _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx code 1.
        /// </summary>
        private static readonly Regex _rgxCode1 = new Regex(
            @"\[code\](?<inner>(.*?))\[/code\]", _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The regex code with language string.
        /// </summary>
        private static readonly Regex _regexCodeWithLanguage = new Regex(
            @"\[code=(?<language>[^\]]*)\](?<inner>(.*?))\[/code\]", _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx hr.
        /// </summary>
        private const string _RgxHr = "^[-][-][-][-][-]*[\r]?[\n]";

        /// <summary>
        ///   The _rgx img.
        /// </summary>
        private static readonly Regex _rgxImg =
            new Regex(
                @"\[img\](?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>((?!.+logout)[^""\r\n\]\[]+?\.((googleusercontent[^\[]*)|(jpg[^\[]*)|(jpeg[^\[]*)|(bmp[^\[]*)|(png[^\[]*)|(gif[^\[]*)|(tif[^\[]*)|(ashx[^\[]*)|(php[^\[]*)|(aspx[^\[]*))))\[/img\]",
                _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx img Title.
        /// </summary>
        private static readonly Regex _rgxImgEmptyTitle =
            new Regex(
                @"\[img=(?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>((?!.+logout)[^""\r\n\]\[]+?\.((googleusercontent[^\[]*)|(jpg[^\]\[/img\]]*)|(jpeg[^\[\[/img\]]*)|(bmp[^\[\[/img\]]*)|(png[^\]\[/img\]]*)|(gif[^\]\[/img\]]*)|(tif[^\]\[/img\]]*)|(ashx[^\]\[/img\]]*)|(php[^\]\[/img\]]*)|(aspx[^\]\[/img\]]*))))\]\[/img\]",
                _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx img Title.
        /// </summary>
        private static readonly Regex _rgxImgTitle =
            new Regex(
                @"\[img=(?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>((?!.+logout)[^""\r\n\]\[]+?\.((googleusercontent[^\[]*)|(jpg[^\]]*)|(jpeg[^\]]*)|(bmp[^\]]*)|(png[^\]]*)|(gif[^\]]*)|(tif[^\]]*)|(ashx[^\]]*)|(php[^\]]*)|(aspx[^\]]*))))\](?<description>[^\[]*)\[/img\]",
                _Options | RegexOptions.Compiled);

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
        private static readonly Regex _rgxModalUrl1 =
            new Regex(
                @"\[modalurl](?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>([^javascript:])(.+?))\[/modalurl\]",
                _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx url 2.
        /// </summary>
        private static readonly Regex _rgxModalUrl2 =
            new Regex(
                @"\[modalurl\=(?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))(?<url>(?<inner>([^javascript:])([^""\r\n\]\[]*?))\](.+?))\[/modalurl\]",
                _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx url 1.
        /// </summary>
        private static readonly Regex _rgxUrl1 =
            new Regex(
                @"\[url\](?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://)|(mailto:))?(?<inner>([^javascript:])(.+?))\[/url\]",
                _Options | RegexOptions.Compiled);

        /// <summary>
        ///   The _rgx url 2.
        /// </summary>
        private static readonly Regex _rgxUrl2 =
            new Regex(
                @"\[url\=(?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))?(?<url>([^javascript:])([^""\r\n\]\[]*?))\](?<inner>(.+?))\[/url\]",
                _Options | RegexOptions.Compiled);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafBBCode"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="processReplaceRulesFactory">
        /// The process replace rules factory.
        /// </param>
        public YafBBCode(
            IServiceLocator serviceLocator, Func<IEnumerable<bool>, IProcessReplaceRules> processReplaceRulesFactory)
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

        #region Implemented Interfaces

        #region IBBCode

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
            var ruleEngine =
                this.ProcessReplaceRulesFactory(
                    new[] { false, true, this.Get<YafBoardSettings>().UseNoFollowLinks, true });

            if (!ruleEngine.HasRules)
            {
                // NOTE : Do not convert BBQuotes, BBCodes and Custom BBCodes to HTML when editing -- "[quote]...[/quote]", and [code]..[/code] will remain in plaintext in the rich text editor
                this.CreateBBCodeRules(
                    ruleEngine, false, true, this.Get<YafBoardSettings>().UseNoFollowLinks, false /*convertBBQuotes*/);
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
            var ruleEngine =
                this.ProcessReplaceRulesFactory(
                    new[]
                        {
                            DoFormatting, TargetBlankOverride, this.Get<YafBoardSettings>().UseNoFollowLinks,
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
        /// Creates the rules that convert <see cref="YafBBCode" /> to HTML
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
            var target = (this.Get<YafBoardSettings>().BlankLinks || targetBlankOverride)
                                ? "target=\"_blank\""
                                : string.Empty;

            var nofollow = useNoFollow ? "rel=\"nofollow\"" : string.Empty;

            const string ClassModal = "";

            // pull localized strings
            var localQuoteStr = this.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTE");
            var localCodeStr = this.Get<ILocalization>().GetText("COMMON", "BBCODE_CODE");

            // handle font sizes -- this rule class internally handles the "size" variable
            ruleEngine.AddRule(
                new FontSizeRegexReplaceRule(
                    @"\[size=(?<size>(.*?))\](?<inner>(.*?))\[/size\]",
                    @"<span style=""font-size:${size}"">${inner}</span>",
                    _Options));

            if (doFormatting)
            {
                ruleEngine.AddRule(
                   new CodeRegexReplaceRule(
                      _rgxNoParse,
                       @"${inner}"));

                ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxBold, "<strong>${inner}</strong>"));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxStrike, "<s>${inner}</s>", _Options));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxItalic, "<em>${inner}</em>", _Options));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxUnderline, "<u>${inner}</u>", _Options));
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(_RgxHighlighted, @"<span class=""highlight"">${inner}</span>", _Options));

                // e-mails
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxEmail2, "<a href=\"mailto:${email}\">${inner}</a>", new[] { "email" }));

                ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxEmail1, @"<a href=""mailto:${inner}"">${inner}</a>"));

                // urls
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxUrl2,
                        "<a {0} {1} href=\"${http}${url}\" title=\"${http}${url}\">${inner}</a>".Replace("{0}", target)
                            .Replace("{1}", nofollow),
                        new[]
                            {
                                "url", "http"
                            },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            })
                    { RuleRank = 10 });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxUrl1,
                        "<a {0} {1} href=\"${http}${inner}\" title=\"${http}${inner}\">${http}${innertrunc}</a>".Replace("{0}", target).Replace("{1}", nofollow),
                        new[]
                            {
                                "http"
                            },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            },
                        50)
                    { RuleRank = 11 });

                // urls
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxModalUrl2,
                        "<a {0} {1} {2} href=\"#\" data-href=\"${http}${url}\" title=\"${http}${url}\">${inner}</a>".Replace("{0}", target).Replace("{1}", nofollow).Replace("{2}", ClassModal),
                        new[] { "url", "http" },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            }));
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxModalUrl1,
                        "<a {0} {1} {2} href=\"#\" data-href=\"${http}${inner}\" title=\"${http}${inner}\">${http}${innertrunc}</a>".Replace("{0}", target).Replace("{1}", nofollow).Replace("{2}", ClassModal),
                        new[] { "http" },
                        new[]
                            {
                                string.Empty, string.Empty // "http://"
                            },
                        50));

                // font
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _RgxFont, "<span style=\"font-family:${font}\">${inner}</span>", _Options, new[] { "font" }));

                // color
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _RgxColor, "<span style=\"color:${color}\">${inner}</span>", _Options, new[] { "color" }));

                // lists
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxList1, "<ul>${inner}</ul>", _Options));
                /*ruleEngine.AddRule(
                    new VariableRegexReplaceRule(_rgxList2, "<ol type=\"${type}\">${inner}</ol>", _options, new[] { "type" }));*/
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxListNumber, "<ol style=\"list-style-type:number\">${inner}</ol>", RegexOptions.Singleline));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxListLowerRoman, "<ol style=\"list-style-type:lower-roman\">${inner}</ol>", RegexOptions.Singleline));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxListUpperRoman, "<ol style=\"list-style-type:upper-roman\">${inner}</ol>", RegexOptions.Singleline));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxListLowerAlpha, "<ol style=\"list-style-type:lower-alpha\">${inner}</ol>", RegexOptions.Singleline));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxListUpperAlpha, "<ol style=\"list-style-type:upper-alpha\">${inner}</ol>", RegexOptions.Singleline));

                // bullets
                ruleEngine.AddRule(new SingleRegexReplaceRule(_RgxBullet, "<li>", _Options));

                // alignment
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(_RgxCenter, "<div align=\"center\">${inner}</div>", _Options));
                ruleEngine.AddRule(new SimpleRegexReplaceRule(_RgxLeft, "<div align=\"left\">${inner}</div>", _Options));
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(_RgxRight, "<div align=\"right\">${inner}</div>", _Options));

                // indent
                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(_RgxIndent, "<div style=\"margin-left:40px\">${inner}</div>", _Options));

                // add max-width and max-height to posted Image
                var maxWidth = this.Get<YafBoardSettings>().ImageAttachmentResizeWidth;
                var maxHeight = this.Get<YafBoardSettings>().ImageAttachmentResizeHeight;

                var styleAttribute = this.Get<YafBoardSettings>().ResizePostedImages
                                            ? " style=\"max-width:{0}px;max-height:{1}px\"".FormatWith(
                                                maxWidth, maxHeight)
                                            : string.Empty;

                // image
                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxImg,
                        "<img src=\"${http}${inner}\" alt=\"UserPostedImage\" class=\"UserPostedImage\"{0} />".Replace(
                            "{0}", styleAttribute),
                        new[]
                            {
                                "http"
                            },
                        new[] { "http://" })
                    { RuleRank = 70 });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxImgEmptyTitle,
                        "<img src=\"${http}${inner}\" alt=\"UserPostedImage\" class=\"UserPostedImage\"{0} />".Replace(
                            "{0}", styleAttribute),
                        new[]
                            {
                                "http"
                            },
                        new[] { "http://" })
                    { RuleRank = 71 });

                ruleEngine.AddRule(
                    new VariableRegexReplaceRule(
                        _rgxImgTitle,
                        "<img src=\"${http}${inner}\" alt=\"${description}\" title=\"${description}\" class=\"UserPostedImage\"{0} />"
                            .Replace("{0}", styleAttribute),
                        new[]
                            {
                                "http", "description"
                            },
                        new[] { "http://", string.Empty })
                    { RuleRank = 72 });

                // basic hr and br rules
                var hrRule = new SingleRegexReplaceRule(
                    _RgxHr, "<hr />", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                ruleEngine.AddRule(
                    new SingleRegexReplaceRule(@"\[br\]", "</p>", _Options));

                // Multiline, since ^ must match beginning of line
                var brRule = new SingleRegexReplaceRule(
                    _RgxBr, "<br />", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                {
                    RuleRank = hrRule.RuleRank + 1
                };

                // Ensure the newline rule is processed after the HR rule, otherwise the newline characters in the HR regex will never match
                ruleEngine.AddRule(hrRule);

                if (!isHtml)
                {
                    ruleEngine.AddRule(brRule);
                }
                else
                {
                    // handle paragraphs
                    ruleEngine.AddRule(
                        new SingleRegexReplaceRule(@"\r\n", "<p>", _Options));
                }


            }

            if (convertBBQuotes)
            {
                // add rule for code block type with syntax highlighting
                ruleEngine.AddRule(
                    new SyntaxHighlightedCodeRegexReplaceRule(
                        _regexCodeWithLanguage,
                        @"<div class=""code""><strong>{0}</strong><div class=""innercode"">${inner}</div></div>".Replace("{0}", localCodeStr))
                    {
                        RuleRank = 30
                    });

                // handle custom YafBBCode
                this.AddCustomBBCodeRules(ruleEngine);

                // add rule for code block type with no syntax highlighting
                ruleEngine.AddRule(
                    new SyntaxHighlightedCodeRegexReplaceRule(
                        _rgxCode1,
                        @"<div class=""code""><strong>{0}</strong><div class=""innercode"">${inner}</div></div>".Replace("{0}", localCodeStr)));

                ruleEngine.AddRule(
                    new QuoteRegexReplaceRule(
                        OpenQuoteUserIdRegex,
                        @"<div class=""quote""><span class=""quotetitle"">${quote}</span><div class=""innerquote"">",
                        _Options));

                // simple open quote tag
                var simpleOpenQuoteReplace =
                    @"<div class=""quote""><span class=""quotetitle"">{0}</span><div class=""innerquote"">"
                        .FormatWith(localQuoteStr);

                ruleEngine.AddRule(
                    new SimpleRegexReplaceRule(OpenQuoteRegex, simpleOpenQuoteReplace, _Options) { RuleRank = 62 });

                // and finally the closing quote tag
                ruleEngine.AddRule(
                    new SingleRegexReplaceRule(CloseQuoteRegex, "</div></div>", _Options) { RuleRank = 63 });
            }

            // post and topic rules...
            ruleEngine.AddRule(
                new PostTopicRegexReplaceRule(
                    _RgxPost, @"<a href=""${post}"" title=""${inner}"">${inner}</a>", _Options));

            ruleEngine.AddRule(
                new PostTopicRegexReplaceRule(
                    _RgxTopic, @"<a href=""${topic}"" title=""${inner}"">${inner}</a>", _Options));
        }

        /// <summary>
        /// Creates the rules that convert HTML to <see cref="YafBBCode"/>
        /// </summary>
        /// <param name="ruleEngine">
        /// The rule Engine.
        /// </param>
        public void CreateHtmlRules(IProcessReplaceRules ruleEngine)
        {
            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"center\">(?<inner>(.*?))</div>", "[center]${inner}[/center]", _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"left\">(?<inner>(.*?))</div>", "[left]${inner}[/left]", _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"right\">(?<inner>(.*?))</div>", "[right]${inner}[/right]", _Options));

            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: center;\">(?<inner>(.*?))</p>", "[center]${inner}[/center]", _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: left;\">(?<inner>(.*?))</p>", "[left]${inner}[/left]", _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: right;\">(?<inner>(.*?))</p>", "[right]${inner}[/right]", _Options));

            // handle font sizes -- this rule class internally handles the "size" variable
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<span style=""font-size:(?<size>(.*?))px;"">(?<inner>(.*?))</span>",
                    "[size=${size}]${inner}[/size]",
                    _Options,
                    new[] { "size" }) { RuleRank = 10 });

            // font
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<span style=""font-family: (?<font>(.*?));"">(?<inner>(.*?))</span>",
                    "[font=${font}]${inner}[/font]",
                    _Options,
                    new[] { "font" }));

            // color
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<span style=""color: (?<color>(\#?[-a-z0-9]*));"">(?<inner>(.*?))</span>",
                    "[color=${color}]${inner}[/color]",
                    _Options,
                    new[] { "color" }));

            // lists
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule("<ul>(?<inner>(.*?))</ul>", "[list]${inner}[/list]", _Options));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<ol type=\"1\">(?<inner>(.*?))</ol>",
                    "[list=1]${inner}[/list]",
                    RegexOptions.Singleline));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule("<ol>(?<inner>(.*?))</ol>", "[list=i]${inner}[/list]", _Options));

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
            ruleEngine.AddRule(new SingleRegexReplaceRule("<li>", "[*]", _Options));

            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"center\">(?<inner>(.*?))</div>",
                    "[center]${inner}[/center]",
                    _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"left\">(?<inner>(.*?))</div>",
                    "[left]${inner}[/left]",
                    _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div align=\"right\">(?<inner>(.*?))</div>",
                    "[right]${inner}[/right]",
                    _Options));

            // alignment
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: center;\">(?<inner>(.*?))</p>",
                    "[center]${inner}[/center]",
                    _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: left;\">(?<inner>(.*?))</p>",
                    "[left]${inner}[/left]",
                    _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<p style=\"text-align: right;\">(?<inner>(.*?))</p>",
                    "[right]${inner}[/right]",
                    _Options));

            // Indent text
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div style=\"margin-left:40px\">(?<inner>(.*?))</div>",
                    "[indent]${inner}[/indent]",
                    _Options));

            ruleEngine.AddRule(new SingleRegexReplaceRule("<b>", "[b]", _Options) { RuleRank = 4 });
            ruleEngine.AddRule(new SingleRegexReplaceRule("</b>", "[/b]", _Options) { RuleRank = 5 });

            ruleEngine.AddRule(new SingleRegexReplaceRule("<strong>", "[b]", _Options) { RuleRank = 3 });
            ruleEngine.AddRule(new SingleRegexReplaceRule("</strong>", "[/b]", _Options) { RuleRank = 4 });

            ruleEngine.AddRule(new SingleRegexReplaceRule("<s>", "[s]", _Options));
            ruleEngine.AddRule(new SingleRegexReplaceRule("</s>", "[/s]", _Options));

            ruleEngine.AddRule(new SingleRegexReplaceRule("<em>", "[i]", _Options));
            ruleEngine.AddRule(new SingleRegexReplaceRule("</em>", "[/i]", _Options));

            ruleEngine.AddRule(new SingleRegexReplaceRule("<u>", "[u]", _Options));
            ruleEngine.AddRule(new SingleRegexReplaceRule("</u>", "[/u]", _Options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    @"<span style=""text-decoration: underline;"">(?<inner>(.*?))</span>",
                    "[u]${inner}[/u]",
                    _Options));

            // urls
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<a.*?href=""(?<inner>(.*?))"".*?>(?<description>(.*?))</a>",
                    "[url=${inner}]${description}[/url]",
                    _Options,
                    new[] { "description" }) { RuleRank = 2 });

            // e-mails
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<a.*?href=""mailto:(?<email>(.*?))"".*?>(?<inner>(.*?))</a>",
                    "[email=${email}]${inner}[/email]",
                    _Options,
                    new[] { "email" }) { RuleRank = 1 });

            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<img.*?src=""(?<inner>(.*?))"".*?alt=""(?<description>(.*?))"".*?/>",
                    "[img=${inner}]${description}[/img]",
                    _Options,
                    new[] { "description" }));

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    @"<span class=""highlight"">(?<inner>(.*?))</span>",
                    "[h]${inner}[/h]",
                    _Options));

            // CODE Tags
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    @"<div class=""code"">.*?<div class=""innercode"">.*?<pre class=""brush:(?<language>(.*?));"">(?<inner>(.*?))</pre>.*?</div>",
                    "[code=${language}]${inner}[/code]",
                    _Options,
                    new[] { "language" }) { RuleRank = 97 });

            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(
                    "<div class=\"code\">.*?<div class=\"innercode\">(?<inner>(.*?))</div>",
                    "[code]${inner}[/code]",
                    _Options)
                        {
                            RuleRank = 98
                        });

            ruleEngine.AddRule(new SimpleRegexReplaceRule("<br />", "\r\n", _Options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule("<br>", "\r\n", _Options));

            // Format paragraphs
            ruleEngine.AddRule(new SimpleRegexReplaceRule("<p>", "\r\n\r\n", _Options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule("</p>", "[br]", _Options));


            ruleEngine.AddRule(new SimpleRegexReplaceRule("&nbsp;", string.Empty, _Options));

            // remove remaining tags
            ruleEngine.AddRule(new SimpleRegexReplaceRule("<[^>]+>", string.Empty, _Options) { RuleRank = 100 });
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
            var regExSearch = new Regex(_RgxBBCodeLocalizationTag, _Options);

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
            var ruleEngine =
                this.ProcessReplaceRulesFactory(
                    new[] { doFormatting, targetBlankOverride, this.Get<YafBoardSettings>().UseNoFollowLinks });

            if (!ruleEngine.HasRules)
            {
                this.CreateBBCodeRules(
                    ruleEngine, isHtml, doFormatting, targetBlankOverride, this.Get<YafBoardSettings>().UseNoFollowLinks);
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
            var bbCodeTable = this.Get<YafDbBroker>().GetCustomBBCode();
            const string ScriptID = "custombbcode";
            var jsScriptBuilder = new StringBuilder();
            var cssBuilder = new StringBuilder();

            jsScriptBuilder.Append("\r\n");
            cssBuilder.Append("\r\n");

            foreach (var row in bbCodeTable)
            {
                string displayScript = null;
                string editScript = null;

                if (StringExtensions.IsSet(row.DisplayJS))
                {
                    displayScript = this.LocalizeCustomBBCodeElement(row.DisplayJS.Trim());
                }

                if (editorID.IsSet() && StringExtensions.IsSet(row.EditJS))
                {
                    editScript = this.LocalizeCustomBBCodeElement(row.EditJS.Trim());

                    // replace any instances of editor ID in the javascript in case the ID is needed
                    editScript = editScript.Replace("{editorid}", editorID);
                }

                if (displayScript.IsSet() || editScript.IsSet())
                {
                    jsScriptBuilder.AppendLine("{0}\r\n{1}".FormatWith(displayScript, editScript));
                }

                // see if there is any CSS associated with this YafBBCode
                if (StringExtensions.IsSet(row.DisplayCSS) && StringExtensions.IsSet(row.DisplayCSS))
                {
                    // yes, add it into the builder
                    cssBuilder.AppendLine(this.LocalizeCustomBBCodeElement(row.DisplayCSS.Trim()));
                }
            }

            if (jsScriptBuilder.ToString().Trim().Length > 0)
            {
                YafContext.Current.PageElements.RegisterJsBlock(
                    currentPage, "{0}_script".FormatWith(ScriptID), jsScriptBuilder.ToString());
            }

            if (cssBuilder.ToString().Trim().Length > 0)
            {
                // register the CSS from all custom bbcode...
                YafContext.Current.PageElements.RegisterCssBlock("{0}_css".FormatWith(ScriptID), cssBuilder.ToString());
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Applies Custom YafBBCode Rules from the YafBBCode table
        /// </summary>
        /// <param name="rulesEngine">
        /// The rules Engine.
        /// </param>
        protected void AddCustomBBCodeRules(IProcessReplaceRules rulesEngine)
        {
            var bbcodeTable = this.Get<YafDbBroker>().GetCustomBBCode();

            // handle custom bbcodes row by row...
            foreach (var codeRow in
                bbcodeTable.Where(codeRow => !(codeRow.UseModule ?? false) && codeRow.SearchRegex.IsSet()))
            {
                if (codeRow.Variables.IsSet())
                {
                    // handle variables...
                    var variables = codeRow.Variables.Split(';');

                    var rule = new VariableRegexReplaceRule(
                        codeRow.SearchRegex, codeRow.ReplaceRegex, _Options, variables) { RuleRank = 50 };

                    rulesEngine.AddRule(rule);
                }
                else
                {
                    // just standard replace...
                    var rule = new SimpleRegexReplaceRule(codeRow.SearchRegex, codeRow.ReplaceRegex, _Options)
                        {
                           RuleRank = 50
                        };

                    rulesEngine.AddRule(rule);
                }
            }
        }

        #endregion
    }
}