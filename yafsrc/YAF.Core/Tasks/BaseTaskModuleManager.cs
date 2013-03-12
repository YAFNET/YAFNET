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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
  using System.Linq;
  using System.Web;

  using YAF.Core.Tasks;
  using YAF.Types;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The base task module manager.
  /// </summary>
  public abstract class BaseTaskModuleManager : ITaskModuleManager
  {
    #region Constants and Fields

		protected static ConcurrentDictionary<string, IBackgroundTask> _taskManager = new ConcurrentDictionary<string, IBackgroundTask>();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets TaskCount.
    /// </summary>
    public virtual int TaskCount
    {
      get
      {
        return _taskManager.Count;
      }
    }

    /// <summary>
    ///   All the names of tasks running.
    /// </summary>
    [NotNull]
    public virtual IList<string> TaskManagerInstances
    {
      get
      {
        return _taskManager.Keys.ToList();
      }
    }

    /// <summary>
    ///   Gets TaskManagerSnapshot.
    /// </summary>
    [NotNull]
    public virtual IDictionary<string, IBackgroundTask> TaskManagerSnapshot
    {
      get
      {
      	return _taskManager.ToDictionary(k => k.Key, v => v.Value);
      }
    }

    #endregion

    #region Implemented Interfaces

    #region ITaskModuleManager

    /// <summary>
    /// Check if a Task is Running.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// The is task running.
    /// </returns>
    public virtual bool IsTaskRunning([NotNull] string instanceName)
    {
    	IBackgroundTask task;

    	return this.TryGetTask(instanceName, out task) && task.IsRunning;
    }

    /// <summary>
    /// Check if Tasks are Running.
    /// </summary>
    /// <param name="instanceNames">
    /// </param>
    /// <returns>
    /// The tasks are running.
    /// </returns>
    public virtual bool AreTasksRunning([NotNull] string[] instanceNames)
    {
        bool isRunning = false;
        foreach (var s in instanceNames)
        {
            IBackgroundTask task;
            isRunning = this.TryGetTask(s, out task) && task.IsRunning;
            if (isRunning) break;
        }

        return isRunning;

    }

  	/// <summary>
    /// Start a non-running task -- will set the <see cref="HttpApplication"/> instance.
    /// </summary>
    /// <param name="instanceName">
    /// Unique name of this task
    /// </param>
    /// <param name="start">
    /// Task to run
    /// </param>
		public abstract bool StartTask([NotNull] string instanceName, [NotNull] Func<IBackgroundTask> start);

    /// <summary>
    /// Check if a task exists in the task manager. May not be running.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// The task exists.
    /// </returns>
    public virtual bool TaskExists([NotNull] string instanceName)
    {
    	return _taskManager.ContainsKey(instanceName);
    }

    /// <summary>
    /// Attempt to get the instance of the task.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// </returns>
    public virtual bool TryGetTask([NotNull] string instanceName, out IBackgroundTask task)
    {
    	return _taskManager.TryGetValue(instanceName, out task);
    }

    /// <summary>
    /// The try remove task.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name.
    /// </param>
    /// <returns>
    /// The try remove task.
    /// </returns>
    public virtual bool TryRemoveTask([NotNull] string instanceName)
    {
			IBackgroundTask task;

    	return _taskManager.TryRemove(instanceName, out task);
    }

    /// <summary>
    /// Stops a task from running if it's not critical
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    public virtual void StopTask([NotNull] string instanceName)
    {
			IBackgroundTask task;

			if (this.TryGetTask(instanceName, out task))
			{
				if (task != null && task.IsRunning && !(task is ICriticalBackgroundTask))
				{
					if (this.TryRemoveTask(instanceName))
					{
						task.Dispose();
					}
				}
			}
    }

    #endregion

    #endregion
  }
}