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
  using System.Linq;
  using System.Reflection;

  using Autofac;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The auto fac service locator provider.
  /// </summary>
  public class AutoFacServiceLocatorProvider : IServiceLocator, IInjectServices
  {
    #region Constants and Fields

    /// <summary>
    /// The default flags.
    /// </summary>
    private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoFacServiceLocatorProvider"/> class.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    public AutoFacServiceLocatorProvider([NotNull] ILifetimeScope container)
    {
      CodeContracts.ArgumentNotNull(container, "container");

      this.Container = container;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets Container.
    /// </summary>
    public ILifetimeScope Container { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IInjectServices

    /// <summary>
    /// Inject an object with services.
    /// </summary>
    /// <typeparam name="TAttribute">
    /// TAttribute is the attribute that marks properties to inject to.
    /// </typeparam>
    /// <param name="instance">
    /// the object to inject
    /// </param>
    public void InjectMarked<TAttribute>(object instance) where TAttribute : Attribute
    {
      CodeContracts.ArgumentNotNull(instance, "instance");

      var type = instance.GetType();

      var properties =
        type.GetProperties(DefaultFlags).Where(
          p => Attribute.IsDefined(p, typeof(TAttribute)) && p.GetSetMethod(false) != null).ToList();

      foreach (var injectProp in properties)
      {
        var serviceInstance = this.Container.Resolve(injectProp.PropertyType);
        injectProp.SetValue(instance, serviceInstance, null);
      }
    }

    #endregion

    #region IServiceLocator

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="serviceType">
    /// The service type.
    /// </param>
    /// <returns>
    /// The get.
    /// </returns>
    public object Get(Type serviceType)
    {
      CodeContracts.ArgumentNotNull(serviceType, "serviceType");

      return this.Container.Resolve(serviceType);
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="serviceType">
    /// The service type.
    /// </param>
    /// <param name="named">
    /// The named.
    /// </param>
    /// <returns>
    /// The get.
    /// </returns>
    public object Get(Type serviceType, string named)
    {
      CodeContracts.ArgumentNotNull(serviceType, "serviceType");
      CodeContracts.ArgumentNotNull(named, "named");

      return this.Container.ResolveNamed(named, serviceType);
    }

    /// <summary>
    /// The try get.
    /// </summary>
    /// <param name="serviceType">
    /// The service type.
    /// </param>
    /// <param name="instance">
    /// The instance.
    /// </param>
    /// <returns>
    /// The try get.
    /// </returns>
    public bool TryGet(Type serviceType, [NotNull] out object instance)
    {
      CodeContracts.ArgumentNotNull(serviceType, "serviceType");

      return this.Container.TryResolve(out instance);
    }

    /// <summary>
    /// The try get.
    /// </summary>
    /// <param name="serviceType">
    /// The service type.
    /// </param>
    /// <param name="named">
    /// The named.
    /// </param>
    /// <param name="instance">
    /// The instance.
    /// </param>
    /// <returns>
    /// The try get.
    /// </returns>
    public bool TryGet(Type serviceType, string named, [NotNull] out object instance)
    {
      CodeContracts.ArgumentNotNull(serviceType, "serviceType");
      CodeContracts.ArgumentNotNull(named, "named");

      return this.Container.TryResolve(out instance);
    }

    #endregion

    #region IServiceProvider

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <returns>
    /// A service object of type <paramref name="serviceType"/>.
    ///   -or- 
    ///   null if there is no service object of type <paramref name="serviceType"/>.
    /// </returns>
    /// <param name="serviceType">
    /// An object that specifies the type of service object to get. 
    /// </param>
    /// <filterpriority>2</filterpriority>
    [CanBeNull]
    public object GetService([NotNull] Type serviceType)
    {
      object instance;

      return this.TryGet(serviceType, out instance) ? instance : null;
    }

    #endregion

    #endregion
  }
}