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
namespace YAF.Types.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using YAF.Types.Objects;

    /// <summary>
    /// The Localization interface.
    /// </summary>
    public interface ILocalization
    {
        /// <summary>
        ///   Gets Culture.
        /// </summary>
        CultureInfo Culture { get; }

        /// <summary>
        ///   Gets LanguageCode.
        /// </summary>
        string LanguageCode { get; }

        /// <summary>
        ///   Gets LanguageFileName.
        /// </summary>
        string LanguageFileName { get; }

        /// <summary>
        ///   Gets or sets What section of the xml is used to translate this page
        /// </summary>
        string TransPage { get; set; }

        /// <summary>
        ///   Gets a value indicating whether TranslationLoaded.
        /// </summary>
        bool TranslationLoaded { get; }

        /// <summary>
        /// Formats date using given formatting string and current culture.
        /// </summary>
        /// <param name="format">
        /// Format string.
        /// </param>
        /// <param name="date">
        /// Date to format.
        /// </param>
        /// <returns>
        /// Formatted string.
        /// </returns>
        /// <remarks>
        /// If current localization culture is neutral, it's not used in formatting.
        /// </remarks>
        string FormatDateTime(string format, DateTime date);

        /// <summary>
        /// Formats string using current culture.
        /// </summary>
        /// <param name="format">
        /// Format string.
        /// </param>
        /// <param name="args">
        /// Parameters used in format string.
        /// </param>
        /// <returns>
        /// Formatted string.
        /// </returns>
        /// <remarks>
        /// If current localization culture is neutral, it's not used in formatting.
        /// </remarks>
        string FormatString(string format, params object[] args);

        /// <summary>
        /// The get nodes using query.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The Nodes
        /// </returns>
        IEnumerable<LanguageResourcesPageResource> GetNodesUsingQuery(
          string page, Func<LanguageResourcesPageResource, bool> predicate);

        /// <summary>
        /// The get country nodes using query.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The Nodes
        /// </returns>
        IEnumerable<LanguageResourcesPageResource> GetCountryNodesUsingQuery(
          string page, Func<LanguageResourcesPageResource, bool> predicate);

        /// <summary>
        /// The get region nodes using query.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The Nodes
        /// </returns>
        IEnumerable<LanguageResourcesPageResource> GetRegionNodesUsingQuery(
          string page, Func<LanguageResourcesPageResource, bool> predicate);

        /// <summary>
        /// Gets the Text
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetText([NotNull] string text);

        /// <summary>
        /// Gets the attribute encoded text. 
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetAttributeText([NotNull] string text);

        /// <summary>
        /// Gets the Text
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
        string GetText(string page, string tag);

        /// <summary>
        /// Gets the Text
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
        string GetAttributeText(string page, string tag);

        /// <summary>
        /// Gets text with a language file.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="languageFile">
        /// The language File.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetText(string page, string tag, string languageFile);

        /// <summary>
        /// Checks if the Text exists
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The get text exists.
        /// </returns>
        bool GetTextExists(string page, string tag);

        /// <summary>
        /// Formats a localized string -- but verifies the parameter count matches
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The get text formatted.
        /// </returns>
        string GetTextFormatted([NotNull] string text, [NotNull] params object[] args);

        /// <summary>
        /// Loads the translation.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="CultureInfo"/>.
        /// </returns>
        CultureInfo LoadTranslation([NotNull] string fileName);

        /// <summary>
        /// Loads the translation.
        /// </summary>
        /// <returns>
        /// The <see cref="CultureInfo"/>.
        /// </returns>
        CultureInfo LoadTranslation();
    }
}