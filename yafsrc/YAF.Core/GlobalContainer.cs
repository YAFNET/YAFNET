/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Core
{
    #region Using

    using System.Reflection;

    using Autofac;

    using YAF.Core.Helpers;
    using YAF.Core.Modules;
    using YAF.Core.Services.Logger;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Instance of the Global Container... yes, a God class. It's the best way to do it, though.
    /// </summary>
    public static class GlobalContainer
    {
        #region Constants and Fields

        /// <summary>
        ///   The _container.
        /// </summary>
        private static readonly IContainer _container;

        #endregion

        static GlobalContainer()
        {
            var container = CreateContainer();
            ServiceLocatorAccess.CurrentServiceProvider = container.Resolve<IServiceLocator>();
            _container = container;
        }

        #region Properties

        /// <summary>
        ///   Gets Container.
        /// </summary>
        public static IContainer Container
        {
            get
            {
                return _container;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create container.
        /// </summary>
        /// <returns>
        /// The Autofac.IContainer.
        /// </returns>
        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<BootstrapModule>();

            return builder.Build();
        }

        #endregion
    }
}