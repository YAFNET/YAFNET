/* Yet Another Forum.net
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
namespace YAF.Core.Tasks
{
  using System;

  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;

  /// <summary>
  /// The forum delete task.
  /// </summary>
  public class ForumDeleteTask : LongBackgroundTask
  {
    #region Constants and Fields

    /// <summary>
    /// The _task name.
    /// </summary>
    private const string _taskName = "ForumDeleteTask";

    /// <summary>
    /// The _forum id.
    /// </summary>
    private int _forumId;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumDeleteTask"/> class.
    /// </summary>
    public ForumDeleteTask()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets TaskName.
    /// </summary>
    public static string TaskName
    {
      get
      {
        return _taskName;
      }
    }

    /// <summary>
    /// Gets or sets ForumId.
    /// </summary>
    public int ForumId
    {
      get
      {
        return this._forumId;
      }

      set
      {
        this._forumId = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// The start.
    /// </returns>
    public static bool Start(int boardId, int forumId)
    {
      if (YafTaskModule.Current == null)
      {
        return false;
      }

      if (!YafTaskModule.Current.TaskExists(TaskName))
      {
        var task = new ForumDeleteTask { BoardID = boardId, ForumId = forumId };
        YafTaskModule.Current.StartTask(TaskName, task);
      }

      return true;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      try
      {
        DB.forum_delete(this.ForumId);
        DB.eventlog_create(null, TaskName, "Forum (ID: {0}) Delete Task Complete.".FormatWith(this.ForumId), 2);
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, TaskName, "Error In Forum (ID: {0}) Delete Task: {1}".FormatWith(this.ForumId, x));
      }
    }

    #endregion
  }
}