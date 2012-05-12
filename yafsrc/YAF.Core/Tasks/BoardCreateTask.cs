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

namespace YAF.Core.Tasks
{
    #region Using

    using System;

    using YAF.Classes.Data;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Run when we want to do migration of users in the background...
    /// </summary>
    public class BoardCreateTask : LongBackgroundTask, ICriticalBackgroundTask, IBlockableTask
    {
        private static long _boardOut;

        /// <summary>
        /// The Blocking Task Names.
        /// </summary>
        private static readonly string[] _blockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;

        /// <summary>
        /// The _task name.
        /// </summary>
        private const string _TaskName = "BoardCreateTask";

        /// <summary>
        ///  Gets TaskName.
        /// </summary>
        public static string TaskName
        {
            get
            {
                return _TaskName;
            }
        }

        /// <summary>
        /// Gets Blocking Task Names.
        /// </summary>
        public static string[] BlockingTaskNames
        {
            get
            {
                return _blockingTaskNames;
            }
        }

        /// <summary>
        /// Gets or sets ForumOut.
        /// </summary>
        public static long BoardOut
        {
            get
            {
                return _boardOut;
            }

            set
            {
                _boardOut = value;
            }
        }

        /// <summary>
        /// Gets or sets AdminUserName.
        /// </summary>
        public object AdminUserName { get; set; }

        /// <summary>
        /// Gets or sets AdminUserEmail.
        /// </summary>
        public object AdminUserEmail { get; set; }

        /// <summary>
        /// Gets or sets AdminUserProviderUserKey.
        /// </summary>
        public object AdminUserProviderUserKey { get; set; }

        /// <summary>
        /// Gets or sets Board Name.
        /// </summary>
        public object BoardName { get; set; }

        /// <summary>
        /// Gets or sets Culture.
        /// </summary>
        public object Culture { get; set; }

        /// <summary>
        /// Gets or sets LanguageFileName.
        /// </summary>
        public object LanguageFileName { get; set; }

        /// <summary>
        /// Gets or sets BoardMembershipAppName.
        /// </summary>
        public object BoardMembershipAppName { get; set; }

        /// <summary>
        /// Gets or sets BoardRolesAppName.
        /// </summary>
        public object BoardRolesAppName { get; set; }

        /// <summary>
        /// Gets or sets RolePrefix.
        /// </summary>
        public object RolePrefix { get; set; }

        /// <summary>
        /// Starts the Board Create Task
        /// </summary>
        /// <param name="adminUserName">Name of the admin user.</param>
        /// <param name="admiUserEmail">The admi user email.</param>
        /// <param name="adminUserProviderUserkey">The admin user provider userkey.</param>
        /// <param name="boardName">Name of the board.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="languageFileName">Name of the language file.</param>
        /// <param name="boardMembershipAppName">Name of the board membership app.</param>
        /// <param name="boardRolesAppName">Name of the board roles app.</param>
        /// <param name="rolePrefix">The role prefix.</param>
        /// <param name="failureMessage">The failure message.</param>
        /// <returns>
        /// Returns the new Board ID
        /// </returns>
        public static long Start(
            object adminUserName,
            object admiUserEmail,
            object adminUserProviderUserkey,
            object boardName,
            object culture,
            object languageFileName,
            object boardMembershipAppName,
            object boardRolesAppName,
            object rolePrefix,
            out string failureMessage)
        {
            failureMessage = string.Empty;
            if (YafContext.Current.Get<ITaskModuleManager>() == null)
            {
                return 0;
            }

            if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
            {
                YafContext.Current.Get<ITaskModuleManager>().StartTask(
                    TaskName,
                    () =>
                    new BoardCreateTask
                        {
                            AdminUserName = adminUserName,
                            AdminUserEmail = admiUserEmail,
                            AdminUserProviderUserKey = adminUserProviderUserkey,
                            BoardName = boardName,
                            Culture = culture,
                            LanguageFileName = languageFileName,
                            BoardMembershipAppName = boardMembershipAppName,
                            BoardRolesAppName = boardRolesAppName,
                            RolePrefix = rolePrefix
                        });
            }
            else
            {
                failureMessage =
                    "You can't delete forum while blocking {0} tasks are running.".FormatWith(
                        BlockingTaskNames.ToDelimitedString(","));
                _boardOut = -1;
            }

            return BoardOut;
        }

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                this.Logger.Info("Starting  Board Add Task for board {0}.", _boardOut);

                // Create Board
                _boardOut = LegacyDb.board_create(
                    this.AdminUserName,
                    this.AdminUserEmail,
                    this.AdminUserProviderUserKey,
                    this.BoardName,
                    this.Culture,
                    this.LanguageFileName,
                    this.BoardMembershipAppName,
                    this.BoardRolesAppName,
                    this.RolePrefix,
                    0);
                this.Logger.Info("Board Add Task for board {0} completed.", _boardOut);
            }
            catch (Exception)
            {
                _boardOut = -1;
            }
        }
    }
}