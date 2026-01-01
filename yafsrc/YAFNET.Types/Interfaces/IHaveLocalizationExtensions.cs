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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Types.Interfaces;

/// <summary>
/// The I Have localization extensions.
/// </summary>
public static class IHaveLocalizationExtensions
{
    /// <param name="haveLocalization">
    /// The have localization.
    /// </param>
    extension(IHaveLocalization haveLocalization)
    {
        /// <summary>
        /// Gets a text localization using the page and tag name.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetText(string page,
            string tag)
        {
            return haveLocalization.Localization.GetText(page, tag);
        }

        /// <summary>
        /// Gets a text localization.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetText(string tag)
        {
            return haveLocalization.Localization.GetText(tag);
        }

        /// <summary>
        /// Gets a text localization using formatting.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The get text formatted.
        /// </returns>
        public string GetTextFormatted(string tag,
            params object[] args)
        {
            return haveLocalization.Localization.GetTextFormatted(tag, args);
        }
    }
}