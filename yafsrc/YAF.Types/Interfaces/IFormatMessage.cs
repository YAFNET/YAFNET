﻿/* YetAnotherForum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Types.Interfaces
{
    using System;
    using System.Collections.Generic;

    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Flags;

    /// <summary>
    /// The FormatMessage Interface
    /// </summary>
    public interface IFormatMessage
    {
        /// <summary>
        /// The method to detect a forbidden BBCode tag from delimited by 'delim' list 
        ///   'stringToMatch'
        /// </summary>
        /// <param name="stringToClear">
        /// Input string
        /// </param>
        /// <param name="stringToMatch">
        /// String with delimiter
        /// </param>
        /// <param name="delim">
        /// The delim.
        /// </param>
        /// <returns>
        /// Returns a string containing a forbidden BBCode or a null string
        /// </returns>
        [CanBeNull]
        string BBCodeForbiddenDetector([NotNull] string stringToClear, [NotNull] string stringToMatch, char delim);

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
        /// The delimeter in a tags list.
        /// </param>
        /// <returns>
        /// A message string.
        /// </returns>
        string CheckHtmlTags([NotNull] string checkString, [NotNull] string acceptedTags, char delim);

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
        string FormatMessage([NotNull] string message, [NotNull] MessageFlags messageFlags, bool targetBlankOverride, DateTime messageLastEdited);

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
        string FormatSyndicationMessage([NotNull] string message, [NotNull] MessageFlags messageFlags, bool altItem, int charsToFetch);

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
        MessageCleaned GetCleanedTopicMessage([NotNull] object topicMessage, [NotNull] object topicId);

        /// <summary>
        /// The method to detect a forbidden HTML code from delimited by 'delim' list
        /// </summary>
        /// <param name="stringToClear">
        /// The string To Clear.
        /// </param>
        /// <param name="stringToMatch">
        /// The string To Match.
        /// </param>
        /// <param name="delim">
        /// The delim.
        /// </param>
        /// <returns>
        /// Returns a forbidden HTML tag or a null string
        /// </returns>
        [CanBeNull]
        string HtmlTagForbiddenDetector([NotNull] string stringToClear, [NotNull] string stringToMatch, char delim);

        /// <summary>
        /// Removes nested YafBBCode quotes from the given message body.
        /// </summary>
        /// <param name="body">
        /// Message body test to remove nested quotes from
        /// </param>
        /// <returns>
        /// A version of <paramref name="body"/> that contains no nested quotes.
        /// </returns>
        [NotNull]
        string RemoveNestedQuotes([NotNull] string body);

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
        string RemoveHiddenBBCodeContent([NotNull] string body);

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
        string RemoveCustomBBCodes([NotNull] string body);

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
        string RepairHtml([NotNull] string html, bool allowHtml, [NotNull] IEnumerable<string> matchList);

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
        string RepairHtml([NotNull] string html, bool allowHtml);

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
        string RepairHtmlFeeds([NotNull] string html, bool allowHtml);

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
        string SurroundWordList(
          [NotNull] string message, [NotNull] IList<string> wordList, [NotNull] string prefix, [NotNull] string postfix);
    }
}