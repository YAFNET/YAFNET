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

namespace YAF.Pages.Admin;

using System.Collections.Generic;

using Newtonsoft.Json;

using System.IO;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Objects;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Context;

/// <summary>
/// Administrative Page for the editing of forum properties.
/// </summary>
public class EditLanguageModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditLanguageInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<Translation> Locals { get; set; }

    public List<SelectListItem> PagesList { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditLanguageModel"/> class.
    /// </summary>
    public EditLanguageModel()
        : base("ADMIN_EDITLANGUAGE", ForumPages.Admin_EditLanguage)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_LANGUAGES", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Languages));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITLANGUAGE", "TITLE"), string.Empty);
    }

    /// <summary>
    ///   The translations.
    /// </summary>
    private readonly List<Translation> translations = [];

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(string x)
    {
        this.Input = new EditLanguageInputModel
        {
                                        Pages = "DEFAULT"
                                    };

        if (x.IsNotSet())
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.BindData(x);

        return this.Page();
    }

    /// <summary>
    /// Checks if Resources are translated and handle Size of the Textboxes based on the Content Length
    /// </summary>
    public string LocalizedTextBoxClass(Translation modelLocal)
    {
        return modelLocal.LocalizedResourceText.Equals(modelLocal.OriginalResourceText, StringComparison.OrdinalIgnoreCase)
                                    ? "form-control is-invalid"
                                    : "form-control is-valid";
    }

    /// <summary>
    /// Load Selected Page Resources
    /// </summary>
    public void OnPostLoadPageLocalization(string x)
    {
        this.BindData(x);
    }

    /// <summary>
    /// Save the Updated Xml File
    /// </summary>
    public IActionResult OnPostSave(string x)
    {
        this.SaveLanguageFile(x);

        return this.PageBoardContext.Notify(this.GetText("ADMIN_EDITLANGUAGE", "SAVED_FILE"), MessageTypes.success);
    }

    /// <summary>
    /// Save the Updated Xml File.
    /// </summary>
    private void SaveLanguageFile(string x)
    {
        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        // Get all language files info
        var langPath = Path.Combine(webRootPath, "languages");

        var translationResource = this.Get<ILocalization>().LoadLanguageFile(Path.Combine(langPath, x));

        var pageName = this.Input.Pages;
        var pageTranslations = this.Locals;

        translationResource.Resources.Page.First(x => x.Name == pageName).Resource.ForEach(
            resource =>
            {
                resource.Text = pageTranslations.First(x => x.ResourceName == resource.Tag).LocalizedResourceText;
            });

        var serializer = new JsonSerializer { Formatting = Formatting.Indented };

        using var sw = new StreamWriter(Path.Combine(langPath, x));
        using JsonWriter writer = new JsonTextWriter(sw);
        serializer.Serialize(writer, translationResource);
    }

    /// <summary>
    /// Wraps creation of translation controls.
    /// </summary>
    /// <param name="sourceFile">The source file.</param>
    /// <param name="translationFile">The DST file.</param>
    private void PopulateTranslations(string sourceFile, string translationFile)
    {
        try
        {
            var sourceResource = this.Get<ILocalization>().LoadLanguageFile(sourceFile);
            var translationResource = this.Get<ILocalization>().LoadLanguageFile(translationFile);

            sourceResource.Resources.Page.ForEach(
                page =>
                {
                    var translationPage = translationResource.Resources.Page.Find(
                        p => p.Name.Equals(page.Name, StringComparison.OrdinalIgnoreCase));

                    this.CreatePageResourceHeader(page.Name);

                    var pageResources = page.Resource;

                    pageResources.ForEach(
                        resource =>
                        {
                            var translationResourceText = translationPage.Resource.Find(
                                r => r.Tag.Equals(resource.Tag, StringComparison.OrdinalIgnoreCase));

                            this.CreatePageResourceControl(
                                page.Name,
                                resource.Tag,
                                resource.Text,
                                translationResourceText.Text);
                        });
                });
        }
        catch (Exception exception)
        {
            this.Get<ILogger<EditLanguageModel>>().Log(this.PageBoardContext.PageUserID, this, $"Error loading files. {exception.Message}");
        }
    }

    /// <summary>
    /// Creates controls for column 1 (Resource tag) and column 2 (Resource value).
    /// </summary>
    /// <param name="page">Name of the page.</param>
    /// <param name="resourceName">Name of the resource.</param>
    /// <param name="originalResourceText">The original (english) resource text.</param>
    /// <param name="translationResourceText">The translation resource text.</param>
    private void CreatePageResourceControl(
        string page,
        string resourceName,
        string originalResourceText,
        string translationResourceText)
    {
        var translation = new Translation
        {
            PageName = page,
            ResourceName = resourceName,
            OriginalResourceText = originalResourceText,
            LocalizedResourceText = translationResourceText
        };

        this.translations.Add(translation);
    }

    /// <summary>
    /// Creates a header row in the Resource Page DropDown Header text is page section name in XML file.
    /// </summary>
    /// <param name="name">Name of the page.</param>
    private void CreatePageResourceHeader(string name)
    {
        this.PagesList.Add(new SelectListItem(name, name));
    }

    private void BindData(string x)
    {
        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        // Get all language files info
        var langPath = Path.Combine(webRootPath, "languages");

        this.PagesList = [];

        this.PopulateTranslations(Path.Combine(langPath, "english.json"), Path.Combine(langPath, x));

        this.Locals = this.translations.FindAll(check => check.PageName.Equals(this.Input.Pages));
    }
}