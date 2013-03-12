/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Core
{
  #region Using

  using System.Text.RegularExpressions;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Summary description for BaseForumEditor.
  /// </summary>
  public abstract class ForumEditor : BaseControl, IEditorModule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _base dir.
    /// </summary>
    protected string _baseDir = string.Empty;

    /// <summary>
    ///   The _options.
    /// </summary>
    protected RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value indicating whether Active.
    /// </summary>
    public abstract bool Active { get; }

    /// <summary>
    ///   Sets BaseDir.
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
    ///   Gets Description.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    ///   Gets ModuleId.
    /// </summary>
    public virtual string ModuleId
    {
      get
      {
        return this.Description.GetHashCode().ToString();
      }
    }

    /// <summary>
    ///   Gets or sets StyleSheet.
    /// </summary>
    [NotNull]
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
    ///   Gets or sets Text.
    /// </summary>
    public abstract string Text { get; set; }

    /// <summary>
    ///   Gets a value indicating whether UsesBBCode.
    /// </summary>
    public virtual bool UsesBBCode
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether UsesHTML.
    /// </summary>
    public virtual bool UsesHTML
    {
      get
      {
        return false;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The resolve url.
    /// </summary>
    /// <param name="relativeUrl">
    /// The relative url.
    /// </param>
    /// <returns>
    /// The resolve url.
    /// </returns>
    public new string ResolveUrl([NotNull] string relativeUrl)
    {
      if (this._baseDir != string.Empty)
      {
        return this._baseDir + relativeUrl;
      }

      return base.ResolveUrl(relativeUrl);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The add editor control.
    /// </summary>
    /// <param name="editor">
    /// The editor.
    /// </param>
    protected virtual void AddEditorControl([NotNull] Control editor)
    {
      var newDiv = new HtmlGenericControl("div") { ID = "EditorDiv" };
      newDiv.Attributes.Add("class", "EditorDiv");
      newDiv.Controls.Add(editor);
      this.Controls.Add(newDiv);
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
    protected virtual string Replace([NotNull] string txt, [NotNull] string match, [NotNull] string replacement)
    {
      while (Regex.IsMatch(txt, match, this._options))
      {
        txt = Regex.Replace(txt, match, replacement, this._options);
      }

      return txt;
    }

    #endregion
  }
}