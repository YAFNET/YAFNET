/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Model;
  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Models;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The showsmilies.
  /// </summary>
  public partial class showsmilies : ForumPage
  {
    // constructor
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "showsmilies" /> class.
    /// </summary>
    public showsmilies()
      : base("SHOWSMILIES")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get smiley script.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <returns>
    /// The get smiley script.
    /// </returns>
    protected string GetSmileyScript([NotNull] string code, [NotNull] string icon)
    {
      code = code.ToLower();
      code = code.Replace("&", "&amp;");
      code = code.Replace("\"", "&quot;");
      code = code.Replace("'", "\\'");

      return "javascript:{0}('{1} ','{3}{4}/{2}');".FormatWith(
        "insertsmiley", code, icon, YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.ShowToolBar = false;
      this.ShowFooter = false;

      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.List.DataSource = this.GetRepository<Smiley>().ListUnique(this.PageContext.PageBoardID);
      this.DataBind();
    }

    #endregion
  }
}