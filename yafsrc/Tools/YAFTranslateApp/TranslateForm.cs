/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using SourceGrid;
using SourceGrid.Cells.Views;
using BorderStyle = System.Windows.Forms.BorderStyle;

namespace YAF.TranslateApp
{
    public partial class TranslateForm : Form
    {
        #region Private instance variables

        // Header separator row font style, backing store for property
        readonly Font _pageHeaderFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        // Column 1 (Resource tag) font style, backing store for property
        readonly Font _resourceHeaderFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        // List of namespaces for <Resources> in destination translation file, backing store for property
        readonly StringDictionary _resourcesNamespaces = new StringDictionary();

        // List of attributes for <Resources> in destination translation file, backing store for property
        readonly StringDictionary _resourcesAttributes = new StringDictionary();

        private List<Translation> translations = new List<Translation>();

        /// <summary>
        /// Language Code of the Source 
        /// Translation File
        /// </summary>
        private string sLangCodeSrc;

        /// <summary>
        /// Language Code of the Destionation 
        /// Translation File
        /// </summary>
        private string sLangCodeDest;

        private int RowCount;

        #endregion

        #region Properties

        /// <summary>
        /// Header separator row font style
        /// </summary>
        public Font PageHeaderFont { get { return _pageHeaderFont; } }

        /// <summary>
        /// Column 1 (Resource tag) font style
        /// </summary>
        public Font ResourceHeaderFont { get { return _resourceHeaderFont; } }

        /// <summary>
        /// Source translation file name (e.g. english.xml)
        /// </summary>
        //public string SourceTranslationFileName { get; set; }

        /// <summary>
        /// Destionation, target, translated file name
        /// </summary>
        //public string DestinationTranslationFileName { get; set; }

        /// <summary>
        /// Destination file changed flag
        /// </summary>
        public bool DestinationTranslationFileChanged { get; set; }

        // List of namespaces for <Resources> in destination translation file
        public StringDictionary ResourcesNamespaces { get { return _resourcesNamespaces; } }

        // List of attributes for <Resources> in destination translation file
        public StringDictionary ResourcesAttributes { get { return _resourcesAttributes; } }

        private readonly Cell cellLocalResource;

        private readonly Cell cellLocalResourceRed;

        #endregion

        #region Classes

        /// <summary>
        /// Support class for TextBox.Tag property value
        /// </summary>
        private class TextBoxTranslation
        {
            public string pageName;
            public string resourceName;
            public string srcResourceValue;
            // public string dstResourceValue;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default generated
        /// </summary>
        public TranslateForm()
        {
            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);

            cellLocalResourceRed = new Cell
                                       {
                                           Font = ResourceHeaderFont,
                                           TextAlignment =
                                               DevAge.Drawing.ContentAlignment.TopCenter,
                                           ForeColor = Color.Red,
                                           WordWrap = true,
                                           Border = cellBorder
                                       };
            cellLocalResource = new Cell
                                    {
                                        Font = ResourceHeaderFont,
                                        TextAlignment =
                                            DevAge.Drawing.ContentAlignment.TopCenter,
                                        WordWrap = true,
                                        Border = cellBorder
                                    };
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Populate translation tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnPopulateTranslationsClick(object sender, EventArgs e)
        {
            PopulateTranslations(tbxSourceTranslationFile.Text, tbxDestinationTranslationFile.Text);
        }


        /// <summary>
        /// Load source translation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnLoadSourceTranslationClick(object sender, EventArgs e)
        {
            string fileName = GetTranslationFileName("Select a File as Source Translation", "english.xml");

            if (!String.IsNullOrEmpty(fileName))
            {
                tbxSourceTranslationFile.Text = fileName;
            }

            if (!String.IsNullOrEmpty(tbxSourceTranslationFile.Text) && !String.IsNullOrEmpty(tbxDestinationTranslationFile.Text))
            {
                btnPopulateTranslations.Enabled = true;
            }

            string fileName2 = GetTranslationFileName("Select the Language File you want to Translate", null);

            if (!String.IsNullOrEmpty(fileName2))
            {
                tbxDestinationTranslationFile.Text = fileName2;
            }

            if (String.IsNullOrEmpty(tbxSourceTranslationFile.Text) ||
                String.IsNullOrEmpty(tbxDestinationTranslationFile.Text)) return;

            btnPopulateTranslations.Enabled = true;

            PopulateTranslations(tbxSourceTranslationFile.Text, tbxDestinationTranslationFile.Text);
        }


        /// <summary>
        /// Load destination, target, translation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnLoadDestinationTranslationClick(object sender, EventArgs e)
        {
            string fileName = GetTranslationFileName("Select the Language File you want to Translate", null);

            if (!String.IsNullOrEmpty(fileName))
            {
                tbxDestinationTranslationFile.Text = fileName;
            }

            if (String.IsNullOrEmpty(tbxSourceTranslationFile.Text) ||
                String.IsNullOrEmpty(tbxDestinationTranslationFile.Text)) return;

            btnPopulateTranslations.Enabled = true;

            PopulateTranslations(tbxSourceTranslationFile.Text, tbxDestinationTranslationFile.Text);
        }


        /// <summary>
        /// Exit application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnQuitClick(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// Save translation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnSaveClick(object sender, EventArgs e)
        {
            DestinationTranslationFileChanged = false;
            SaveTransalation();
        }


        /// <summary>
        /// Check if translation changed and ask to save if changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TranslateFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!DestinationTranslationFileChanged) return;

            switch (MessageBox.Show("Save changes before exiting?", "Save", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Yes:
                    if (!SaveTransalation())
                    {
                        e.Cancel = true;
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Set flag that transaltion has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TbxTextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
        }
        /// <summary>
        /// Auto Translate The Selected Resource via Google Translator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemClick(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;


            TextBox tbx = (TextBox)contextMenu.SourceControl;
            TextBoxTranslation tbt = (TextBoxTranslation)tbx.Tag;

            tbx.Text = Translator.TranslateText(tbx.Text, string.Format("{0}|{1}", sLangCodeSrc, sLangCodeDest));

            RangeRegion region = grid1.Selection.GetSelectionRegion();
            PositionCollection poss = region.GetCellsPositions();

            foreach (Position t in
                from t in poss let cell = grid1.GetCell(t) as SourceGrid.Cells.Cell where cell != null select t)
            {
                GetCell(grid1, t).View = tbt.srcResourceValue.Equals(tbx.Text, StringComparison.OrdinalIgnoreCase) ? cellLocalResourceRed : cellLocalResource;
            }

            // Update Translations List
            translations.Find(check =>
                              check.sPageName.Equals(tbt.pageName) && check.sResourceName.Equals(tbt.resourceName)).
                sLocalizedValue = tbx.Text;

            // tlpTranslations.Focus();
        }

        /// <summary>
        /// Compare source and destination values on focus lost and indicate (guess) whether text is translated or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TbxLostFocus(object sender, EventArgs e)
        {
            TextBox tbx = (TextBox)sender;
            TextBoxTranslation tbt = (TextBoxTranslation)tbx.Tag;


            tbx.ForeColor = tbt.srcResourceValue.Equals(tbx.Text, StringComparison.OrdinalIgnoreCase) ? Color.Red : Color.Black;

            RangeRegion region = grid1.Selection.GetSelectionRegion();
            PositionCollection poss = region.GetCellsPositions();

            foreach (Position t in
                from t in poss let cell = grid1.GetCell(t) as SourceGrid.Cells.Cell where cell != null select t)
            {
                GetCell(grid1, t).View = tbt.srcResourceValue.Equals(tbx.Text, StringComparison.OrdinalIgnoreCase) ? cellLocalResourceRed : cellLocalResource;
            }

            // Update Translations List
            translations.Find(check =>
                              check.sPageName.Equals(tbt.pageName) && check.sResourceName.Equals(tbt.resourceName)).
                sLocalizedValue = tbx.Text;

            //tlpTranslations.Focus();

        }

        /// <summary>
        /// cast cell on position pos to Cell
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static SourceGrid.Cells.Cell GetCell(GridVirtual grid, Position pos)
        {
            return grid.GetCell(pos) as SourceGrid.Cells.Cell;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Wraps creation of translation controls.
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="dstFile"></param>
        private void PopulateTranslations(string srcFile, string dstFile)
        {

            Cursor = Cursors.WaitCursor;

            RowCount = 0;

            grid1.Rows.Clear();
            grid1.Columns.Clear();

            grid1.BorderStyle = BorderStyle.FixedSingle;
           
            grid1.ColumnsCount = 3;

            grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize | SourceGrid.AutoSizeMode.Default;
            grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize | SourceGrid.AutoSizeMode.Default;
            grid1.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize | SourceGrid.AutoSizeMode.Default;

            grid1.MinimumWidth = 100;

            grid1.AutoStretchColumnsToFitWidth = true;
            grid1.AutoSizeCells();
            grid1.Columns.StretchToFit();
            grid1.Columns.AutoSizeView();

            Properties.Settings.Default.SourceTranslation = srcFile;
            Properties.Settings.Default.DestinationTranslation = dstFile;

            Properties.Settings.Default.Save();

            tbxSourceTranslationFile.Text = srcFile;
            tbxDestinationTranslationFile.Text = dstFile;

            translations.Clear();

            CreateTranslateControls(Properties.Settings.Default.SourceTranslation, Properties.Settings.Default.DestinationTranslation);

            Cursor = Cursors.Default;

            btnSave.Enabled = true;
            btnAutoTranslate.Enabled = true;
        }


        /// <summary>
        /// Creates and populates the translation controls given source and destination file names.
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="dstFile"></param>
        private void CreateTranslateControls(string srcFile, string dstFile)
        {
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

                if (navSrc.MoveToFirstAttribute())
                {
                    do
                    {
                        if (!navSrc.Name.Equals("code")) continue;

                        sLangCodeSrc = navSrc.Value;
                    } while (navSrc.MoveToNextAttribute());
                }


                navSrc.MoveToRoot();
                navSrc.MoveToFirstChild();

                if (navDst.MoveToFirstAttribute())
                {
                    do
                    {
                        if (navDst.Name.Equals("code"))
                        {
                            sLangCodeDest = navDst.Value;
                        }

                        ResourcesAttributes.Add(navDst.Name, navDst.Value);
                    } while (navDst.MoveToNextAttribute());
                }

                int totalResourceCount = 0;
                int resourcesNotTranslated = 0;
                //int pageNodeCount = 0;
                //int resourceMissingCount = 0;

                navDst.MoveToRoot();
                navDst.MoveToFirstChild();

                foreach (XPathNavigator pageItemNavigator in navSrc.Select("page"))
                {
                    //pageNodeCount++;
                    // int pageResourceCount = 0;

                    string pageNameAttributeValue = pageItemNavigator.GetAttribute("name", String.Empty);

                    CreatePageResourceHeader(pageNameAttributeValue);

                    XPathNodeIterator resourceItemCollection = pageItemNavigator.Select("Resource");

                    progressBar.Maximum = resourceItemCollection.Count;
                    progressBar.Minimum = 0;
                    progressBar.Value = 0;


                    foreach (XPathNavigator resourceItem in resourceItemCollection)
                    {
                        progressBar.Value++;
                        totalResourceCount++;

                        string resourceTagAttributeValue = resourceItem.GetAttribute("tag", String.Empty);

                        XPathNodeIterator iteratorSe = navDst.Select("/Resources/page[@name=\"" + pageNameAttributeValue + "\"]/Resource[@tag=\"" + resourceTagAttributeValue + "\"]");

                        if (iteratorSe.Count <= 0)
                        {
                            //pageResourceCount++;
                            //resourceMissingCount++;

                            DestinationTranslationFileChanged = true;

                            CreatePageResourceControl(pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value, resourceItem.Value);
                        }

                        while (iteratorSe.MoveNext())
                        {
                            //pageResourceCount++;

                            if (!iteratorSe.Current.Value.Equals(resourceItem.Value, StringComparison.OrdinalIgnoreCase))
                            {
                            }
                            else
                            {
                                resourcesNotTranslated++;
                            }

                            CreatePageResourceControl(pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value, iteratorSe.Current.Value);

                        }


                    }
                    //pageNodeCount++;
                }

                grid1.Columns.SetWidth(1, 100);
                grid1.Columns.StretchToFit();


                // Show Info
                toolStripStatusLabel1.Text =
                    string.Format("Total Resources: {0}; Resources Not Translated: {1}",
                                  totalResourceCount, resourcesNotTranslated);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading files. " + ex.Message, "Error", MessageBoxButtons.OK);
            }
        }


        /// <summary>
        /// Creates a header row in the TableLayoutPanel. Header text is page section name in XML file.
        /// </summary>
        /// <param name="pageName"></param>
        private void CreatePageResourceHeader(string pageName)
        {
            var pageHeader = new Cell
                                                          {
                                                              BackColor = Color.LightBlue,
                                                              Font = PageHeaderFont,
                                                              TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft,
                                                          };

            grid1.Rows.Insert(RowCount);


            grid1[RowCount, 0] = new SourceGrid.Cells.Cell(pageName) { View = pageHeader, ColumnSpan = 3 };
            grid1[RowCount, 1].AddController(new SourceGrid.Cells.Controllers.Unselectable());
            grid1.Rows[RowCount].Height = 50;

            RowCount++;

            grid1.Rows.Insert(RowCount);
            grid1[RowCount, 0] = new SourceGrid.Cells.ColumnHeader("Original Resource");
            grid1[RowCount, 1] = new SourceGrid.Cells.ColumnHeader("Resource Name");
            grid1[RowCount, 2] = new SourceGrid.Cells.ColumnHeader("Localized Resource");


            RowCount++;
        }


        /// <summary>
        /// Creates controls for column 1 (Resource tag) and column 2 (Resource value).
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="resourceName"></param>
        /// <param name="srcResourceValue"></param>
        /// <param name="dstResourceValue"></param>
        private void CreatePageResourceControl(string pageName, string resourceName, string srcResourceValue, string dstResourceValue)
        {
            SourceGrid.Cells.Editors.TextBox tbx = new SourceGrid.Cells.Editors.TextBox(typeof(string));

            tbx.Control.Text = dstResourceValue;
            tbx.Control.Multiline = true;

            if (tbx.Control.Text.Length > 30)
            {
                int height = 60 * (tbx.Control.Text.Length / 60);
                tbx.Control.Height = height;

            }


            Translation translation = new Translation
                                          {
                                              sPageName = pageName,
                                              sResourceName = resourceName,
                                              sResourceValue = srcResourceValue,
                                              sLocalizedValue = dstResourceValue
                                          };

            translations.Add(translation);


            if (srcResourceValue.Equals(dstResourceValue, StringComparison.OrdinalIgnoreCase))
            {
                tbx.Control.ForeColor = Color.Red;
            }
            else
            {
                // Show only not translated
                if (checkPendingOnly.Checked)
                {
                    return;
                }
            }

            tbx.Control.LostFocus += TbxLostFocus;
            tbx.Control.TextChanged += TbxTextChanged;

            MenuItem menuItem = new MenuItem { Text = "Auto Translate" };

            menuItem.Click += MenuItemClick;

            ContextMenu contextMenu = new ContextMenu();

            contextMenu.MenuItems.Add(menuItem);

            tbx.Control.ContextMenu = contextMenu;

            DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black, 1);
            DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);

            tbx.Control.Tag = new TextBoxTranslation
                                  {
                                      pageName = pageName,
                                      resourceName = resourceName,
                                      srcResourceValue = srcResourceValue
                                  };


            Cell cellResourceValue = new Cell
                                         {
                                             Font = ResourceHeaderFont,
                                             TextAlignment =
                                                 DevAge.Drawing.ContentAlignment.TopLeft,
                                             WordWrap = true,
                                             Border = cellBorder,
                                             BackColor = Color.LightGray
                                         };

            Cell cellResourceName = new Cell
                                        {
                                            Font = ResourceHeaderFont,
                                            TextAlignment =
                                                DevAge.Drawing.ContentAlignment.TopCenter,
                                            WordWrap = true,
                                            Border = cellBorder,
                                            BackColor = Color.LightGray
                                        };

            
            grid1.Rows.Insert(RowCount);
            grid1[RowCount, 0] = new SourceGrid.Cells.Cell(srcResourceValue, typeof(string)) { View = cellResourceValue };
            grid1[RowCount, 0].AddController(new SourceGrid.Cells.Controllers.Unselectable());

            grid1[RowCount, 1] = new SourceGrid.Cells.Cell(resourceName, typeof(string)) { View = cellResourceName };
            grid1[RowCount, 1].AddController(new SourceGrid.Cells.Controllers.Unselectable());

            if (tbx.Control.ForeColor.Equals(Color.Red))
            {
                grid1[RowCount, 2] = new SourceGrid.Cells.Cell(tbx.Control.Text) { View = cellLocalResourceRed, Editor = tbx };
            }
            else
            {
                grid1[RowCount, 2] = new SourceGrid.Cells.Cell(tbx.Control.Text) { View = cellLocalResource, Editor = tbx };
            }



            if (tbx.Control.Text.Length > 30)
            {
                int height = 60 * (tbx.Control.Text.Length / 60);

                grid1.Rows[RowCount].Height = height;
            }


            RowCount++;
        }

        /// <summary>
        /// Save translations back to original file.
        /// </summary>
        /// <returns></returns>
        private bool SaveTransalation()
        {
            bool result = true;

            int iOldCount = translations.Count;

            translations = RemoveDuplicateSections(translations);

            int iDuplicates = iOldCount - translations.Count;

            if (iDuplicates >= 1)
            {
                //MessageBox.Show(string.Format("{0} - Duplicate Entries Removed.", iDuplicates));
            }

            Cursor = Cursors.WaitCursor;

            try
            {
                new XmlDocument();

                XmlWriterSettings xwSettings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false,
                    Indent = true,
                    IndentChars = "\t"
                };


                XmlWriter xw = XmlWriter.Create(tbxDestinationTranslationFile.Text, xwSettings);
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
                    xw.WriteString(trans.sLocalizedValue);
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

                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving destination translation: " + ex.Message, "Error", MessageBoxButtons.OK);

                result = false;
            }

            Cursor = Cursors.Default;

            return result;

        }
        /// <summary>
        /// Remove all Resources with the same Name and Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>The Cleaned List</returns>
        public static List<T> RemoveDuplicateSections<T>(List<T> list) where T : Translation
        {
            List<T> finalList = new List<T>();

            /*foreach (T item1 in
                list.Where(item1 => finalList.Find(check => check.sPageName.Equals(item1.sPageName) && check.sResourceName.Equals(item1.sResourceName) && check.sResourceValue.Equals(item1.sResourceValue) && check.sLocalizedValue.Equals(item1.sLocalizedValue)) == null))
            {
                finalList.Add(item1);
            }*/

            foreach (T item1 in
                list.Where(item1 => finalList.Find(check => check.sPageName.Equals(item1.sPageName) && check.sResourceName.Equals(item1.sResourceName)) == null))
            {
                finalList.Add(item1);
            }

            return finalList;
        }

        /// <summary>
        /// Show open file dialog and return single filename
        /// </summary>
        /// <returns></returns>
        private static string GetTranslationFileName(string sTitle, string sFileName)
        {
            string result = null;

            OpenFileDialog ofd = new OpenFileDialog
                                     {
                                         CheckFileExists = true,
                                         CheckPathExists = true,
                                         Multiselect = false,
                                         Filter = "XML files (*.xml)|*.xml",
                                         Title = sTitle,
                                         FileName = sFileName
                                     };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                result = ofd.FileName;
            }

            return result;
        }
        /// <summary>
        /// Shows only Pending Translations or all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckPendingOnlyCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowPendingOnly = checkPendingOnly.Checked;

            Properties.Settings.Default.Save();

            if (!String.IsNullOrEmpty(tbxSourceTranslationFile.Text) &&
               !String.IsNullOrEmpty(tbxDestinationTranslationFile.Text))
            {
                PopulateTranslations(tbxSourceTranslationFile.Text, tbxDestinationTranslationFile.Text);
            }

        }

        #endregion

        /// <summary>
        /// Translate all Pending Ressources with Google Translator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoTranslateAll(object sender, EventArgs e)
        {
            progressBar.Maximum = grid1.Rows.Count;
            progressBar.Minimum = 0;

            progressBar.Value = 0;

            Cursor = Cursors.WaitCursor;

            for (int i = 0; i < grid1.Rows.Count; i++)
            {
                progressBar.Value = i;

                if (grid1[i, 2].Editor == null) continue;

                SourceGrid.Cells.Editors.TextBox tbx = (SourceGrid.Cells.Editors.TextBox)grid1[i, 2].Editor;

                TextBoxTranslation tbt = (TextBoxTranslation)tbx.Control.Tag;

                if (!tbx.Control.ForeColor.Equals(Color.Red)) continue;

                tbx.Control.Text = Translator.TranslateText(tbx.Control.Text,
                                                            string.Format("{0}|{1}", sLangCodeSrc, sLangCodeDest));

                tbx.Control.ForeColor = tbt.srcResourceValue.Equals(tbx.Control.Text, StringComparison.OrdinalIgnoreCase)
                                            ? Color.Red
                                            : Color.Black;

                // Update Translations List
                translations.Find(check =>
                                  check.sPageName.Equals(tbt.pageName) &&
                                  check.sResourceName.Equals(tbt.resourceName)).
                    sLocalizedValue = tbx.Control.Text;


                grid1[i, 2].View = tbt.srcResourceValue.Equals(tbx.Control.Text, StringComparison.OrdinalIgnoreCase)
                                       ? cellLocalResourceRed
                                       : cellLocalResource;

                grid1[i, 2].Value = tbx.Control.Text;
            }

            grid1.Update();


            Cursor = Cursors.Default;
        }

        private void TranslateForm_Load(object sender, EventArgs e)
        {
            checkPendingOnly.Checked = Properties.Settings.Default.ShowPendingOnly;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.SourceTranslation) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.DestinationTranslation))
            {
                PopulateTranslations(Properties.Settings.Default.SourceTranslation, Properties.Settings.Default.DestinationTranslation);
            }
        }
    }
}


