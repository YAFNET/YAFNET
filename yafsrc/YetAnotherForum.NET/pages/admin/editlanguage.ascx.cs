/* Yet Another Forum.NET
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

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Administrative Page for the editing of forum properties.
  /// </summary>
  public partial class editlanguage : AdminPage
  {
    #region Constants and Fields

    /// <summary>
    ///   List of attributes for Resources in destination translation file, backing store for property
    /// </summary>
    private readonly StringDictionary _resourcesAttributes = new StringDictionary();

    /// <summary>
    ///   List of namespaces for  Resources  in destination translation file, backing store for property
    /// </summary>
    private readonly StringDictionary _resourcesNamespaces = new StringDictionary();

    /// <summary>
    ///   Indicates if Xml File is Syncronized
    /// </summary>
    private bool bUpdate;

    /// <summary>
    ///   Pysical Path to The languages folder
    /// </summary>
    private string sLangPath;

    /// <summary>
    ///   Xml File Name of Current Language
    /// </summary>
    private string sXmlFile;

    /// <summary>
    ///   The translations.
    /// </summary>
    private List<Translation> translations = new List<Translation>();

    #endregion

    #region Properties

    /// <summary>
    ///  List of attributes for Resources in destination translation file
    /// </summary>
    private StringDictionary ResourcesAttributes
    {
      get
      {
        return this._resourcesAttributes;
      }
    }

    /// <summary>
    ///  List of namespaces for Resources in destination translation file
    /// </summary>
    private StringDictionary ResourcesNamespaces
    {
      get
      {
        return this._resourcesNamespaces;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Remove all Resources with the same Name and Page
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="list">
    /// </param>
    /// <returns>
    /// The Cleaned List
    /// </returns>
    [NotNull]
    public static List<T> RemoveDuplicateSections<T>([NotNull] List<T> list) where T : Translation
    {
      var finalList = new List<T>();

      /*foreach (T item1 in
                list.Where(item1 => finalList.Find(check => check.sPageName.Equals(item1.sPageName) && check.sResourceName.Equals(item1.sResourceName) && check.sResourceValue.Equals(item1.sResourceValue) && check.sLocalizedValue.Equals(item1.sLocalizedValue)) == null))
            {
                finalList.Add(item1);
            }*/
      foreach (T item1 in
        list.Where(
          item1 =>
          finalList.Find(
            check => check.PageName.Equals(item1.PageName) && check.ResourceName.Equals(item1.ResourceName)) == null))
      {
        finalList.Add(item1);
      }

      return finalList;
    }

    /// <summary>
    /// Compare source and destination values on focus lost and indicate (guess) whether text is translated or not
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="args">
    /// </param>
    public void LocalizedTextCheck([NotNull] object sender, [NotNull] ServerValidateEventArgs args)
    {
      foreach (TextBox tbx in
        this.grdLocals.Items.Cast<DataGridItem>().Select(item => (TextBox)item.FindControl("txtLocalized")).Where(
          tbx => args.Value.Equals(tbx.Text)))
      {
        tbx.ForeColor = tbx.Text.Equals(tbx.ToolTip, StringComparison.OrdinalIgnoreCase) ? Color.Red : Color.Black;
        break;
      }

      args.IsValid = true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.sLangPath = HttpContext.Current.Request.MapPath("{0}languages".FormatWith(YafForumInfo.ForumServerFileRoot));

      if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("x") != null)
      {
        this.sXmlFile = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("x");

        this.dDLPages.Items.Clear();

        this.PopulateTranslations(
          Path.Combine(this.sLangPath, "english.xml"), Path.Combine(this.sLangPath, this.sXmlFile));
      }

      if (this.IsPostBack)
      {
        return;
      }

      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
     this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

     this.PageLinks.AddLink(this.GetText("ADMIN_LANGUAGES", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_languages));
     this.PageLinks.AddLink(this.GetText("ADMIN_EDITLANGUAGE", "TITLE"), string.Empty);

     this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
           this.GetText("ADMIN_ADMIN", "Administration"),
           this.GetText("ADMIN_LANGUAGES", "TITLE"),
           this.GetText("ADMIN_EDITLANGUAGE", "TITLE"));

     this.btnLoadPageLocalization.Text = this.GetText("ADMIN_EDITLANGUAGE", "LOAD_PAGE");
     this.btnSave.Text = this.GetText("COMMON", "SAVE");
     this.btnCancel.Text = this.GetText("COMMON", "CANCEL");

      this.dDLPages.Items.FindByText("DEFAULT").Selected = true;

      this.lblPageName.Text = "DEFAULT";

      if (this.bUpdate)
      {
        this.lblInfo.Visible = true;

        this.lblInfo.Text =  this.GetText("ADMIN_EDITLANGUAGE", "AUTO_SYNC");

        this.SaveLanguageFile();
      }
      else
      {
        this.lblInfo.Visible = false;
      }

      this.grdLocals.DataSource = this.translations.FindAll(check => check.PageName.Equals("DEFAULT"));
      this.grdLocals.DataBind();
    }

    /// <summary>
    /// Returns Back to The Languages Page
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private static void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_languages);
    }

    /// <summary>
    /// Checks if Resources are translated and handle Size of the Textboxes based on the Content Length
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    private static void GrdLocalsItemDataBound([NotNull] object sender, [NotNull] DataGridItemEventArgs e)
    {
      if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
      {
        return;
      }

      var txtLocalized = (TextBox)e.Item.FindControl("txtLocalized");

      var txtResource = (TextBox)e.Item.FindControl("txtResource");

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
    /// <param name="sPageName">
    /// </param>
    /// <param name="resourceName">
    /// </param>
    /// <param name="srcResourceValue">
    /// </param>
    /// <param name="dstResourceValue">
    /// </param>
    private void CreatePageResourceControl(
      [NotNull] string sPageName, 
      [NotNull] string resourceName, 
      [NotNull] string srcResourceValue, 
      [NotNull] string dstResourceValue)
    {
      var translation = new Translation
        {
          PageName = sPageName, 
          ResourceName = resourceName, 
          ResourceValue = srcResourceValue, 
          LocalizedValue = dstResourceValue
        };

      this.translations.Add(translation);
    }

    /// <summary>
    /// Creates a header row in the Resource Page DropDown Header text is page section name in XML file.
    /// </summary>
    /// <param name="sPageName">
    /// </param>
    private void CreatePageResourceHeader([NotNull] string sPageName)
    {
      this.dDLPages.Items.Add(new ListItem(sPageName, sPageName));
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
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    private void LoadPageLocalization([NotNull] object sender, [NotNull] EventArgs e)
    {
      // Save Values
      this.UpdateLocalizedValues();

      this.SaveLanguageFile();

      this.lblPageName.Text = this.dDLPages.SelectedValue;

      this.grdLocals.DataSource = this.translations.FindAll(check => check.PageName.Equals(this.dDLPages.SelectedValue));
      this.grdLocals.DataBind();
    }

    /// <summary>
    /// Wraps creation of translation controls.
    /// </summary>
    /// <param name="srcFile">
    /// </param>
    /// <param name="dstFile">
    /// </param>
    private void PopulateTranslations([NotNull] string srcFile, [NotNull] string dstFile)
    {
      this.bUpdate = false;

      try
      {
        new StringBuilder();
        new StringBuilder();

        var docSrc = new XmlDocument();
        var docDst = new XmlDocument();

        docSrc.Load(srcFile);
        docDst.Load(dstFile);

        XPathNavigator navSrc = docSrc.DocumentElement.CreateNavigator();
        XPathNavigator navDst = docDst.DocumentElement.CreateNavigator();

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
          string pageNameAttributeValue = pageItemNavigator.GetAttribute("name", String.Empty);

          this.CreatePageResourceHeader(pageNameAttributeValue);

          XPathNodeIterator resourceItemCollection = pageItemNavigator.Select("Resource");

          foreach (XPathNavigator resourceItem in resourceItemCollection)
          {
            string resourceTagAttributeValue = resourceItem.GetAttribute("tag", String.Empty);

            XPathNodeIterator iteratorSe =
              navDst.Select(
                "/Resources/page[@name=\"" + pageNameAttributeValue + "\"]/Resource[@tag=\"" + resourceTagAttributeValue +
                "\"]");

            if (iteratorSe.Count <= 0)
            {
              // pageResourceCount++;

              // Generate Missing Languages
              this.CreatePageResourceControl(
                pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value, resourceItem.Value);

              this.bUpdate = true;
            }

            while (iteratorSe.MoveNext())
            {
              // pageResourceCount++;
              if (!iteratorSe.Current.Value.Equals(resourceItem.Value, StringComparison.OrdinalIgnoreCase))
              {
              }

              this.CreatePageResourceControl(
                pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value, iteratorSe.Current.Value);
            }
          }
        }
      }
      catch (Exception exception)
      {
        LegacyDb.eventlog_create(null, this.GetType().ToString(), "Error loading files. {0}".FormatWith(exception.Message), 1);
      }
    }

    /// <summary>
    /// Save the Updated Xml File
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
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

      new XmlDocument();

      var xwSettings = new XmlWriterSettings
        {
           Encoding = Encoding.UTF8, OmitXmlDeclaration = false, Indent = true, IndentChars = "\t" 
        };

      XmlWriter xw = XmlWriter.Create(Path.Combine(this.sLangPath, this.sXmlFile), xwSettings);
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

      string currentPageName = String.Empty;

      foreach (Translation trans in this.translations)
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
    }

    /// <summary>
    /// Update Localizated Values in the Generics List
    /// </summary>
    private void UpdateLocalizedValues()
    {
      foreach (DataGridItem item in this.grdLocals.Items)
      {
        var txtLocalized = (TextBox)item.FindControl("txtLocalized");

        var lblResourceName = (Label)item.FindControl("lblResourceName");

        this.translations.Find(
          check => check.PageName.Equals(this.lblPageName.Text) && check.ResourceName.Equals(lblResourceName.Text)).
          LocalizedValue = txtLocalized.Text;
      }
    }

    #endregion

    /// <summary>
    /// Support class for TextBox.Tag property value
    /// </summary>
    public class Translation
    {
      #region Properties

      /// <summary>
      ///   The s localized value.
      /// </summary>
      public string LocalizedValue { get; set; }

      /// <summary>
      ///   The s page name.
      /// </summary>
      public string PageName { get; set; }

      /// <summary>
      ///   The s resource name.
      /// </summary>
      public string ResourceName { get; set; }

      /// <summary>
      ///   The s resource value.
      /// </summary>
      public string ResourceValue { get; set; }

      #endregion
    }
  }
}