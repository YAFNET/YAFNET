/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Tasks
{
  using System;
  using System.Web;

  using YAF.Classes;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;

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