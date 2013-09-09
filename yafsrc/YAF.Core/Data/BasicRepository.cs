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
    #region Using

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    /// The basic repository.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class BasicRepository<T> : IRepository<T>
        where T : IEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicRepository{T}"/> class.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function. 
        /// </param>
        /// <param name="dbAccess">
        /// The db Access.
        /// </param>
        /// <param name="raiseEvent">
        /// The raise Event. 
        /// </param>
        /// <param name="haveBoardId">
        /// The have Board Id. 
        /// </param>
        public BasicRepository(IDbFunction dbFunction, IDbAccess dbAccess, IRaiseEvent raiseEvent, IHaveBoardID haveBoardId)
        {
            this.DbFunction = dbFunction;
            this.DbAccess = dbAccess;
            this.DbEvent = raiseEvent;
            this.BoardID = haveBoardId.BoardID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the board id.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        /// Gets the db access.
        /// </summary>
        public IDbAccess DbAccess { get; private set; }

        /// <summary>
        ///     Gets the db event.
        /// </summary>
        public IRaiseEvent DbEvent { get; private set; }

        /// <summary>
        ///     Gets the db function.
        /// </summary>
        public IDbFunction DbFunction { get; private set; }

        #endregion
    }
}