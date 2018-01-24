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

namespace YAF.Core.Services.Localization
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The yaf localization.
    /// </summary>
    public class YafLocalization : ILocalization
    {
        #region Constants and Fields

        /// <summary>
        ///   The _culture.
        /// </summary>
        private CultureInfo culture;

        /// <summary>
        ///   The _default locale.
        /// </summary>
        private Localizer defaultLocale;

        /// <summary>
        ///   The _localizer.
        /// </summary>
        private Localizer localizer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafLocalization"/> class. 
        ///   Initializes a new instance of the <see cref="YAF.Core"/> class.
        /// </summary>
        public YafLocalization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YafLocalization"/> class. 
        /// Initializes a new instance of the <see cref="YAF.Core"/> class.
        /// </summary>
        /// <param name="transPage">
        /// The trans page.
        /// </param>
        public YafLocalization([NotNull] string transPage)
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

                if (this.localizer == null)
                {
                    this.culture = this.LoadTranslation();
                    return this.culture;
                }

                // fall back to current culture if there is some error
                return CultureInfo.CurrentCulture;
            }
        }

        /// <summary>
        ///   Gets LanguageCode.
        /// </summary>
        [NotNull]
        public string LanguageCode
        {
            get
            {
                return this.localizer != null
                           ? this.localizer.CurrentCulture.TwoLetterISOLanguageName
                           : this.LoadTranslation().TwoLetterISOLanguageName;
            }
        }

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
        public bool TranslationLoaded
        {
            get
            {
                return this.localizer != null;
            }
        }

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
            return this.Culture.IsNeutralCulture ? format.FormatWith(args) : string.Format(this.Culture, format, args);
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
        public IEnumerable<LanuageResourcesPageResource> GetNodesUsingQuery(
            [NotNull] string page,
            [NotNull] Func<LanuageResourcesPageResource, bool> predicate)
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
        public IEnumerable<LanuageResourcesPageResource> GetCountryNodesUsingQuery(
            [NotNull] string page,
            [NotNull] Func<LanuageResourcesPageResource, bool> predicate)
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
        public IEnumerable<LanuageResourcesPageResource> GetRegionNodesUsingQuery(
            [NotNull] string page,
            [NotNull] Func<LanuageResourcesPageResource, bool> predicate)
        {
            this.LoadTranslation();

            this.localizer.SetPage(page);
            return this.localizer.GetCountryNodesUsingQuery(predicate);
        }

        /// <summary>
        /// The get text.
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

        private static readonly Regex _rgxBegin = new Regex(@"(?<!\[noparse\])(?<inner>\[b\])", RegexOptions.Compiled);

        private static readonly Regex _rgxEnd = new Regex(@"(?<inner>\[/b\])(?!\[/noparse\])", RegexOptions.Compiled);

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

                if (YafContext.Current.PageIsNull() || YafContext.Current.LanguageFile == string.Empty
                    || YafContext.Current.LanguageFile == string.Empty
                    || !YafContext.Current.Get<YafBoardSettings>().AllowUserLanguage)
                {
                    filename = YafContext.Current.Get<YafBoardSettings>().Language;
                }
                else
                {
                    filename = YafContext.Current.LanguageFile;
                }

                if (filename == string.Empty) filename = "english.xml";

                HttpContext.Current.Cache.Remove("Localizer.{0}".FormatWith(filename));
#endif
                YafContext.Current.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        string.Format("{0}.ascx", page.ToLower()),
                        "Missing Translation For {1}.{0}".FormatWith(tag.ToUpper(), page.ToUpper()));

                return "[{1}.{0}]".FormatWith(tag.ToUpper(), page.ToUpper());
            }

            localizedText = _rgxBegin.Replace(localizedText, "<strong>");
            localizedText = _rgxEnd.Replace(localizedText, "</strong>");

            localizedText = localizedText.Replace("[noparse]", string.Empty);
            localizedText = localizedText.Replace("[/noparse]", string.Empty);

            return localizedText;
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

            if (!string.IsNullOrEmpty(languageFile))
            {
                var localization = new YafLocalization();
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

                if (!string.IsNullOrEmpty(languageFile))
                {
                    filename = languageFile;
                }
                else
                {
                    if (YafContext.Current.PageIsNull() || YafContext.Current.LanguageFile == string.Empty
                        || YafContext.Current.LanguageFile == string.Empty
                        || !YafContext.Current.Get<YafBoardSettings>().AllowUserLanguage)
                    {
                        filename = YafContext.Current.Get<YafBoardSettings>().Language;
                    }
                    else
                    {
                        filename = YafContext.Current.LanguageFile;
                    }
                }


                if (filename == string.Empty) filename = "english.xml";

                HttpContext.Current.Cache.Remove("Localizer." + filename);
#endif
                YafContext.Current.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        page.ToLower() + ".ascx",
                        "Missing Translation For {1}.{0}".FormatWith(tag.ToUpper(), page.ToUpper()),
                        EventLogTypes.Error);

                return "[{1}.{0}]".FormatWith(tag.ToUpper(), page.ToUpper());
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
                copiedArgs[arrayIndex] = "[INVALID: {1}.{0} -- EMPTY PARAM #{2}]".FormatWith(
                    text.ToUpper(),
                    this.TransPage.IsNotSet() ? "NULL" : this.TransPage.ToUpper(),
                    arrayIndex);
            }

            // run format command...
            localizedText = localizedText.FormatWith(copiedArgs);

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
            if (this.localizer == null && HttpContext.Current.Cache["Localizer." + fileName] != null) this.localizer = (Localizer)HttpContext.Current.Cache["Localizer." + fileName];
#endif
            if (this.localizer == null)
            {
                this.localizer =
                    new Localizer(
                        HttpContext.Current.Server.MapPath(
                            "{0}languages/{1}".FormatWith(YafForumInfo.ForumServerFileRoot, fileName)));

#if !DEBUG
                HttpContext.Current.Cache["Localizer." + fileName] = this.localizer;
#endif
            }

            // If not using default language load that too
            if (fileName.ToLower() != "english.xml")
            {
#if !DEBUG
                if (this.defaultLocale == null && HttpContext.Current.Cache["DefaultLocale"] != null) this.defaultLocale = (Localizer)HttpContext.Current.Cache["DefaultLocale"];
#endif

                if (this.defaultLocale == null)
                {
                    this.defaultLocale =
                        new Localizer(
                            HttpContext.Current.Server.MapPath(
                                "{0}languages/english.xml".FormatWith(YafForumInfo.ForumServerFileRoot)));
#if !DEBUG
                    HttpContext.Current.Cache["DefaultLocale"] = this.defaultLocale;
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

            if (YafContext.Current.PageIsNull() || YafContext.Current.Page["LanguageFile"] == null
                || !YafContext.Current.Get<YafBoardSettings>().AllowUserLanguage)
            {
                filename = YafContext.Current.Get<YafBoardSettings>().Language;
            }
            else
            {
                filename = YafContext.Current.LanguageFile;
            }

            if (filename == null)
            {
                filename = "english.xml";
            }

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
            string localizedText;

            this.LoadTranslation();

            this.localizer.SetPage(page);
            this.localizer.GetText(tag, out localizedText);

            // If not default language, try to use that instead
            if (localizedText != null || this.defaultLocale == null)
            {
                return localizedText;
            }

            this.defaultLocale.SetPage(page);
            this.defaultLocale.GetText(tag, out localizedText);
            if (localizedText != null)
            {
                localizedText = '[' + localizedText + ']';
            }

            return localizedText;
        }

        #endregion
    }
}