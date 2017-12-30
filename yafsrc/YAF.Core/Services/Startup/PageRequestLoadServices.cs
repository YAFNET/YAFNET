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
namespace YAF.Core.Services.Startup
{
  #region Using

    using System;
    using System.Web.UI;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The page request load services.
  /// </summary>
  [ExportService(ServiceLifetimeScope.OwnedByContainer)]
  public class PageRequestLoadServices : IHandleEvent<EventPreRequestPageExecute>, IHaveServiceLocator
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRequestLoadServices"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public PageRequestLoadServices([NotNull] IServiceLocator serviceLocator)
    {
      CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

      this.ServiceLocator = serviceLocator;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order
    {
      get
      {
        return 10;
      }
    }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets or sets RequestPage.
    /// </summary>
    protected Page RequestPage { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<EventPreRequestPageExecute>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] EventPreRequestPageExecute @event)
    {
      this.RequestPage = @event.RequestedPage;
      this.RequestPage.PreInit += this.RequestedPage_PreInit;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The requested page_ pre init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    private void RequestedPage_PreInit([NotNull] object sender, [NotNull] EventArgs e)
    {
      // only run startup services for pages that require it.
      if (this.RequestPage.HasInterface<IRequireStartupServices>())
      {
        // run startup services...
        this.RunStartupServices();
      }
    }

    #endregion
  }
}