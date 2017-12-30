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
namespace YAF.Types.Attributes
{
    #region Using

    using System;

    #endregion

    /// <summary>
    /// The service lifetime scope.
    /// </summary>
    public enum ServiceLifetimeScope
    {
        /// <summary>
        ///   The singleton.
        /// </summary>
        Singleton,

        /// <summary>
        ///   Externally owned scope -- regular garbage collection
        /// </summary>
        Transient,

        /// <summary>
        ///   The owned by the container lifetime.
        /// </summary>
        OwnedByContainer,

        /// <summary>
        ///   One instance per container scope
        /// </summary>
        InstancePerScope,

        /// <summary>
        ///   One instance per dependancy.
        /// </summary>
        InstancePerDependancy,

        /// <summary>
        ///   One instance per context.
        /// </summary>
        InstancePerContext
    }

    /// <summary>
    /// The export service attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExportServiceAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportServiceAttribute"/> class.
        /// </summary>
        /// <param name="serviceLifetimeScope">
        /// The service lifetime scope.
        /// </param>
        /// <param name="named">
        /// The named.
        /// </param>
        /// <param name="registerSpecifiedTypes">
        /// The register Specified Types.
        /// </param>
        public ExportServiceAttribute(
          ServiceLifetimeScope serviceLifetimeScope, [CanBeNull] string named, [NotNull] params Type[] registerSpecifiedTypes)
        {
            this.Named = named;
            this.ServiceLifetimeScope = serviceLifetimeScope;
            this.RegisterSpecifiedTypes = registerSpecifiedTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportServiceAttribute"/> class.
        /// </summary>
        public ExportServiceAttribute(
          ServiceLifetimeScope serviceLifetimeScope, [CanBeNull] object keyed, [NotNull] params Type[] registerSpecifiedTypes)
        {
            this.ServiceLifetimeScope = serviceLifetimeScope;
            this.Keyed = keyed;
            this.RegisterSpecifiedTypes = registerSpecifiedTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportServiceAttribute"/> class.
        /// </summary>
        /// <param name="serviceLifetimeScope">
        /// The service lifetime scope.
        /// </param>
        public ExportServiceAttribute(ServiceLifetimeScope serviceLifetimeScope)
            : this(serviceLifetimeScope, null)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Named.
        /// </summary>
        public string Named { get; set; }

        /// <summary>
        ///   Gets or sets RegisterSpecifiedTypes.
        /// </summary>
        public Type[] RegisterSpecifiedTypes { get; set; }

        /// <summary>
        ///   Gets or sets ServiceLifetimeScope.
        /// </summary>
        public ServiceLifetimeScope ServiceLifetimeScope { get; protected set; }

        /// <summary>
        /// Gets or sets Keyed
        /// </summary>
        public object Keyed { get; set; }

        #endregion
    }
}