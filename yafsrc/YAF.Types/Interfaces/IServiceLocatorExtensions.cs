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
namespace YAF.Types.Interfaces
{
    #region Using

    using System.Collections.Generic;

    using YAF.Types.Constants;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The i service locator extensions.
    /// </summary>
    public static class IServiceLocatorExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IServiceLocator serviceLocator)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

            return (TService)serviceLocator.Get(typeof(TService));
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IServiceLocator serviceLocator, [NotNull] string named)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
            CodeContracts.VerifyNotNull(named, "named");

            return (TService)serviceLocator.Get(typeof(TService), named);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="parameters">
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IServiceLocator serviceLocator, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
            CodeContracts.VerifyNotNull(parameters, "parameters");

            return (TService)serviceLocator.Get(typeof(TService), parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <param name="parameters">
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IServiceLocator serviceLocator, [NotNull] string named, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
            CodeContracts.VerifyNotNull(named, "named");
            CodeContracts.VerifyNotNull(parameters, "parameters");

            return (TService)serviceLocator.Get(typeof(TService), named, parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IHaveServiceLocator haveLocator)
        {
            CodeContracts.VerifyNotNull(haveLocator, "haveLocator");

            return haveLocator.ServiceLocator.Get<TService>();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator.
        /// </param>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>([NotNull] this IHaveServiceLocator haveLocator, [NotNull] string named)
        {
            CodeContracts.VerifyNotNull(haveLocator, "haveLocator");
            CodeContracts.VerifyNotNull(named, "named");

            return haveLocator.ServiceLocator.Get<TService>(named);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IHaveServiceLocator haveLocator, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.VerifyNotNull(haveLocator, "haveLocator");
            CodeContracts.VerifyNotNull(parameters, "parameters");

            return haveLocator.ServiceLocator.Get<TService>(parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator.
        /// </param>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public static TService Get<TService>(
            [NotNull] this IHaveServiceLocator haveLocator, [NotNull] string named, [NotNull] IEnumerable<IServiceLocationParameter> parameters)
        {
            CodeContracts.VerifyNotNull(haveLocator, "haveLocator");
            CodeContracts.VerifyNotNull(named, "named");
            CodeContracts.VerifyNotNull(parameters, "parameters");

            return haveLocator.ServiceLocator.Get<TService>(named, parameters);
        }

        /// <summary>
        /// The get repository.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRepository{T}"/>.
        /// </returns>
        public static IRepository<T> GetRepository<T>([NotNull] this IHaveServiceLocator serviceLocator)
            where T : IEntity
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

            return serviceLocator.Get<IRepository<T>>();
        }

        /// <summary>
        /// The is yaf context.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsBoardContext([NotNull] this IServiceLocator serviceLocator)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

            return serviceLocator.Tag == (object)LifetimeScope.Context;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get.
        /// </returns>
        public static bool TryGet<TService>([NotNull] this IServiceLocator serviceLocator, out TService instance)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

            instance = default;

            if (!serviceLocator.TryGet(typeof(TService), out var tempInstance))
            {
                return false;
            }

            instance = (TService)tempInstance;

            return true;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <param name="instance">
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get.
        /// </returns>
        public static bool TryGet<TService>(
            [NotNull] this IServiceLocator serviceLocator, [NotNull] string named, out TService instance)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
            CodeContracts.VerifyNotNull(named, "named");

            instance = default;

            if (!serviceLocator.TryGet(typeof(TService), named, out var tempInstance))
            {
                return false;
            }

            instance = (TService)tempInstance;

            return true;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get.
        /// </returns>
        public static bool TryGet<TService>([NotNull] this IHaveServiceLocator haveLocator, out TService instance)
        {
            CodeContracts.VerifyNotNull(haveLocator, "haveLocator");

            return haveLocator.ServiceLocator.TryGet(out instance);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="haveLocator">
        /// The have locator.
        /// </param>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get.
        /// </returns>
        public static bool TryGet<TService>(
            [NotNull] this IHaveServiceLocator haveLocator, [NotNull] string named, out TService instance)
        {
            CodeContracts.VerifyNotNull(haveLocator, "haveLocator");
            CodeContracts.VerifyNotNull(named, "named");

            return haveLocator.ServiceLocator.TryGet(named, out instance);
        }

        #endregion
    }
}