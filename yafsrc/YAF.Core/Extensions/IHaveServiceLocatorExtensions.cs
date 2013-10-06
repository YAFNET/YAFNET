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

  using System.Collections.Generic;
  using System.Linq;

  using YAF.Core.Tasks;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The i have service locator extensions.
  /// </summary>
  public static class IHaveServiceLocatorExtensions
  {
    #region Public Methods

    /// <summary>
    /// The run startup services.
    /// </summary>
    /// <param name="serviceLocator">
    /// The instnace that has a service locator.
    /// </param>
    public static void RunStartupServices([NotNull] this IHaveServiceLocator serviceLocator)
    {
      CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

      var startupServices =
        serviceLocator.Get<IEnumerable<IStartupService>>();

      // run critical first...
      startupServices.Where(s => s.HasInterface<ICriticalStartupService>()).OrderByDescending(i => i.Priority).ForEach(
        service => service.Run());

      // run secondary...
      startupServices.Where(s => !s.HasInterface<ICriticalStartupService>()).OrderByDescending(i => i.Priority).ForEach(
        service => service.Run());
    }

    /// <summary>
    /// The with parameters.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IEnumerable<IServiceLocationParameter> WithParameters([NotNull] this IHaveServiceLocator serviceLocator, [NotNull] params IServiceLocationParameter[] parameters)
    {
      CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
      CodeContracts.VerifyNotNull(parameters, "parameters");

      return parameters;
    }

    #endregion
  }
}