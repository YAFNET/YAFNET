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

namespace YAF.Web.HtmlHelpers;

/// <summary>
/// The localized text html helper.
/// </summary>
public static class LocalizedTextHtmlHelper
{
    /// <param name="_">
    /// The html helper.
    /// </param>
    extension(IHtmlHelper _)
    {
        /// <summary>
        /// The localized text.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="IHtmlContent"/>.
        /// </returns>
        public IHtmlContent LocalizedText(string tag)
        {
            var content = new HtmlContentBuilder();

            var text = BoardContext.Current.Get<ILocalization>().GetText(tag);

            return content.AppendHtml(text);
        }

        /// <summary>
        /// The localized text.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="enableBBCode">
        /// The enable bb code.
        /// </param>
        /// <returns>
        /// The <see cref="IHtmlContent"/>.
        /// </returns>
        public IHtmlContent LocalizedText(string page,
            string tag,
            bool enableBBCode = false)
        {
            var content = new HtmlContentBuilder();

            var text = BoardContext.Current.Get<ILocalization>().GetText(page, tag);

            // convert from BBCode to HTML
            if (enableBBCode)
            {
                text = BoardContext.Current.Get<IBBCodeService>().MakeHtml(text, true, true);
            }

            return content.AppendHtml(text);
        }

        /// <summary>
        /// The localized text.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="args">
        /// The parameters
        /// </param>
        /// <returns>
        /// The <see cref="IHtmlContent"/>.
        /// </returns>
        public IHtmlContent LocalizedTextFormatted(string tag,
            params object[] args)
        {
            var content = new HtmlContentBuilder();

            var text = BoardContext.Current.Get<ILocalization>().GetTextFormatted(tag, args);

            return content.AppendHtml(text);
        }
    }
}