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
using System.Linq;
using System.Web;

namespace YAF.Classes.Core
{
  /// <summary>
  /// The load message.
  /// </summary>
  public class LoadMessage
  {
    /// <summary>
    /// The _load string list.
    /// </summary>
    private List<string> _loadStringList = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadMessage"/> class.
    /// </summary>
    public LoadMessage()
    {
      YafContext.Current.Unload += new EventHandler<EventArgs>(Current_Unload);
    }

    /// <summary>
    /// Gets LoadStringList.
    /// </summary>
    public List<string> LoadStringList
    {
      get
      {
        if (this._loadStringList == null && HttpContext.Current.Session["LoadStringList"] != null)
        {
          // get this as the current "loadstring"
          this._loadStringList = HttpContext.Current.Session["LoadStringList"] as List<string>;

          // session load string no longer needed
          HttpContext.Current.Session["LoadStringList"] = null;
        }
        else if (this._loadStringList == null)
        {
          this._loadStringList = new List<string>();
        }

        return this._loadStringList;
      }
    }

    #region Load Message

    /// <summary>
    /// Gets LoadString.
    /// </summary>
    public string LoadString
    {
      get
      {
        if (LoadStringList.Count() == 0)
        {
          return String.Empty;
        }

        return LoadStringDelimited("\r\n");
      }
    }

    /// <summary>
    /// Gets StringJavascript.
    /// </summary>
    public string StringJavascript
    {
      get
      {
        return CleanJsString(LoadString);
      }
    }

    /// <summary>
    /// The clean js string.
    /// </summary>
    /// <param name="jsString">
    /// The js string.
    /// </param>
    /// <returns>
    /// The clean js string.
    /// </returns>
    public static string CleanJsString(string jsString)
    {
      string message = jsString;
      message = message.Replace("\\", "\\\\");
      message = message.Replace("'", "\\'");
      message = message.Replace("\r\n", "\\r\\n");
      message = message.Replace("\n", "\\n");
      message = message.Replace("\"", "\\\"");
      return message;
    }

    /// <summary>
    /// The load string delimited.
    /// </summary>
    /// <param name="delimiter">
    /// The delimiter.
    /// </param>
    /// <returns>
    /// The load string delimited.
    /// </returns>
    public string LoadStringDelimited(string delimiter)
    {
      if (LoadStringList.Count() == 0)
      {
        return String.Empty;
      }

      return LoadStringList.Aggregate((current, next) => current + delimiter + next);
    }

    /// <summary>
    /// AddLoadMessage creates a message that will be returned on the next page load.
    /// </summary>
    /// <param name="message">
    /// The message you wish to display.
    /// </param>
    public void Add(string message)
    {
      LoadStringList.Add(message);
    }

    /// <summary>
    /// AddLoadMessageSession creates a message that will be returned on the next page.
    /// </summary>
    /// <param name="message">
    /// The message you wish to display.
    /// </param>
    public void AddSession(string message)
    {
      List<string> list = null;

      if (HttpContext.Current.Session["LoadStringList"] != null)
      {
        list = HttpContext.Current.Session["LoadStringList"] as List<string>;
      }
      else
      {
        list = new List<string>();
      }

      // add it too the session list...
      if (list != null)
      {
        list.Add(message);
      }
    }

    /// <summary>
    /// Clear the Load String (error) List
    /// </summary>
    public void Clear()
    {
      LoadStringList.Clear();
    }

    #endregion

    /// <summary>
    /// The current_ unload.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Current_Unload(object sender, EventArgs e)
    {
      // clear the load message...
      Clear();
    }
  }
}