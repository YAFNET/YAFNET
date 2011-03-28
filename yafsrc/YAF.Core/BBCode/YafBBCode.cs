/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Core.BBCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.BBCode.ReplaceRules;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  /// <summary>
  /// Summary description for YafBBCode.
  /// </summary>
  public class YafBBCode : IBBCode, IHaveServiceLocator
  {
      /// <summary>
      /// Initializes a new instance of the <see cref="YafBBCode"/> class.
      /// </summary>
      /// <param name="serviceLocator">
      /// The service locator.
      /// </param>
      /// <param name="processReplaceRulesFactory">
      /// The process replace rules factory.
      /// </param>
      public YafBBCode(IServiceLocator serviceLocator, Func<IEnumerable<bool>, IProcessReplaceRules> processReplaceRulesFactory)
      {
          this.ServiceLocator = serviceLocator;
          this.ProcessReplaceRulesFactory = processReplaceRulesFactory;
      }

      /// <summary>
      /// Gets or sets ServiceLocator.
      /// </summary>
      public IServiceLocator ServiceLocator { get; set; }

      /// <summary>
      /// Gets or sets ProcessReplaceRulesFactory.
      /// </summary>
      public Func<IEnumerable<bool>, IProcessReplaceRules> ProcessReplaceRulesFactory { get; set; }

    /* Ederon : 6/16/2007 - conventions */

    /// <summary>
    /// The _options.
    /// </summary>
    private static readonly RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

    /// <summary>
    /// The _rgx bb code localization tag.
    /// </summary>
    private static readonly string _rgxBBCodeLocalizationTag = @"\[localization=(?<tag>[^\]]*)\](?<inner>(.+?))\[/localization\]";

    /// <summary>
    /// The _rgx bold.
    /// </summary>
    private static readonly Regex _rgxBold = new Regex(@"\[B\](?<inner>(.*?))\[/B\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx br.
    /// </summary>
    private static readonly string _rgxBr = "[\r]?\n";

    /// <summary>
    /// The _rgx bullet.
    /// </summary>
    private static readonly string _rgxBullet = @"\[\*\]";

    /// <summary>
    /// The _rgx center.
    /// </summary>
    private static readonly string _rgxCenter = @"\[center\](?<inner>(.*?))\[/center\]";

    /// <summary>
    /// The _rgx code 1.
    /// </summary>
    private static readonly Regex _rgxCode1 = new Regex(@"\[code\](?<inner>(.*?))\[/code\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx code 2.
    /// </summary>
    private static readonly Regex _rgxCode2 = new Regex(@"\[code=(?<language>[^\]]*)\](?<inner>(.*?))\[/code\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx color.
    /// </summary>
    private static readonly string _rgxColor = @"\[color=(?<color>(\#?[-a-z0-9]*))\](?<inner>(.*?))\[/color\]";

    /// <summary>
    /// The _rgx email 1.
    /// </summary>
    private static readonly Regex _rgxEmail1 = new Regex(@"\[email[^\]]*\](?<inner>(.+?))\[/email\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx email 2.
    /// </summary>
    private static readonly Regex _rgxEmail2 = new Regex(@"\[email=(?<email>[^\]]*)\](?<inner>(.+?))\[/email\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx font.
    /// </summary>
    private static readonly string _rgxFont = @"\[font=(?<font>([-a-z0-9, ]*))\](?<inner>(.*?))\[/font\]";

    /// <summary>
    /// The _rgx hr.
    /// </summary>
    private static readonly string _rgxHr = "^[-][-][-][-][-]*[\r]?[\n]";

    /// <summary>
    /// The _rgx img.
    /// </summary>
    private static readonly Regex _rgxImg = new Regex(
      @"\[img\](?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>(.+?\.((jpg(.*?))|(png(.*?))|(gif(.*?))|(tif(.*?))|(ashx(.*?))|(php(.*?)))))\[/img\]", _options | RegexOptions.Compiled);

       /// <summary>
    /// The _rgx img Title.
    /// </summary>
    private static readonly Regex _rgxImgTitle = new Regex(
      @"\[img=(?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>(.+?\.((jpg(.*?))|(png(.*?))|(gif(.*?))|(tif(.*?))|(ashx(.*?))|(php(.*?)))))\](?<description>(.*?))\[/img\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx italic.
    /// </summary>
    private static readonly string _rgxHighlighted = @"\[h\](?<inner>(.*?))\[/h\]";

    /// <summary>
    /// The _rgx italic.
    /// </summary>
    private static readonly string _rgxItalic = @"\[I\](?<inner>(.*?))\[/I\]";

    /// <summary>
    /// The _rgx left.
    /// </summary>
    private static readonly string _rgxLeft = @"\[left\](?<inner>(.*?))\[/left\]";

    /// <summary>
    /// The _rgx list 1.
    /// </summary>
    private static readonly string _rgxList1 = @"\[list\](?<inner>(.*?))\[/list\]";

    /// <summary>
    /// The _rgx list 2.
    /// </summary>
    private static readonly string _rgxList2 = @"\[list=1\](?<inner>(.*?))\[/list\]";

    /// <summary>
    /// The _rgx list 3.
    /// </summary>
    private static readonly string _rgxList3 = @"\[list=a\](?<inner>(.*?))\[/list\]";

    /// <summary>
    /// The _rgx list 4.
    /// </summary>
    private static readonly string _rgxList4 = @"\[list=i\](?<inner>(.*?))\[/list\]";

    /// <summary>
    /// The _rgx post.
    /// </summary>
    private static readonly string _rgxPost = @"\[post=(?<post>[^\]]*)\](?<inner>(.*?))\[/post\]";

    /// <summary>
    /// The _rgx quote 1.
    /// </summary>
    private static readonly Regex _rgxQuote1 = new Regex(@"\[quote\](?<inner>(.*?))\[/quote\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx quote 2.
    /// </summary>
    private static readonly Regex _rgxQuote2 = new Regex(@"\[quote=(?<quote>[^\]]*)\](?<inner>(.*?))\[/quote\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx quote 3.
    /// </summary>
    private static readonly Regex _rgxQuote3 = new Regex(@"\[quote=(?<quote>(.*?));(?<id>([0-9]*))\](?<inner>(.*?))\[/quote\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx right.
    /// </summary>
    private static readonly string _rgxRight = @"\[right\](?<inner>(.*?))\[/right\]";

    /// <summary>
    /// The _rgx size.
    /// </summary>
    private static readonly string _rgxSize = @"\[size=(?<size>([1-9]))\](?<inner>(.*?))\[/size\]";

    /// <summary>
    /// The _rgx strike.
    /// </summary>
    private static readonly string _rgxStrike = @"\[S\](?<inner>(.*?))\[/S\]";

    /// <summary>
    /// The _rgx topic.
    /// </summary>
    private static readonly string _rgxTopic = @"\[topic=(?<topic>[^\]]*)\](?<inner>(.*?))\[/topic\]";

    /// <summary>
    /// The _rgx underline.
    /// </summary>
    private static readonly string _rgxUnderline = @"\[U\](?<inner>(.*?))\[/U\]";

    /// <summary>
    /// The _rgx url 1.
    /// </summary>
    private static readonly Regex _rgxUrl1 = new Regex(
      @"\[url\](?<http>(skype:)|(http://)|(https://)| (ftp://)|(ftps://))?(?<inner>(.+?))\[/url\]", _options | RegexOptions.Compiled);

    /// <summary>
    /// The _rgx url 2.
    /// </summary>
    private static readonly Regex _rgxUrl2 =
      new Regex(
        @"\[url\=(?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))?(?<url>([^\]]*?))\](?<inner>(.+?))\[/url\]", _options | RegexOptions.Compiled);

      /// <summary>
      /// The _rgx url 1.
      /// </summary>
      private static readonly Regex _rgxModalUrl1 =
          new Regex(
              @"\[modalurl](?<http>(skype:)|(http://)|(https://)| (ftp://)|(ftps://))?(?<inner>(.+?))\[/modalurl\]",
              _options | RegexOptions.Compiled);

      /// <summary>
      /// The _rgx url 2.
      /// </summary>
      private static readonly Regex _rgxModalUrl2 =
          new Regex(
              @"\[modalurl\=(?<http>(skype:)|(http://)|(https://)|(ftp://)|(ftps://))?(?<url>([^\]]*?))\](?<inner>(.+?))\[/modalurl\]",
              _options | RegexOptions.Compiled);

    /// <summary>
    /// Converts a string containing YafBBCode to the equivalent HTML string.
    /// </summary>
    /// <param name="inputString">
    /// Input string containing YafBBCode to convert to HTML
    /// </param>
    /// <param name="doFormatting">
    /// </param>
    /// <param name="targetBlankOverride">
    /// </param>
    /// <returns>
    /// The make html.
    /// </returns>
    public string MakeHtml(string inputString, bool doFormatting, bool targetBlankOverride)
    {
      var ruleEngine =
        this.ProcessReplaceRulesFactory(
          new[] { doFormatting, targetBlankOverride, this.Get<YafBoardSettings>().UseNoFollowLinks });

      if (!ruleEngine.HasRules)
      {
        this.CreateBBCodeRules(ruleEngine, doFormatting, targetBlankOverride, this.Get<YafBoardSettings>().UseNoFollowLinks);
      }

      ruleEngine.Process(ref inputString);

      return inputString;
    }

    /// <summary>
    /// Converts a message containing YafBBCode to HTML appropriate for editing in a rich text editor.
    /// </summary>
    /// <remarks>
    /// YafBBCode quotes are not converted to HTML.  "[quote]...[/quote]" will remain in the string 
    /// returned, as to appear in plain text in rich text editors.
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
          new[] { true, false, this.Get<YafBoardSettings>().UseNoFollowLinks, true });

      if (!ruleEngine.HasRules)
      {
          // NOTE : Do not convert BBQuotes, BBCodes and Custom BBCodes to HTML when editing -- "[quote]...[/quote]", and [code]..[/code] will remain in plaintext in the rich text editor
          this.CreateBBCodeRules(ruleEngine, true, false, this.Get<YafBoardSettings>().UseNoFollowLinks, false /*convertBBQuotes*/);
      }

      ruleEngine.Process(ref message);

      return message;
    }

    /// <summary>
    /// Converts a message containing HTML to YafBBCode for editing in a rich bbcode editor.
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
            new[] { DoFormatting, TargetBlankOverride, this.Get<YafBoardSettings>().UseNoFollowLinks, ForBBCodeEditing });

        if (!ruleEngine.HasRules)
        {
            this.CreateHtmlRules(ruleEngine, DoFormatting, true);
        }

        ruleEngine.Process(ref message);

        return message;
    }

    /// <summary>
    /// Creates the rules that convert <see cref="YafBBCode"/> to HTML
    /// </summary>
    /// <param name="ruleEngine">
    /// The rule Engine.
    /// </param>
    /// <param name="doFormatting">
    /// The do Formatting.
    /// </param>
    /// <param name="targetBlankOverride">
    /// The target Blank Override.
    /// </param>
    /// <param name="useNoFollow">
    /// The use No Follow.
    /// </param>
    /// <param name="convertBBQuotes">
    /// The convert BB Quotes.
    /// </param>
    public void CreateBBCodeRules(IProcessReplaceRules ruleEngine, bool doFormatting, bool targetBlankOverride, bool useNoFollow, bool convertBBQuotes)
    {
        string target = (this.Get<YafBoardSettings>().BlankLinks || targetBlankOverride)
                            ? "target=\"_blank\""
                            : string.Empty;
        string nofollow = useNoFollow ? "rel=\"nofollow\"" : string.Empty;

        const string ClassModal = "class=\"ceebox\"";

        // pull localized strings
        string localQuoteStr = this.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTE");
        string localQuoteWroteStr = this.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEWROTE");
        string localQuotePostedStr = this.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEPOSTED");
        string localCodeStr = YafContext.Current.Get<ILocalization>().GetText("COMMON", "BBCODE_CODE");

        // handle font sizes -- this rule class internally handles the "size" variable
        ruleEngine.AddRule(
            new FontSizeRegexReplaceRule(_rgxSize, @"<span style=""font-size:${size}"">${inner}</span>", _options));

        if (doFormatting)
        {
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxBold, "<b>${inner}</b>"));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxStrike, "<s>${inner}</s>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxItalic, "<em>${inner}</em>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxUnderline, "<u>${inner}</u>", _options));
            ruleEngine.AddRule(
                new SimpleRegexReplaceRule(_rgxHighlighted, @"<span class=""highlight"">${inner}</span>", _options));
            
            // e-mails
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxEmail2,
                    "<a href=\"mailto:${email}\">${inner}</a>",
                    new[]
                        {
                            "email"
                        }));

            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxEmail1, @"<a href=""mailto:${inner}"">${inner}</a>"));

            // urls
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxUrl2,
                    "<a {0} {1} href=\"${http}${url}\" title=\"${http}${url}\">${inner}</a>".Replace("{0}", target).
                        Replace("{1}", nofollow),
                    new[]
                        {
                            "url", "http"
                        },
                    new[]
                        {
                            string.Empty, "http://"
                        }));
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxUrl1,
                    "<a {0} {1} href=\"${http}${inner}\" title=\"${http}${inner}\">${http}${innertrunc}</a>".Replace(
                        "{0}", target).Replace("{1}", nofollow),
                    new[]
                        {
                            "http"
                        },
                    new[]
                        {
                            string.Empty, "http://"
                        },
                    50));

            // urls
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxModalUrl2,
                    "<a {0} {1} {2} href=\"${http}${url}\" title=\"${http}${url}\">${inner}</a>".Replace("{0}", target).
                        Replace("{1}", nofollow).Replace("{2}", ClassModal),
                    new[]
                        {
                            "url", "http"
                        },
                    new[]
                        {
                            string.Empty, "http://"
                        }));
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxModalUrl1,
                    "<a {0} {1} {2} href=\"${http}${inner}\" title=\"${http}${inner}\">${http}${innertrunc}</a>".Replace(
                        "{0}", target).Replace("{1}", nofollow).Replace("{2}", ClassModal),
                    new[]
                        {
                            "http"
                        },
                    new[]
                        {
                            string.Empty, "http://"
                        },
                    50));

            // font
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxFont,
                    "<span style=\"font-family:${font}\">${inner}</span>",
                    _options,
                    new[]
                        {
                            "font"
                        }));

            // color
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxColor,
                    "<span style=\"color:${color}\">${inner}</span>",
                    _options,
                    new[]
                        {
                            "color"
                        }));

            // bullets
            ruleEngine.AddRule(new SingleRegexReplaceRule(_rgxBullet, "<li>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxList4, "<ol type=\"i\">${inner}</ol>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxList3, "<ol type=\"a\">${inner}</ol>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxList2, "<ol>${inner}</ol>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxList1, "<ul>${inner}</ul>", _options));

            // alignment
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxCenter, "<div align=\"center\">${inner}</div>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxLeft, "<div align=\"left\">${inner}</div>", _options));
            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxRight, "<div align=\"right\">${inner}</div>", _options));

            // image
            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxImg,
                    "<img src=\"${http}${inner}\" alt=\"\"/>",
                    new[]
                        {
                            "http"
                        },
                    new[]
                        {
                            "http://"
                        }));

            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxImgTitle,
                    "<img src=\"${http}${inner}\" alt=\"${description}\" title=\"${description}\" />",
                    new[]
                        {
                            "http", "description"
                        },
                    new[]
                        {
                            "http://"
                        }));           

            // basic hr and br rules
            var hrRule = new SingleRegexReplaceRule(_rgxHr, "<hr />", _options | RegexOptions.Multiline);

            // Multiline, since ^ must match beginning of line
            var brRule = new SingleRegexReplaceRule(_rgxBr, "<br />", _options) { RuleRank = hrRule.RuleRank + 1 };

            // Ensure the newline rule is processed after the HR rule, otherwise the newline characters in the HR regex will never match
            ruleEngine.AddRule(hrRule);
            ruleEngine.AddRule(brRule);
        }

        // add smilies
        this.AddSmiles(ruleEngine);

        if (convertBBQuotes)
        {
            // add rule for code block type with syntax highlighting
            ruleEngine.AddRule(
                new SyntaxHighlightedCodeRegexReplaceRule(
                    _rgxCode2,
                    @"<div class=""code""><strong>{0}</strong><div class=""innercode"">${inner}</div></div>".Replace(
                        "{0}", localCodeStr)));

            // handle custom YafBBCode
            this.AddCustomBBCodeRules(ruleEngine);

            // add rule for code block type with no syntax highlighting
            ruleEngine.AddRule(
                new CodeRegexReplaceRule(
                    _rgxCode1,
                    @"<div class=""code""><strong>{0}</strong><div class=""innercode"">${inner}</div></div>".Replace("{0}", localCodeStr)));

            // "quote" handling...
            string tmpReplaceStr2 =
                @"<div class=""quote""><span class=""quotetitle"">{0}</span><div class=""innerquote"">{1}</div></div>".
                    FormatWith(localQuoteWroteStr.Replace("{0}", "${quote}"), "${inner}");

            ruleEngine.AddRule(
               new VariableRegexReplaceRule(
                   _rgxQuote2,
                   tmpReplaceStr2,
                   new[]
                        {
                            "quote"
                        }));

            string tmpReplaceStr1 =
                @"<div class=""quote""><span class=""quotetitle"">{0}</span><div class=""innerquote"">{1}</div></div>".
                    FormatWith(localQuoteStr, "${inner}");

            ruleEngine.AddRule(new SimpleRegexReplaceRule(_rgxQuote1, tmpReplaceStr1));

          var sl = YafContext.Current.ServiceLocator;

          string tmpReplaceStr3 =
            @"<div class=""quote""><span class=""quotetitle"">{0} <a href=""{1}""><img src=""{2}"" title=""{3}"" alt=""{3}"" /></a></span><div class=""innerquote"">{4}</div></div>"
              .FormatWith(
                localQuotePostedStr.Replace("{0}", "${quote}"),
                YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", "${id}"),
                sl.Get<ITheme>().GetItem("ICONS", "ICON_LATEST"),
                sl.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEPOSTED_TT"),
                "${inner}");

            ruleEngine.AddRule(
                new VariableRegexReplaceRule(
                    _rgxQuote3,
                    tmpReplaceStr3,
                    new[]
                        {
                            "quote", "id"
                        }));
        }

        // post and topic rules...
        ruleEngine.AddRule(
            new PostTopicRegexReplaceRule(
                _rgxPost, @"<a {0} href=""${post}"">${inner}</a>".Replace("{0}", target), _options));

        ruleEngine.AddRule(
            new PostTopicRegexReplaceRule(
                _rgxTopic, @"<a {0} href=""${topic}"">${inner}</a>".Replace("{0}", target), _options));
    }

    /// <summary>
    /// Creates the rules that convert HTML to <see cref="YafBBCode"/>
    /// </summary>
    /// <param name="ruleEngine">
    /// The rule Engine.
    /// </param>
    /// <param name="doFormatting">
    /// The do Formatting.
    /// </param>
    /// <param name="convertBBQuotes">
    /// The convert BB Quotes.
    /// </param>
    public void CreateHtmlRules(IProcessReplaceRules ruleEngine, bool doFormatting, bool convertBBQuotes)
    {
        // e-mails
        ruleEngine.AddRule(
            new VariableRegexReplaceRule(
                @"<a.*?href=""mailto:(?<email>(.*?))"".*?>(?<inner>(.*?))</a>",
               "[email=${email}]${inner}[/email]",
                _options,
                new[] { "email" }));

        // urls
        ruleEngine.AddRule(
            new VariableRegexReplaceRule(
                @"<a.*?href=""(?<inner>(.*?))"".*?>(?<description>(.*?))</a>",
               "[url=${inner}]${description}[/url]",
                _options,
                new[] { "description" }));

        // TODO : this.AddSmiles(ruleEngine);

        ruleEngine.AddRule(
            new VariableRegexReplaceRule(
                @"<img.*?src=""(?<inner>(.*?))"".*?alt=""(?<description>(.*?))"".*?/>",
                "[img=${inner}]${description}[/img]",
                _options,
                new[] { "description" }));

        // handle font sizes -- this rule class internally handles the "size" variable
        ruleEngine.AddRule(
            new FontSizeRegexReplaceRule(
                @"<span style=""font-size: (?<size>([1-9]));"">(?<inner>(.*?))</span>",
                "[size=${size}]${inner}[/size]",
                _options));

        // font
        ruleEngine.AddRule(
            new VariableRegexReplaceRule(
                @"<span style=""font-family: (?<font>(.*?));"">(?<inner>(.*?))</span>",
                "[font=${font}]${inner}[/font]",
                _options,
                new[] { "font" }));

        // color
        ruleEngine.AddRule(
            new VariableRegexReplaceRule(
                @"<span style=""color: (?<color>(\#?[-a-z0-9]*));"">(?<inner>(.*?))</span>",
                "[color=${color}]${inner}[/color]",
                _options,
                new[] { "color" }));

        // bullets
        ruleEngine.AddRule(new SingleRegexReplaceRule("<li>", "[*]", _options));
        ruleEngine.AddRule(new SimpleRegexReplaceRule("<ol type=\"i\">(?<inner>(.*?))</ol>", "[list=i]${inner}[/list]", _options));
        ruleEngine.AddRule(new SimpleRegexReplaceRule("<ol type=\"a\">(?<inner>(.*?))</ol>", "[list=a]${inner}[/list]", _options));
        ruleEngine.AddRule(new SimpleRegexReplaceRule("<ol>(?<inner>(.*?))</ol>", "[list=1]${inner}[/list]", _options));
        ruleEngine.AddRule(new SimpleRegexReplaceRule("<ul>(?<inner>(.*?))</ul>", "[list]${inner}[/list]", _options));

        // alignment
        ruleEngine.AddRule(new SimpleRegexReplaceRule("<div align=\"center\">(?<inner>(.*?))</div>", "[center]${inner}[/center]", _options));
        ruleEngine.AddRule(new SimpleRegexReplaceRule("<div align=\"left\">(?<inner>(.*?))</div>", "[left]${inner}[/left]", _options));
        ruleEngine.AddRule(new SimpleRegexReplaceRule("<div align=\"right\">(?<inner>(.*?))</div>", "[right]${inner}[/right]", _options));

        ruleEngine.AddRule(new SingleRegexReplaceRule("<b>", "[b]",  _options));
        ruleEngine.AddRule(new SingleRegexReplaceRule("</b>", "[/b]", _options));

        ruleEngine.AddRule(new SingleRegexReplaceRule("<strong>", "[b]", _options));
        ruleEngine.AddRule(new SingleRegexReplaceRule("</strong>", "[/b]", _options));

        ruleEngine.AddRule(new SingleRegexReplaceRule("<s>", "[s]",  _options));
        ruleEngine.AddRule(new SingleRegexReplaceRule("</s>", "[/s]", _options));

        ruleEngine.AddRule(new SingleRegexReplaceRule("<em>", "[i]",  _options));
        ruleEngine.AddRule(new SingleRegexReplaceRule("</em>", "[/i]", _options));

        ruleEngine.AddRule(new SingleRegexReplaceRule("<u>", "[u]",  _options));
        ruleEngine.AddRule(new SingleRegexReplaceRule("</u>", "[/u]", _options));
        ruleEngine.AddRule(
            new SimpleRegexReplaceRule(@"<span style=""text-decoration: underline;"">(?<inner>(.*?))</span>", "[u]${inner}[/u]", _options));

        ruleEngine.AddRule(
            new SimpleRegexReplaceRule(@"<span class=""highlight"">(?<inner>(.*?))</span>", "[h]${inner}[/h]", _options));

        // CODE Tags
        ruleEngine.AddRule(
            new VariableRegexReplaceRule(
                @"<div class=""code"">.*?<div class=""innercode"">.*?<pre class=""brush:(?<language>(.*?));"">(?<inner>(.*?))</pre>.*?</div>",
                "[code=${language}]${inner}[/code]",
                _options,
                new[] { "language" }));

        ruleEngine.AddRule(
           new SimpleRegexReplaceRule(
               "<div class=\"code\">.*?<div class=\"innercode\">(?<inner>(.*?))</div>",
               "[code]${inner}[/code]",
               _options));

        ruleEngine.AddRule(new SingleRegexReplaceRule("<br />", "\r\n", _options));

        // Remove remaining tags.
        ruleEngine.AddRule(new SingleRegexReplaceRule("<p>", string.Empty, _options));
        ruleEngine.AddRule(new SingleRegexReplaceRule("</p>", string.Empty, _options));
        ruleEngine.AddRule(new SingleRegexReplaceRule("&nbsp;", string.Empty, _options));

        // remove remaining tags
        ruleEngine.AddRule(new SingleRegexReplaceRule("<[^>]+>", string.Empty, _options) { RuleRank = 100 });
    }

    /// <summary>
    /// Adds smiles replacement rules to the collection from the DB
    /// </summary>
    /// <param name="rules">
    /// The rules.
    /// </param>
    public void AddSmiles([NotNull] IProcessReplaceRules rules)
    {
      CodeContracts.ArgumentNotNull(rules, "rules");

      var smiles = YafContext.Current.Get<IDBBroker>().GetSmilies();
      int codeOffset = 0;

      foreach (var smile in smiles)
      {
        string code = smile.Code;
        code = code.Replace("&", "&amp;");
        code = code.Replace(">", "&gt;");
        code = code.Replace("<", "&lt;");
        code = code.Replace("\"", "&quot;");

        // add new rules for smilies...
        var lowerRule = new SimpleReplaceRule(
          code.ToLower(),
          @"<img src=""{0}"" alt=""{1}"" />".FormatWith(
            YafBuildLink.Smiley(smile.Icon), HttpContext.Current.Server.HtmlEncode(smile.Emoticon)));

        var upperRule = new SimpleReplaceRule(
          code.ToUpper(),
          @"<img src=""{0}"" alt=""{1}"" />".FormatWith(
            YafBuildLink.Smiley(smile.Icon), HttpContext.Current.Server.HtmlEncode(smile.Emoticon)));

        // increase the rank as we go...
        lowerRule.RuleRank = lowerRule.RuleRank + 100 + codeOffset;
        upperRule.RuleRank = upperRule.RuleRank + 100 + codeOffset;

        rules.AddRule(lowerRule);
        rules.AddRule(upperRule);

        // add a bit more rank
        codeOffset++;
      }
    }

      /// <summary>
    /// Applies Custom YafBBCode Rules from the YafBBCode table
    /// </summary>
    /// <param name="rulesEngine">
    /// The rules Engine.
    /// </param>
    protected void AddCustomBBCodeRules(IProcessReplaceRules rulesEngine)
      {
        var bbcodeTable = YafContext.Current.Get<IDBBroker>().GetCustomBBCode();

      // handle custom bbcodes row by row...
        foreach (var codeRow in
          bbcodeTable.Where(codeRow => !(codeRow.UseModule ?? false) && codeRow.SearchRegex.IsSet()))
        {
          if (codeRow.Variables.IsSet())
          {
            // handle variables...
            string[] variables = codeRow.Variables.Split(
              new[]
                {
                  ';'
                });

              var rule = new VariableRegexReplaceRule(codeRow.SearchRegex, codeRow.ReplaceRegex, _options, variables)
                  {
                      RuleRank = 50
                  };

              rulesEngine.AddRule(rule);
          }
          else
          {
            // just standard replace...
              var rule = new SimpleRegexReplaceRule(codeRow.SearchRegex, codeRow.ReplaceRegex, _options)
                  {
                      RuleRank = 50
                  };

            rulesEngine.AddRule(rule);
          }
        }
      }

    /// <summary>
    /// Helper function that dandles registering "custom bbcode" javascript (if there is any)
    /// for all the custom YafBBCode.
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
    /// for all the custom YafBBCode. Defining editorID make the system also show "editor js" (if any).
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
      var bbCodeTable = YafContext.Current.Get<IDBBroker>().GetCustomBBCode();
      const string ScriptID = "custombbcode";
      var jsScriptBuilder = new StringBuilder();
      var cssBuilder = new StringBuilder();

      jsScriptBuilder.Append("\r\n");
      cssBuilder.Append("\r\n");

      foreach (var row in bbCodeTable)
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
          jsScriptBuilder.AppendLine(displayScript + "\r\n" + editScript);
        }

        // see if there is any CSS associated with this YafBBCode
        if (row.DisplayCSS.IsSet() && row.DisplayCSS.IsSet())
        {
          // yes, add it into the builder
          cssBuilder.AppendLine(this.LocalizeCustomBBCodeElement(row.DisplayCSS.Trim()));
        }
      }

      if (jsScriptBuilder.ToString().Trim().Length > 0)
      {
        YafContext.Current.PageElements.RegisterJsBlock(currentPage, "{0}_script".FormatWith(ScriptID), jsScriptBuilder.ToString());
      }

      if (cssBuilder.ToString().Trim().Length > 0)
      {
        // register the CSS from all custom bbcode...
        YafContext.Current.PageElements.RegisterCssBlock("{0}_css".FormatWith(ScriptID), cssBuilder.ToString());
      }
    }

    /// <summary>
    /// Handles localization for a Custom YafBBCode Elements using
    /// the code [localization=tag]default[/localization]
    /// </summary>
    /// <param name="strToLocalize">
    /// </param>
    /// <returns>
    /// The localize custom bb code element.
    /// </returns>
    public string LocalizeCustomBBCodeElement(string strToLocalize)
    {
      var regExSearch = new Regex(_rgxBBCodeLocalizationTag, _options);

      var sb = new StringBuilder(strToLocalize);

      Match m = regExSearch.Match(strToLocalize);
      while (m.Success)
      {
        // get the localization tag...
        string tagValue = m.Groups["tag"].Value;
        string defaultValue = m.Groups["inner"].Value;

        // remove old code...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert localized value...
        string localValue = defaultValue;

        if (YafContext.Current.Get<ILocalization>().GetTextExists("BBCODEMODULE", tagValue))
        {
          localValue = YafContext.Current.Get<ILocalization>().GetText("BBCODEMODULE", tagValue);
        }

        sb.Insert(m.Groups[0].Index, localValue);
        m = regExSearch.Match(sb.ToString());
      }

      return sb.ToString();
    }
  }
}