/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using YAF.Controls;
using YAF.Modules;

namespace YAF.Editors
{
  /// <summary>
  /// IBaseEditorModule Interface for Editor classes.
  /// </summary>
  internal interface IBaseEditorModule
  {
    /// <summary>
    /// Gets a value indicating whether Active.
    /// </summary>
    bool Active
    {
      get;
    }

    /// <summary>
    /// Gets Description.
    /// </summary>
    string Description
    {
      get;
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    int ModuleId
    {
      get;
    }
  }

  /// <summary>
  /// Summary description for BaseForumEditor.
  /// </summary>
  public abstract class BaseForumEditor : BaseControl, IBaseEditorModule
  {
    /// <summary>
    /// The _base dir.
    /// </summary>
    protected string _baseDir = string.Empty;

    /// <summary>
    /// The _options.
    /// </summary>
    protected RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    #region IBaseEditorModule Members

    /// <summary>
    /// Gets a value indicating whether Active.
    /// </summary>
    public abstract bool Active
    {
      get;
    }

    /// <summary>
    /// Gets Description.
    /// </summary>
    public abstract string Description
    {
      get;
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    public virtual int ModuleId
    {
      get
      {
        return Description.GetHashCode();
      }
    }

    #endregion

    /// <summary>
    /// The resolve url.
    /// </summary>
    /// <param name="relativeUrl">
    /// The relative url.
    /// </param>
    /// <returns>
    /// The resolve url.
    /// </returns>
    public new string ResolveUrl(string relativeUrl)
    {
      if (this._baseDir != string.Empty)
      {
        return this._baseDir + relativeUrl;
      }

      return base.ResolveUrl(relativeUrl);
    }

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="txt">
    /// The txt.
    /// </param>
    /// <param name="match">
    /// The match.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    /// <returns>
    /// The replace.
    /// </returns>
    protected virtual string Replace(string txt, string match, string replacement)
    {
      while (Regex.IsMatch(txt, match, this._options))
      {
        txt = Regex.Replace(txt, match, replacement, this._options);
      }

      return txt;
    }

    #region Virtual Properties

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    public abstract string Text
    {
      get;
      set;
    }

    /// <summary>
    /// Sets BaseDir.
    /// </summary>
    public virtual string BaseDir
    {
      set
      {
        this._baseDir = value;
        if (!this._baseDir.EndsWith("/"))
        {
          this._baseDir += "/";
        }
      }
    }

    /// <summary>
    /// Gets or sets StyleSheet.
    /// </summary>
    public virtual string StyleSheet
    {
      get
      {
        return string.Empty;
      }

      set
      {
        ;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesHTML.
    /// </summary>
    public virtual bool UsesHTML
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    public virtual bool UsesBBCode
    {
      get
      {
        return false;
      }
    }

    #endregion
  }

  /// <summary>
  /// The yaf editor module manager.
  /// </summary>
  public class YafEditorModuleManager : YafModuleManager<BaseForumEditor>
  {
    /// <summary>
    /// Prevents a default instance of the <see cref="YafEditorModuleManager"/> class from being created.
    /// </summary>
    private YafEditorModuleManager()
      : base("YAF.Editors", "YAF.Editors.IBaseEditorModule")
    {
      if (ModuleClassTypes == null)
      {
        // re-add these modules...
        AddModules(BuildManager.CodeAssemblies);
      }
    }

    /// <summary>
    /// The get editor instance.
    /// </summary>
    /// <param name="moduleId">
    /// The module id.
    /// </param>
    /// <returns>
    /// </returns>
    public BaseForumEditor GetEditorInstance(int moduleId)
    {
      Load();

      // find the module (LINQ would be nice here)...
      foreach (BaseForumEditor editor in Modules)
      {
        if (editor.ModuleId == moduleId)
        {
          return editor;
        }
      }

      // not found
      return null;
    }

    /// <summary>
    /// The get editors table.
    /// </summary>
    /// <returns>
    /// </returns>
    public DataTable GetEditorsTable()
    {
      Load();

      using (var dt = new DataTable("Editors"))
      {
        dt.Columns.Add("Value", Type.GetType("System.Int32"));
        dt.Columns.Add("Name", Type.GetType("System.String"));

        foreach (BaseForumEditor editor in Modules)
        {
          if (editor.Active)
          {
            dt.Rows.Add(
              new object[]
                {
                  editor.ModuleId, editor.Description
                });
          }
        }

        return dt;
      }
    }
  }
}