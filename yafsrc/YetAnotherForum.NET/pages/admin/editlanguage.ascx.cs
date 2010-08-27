/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages.Admin
{
    /// <summary>
    /// Administrative Page for the editting of forum properties.
    /// </summary>
    public partial class editlanguage : AdminPage
    {
        /// <summary>
        /// List of namespaces for  Resources  in destination translation file, backing store for property
        /// </summary>
        readonly StringDictionary _resourcesNamespaces = new StringDictionary();

        /// <summary>
        /// List of attributes for Resources in destination translation file, backing store for property
        /// </summary>
        readonly StringDictionary _resourcesAttributes = new StringDictionary();

        ///<summary>
        /// List of namespaces for Resources in destination translation file
        ///</summary>
        private StringDictionary ResourcesNamespaces { get { return _resourcesNamespaces; } }

        ///<summary>
        /// List of attributes for Resources in destination translation file
        ///</summary>
        private StringDictionary ResourcesAttributes { get { return _resourcesAttributes; } }

        /// <summary>
        /// Pysical Path to The languages folder
        /// </summary>
        private string sLangPath;

        /// <summary>
        /// Xml File Name of Current Language
        /// </summary>
        private string sXmlFile;

        /// <summary>
        /// Indicates if Xml File is Syncronized
        /// </summary>
        private bool bUpdate;

        /// <summary>
        /// Support class for TextBox.Tag property value
        /// </summary>
        public class Translation
        {
            ///<summary>
            ///</summary>
            public string sPageName;
            ///<summary>
            ///</summary>
            public string sResourceName;
            ///<summary>
            ///</summary>
            public string sResourceValue;
            ///<summary>
            ///</summary>
            public string sLocalizedValue;
        }

        private readonly List<Translation> translations = new List<Translation>();

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            sLangPath =
                HttpContext.Current.Request.MapPath(String.Format("{0}languages", YafForumInfo.ForumServerFileRoot));

            if (Request.QueryString.GetFirstOrDefault("x") != null)
            {
                sXmlFile = Request.QueryString.GetFirstOrDefault("x");

                PopulateTranslations(Path.Combine(sLangPath, "english.xml"),
                                     Path.Combine(sLangPath, sXmlFile));
            }

            if (IsPostBack) return;

            PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
            PageLinks.AddLink("Language Translator", string.Empty);

            dDLPages.Items.FindByText("DEFAULT").Selected = true;

            lblPageName.Text = "DEFAULT";

            if(bUpdate)
            {
                lblInfo.Visible = true;

                lblInfo.Text =
                    "Missing Translation Ressources are Automatically Syncronized and Updated.";

                SaveLanguageFile();
            }
            else
            {
                lblInfo.Visible = false;
            }

            grdLocals.DataSource = ListToDataTable(translations.FindAll(check => (check.sPageName.Equals("DEFAULT"))));
            grdLocals.DataBind();
        }

        ///<summary>
        /// Converts an Generics List to a DataTable
        ///</summary>
        ///<param name="list">List to Convert</param>
        ///<returns>The New Created DataTable</returns>
        public static DataTable ListToDataTable(IList list)
        {

            DataTable dt = null;
            Type listType = list.GetType();

            if (listType.IsGenericType)
            {
                Type elementType = listType.GetGenericArguments()[0];

                dt = new DataTable(elementType.Name + "List");

                MemberInfo[] miArray = elementType.GetMembers(
                    BindingFlags.Public | BindingFlags.Instance);

                foreach (MemberInfo mi in miArray)
                {
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Property:
                            {
                                PropertyInfo pi = mi as PropertyInfo;
                                dt.Columns.Add(pi.Name, pi.PropertyType);
                            }
                            break;
                        case MemberTypes.Field:
                            {
                                FieldInfo fi = mi as FieldInfo;
                                dt.Columns.Add(fi.Name, fi.FieldType);
                            }
                            break;
                    }
                }

                IList il = list;

                foreach (object record in il)
                {
                    int i = 0;
                    object[] fieldValues = new object[dt.Columns.Count];

                    foreach (MemberInfo mi in
                        from DataColumn c in dt.Columns select elementType.GetMember(c.ColumnName)[0])
                    {
                        switch (mi.MemberType)
                        {
                            case MemberTypes.Property:
                                {
                                    PropertyInfo pi = mi as PropertyInfo;
                                    fieldValues[i] = pi.GetValue(record, null);
                                }
                                break;
                            case MemberTypes.Field:
                                {
                                    FieldInfo fi = mi as FieldInfo;
                                    fieldValues[i] = fi.GetValue(record);
                                }
                                break;
                        }
                        i++;
                    }
                    dt.Rows.Add(fieldValues);
                }
            }
            return dt;

        }
        /// <summary>
        /// Wraps creation of translation controls.
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="dstFile"></param>
        private void PopulateTranslations(string srcFile, string dstFile)
        {
            bUpdate = false;

            try
            {
                new StringBuilder();
                new StringBuilder();

                XmlDocument docSrc = new XmlDocument();
                XmlDocument docDst = new XmlDocument();

                docSrc.Load(srcFile);
                docDst.Load(dstFile);

                XPathNavigator navSrc = docSrc.DocumentElement.CreateNavigator();
                XPathNavigator navDst = docDst.DocumentElement.CreateNavigator();

                ResourcesNamespaces.Clear();
                if (navDst.MoveToFirstNamespace())
                {
                    do
                    {
                        ResourcesNamespaces.Add(navDst.Name, navDst.Value);
                    } while (navDst.MoveToNextNamespace());
                }

                navDst.MoveToRoot();
                navDst.MoveToFirstChild();

                ResourcesAttributes.Clear();
                if (navDst.MoveToFirstAttribute())
                {
                    do
                    {
                        ResourcesAttributes.Add(navDst.Name, navDst.Value);
                    } while (navDst.MoveToNextAttribute());
                }



                navDst.MoveToRoot();
                navDst.MoveToFirstChild();

                foreach (XPathNavigator pageItemNavigator in navSrc.Select("page"))
                {
                    //int pageResourceCount = 0;

                    string pageNameAttributeValue = pageItemNavigator.GetAttribute("name", String.Empty);

                    CreatePageResourceHeader(pageNameAttributeValue);

                    XPathNodeIterator resourceItemCollection = pageItemNavigator.Select("Resource");

                    foreach (XPathNavigator resourceItem in resourceItemCollection)
                    {

                        string resourceTagAttributeValue = resourceItem.GetAttribute("tag", String.Empty);

                        XPathNodeIterator iteratorSe =
                            navDst.Select("/Resources/page[@name=\"" + pageNameAttributeValue + "\"]/Resource[@tag=\"" +
                                          resourceTagAttributeValue + "\"]");

                        if (iteratorSe.Count <= 0)
                        {
                            //pageResourceCount++;

                            // Generate Missing Languages
                            CreatePageResourceControl(pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value,
                                                      resourceItem.Value);

                            bUpdate = true;
                        }

                        while (iteratorSe.MoveNext())
                        {
                            //pageResourceCount++;

                            if (!iteratorSe.Current.Value.Equals(resourceItem.Value, StringComparison.OrdinalIgnoreCase))
                            {
                            }

                            CreatePageResourceControl(pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value,
                                                      iteratorSe.Current.Value);
                        }


                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Error loading files. " + ex.Message, "Error", MessageBoxButtons.OK);
            }
        }
        /// <summary>
        /// Creates a header row in the Ressource Page DropDown Header text is page section name in XML file.
        /// </summary>
        /// <param name="sPageName"></param>
        private void CreatePageResourceHeader(string sPageName)
        {
            dDLPages.Items.Add(new ListItem(sPageName, sPageName));
        }

        /// <summary>
        /// Creates controls for column 1 (Resource tag) and column 2 (Resource value).
        /// </summary>
        /// <param name="sPageName"></param>
        /// <param name="resourceName"></param>
        /// <param name="srcResourceValue"></param>
        /// <param name="dstResourceValue"></param>
        private void CreatePageResourceControl(string sPageName, string resourceName, string srcResourceValue, string dstResourceValue)
        {
            Translation translation = new Translation
                                          {
                                              sPageName = sPageName,
                                              sResourceName = resourceName,
                                              sResourceValue = Server.HtmlEncode(srcResourceValue),
                                              sLocalizedValue = Server.HtmlEncode(dstResourceValue)
                                          };

            translations.Add(translation);
        }
        /// <summary>
        /// Load Selected Page Ressources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPageLocalization(object sender, EventArgs e)
        {
            lblPageName.Text = dDLPages.SelectedValue;

            grdLocals.DataSource = ListToDataTable(translations.FindAll(check => (check.sPageName.Equals(dDLPages.SelectedValue))));
            grdLocals.DataBind();
        }

        /// <summary>
        /// Compare source and destination values on focus lost and indicate (guess) whether text is translated or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBoxTextChanged(object sender, EventArgs e)
        {
            TextBox tbx = (TextBox)sender;

            if (tbx.Text.Equals(tbx.ToolTip, StringComparison.OrdinalIgnoreCase))
            {
                tbx.ForeColor = Color.Red;
            }
            else
            {
                tbx.ForeColor = Color.Black;
            }

        }


        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
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
        private void SaveClick(object sender, EventArgs e)
        {
           
            UpdateLocalizedValues();

            SaveLanguageFile();
        }
        /// <summary>
        /// Save the Updated Xml File.
        /// </summary>
        private void SaveLanguageFile()
        {
            new XmlDocument();

            XmlWriterSettings xwSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false,
                Indent = true,
                IndentChars = "\t"
            };


            XmlWriter xw = XmlWriter.Create(Path.Combine(sLangPath, "test.xml"), xwSettings);
            xw.WriteStartDocument();

            // <Resources>
            xw.WriteStartElement("Resources");

            foreach (string key in ResourcesNamespaces.Keys)
            {
                xw.WriteAttributeString("xmlns", key, null, ResourcesNamespaces[key]);
            }

            foreach (string key in ResourcesAttributes.Keys)
            {
                xw.WriteAttributeString(key, ResourcesAttributes[key]);
            }

            string currentPageName = String.Empty;


            foreach (Translation trans in translations)
            {
                // <page></page>
                if (!trans.sPageName.Equals(currentPageName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!String.IsNullOrEmpty(currentPageName))
                    {
                        xw.WriteFullEndElement();
                    }

                    currentPageName = trans.sPageName;

                    xw.WriteStartElement("page");
                    xw.WriteAttributeString("name", currentPageName);

                }

                xw.WriteStartElement("Resource");
                xw.WriteAttributeString("tag", trans.sResourceName);
                xw.WriteString(Server.HtmlDecode(trans.sLocalizedValue));
                xw.WriteFullEndElement();
            }

            // final </page>
            if (!String.IsNullOrEmpty(currentPageName))
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
            foreach (DataGridItem item in grdLocals.Items)
            {
                TextBox txtLocalized = (TextBox)item.FindControl("txtLocalized");

                Label lblResourceName = (Label)item.FindControl("lblResourceName");

                translations.Find(
                    check =>
                    check.sPageName.Equals(lblPageName.Text) && check.sResourceName.Equals(lblResourceName.Text)).
                    sLocalizedValue = txtLocalized.Text;
            }
        }

        /// <summary>
        /// Checks if Resources are translated and handle Size of the Textboxes based on the Content Length
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GrdLocalsItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            TextBox txtLocalized = (TextBox)e.Item.FindControl("txtLocalized");

            TextBox txtResource = (TextBox)e.Item.FindControl("txtResource");

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
        /// Returns Back to The Languages Page
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void CancelClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_languages);
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoadPageLocalization.Click += LoadPageLocalization;
            btnCancel.Click += CancelClick;
            btnSave.Click += SaveClick;

            grdLocals.ItemDataBound += GrdLocalsItemDataBound;
        }


    }
}