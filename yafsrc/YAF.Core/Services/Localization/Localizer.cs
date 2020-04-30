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
    using System.IO;
    using System.Linq;

    using YAF.Core.Context;
    using YAF.Core.Helpers;
    using YAF.Core.Services.Startup;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    /// YAF Localizer
    /// </summary>
    public class Localizer
    {
        #region Constants and Fields

        /// <summary>
        ///   The current page.
        /// </summary>
        private string currentPage = string.Empty;

        /// <summary>
        ///   The file name.
        /// </summary>
        private string fileName = string.Empty;

        /// <summary>
        /// The localization resources.
        /// </summary>
        private LanguageResources localizationLanguageResources;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Localizer" /> class.
        /// </summary>
        public Localizer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Localizer"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public Localizer(string fileName)
        {
            this.fileName = fileName;
            this.LoadFile();
            this.InitCulture();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets LanguageCode.
        /// </summary>
        public CultureInfo CurrentCulture { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get nodes using query.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The Nodes.
        /// </returns>
        public IEnumerable<LanguageResourcesPageResource> GetNodesUsingQuery(
            Func<LanguageResourcesPageResource, bool> predicate)
        {
            var pagePointer =
                this.localizationLanguageResources.page.FirstOrDefault(p => p.name.ToUpper().Equals(this.currentPage));

            return pagePointer != null
                       ? pagePointer.Resource.Where(predicate)
                       : this.localizationLanguageResources.page.SelectMany(p => p.Resource).Where(predicate);
        }

        /// <summary>
        /// The get nodes using query.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The Nodes.
        /// </returns>
        public IEnumerable<LanguageResourcesPageResource> GetCountryNodesUsingQuery(
            Func<LanguageResourcesPageResource, bool> predicate)
        {
            var pagePointer =
                this.localizationLanguageResources.page.FirstOrDefault(p => p.name.ToUpper().Equals(this.currentPage));

            return pagePointer != null
                       ? pagePointer.Resource.Where(predicate)
                       : this.localizationLanguageResources.page.SelectMany(p => p.Resource).Where(predicate);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="localizedText">The localized text.</param>
        public void GetText(string tag, out string localizedText)
        {
            // default the out parameters
            localizedText = string.Empty;

            tag = tag.ToUpper();

            var pagePointer =
                this.localizationLanguageResources.page.FirstOrDefault(p => p.name.Equals(this.currentPage));

            LanguageResourcesPageResource pageResource = null;

            if (pagePointer != null)
            {
                pageResource = pagePointer.Resource.FirstOrDefault(r => r.tag.Equals(tag));
            }

            pageResource ??= this.localizationLanguageResources.page.SelectMany(p => p.Resource)
                .FirstOrDefault(r => r.tag.Equals(tag));

            if (pageResource != null && pageResource.Value.IsSet())
            {
                localizedText = pageResource.Value;
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The get text.
        /// </returns>
        public string GetText(string page, string tag)
        {
            this.SetPage(page);
            this.GetText(tag, out var text);

            return text;
        }

        /// <summary>
        /// The load file.
        /// </summary>
        /// <param name="file">
        /// The file name.
        /// </param>
        public void LoadFile(string file)
        {
            this.fileName = file;
            this.LoadFile();
        }

        /// <summary>
        /// The set page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        public void SetPage(string page)
        {
            if (this.currentPage == page)
            {
                return;
            }

            if (page.IsNotSet())
            {
                page = "DEFAULT";
            }

            this.currentPage = page.ToUpper();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the culture.
        /// </summary>
        private void InitCulture()
        {
            if (!BoardContext.Current.Get<StartupInitializeDb>().Initialized)
            {
                return;
            }

            var langCode = this.CurrentCulture.TwoLetterISOLanguageName;

            // vzrus: Culture code is missing for a user until he saved his profile.
            // First set it to board culture
            try
            {
                if (langCode.Equals(BoardContext.Current.BoardSettings.Culture.Substring(0, 2)))
                {
                    this.CurrentCulture = new CultureInfo(BoardContext.Current.BoardSettings.Culture);
                }
            }
            catch (Exception)
            {
                this.CurrentCulture = new CultureInfo(BoardContext.Current.BoardSettings.Culture);
            }

            var cultureUser = BoardContext.Current.CultureUser;

            if (!cultureUser.IsSet())
            {
                return;
            }

            if (!cultureUser.Trim().Substring(0, 2).Equals(langCode))
            {
                return;
            }

            try
            {
                this.CurrentCulture =
                    new CultureInfo(
                        cultureUser.Trim().Length > 5 ? cultureUser.Trim().Substring(0, 2) : cultureUser.Trim());
            }
            catch (Exception)
            {
                this.CurrentCulture = new CultureInfo(BoardContext.Current.BoardSettings.Culture);
            }
        }

        /// <summary>
        /// The load file.
        /// </summary>
        private void LoadFile()
        {
            if (this.fileName == string.Empty || !File.Exists(this.fileName))
            {
                throw new ApplicationException($"Invalid language file {this.fileName}");
            }

            this.localizationLanguageResources = new LoadSerializedXmlFile<LanguageResources>().FromFile(
                this.fileName,
                $"LOCALIZATIONFILE{this.fileName}",
                r =>
                    {
                        // transform the page and tag name ToUpper...
                        r.page.ForEach(p => p.name = p.name.ToUpper());
                        r.page.ForEach(p => p.Resource.ForEach(i => i.tag = i.tag.ToUpper()));
                    });

            var userLanguageCode = this.localizationLanguageResources.code.IsSet()
                                       ? this.localizationLanguageResources.code.Trim()
                                       : "en-US";

            if (userLanguageCode.Length > 5)
            {
                userLanguageCode = this.localizationLanguageResources.code.Trim().Substring(0, 2);
            }

            this.CurrentCulture = new CultureInfo(userLanguageCode);
        }

        #endregion
    }
}