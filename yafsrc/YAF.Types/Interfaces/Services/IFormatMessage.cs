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
namespace YAF.Types.Interfaces.Services;

using System.Collections.Generic;

/// <summary>
/// The FormatMessage Interface
/// </summary>
public interface IFormatMessage
{
    /// <summary>
    /// The method to detect a forbidden BBCode tag from delimited by 'delimiter' list 
    ///   'stringToMatch'
    /// </summary>
    /// <param name="stringToClear">
    /// Input string
    /// </param>
    /// <param name="stringToMatch">
    /// String with delimiter
    /// </param>
    /// <param name="delimiter">
    /// The delimiter
    /// </param>
    /// <returns>
    /// Returns a string containing a forbidden BBCode or a null string
    /// </returns>
    
    string BBCodeForbiddenDetector(string stringToClear, string stringToMatch, char delimiter);

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
    string CheckHtmlTags(string checkString, string acceptedTags, char delimiter);

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
    string Format(int messageId, string message, MessageFlags messageFlags, bool targetBlankOverride, DateTime messageLastEdited);

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
        MessageFlags messageFlags);

    /// <summary>
    /// Removes nested quotes from the given message body.
    /// </summary>
    /// <param name="body">
    /// Message body test to remove nested quotes from
    /// </param>
    /// <returns>
    /// A version of <paramref name="body"/> that contains no nested quotes.
    /// </returns>
    
    string RemoveNestedQuotes(string body);

    /// <summary>
    /// Removes BBCode Posted Hidden Content
    /// </summary>
    /// <param name="body">
    /// Message body to remove the hidden content from
    /// </param>
    /// <returns>
    /// The Cleaned body.
    /// </returns>
    
    string RemoveHiddenBBCodeContent(string body);

    /// <summary>
    /// Removes Custom BBCodes
    /// </summary>
    /// <param name="body">
    /// Message body to remove the hidden content from
    /// </param>
    /// <returns>
    /// The Cleaned body.
    /// </returns>
    
    string RemoveCustomBBCodes(string body);

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
    string RepairHtml(string html, bool allowHtml);

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
        string message, IList<string> wordList, string prefix, string postfix);
}