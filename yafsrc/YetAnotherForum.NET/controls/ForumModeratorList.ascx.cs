/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections;
  using System.Data;

  using YAF.Core;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The forum moderator list.
  /// </summary>
  public partial class ForumModeratorList : BaseUserControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumModeratorList" /> class.
    /// </summary>
    public ForumModeratorList()
    {
      this.PreRender += this.ForumModeratorList_PreRender;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Sets DataSource.
    /// </summary>
    public IEnumerable DataSource
    {
      set
      {
        this.ModeratorList.DataSource = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum moderator list_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumModeratorList_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (((DataRow[])this.ModeratorList.DataSource).Length > 0)
      {
        // no need for the "blank dash"...
        this.BlankDash.Visible = false;
      }
    }

    #endregion
  }
}