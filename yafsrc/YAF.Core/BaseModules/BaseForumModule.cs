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
namespace YAF.Core
{
  #region Using

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The base forum module.
  /// </summary>
  public abstract class BaseForumModule : IBaseForumModule, IHaveServiceLocator, IHaveLocalization
  {
    #region Properties

    /// <summary>
    ///   Gets a value indicating whether Active.
    /// </summary>
    public virtual bool Active
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    ///   Gets Description.
    /// </summary>
    public virtual string Description
    {
      get
      {
        return this.GetType().GetAttribute<YafModule>().ModuleName;
      }
    }

    /// <summary>
    ///   Gets or sets ForumControlObj.
    /// </summary>
    public virtual object ForumControlObj { get; set; }

    /// <summary>
    ///   Gets ModuleId.
    /// </summary>
    [NotNull]
    public virtual string ModuleId
    {
      get
      {
        return this.Description.GetHashCode().ToString();
      }
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        return YafContext.Current;
      }
    }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public virtual IServiceLocator ServiceLocator
    {
      get
      {
        return YafContext.Current.ServiceLocator;
      }
    }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public virtual ILocalization Localization
    {
      get
      {
        return this.Get<ILocalization>();
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IBaseForumModule

    /// <summary>
    /// The initialization function.
    /// </summary>
    public virtual void Init()
    {
      // do nothing... 
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public virtual void Dispose()
    {
      // no default implementation
    }

    #endregion

    #endregion
  }
}