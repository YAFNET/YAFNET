/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
    ///   Gets or sets a value indicating whether AttachChecked.
    /// </summary>
    public bool AttachChecked
    {
      get
      {
        return this.TopicAttach.Checked;
      }

      set
      {
        this.TopicAttach.Checked = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether AttachOptionVisible.
    /// </summary>
    public bool AttachOptionVisible
    {
      get
      {
        return this.liTopicAttach.Visible;
      }

      set
      {
        this.liTopicAttach.Visible = value;
      }
    }

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
    ///   Gets or sets a value indicating whether PersistantChecked.
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
    ///   Gets or sets a value indicating whether PersistantOptionVisible.
    /// </summary>
    public bool PersistantOptionVisible
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
    }

    #endregion
  }
}