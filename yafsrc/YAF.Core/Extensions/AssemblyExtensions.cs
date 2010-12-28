/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The assembly extensions.
  /// </summary>
  public static class AssemblyExtensions
  {
    #region Public Methods

    [NotNull]
    public static IEnumerable<Type> FindModules<T>([NotNull] this IEnumerable<Assembly> assemblies)
    {
      CodeContracts.ArgumentNotNull(assemblies, "assemblies");

      var moduleClassTypes = new List<Type>();
      var implementedInterfaceType = typeof(T);

      // get classes...
      foreach (
        var types in
          assemblies.Select(
            a =>
            a.GetExportedTypes().Where(t => !t.IsAbstract).ToList()))
      {
        moduleClassTypes.AddRange(
          types.Where(modClass => modClass.GetInterfaces().Any(i => i.Equals(implementedInterfaceType))));
      }

      return moduleClassTypes.Distinct();
    }

    /// <summary>
    /// The find modules.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    /// <param name="namespaceName">
    /// The module namespace.
    /// </param>
    /// <param name="implementedInterfaceName">
    /// The module base interface.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static IEnumerable<Type> FindModules(
      [NotNull] this IEnumerable<Assembly> assemblies,
      [NotNull] string namespaceName,
      [NotNull] string implementedInterfaceName)
    {
      CodeContracts.ArgumentNotNull(assemblies, "assemblies");
      CodeContracts.ArgumentNotNull(namespaceName, "namespaceName");
      CodeContracts.ArgumentNotNull(implementedInterfaceName, "implementedInterfaceName");

      var moduleClassTypes = new List<Type>();
      var implementedInterfaceType = Type.GetType(implementedInterfaceName);

      // get classes...
      foreach (
        var types in
          assemblies.OfType<Assembly>().Select(
            a =>
            a.GetExportedTypes().Where(t => t.Namespace != null && !t.IsAbstract && t.Namespace.Equals(namespaceName))
              .ToList()))
      {
        moduleClassTypes.AddRange(types.Where(modClass => modClass.GetInterfaces().Any(i => i.Equals(implementedInterfaceType))));
      }

      return moduleClassTypes.Distinct();
    }

    #endregion
  }
}