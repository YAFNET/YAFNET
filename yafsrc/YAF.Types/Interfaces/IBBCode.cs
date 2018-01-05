/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
        bool isHtml,
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
    void CreateHtmlRules(
        [NotNull] IProcessReplaceRules ruleEngine);

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
    string MakeHtml([NotNull] string inputString, bool isHtml, bool doFormatting, bool targetBlankOverride);

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