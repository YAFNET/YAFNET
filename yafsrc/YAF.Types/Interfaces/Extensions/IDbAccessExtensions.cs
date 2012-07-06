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
namespace YAF.Types.Interfaces
{
  #region Using

  using System.Data;

  #endregion

  /// <summary>
  /// The i db access extensions.
  /// </summary>
  public static class IDbAccessExtensions
  {
    #region Public Methods

    /// <summary>
    /// The execute non query.
    /// </summary>
    /// <param name="dbAccess">
    /// The db access.
    /// </param>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    public static void ExecuteNonQuery([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
    {
      CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
      CodeContracts.ArgumentNotNull(cmd, "cmd");

      dbAccess.ExecuteNonQuery(cmd, false);
    }

    /// <summary>
    /// The execute scalar.
    /// </summary>
    /// <param name="dbAccess">
    /// The db access.
    /// </param>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <returns>
    /// The execute scalar.
    /// </returns>
    public static object ExecuteScalar([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
    {
      CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
      CodeContracts.ArgumentNotNull(cmd, "cmd");

      return dbAccess.ExecuteScalar(cmd, false);
    }

    /// <summary>
    /// The get data.
    /// </summary>
    /// <param name="dbAccess">
    /// The db access.
    /// </param>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable GetData([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
    {
      CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
      CodeContracts.ArgumentNotNull(cmd, "cmd");

      return dbAccess.GetData(cmd, false);
    }

    /// <summary>
    /// The get dataset.
    /// </summary>
    /// <param name="dbAccess">
    /// The db access.
    /// </param>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataSet GetDataset([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
    {
      CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
      CodeContracts.ArgumentNotNull(cmd, "cmd");

      return dbAccess.GetDataset(cmd, false);
    }

    /// <summary>
    /// The get data.
    /// </summary>
    /// <param name="dbAccess">
    /// The db access.
    /// </param>
    /// <param name="sql">
    /// The sql.
    /// </param>
    /// <param name="unitOfWork"></param>
    /// <returns>
    /// </returns>
    public static DataTable GetData([NotNull] this IDbAccessV2 dbAccess, [NotNull] string sql, [CanBeNull] IDbUnitOfWork unitOfWork = null)
    {
        CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
        CodeContracts.ArgumentNotNull(sql, "sql");

        using (var cmd = dbAccess.GetCommand(sql, false))
        {
            return dbAccess.GetData(cmd, unitOfWork);
        }
    }

    #endregion
  }
}