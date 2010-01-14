/* Yet Another Forum.NET
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
namespace YAF.Editors
{
  using System;
  using System.Reflection;
  using System.Web.UI;

  /// <summary>
  /// The rich class editor.
  /// </summary>
  public abstract class RichClassEditor : BaseForumEditor
  {
    /// <summary>
    /// The _editor.
    /// </summary>
    protected Control _editor;

    /// <summary>
    /// The _init.
    /// </summary>
    protected bool _init;

    /// <summary>
    /// The _style sheet.
    /// </summary>
    protected string _styleSheet;

    /// <summary>
    /// The _typ editor.
    /// </summary>
    protected Type _typEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RichClassEditor"/> class.
    /// </summary>
    protected RichClassEditor()
    {
      this._init = false;
      this._styleSheet = string.Empty;
      this._editor = null;
      this._typEditor = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RichClassEditor"/> class.
    /// </summary>
    /// <param name="classBinStr">
    /// The class bin str.
    /// </param>
    protected RichClassEditor(string classBinStr)
    {
      this._init = false;
      this._styleSheet = string.Empty;
      this._editor = null;

      try
      {
        this._typEditor = Type.GetType(classBinStr, true);
      }
      catch (Exception x)
      {
        /*
#if DEBUG
				throw new Exception( "Unable to load editor class/dll: " + classBinStr + " Exception: " + x.Message );
#else
				YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Error);
#endif
*/
      }
    }

    /// <summary>
    /// The init editor object.
    /// </summary>
    /// <returns>
    /// The init editor object.
    /// </returns>
    protected bool InitEditorObject()
    {
      try
      {
        if (!this._init && this._typEditor != null)
        {
          // create instance of main class
          this._editor = (Control) Activator.CreateInstance(this._typEditor);
          this._init = true;
        }
      }
      catch (Exception)
      {
        // dll is not accessible
        return false;
      }

      return true;
    }

    /// <summary>
    /// The get interface in assembly.
    /// </summary>
    /// <param name="cAssembly">
    /// The c assembly.
    /// </param>
    /// <param name="className">
    /// The class name.
    /// </param>
    /// <returns>
    /// </returns>
    protected Type GetInterfaceInAssembly(Assembly cAssembly, string className)
    {
      Type[] types = cAssembly.GetTypes();
      foreach (Type typ in types)
      {
        // dynamically create or activate(if exist) object
        if (typ.FullName == className)
        {
          return typ;
        }
      }

      return null;
    }

    #region Properties

    /// <summary>
    /// Gets a value indicating whether Active.
    /// </summary>
    public override bool Active
    {
      get
      {
        return this._typEditor != null;
      }
    }

    /// <summary>
    /// Gets SafeID.
    /// </summary>
    protected virtual string SafeID
    {
      get
      {
        if (this._init)
        {
          return this._editor.ClientID.Replace("$", "_");
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsInitialized.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return this._init;
      }
    }

    /// <summary>
    /// Gets or sets StyleSheet.
    /// </summary>
    public override string StyleSheet
    {
      get
      {
        return this._styleSheet;
      }

      set
      {
        this._styleSheet = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesHTML.
    /// </summary>
    public override bool UsesHTML
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode
    {
      get
      {
        return false;
      }
    }

    #endregion
  }
}