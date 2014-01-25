/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
namespace YAF.Editors
{
  using System;
  using System.Reflection;

  using YAF.Core;
  using YAF.Types;

  /// <summary>
  /// The fck editor v 1.
  /// </summary>
  public class FCKEditorV1 : RichClassEditor
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "FCKEditorV1" /> class.
    /// </summary>
    public FCKEditorV1()
      : base("FredCK.FCKeditor,FredCK.FCKeditor")
    {
      this.InitEditorObject();
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Description.
    /// </summary>
    [NotNull]
    public override string Description
    {
      get
      {
        return "FCK Editor v1.6 (HTML)";
      }
    }

    /// <summary>
    ///   Gets ModuleId.
    /// </summary>
    public override string ModuleId
    {
      get
      {
        // backward compatibility...
        return "4";
      }
    }

    /// <summary>
    ///   Gets or sets Text.
    /// </summary>
    [NotNull]
    public override string Text
    {
      get
      {
        if (this._init)
        {
          PropertyInfo pInfo = this._typEditor.GetProperty("Value");
          return Convert.ToString(pInfo.GetValue(this._editor, null));
        }
        else
        {
          return string.Empty;
        }
      }

      set
      {
        if (this._init)
        {
          PropertyInfo pInfo = this._typEditor.GetProperty("Value");
          pInfo.SetValue(this._editor, value, null);
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The editor_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected virtual void Editor_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this._init && this._editor.Visible)
      {
        PropertyInfo pInfo;
        pInfo = this._typEditor.GetProperty("BasePath");
        pInfo.SetValue(this._editor, this.ResolveUrl("FCKEditorV1/"), null);
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      if (this._init)
      {
        this.Load += this.Editor_Load;
        PropertyInfo pInfo = this._typEditor.GetProperty("ID");
        pInfo.SetValue(this._editor, "edit", null);
        this.Controls.Add(this._editor);
      }

      base.OnInit(e);
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      YafContext.Current.PageElements.RegisterJsInclude("FckEditorJs", this.ResolveUrl("FCKEditorV1/FCKEditor.js"));
    }

    #endregion
  }
}