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
namespace YAF.Classes.Data
{
  #region Using

  using System.Collections.Generic;
  using System.Data;
  using System.Linq;

  using YAF.Types.Interfaces;
  using YAF.Types.Interfaces.Data;

    #endregion

  /// <summary>
  /// The abstract base data table result filter.
  /// </summary>
  public abstract class BaseDataTableResultFilter : IDataTableResultFilter
  {
    #region Constants and Fields

    /// <summary>
    /// The filter that sql commands starts with -- if none added all commands are processed.
    /// </summary>
    private List<string> _sqlCommandStartsWith = new List<string>();

    #endregion

    #region Properties

    /// <summary>
    /// Gets Rank.
    /// </summary>
    public abstract int Rank { get; }

    /// <summary>
    /// Gets or sets SqlCommandStartsWith.
    /// </summary>
    public List<string> SqlCommandStartsWith
    {
      get
      {
        return this._sqlCommandStartsWith;
      }

      set
      {
        this._sqlCommandStartsWith = value;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IDataTableResultFilter

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="dataTable">
    /// The data table.
    /// </param>
    /// <param name="sqlCommand">
    /// The sql command.
    /// </param>
    public void Process(ref DataTable dataTable, string sqlCommand)
    {
      if (this.SqlCommandStartsWith.Count > 0)
      {
        // validate if this command should be processed
        if (!this.SqlCommandStartsWith.Any(sqlCommand.StartsWith))
        {
          return;
        }
      }

      this.FilteredProcess(ref dataTable, sqlCommand);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The filtered process.
    /// </summary>
    /// <param name="dataTable">
    /// The data table.
    /// </param>
    /// <param name="sqlCommand">
    /// The sql command.
    /// </param>
    protected abstract void FilteredProcess(ref DataTable dataTable, string sqlCommand);

    #endregion
  }
}