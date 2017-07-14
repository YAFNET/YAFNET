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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Omu.ValueInjecter.Injections;
    using Omu.ValueInjecter.Utils;

    using ServiceStack.DataAnnotations;

    /// <summary>
    /// The Alias Injection 
    /// </summary>
    /// <seealso cref="Omu.ValueInjecter.Injections.LoopInjection" />
    public class AliasInjection : LoopInjection
    {
        /// <summary>
        /// The alias mapping
        /// </summary>
        private Dictionary<string, string> aliasMapping;

        /// <summary>
        /// The name map
        /// </summary>
        private Func<string, string> nameMap;

        /// <summary>
        /// Injects the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        protected override void Inject(object source, object target)
        {
            var props = target.GetProps();

            this.aliasMapping = props.OfType<PropertyDescriptor>()
                .Where(p => p.Attributes.OfType<AliasAttribute>().Any())
                .ToDictionary(k => k.Attributes.OfType<AliasAttribute>().FirstOrDefault().Name, v => v.Name);

            this.nameMap = inputName => this.aliasMapping.ContainsKey(inputName)
                                            ? this.aliasMapping[inputName]
                                            : inputName;

            base.Inject(source, target);
        }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <param name="sourcePropName">Name of the source property.</param>
        /// <returns>Returns the target property</returns>
        protected override string GetTargetProp(string sourcePropName)
        {
            return base.GetTargetProp(this.nameMap != null ? this.nameMap(sourcePropName) : sourcePropName);
        }
    }
}