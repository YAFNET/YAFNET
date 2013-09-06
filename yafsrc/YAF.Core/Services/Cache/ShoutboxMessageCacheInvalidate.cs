/* Yet Another Forum.NET
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
namespace YAF.Core.Services.Cache
{
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class ShoutboxMessageCacheInvalidate : IHandleEvent<RepositoryEvent<ShoutboxMessage>>
    {
        #region Fields

        /// <summary>
        /// The _data cache.
        /// </summary>
        private readonly IDataCache _dataCache;

        /// <summary>
        /// The _object store.
        /// </summary>
        private readonly IObjectStore _objectStore;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmileyEventCacheInvalidate"/> class.
        /// </summary>
        /// <param name="dataCache">
        /// The data cache.
        /// </param>
        /// <param name="objectStore">
        /// The object store.
        /// </param>
        public ShoutboxMessageCacheInvalidate(IDataCache dataCache, IObjectStore objectStore)
        {
            this._dataCache = dataCache;
            this._objectStore = objectStore;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the order.
        /// </summary>
        public int Order
        {
            get
            {
                return 1000;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event. 
        /// </param>
        public void Handle(RepositoryEvent<ShoutboxMessage> @event)
        {
            this._dataCache.Remove(Constants.Cache.Shoutbox);
        }

        #endregion
    }
}