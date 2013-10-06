/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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