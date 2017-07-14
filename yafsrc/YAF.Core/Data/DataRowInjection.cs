/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2017 Ingo Herbote
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
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Omu.ValueInjecter.Injections;
    using Omu.ValueInjecter.Utils;

    using ServiceStack.DataAnnotations;

    /// <summary>
    /// The DataRow Injection Class
    /// </summary>
    public class DataRowInjection : KnownSourceInjection<DataRow>
    {
        #region Methods

        /// <summary>
        /// Injects the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        protected override void Inject(DataRow source, object target)
        {
            var props = target.GetProps();

            var aliasMapping = props.OfType<PropertyDescriptor>()
                .Where(p => p.Attributes.OfType<AliasAttribute>().Any())
                .ToDictionary(k => k.Attributes.OfType<AliasAttribute>().FirstOrDefault().Name, v => v.Name);

            var nameMap = new Func<string, string>(
                inputName => aliasMapping.ContainsKey(inputName) ? aliasMapping[inputName] : inputName);

            for (var i = 0; i < source.ItemArray.Length; i++)
            {
                var activeTarget = target.GetType()
                    .GetProperty(
                        nameMap(source.Table.Columns[i].ColumnName),
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (activeTarget == null)
                {
                    continue;
                }

                var value = source.ItemArray[i];
                if (value == DBNull.Value)
                {
                    continue;
                }

                if (activeTarget.PropertyType == value.GetType())
                {
                    activeTarget.SetValue(target, value);
                }
                else
                {
                    var conversionType = activeTarget.PropertyType;

                    if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        conversionType = new NullableConverter(conversionType).UnderlyingType;
                    }

                    activeTarget.SetValue(target, Convert.ChangeType(value, conversionType));
                }
            }
        }

        #endregion
    }
}