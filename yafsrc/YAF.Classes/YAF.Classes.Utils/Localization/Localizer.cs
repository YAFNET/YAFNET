/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Xml;

namespace YAF.Classes.Utils
{
    /// <summary>
    /// Summary description for Localizer.
    /// </summary>
    public class Localizer
    {
        private XmlDocument _doc = null;
        private XmlNode _pagePointer = null;
        private string _fileName = "";
        private string _currentPage = "";
        private string _code = "";

        public Localizer()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public Localizer(string fileName)
        {
            _fileName = fileName;
            LoadFile();
        }

        private void LoadFile()
        {
            if (_fileName == "" || !System.IO.File.Exists(_fileName))
                throw (new ApplicationException("Invalid language file " + _fileName));

            if (_doc == null)
                _doc = new XmlDocument();

            try
            {
                _doc.Load(_fileName);
                if (_doc.DocumentElement.Attributes["code"] != null)
                    _code = _doc.DocumentElement.Attributes["code"].Value;
                else
                    _code = "en";
            }
            catch
            {
                _doc = null;
            }
        }

        public void LoadFile(string fileName)
        {
            _fileName = fileName;
            LoadFile();
        }

        public void SetPage(string Page)
        {
            if (_currentPage == Page)
                return;

            _pagePointer = null;
            _currentPage = "";

            if (_doc != null)
            {
                _pagePointer = _doc.SelectSingleNode(string.Format("//page[@name='{0}']", Page.ToUpper()));
                _currentPage = Page;
            }
        }

        public void GetText(string tag, out string localizedText)
        {
            // default the out parameters
            localizedText = "";
            XmlNode el = null;

            // verify that a document is loaded
            if (_doc == null)
                return;

            tag = tag.ToUpper(new System.Globalization.CultureInfo("en"));

#if DEBUG
            if (_pagePointer == null)
                throw new Exception("Missing page pointer: " + tag);
#endif

            if (_pagePointer != null)
            {
                el = _pagePointer.SelectSingleNode(string.Format("Resource[@tag='{0}']", tag));
                // if in page subnode the text doesn't exist, try in whole file
                if (el == null)
                    el = _doc.SelectSingleNode(string.Format("//Resource[@tag='{0}']", tag));
            }
            else
            {
                el = _doc.SelectSingleNode(string.Format("//Resource[@tag='{0}']", tag));
            }

            if (el != null)
            {
                localizedText = el.InnerText;
            }
            else
            {
                //YAF.Classes.Data.YAF.Classes.Data.DB.eventlog_create(null,"Localizer: GetText",String.Format("Missing Language Item \"{0}\"",text),EventLogTypes.Warning);
                localizedText = null;
            }
        }

        public string GetText(string page, string tag)
        {
            SetPage(page);
            string text;
            GetText(tag, out text);
            return text;
        }

        public string LanguageCode
        {
            get
            {
                return _code;
            }
        }
    }
}
