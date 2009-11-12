/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
namespace YAF.Classes.Core
{
  /// <summary>
  /// Automatically cleans up the tasks if they are no longer running...
  /// </summary>
  public class CleanUpTask : IntermittentBackgroundTask
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CleanUpTask"/> class.
    /// </summary>
    public CleanUpTask()
    {
      // set interval values...
      RunPeriodMs = 500;
      StartDelayMs = 500;
    }

    /// <summary>
    /// Gets or sets Module.
    /// </summary>
    public YafTaskModule Module
    {
      get;
      set;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      // look for tasks to clean up...
      if (Module != null)
      {
        // make collection local...
        var taskListKeys = Module.TaskManagerInstances;

        foreach (string instanceName in taskListKeys)
        {
          IBackgroundTask task = Module.TryGetTask(instanceName);

          if (task == null)
          {
            Module.TryRemoveTask(instanceName);
          }
          else if (!task.IsRunning)
          {
            Module.TryRemoveTask(instanceName);
            task.Dispose();
          }
        }
      }
    }
  }
}