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
using System;
using System.Threading;
using System.Web;

namespace YAF.Classes.Core
{
  /// <summary>
  /// The i background task.
  /// </summary>
  public interface IBackgroundTask : IDisposable
  {
    /// <summary>
    /// Sets BoardID.
    /// </summary>
    int BoardID
    {
      set;
    }

    /// <summary>
    /// Gets Started.
    /// </summary>
    DateTime Started
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether IsRunning.
    /// </summary>
    bool IsRunning
    {
      get;
    }


    /// <summary>
    /// Sets AppContext.
    /// </summary>
    HttpApplication AppContext
    {
      set;
    }

    /// <summary>
    /// The run.
    /// </summary>
    void Run();
  }

  /// <summary>
  /// The base background task.
  /// </summary>
  public class BaseBackgroundTask : IBackgroundTask
  {
    /// <summary>
    /// The _app context.
    /// </summary>
    protected HttpApplication _appContext = null;

    /// <summary>
    /// The _board id.
    /// </summary>
    protected int _boardId = YafControlSettings.Current.BoardID;

    /// <summary>
    /// The _is running.
    /// </summary>
    protected bool _isRunning = false;

    /// <summary>
    /// The _lock object.
    /// </summary>
    protected object _lockObject = new object();

    /// <summary>
    /// The _started.
    /// </summary>
    protected DateTime _started;

    #region IBackgroundTask Members

    /// <summary>
    /// Gets or sets BoardID.
    /// </summary>
    public virtual int BoardID
    {
      protected get
      {
        return this._boardId;
      }

      set
      {
        this._boardId = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsRunning.
    /// </summary>
    public virtual bool IsRunning
    {
      get
      {
        lock (this._lockObject)
        {
          return this._isRunning;
        }
      }

      protected set
      {
        lock (this._lockObject)
        {
          if (!this._isRunning && value)
          {
            this._started = DateTime.Now;
          }

          this._isRunning = value;
        }
      }
    }

    /// <summary>
    /// Gets Started.
    /// </summary>
    public virtual DateTime Started
    {
      get
      {
        return this._started;
      }
    }

    /// <summary>
    /// Gets or sets AppContext.
    /// </summary>
    public virtual HttpApplication AppContext
    {
      protected get
      {
        return this._appContext;
      }

      set
      {
        this._appContext = value;
      }
    }

    /// <summary>
    /// The run.
    /// </summary>
    public virtual void Run()
    {
      YafContext.Application = AppContext.Application;

      IsRunning = true;

      RunOnce();

      IsRunning = false;
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public virtual void Dispose()
    {
      IsRunning = false;
    }

    #endregion

    /// <summary>
    /// The run once.
    /// </summary>
    public virtual void RunOnce()
    {
      // run once code
    }
  }

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
    /// The _intermittent timer semaphore.
    /// </summary>
    protected object _intermittentTimerSemaphore = new object();

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
      base.RunOnce();
    }

    /// <summary>
    /// The run.
    /// </summary>
    public override void Run()
    {
      if (!IsRunning)
      {
        YafContext.Application = AppContext.Application;

        // we're running this thread now...
        IsRunning = true;

        // create the timer...
        this._intermittentTimer = new Timer(new TimerCallback(TimerCallback), null, StartDelayMs, RunPeriodMs);
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
      lock (this._intermittentTimerSemaphore)
      {
        RunOnce();
      }
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public override void Dispose()
    {
      this._intermittentTimer.Dispose();
      base.Dispose();
    }
  }

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
      StartDelayMs = 50;
      RunPeriodMs = Timeout.Infinite;
    }

    /// <summary>
    /// The timer callback.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    protected override void TimerCallback(object sender)
    {
      lock (_intermittentTimerSemaphore)
      {
        // we're done with this timer...
        _intermittentTimer.Dispose();

        // run this item once...
        RunOnce();

        // no longer running when we get here...
        IsRunning = false;
      }
    }
  }
}