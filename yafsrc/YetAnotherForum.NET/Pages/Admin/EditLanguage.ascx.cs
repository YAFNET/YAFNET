/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using Newtonsoft.Json;

using System.IO;

using YAF.Types.Objects;

using ListItem = ListItem;

/// <summary>
/// Administrative Page for the editing of forum properties.
/// </summary>
public partial class EditLanguage : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditLanguage"/> class. 
    /// </summary>
    public EditLanguage()
        : base("ADMIN_EDITLANGUAGE", ForumPages.Admin_EditLanguage)
    {
    }

    /// <summary>
    ///   Physical Path to The languages folder
    /// </summary>
    private string langPath;

    /// <summary>
    ///   File Name of Current Language
    /// </summary>
    private string languageFile;

    /// <summary>
    /// The current page name.
    /// </summary>
    private string pageName;

    /// <summary>
    ///   The translations.
    /// </summary>
    private readonly List<Translation> translations = new();

    /// <summary>
    /// Compare source and destination values on focus lost and indicate (guess) whether text is translated or not
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
    public void LocalizedTextCheck([NotNull] object sender, [NotNull] ServerValidateEventArgs args)
    {
        var textBox = this.Locals.Items.Cast<DataGridItem>()
            .Select(item => item.FindControlAs<TextBox>("txtLocalized"))
            .FirstOrDefault(tbx => args.Value.Equals(tbx.Text));

        textBox.CssClass = textBox.Text.Equals(textBox.ToolTip, StringComparison.OrdinalIgnoreCase)
                                ? "form-control is-invalid"
                                : "form-control is-valid";

        args.IsValid = true;
    }

    /// <summary>
    /// Registers the needed Java Scripts
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        base.OnPreRender(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.langPath = this.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}languages");

        if (this.Get<HttpRequestBase>().QueryString.Exists("x"))
        {
            this.languageFile = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("x");

            this.Pages.Items.Clear();

            this.PopulateTranslations(
                Path.Combine(this.langPath, "english.json"),
                Path.Combine(this.langPath, this.languageFile));
        }

        if (this.IsPostBack)
        {
            return;
        }

        this.Pages.Items.FindByText("DEFAULT").Selected = true;

        this.pageName = "DEFAULT";

        this.IconHeader.Text = $"{this.GetText("ADMIN_EDITLANGUAGE", "HEADER")} {this.pageName}";

        this.Locals.DataSource = this.translations.FindAll(check => check.PageName.Equals("DEFAULT"));
        this.Locals.DataBind();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_LANGUAGES", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Languages));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITLANGUAGE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Checks if Resources are translated and handle Size of the Textboxes based on the Content Length
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
    protected void LocalsItemDataBound([NotNull] object sender, [NotNull] DataGridItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var txtLocalized = e.Item.FindControlAs<TextBox>("txtLocalized");

        var txtResource = e.Item.FindControlAs<TextBox>("txtResource");

        if (txtResource.Text.Length > 30)
        {
            // int height = 80 * (txtSource.Text.Length / 80);
            txtResource.TextMode = TextBoxMode.MultiLine;
            txtResource.Height = Unit.Pixel(80);

            txtLocalized.TextMode = TextBoxMode.MultiLine;
            txtLocalized.Height = Unit.Pixel(80);
        }

        txtLocalized.CssClass = txtLocalized.Text.Equals(txtResource.Text, StringComparison.OrdinalIgnoreCase)
                                    ? "form-control is-invalid"
                                    : "form-control is-valid";
    }

    /// <summary>
    /// Returns Back to The Languages Page
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Languages);
    }

    /// <summary>
    /// Load Selected Page Resources
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void LoadPageLocalizationClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.pageName = this.Pages.SelectedValue;

        this.IconHeader.Text = $"{this.GetText("ADMIN_EDITLANGUAGE", "HEADER")} {this.pageName}";

        this.Locals.DataSource =
            this.translations.FindAll(check => check.PageName.Equals(this.Pages.SelectedValue));
        this.Locals.DataBind();
    }

    /// <summary>
    /// Save the Updated Xml File
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.UpdateLocalizedValues();

        this.SaveLanguageFile();

        this.PageBoardContext.Notify(this.GetText("ADMIN_EDITLANGUAGE", "SAVED_FILE"), MessageTypes.success);
    }

    /// <summary>
    /// Save the Updated Xml File.
    /// </summary>
    private void SaveLanguageFile()
    {
        var translationResource = this.Get<ILocalization>().LoadLanguageFile(Path.Combine(this.langPath, this.languageFile));

        this.pageName = this.Pages.SelectedValue;
        var pageTranslations = this.translations.Where(x => x.PageName == this.pageName);

        translationResource.Resources.Page.First(x => x.Name == this.pageName).Resource.ForEach(
            resource =>
            {
                resource.Text = pageTranslations.First(x => x.ResourceName == resource.Tag).LocalizedResourceText;
            });

        var serializer = new JsonSerializer { Formatting = Formatting.Indented };

        using var sw = new StreamWriter(Path.Combine(this.langPath, this.languageFile));
        using JsonWriter writer = new JsonTextWriter(sw);
        serializer.Serialize(writer, translationResource);

        HttpRuntime.UnloadAppDomain();
    }

    /// <summary>
    /// Update Localized Values in the Generics List
    /// </summary>
    private void UpdateLocalizedValues()
    {
        this.Locals.Items.Cast<DataGridItem>().ForEach(
            item =>
                {
                    var txtLocalized = item.FindControlAs<TextBox>("txtLocalized");
                    var txtResource = item.FindControlAs<TextBox>("txtResource");

                    var lblResourceName = item.FindControlAs<Label>("lblResourceName");

                    this.translations.Find(
                        check => check.PageName.Equals(this.Pages.SelectedValue) &&
                                 check.ResourceName.Equals(lblResourceName.Text)).LocalizedResourceText = txtLocalized.Text;

                    txtLocalized.CssClass = txtLocalized.Text.Equals(txtResource.Text, StringComparison.OrdinalIgnoreCase)
                                                ? "form-control is-invalid"
                                                : "form-control is-valid";
                });
    }

    /// <summary>
    /// Wraps creation of translation controls.
    /// </summary>
    /// <param name="sourceFile">The source file.</param>
    /// <param name="translationFile">The DST file.</param>
    private void PopulateTranslations([NotNull] string sourceFile, [NotNull] string translationFile)
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
            this.Logger.Log(null, this, $"Error loading files. {exception.Message}");
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
        [NotNull] string page,
        [NotNull] string resourceName,
        [NotNull] string originalResourceText,
        [NotNull] string translationResourceText)
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
    private void CreatePageResourceHeader([NotNull] string name)
    {
        this.Pages.Items.Add(new ListItem(name, name));
    }
}