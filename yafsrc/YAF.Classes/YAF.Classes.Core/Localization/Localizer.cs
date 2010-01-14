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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace YAF.Classes.Core
{
  using System.Diagnostics;

  /// <summary>
  /// Summary description for Localizer.
  /// </summary>
  public class Localizer
  {
    /// <summary>
    /// The _code.
    /// </summary>
    private string _code = string.Empty;

    /// <summary>
    /// The _current page.
    /// </summary>
    private string _currentPage = string.Empty;

    /// <summary>
    /// The _doc.
    /// </summary>
    private XmlDocument _doc = null;

    /// <summary>
    /// The _file name.
    /// </summary>
    private string _fileName = string.Empty;

    /// <summary>
    /// The _page pointer.
    /// </summary>
    private XmlNode _pagePointer = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Localizer"/> class.
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
      this._fileName = fileName;
      LoadFile();
    }

    /// <summary>
    /// Gets LanguageCode.
    /// </summary>
    public string LanguageCode
    {
      get
      {
        return this._code;
      }
    }

    /// <summary>
    /// The load file.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// </exception>
    private void LoadFile()
    {
      if (this._fileName == string.Empty || !File.Exists(this._fileName))
      {
        throw new ApplicationException("Invalid language file " + this._fileName);
      }

      if (this._doc == null)
      {
        this._doc = new XmlDocument();
      }

      try
      {
        this._doc.Load(this._fileName);

        if (this._doc.DocumentElement != null)
        {
          this._code = this._doc.DocumentElement.Attributes["code"] != null ? this._doc.DocumentElement.Attributes["code"].Value : "en";
        }
        else
        {
          this._doc = null;
        }
      }
      catch
      {
        this._doc = null;
      }
    }

    /// <summary>
    /// The load file.
    /// </summary>
    /// <param name="fileName">
    /// The file name.
    /// </param>
    public void LoadFile(string fileName)
    {
      this._fileName = fileName;
      LoadFile();
    }

    /// <summary>
    /// The set page.
    /// </summary>
    /// <param name="Page">
    /// The page.
    /// </param>
    public void SetPage(string Page)
    {
      if (this._currentPage == Page)
      {
        return;
      }

      this._pagePointer = null;
      this._currentPage = string.Empty;

      if (this._doc != null)
      {
        this._pagePointer = this._doc.SelectSingleNode(string.Format("//page[@name='{0}']", Page.ToUpper()));
        this._currentPage = Page;
      }
    }

    /// <summary>
    /// The get text.
    /// </summary>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="localizedText">
    /// The localized text.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public void GetText(string tag, out string localizedText)
    {
      // default the out parameters
      localizedText = string.Empty;
      XmlNode xmlPageNode = null;

      // verify that a document is loaded
      if (this._doc == null)
      {
        return;
      }

      tag = tag.ToUpper(new CultureInfo("en"));

#if DEBUG
      if (this._pagePointer == null)
      {
        Debug.WriteLine("Invalid Page Pointer: " + this._currentPage);
      }
#endif

      if (this._pagePointer != null)
      {
        // if in page subnode the text doesn't exist, try in whole file
        xmlPageNode = this._pagePointer.SelectSingleNode(string.Format("Resource[@tag='{0}']", tag)) ??
                      this._doc.SelectSingleNode(string.Format("//Resource[@tag='{0}']", tag));
      }
      else
      {
        xmlPageNode = this._doc.SelectSingleNode(string.Format("//Resource[@tag='{0}']", tag));
      }

      localizedText = xmlPageNode != null ? xmlPageNode.InnerText : null;
    }

    /// <summary>
    /// The get text.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    public string GetText(string page, string tag)
    {
      string text;

      SetPage(page);
      GetText(tag, out text);

      return text;
    }

    /// <summary>
    /// The get nodes using query.
    /// </summary>
    /// <param name="tagQuery">
    /// The tag query.
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="Exception">
    /// </exception>
    public List<XmlNode> GetNodesUsingQuery(string tagQuery)
    {
      XmlNodeList xmlNodeList = null;

      // verify that a document is loaded
      if (this._doc == null)
      {
        return null;
      }

#if DEBUG
      if (this._pagePointer == null)
      {
        throw new Exception("Invalid Page Pointer: " + this._currentPage);
      }

#endif

      if (this._pagePointer != null)
      {
        // if in page subnode the text doesn't exist, try in whole file
        xmlNodeList = this._pagePointer.SelectNodes(string.Format("Resource[{0}]", tagQuery));
      }
      else
      {
        xmlNodeList = this._doc.SelectNodes(string.Format("//Resource[{0}]", tagQuery));
      }

      // convert to dictionary...
      var items = new List<XmlNode>();

      if (xmlNodeList != null)
      {
        foreach (XmlNode node in xmlNodeList)
        {
          if (node != null)
          {
            items.Add(node);
          }
        }
      }

      return items;
    }
  }
}