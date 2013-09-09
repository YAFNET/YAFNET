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
namespace YAF.Core.Data
{
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// The init database provider event.
    /// </summary>
    public class InitDatabaseProviderEvent : IAmEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InitDatabaseProviderEvent"/> class.
        /// </summary>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        public InitDatabaseProviderEvent(string providerName, IDbAccess dbAccess)
        {
            this.ProviderName = providerName;
            this.DbAccess = dbAccess;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the db access.
        /// </summary>
        public IDbAccess DbAccess { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        #endregion
    }
}