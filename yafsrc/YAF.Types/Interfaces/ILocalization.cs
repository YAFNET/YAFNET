/* Yet Another Forum.NET
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
  using System.Globalization;

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
    IEnumerable<LanuageResourcesPageResource> GetNodesUsingQuery(
      string page, Func<LanuageResourcesPageResource, bool> predicate);

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
    IEnumerable<LanuageResourcesPageResource> GetCountryNodesUsingQuery(
      string page, Func<LanuageResourcesPageResource, bool> predicate);

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
    IEnumerable<LanuageResourcesPageResource> GetRegionNodesUsingQuery(
      string page, Func<LanuageResourcesPageResource, bool> predicate);

    /// <summary>
    /// The get text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    string GetText([NotNull] string text);

    /// <summary>
    /// The get text.
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
    /// Gets text with a language file.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="tag"></param>
    /// <param name="languageFile"></param>
    /// <returns></returns>
    string GetText(string page, string tag, string languageFile);

    /// <summary>
    /// The get text exists.
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
    /// </param>
    /// <param name="args">
    /// </param>
    /// <returns>
    /// The get text formatted.
    /// </returns>
    string GetTextFormatted([NotNull] string text, [NotNull] params object[] args);

    /// <summary>
    /// The load translation.
    /// </summary>
    /// <param name="fileName">
    /// The file name.
    /// </param>
    /// <returns>
    /// The load translation.
    /// </returns>
    CultureInfo LoadTranslation([NotNull] string fileName);

    /// <summary>
    /// The load translation.
    /// </summary>
    /// <returns>
    /// The load translation.
    /// </returns>
    CultureInfo LoadTranslation();
  }
}