/* Yet Another Forum.NET
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
namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The i task module manager extensions.
  /// </summary>
  public static class ITaskModuleManagerExtensions
  {
    #region Public Methods

    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="taskModuleManager">
    /// The task module manager.
    /// </param>
    /// <param name="createTask">
    /// The create task.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// The start.
    /// </returns>
    public static bool Start<T>([NotNull] this ITaskModuleManager taskModuleManager, [NotNull] Func<T> createTask) where T : IBackgroundTask
    {
      CodeContracts.VerifyNotNull(taskModuleManager, "taskModuleManager");
      CodeContracts.VerifyNotNull(createTask, "createTask");

      string taskName = typeof(T).ToString();

    	return taskModuleManager.StartTask(taskName, () => createTask());
    }

    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="taskModuleManager">
    /// The task module manager.
    /// </param>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// The start.
    /// </returns>
    public static bool Start<T>([NotNull] this ITaskModuleManager taskModuleManager, [CanBeNull] object data) where T : IBackgroundTask, new()
    {
      CodeContracts.VerifyNotNull(taskModuleManager, "taskModuleManager");

      return Start(taskModuleManager, () => new T { Data = data });
    }

    #endregion
  }
}