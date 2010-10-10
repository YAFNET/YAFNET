/* YetAnotherForum.NET
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

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
        public string SourceTranslationFileName { get; set; }

        /// <summary>
        /// Destionation, target, translated file name
        /// </summary>
        public string DestinationTranslationFileName { get; set; }

        /// <summary>
        /// Destination file changed flag
        /// </summary>
        public bool DestinationTranslationFileChanged { get; set; }

        // List of namespaces for <Resources> in destination translation file
        public StringDictionary ResourcesNamespaces { get { return _resourcesNamespaces; } }

        // List of attributes for <Resources> in destination translation file
        public StringDictionary ResourcesAttributes { get { return _resourcesAttributes; } }

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
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            tlpTranslations.DoubleBuffered(true);

        }


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
            if (DestinationTranslationFileChanged)
            {
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
        }


        /// <summary>
        /// Set flag that transaltion has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TbxTextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;

            tlpTranslations.Focus();
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

            tbx.ForeColor = tbt.srcResourceValue.Equals(tbx.Text, StringComparison.OrdinalIgnoreCase) ? Color.Red : Color.Black;

            // Update Translations List
            translations.Find(check =>
                              check.sPageName.Equals(tbt.pageName) && check.sResourceName.Equals(tbt.resourceName)).
                sLocalizedValue = tbx.Text;

            tlpTranslations.Focus();
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

            // Update Translations List
            translations.Find(check =>
                              check.sPageName.Equals(tbt.pageName) && check.sResourceName.Equals(tbt.resourceName)).
                sLocalizedValue = tbx.Text;

            tlpTranslations.Focus();

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
            tlpTranslations.Controls.Clear();

            Cursor = Cursors.WaitCursor;

            tlpTranslations.SuspendLayout();
            SuspendLayout();

            tlpTranslations.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            tlpTranslations.ColumnStyles.Clear();
            tlpTranslations.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));
            tlpTranslations.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tlpTranslations.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            tlpTranslations.RowStyles.Clear();
            tlpTranslations.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            SourceTranslationFileName = srcFile;
            DestinationTranslationFileName = dstFile;

            tbxSourceTranslationFile.Text = srcFile;
            tbxDestinationTranslationFile.Text = dstFile;

            translations.Clear();

            CreateTranslateControls(SourceTranslationFileName, DestinationTranslationFileName);

            tlpTranslations.ResumeLayout(false);
            tlpTranslations.PerformLayout();
            ResumeLayout(false);

            // Fix TableLayoutPanel bug, always visible horizontal scrollbar
            Padding pad = tlpTranslations.Padding;
            tlpTranslations.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);

            tlpTranslations.PerformLayout();
            tlpTranslations.Padding = pad;
            tlpTranslations.PerformLayout();

            tlpTranslations.Focus();


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
                int resourceMissingCount = 0;

                navDst.MoveToRoot();
                navDst.MoveToFirstChild();

                foreach (XPathNavigator pageItemNavigator in navSrc.Select("page"))
                {
                    //pageNodeCount++;
                    int pageResourceCount = 0;

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
                            pageResourceCount++;
                            resourceMissingCount++;

                            DestinationTranslationFileChanged = true;

                            CreatePageResourceControl(pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value, resourceItem.Value, pageResourceCount);
                        }

                        while (iteratorSe.MoveNext())
                        {
                            pageResourceCount++;

                            if (!iteratorSe.Current.Value.Equals(resourceItem.Value, StringComparison.OrdinalIgnoreCase))
                            {
                            }
                            else
                            {
                                resourcesNotTranslated++;
                            }

                            CreatePageResourceControl(pageNameAttributeValue, resourceTagAttributeValue, resourceItem.Value, iteratorSe.Current.Value, pageResourceCount);

                        }


                    }
                    //pageNodeCount++;
                }

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
            Label lbl = new Label
                            {
                                AutoSize = false,
                                Height = 30,
                                Text = pageName,
                                TextAlign = ContentAlignment.MiddleLeft,
                                Font = PageHeaderFont,
                                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                                BackColor = Color.Azure
                            };

            tlpTranslations.Controls.Add(lbl);
            tlpTranslations.SetColumnSpan(lbl, 3);
        }


        /// <summary>
        /// Creates controls for column 1 (Resource tag) and column 2 (Resource value).
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="resourceName"></param>
        /// <param name="srcResourceValue"></param>
        /// <param name="dstResourceValue"></param>
        /// <param name="rowNum"></param>
        private void CreatePageResourceControl(string pageName, string resourceName, string srcResourceValue, string dstResourceValue, int rowNum)
        {
            Label lbl = new Label();
            Label lblSource = new Label();
            TextBox tbx = new TextBox();

            ////
            lblSource.Text = resourceName;
            lblSource.TextAlign = ContentAlignment.MiddleLeft;
            lblSource.Font = ResourceHeaderFont;
            lblSource.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblSource.AutoEllipsis = true;
            lblSource.TextAlign = ContentAlignment.TopCenter;
            ////

            lbl.Text = srcResourceValue;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Font = ResourceHeaderFont;
            lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lbl.AutoEllipsis = true;
            lbl.TextAlign = ContentAlignment.TopCenter;

            tbx.Text = dstResourceValue;
            tbx.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbx.Multiline = true;

            if (tbx.Text.Length > 30)
            {
                int height = 60 * (tbx.Text.Length / 60);
                tbx.Height = height;
                lbl.Height = height;
                lblSource.Height = height;
               
                //tbx.Height = Unit.Pixel(80);
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
                tbx.ForeColor = Color.Red;
            }
            else
            {
                // Show only not translated
                if (checkPendingOnly.Checked)
                {
                    return;
                }
            }

            tbx.LostFocus += TbxLostFocus;
            tbx.TextChanged += TbxTextChanged;

            MenuItem menuItem = new MenuItem { Text = "Auto Translate" };

            menuItem.Click += MenuItemClick;

            ContextMenu contextMenu = new ContextMenu();

            contextMenu.MenuItems.Add(menuItem);

            tbx.ContextMenu = contextMenu;



            tbx.Tag = new TextBoxTranslation { pageName = pageName, resourceName = resourceName, srcResourceValue = srcResourceValue };

            tlpTranslations.Controls.Add(lbl);
            ////
            tlpTranslations.Controls.Add(lblSource);
            ////
            tlpTranslations.Controls.Add(tbx);

            lbl.Text = lbl.Text;


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
                MessageBox.Show(string.Format("{0} - Duplicate Entries Removed.", iDuplicates));
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
            progressBar.Maximum = tlpTranslations.Controls.Count;
            progressBar.Minimum = 0;

            progressBar.Value = 0;

            foreach (Control c in tlpTranslations.Controls)
            {
                progressBar.Value++;

                if (!(c is TextBox)) continue;

                TextBox tbx = (TextBox)c;
                TextBoxTranslation tbt = (TextBoxTranslation)c.Tag;

                if (!tbx.ForeColor.Equals(Color.Red)) continue;

                tbx.Text = Translator.TranslateText(tbx.Text, string.Format("{0}|{1}", sLangCodeSrc, sLangCodeDest));

                tbx.ForeColor = tbt.srcResourceValue.Equals(tbx.Text, StringComparison.OrdinalIgnoreCase) ? Color.Red : Color.Black;

                // Update Translations List
                translations.Find(check =>
                                  check.sPageName.Equals(tbt.pageName) &&
                                  check.sResourceName.Equals(tbt.resourceName)).
                    sLocalizedValue = tbx.Text;
            }
        }
    }

}
