/* Yet Another Forum.net
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
namespace YAF.Core.Tasks
{
  using System;

  using YAF.Classes.Data;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;

  /// <summary>
  /// Run when we want to do migration of users in the background...
  /// </summary>
  public class PruneTopicTask : LongBackgroundTask
  {
    /// <summary>
    /// The _task name.
    /// </summary>
    private const string _taskName = "PruneTopicTask";

    /// <summary>
    /// The _days.
    /// </summary>
    private int _days;

    /// <summary>
    /// The _forum id.
    /// </summary>
    private int _forumId;

    /// <summary>
    /// The _perm delete.
    /// </summary>
    private bool _permDelete;

    /// <summary>
    /// Initializes a new instance of the <see cref="PruneTopicTask"/> class.
    /// </summary>
    public PruneTopicTask()
    {
    }

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

    /// <summary>
    /// Gets or sets Days.
    /// </summary>
    public int Days
    {
      get
      {
        return this._days;
      }

      set
      {
        this._days = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether PermDelete.
    /// </summary>
    public bool PermDelete
    {
      get
      {
        return this._permDelete;
      }

      set
      {
        this._permDelete = value;
      }
    }

    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="days">
    /// The days.
    /// </param>
    /// <param name="permDelete">
    /// The perm delete.
    /// </param>
    /// <returns>
    /// The start.
    /// </returns>
    public static bool Start(int boardId, int forumId, int days, bool permDelete)
    {
      if (YafContext.Current.Get<ITaskModuleManager>() == null)
      {
        return false;
      }

    	YafContext.Current.Get<ITaskModuleManager>().StartTask(
    		TaskName, () => new PruneTopicTask { Data = boardId, ForumId = forumId, Days = days, PermDelete = permDelete });

      return true;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      try
      {
        this.Logger.Info("Starting Prune Task for ForumID {0}, {1} Days, Perm Delete {2}.", this.ForumId, this.Days, this.PermDelete);

        int count = LegacyDb.topic_prune((int)this.Data, this.ForumId, this.Days, this.PermDelete);

        this.Logger.Info("Prune Task Complete. Pruned {0} topics.", count);
      }
      catch (Exception x)
      {
        this.Logger.Error(x, "Error In Prune Topic Task: {0}", x);
      }
    }
  }
}