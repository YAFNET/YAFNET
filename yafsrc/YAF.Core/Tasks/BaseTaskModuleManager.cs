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
namespace YAF.Core
{
  #region Using

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

    /// <summary>
    ///   The _lock task manager object.
    /// </summary>
    protected static object _lockTaskManagerObject = new object();

    /// <summary>
    ///   The _task manager.
    /// </summary>
    protected static Dictionary<string, IBackgroundTask> _taskManager = new Dictionary<string, IBackgroundTask>();

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
    ///   Current Page Instance of the Module Manager
    /// </summary>
    public virtual Dictionary<string, IBackgroundTask> TaskManager
    {
      get
      {
        return _taskManager;
      }
    }

    /// <summary>
    ///   All the names of tasks running.
    /// </summary>
    [NotNull]
    public virtual List<string> TaskManagerInstances
    {
      get
      {
        lock (_lockTaskManagerObject)
        {
          return this.TaskManager.Keys.ToList();
        }
      }
    }

    /// <summary>
    ///   Gets TaskManagerSnapshot.
    /// </summary>
    [NotNull]
    public virtual Dictionary<string, IBackgroundTask> TaskManagerSnapshot
    {
      get
      {
        var tasks = new Dictionary<string, IBackgroundTask>();

        lock (_lockTaskManagerObject)
        {
          _taskManager.ToList().ForEach(x => tasks.Add(x.Key, x.Value));
        }

        return tasks;
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
      lock (_lockTaskManagerObject)
      {
        if (this.TaskManager.ContainsKey(instanceName) && this.TaskManager[instanceName].IsRunning)
        {
          return true;
        }
      }

      return false;
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
    public abstract void StartTask([NotNull] string instanceName, [NotNull] IBackgroundTask start);

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
      lock (_lockTaskManagerObject)
      {
        return this.TaskManager.ContainsKey(instanceName);
      }
    }

    /// <summary>
    /// Attempt to get the instance of the task.
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    /// <returns>
    /// </returns>
    public virtual IBackgroundTask TryGetTask([NotNull] string instanceName)
    {
      lock (_lockTaskManagerObject)
      {
        if (this.TaskManager.ContainsKey(instanceName))
        {
          return this.TaskManager[instanceName];
        }
      }

      return null;
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
      lock (_lockTaskManagerObject)
      {
        if (this.TaskManager.ContainsKey(instanceName))
        {
          this.TaskManager.Remove(instanceName);
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Stops a task from running if it's not critical
    /// </summary>
    /// <param name="instanceName">
    /// </param>
    public virtual void StopTask([NotNull] string instanceName)
    {
      lock (_lockTaskManagerObject)
      {
        var task = this.TryGetTask(instanceName);

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

    #region Methods

    /// <summary>
    /// The add task.
    /// </summary>
    /// <param name="instanceName">
    /// The instance name.
    /// </param>
    /// <param name="newTask">
    /// The new task.
    /// </param>
    protected virtual void AddTask([NotNull] string instanceName, [NotNull] IBackgroundTask newTask)
    {
      lock (_lockTaskManagerObject)
      {
        if (!this.TaskManager.ContainsKey(instanceName))
        {
          this.TaskManager.Add(instanceName, newTask);
        }
        else
        {
          this.TaskManager[instanceName] = newTask;
        }
      }
    }

    #endregion
  }
}