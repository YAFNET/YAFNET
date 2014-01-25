/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
namespace YAF.Core.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using Omu.ValueInjecter;

    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     The mapping helper.
    /// </summary>
    public static class MappingHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Maps multiple objects to entities.
        /// </summary>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <typeparam name="TTypeIn">
        /// </typeparam>
        /// <typeparam name="TTypeOut">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<TTypeOut> ToMapped<TTypeIn, TTypeOut>(this IEnumerable<TTypeIn> objects)
            where TTypeIn : class
            where TTypeOut : IEntity, new()
        {
            return objects.Select(obj => obj.ToMappedEntity<TTypeOut>());
        }

        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="inputObject">
        /// The input object.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public static TEntity ToMappedEntity<TEntity>(this object inputObject) where TEntity : IEntity, new()
        {
            var entity = new TEntity();

            entity.InjectFrom(inputObject);

            return entity;
        }

        #endregion
    }
}