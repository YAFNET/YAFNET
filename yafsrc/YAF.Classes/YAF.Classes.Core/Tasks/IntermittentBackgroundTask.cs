/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System;
  using System.Threading;

  /// <summary>
  /// The intermittent background task.
  /// </summary>
  public class IntermittentBackgroundTask : BaseBackgroundTask
  {
    /// <summary>
    /// The _intermittent timer.
    /// </summary>
    protected Timer _intermittentTimer = null;

    /// <summary>
    /// The _run period ms.
    /// </summary>
    private long _runPeriodMs;

    /// <summary>
    /// The _start delay ms.
    /// </summary>
    private long _startDelayMs;

    /// <summary>
    /// Gets or sets StartDelayMs.
    /// </summary>
    public long StartDelayMs
    {
      get
      {
        return this._startDelayMs;
      }

      set
      {
        this._startDelayMs = value;
      }
    }

    /// <summary>
    /// Gets or sets RunPeriodMs.
    /// </summary>
    public long RunPeriodMs
    {
      get
      {
        return this._runPeriodMs;
      }

      set
      {
        this._runPeriodMs = value;
      }
    }


    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {

    }

    /// <summary>
    /// The run.
    /// </summary>
    public override void Run()
    {
      if (!this.IsRunning)
      {
        YafContext.Application = this.AppContext.Application;

        // we're running this thread now...
        this.IsRunning = true;

        // create the timer...
        this._intermittentTimer = new Timer(new TimerCallback(this.TimerCallback), null, this.StartDelayMs, this.RunPeriodMs);
      }
    }

    /// <summary>
    /// The timer callback.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    protected virtual void TimerCallback(object sender)
    {
      if (Monitor.TryEnter(this))
      {
        try
        {
          Monitor.Enter(this);
          this.RunOnce();
        }
        finally
        {
          Monitor.Exit(this);
        }
      }
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public override void Dispose()
    {
      base.Dispose();
    }
  }
}