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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Autofac;
    using Autofac.Core;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    using NamedParameter = YAF.Types.NamedParameter;
    using TypedParameter = YAF.Types.TypedParameter;

    #endregion

    /// <summary>
    ///     The auto fac service locator provider.
    /// </summary>
    public class AutoFacServiceLocatorProvider : IScopeServiceLocator, IInjectServices
    {
        #region Constants

        /// <summary>
        ///     The default flags.
        /// </summary>
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The _injection cache.
        /// </summary>
        private static readonly ConcurrentDictionary<KeyValuePair<Type, Type>, IList<Tuple<Type, Type, Action<object, object>>>> _injectionCache =
            new ConcurrentDictionary<KeyValuePair<Type, Type>, IList<Tuple<Type, Type, Action<object, object>>>>();

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
            CodeContracts.VerifyNotNull(container, "container");

            this.Container = container;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets Container.
        /// </summary>
        public ILifetimeScope Container { get; set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        public object Tag
        {
            get
            {
                return this.Container.Tag;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create scope.
        /// </summary>
        /// <returns>
        ///     The <see cref="IScopeServiceLocator" />.
        /// </returns>
        public IScopeServiceLocator CreateScope(object tag = null)
        {
            var newLifetime =
                this.Container.BeginLifetimeScope(
                    tag,
                    builder => builder.Register(c => c.Resolve<AutoFacServiceLocatorProvider>()).As<IScopeServiceLocator>().ExternallyOwned());

            return newLifetime.Resolve<IScopeServiceLocator>();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Container.Dispose();
        }

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
            CodeContracts.VerifyNotNull(serviceType, "serviceType");

            return this.Container.Resolve(serviceType);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The get.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// <c>NotSupportedException</c>.
        /// </exception>
        public object Get(Type serviceType, IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.VerifyNotNull(serviceType, "serviceType");
            CodeContracts.VerifyNotNull(parameters, "parameters");

            return this.Container.Resolve(serviceType, ConvertToAutofacParameters(parameters));
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
            CodeContracts.VerifyNotNull(serviceType, "serviceType");
            CodeContracts.VerifyNotNull(named, "named");

            return this.Container.ResolveNamed(named, serviceType);
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
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The get.
        /// </returns>
        public object Get(Type serviceType, string named, IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.VerifyNotNull(serviceType, "serviceType");
            CodeContracts.VerifyNotNull(named, "named");
            CodeContracts.VerifyNotNull(parameters, "parameters");

            return this.Container.ResolveNamed(named, serviceType, ConvertToAutofacParameters(parameters));
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        ///     -or-
        ///     null if there is no service object of type <paramref name="serviceType"/>.
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
            CodeContracts.VerifyNotNull(instance, "instance");

            // Container.InjectUnsetProperties(instance);
            var type = instance.GetType();
            var attributeType = typeof(TAttribute);

            var keyPair = new KeyValuePair<Type, Type>(type, attributeType);

            IList<Tuple<Type, Type, Action<object, object>>> properties;

            if (!_injectionCache.TryGetValue(keyPair, out properties))
            {
                // find them...
                properties =
                    type.GetProperties(DefaultFlags)
                        .Where(p => p.GetSetMethod(false) != null && !p.GetIndexParameters().Any() && p.IsDefined(attributeType, true))
                        .Select(p => Tuple.Create(p.PropertyType, p.DeclaringType, new Action<object, object>((i, v) => p.SetValue(i, v, null))))
                        .ToList();

                _injectionCache.AddOrUpdate(keyPair, k => properties, (k, v) => properties);
            }

            foreach (var injectProp in properties)
            {
                object serviceInstance = injectProp.Item1 == typeof(ILogger)
                                             ? this.Container.Resolve<ILoggerProvider>().Create(injectProp.Item2)
                                             : this.Container.Resolve(injectProp.Item1);

                // set value is super slow... best not to use it very much.
                injectProp.Item3(instance, serviceInstance);
            }
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
            CodeContracts.VerifyNotNull(serviceType, "serviceType");

            return this.Container.TryResolve(serviceType, out instance);
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
            CodeContracts.VerifyNotNull(serviceType, "serviceType");
            CodeContracts.VerifyNotNull(named, "named");

            return this.Container.TryResolveNamed(named, serviceType, out instance);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert to autofac parameters.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// <c>NotSupportedException</c>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Parameter Type of is not supported.
        /// </exception>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [NotNull]
        private static IEnumerable<Parameter> ConvertToAutofacParameters(
            [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.VerifyNotNull(parameters, "parameters");

            var autoParams = new List<Parameter>();

            foreach (var parameter in parameters)
            {
                if (parameter is NamedParameter)
                {
                    var param = parameter as NamedParameter;
                    autoParams.Add(new Autofac.NamedParameter(param.Name, param.Value));
                }
                else if (parameter is TypedParameter)
                {
                    var param = parameter as TypedParameter;
                    autoParams.Add(new Autofac.TypedParameter(param.Type, param.Value));
                }
                else
                {
                    throw new NotSupportedException("Parameter Type of {0} is not supported.".FormatWith(parameter.GetType()));
                }
            }

            return autoParams;
        }

        #endregion
    }
}