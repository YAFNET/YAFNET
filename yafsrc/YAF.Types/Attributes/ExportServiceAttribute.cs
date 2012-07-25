/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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