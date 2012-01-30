/* Yet Another Forum.net
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
  #region Using

  using System;
  using System.Web.UI;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The ibb code.
  /// </summary>
  public interface IBBCode
  {
    #region Public Methods

    /// <summary>
    /// Adds smiles replacement rules to the collection from the DB
    /// </summary>
    /// <param name="rules">
    /// The rules.
    /// </param>
    void AddSmiles([NotNull] IProcessReplaceRules rules);

    /// <summary>
    /// Converts a message containing YafBBCode to HTML appropriate for editing in a rich text editor.
    /// </summary>
    /// <remarks>
    /// YafBBCode quotes are not converted to HTML.  "[quote]...[/quote]" will remain in the string 
    ///   returned, as to appear in plain text in rich text editors.
    /// </remarks>
    /// <param name="message">
    /// String containing the body of the message to convert
    /// </param>
    /// <returns>
    /// The converted text
    /// </returns>
    string ConvertBBCodeToHtmlForEdit([NotNull] string message);

    /// <summary>
    /// Converts a message containing HTML to YafBBCode for editing in a rich bbcode editor.
    /// </summary>
    /// <param name="message">
    /// String containing the body of the message to convert
    /// </param>
    /// <returns>
    /// The converted text
    /// </returns>
    string ConvertHtmltoBBCodeForEdit([NotNull] string message);

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
    void CreateBBCodeRules(
        [NotNull] IProcessReplaceRules ruleEngine,
        bool doFormatting,
        bool targetBlankOverride, 
        bool useNoFollow, 
        bool convertBBQuotes);

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
    void CreateHtmlRules(
        [NotNull] IProcessReplaceRules ruleEngine,
        bool doFormatting,
        bool convertBBQuotes);

    /// <summary>
    /// Handles localization for a Custom YafBBCode Elements using
    ///   the code [localization=tag]default[/localization]
    /// </summary>
    /// <param name="strToLocalize">
    /// </param>
    /// <returns>
    /// The localize custom bb code element.
    /// </returns>
    string LocalizeCustomBBCodeElement([NotNull] string strToLocalize);

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
    string MakeHtml([NotNull] string inputString, bool doFormatting, bool targetBlankOverride);

    /// <summary>
    /// Helper function that dandles registering "custom bbcode" javascript (if there is any)
    ///   for all the custom YafBBCode.
    /// </summary>
    /// <param name="currentPage">
    /// The current Page.
    /// </param>
    /// <param name="currentType">
    /// The current Type.
    /// </param>
    void RegisterCustomBBCodePageElements([NotNull] Page currentPage, [NotNull] Type currentType);

    /// <summary>
    /// Helper function that dandles registering "custom bbcode" javascript (if there is any)
    ///   for all the custom YafBBCode. Defining editorID make the system also show "editor js" (if any).
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
    void RegisterCustomBBCodePageElements([NotNull] Page currentPage, [NotNull] Type currentType, [NotNull] string editorID);

    #endregion
  }
}