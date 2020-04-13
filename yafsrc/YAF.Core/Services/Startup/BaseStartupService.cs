/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Services.Startup
{
  #region Using

    using System;

    using YAF.Core.Context;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The root service.
  /// </summary>
  public abstract class BaseStartupService : IStartupService
  {
    #region Properties

    /// <summary>
    ///   Gets a value indicating whether Initialized.
    /// </summary>
    public virtual bool Initialized
    {
      get
      {
        if (BoardContext.Current[this.InitVarName] == null)
        {
          return false;
        }

        return Convert.ToBoolean(BoardContext.Current[this.InitVarName]);
      }

      private set => BoardContext.Current[this.InitVarName] = value;
    }

    /// <summary>
    ///   Gets Priority.
    /// </summary>
    public virtual int Priority => 1000;

    /// <summary>
    ///   Gets InitVarName.
    /// </summary>
    protected abstract string InitVarName { get; }

    #endregion

    #region Implemented Interfaces

    #region IStartupService

    /// <summary>
    /// The run.
    /// </summary>
    public void Run()
    {
      if (!this.Initialized)
      {
        this.Initialized = this.RunService();
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The run service.
    /// </summary>
    /// <returns>
    /// The run service.
    /// </returns>
    protected abstract bool RunService();

    #endregion
  }
}