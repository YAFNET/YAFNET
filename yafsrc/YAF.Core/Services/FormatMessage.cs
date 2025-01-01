﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
namespace YAF.Core.Services;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using YAF.Core.Utilities.StringUtils;

/// <summary>
/// YAF FormatMessage provides functions related to formatting the post messages.
/// </summary>
public class FormatMessage : IFormatMessage, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Services.FormatMessage"/> class.
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
    public FormatMessage(
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
    /// <param name="delimiter">
    /// The delimiter.
    /// </param>
    /// <returns>
    /// Returns a string containing a forbidden BBCode or a null string
    /// </returns>
    
    public string BBCodeForbiddenDetector(
        string stringToClear,
        string stringToMatch,
        char delimiter)
    {
        var codes = stringToMatch.Split(delimiter);

        var forbiddenTagList = new List<string>();

        MatchAndPerformAction(
            @"\[.*?\]",
            stringToClear,
            (tag, index, len) =>
                {
                    var bbCode = tag.Replace("/", string.Empty).Replace("]", string.Empty);

                    // If tag contains attributes kill them for checking
                    if (bbCode.Contains("="))
                    {
                        bbCode = bbCode.Remove(bbCode.IndexOf("=", StringComparison.Ordinal));
                    }

                    if (codes.Exists(allowedTag => bbCode.ToLower().Equals(allowedTag.ToLower())))
                    {
                        return;
                    }

                    if (!forbiddenTagList.Contains(bbCode))
                    {
                        forbiddenTagList.Add(bbCode);
                    }
                });

        return forbiddenTagList.ToDelimitedString(",");
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
    /// <param name="delimiter">
    /// The delimiter in a tags list.
    /// </param>
    /// <returns>
    /// A message string.
    /// </returns>
    public string CheckHtmlTags(string checkString, string acceptedTags, char delimiter)
    {
        var detectedHtmlTag = this.HtmlTagForbiddenDetector(checkString, acceptedTags, delimiter);

        if (detectedHtmlTag.IsSet() && detectedHtmlTag != "ALL")
        {
            return this.Get<ILocalization>().GetTextFormatted(
                "HTMLTAG_WRONG",
                HttpUtility.HtmlEncode(detectedHtmlTag));
        }

        return detectedHtmlTag == "ALL"
                   ? this.Get<ILocalization>().GetText("HTMLTAG_FORBIDDEN")
                   : string.Empty;
    }

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
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
    public string Format(
        int messageId,
        string message,
        MessageFlags messageFlags,
        bool targetBlankOverride,
        DateTime messageLastEdited)
    {
        var boardSettings = this.Get<BoardSettings>();

        var useNoFollow = boardSettings.UseNoFollowLinks;

        // check to see if no follow should be disabled since the message is properly aged
        if (useNoFollow && boardSettings.DisableNoFollowLinksAfterDay > 0)
        {
            var messageAge = messageLastEdited - DateTime.UtcNow;
            if (messageAge.Days > boardSettings.DisableNoFollowLinksAfterDay)
            {
                // disable no follow
                useNoFollow = false;
            }
        }

        // do html damage control
        message = this.RepairHtml(message, messageFlags.IsHtml);

        // get the rules engine from the creator...
        var ruleEngine = this.ProcessReplaceRuleFactory(
            new[] { true /*messageFlags.IsBBCode*/, targetBlankOverride, useNoFollow });

        // see if the rules are already populated...
        if (!ruleEngine.HasRules)
        {
            // get rules for YafBBCode
            this.Get<IBBCode>().CreateBBCodeRules(
                messageId,
                ruleEngine,
                true,
                targetBlankOverride,
                useNoFollow);
        }

        message = this.Get<IBadWordReplace>().Replace(message);

        // process...
        ruleEngine.Process(ref message);

        // Format Emoticons
        message = EmojiOne.ShortNameToUnicode(message, true);

        return message;
    }

    /// <summary>
    /// Format the Syndication Message
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageId">
    /// The Message Id</param>
    /// <param name="messageAuthorId">The Message Author User Id</param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <returns>
    /// The formatted message.
    /// </returns>
    
    public string FormatSyndicationMessage(
        string message,
        int messageId,
        int messageAuthorId,
        MessageFlags messageFlags)
    {
        message =
            $@"{this.Format(0, message, messageFlags, false)}";

        message = message.Replace("<div class=\"innerquote\">", "<blockquote>").Replace("[quote]", "</blockquote>");

        // Remove HIDDEN Text
        message = this.RemoveHiddenBBCodeContent(message);

        message = this.RemoveCustomBBCodes(message);

        var formattedMessage = this.Get<IFormatMessage>().Format(
            messageId,
            message,
            messageFlags);

        formattedMessage = this.Get<IBBCode>().FormatMessageWithCustomBBCode(
            formattedMessage,
            messageFlags,
            messageAuthorId,
            messageId);

        return formattedMessage;
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
    /// <param name="delimiter">
    /// The delimiter string.
    /// </param>
    /// <returns>
    /// Returns a forbidden HTML tag or a null string
    /// </returns>
    
    public string HtmlTagForbiddenDetector(
        string stringToClear,
        string stringToMatch,
        char delimiter)
    {
        var codes = stringToMatch.Split(delimiter);

        var forbiddenTagList = new List<string>();

        MatchAndPerformAction(
            "<.*?>",
            stringToClear,
            (tag, index, len) =>
                {
                    var code = tag.Replace("/", string.Empty).Replace(">", string.Empty);

                    // If tag contains attributes kill them for checking
                    if (code.Contains("=\""))
                    {
                        code = code.Remove(code.IndexOf(" ", StringComparison.Ordinal));
                    }

                    if (codes.Exists(allowedTag => code.ToLower().Equals(allowedTag.ToLower())))
                    {
                        return;
                    }

                    if (!forbiddenTagList.Contains(code))
                    {
                        forbiddenTagList.Add(code);
                    }
                });

        return forbiddenTagList.ToDelimitedString(",");
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
    
    public string RemoveNestedQuotes(string body)
    {
        const RegexOptions RegexOptions =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
        var quote = new Regex(@"\[quote(\=[^\]]*)?\](.*?)\[/quote\]", RegexOptions);

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
    
    public string RemoveHiddenBBCodeContent(string body)
    {
        const RegexOptions RegexOptions =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;

        var hiddenRegex = new Regex(
            @"\[hide-reply\](?<inner>(.|\n)*?)\[\/hide-reply\]|\[hide-reply-thanks\](?<inner>(.|\n)*?)\[\/hide-reply-thanks\]|\[group-hide\](?<inner>(.|\n)*?)\[\/group-hide\]|\[hide\](?<inner>(.|\n)*?)\[\/hide\]|\[group-hide(\=[^\]]*)?\](?<inner>(.|\n)*?)\[\/group-hide\]|\[hide-thanks(\=[^\]]*)?\](?<inner>(.|\n)*?)\[\/hide-thanks\]|\[hide-posts(\=[^\]]*)?\](?<inner>(.|\n)*?)\[\/hide-posts\]",
            RegexOptions);

        var hiddenTagMatch = hiddenRegex.Match(body);

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
    
    public string RemoveCustomBBCodes(string body)
    {
        const RegexOptions RegexOptions =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;

        var spoilerRegex = new Regex(@"\[SPOILER\](?<inner>(.|\n)*?)\[\/SPOILER\]", RegexOptions);

        var spoilerTagMatch = spoilerRegex.Match(body);

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
    public string RepairHtml(string html, bool allowHtml)
    {
        // vzrus: NNTP temporary tweaks to wipe out server hangs. Put it here as it can be in every place.
        // These are '\n\r' things related to multiline regexps.
        var mc1 = Regex.Matches(html, "[^\r]\n[^\r]", RegexOptions.IgnoreCase);
        for (var i = mc1.Count - 1; i >= 0; i--)
        {
            html = html.Insert(mc1[i].Index + 1, " \r");
        }

        var mc2 = Regex.Matches(html, "[^\r]\n\r\n[^\r]", RegexOptions.IgnoreCase);
        for (var i = mc2.Count - 1; i >= 0; i--)
        {
            html = html.Insert(mc2[i].Index + 1, " \r");
        }

        html = !allowHtml
                   ? html//this.HttpServer.HtmlEncode(html)
                   : RemoveHtmlByList(html, this.Get<BoardSettings>().AcceptedHTML.Split(','));

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
        string message,
        IList<string> wordList,
        string prefix,
        string postfix)
    {
        wordList.Where(w => w.Length > 3).ForEach(
            word => MatchAndPerformAction(
                $"({word.ToLower().ToRegExString()})",
                message,
                (inner, index, length) =>
                    {
                        message = message.Insert(index + length, postfix);
                        message = message.Insert(index, prefix);
                    }));

        return message;
    }

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
        string matchRegEx,
        string text,
        Action<string, int, int> matchAction)
    {
        const RegexOptions RegexOptions = RegexOptions.IgnoreCase;

        var matches = Regex.Matches(text, matchRegEx, RegexOptions).Cast<Match>().OrderByDescending(x => x.Index);

        matches.ForEach(
            match =>
                {
                    var inner = text.Substring(match.Index + 1, match.Length - 1).Trim().ToLower();
                    matchAction(inner, match.Index, match.Length);
                });
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
    private static string RemoveHtmlByList(string text, IEnumerable<string> matchList)
    {
        var allowedTags = matchList.ToList();

        MatchAndPerformAction(
            "<.*?>",
            text,
            (tag, index, len) =>
                {
                    if (!HtmlTagHelper.IsValidTag(tag, allowedTags))
                    {
                        text = text.Remove(index, len);
                    }
                });

        return text;
    }
}