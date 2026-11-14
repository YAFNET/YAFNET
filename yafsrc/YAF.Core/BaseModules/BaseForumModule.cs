/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
namespace YAF.Core.BaseModules;

/// <summary>
/// The base forum module.
/// </summary>
#pragma warning disable S3881 // the public virtual Dispose is overridden by the modules, changing it would break them
public abstract class BaseForumModule : IBaseForumModule, IHaveServiceLocator, IHaveLocalization
#pragma warning restore S3881
{
    /// <summary>
    ///   Gets a value indicating whether Active.
    /// </summary>
    public virtual bool Active => true;

    /// <summary>
    ///   Gets Description.
    /// </summary>
    public virtual string Description => this.GetType().GetAttribute<Module>().ModuleName;

    /// <summary>
    ///   Gets or sets Forum Control Object.
    /// </summary>
    public virtual object ForumControlObj { get; set; }

    /// <summary>
    ///   Gets ModuleId.
    /// </summary>
    public virtual string ModuleId => this.Description.GetHashCode().ToString();

    /// <summary>
    /// Gets PageBoardContext.
    /// </summary>
#pragma warning disable S2325 // public API, used as this.PageBoardContext everywhere
    public BoardContext PageBoardContext => BoardContext.Current;
#pragma warning restore S2325

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public virtual IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public virtual ILocalization Localization => this.Get<ILocalization>();

    /// <summary>
    /// The initialization function.
    /// </summary>
    public virtual void Init()
    {
        // do nothing... 
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public virtual void Dispose()
    {
        // no default implementation
    }
}