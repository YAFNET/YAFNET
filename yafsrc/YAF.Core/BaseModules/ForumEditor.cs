/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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