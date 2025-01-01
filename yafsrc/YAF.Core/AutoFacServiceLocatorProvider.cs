/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

using Autofac;
using Autofac.Core;

using NamedParameter = YAF.Types.Objects.NamedParameter;
using TypedParameter = YAF.Types.Objects.TypedParameter;

/// <summary>
///     The AutoFac service locator provider.
/// </summary>
public class AutoFacServiceLocatorProvider : IScopeServiceLocator, IInjectServices
{
    /// <summary>
    ///     The default flags.
    /// </summary>
    private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance;

    /// <summary>
    ///     The _injection cache.
    /// </summary>
    private readonly static
        ConcurrentDictionary<KeyValuePair<Type, Type>, IList<Tuple<Type, Type, Action<object, object>>>>
        InjectionCache =
            new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoFacServiceLocatorProvider"/> class.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    public AutoFacServiceLocatorProvider(ILifetimeScope container)
    {
        this.Container = container;
    }

    /// <summary>
    ///     Gets or sets Container.
    /// </summary>
    public ILifetimeScope Container { get; set; }

    /// <summary>
    /// Gets the tag.
    /// </summary>
    public object Tag => this.Container.Tag;

    /// <summary>
    /// The create scope.
    /// </summary>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The <see cref="IScopeServiceLocator"/>.
    /// </returns>
    public IScopeServiceLocator CreateScope(object tag = null)
    {
        var newLifetime = this.Container.BeginLifetimeScope(
            tag,
            builder => builder.Register(c => c.Resolve<AutoFacServiceLocatorProvider>()).As<IScopeServiceLocator>()
                .ExternallyOwned());

        return newLifetime.Resolve<IScopeServiceLocator>();
    }

    /// <summary>
    ///     The dispose.
    /// </summary>
    public void Dispose()
    {
#if DEBUG
        var diagnosticsLib =
            Assembly.Load(
                "System.Diagnostics.DiagnosticSource, Version=8.0.0.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");
        var diagnosticSourceEventSourceType = diagnosticsLib.GetType("System.Diagnostics.DiagnosticSourceEventSource");
        var diagnosticSourceEventSource = diagnosticSourceEventSourceType.InvokeMember("Log",
            BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public, null, null, null);

        if (diagnosticSourceEventSource is IDisposable disposable)
        {
            disposable.Dispose();
        }
#endif

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
    public object GetService(Type serviceType)
    {
        return this.TryGet(serviceType, out var instance) ? instance : null;
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
        // Container.InjectUnsetProperties(instance);
        var type = instance.GetType();
        var attributeType = typeof(TAttribute);

        var keyPair = new KeyValuePair<Type, Type>(type, attributeType);

        if (!InjectionCache.TryGetValue(keyPair, out var properties))
        {
            // find them...
            properties =
                type.GetProperties(DefaultFlags)
                    .Where(p => p.GetSetMethod(false) != null && !p.GetIndexParameters().Any() &&
                                p.IsDefined(attributeType, true))
                    .Select(p => Tuple.Create(p.PropertyType, p.DeclaringType,
                        new Action<object, object>((i, v) => p.SetValue(i, v, null))))
                    .ToList();

            InjectionCache.AddOrUpdate(keyPair, k => properties, (k, v) => properties);
        }

        properties.ForEach(
            injectProp =>
            {
                var serviceInstance = injectProp.Item1 == typeof(ILoggerService)
                    ? this.Container.Resolve<ILoggerProvider>().Create(injectProp.Item2)
                    : this.Container.Resolve(injectProp.Item1);

                // set value is super slow... best not to use it very much.
                injectProp.Item3(instance, serviceInstance);
            });
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
    public bool TryGet(Type serviceType, out object instance)
    {
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
    public bool TryGet(Type serviceType, string named, out object instance)
    {
        return this.Container.TryResolveNamed(named, serviceType, out instance);
    }

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
    private static IEnumerable<Parameter> ConvertToAutofacParameters(
        IEnumerable<IServiceLocationParameter> parameters)
    {
        var autoParams = new List<Parameter>();

        parameters.ForEach(
            parameter =>
            {
                switch (parameter)
                {
                    case NamedParameter param1:
                        autoParams.Add(new Autofac.NamedParameter(param1.Name, param1.Value));
                        break;
                    case TypedParameter param:
                        autoParams.Add(new Autofac.TypedParameter(param.Type, param.Value));
                        break;
                    default:
                        throw new NotSupportedException($"Parameter Type of {parameter.GetType()} is not supported.");
                }
            });

        return autoParams;
    }
}