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
namespace YAF.Classes.Core
{
  using System;
  using System.Xml;
  using YAF.Classes.Utils;
  using Interfaces;

  public class YafTheme : IYafTheme
  {
    private string _themeFile = null;
    private XmlDocument _themeXmlDoc = null;
    private bool _logMissingThemeItem = false;

    public YafTheme()
    {

    }

    public YafTheme(string newThemeFile)
    {
      ThemeFile = newThemeFile;
    }

    /// <summary>
    /// Get or Set the current Theme File
    /// </summary>
    public string ThemeFile
    {
      get
      {
        return _themeFile;
      }
      set
      {
        if (_themeFile != value)
        {
          if (IsValidTheme(value))
          {
            _themeFile = value;
            _themeXmlDoc = null;
          }
        }
      }
    }

    public bool LogMissingThemeItem
    {
      get
      {
        return _logMissingThemeItem;
      }
      set
      {
        _logMissingThemeItem = value;
      }
    }

    private void LoadThemeFile()
    {
      if (ThemeFile != null)
      {
#if !DEBUG
        if (_themeXmlDoc == null)
        {
          _themeXmlDoc = (XmlDocument)System.Web.HttpContext.Current.Cache[ThemeFile];
        }
#endif

        if (_themeXmlDoc == null)
        {
          _themeXmlDoc = new XmlDocument();
          _themeXmlDoc.Load(
                      System.Web.HttpContext.Current.Server.MapPath(String.Concat(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Themes, "/", ThemeFile)));
#if !DEBUG
          System.Web.HttpContext.Current.Cache[ThemeFile] = _themeXmlDoc;
#endif
        }
      }
    }

    /// <summary>
    /// Basic testing of the theme's validity...
    /// </summary>
    /// <param name="themeFile"></param>
    /// <returns></returns>
    public static bool IsValidTheme(string themeFile)
    {
      if (String.IsNullOrEmpty(themeFile)) return false;

      themeFile = themeFile.Trim().ToLower();

      if (themeFile.Length == 0) return false;

      if (!themeFile.EndsWith(".xml")) return false;

      return System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(String.Concat(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Themes, "/", themeFile.Trim())));
    }

    public string GetItem(string page, string tag)
    {
      return GetItem(page, tag, String.Format("[{0}.{1}]", page.ToUpper(), tag.ToUpper()));
    }

    public string GetItem(string page, string tag, string defaultValue)
    {
      string item = "";

      LoadThemeFile();

      if (_themeXmlDoc != null)
      {
        string themeDir = _themeXmlDoc.DocumentElement.Attributes["dir"].Value;
        string langCode = YafContext.Current.Localization.LanguageCode.ToUpper();
        string select = string.Format("//page[@name='{0}']/Resource[@tag='{1}' and @language='{2}']", page.ToUpper(), tag.ToUpper(), langCode);

        XmlNode node = _themeXmlDoc.SelectSingleNode(select);
        if (node == null)
        {
          select = string.Format("//page[@name='{0}']/Resource[@tag='{1}']", page.ToUpper(), tag.ToUpper());
          node = _themeXmlDoc.SelectSingleNode(select);
        }

        if (node == null)
        {
          if (LogMissingThemeItem) YAF.Classes.Data.DB.eventlog_create(YafContext.Current.PageUserID, page.ToLower() + ".ascx", String.Format("Missing Theme Item: {0}.{1}", page.ToUpper(), tag.ToUpper()), EventLogTypes.Error);
          return defaultValue;
        }

        item = node.InnerText.Replace("~", String.Format("{0}{1}/{2}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Themes, themeDir));
      }

      return item;
    }

    public string ThemeDir
    {
      get
      {
        LoadThemeFile();
        return String.Format("{0}{1}/{2}/", YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Themes, _themeXmlDoc.DocumentElement.Attributes["dir"].Value);
      }
    }

    /// <summary>
    /// Gets full path to the given theme file.
    /// </summary>
    /// <param name="filename">Short name of theme file.</param>
    /// <returns></returns>
    public string BuildThemePath(string filename)
    {
      return ThemeDir + filename;
    }

    /// <summary>
    /// Gets the collapsible panel image url (expanded or collapsed).
    /// <param name="panelID">ID of collapsible panel</param>
    /// <param name="defaultState">Default Panel State</param>
    /// </summary>
    /// <returns>Image URL</returns>
    public string GetCollapsiblePanelImageURL(string panelID, PanelSessionState.CollapsiblePanelState defaultState)
    {
      PanelSessionState.CollapsiblePanelState stateValue = Mession.PanelState[panelID];
      if (stateValue == PanelSessionState.CollapsiblePanelState.None)
      {
        stateValue = defaultState;
        Mession.PanelState[panelID] = defaultState;
      }

      return GetItem("ICONS", (stateValue == PanelSessionState.CollapsiblePanelState.Expanded ? "PANEL_COLLAPSE" : "PANEL_EXPAND"));
    }
  }
}