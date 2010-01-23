/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Data
{
  using System.Data;
  using System.Data.SqlClient;

  /// <summary>
  /// DBAccess Interface
  /// </summary>
  public interface IYafDBAccess
  {
    /// <summary>
    /// Gets a whole dataset out of the database
    /// </summary>
    /// <param name="cmd">
    /// The SQL Command
    /// </param>
    /// <returns>
    /// Dataset with the results
    /// </returns>
    /// <remarks>
    /// Without transaction.
    /// </remarks>
    DataSet GetDataset(SqlCommand cmd);

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
    DataSet GetDataset(SqlCommand cmd, bool transaction);

    /// <summary>
    /// Gets data out of the database
    /// </summary>
    /// <param name="cmd">
    /// The SQL Command
    /// </param>
    /// <returns>
    /// DataTable with the results
    /// </returns>
    /// <remarks>
    /// Without transaction.
    /// </remarks>
    DataTable GetData(SqlCommand cmd);

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
    DataTable GetData(SqlCommand cmd, bool transaction);

    /// <summary>
    /// Gets data out of database using a plain text string command
    /// </summary>
    /// <param name="commandText">
    /// command text to be executed
    /// </param>
    /// <returns>
    /// DataTable with results
    /// </returns>
    /// <remarks>
    /// Without transaction.
    /// </remarks>
    DataTable GetData(string commandText);

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
    /// Executes a NonQuery
    /// </summary>
    /// <param name="cmd">
    /// NonQuery to execute
    /// </param>
    /// <remarks>
    /// Without transaction
    /// </remarks>
    void ExecuteNonQuery(SqlCommand cmd);

    /// <summary>
    /// The execute non query.
    /// </summary>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <param name="transaction">
    /// The transaction.
    /// </param>
    void ExecuteNonQuery(SqlCommand cmd, bool transaction);

    /// <summary>
    /// The execute scalar.
    /// </summary>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <returns>
    /// The execute scalar.
    /// </returns>
    object ExecuteScalar(SqlCommand cmd);

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
    object ExecuteScalar(SqlCommand cmd, bool transaction);
  }
}