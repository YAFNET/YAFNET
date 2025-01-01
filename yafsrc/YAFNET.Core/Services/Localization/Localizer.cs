/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Services.Localization;

using System;
using System.Collections.Generic;
using System.IO;

using YAF.Types.Objects.Language;

/// <summary>
/// YAF Localizer
/// </summary>
public class Localizer
{
    /// <summary>
    ///   The current page.
    /// </summary>
    private string currentPage = string.Empty;

    /// <summary>
    ///   The file name.
    /// </summary>
    private string fileName;

    /// <summary>
    /// The localization resources.
    /// </summary>
    private LanguageResource localizationLanguageResources;

    /// <summary>
    /// Initializes a new instance of the <see cref="Localizer"/> class.
    /// </summary>
    /// <param name="fileName">
    /// The file name.
    /// </param>
    /// <param name="initCulture"></param>
    public Localizer(string fileName, bool initCulture)
    {
        this.fileName = fileName.Replace(".xml", ".json");
        this.LoadFile();

        if (initCulture)
        {
            this.InitCulture();
        }
    }

    /// <summary>
    ///   Gets LanguageCode.
    /// </summary>
    public CultureInfo CurrentCulture { get; private set; }

    /// <summary>
    /// The get nodes using query.
    /// </summary>
    /// <param name="predicate">
    /// The predicate.
    /// </param>
    /// <returns>
    /// The Nodes.
    /// </returns>
    public IEnumerable<Resource> GetNodesUsingQuery(
        Func<Resource, bool> predicate)
    {
        var pagePointer =
            this.localizationLanguageResources.Resources.Page.Find(p => p.Name.ToUpper().Equals(this.currentPage));

        return pagePointer != null
                   ? pagePointer.Resource.Where(predicate)
                   : this.localizationLanguageResources.Resources.Page.SelectMany(p => p.Resource).Where(predicate);
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

        tag = tag.ToUpper(CultureInfo.InvariantCulture);

        var pagePointer =
            this.localizationLanguageResources.Resources.Page.Find(p => p.Name.Equals(this.currentPage));

        Resource pageResource = null;

        if (pagePointer != null)
        {
            pageResource = pagePointer.Resource.Find(r => r.Tag.Equals(tag));
        }

        pageResource ??= this.localizationLanguageResources.Resources.Page.SelectMany(p => p.Resource)
            .FirstOrDefault(r => r.Tag.Equals(tag));

        if (pageResource != null && pageResource.Text.IsSet())
        {
            localizedText = pageResource.Text;
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

    /// <summary>
    /// Initializes the culture.
    /// </summary>
    private void InitCulture()
    {
        var langCode = this.CurrentCulture.TwoLetterISOLanguageName;

        try
        {
            if (langCode.Equals(BoardContext.Current.BoardSettings.Culture[..2]))
            {
                this.CurrentCulture = new CultureInfo(BoardContext.Current.BoardSettings.Culture);
            }
        }
        catch (Exception)
        {
            this.CurrentCulture = new CultureInfo(BoardContext.Current.BoardSettings.Culture);
        }

        var cultureUser = BoardContext.Current.PageUser.Culture;

        if (!cultureUser.IsSet())
        {
            return;
        }

        if (!cultureUser.Trim()[..2].Equals(langCode))
        {
            return;
        }

        try
        {
            this.CurrentCulture =
                new CultureInfo(
                    cultureUser.Trim().Length > 5 ? cultureUser.Trim()[..2] : cultureUser.Trim());
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
            throw new ArgumentException($"Invalid language file {this.fileName}");
        }

        var json = BoardContext.Current.Get<ILocalization>().LoadLanguageFile(this.fileName);

        this.localizationLanguageResources = json;

        var userLanguageCode = this.localizationLanguageResources.Resources.Code.IsSet()
                                   ? this.localizationLanguageResources.Resources.Code.Trim()
                                   : "en-US";

        if (userLanguageCode.Length > 5)
        {
            userLanguageCode = this.localizationLanguageResources.Resources.Code.Trim()[..2];
        }

        this.CurrentCulture = new CultureInfo(userLanguageCode);
    }
}