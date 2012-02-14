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
  using YAF.Types.Interfaces;

  /// <summary>
  /// Automatically cleans up the tasks if they are no longer running...
  /// </summary>
  public class CleanUpTask : IntermittentBackgroundTask, ICriticalBackgroundTask
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CleanUpTask"/> class.
    /// </summary>
    public CleanUpTask()
    {
      // set interval values...
      this.RunPeriodMs = 500;
      this.StartDelayMs = 500;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets TaskManager.
    /// </summary>
    public ITaskModuleManager TaskManager { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      // look for tasks to clean up...
      if (this.TaskManager != null)
      {
        // make collection local...
        var taskListKeys = this.TaskManager.TaskManagerInstances;

        foreach (string instanceName in taskListKeys)
        {
          IBackgroundTask task = this.TaskManager.TryGetTask(instanceName);

          if (task == null)
          {
            this.TaskManager.TryRemoveTask(instanceName);
          }
          else if (!task.IsRunning)
          {
            this.TaskManager.TryRemoveTask(instanceName);
            task.Dispose();
          }
        }
      }
    }

    #endregion
  }
}