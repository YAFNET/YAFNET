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
namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Data;

  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The yaf load board settings.
  /// </summary>
  public class YafLoadBoardSettings : YafBoardSettings
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafLoadBoardSettings"/> class.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public YafLoadBoardSettings([NotNull] object boardID)
    {
      this._boardID = boardID;

      // get the board table
      DataTable dataTable = DB.board_list(this._boardID);

      if (dataTable.Rows.Count == 0)
      {
        throw new Exception("No data for board with id: " + this._boardID);
      }

      // setup legacy board settings...
      this.SetupLegacyBoardSettings(dataTable.Rows[0]);

      // get all the registry values for the forum
      this.LoadBoardSettingsFromDB();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Saves the whole setting registry to the database.
    /// </summary>
    public void SaveRegistry()
    {
      // loop through all values and commit them to the DB
      foreach (string key in this._reg.Keys)
      {
        DB.registry_save(key, this._reg[key]);
      }

      foreach (string key in this._regBoard.Keys)
      {
        DB.registry_save(key, this._regBoard[key], this._boardID);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The load board settings from db.
    /// </summary>
    /// <exception cref="Exception">
    /// </exception>
    protected void LoadBoardSettingsFromDB()
    {
      // verify DB is initialized...
      if (!YafContext.Current.Get<YafInitializeDb>().Initialized)
      {
        YafContext.Current.Get<YafInitializeDb>().Run();
      }

      DataTable dataTable;
      using (dataTable = DB.registry_list())
      {
        // get all the registry settings into our hash table
        foreach (DataRow dr in dataTable.Rows)
        {
          if (dr["Value"] == DBNull.Value)
          {
            this._reg.Add(dr["Name"].ToString().ToLower(), null);
          }
          else
          {
            this._reg.Add(dr["Name"].ToString().ToLower(), dr["Value"]);
          }
        }
      }

      using (dataTable = DB.registry_list(null, this._boardID))
      {
        // get all the registry settings into our hash table
        foreach (DataRow dr in dataTable.Rows)
        {
          if (dr["Value"] == DBNull.Value)
          {
            this._regBoard.Add(dr["Name"].ToString().ToLower(), null);
          }
          else
          {
            this._regBoard.Add(dr["Name"].ToString().ToLower(), dr["Value"]);
          }
        }
      }
    }

    /// <summary>
    /// The setup legacy board settings.
    /// </summary>
    /// <param name="board">
    /// The board.
    /// </param>
    private void SetupLegacyBoardSettings([NotNull] DataRow board)
    {
      this._membershipAppName = board["MembershipAppName"].ToString().IsNotSet()
                                  ? YafContext.Current.CurrentMembership.ApplicationName
                                  : board["MembershipAppName"].ToString();

      this._rolesAppName = board["RolesAppName"].ToString().IsNotSet()
                             ? YafContext.Current.CurrentRoles.ApplicationName
                             : board["RolesAppName"].ToString();

      this._legacyBoardSettings = new YafLegacyBoardSettings(
        board["Name"].ToString(), 
        Convert.ToString(board["SQLVersion"]), 
        SqlDataLayerConverter.VerifyBool(board["AllowThreaded"].ToString()), 
        this._membershipAppName, 
        this._rolesAppName);
    }

    #endregion
  }
}