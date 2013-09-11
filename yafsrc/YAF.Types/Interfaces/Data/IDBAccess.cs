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
namespace YAF.Types.Interfaces.Data
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    #endregion

    /// <summary>
    ///     DBAccess Interface
    /// </summary>
    public interface IDbAccess
    {
        #region Public Properties

        /// <summary>
        ///     Gets the database information
        /// </summary>
        IDbInformation Information { get; }

        /// <summary>
        ///     Gets the current db provider factory
        /// </summary>
        /// <returns> </returns>
        DbProviderFactory DbProviderFactory { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="execFunc">
        /// The exec func.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="dbTransaction">
        /// The db transaction.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Execute<T>(Func<IDbCommand, T> execFunc, IDbCommand cmd = null, [CanBeNull] IDbTransaction dbTransaction = null);

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="sql">
        /// The sql. 
        /// </param>
        /// <param name="commandType"></param>
        /// <param name="parameters">
        /// Command Parameters 
        /// </param>
        /// <returns>
        /// The <see cref="DbCommand"/> . 
        /// </returns>
        IDbCommand GetCommand(
            [NotNull] string sql,
            CommandType commandType = CommandType.StoredProcedure, 
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null);

        #endregion
    }
}