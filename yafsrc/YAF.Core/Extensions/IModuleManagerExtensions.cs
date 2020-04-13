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

namespace YAF.Core.Extensions
{
    #region Using

    using System.Collections.Generic;

    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The module manager extensions.
    /// </summary>
    public static class IModuleManagerExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Get all active modules.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="moduleManager">The module Manager.</param>
        /// <returns>Returns all active modules</returns>
        public static IEnumerable<TModule> GetAll<TModule>([NotNull] this IModuleManager<TModule> moduleManager)
            where TModule : IModuleDefinition
        {
            CodeContracts.VerifyNotNull(moduleManager, "moduleManager");

            return moduleManager.GetAll(false);
        }

        #endregion
    }
}