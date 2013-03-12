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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;

    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Utils.Helpers.StringUtils;

    /// <summary>
    /// Does some user clean up tasks such as unsuspending users...
    /// </summary>
    public class UserCleanUpTask : IntermittentBackgroundTask
    {
        /// <summary>
        /// The _task name.
        /// </summary>
        private const string _taskName = "UserCleanUpTask";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCleanUpTask"/> class.
        /// </summary>
        public UserCleanUpTask()
        {
            // set interval values...
            RunPeriodMs = 3600000;
            StartDelayMs = 30000;
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
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                // get all boards...
                List<int> boardIds = this.GetRepository<Board>().ListTyped().Select(x => x.ID).ToList();

                // go through each board...
                foreach (int boardId in boardIds)
                {
                    // get users for this board...
                    List<DataRow> users = LegacyDb.user_list(boardId, null, null).Rows.Cast<DataRow>().ToList();

                    // handle unsuspension...
                    var suspendedUsers = from u in users
                                         where (u["Suspended"] != DBNull.Value && (DateTime)u["Suspended"] < DateTime.UtcNow)
                                         select u;

                    // unsuspend these users...
                    foreach (var user in suspendedUsers)
                    {
                        LegacyDb.user_suspend(user["UserId"], null);

                        // sleep for a quarter of a second so we don't pound the server...
                        Thread.Sleep(250);
                    }

                    // sleep for a second...
                    Thread.Sleep(1000);
                }
            }
            catch (Exception x)
            {
                this.Logger.Error(x, "Error In {0} Task".FormatWith(TaskName));
            }
        }
    }
}