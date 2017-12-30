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

  using YAF.Core;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The post options.
  /// </summary>
  public partial class PostOptions : BaseUserControl
  {
    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether IsQuestionChecked.
    /// </summary>
    public bool IsQuestionChecked
    {
      get
      {
        return this.chkIsQuestion.Checked;
      }

      set
      {
        this.chkIsQuestion.Checked = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether IsQuestionVisible.
    /// </summary>
    public bool IsQuestionVisible
    {
      get
      {
        return this.liQuestion.Visible;
      }

      set
      {
        this.liQuestion.Visible = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether Persistent Checked.
    /// </summary>
    public bool PersistantChecked
    {
      get
      {
        return this.Persistency.Checked;
      }

      set
      {
        this.Persistency.Checked = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether Persistent Option Visible.
    /// </summary>
    public bool PersistentOptionVisible
    {
      get
      {
        return this.liPersistency.Visible;
      }

      set
      {
        this.liPersistency.Visible = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether Poll Option is Visible.
    /// </summary>
    public bool PollChecked
    {
      get
      {
        return this.AddPollCheckBox.Checked;
      }

      set
      {
        this.AddPollCheckBox.Checked = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether Poll Option is Visible.
    /// </summary>
    public bool PollOptionVisible
    {
      get
      {
        return this.liAddPoll.Visible;
      }

      set
      {
        this.liAddPoll.Visible = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether WatchChecked.
    /// </summary>
    public bool WatchChecked
    {
      get
      {
        return this.TopicWatch.Checked;
      }

      set
      {
        this.TopicWatch.Checked = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether WatchOptionVisible.
    /// </summary>
    public bool WatchOptionVisible
    {
      get
      {
        return this.liTopicWatch.Visible;
      }

      set
      {
        this.liTopicWatch.Visible = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
    }

    #endregion
  }
}