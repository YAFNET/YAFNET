/* Yet Another Forum.net
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
namespace YAF.Core.Tasks
{
	using System;

	using YAF.Classes.Data;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	/// <summary>
	/// The forum delete task.
	/// </summary>
    public class ForumDeleteTask : LongBackgroundTask,ICriticalBackgroundTask
	{
		#region Constants and Fields

		/// <summary>
		/// The _task name.
		/// </summary>
		private const string _TaskName = "ForumDeleteTask";

		#endregion

		#region Properties

		/// <summary>
		/// Gets TaskName.
		/// </summary>
		public static string TaskName
		{
			get
			{
				return _TaskName;
			}
		}

        private static readonly string[] BlockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;
		/// <summary>
		/// Gets or sets ForumId.
		/// </summary>
		public int ForumId { get; set; }

		/// <summary>
		/// Gets or sets Forum New Id.
		/// </summary>
		public int ForumNewId { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates the Forum Delete Task
		/// </summary>
		/// <param name="boardId">
		/// The board id.
		/// </param>
		/// <param name="forumId">
		/// The forum id.
		/// </param>
		/// <returns>
		/// Returns if Task was Successfull
		/// </returns>
	    public static bool Start(int boardId, int forumId, out string failureMessage)
		{
            failureMessage = string.Empty;
			if (YafContext.Current.Get<ITaskModuleManager>() == null)
			{
				return false;
			}

            if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
			{
                YafContext.Current.Get<ITaskModuleManager>().StartTask(TaskName,
                                                                       () =>
                                                                       new ForumDeleteTask
                                                                           {
                                                                               Data = boardId,
                                                                               ForumId = forumId,
                                                                               ForumNewId = -1
                                                                           });
            }
            else
            {
                failureMessage = "You can't delete forum while blocking {0} tasks are running.".FormatWith(BlockingTaskNames.ToDelimitedString(","));
                return false;
			}

			return true;
		}

		/// <summary>
		/// Creates the Forum Delete Task and moves the Messages to a new Forum
		/// </summary>
		/// <param name="boardId">
		/// The board id.
		/// </param>
		/// <param name="forumOldId">
		/// The forum Old Id.
		/// </param>
		/// <param name="forumNewId">
		/// The Forum New Id.
		/// </param>
		/// <returns>
		/// Returns if Task was Successfull
		/// </returns>
        public static bool Start(int boardId, int forumOldId, int forumNewId, out string failureMessage)
		{
            failureMessage = string.Empty;
			if (YafContext.Current.Get<ITaskModuleManager>() == null)
			{
				return false;
			}

            if (!YafContext.Current.Get<ITaskModuleManager>().IsTaskRunning("ForumSaveTask"))
			{
			YafContext.Current.Get<ITaskModuleManager>().StartTask(TaskName, () => new ForumDeleteTask { Data = boardId, ForumId = forumOldId, ForumNewId = forumNewId });
            }
            else
            {
                failureMessage = "You can't delete forum while ForumSaveTask is running.";
                return false;
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
				if (this.ForumNewId.Equals(-1))
				{
					LegacyDb.forum_delete(this.ForumId);

					this.Logger.Info("Forum (ID: {0}) Delete Task Complete.".FormatWith(this.ForumId));
					LegacyDb.eventlog_create(
					null, TaskName, "Forum (ID: {0}) Delete Task Complete.".FormatWith(this.ForumId), 2);
				}
				else
				{
					LegacyDb.forum_move(this.ForumId, this.ForumNewId);

					LegacyDb.eventlog_create(
					null, TaskName, "Forum (ID: {0}) Delete Task Complete, and Topics has been moved to Forum (ID: {1})".FormatWith(this.ForumId, this.ForumNewId), 2);
				}
			}
			catch (Exception x)
			{
				LegacyDb.eventlog_create(
						null, TaskName, "Error In Forum (ID: {0}) Delete Task: {1}".FormatWith(this.ForumId, x));
			}
		}

		#endregion
	}
}