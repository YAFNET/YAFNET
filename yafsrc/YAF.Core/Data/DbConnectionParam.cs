/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
	using YAF.Types;
	using YAF.Types.Interfaces.Data;

	//public class MsSqlDbSetup
	//{
	//  /// <summary>
	//  /// Gets DbConnectionParameters.
	//  /// </summary>
	//  IDbConnectionParam DbConnectionParameters { get; protected set; }

	//  #region Constants and Fields

	//  /// <summary>
	//  ///   The _script list.
	//  /// </summary>
	//  private static readonly string[] _scriptList = 
	//      {
	//          "mssql/tables.sql", 
	//          "mssql/indexes.sql", 
	//          "mssql/views.sql",
	//          "mssql/constraints.sql", 
	//          "mssql/triggers.sql",
	//          "mssql/functions.sql", 
	//          "mssql/procedures.sql",
	//          "mssql/providers/tables.sql",
	//          "mssql/providers/indexes.sql",
	//          "mssql/providers/procedures.sql" 
	//      };

	//  /// <summary>
	//  ///   The _full text script.
	//  /// </summary>
	//  private static string _fullTextScript = "mssql/fulltext.sql";

	//  /// <summary>
	//  ///   The _full text supported.
	//  /// </summary>
	//  private static bool _fullTextSupported = true;

	//  #endregion

	//  #region Properties

	//  /// <summary>
	//  ///   Gets a value indicating whether IsForumInstalled.
	//  /// </summary>
	//  public static bool GetIsForumInstalled()
	//  {
	//    try
	//    {
	//      using (DataTable dt = board_list(DBNull.Value))
	//      {
	//        return dt.Rows.Count > 0;
	//      }
	//    }
	//    catch
	//    {
	//    }

	//    return false;
	//  }

	//  /// <summary>
	//  ///   Gets the database size
	//  /// </summary>
	//  /// <returns>intager value for database size</returns>
	//  public static int GetDBSize()
	//  {
	//    using (var cmd = new SqlCommand("select sum(cast(size as integer))/128 from sysfiles"))
	//    {
	//      cmd.CommandType = CommandType.Text;
	//      return (int)Current.ExecuteScalar(cmd);
	//    }
	//  }

	//  /// <summary>
	//  ///   Gets DBVersion.
	//  /// </summary>
	//  public static int GetDBVersion()
	//  {
	//    try
	//    {
	//      using (DataTable dt = registry_list("version"))
	//      {
	//        if (dt.Rows.Count > 0)
	//        {
	//          // get the version...
	//          return dt.Rows[0]["Value"].ToType<int>();
	//        }
	//      }
	//    }
	//    catch
	//    {
	//      // not installed...
	//    }

	//    return -1;
	//  }

	//  /// <summary>
	//  ///   Gets or sets FullTextScript.
	//  /// </summary>
	//  public static string FullTextScript
	//  {
	//    get
	//    {
	//      return _fullTextScript;
	//    }

	//    set
	//    {
	//      _fullTextScript = value;
	//    }
	//  }

	//  /// <summary>
	//  ///   Gets or sets a value indicating whether FullTextSupported.
	//  /// </summary>
	//  public static bool FullTextSupported
	//  {
	//    get
	//    {
	//      return _fullTextSupported;
	//    }

	//    set
	//    {
	//      _fullTextSupported = value;
	//    }
	//  }


	//  /// <summary>
	//  ///   Gets a value indicating whether PanelGetStats.
	//  /// </summary>
	//  public static bool PanelGetStats
	//  {
	//    get
	//    {
	//      return true;
	//    }
	//  }

	//  /// <summary>
	//  ///   Gets a value indicating whether PanelRecoveryMode.
	//  /// </summary>
	//  public static bool PanelRecoveryMode
	//  {
	//    get
	//    {
	//      return true;
	//    }
	//  }

	//  /// <summary>
	//  ///   Gets a value indicating whether PanelReindex.
	//  /// </summary>
	//  public static bool PanelReindex
	//  {
	//    get
	//    {
	//      return true;
	//    }
	//  }

	//  /// <summary>
	//  ///   Gets a value indicating whether PanelShrink.
	//  /// </summary>
	//  public static bool PanelShrink
	//  {
	//    get
	//    {
	//      return true;
	//    }
	//  }



	//  /// <summary>
	//  /// Lists the UI parameters...
	//  /// </summary>
	//  public static DbConnectionUIParam[] DbUIParameters = new DbConnectionUIParam[]
	//      {
	//        new DbConnectionUIParam(1, "Data Source", "(local)", true), 
	//        new DbConnectionUIParam(2, "Initial Catalog", string.Empty, true), 
	//        new DbConnectionUIParam(11, "Use Integrated Security", "true", true),
	//      };

	//  /// <summary>
	//  ///   Gets a value indicating whether PasswordPlaceholderVisible.
	//  /// </summary>
	//  public static bool PasswordPlaceholderVisible
	//  {
	//    get
	//    {
	//      return false;
	//    }
	//  }

	//  /// <summary>
	//  ///   Gets ProviderAssemblyName.
	//  /// </summary>
	//  [NotNull]
	//  public static string ProviderAssemblyName
	//  {
	//    get
	//    {
	//      return "System.Data.SqlClient";
	//    }
	//  }

	//  /// <summary>
	//  ///   Gets ScriptList.
	//  /// </summary>
	//  public static string[] ScriptList
	//  {
	//    get
	//    {
	//      return _scriptList;
	//    }
	//  }

	//  #endregion
	//}

	/// <summary>
	/// The db connection param.
	/// </summary>
	public class DbConnectionParam : IDbConnectionParam
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DbConnectionParam"/> class.
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <param name="value">
		/// The value.
		/// </param>
		/// <param name="visible">
		/// The visible.
		/// </param>
		public DbConnectionParam(int id, [NotNull] string name = null, [NotNull] string value = null, bool visible = false)
		{
			this.ID = id;
			this.Label = name ?? string.Empty;
			this.DefaultValue = value ?? string.Empty;
			this.Visible = visible;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets DefaultValue.
		/// </summary>
		public string DefaultValue { get; protected set; }

		/// <summary>
		/// Gets or sets ID.
		/// </summary>
		public int ID { get; protected set; }

		/// <summary>
		/// Gets or sets Label.
		/// </summary>
		public string Label { get; protected set; }

		/// <summary>
		/// Gets or sets a value indicating whether Visible.
		/// </summary>
		public bool Visible { get; protected set; }

		#endregion
	}
}