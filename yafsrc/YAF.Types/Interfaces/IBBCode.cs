/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using YAF.Types;
    using YAF.Types.Flags;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The BBCode Interface.
    /// </summary>
    public interface IBBCode
    {
        #region Public Methods

        /// <summary>
        /// Formats the message with Custom BBCodes
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="flags">
        /// The Message flags.
        /// </param>
        /// <param name="displayUserId">
        /// The display user id.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// Returns the formatted Message.
        /// </returns>
        string FormatMessageWithCustomBBCode([NotNull] string message, [NotNull] MessageFlags flags, int? displayUserId, int? messageId);

        /// <summary>
        /// Converts a message containing BBCode to HTML appropriate for editing in a rich text editor.
        /// </summary>
        /// <remarks>
        /// quotes are not converted to HTML.  "[quote]...[/quote]" will remain in the string 
        ///   returned, as to appear in plain text in rich text editors.
        /// </remarks>
        /// <param name="message">
        /// String containing the body of the message to convert
        /// </param>
        /// <returns>
        /// The converted text
        /// </returns>
        //[Obsolete]
        //string ConvertBBCodeToHtmlForEdit([NotNull] string message);

        /// <summary>
        /// Converts a message containing HTML to BBCode for editing.
        /// </summary>
        /// <param name="message">
        /// String containing the body of the message to convert
        /// </param>
        /// <returns>
        /// The converted text
        /// </returns>
        string ConvertHtmlToBBCodeForEdit([NotNull] string message);

        /// <summary>
        /// Creates the rules that convert BBCode to HTML
        /// </summary>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
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
        /// <param name="isEditMode">
        /// Indicates if the formatting is for the Editor.
        /// </param>
        void CreateBBCodeRules(
            [NotNull] int messageId,
            [NotNull] IProcessReplaceRules ruleEngine,
            bool doFormatting,
            bool targetBlankOverride,
            bool useNoFollow,
            bool isEditMode = false);

        /// <summary>
        /// Handles localization for a Custom BBCode Elements using
        ///   the code [localization=tag]default[/localization]
        /// </summary>
        /// <param name="strToLocalize">
        /// The string to Localize
        /// </param>
        /// <returns>
        /// The localize custom bb code element.
        /// </returns>
        string LocalizeCustomBBCodeElement([NotNull] string strToLocalize);

        /// <summary>
        /// Converts a string containing BBCode to the equivalent HTML string.
        /// </summary>
        /// <param name="inputString">
        /// Input string containing BBCode to convert to HTML
        /// </param>
        /// <param name="doFormatting">
        /// The do Formatting.
        /// </param>
        /// <param name="targetBlankOverride">
        /// The target Blank Override.
        /// </param>
        /// <returns>
        /// The make html.
        /// </returns>
        string MakeHtml([NotNull] string inputString, bool doFormatting, bool targetBlankOverride);

        /// <summary>
        /// Helper function that dandles registering "Custom BBCode" JavaScript (if there is any)
        ///   for all the custom BBCode.
        /// </summary>
        /// <param name="currentPage">
        /// The current Page.
        /// </param>
        /// <param name="currentType">
        /// The current Type.
        /// </param>
        void RegisterCustomBBCodePageElements([NotNull] Page currentPage, [NotNull] Type currentType);

        /// <summary>
        /// Helper function that dandles registering "Custom BBCode" JavaScript (if there is any)
        ///   for all the custom BBCode. Defining Editor ID make the system also show "Editor JS" (if any).
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
        void RegisterCustomBBCodePageElements(
            [NotNull] Page currentPage,
            [NotNull] Type currentType,
            [NotNull] string editorID);

        /// <summary>
        ///     The get custom bb code.
        /// </summary>
        /// <returns> Returns List with Custom BBCodes </returns>
        IEnumerable<BBCode> GetCustomBBCode();

        #endregion
    }
}