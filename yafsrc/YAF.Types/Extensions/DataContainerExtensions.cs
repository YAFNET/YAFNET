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
namespace YAF.Types.Extensions
{
    #region Using

    using System.Data;
    using System.Web.UI;

    #endregion

    /// <summary>
    /// The data container extensions.
    /// </summary>
    public static class DataContainerExtensions
    {
        #region Public Methods

        /// <summary>
        /// The data item to field.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T DataItemToField<T>(this IDataItemContainer container, string fieldName)
        {
            if (container == null || fieldName.IsNotSet())
            {
                return default;
            }

            return container.DataItem switch
                {
                    DataRow _ => container.DataItem.ToType<DataRow>().Field<T>(fieldName),
                    DataRowView _ => container.DataItem.ToType<DataRowView>()[fieldName].ToType<T>(),
                    _ => container.DataItem.ToType<T>()
                };
        }

        /// <summary>
        /// The to data item type.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ToDataItemType<T>(this IDataItemContainer container)
        {
            return Equals(container, default(T)) ? default : container.DataItem.ToType<T>();
        }

        /// <summary>
        /// Converts DataItem to a class.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="container">
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ToDataItemClass<T>(this IDataItemContainer container) where T : class
        {
            return container?.DataItem.ToClass<T>();
        }

        #endregion
    }
}