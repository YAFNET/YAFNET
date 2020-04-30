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

namespace YAF.Core.Services.Localization
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The YAF localization.
    /// </summary>
    public class Localization : ILocalization
    {
        #region Constants and Fields

        /// <summary>
        /// The begin no parse regex.
        /// </summary>
        private static readonly Regex BeginNoParseRegex = new Regex(@"(?<!\[noparse\])(?<inner>\[b\])", RegexOptions.Compiled);

        /// <summary>
        /// The end no parse regex.
        /// </summary>
        private static readonly Regex EndNoParseRegex = new Regex(@"(?<inner>\[/b\])(?!\[/noparse\])", RegexOptions.Compiled);

        /// <summary>
        ///   The _culture.
        /// </summary>
        private CultureInfo culture;

        /// <summary>
        ///   The _default locale.
        /// </summary>
        private Localizer defaultLocale;

        /// <summary>
        ///   The localizer.
        /// </summary>
        private Localizer localizer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Localization"/> class. 
        ///   Initializes a new instance of the <see cref="YAF.Core"/> class.
        /// </summary>
        public Localization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Localization"/> class. 
        /// Initializes a new instance of the <see cref="YAF.Core"/> class.
        /// </summary>
        /// <param name="transPage">
        /// The trans page.
        /// </param>
        public Localization([NotNull] string transPage)
            : this()
        {
            this.TransPage = transPage;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Culture.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                if (this.culture != null)
                {
                    return this.culture;
                }

                if (this.localizer != null)
                {
                    return CultureInfo.CurrentCulture;
                }

                this.culture = this.LoadTranslation();

                return this.culture;

                // fall back to current culture if there is some error
            }
        }

        /// <summary>
        ///   Gets LanguageCode.
        /// </summary>
        [NotNull]
        public string LanguageCode =>
            this.localizer != null
                ? this.localizer.CurrentCulture.TwoLetterISOLanguageName
                : this.LoadTranslation().TwoLetterISOLanguageName;

        /// <summary>
        ///   Gets LanguageFileName.
        /// </summary>
        public string LanguageFileName { get; private set; }

        /// <summary>
        ///   Gets or sets What section of the xml is used to translate this page
        /// </summary>
        public string TransPage { get; set; }

        /// <summary>
        ///   Gets a value indicating whether TranslationLoaded.
        /// </summary>
        public bool TranslationLoaded => this.localizer != null;

        #endregion

        #region Implemented Interfaces

        #region ILocalization

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
        [NotNull]
        public string FormatDateTime([NotNull] string format, DateTime date)
        {
            return this.Culture.IsNeutralCulture ? date.ToString(format) : date.ToString(format, this.Culture);
        }

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
        public string FormatString([NotNull] string format, [NotNull] params object[] args)
        {
            return this.Culture.IsNeutralCulture ? string.Format(format, args) : string.Format(this.Culture, format, args);
        }

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
        public IEnumerable<LanguageResourcesPageResource> GetNodesUsingQuery(
            [NotNull] string page,
            [NotNull] Func<LanguageResourcesPageResource, bool> predicate)
        {
            this.LoadTranslation();

            this.localizer.SetPage(page);
            return this.localizer.GetNodesUsingQuery(predicate);
        }

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
        public IEnumerable<LanguageResourcesPageResource> GetCountryNodesUsingQuery(
            [NotNull] string page,
            [NotNull] Func<LanguageResourcesPageResource, bool> predicate)
        {
            this.LoadTranslation();

            this.localizer.SetPage(page);
            return this.localizer.GetCountryNodesUsingQuery(predicate);
        }

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
        public IEnumerable<LanguageResourcesPageResource> GetRegionNodesUsingQuery(
            [NotNull] string page,
            [NotNull] Func<LanguageResourcesPageResource, bool> predicate)
        {
            this.LoadTranslation();

            this.localizer.SetPage(page);
            return this.localizer.GetCountryNodesUsingQuery(predicate);
        }

        /// <summary>
        /// Gets the Localized Text
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetText([NotNull] string text)
        {
            CodeContracts.VerifyNotNull(text, "text");

            return this.GetText(this.TransPage, text);
        }

        /// <summary>
        /// Gets the attribute encoded text. 
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetAttributeText([NotNull] string text)
        {
            return HttpUtility.HtmlAttributeEncode(this.GetText(text));
        }

        /// <summary>
        /// Gets the localized text
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetText([NotNull] string page, [NotNull] string tag)
        {
            var localizedText = this.GetLocalizedTextInternal(page, tag);

            if (localizedText == null)
            {
#if !DEBUG
                string filename;

                if (BoardContext.Current.PageIsNull() || BoardContext.Current.LanguageFile.IsNotSet()
                    || !BoardContext.Current.Get<BoardSettings>().AllowUserLanguage)
                {
                    filename = BoardContext.Current.IsGuest
                                   ? UserHelper.GetGuestUserLanguageFile()
                                   : BoardContext.Current.Get<BoardSettings>().Language;
                }
                else
                {
                    filename = BoardContext.Current.IsGuest
                                   ? UserHelper.GetGuestUserLanguageFile()
                                   : BoardContext.Current.LanguageFile;

                }

                if (filename.IsNotSet())
                {
                    filename = "english.xml";
                }

                HttpContext.Current.Cache.Remove($"Localizer.{filename}");
#endif
                BoardContext.Current.Get<ILogger>()
                    .Log(
                        BoardContext.Current.PageUserID,
                        $"{page.ToLower()}.ascx",
                        $"Missing Translation For {page.ToUpper()}.{tag.ToUpper()}");

                return $"[{page.ToUpper()}.{tag.ToUpper()}]";
            }

            localizedText = BeginNoParseRegex.Replace(localizedText, "<strong>");
            localizedText = EndNoParseRegex.Replace(localizedText, "</strong>");

            localizedText = localizedText.Replace("[noparse]", string.Empty);
            localizedText = localizedText.Replace("[/noparse]", string.Empty);

            return localizedText;
        }

        /// <summary>
        /// Gets the localized text
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetAttributeText([NotNull] string page, [NotNull] string tag)
        {
            return HttpUtility.HtmlAttributeEncode(this.GetText(page, tag));
        }

        /// <summary>
        /// The get text, with a Specific Language.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="languageFile">
        /// The Language file
        /// </param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetText([NotNull] string page, [NotNull] string tag, [NotNull] string languageFile)
        {
            string localizedText;

            if (languageFile.IsSet())
            {
                var localization = new Localization();
                localization.LoadTranslation(languageFile);
                localizedText = localization.GetText(page, tag);
            }
            else
            {
                localizedText = this.GetLocalizedTextInternal(page, tag);
            }

            if (localizedText == null)
            {
#if !DEBUG
                string filename;

                if (languageFile.IsSet())
                {
                    filename = languageFile;
                }
                else
                {
                    if (BoardContext.Current.PageIsNull() || BoardContext.Current.LanguageFile.IsNotSet()
                        || !BoardContext.Current.Get<BoardSettings>().AllowUserLanguage)
                    {
                        filename = BoardContext.Current.IsGuest
                                       ? UserHelper.GetGuestUserLanguageFile()
                                       : BoardContext.Current.Get<BoardSettings>().Language;
                    }
                    else
                    {
                        filename = BoardContext.Current.IsGuest
                                       ? UserHelper.GetGuestUserLanguageFile()
                                       : BoardContext.Current.LanguageFile;
                    }
                }


                if (filename == string.Empty)
                {
                    filename = "english.xml";
                }

                HttpContext.Current.Cache.Remove($"Localizer.{filename}");
#endif
                BoardContext.Current.Get<ILogger>()
                    .Log(
                        BoardContext.Current.PageUserID,
                        $"{page.ToLower()}.ascx",
                        $"Missing Translation For {page.ToUpper()}.{tag.ToUpper()}");

                return $"[{page.ToUpper()}.{tag.ToUpper()}]";
            }

            localizedText = localizedText.Replace("[b]", "<b>");
            localizedText = localizedText.Replace("[/b]", "</b>");

            return localizedText;
        }

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
        /// Returns value if text exists.
        /// </returns>
        public bool GetTextExists([NotNull] string page, [NotNull] string tag)
        {
            return this.GetLocalizedTextInternal(page, tag).IsSet();
        }
        
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
        /// Returns the Formatted Text
        /// </returns>
        public string GetTextFormatted([NotNull] string text, [NotNull] params object[] args)
        {
            CodeContracts.VerifyNotNull(text, "text");
            CodeContracts.VerifyNotNull(args, "args");

            var localizedText = this.GetText(this.TransPage, text);

            var arraySize = Math.Max(args.Length, 10);
            var copiedArgs = new object[arraySize];
            args.CopyTo(copiedArgs, 0);

            for (var arrayIndex = args.Length; arrayIndex < arraySize; arrayIndex++)
            {
                copiedArgs[arrayIndex] =
                    $"[INVALID: {(this.TransPage.IsNotSet() ? "NULL" : this.TransPage.ToUpper())}.{text.ToUpper()} -- EMPTY PARAM #{arrayIndex}]";
            }

            // run format command...
            localizedText = string.Format(localizedText, copiedArgs);

            return localizedText;
        }

        /// <summary>
        /// The load translation.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// Returns the translation Culture Info.
        /// </returns>
        public CultureInfo LoadTranslation([NotNull] string fileName)
        {
            CodeContracts.VerifyNotNull(fileName, "fileName");

            if (this.localizer != null)
            {
                return this.localizer.CurrentCulture;
            }

#if !DEBUG
            if (this.localizer == null && BoardContext.Current.Get<HttpContextBase>().Cache[$"Localizer.{fileName}"] != null) this.localizer = (Localizer)BoardContext.Current.Get<HttpContextBase>().Cache[
                $"Localizer.{fileName}"];
#endif
            if (this.localizer == null)
            {
                this.localizer =
                    new Localizer(
                        BoardContext.Current.Get<HttpContextBase>().Server.MapPath($"{BoardInfo.ForumServerFileRoot}languages/{fileName}"));

#if !DEBUG
                BoardContext.Current.Get<HttpContextBase>().Cache[$"Localizer.{fileName}"] = this.localizer;
#endif
            }

            // If not using default language load that too
            if (fileName.ToLower() != "english.xml")
            {
#if !DEBUG
                if (this.defaultLocale == null && BoardContext.Current.Get<HttpContextBase>().Cache["DefaultLocale"] != null) this.defaultLocale = (Localizer)BoardContext.Current.Get<HttpContextBase>().Cache["DefaultLocale"];
#endif

                if (this.defaultLocale == null)
                {
                    this.defaultLocale =
                        new Localizer(
                            BoardContext.Current.Get<HttpContextBase>().Server.MapPath(
                                $"{BoardInfo.ForumServerFileRoot}languages/english.xml"));
#if !DEBUG
                    BoardContext.Current.Get<HttpContextBase>().Cache["DefaultLocale"] = this.defaultLocale;
#endif
                }
            }

            try
            {
                // try to load culture info defined in localization file
                this.culture = this.localizer.CurrentCulture;
            }
            catch
            {
                // if it's wrong, fall back to current culture
                this.culture = CultureInfo.CurrentCulture;
            }

            this.LanguageFileName = fileName.ToLower();

            return this.culture;
        }

        /// <summary>
        /// The load translation.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="CultureInfo"/>.
        /// </returns>
        public CultureInfo LoadTranslation(string fileName, HttpContext context)
        {
            CodeContracts.VerifyNotNull(fileName, "fileName");

            if (this.localizer != null)
            {
                return this.localizer.CurrentCulture;
            }

#if !DEBUG
            if (this.localizer == null && context.Cache[$"Localizer.{fileName}"] != null) this.localizer = (Localizer)context.Cache[
                $"Localizer.{fileName}"];
#endif
            if (this.localizer == null)
            {
                this.localizer =
                    new Localizer(
                        context.Server.MapPath($"{BoardInfo.ForumServerFileRoot}languages/{fileName}"));

#if !DEBUG
                context.Cache[$"Localizer.{fileName}"] = this.localizer;
#endif
            }

            // If not using default language load that too
            if (fileName.ToLower() != "english.xml")
            {
#if !DEBUG
                if (this.defaultLocale == null && context.Cache["DefaultLocale"] != null) this.defaultLocale = (Localizer)context.Cache["DefaultLocale"];
#endif

                if (this.defaultLocale == null)
                {
                    this.defaultLocale =
                        new Localizer(
                            context.Server.MapPath(
                                $"{BoardInfo.ForumServerFileRoot}languages/english.xml"));
#if !DEBUG
                    context.Cache["DefaultLocale"] = this.defaultLocale;
#endif
                }
            }

            try
            {
                // try to load culture info defined in localization file
                this.culture = this.localizer.CurrentCulture;
            }
            catch
            {
                // if it's wrong, fall back to current culture
                this.culture = CultureInfo.CurrentCulture;
            }

            this.LanguageFileName = fileName.ToLower();

            return this.culture;
        }

        /// <summary>
        /// Loads the Translation
        /// </summary>
        /// <returns>
        /// The load translation.
        /// </returns>
        public CultureInfo LoadTranslation()
        {
            if (this.localizer != null)
            {
                return this.localizer.CurrentCulture;
            }

            string filename;

            if (BoardContext.Current.PageIsNull() || BoardContext.Current.Page["LanguageFile"] == null
                || !BoardContext.Current.Get<BoardSettings>().AllowUserLanguage)
            {
                filename = BoardContext.Current.IsGuest
                               ? UserHelper.GetGuestUserLanguageFile()
                               : BoardContext.Current.Get<BoardSettings>().Language;
            }
            else
            {
                filename = BoardContext.Current.IsGuest
                               ? UserHelper.GetGuestUserLanguageFile()
                               : BoardContext.Current.LanguageFile;
            }

            filename ??= "english.xml";

            return this.LoadTranslation(filename);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The get localized text internal.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// Returns the localized text internal.
        /// </returns>
        protected string GetLocalizedTextInternal([NotNull] string page, [NotNull] string tag)
        {
            this.LoadTranslation();

            this.localizer.SetPage(page);
            this.localizer.GetText(tag, out var localizedText);

            // If not default language, try to use that instead
            if (localizedText != null || this.defaultLocale == null)
            {
                return localizedText;
            }

            this.defaultLocale.SetPage(page);
            this.defaultLocale.GetText(tag, out localizedText);

            if (localizedText != null)
            {
                localizedText = $"{'['}{localizedText}{']'}";
            }

            return localizedText;
        }

        #endregion
    }
}