/* Yet Another Forum.net
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

  using Autofac;
  using Autofac.Core;

  using YAF.Types;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Instance of the Global Container... yes, a God class. It's the best way to do it, though.
  /// </summary>
  public static class GlobalContainer
  {
    #region Constants and Fields

    /// <summary>
    /// The _sync object.
    /// </summary>
    private static readonly object _syncObject = new object();

    /// <summary>
    ///   The _container.
    /// </summary>
    private static IContainer _container;

    /// <summary>
    /// Autofac external modules.
    /// </summary>
    private static List<IModule> _externalModules = new List<IModule>();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Container.
    /// </summary>
    public static IContainer Container
    {
      get
      {
        if (_container == null)
        {
          lock (_syncObject)
          {
            if (_container == null)
            {
              _container = CreateContainer();

              // immediately setup the static service locator...
              ServiceLocatorAccess.CurrentServiceProvider = _container.Resolve<IServiceLocator>();
            }
          }
        }

        return _container;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create container.
    /// </summary>
    private static IContainer CreateContainer()
    {
      var builder = new ContainerBuilder();
      builder.RegisterModule(new YafBaseContainerModule());

      // register all IModules...
      RegisterExternalModules(builder);

      return builder.Build();
    }

    /// <summary>
    /// The register external modules.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterExternalModules([NotNull] ContainerBuilder builder)
    {
      CodeContracts.ArgumentNotNull(builder, "builder");

      _externalModules.Clear();

      var moduleList =
        AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.IsSet() && a.FullName.ToLower().StartsWith("yaf"))
          .ToList();

      // make sure we don't include this assembly -- otherwise we'll have a recusive situation.
      moduleList.Remove(Assembly.GetExecutingAssembly());

      // little bit of filtering...
      moduleList.OrderByDescending(x => x.GetAssemblySortOrder());

      // TODO: create real abstracted plugin model. This is a stop-gap.
      var modules = moduleList.FindModules<IModule>();

      // create module instances...
      modules.ForEach(mi => _externalModules.Add(Activator.CreateInstance(mi) as IModule));

      _externalModules.ForEach(m => builder.RegisterModule(m));

    }

    #endregion
  }
}