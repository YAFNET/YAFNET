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
  using System.Linq;
  using System.Threading;

  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for Localizer.
  /// </summary>
  public class Localizer
  {
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

    private CultureInfo _currentCulture = null;

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
      this.LoadFile();
      this.InitCulture();
    }

    private void InitCulture()
    {
      if (YafServices.InitializeDb.Initialized)
      {
        // vzrus: Culture code is missing for a user until he saved his profile.
        // First set it to board culture              
        if (this.CurrentCulture == null)
        {
          try
          {
            this._currentCulture = new CultureInfo(YafContext.Current.BoardSettings.Culture);
          }
          catch
          {
            this._currentCulture = Thread.CurrentThread.CurrentCulture;
          }
        }

        //  && this.CurrentCulture.Name.Substring(0, 2) == YafContext.Current.BoardSettings.Culture.Substring(0, 2)

        string cultureUser = YafContext.Current.CultureUser;

        if (YafContext.Current.CultureUser.IsSet())
        {
          if (cultureUser.Substring(0, 2).Contains(this.CurrentCulture.Name.Substring(0, 2)))
          {
            this._currentCulture = new CultureInfo(cultureUser);
          }
        }
      }
    }

    /// <summary>
    /// Gets LanguageCode.
    /// </summary>
    public CultureInfo CurrentCulture
    {
      get
      {         
          return this._currentCulture;
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
        this._fileName = "english.xml";
        //throw new ApplicationException("Invalid language file " + this._fileName);
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
          this._currentCulture =
            new CultureInfo(
              _doc.DocumentElement.Attributes["code"] != null
                ? this._doc.DocumentElement.Attributes["code"].Value
                : "en-US");

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
    /// <param name="page">
    /// The page.
    /// </param>
    public void SetPage(string page)
    {
      if (this._currentPage == page)
      {
        return;
      }

      this._pagePointer = null;
      this._currentPage = string.Empty;

      if (String.IsNullOrEmpty(page))
      {
        page = "DEFAULT";
      }

      if (this._doc != null)
      {
        this._pagePointer = this._doc.SelectSingleNode(string.Format("//page[@name='{0}']", page.ToUpper()));
        this._currentPage = page;
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

      tag = tag.ToUpper(this._currentCulture);

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
        items.AddRange(xmlNodeList.Cast<XmlNode>().Where(node => node != null));
      }

      return items;
    }
  }
}