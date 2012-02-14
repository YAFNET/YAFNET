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
  using System;
  using System.Web;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces; using YAF.Types.Constants;

  /// <summary>
  /// The base background task.
  /// </summary>
  public abstract class BaseBackgroundTask : IBackgroundTask, IHaveServiceLocator
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
    public virtual object Data
    {
      protected get
      {
        return this._boardId;
      }

      set
      {
        this._boardId = (int)value;
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
            this._started = DateTime.UtcNow;
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
    /// The run.
    /// </summary>
    public virtual void Run()
    {
      this.IsRunning = true;

      this.RunOnce();

      this.IsRunning = false;
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public virtual void Dispose()
    {
      this.IsRunning = false;
    }

    #endregion

    /// <summary>
    /// The run once.
    /// </summary>
    public abstract void RunOnce();

    #region Implementation of IHaveServiceLocator

    /// <summary>
    /// Gets ServiceLocator.
    /// </summary>
    [Inject]
    public IServiceLocator ServiceLocator { get; set; }

    [Inject]
    public ILogger Logger { get; set; }

    #endregion
  }
}