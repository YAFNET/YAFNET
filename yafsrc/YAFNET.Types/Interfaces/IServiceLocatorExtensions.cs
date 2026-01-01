/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Interfaces;

using System.Collections.Generic;

/// <summary>
///     The i service locator extensions.
/// </summary>
public static class IServiceLocatorExtensions
{
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    extension(IServiceLocator serviceLocator)
    {
        /// <summary>
        /// The get.
        /// </summary>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public TService Get<TService>()
        {
            return (TService)serviceLocator.Get(typeof(TService));
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public TService Get<TService>(string named)
        {
            return (TService)serviceLocator.Get(typeof(TService), named);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="parameters">
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public TService Get<TService>(IEnumerable<IServiceLocationParameter> parameters)
        {
            return (TService)serviceLocator.Get(typeof(TService), parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
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
        public TService Get<TService>(string named,
            IEnumerable<IServiceLocationParameter> parameters)
        {
            return (TService)serviceLocator.Get(typeof(TService), named, parameters);
        }
    }

    /// <param name="haveLocator">
    /// The have locator.
    /// </param>
    extension(IHaveServiceLocator haveLocator)
    {
        /// <summary>
        /// The get.
        /// </summary>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public TService Get<TService>()
        {
            return haveLocator.ServiceLocator.Get<TService>();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public TService Get<TService>(string named)
        {
            return haveLocator.ServiceLocator.Get<TService>(named);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        public TService Get<TService>(IEnumerable<IServiceLocationParameter> parameters)
        {
            return haveLocator.ServiceLocator.Get<TService>(parameters);
        }

        /// <summary>
        /// The get.
        /// </summary>
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
        public TService Get<TService>(string named,
            IEnumerable<IServiceLocationParameter> parameters)
        {
            return haveLocator.ServiceLocator.Get<TService>(named, parameters);
        }

        /// <summary>
        /// The get repository.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRepository{T}"/>.
        /// </returns>
        public IRepository<T> GetRepository<T>()
            where T : IEntity
        {
            return haveLocator.Get<IRepository<T>>();
        }
    }

    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    extension(IServiceLocator serviceLocator)
    {
        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get.
        /// </returns>
        public bool TryGet<TService>(out TService instance)
        {
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
        public bool TryGet<TService>(string named, out TService instance)
        {
            instance = default;

            if (!serviceLocator.TryGet(typeof(TService), named, out var tempInstance))
            {
                return false;
            }

            instance = (TService)tempInstance;

            return true;
        }
    }

    /// <param name="haveLocator">
    /// The have locator.
    /// </param>
    extension(IHaveServiceLocator haveLocator)
    {
        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The try get.
        /// </returns>
        public bool TryGet<TService>(out TService instance)
        {
            return haveLocator.ServiceLocator.TryGet(out instance);
        }

        /// <summary>
        /// The get.
        /// </summary>
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
        public bool TryGet<TService>(string named, out TService instance)
        {
            return haveLocator.ServiceLocator.TryGet(named, out instance);
        }
    }
}