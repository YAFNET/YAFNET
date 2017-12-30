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
namespace YAF.Types.Interfaces
{
    #region Using

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// The ScopeServiceLocator interface.
    /// </summary>
    public interface IScopeServiceLocator : IServiceLocator, IDisposable
    {
    }

    /// <summary>
    ///     The i service locator.
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        #region Public Properties

        /// <summary>
        /// Gets the tag.
        /// </summary>
        object Tag { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create scope.
        /// </summary>
        /// <returns>
        /// The <see cref="IScopeServiceLocator"/>.
        /// </returns>
        IScopeServiceLocator CreateScope(object tag = null);

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The get.
        /// </returns>
        object Get([NotNull] Type serviceType);

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
        object Get([NotNull] Type serviceType, [NotNull] IEnumerable<IServiceLocationParameter> parameters);

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
        object Get([NotNull] Type serviceType, [NotNull] string named);

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
        object Get([NotNull] Type serviceType, [NotNull] string named, [NotNull] IEnumerable<IServiceLocationParameter> parameters);

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
        bool TryGet([NotNull] Type serviceType, [NotNull] out object instance);

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
        bool TryGet([NotNull] Type serviceType, [NotNull] string named, [NotNull] out object instance);

        #endregion
    }
}