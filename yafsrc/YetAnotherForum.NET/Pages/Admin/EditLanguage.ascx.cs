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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml;
    using System.Xml.XPath;

    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Administrative Page for the editing of forum properties.
    /// </summary>
    public partial class EditLanguage : AdminPage
    {
        #region Constants and Fields

        /// <summary>
        ///   Indicates if Xml File is Synchronized
        /// </summary>
        private bool update;

        /// <summary>
        ///   Physical Path to The languages folder
        /// </summary>
        private string langPath;

        /// <summary>
        ///   Xml File Name of Current Language
        /// </summary>
        private string xmlFile;

        /// <summary>
        /// The current page name.
        /// </summary>
        private string pageName;

        /// <summary>
        ///   The translations.
        /// </summary>
        private List<Translation> translations = new List<Translation>();

        #endregion

        #region Properties

        /// <summary>
        ///  Gets the List of attributes for Resources in destination translation file
        /// </summary>
        private StringDictionary ResourcesAttributes { get; } = new StringDictionary();

        /// <summary>
        ///  Gets the List of namespaces for Resources in destination translation file
        /// </summary>
        private StringDictionary ResourcesNamespaces { get; } = new StringDictionary();

        #endregion

        #region Public Methods

        /// <summary>
        /// Remove all Resources with the same Name and Page
        /// </summary>
        /// <typeparam name="T">The typed parameter</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// The Cleaned List
        /// </returns>
        [NotNull]
        public static List<T> RemoveDuplicateSections<T>([NotNull] List<T> list)
            where T : Translation
        {
            var finalList = new List<T>();

            finalList.AddRange(
                list.Where(
                    item1 => finalList.Find(
                                 check => check.PageName.Equals(item1.PageName)
                                          && check.ResourceName.Equals(item1.ResourceName)) == null));

            return finalList;
        }

        /// <summary>
        /// Compare source and destination values on focus lost and indicate (guess) whether text is translated or not
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void LocalizedTextCheck([NotNull] object sender, [NotNull] ServerValidateEventArgs args)
        {
            foreach (var tbx in this.grdLocals.Items.Cast<DataGridItem>()
                .Select(item => item.FindControlAs<TextBox>("txtLocalized")).Where(tbx => args.Value.Equals(tbx.Text)))
            {
                tbx.ForeColor = tbx.Text.Equals(tbx.ToolTip, StringComparison.OrdinalIgnoreCase)
                                    ? Color.Red
                                    : Color.Black;
                break;
            }

            args.IsValid = true;
        }

        #endregion

        #region Methods

        /// <summary>Raises the <see cref="E:System.Web.UI.Control.Init"/> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
           BoardContext.Current.PageElements.RegisterJsBlock(
                "FixGridTableJs",
                JavaScriptBlocks.FixGridTable(this.grdLocals.ClientID));

           base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.langPath = HttpContext.Current.Request.MapPath($"{BoardInfo.ForumServerFileRoot}languages");

            if (this.Get<HttpRequestBase>().QueryString.Exists("x"))
            {
                this.xmlFile = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("x");

                this.dDLPages.Items.Clear();

                this.PopulateTranslations(
                    Path.Combine(this.langPath, "english.xml"),
                    Path.Combine(this.langPath, this.xmlFile));
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.dDLPages.Items.FindByText("DEFAULT").Selected = true;

            this.pageName = "DEFAULT";

            this.IconHeader.Text = $"{this.GetText("ADMIN_EDITLANGUAGE", "HEADER")} {this.pageName}";

            if (this.update)
            {
                this.Info.Visible = true;

                this.lblInfo.Text = this.GetText("ADMIN_EDITLANGUAGE", "AUTO_SYNC");

                this.SaveLanguageFile();
            }
            else
            {
                this.Info.Visible = false;
            }

            this.grdLocals.DataSource = this.translations.FindAll(check => check.PageName.Equals("DEFAULT"));
            this.grdLocals.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(
                this.GetText("ADMIN_LANGUAGES", "TITLE"),
                BuildLink.GetLink(ForumPages.Admin_Languages));
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITLANGUAGE", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_LANGUAGES", "TITLE")} - {this.GetText("ADMIN_EDITLANGUAGE", "TITLE")}";
        }

        /// <summary>
        /// Returns Back to The Languages Page
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_Languages);
        }

        /// <summary>
        /// Checks if Resources are translated and handle Size of the Textboxes based on the Content Length
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
        private static void GrdLocalsItemDataBound([NotNull] object sender, [NotNull] DataGridItemEventArgs e)
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

            if (txtLocalized.Text.Equals(txtResource.Text, StringComparison.OrdinalIgnoreCase))
            {
                txtLocalized.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Creates controls for column 1 (Resource tag) and column 2 (Resource value).
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="srcResourceValue">The SRC resource value.</param>
        /// <param name="dstResourceValue">The DST resource value.</param>
        private void CreatePageResourceControl(
            [NotNull] string pageName,
            [NotNull] string resourceName,
            [NotNull] string srcResourceValue,
            [NotNull] string dstResourceValue)
        {
            var translation = new Translation
                                  {
                                      PageName = pageName,
                                      ResourceName = resourceName,
                                      ResourceValue = srcResourceValue,
                                      LocalizedValue = dstResourceValue
                                  };

            this.translations.Add(translation);
        }

        /// <summary>
        /// Creates a header row in the Resource Page DropDown Header text is page section name in XML file.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        private void CreatePageResourceHeader([NotNull] string pageName)
        {
            this.dDLPages.Items.Add(new ListItem(pageName, pageName));
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLoadPageLocalization.Click += this.LoadPageLocalization;
            this.btnCancel.Click += CancelClick;
            this.btnSave.Click += this.SaveClick;

            this.grdLocals.ItemDataBound += GrdLocalsItemDataBound;
        }

        /// <summary>
        /// Load Selected Page Resources
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoadPageLocalization([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Save Values
            this.UpdateLocalizedValues();

            this.SaveLanguageFile();

            this.pageName = this.dDLPages.SelectedValue;

            this.IconHeader.Text = $"{this.GetText("ADMIN_EDITLANGUAGE", "HEADER")} {this.pageName}";

            this.grdLocals.DataSource =
                this.translations.FindAll(check => check.PageName.Equals(this.dDLPages.SelectedValue));
            this.grdLocals.DataBind();
        }

        /// <summary>
        /// Wraps creation of translation controls.
        /// </summary>
        /// <param name="srcFile">The SRC file.</param>
        /// <param name="dstFile">The DST file.</param>
        private void PopulateTranslations([NotNull] string srcFile, [NotNull] string dstFile)
        {
            this.update = false;

            try
            {
                var docSrc = new XmlDocument();
                var docDst = new XmlDocument();

                docSrc.Load(srcFile);
                docDst.Load(dstFile);

                var navSrc = docSrc.DocumentElement.CreateNavigator();
                var navDst = docDst.DocumentElement.CreateNavigator();

                this.ResourcesNamespaces.Clear();
                if (navDst.MoveToFirstNamespace())
                {
                    do
                    {
                        this.ResourcesNamespaces.Add(navDst.Name, navDst.Value);
                    }
                    while (navDst.MoveToNextNamespace());
                }

                navDst.MoveToRoot();
                navDst.MoveToFirstChild();

                this.ResourcesAttributes.Clear();
                if (navDst.MoveToFirstAttribute())
                {
                    do
                    {
                        this.ResourcesAttributes.Add(navDst.Name, navDst.Value);
                    }
                    while (navDst.MoveToNextAttribute());
                }

                navDst.MoveToRoot();
                navDst.MoveToFirstChild();

                foreach (XPathNavigator pageItemNavigator in navSrc.Select("page"))
                {
                    // int pageResourceCount = 0;
                    var pageNameAttributeValue = pageItemNavigator.GetAttribute("name", string.Empty);

                    this.CreatePageResourceHeader(pageNameAttributeValue);

                    var resourceItemCollection = pageItemNavigator.Select("Resource");

                    foreach (XPathNavigator resourceItem in resourceItemCollection)
                    {
                        var resourceTagAttributeValue = resourceItem.GetAttribute("tag", string.Empty);

                        var iteratorSe = navDst.Select(
                            $"/Resources/page[@name=\"{pageNameAttributeValue}\"]/Resource[@tag=\"{resourceTagAttributeValue}\"]");

                        if (iteratorSe.Count <= 0)
                        {
                            // pageResourceCount++;

                            // Generate Missing Languages
                            this.CreatePageResourceControl(
                                pageNameAttributeValue,
                                resourceTagAttributeValue,
                                resourceItem.Value,
                                resourceItem.Value);

                            this.update = true;
                        }

                        while (iteratorSe.MoveNext())
                        {
                            // pageResourceCount++;
                            if (!iteratorSe.Current.Value.Equals(
                                    resourceItem.Value,
                                    StringComparison.OrdinalIgnoreCase))
                            {
                            }

                            this.CreatePageResourceControl(
                                pageNameAttributeValue,
                                resourceTagAttributeValue,
                                resourceItem.Value,
                                iteratorSe.Current.Value);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.Log(null, this, $"Error loading files. {exception.Message}");
            }
        }

        /// <summary>
        /// Save the Updated Xml File
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UpdateLocalizedValues();

            this.SaveLanguageFile();
        }

        /// <summary>
        /// Save the Updated Xml File.
        /// </summary>
        private void SaveLanguageFile()
        {
            this.translations = RemoveDuplicateSections(this.translations);

            var settings = new XmlWriterSettings
                               {
                                   Encoding = Encoding.UTF8,
                                   OmitXmlDeclaration = false,
                                   Indent = true,
                                   IndentChars = " "
                               };

            var xw = XmlWriter.Create(Path.Combine(this.langPath, this.xmlFile), settings);
            xw.WriteStartDocument();

            // <Resources>
            xw.WriteStartElement("Resources");

            foreach (string key in this.ResourcesNamespaces.Keys)
            {
                xw.WriteAttributeString("xmlns", key, null, this.ResourcesNamespaces[key]);
            }

            foreach (string key in this.ResourcesAttributes.Keys)
            {
                xw.WriteAttributeString(key, this.ResourcesAttributes[key]);
            }

            var currentPageName = string.Empty;

            foreach (var trans in this.translations.OrderBy(t => t.PageName).ThenBy(t => t.ResourceName))
            {
                // <page></page>
                if (!trans.PageName.Equals(currentPageName, StringComparison.OrdinalIgnoreCase))
                {
                    if (currentPageName.IsSet())
                    {
                        xw.WriteFullEndElement();
                    }

                    currentPageName = trans.PageName;

                    xw.WriteStartElement("page");
                    xw.WriteAttributeString("name", currentPageName);
                }

                xw.WriteStartElement("Resource");
                xw.WriteAttributeString("tag", trans.ResourceName);
                xw.WriteString(trans.LocalizedValue);
                xw.WriteFullEndElement();
            }

            // final </page>
            if (currentPageName.IsSet())
            {
                xw.WriteFullEndElement();
            }

            // </Resources>
            xw.WriteFullEndElement();

            xw.WriteEndDocument();
            xw.Close();

            HttpRuntime.UnloadAppDomain();
        }

        /// <summary>
        /// Update Localized Values in the Generics List
        /// </summary>
        private void UpdateLocalizedValues()
        {
            this.grdLocals.Items.Cast<DataGridItem>().ForEach(
                item =>
                    {
                        var txtLocalized = item.FindControlAs<TextBox>("txtLocalized");

                        var lblResourceName = item.FindControlAs<Label>("lblResourceName");

                        this.translations.Find(
                                check => check.PageName.Equals(this.pageName)
                                         && check.ResourceName.Equals(lblResourceName.Text)).LocalizedValue =
                            txtLocalized.Text;
                    });
        }

        #endregion
    }
}