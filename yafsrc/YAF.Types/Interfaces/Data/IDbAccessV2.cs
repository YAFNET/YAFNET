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
namespace YAF.Types.Interfaces.Data
{
    #region Using

    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    #endregion

    /// <summary>
    ///     DBAccess Interface
    /// </summary>
    public interface IDbAccessV2 : ICreateTransaction
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets ConnectionString.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        ///     Gets DbConnectionParameters.
        /// </summary>
        IEnumerable<IDbConnectionParam> DbConnectionParameters { get; }

        /// <summary>
        ///     Gets the current db provider factory
        /// </summary>
        /// <returns> </returns>
        DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        ///     Gets FullTextScript.
        /// </summary>
        string FullTextScript { get; }

        /// <summary>
        ///     Gets ProviderName.
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        ///     Gets Scripts.
        /// </summary>
        IEnumerable<string> Scripts { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit Of Work. 
        /// </param>
        void ExecuteNonQuery([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null);

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit Of Work. 
        /// </param>
        /// <returns>
        /// The execute scalar. 
        /// </returns>
        object ExecuteScalar([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null);

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="sql">
        /// The sql. 
        /// </param>
        /// <param name="isStoredProcedure">
        /// The is stored procedure. 
        /// </param>
        /// <param name="parameters">
        /// Command Parameters 
        /// </param>
        /// <returns>
        /// The <see cref="DbCommand"/>.
        /// </returns>
        IDbCommand GetCommand(
            [NotNull] string sql, 
            bool isStoredProcedure = true, 
            [CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null);

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit Of Work. 
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        DataTable GetData([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null);

        /// <summary>
        /// The get dataset.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="dbTransaction">
        /// The unit Of Work. 
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        DataSet GetDataset([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction = null);

        /// <summary>
        /// The get reader.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="readAction"> </param>
        /// <param name="dbTransaction">
        /// The unit of work.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        IDataReader GetReader([NotNull] IDbCommand cmd, [CanBeNull] IDbTransaction dbTransaction);

        #endregion
    }
}