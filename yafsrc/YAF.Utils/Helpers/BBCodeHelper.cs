/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Utils.Helpers
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using YAF.Types.Objects;

    /// <summary>
    /// The bb code helper.
    /// </summary>
    public static class BBCodeHelper
    {
        /// <summary>
        /// The find user quoting.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<UserSimple> FindUserQuoting(string text)
        {
            var mentions = Regex.Matches(
                text,
                @"\[quote\=(?<user>.+?);(?<messageId>.+?)\](?<inner>.+?)\[\/quote\]",
                RegexOptions.Singleline);

            var users = new List<UserSimple>();

            foreach (Match match in mentions)
            {
                users.Add(new UserSimple { UserName = match.Groups["user"].Value });
            }

            return users;
        }

        /// <summary>
        /// Find all User mentions in the text
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<UserSimple> FindMentions(string text)
        {
            var mentions = Regex.Matches(text, @"@\[userlink\](?<inner>.+?)\[\/userlink\]", RegexOptions.IgnoreCase);

            var users = new List<UserSimple>();

            foreach (Match match in mentions)
            {
                users.Add(new UserSimple { UserName = match.Groups["inner"].Value });
            }

            return users;
        }


        /// <summary>
        /// The strip bb code.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The strip bb code.
        /// </returns>
        public static string StripBBCode(string text)
        {
            return Regex.Replace(text, @"\[(.|\n)*?\]", string.Empty);
        }

        /// <summary>
        /// Strip Quote BB Code Quotes including the quoted text
        /// </summary>
        /// <param name="text">Text to check
        /// </param>
        /// <returns>The Cleaned Text
        /// </returns>
        public static string StripBBCodeQuotes(string text)
        {
            return Regex.Replace(text, @"\[quote[^\]]*](.|\n)*?\[/quote\]", string.Empty, RegexOptions.Multiline);
        }

        /// <summary>
        /// Strip BB Code Urls
        /// </summary>
        /// <param name="text">Text to check
        /// </param>
        /// <returns>The Cleaned Text
        /// </returns>
        public static string StripBBCodeUrls(string text)
        {
            return Regex.Replace(text, @"\[url[^\]]*](.|\n)*?\[/url\]", string.Empty, RegexOptions.Singleline);
        }
    }
}