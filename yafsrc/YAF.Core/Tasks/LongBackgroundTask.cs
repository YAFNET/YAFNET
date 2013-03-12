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
  using System.Threading;

  /// <summary>
  /// The long background task.
  /// </summary>
  public class LongBackgroundTask : IntermittentBackgroundTask
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LongBackgroundTask"/> class.
    /// </summary>
    public LongBackgroundTask()
    {
      this.StartDelayMs = 50;
      this.RunPeriodMs = Timeout.Infinite;
    }

    /// <summary>
    /// The timer callback.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    protected override void TimerCallback(object sender)
    {
      lock (this)
      {
        // we're done with this timer...
        this._intermittentTimer.Dispose();

        // run this item once...
        this.RunOnce();

        // no longer running when we get here...
        this.IsRunning = false;
      }
    }
  }
}