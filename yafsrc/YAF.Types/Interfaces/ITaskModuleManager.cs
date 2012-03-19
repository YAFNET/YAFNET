/* YetAnotherForum.NET
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
namespace YAF.Types.Interfaces
{
  #region Using

	using System;
	using System.Collections.Generic;
  using System.Web;

  #endregion

  /// <summary>
  /// The i task module manager.
  /// </summary>
  public interface ITaskModuleManager
  {
    #region Properties

    /// <summary>
    ///   Gets TaskCount.
    /// </summary>
    int TaskCount { get; }

    /// <summary>
    ///   All the names of tasks running.
    /// </summary>
    IList<string> TaskManagerInstances { get; }

    /// <summary>
    ///   Gets TaskManagerSnapshot.
    /// </summary>
		IDictionary<string, IBackgroundTask> TaskManagerSnapshot { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Check if a Task is Running.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// The is task running.
    /// </returns>
    bool IsTaskRunning([NotNull] string instanceName);

    /// <summary>
    /// Check if Tasks are Running.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// The are tasks running.
    /// </returns>
    bool AreTasksRunning([NotNull] string[] instanceName);

    /// <summary>
    /// Start a non-running task -- will set the <see cref="HttpApplication"/> instance.
    /// </summary>
    /// <param name="instanceName">
    /// Unique name of this task
    /// </param>
    /// <param name="start">
    /// Task to run
    /// </param>
		bool StartTask([NotNull] string instanceName, Func<IBackgroundTask> startTask);

    /// <summary>
    /// The stop task.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name.
    /// </param>
    void StopTask([NotNull] string instanceName);

    /// <summary>
    /// Attempt to get the instance of the task.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// </returns>
    bool TryGetTask([NotNull] string instanceName, out IBackgroundTask task);

    /// <summary>
    /// The try remove task.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name.
    /// </param>
    /// <returns>
    /// The try remove task.
    /// </returns>
    bool TryRemoveTask([NotNull] string instanceName);

    #endregion
  }
}