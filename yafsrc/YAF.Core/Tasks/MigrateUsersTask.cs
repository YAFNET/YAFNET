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

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;

  /// <summary>
  /// Run when we want to do migration of users in the background...
  /// </summary>
  public class MigrateUsersTask : LongBackgroundTask
  {
    /// <summary>
    /// The _task name.
    /// </summary>
    private const string _taskName = "MigrateUsersTask";

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
    /// The start.
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// The start.
    /// </returns>
    public static bool Start(int boardId)
    {
      if (YafTaskModule.Current == null)
      {
        return false;
      }

      if (!YafTaskModule.Current.TaskExists(TaskName))
      {
        var task = new MigrateUsersTask
          {
            BoardID = boardId
          };
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
        // attempt to run the migration code...
        RoleMembershipHelper.SyncUsers(BoardID);
      }
      catch (Exception x)
      {
        LegacyDb.eventlog_create(null, TaskName, "Error In MigrateUsers Task: {0}".FormatWith(x));
      }
    }
  }
}