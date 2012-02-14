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
  using System.Security.Principal;
  using System.Threading;

  using YAF.Core;

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

    private WindowsIdentity _primaryThreadIdentity;

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
        // keep the context...
        this._primaryThreadIdentity = WindowsIdentity.GetCurrent();

        // we're running this thread now...
        this.IsRunning = true;

        this.Logger.Debug("Starting Background Task {0} Now", this.GetType().Name);

        // create the timer...);
        this._intermittentTimer = new Timer(this.TimerCallback, null, this.StartDelayMs, this.RunPeriodMs);
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
        WindowsImpersonationContext impersonationContext = null;

        if (this._primaryThreadIdentity != null)
        {
          impersonationContext = this._primaryThreadIdentity.Impersonate();
        }

        try
        {
          this.RunOnce();
        }
        finally
        {
          Monitor.Exit(this);

          if (impersonationContext != null)
          {
            impersonationContext.Undo();
          }
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