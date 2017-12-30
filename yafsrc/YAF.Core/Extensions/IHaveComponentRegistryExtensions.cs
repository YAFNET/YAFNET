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

  using System;

  using Autofac;
  using Autofac.Core;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The i have component registry extensions.
  /// </summary>
  public static class IHaveComponentRegistryExtensions
  {
    #region Public Methods

    /// <summary>
    /// Is not registered in the component registry.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsNotRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry)
    {
      CodeContracts.VerifyNotNull(haveComponentRegistry, "haveComponentRegistry");

      return !haveComponentRegistry.ComponentRegistry.IsRegistered(new TypedService(typeof(TRegistered)));
    }

    /// <summary>
    /// Is not registered in the component registry.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <param name="registeredType"></param>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsNotRegistered([NotNull] this IHaveComponentRegistry haveComponentRegistry, Type registeredType)
    {
      CodeContracts.VerifyNotNull(haveComponentRegistry, "haveComponentRegistry");

      return !haveComponentRegistry.ComponentRegistry.IsRegistered(new TypedService(registeredType));
    }

    /// <summary>
    /// The is registered.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <param name="registeredType"></param>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsRegistered([NotNull] this IHaveComponentRegistry haveComponentRegistry, Type registeredType)
    {
      CodeContracts.VerifyNotNull(haveComponentRegistry, "haveComponentRegistry");

      return haveComponentRegistry.ComponentRegistry.IsRegistered(new TypedService(registeredType));
    }

    /// <summary>
    /// The is registered.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry)
    {
      CodeContracts.VerifyNotNull(haveComponentRegistry, "haveComponentRegistry");

      return haveComponentRegistry.ComponentRegistry.IsRegistered(new TypedService(typeof(TRegistered)));
    }

    /// <summary>
    /// Is not registered in the component registry.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsNotRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry, string named)
    {
      CodeContracts.VerifyNotNull(haveComponentRegistry, "haveComponentRegistry");

      return !haveComponentRegistry.ComponentRegistry.IsRegistered(new KeyedService(named, typeof(TRegistered)));
    }

    /// <summary>
    /// The is registered.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <typeparam name="TRegistered">
    /// </typeparam>
    /// <returns>
    /// The is registered.
    /// </returns>
    public static bool IsRegistered<TRegistered>([NotNull] this IHaveComponentRegistry haveComponentRegistry, string named)
    {
      CodeContracts.VerifyNotNull(haveComponentRegistry, "haveComponentRegistry");

      return haveComponentRegistry.ComponentRegistry.IsRegistered(new KeyedService(named, typeof(TRegistered)));
    }

    /// <summary>
    /// The update.
    /// </summary>
    /// <param name="haveComponentRegistry">
    /// The have component registry.
    /// </param>
    /// <param name="containerBuilder">
    /// The container builder.
    /// </param>
    public static void UpdateRegistry(
      [NotNull] this IHaveComponentRegistry haveComponentRegistry, [NotNull] ContainerBuilder containerBuilder)
    {
      CodeContracts.VerifyNotNull(haveComponentRegistry, "haveComponentRegistry");
      CodeContracts.VerifyNotNull(containerBuilder, "containerBuilder");

      containerBuilder.Update(haveComponentRegistry.ComponentRegistry);
    }

    #endregion
  }
}