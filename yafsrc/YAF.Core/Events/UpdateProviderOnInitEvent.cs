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
namespace YAF.Core.Events
{
  #region Using

  using System.Web.Security;

  using YAF.Configuration;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces.Events;

  #endregion

  /// <summary>
  /// The update provider on init event.
  /// </summary>
  [ExportService(ServiceLifetimeScope.InstancePerScope)]
  public class UpdateProviderOnInitEvent : IHandleEvent<ForumPageInitEvent>
  {
    #region Constants and Fields

    /// <summary>
    /// The _board settings.
    /// </summary>
    private readonly BoardSettings _boardSettings;

    /// <summary>
    /// The _membership provider.
    /// </summary>
    private readonly MembershipProvider _membershipProvider;

    /// <summary>
    /// The _role provider.
    /// </summary>
    private readonly RoleProvider _roleProvider;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProviderOnInitEvent"/> class.
    /// </summary>
    /// <param name="membershipProvider">
    /// The membership provider.
    /// </param>
    /// <param name="roleProvider">
    /// The role provider.
    /// </param>
    /// <param name="boardSettings">
    /// The board settings.
    /// </param>
    public UpdateProviderOnInitEvent([NotNull] MembershipProvider membershipProvider, [NotNull] RoleProvider roleProvider, [NotNull] BoardSettings boardSettings)
    {
      this._membershipProvider = membershipProvider;
      this._roleProvider = roleProvider;
      this._boardSettings = boardSettings;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 1000;

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<ForumPageInitEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] ForumPageInitEvent @event)
    {
      // initialize the providers...
      if (!this._membershipProvider.ApplicationName.Equals(this._boardSettings.MembershipAppName))
      {
        this._membershipProvider.ApplicationName = this._boardSettings.MembershipAppName;
      }

      if (!this._roleProvider.ApplicationName.Equals(this._boardSettings.RolesAppName))
      {
        this._roleProvider.ApplicationName = this._boardSettings.RolesAppName;
      }
    }

    #endregion

    #endregion
  }
}