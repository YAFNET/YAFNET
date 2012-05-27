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
    using System.Data;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Extensions;
    using YAF.Utils.Extensions;

    /// <summary>
    /// The load page lazy user data.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
    public class LoadPageLazyUserData : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        ///   The _db broker.
        /// </summary>
        private readonly IDBBroker _dbBroker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPageLazyUserData"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="dbBroker">
        /// The db broker.
        /// </param>
        public LoadPageLazyUserData([NotNull] IServiceLocator serviceLocator, [NotNull] IDBBroker dbBroker)
        {
            this._dbBroker = dbBroker;
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Order.
        /// </summary>
        public int Order
        {
            get
            {
                return 3000;
            }
        }

        /// <summary>
        ///   Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IHandleEvent<InitPageLoadEvent>

        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void Handle([NotNull] InitPageLoadEvent @event)
        {
            DataRow activeUserLazyData = this._dbBroker.ActiveUserLazyData(@event.Data.UserID);

            if (activeUserLazyData != null)
            {
                // add the lazy user data to this page data...
                @event.DataDictionary.AddRange(activeUserLazyData.ToDictionary());
            }
        }

        #endregion

        #endregion
    }
}