/* Yet Another Forum.net
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
namespace YAF.Core
{
    #region Using

    using Autofac;

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
        /// The _sync object.
        /// </summary>
        private static readonly object _syncObject = new object();

        /// <summary>
        ///   The _container.
        /// </summary>
        private static IContainer _container;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Container.
        /// </summary>
        public static IContainer Container
        {
            get
            {
                if (_container == null)
                {
                    lock (_syncObject)
                    {
                        if (_container == null)
                        {
                            _container = CreateContainer();

                            // immediately setup the static service locator...
                            ServiceLocatorAccess.CurrentServiceProvider = _container.Resolve<IServiceLocator>();
                        }
                    }
                }

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

            var mainModule = new YafBaseContainerModule();
            var logModule = new LoggingModule();

            builder.RegisterModule(mainModule);
            builder.RegisterModule(logModule);

            return builder.Build();
        }

        #endregion
    }
}