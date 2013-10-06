/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.BBCode.ReplaceRules;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// YAF FormatMessage provides functions related to formatting the post messages.
    /// </summary>
    public class YafFormatMessage : IFormatMessage, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        ///   format message regex
        /// </summary>
        private const RegexOptions _Options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        /// <summary>
        /// The mail regex
        /// </summary>
        private static readonly Regex _RgxEmail =
            new Regex(
                @"(?<before>^|[ ]|\>|\[[A-Za-z0-9]\])(?<inner>(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6})",
                _Options | RegexOptions.Compiled);

        /// <summary>
        /// The URL Regex
        /// </summary>
        private static readonly Regex _RgxUrl1 =
            new Regex(
                @"(?<before>^|[ ]|\[[A-Za-z0-9]\]|\[\*\]|[A-Za-z0-9])(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?+%#&=;:,~]*)?)",
                _Options | RegexOptions.Compiled);

        /// <summary>
        /// The URL Regex
        /// </summary>
        private static readonly Regex _RgxUrl2 =
            new Regex(
                @"(?<before>^|[ ]|\[[A-Za-z0-9]\]|\[\*\]|[A-Za-z0-9])(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=+;,:#~$]*[^.<|^.\[])?)",
                _Options | RegexOptions.Compiled);

        /// <summary>
        /// The URL Regex
        /// </summary>
        private static readonly Regex _RgxUrl3 =
            new Regex(
                @"(?<before>^|[ ]|\[[A-Za-z0-9]\]|\[\*\]|[A-Za-z0-9])(?<!http://)(?<inner>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%+#&=;,~]*)?)",
                _Options | RegexOptions.Compiled);

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="YafFormatMessage"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="httpServer">
        /// The http server.
        /// </param>
        /// <param name="processReplaceRuleFactory">
        /// The process replace rule factory.
        /// </param>
        public YafFormatMessage(
            IServiceLocator serviceLocator,
            HttpServerUtilityBase httpServer,
            Func<IEnumerable<bool>, IProcessReplaceRules> processReplaceRuleFactory)
        {
            this.ServiceLocator = serviceLocator;
            this.HttpServer = httpServer;
            this.ProcessReplaceRuleFactory = processReplaceRuleFactory;
        }

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Gets or sets HttpServer.
        /// </summary>
        public HttpServerUtilityBase HttpServer { get; set; }

        /// <summary>
        /// Gets or sets ProcessReplaceRuleFactory.
        /// </summary>
        public Func<IEnumerable<bool>, IProcessReplaceRules> ProcessReplaceRuleFactory { get; set; }       

        #region Public Methods

        /// <summary>
        /// The method to detect a forbidden BBCode tag from delimited delimiter list 
        ///   'stringToMatch'
        /// </summary>
        /// <param name="stringToClear">
        /// Input string
        /// </param>
        /// <param name="stringToMatch">
        /// String with delimiter
        /// </param>
        /// <param name="delim">
        /// The delimiter.
        /// </param>
        /// <returns>
        /// Returns a string containing a forbidden BBCode or a null string
        /// </returns>
        [CanBeNull]
        public string BBCodeForbiddenDetector(
            [NotNull] string stringToClear, [NotNull] string stringToMatch, char delim)
        {
            string[] codes = stringToMatch.Split(delim);

            var forbiddenTagList = new List<string>();

            MatchAndPerformAction(
                @"\[.*?\]",
                stringToClear,
                (tag, index, len) =>
                    {
                        var bbCode = tag.Replace("/", string.Empty).Replace("]", string.Empty);

                        // If tag contains attributes kill them for cecking
                        if (bbCode.Contains("="))
                        {
                            bbCode = bbCode.Remove(bbCode.IndexOf("="));
                        }

                        if (codes.Any(allowedTag => bbCode.ToLower() == allowedTag.ToLower()))
                        {
                            return;
                        }

                        if (!forbiddenTagList.Contains(bbCode))
                        {
                            forbiddenTagList.Add(bbCode);
                        }
                    });

            return forbiddenTagList.ToDelimitedString(",");

            /*bool checker = string.IsNullOrEmpty(stringToMatch);

            string[] codes = stringToMatch.Split(delim);
            charf] charray = stringToClear.ToCharArray();
            int currentPosition = 0;

            // Loop through char array i will be current poision
            for (int i = 0; i < charray.Length; i++)
            {
                if (i < currentPosition)
                {
                    continue;
                }

                // bbcode token is detected
                if (charray[i] != '[')
                {
                    continue;
                }

                int openPosition = i;

                // we loop to find closing bracket, beginnin with i position
                for (int j = i; j < charray.Length - 1; j++)
                {
                    // closing bracket found
                    if (charray[j] != ']')
                    {
                        continue;
                    }

                    // we should reset the value in each cycle 
                    // if an opening bracket was found
                    bool detectedTag = false;
                    string res = null;

                    // now we loop through out allowed bbcode list
                    foreach (string t in codes)
                    {
                        // closing bracket is in position 'j' opening is in pos 'i'
                        // we should not take into account closing bracket
                        // as we have tags like '[URL='
                        for (int l = openPosition; l < j; l++)
                        {
                            res = res + charray[l].ToString().ToUpper();
                        }

                        if (checker)
                        {
                            return "ALL";
                        }

                        // detect if the tag from list was found
                        detectedTag = res.Contains("[" + t.ToUpper().Trim()) || res.Contains("[/" + t.ToUpper().Trim());
                        res = string.Empty;

                        // if so we go out from k-loop after we should go out from j-loop too
                        if (!detectedTag)
                        {
                            continue;
                        }

                        currentPosition = j + 1;
                        break;
                    }

                    currentPosition = j + 1;

                    // we didn't found the allowed tag in k-loop through allowed list,
                    // so the tag is forbidden one and we should exit
                    if (!detectedTag)
                    {
                        string tagForbidden = stringToClear.Substring(i, j - i + 1).ToUpper();
                        return tagForbidden;
                    }

                    if (detectedTag)
                    {
                        break;
                    }

                    // continue to loop
                }
            }

            return null;*/
        }

        /// <summary>
        /// The method used to get response string, if a forbidden tag is detected.
        /// </summary>
        /// <param name="checkString">
        /// The string to check.
        /// </param>
        /// <param name="acceptedTags">
        /// The list of accepted tags.
        /// </param>
        /// <param name="delim">
        /// The delimiter in a tags list.
        /// </param>
        /// <returns>
        /// A message string.
        /// </returns>
        public string CheckHtmlTags([NotNull] string checkString, [NotNull] string acceptedTags, char delim)
        {
            string detectedHtmlTag = this.HtmlTagForbiddenDetector(checkString, acceptedTags, delim);

            if (!string.IsNullOrEmpty(detectedHtmlTag) && detectedHtmlTag != "ALL")
            {
                return YafContext.Current.Get<ILocalization>().GetTextFormatted(
                    "HTMLTAG_WRONG", HttpUtility.HtmlEncode(detectedHtmlTag));
            }

            return detectedHtmlTag == "ALL"
                       ? YafContext.Current.Get<ILocalization>().GetText("HTMLTAG_FORBIDDEN")
                       : string.Empty;
        }        

        /// <summary>
        /// The format message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageFlags">
        /// The message flags.
        /// </param>
        /// <param name="targetBlankOverride">
        /// The target blank override.
        /// </param>
        /// <param name="messageLastEdited">
        /// The message last edited.
        /// </param>
        /// <returns>
        /// The formatted message.
        /// </returns>
        public string FormatMessage(
            [NotNull] string message,
            [NotNull] MessageFlags messageFlags,
            bool targetBlankOverride,
            DateTime messageLastEdited)
        {
            var boardSettings = this.Get<YafBoardSettings>();

            bool useNoFollow = boardSettings.UseNoFollowLinks;

            // check to see if no follow should be disabled since the message is properly aged
            if (useNoFollow && boardSettings.DisableNoFollowLinksAfterDay > 0)
            {
                TimeSpan messageAge = messageLastEdited - DateTime.UtcNow;
                if (messageAge.Days > boardSettings.DisableNoFollowLinksAfterDay)
                {
                    // disable no follow
                    useNoFollow = false;
                }
            }

            // do html damage control
            message = this.RepairHtml(message, messageFlags.IsHtml);

            // get the rules engine from the creator...
            var ruleEngine =
                this.ProcessReplaceRuleFactory(
                    new[] { true /*messageFlags.IsBBCode*/, targetBlankOverride, useNoFollow });

            // see if the rules are already populated...
            if (!ruleEngine.HasRules)
            {
                // populate

                // get rules for YafBBCode and Smilies
                this.Get<IBBCode>().CreateBBCodeRules(
                    ruleEngine, true /*messageFlags.IsBBCode*/, targetBlankOverride, useNoFollow);

                // add email rule
                // vzrus: it's freezing  when post body contains full email adress.
                // the fix provided by community 
                var email = new VariableRegexReplaceRule(
                    _RgxEmail, "${before}<a href=\"mailto:${inner}\">${inner}</a>", new[] { "before" })
                    { RuleRank = 10 };

                ruleEngine.AddRule(email);

                // URLs Rules
                string target = (boardSettings.BlankLinks || targetBlankOverride) ? "target=\"_blank\"" : string.Empty;

                string nofollow = useNoFollow ? "rel=\"nofollow\"" : string.Empty;

                var url = new VariableRegexReplaceRule(
                    _RgxUrl1,
                    "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace("{0}", target).Replace("{1}", nofollow),
                    new[] { "before" },
                    new[] { string.Empty },
                    50) { RuleRank = 42 };

                ruleEngine.AddRule(url);

                // ?<! - match if prefixes href="" and src="" are not present
                // <inner> = named capture group
                // (http://|https://|ftp://) - numbered capture group - select from 3 alternatives
                // Match expression but don't capture it, one or more repetions, in the end is dot(\.)? here we match "www." - (?:[\w-]+\.)+
                // Match expression but don't capture it, zero or one repetions (?:/[\w-./?%&=+;,:#~$]*[^.<])?
                // (?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=+;,:#~$]*[^.<])?)
                url = new VariableRegexReplaceRule(
                    _RgxUrl2,
                    "${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace("{0}", target).Replace("{1}", nofollow),
                    new[]
                        {
                            "before"
                        },
                    new[] { string.Empty },
                    50) { RuleRank = 43 };

                ruleEngine.AddRule(url);

                url = new VariableRegexReplaceRule(
                    _RgxUrl3,
                    "${before}<a {0} {1} href=\"http://${inner}\" title=\"http://${inner}\">${innertrunc}</a>".Replace(
                        "{0}", target).Replace("{1}", nofollow),
                    new[]
                        {
                            "before"
                        },
                    new[] { string.Empty },
                    50) { RuleRank = 44 };

                ruleEngine.AddRule(url);
            }

            // process...
            ruleEngine.Process(ref message);

            message = this.Get<IBadWordReplace>().Replace(message);

            return message;
        }

        /// <summary>
        /// The format syndication message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageFlags">
        /// The message flags.
        /// </param>
        /// <param name="altItem">
        /// The alt Item.
        /// </param>
        /// <param name="charsToFetch">
        /// The chars To Fetch.
        /// </param>
        /// <returns>
        /// The formatted message.
        /// </returns>
        [NotNull]
        public string FormatSyndicationMessage(
            [NotNull] string message, [NotNull] MessageFlags messageFlags, bool altItem, int charsToFetch)
        {
            message =
                @"<table class=""{0}"" width=""100%""><tr><td>{1}</td></tr></table>".FormatWith(
                    altItem ? "content postContainer" : "content postContainer_Alt",
                    this.FormatMessage(message, messageFlags, false));

            message = message.Replace("<div class=\"innerquote\">", "<blockquote>").Replace("[quote]", "</blockquote>");

            // Remove HIDDEN Text
            message = this.RemoveHiddenBBCodeContent(message);

            message = this.RemoveCustomBBCodes(message);

            return message;

            // <span class=\"quotetitle\">tester1 íàïèñàë:</span><div class=\"innerquote\">gfhgfhdf</div></div><br />vcxvxcvzcxv</td></tr></tab
        }

        /// <summary>
        /// The get cleaned topic message. Caches cleaned topic message by TopicID.
        /// </summary>
        /// <param name="topicMessage">
        /// The message to clean.
        /// </param>
        /// <param name="topicId">
        ///   The topic id.
        /// </param>
        /// <returns>
        /// The get cleaned topic message.
        /// </returns>
        public MessageCleaned GetCleanedTopicMessage([NotNull] object topicMessage, [NotNull] object topicId)
        {
            CodeContracts.VerifyNotNull(topicMessage, "topicMessage");
            CodeContracts.VerifyNotNull(topicId, "topicId");

            // get the common words for the language -- should be all lower case.
            List<string> commonWords = this.Get<ILocalization>().GetText("COMMON", "COMMON_WORDS").StringToList(',');

            string cacheKey = Constants.Cache.FirstPostCleaned.FormatWith(YafContext.Current.PageBoardID, topicId);
            var message = new MessageCleaned();

            if (!topicMessage.IsNullOrEmptyDBField())
            {
                message = this.Get<IDataCache>().GetOrSet(
                    cacheKey,
                    () =>
                        {
                            string returnMsg = topicMessage.ToString();
                            var keywordList = new List<string>();

                            if (returnMsg.IsSet())
                            {
                                // process message... clean html, strip html, remove bbcode, etc...
                                returnMsg =
                                    BBCodeHelper.StripBBCode(
                                        HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(returnMsg))).RemoveMultipleWhitespace();

                                // encode Message For Security Reasons
                                returnMsg = this.HttpServer.HtmlEncode(returnMsg);

                                if (returnMsg.IsNotSet())
                                {
                                    returnMsg = string.Empty;
                                }
                                else
                                {
                                    // get string without punctuation
                                    string keywordCleaned =
                                        new string(
                                            returnMsg.Where(c => !char.IsPunctuation(c) || char.IsWhiteSpace(c)).ToArray()).Trim().ToLower();

                                    if (keywordCleaned.Length > 5)
                                    {
                                        // create keywords...
                                        keywordList = keywordCleaned.StringToList(' ', commonWords);

                                        // clean up the list a bit...
                                        keywordList =
                                            keywordList.GetNewNoEmptyStrings().GetNewNoSmallStrings(5).Where(x => !char.IsNumber(x[0])).Distinct().ToList();

                                        // sort...
                                        keywordList.Sort();

                                        // get maximum of 50 keywords...
                                        if (keywordList.Count > 50)
                                        {
                                            keywordList = keywordList.GetRange(0, 50);
                                        }
                                    }
                                }
                            }

                            return new MessageCleaned(returnMsg.Truncate(255), keywordList);
                        },
                    TimeSpan.FromMinutes(this.Get<YafBoardSettings>().FirstPostCacheTimeout));
            }

            return message;
        }

        /// <summary>
        /// The method to detect a forbidden HTML code from delimited by delimiter list
        /// </summary>
        /// <param name="stringToClear">
        /// The string To Clear.
        /// </param>
        /// <param name="stringToMatch">
        /// The string To Match.
        /// </param>
        /// <param name="delim">
        /// The delimiter string.
        /// </param>
        /// <returns>
        /// Returns a forbidden HTML tag or a null string
        /// </returns>
        [CanBeNull]
        public string HtmlTagForbiddenDetector(
            [NotNull] string stringToClear, [NotNull] string stringToMatch, char delim)
        {
            string[] codes = stringToMatch.Split(delim);

            var forbiddenTagList = new List<string>();

            MatchAndPerformAction(
                "<.*?>",
                stringToClear,
                (tag, index, len) =>
                    {
                        var code = tag.Replace("/", string.Empty).Replace(">", string.Empty);

                        // If tag contains attributes kill them for cecking
                        if (code.Contains("=\""))
                        {
                            code = code.Remove(code.IndexOf(" "));
                        }

                        if (codes.Any(allowedTag => code.ToLower() == allowedTag.ToLower()))
                        {
                            return;
                        }

                        if (!forbiddenTagList.Contains(code))
                        {
                            forbiddenTagList.Add(code);
                        }
                    });

            return forbiddenTagList.ToDelimitedString(",");

            /*bool checker = string.IsNullOrEmpty(stringToMatch);

            //string[] codes = stringToMatch.Split(delim);
            char[] charray = stringToClear.ToCharArray();
            int currentPosition = 0;

            // Loop through char array i will be current poision
            for (int i = 0; i < charray.Length; i++)
            {
                if (i >= currentPosition)
                {
                    // bbcode token is detected
                    if (charray[i] == '<')
                    {
                        int openPosition = i;

                        // we loop to find closing bracket, beginnin with i position
                        for (int j = i; j < charray.Length - 1; j++)
                        {
                            // closing bracket found
                            if (charray[j] != '>')
                            {
                                continue;
                            }

                            // we should reset the value in each cycle 
                            // if an opening bracket was found
                            bool detectedTag = false;
                            string res = null;

                            // now we loop through out allowed bbcode list
                            foreach (string t in codes)
                            {
                                // closing bracket is in position 'j' opening is in pos 'i'
                                // we should not take into account closing bracket
                                // as we have tags like '[URL='
                                for (int l = openPosition; l < j; l++)
                                {
                                    res = res + charray[l].ToString().ToUpper();
                                }

                                if (checker)
                                {
                                    return "ALL";
                                }

                                // detect if the tag from list was found
                                detectedTag = res.Contains("<" + t.ToUpper().Trim()) || res.Contains("</" + t.ToUpper().Trim());
                                res = string.Empty;

                                // if so we go out from k-loop after we should go out from j-loop too
                                if (!detectedTag)
                                {
                                    continue;
                                }

                                currentPosition = j + 1;
                                break;
                            }

                            currentPosition = j + 1;

                            // we didn't found the allowed tag in k-loop through allowed list,
                            // so the tag is forbidden one and we should exit
                            if (!detectedTag)
                            {
                                string tagForbidden = stringToClear.Substring(i, j - i + 1).ToUpper();
                                return tagForbidden;
                            }

                            if (detectedTag)
                            {
                                break;
                            }

                            // continue to loop
                        }
                    }
                }
            }
            */
        }

        /// <summary>
        /// Removes nested BBCode quotes from the given message body.
        /// </summary>
        /// <param name="body">
        /// Message body test to remove nested quotes from
        /// </param>
        /// <returns>
        /// A version of <paramref name="body"/> that contains no nested quotes.
        /// </returns>
        [NotNull]
        public string RemoveNestedQuotes([NotNull] string body)
        {
            const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
            var quote = new Regex(@"\[quote(\=[^\]]*)?\](.*?)\[/quote\]", Options);

            // remove quotes from old messages
            return quote.Replace(body, string.Empty).TrimStart();
        }

        /// <summary>
        /// Removes BBCode Posted Hidden Content
        /// </summary>
        /// <param name="body">
        /// Message body to remove the hidden content from
        /// </param>
        /// <returns>
        /// The Cleaned body.
        /// </returns>
        [NotNull]
        public string RemoveHiddenBBCodeContent([NotNull] string body)
        {
            const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;

            var hiddenRegex =
                new Regex(
                    @"\[hide-reply\](?<inner>(.|\n)*?)\[\/hide-reply\]|\[hide-reply-thanks\](?<inner>(.|\n)*?)\[\/hide-reply-thanks\]|\[group-hide\](?<inner>(.|\n)*?)\[\/group-hide\]|\[hide\](?<inner>(.|\n)*?)\[\/hide\]|\[group-hide(\=[^\]]*)?\](?<inner>(.|\n)*?)\[\/group-hide\]|\[hide-thanks(\=[^\]]*)?\](?<inner>(.|\n)*?)\[\/hide-thanks\]|\[hide-posts(\=[^\]]*)?\](?<inner>(.|\n)*?)\[\/hide-posts\]",
                    Options);

            Match hiddenTagMatch = hiddenRegex.Match(body);

            if (hiddenTagMatch.Success)
            {
                body = hiddenRegex.Replace(body, string.Empty);
            }

            return body;
        }

        /// <summary>
        /// Removes Custom BBCodes
        /// </summary>
        /// <param name="body">
        /// Message body to remove the hidden content from
        /// </param>
        /// <returns>
        /// The Cleaned body.
        /// </returns>
        [NotNull]
        public string RemoveCustomBBCodes([NotNull] string body)
        {
            const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;

            var spoilerRegex = new Regex(@"\[SPOILER\](?<inner>(.|\n)*?)\[\/SPOILER\]", Options);

            Match spoilerTagMatch = spoilerRegex.Match(body);

            if (spoilerTagMatch.Success)
            {
                body = spoilerTagMatch.Groups["inner"].Value;
            }

            return body;
        }

        /// <summary>
        /// The repair html.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="allowHtml">
        /// The allow html.
        /// </param>
        /// <returns>
        /// The repaired html.
        /// </returns>
        public string RepairHtml([NotNull] string html, bool allowHtml)
        {
            // vzrus: NNTP temporary tweaks to wipe out server hangs. Put it here as it can be in every place.
            // These are '\n\r' things related to multiline regexps.
            MatchCollection mc1 = Regex.Matches(html, "[^\r]\n[^\r]", RegexOptions.IgnoreCase);
            for (int i = mc1.Count - 1; i >= 0; i--)
            {
                html = html.Insert(mc1[i].Index + 1, " \r");
            }

            MatchCollection mc2 = Regex.Matches(html, "[^\r]\n\r\n[^\r]", RegexOptions.IgnoreCase);
            for (int i = mc2.Count - 1; i >= 0; i--)
            {
                html = html.Insert(mc2[i].Index + 1, " \r");
            }

            html = !allowHtml
                       ? this.HttpServer.HtmlEncode(html)
                       : RemoveHtmlByList(html, this.Get<YafBoardSettings>().AcceptedHTML.Split(','));

            return html;
        }

        /// <summary>
        /// The repair html.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="allowHtml">
        /// The allow html.
        /// </param>
        /// <param name="matchList">
        /// The match List.
        /// </param>
        /// <returns>
        /// The repaired html.
        /// </returns>
        public string RepairHtml([NotNull] string html, [NotNull] bool allowHtml, [NotNull] IEnumerable<string> matchList)
        {
            // vzrus: NNTP temporary tweaks to wipe out server hangs. Put it here as it can be in every place.
            // These are '\n\r' things related to multiline regexps.
            MatchCollection mc1 = Regex.Matches(html, "[^\r]\n[^\r]", RegexOptions.IgnoreCase);
            for (int i = mc1.Count - 1; i >= 0; i--)
            {
                html = html.Insert(mc1[i].Index + 1, " \r");
            }

            MatchCollection mc2 = Regex.Matches(html, "[^\r]\n\r\n[^\r]", RegexOptions.IgnoreCase);
            for (int i = mc2.Count - 1; i >= 0; i--)
            {
                html = html.Insert(mc2[i].Index + 1, " \r");
            }

            html = !allowHtml
                       ? this.HttpServer.HtmlEncode(html)
                       : RemoveHtmlByList(html, matchList);

            return html;
        }

        /// <summary>
        /// The repair html.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="allowHtml">
        /// The allow html.
        /// </param>
        /// <returns>
        /// The repaired html.
        /// </returns>
        public string RepairHtmlFeeds([NotNull] string html, bool allowHtml)
        {
            // vzrus: NNTP temporary tweaks to wipe out server hangs. Put it here as it can be in every place.
            // These are '\n\r' things related to multiline regexps.
            MatchCollection mc1 = Regex.Matches(html, "[^\r]\n[^\r]", RegexOptions.IgnoreCase);
            for (int i = mc1.Count - 1; i >= 0; i--)
            {
                html = html.Insert(mc1[i].Index + 1, " \r");
            }

            MatchCollection mc2 = Regex.Matches(html, "[^\r]\n\r\n[^\r]", RegexOptions.IgnoreCase);
            for (int i = mc2.Count - 1; i >= 0; i--)
            {
                html = html.Insert(mc2[i].Index + 1, " \r");
            }

            html = !allowHtml ? this.HttpServer.HtmlEncode(html) : RemoveHtmlByList(html, new[] { "a" });

            return html;
        }

        /// <summary>
        /// Surrounds a word list with prefix/postfix. Case insensitive.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="wordList">
        /// The word List.
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="postfix">
        /// The postfix.
        /// </param>
        /// <returns>
        /// The surround word list.
        /// </returns>
        public string SurroundWordList(
            [NotNull] string message,
            [NotNull] IList<string> wordList,
            [NotNull] string prefix,
            [NotNull] string postfix)
        {
            CodeContracts.VerifyNotNull(message, "message");
            CodeContracts.VerifyNotNull(wordList, "wordList");
            CodeContracts.VerifyNotNull(prefix, "prefix");
            CodeContracts.VerifyNotNull(postfix, "postfix");

            //// const RegexOptions regexOptions = RegexOptions.IgnoreCase;

            foreach (string word in wordList.Where(w => w.Length > 3))
            {
                MatchAndPerformAction(
                    "({0})".FormatWith(word.ToLower().ToRegExString()),
                    message,
                    (inner, index, length) =>
                        {
                            message = message.Insert(index + length, postfix);
                            message = message.Insert(index, prefix);
                        });

                // var matches = regEx.Matches(message).Cast<Match>().ToList().OrderByDescending(x => x.Index);

                // foreach (Match m in matches)
                // {
                // message = message.Insert(m.Index + m.Length, postfix);
                // message = message.Insert(m.Index, prefix);
                // }
            }

            return message;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The match and perform action.
        /// </summary>
        /// <param name="matchRegEx">
        /// The match regex.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="matchAction">
        /// The match action.
        /// </param>
        private static void MatchAndPerformAction(
            [NotNull] string matchRegEx, [NotNull] string text, [NotNull] Action<string, int, int> matchAction)
        {
            CodeContracts.VerifyNotNull(matchRegEx, "matchRegEx");
            CodeContracts.VerifyNotNull(text, "text");
            CodeContracts.VerifyNotNull(matchAction, "MatchAction");

            const RegexOptions Options = RegexOptions.IgnoreCase;

            var matches = Regex.Matches(text, matchRegEx, Options).Cast<Match>().OrderByDescending(x => x.Index);

            foreach (var match in matches)
            {
                string inner = text.Substring(match.Index + 1, match.Length - 1).Trim().ToLower();
                matchAction(inner, match.Index, match.Length);
            }
        }

        /// <summary>
        /// remove html by list.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="matchList">
        /// The match list.
        /// </param>
        /// <returns>
        /// The remove html by list.
        /// </returns>
        private static string RemoveHtmlByList([NotNull] string text, [NotNull] IEnumerable<string> matchList)
        {
            CodeContracts.VerifyNotNull(text, "text");
            CodeContracts.VerifyNotNull(matchList, "matchList");

            MatchAndPerformAction(
                "<.*?>",
                text,
                (tag, index, len) =>
                    {
                        if (!HtmlHelper.IsValidTag(tag, matchList))
                        {
                            text = text.Remove(index, len);
                        }
                    });

            return text;
        }

        #endregion
    }
}