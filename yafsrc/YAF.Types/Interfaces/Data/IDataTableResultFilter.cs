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

    using System.Data;

    #endregion

  /// <summary>
  /// The data table result filter interface for (AOP)
  /// </summary>
  public interface IDataTableResultFilter
  {
    #region Properties

    /// <summary>
    /// Gets Rank.
    /// </summary>
    int Rank { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="dataTable">
    /// The data table.
    /// </param>
    /// <param name="sqlCommand">
    /// The sql command.
    /// </param>
    void Process(ref DataTable dataTable, string sqlCommand);

    #endregion
  }
}