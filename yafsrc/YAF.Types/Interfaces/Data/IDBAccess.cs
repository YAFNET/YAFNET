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
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// DBAccess Interface
    /// </summary>
    public interface IDbAccess
    {
        /// <summary>
        /// Gets the current connection manager.
        /// </summary>
        /// <returns></returns>
        IDbConnectionManager GetConnectionManager();

        /// <summary>
        /// Sets the connection manager adapter.
        /// </summary>
        /// <typeparam name="TManager"></typeparam>
        void SetConnectionManagerAdapter<TManager>() where TManager : IDbConnectionManager;

        /// <summary>
        /// Filter list of result filters.
        /// </summary>
        IList<IDataTableResultFilter> ResultFilterList { get; }

        /// <summary>
        /// The get dataset.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// </returns>
        DataSet GetDataset(IDbCommand cmd, bool transaction);

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// </returns>
        DataTable GetData(IDbCommand cmd, bool transaction);

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// </returns>
        DataTable GetData(string commandText, bool transaction);

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        void ExecuteNonQuery(IDbCommand cmd, bool transaction);

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The execute scalar.
        /// </returns> 
        object ExecuteScalar(IDbCommand cmd, bool transaction);
        /// <summary>
        /// Returns values from data reader. 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="dt"></param>
        /// <param name="transaction"></param>
        /// <param name="acceptChanges"></param>
        /// <param name="firstColumnIndex"></param>
        /// <returns></returns>
        DataTable AddValuesToDataTableFromReader(IDbCommand cmd, DataTable dt, bool transaction, bool acceptChanges,
                                                int firstColumnIndex);
        /// <summary>
        /// Returns values from data reader.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="dt"></param>
        /// <param name="transaction"></param>
        /// <param name="acceptChanges"></param>
        /// <param name="firstColumnIndex"></param>
        /// <param name="currentRow"></param>
        /// <returns></returns>

        DataTable AddValuesToDataTableFromReader(IDbCommand cmd, DataTable dt, bool transaction, bool acceptChanges,
                                                 int firstColumnIndex, int currentRow);
        /// <summary>
        /// Returns values from data reader.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="transaction"></param>
        /// <param name="acceptChanges"></param>
        /// <returns></returns>
        DataTable GetDataTableFromReader(IDbCommand cmd, bool transaction, bool acceptChanges);
    }
}