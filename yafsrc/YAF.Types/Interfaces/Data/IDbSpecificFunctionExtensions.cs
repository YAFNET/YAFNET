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
namespace YAF.Types.Interfaces.Data
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    /// The db specific function extensions.
    /// </summary>
    public static class IDbSpecificFunctionExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns IEnumerable where the provider name is supported.
        /// </summary>
        /// <param name="functions">
        /// The functions.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <returns>
        /// The is operation supported.
        /// </returns>
        [NotNull]
        public static IEnumerable<IDbSpecificFunction> WhereProviderName([NotNull] this IEnumerable<IDbSpecificFunction> functions, [NotNull] string providerName)
        {
            CodeContracts.VerifyNotNull(functions, "functions");
            CodeContracts.VerifyNotNull(providerName, "providerName");

            return functions.Where(p => string.Equals(p.ProviderName, providerName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}