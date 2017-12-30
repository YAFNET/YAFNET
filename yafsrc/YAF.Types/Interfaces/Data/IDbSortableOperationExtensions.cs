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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The i db sortable operation extensions.
    /// </summary>
    public static class IDbSortableOperationExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The by sort order.
        /// </summary>
        /// <param name="sortEnumerable">
        /// The sort enumerable.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IOrderedEnumerable{TElement}"/>.
        /// </returns>
        public static IOrderedEnumerable<T> BySortOrder<T>(this IEnumerable<T> sortEnumerable)
            where T : IDbSortableOperation
        {
            return sortEnumerable.OrderBy(x => x.SortOrder);
        }

        /// <summary>
        /// The is operation supported.
        /// </summary>
        /// <param name="checkSupported">
        /// The check Supported.
        /// </param>
        /// <param name="operationName">
        /// The operation name. 
        /// </param>
        /// <returns>
        /// The is operation supported. 
        /// </returns>
        public static IEnumerable<T> WhereOperationSupported<T>([NotNull] this IEnumerable<T> checkSupported, [NotNull] string operationName)
            where T : IDbSortableOperation
        {
            CodeContracts.VerifyNotNull(checkSupported, "checkSupported");
            CodeContracts.VerifyNotNull(operationName, "operationName");

            return checkSupported.Where(x => x.IsSupportedOperation(operationName));
        }

        #endregion
    }
}