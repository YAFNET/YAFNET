using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using YAF.Classes.Data;

namespace YAF.Classes.Core
{
	public class YafLoadBoardSettings : YafBoardSettings
	{
		public YafLoadBoardSettings(object boardID)
			: base()
		{
			_boardID = boardID;

			// get the board table
			DataTable dataTable = YAF.Classes.Data.DB.board_list(_boardID);

			if (dataTable.Rows.Count == 0)
				throw new Exception("No data for board with id: " + _boardID);

			// setup legacy board settings...
			SetupLegacyBoardSettings(dataTable.Rows[0]);

			// get all the registry values for the forum
			LoadBoardSettingsFromDB();
		}

		private void SetupLegacyBoardSettings(DataRow board)
		{
			_membershipAppName = (String.IsNullOrEmpty(board["MembershipAppName"].ToString()))
														? YafContext.Current.CurrentMembership.ApplicationName
														: board["MembershipAppName"].ToString();

			_rolesAppName = (String.IsNullOrEmpty(board["RolesAppName"].ToString()))
												? YafContext.Current.CurrentRoles.ApplicationName
												: board["RolesAppName"].ToString();

			_legacyBoardSettings = new YafLegacyBoardSettings(board["Name"].ToString(), Convert.ToString(board["SQLVersion"]),
																												 SqlDataLayerConverter.VerifyBool(
																													board["AllowThreaded"].ToString()), _membershipAppName,
																												 _rolesAppName);
		}

		protected void LoadBoardSettingsFromDB()
		{
			// verify DB is initialized...
			if (!YafServices.InitializeDb.Initialized) throw new Exception( "Database is not initialized." );

			DataTable dataTable;
			using (dataTable = YAF.Classes.Data.DB.registry_list())
			{
				// get all the registry settings into our hash table
				foreach (DataRow dr in dataTable.Rows)
				{
					if ( dr["Value"] == DBNull.Value )
						_reg.Add( dr["Name"].ToString().ToLower(), null );
					else
						_reg.Add( dr["Name"].ToString().ToLower(), dr["Value"] );
				}
			}
			using (dataTable = YAF.Classes.Data.DB.registry_list(null, _boardID))
			{
				// get all the registry settings into our hash table
				foreach (DataRow dr in dataTable.Rows)
				{
					if ( dr["Value"] == DBNull.Value )
						_regBoard.Add( dr["Name"].ToString().ToLower(), null );
					else
						_regBoard.Add( dr["Name"].ToString().ToLower(), dr["Value"] );
				}
			}
		}

		/// <summary>
		/// Saves the whole setting registry to the database.
		/// </summary>
		public void SaveRegistry()
		{
			// loop through all values and commit them to the DB
			foreach (string key in _reg.Keys)
			{
				DB.registry_save( key, _reg[key] );
			}
			foreach (string key in _regBoard.Keys)
			{
				DB.registry_save( key, _regBoard[key], _boardID );
			}
		}
	}
}
