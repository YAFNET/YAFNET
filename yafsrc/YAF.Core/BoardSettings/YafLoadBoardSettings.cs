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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Data;
  using System.Web.Security;

  using YAF.Classes;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The yaf load board settings.
  /// </summary>
  public class YafLoadBoardSettings : YafBoardSettings
  {
  	private readonly IDbFunction _dbFunction;

  	#region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafLoadBoardSettings"/> class.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    /// <exception cref="EmptyBoardSettingException"><c>EmptyBoardSettingException</c>.</exception>
    public YafLoadBoardSettings([NotNull] int boardID, IDbFunction dbFunction)
    {
    	_dbFunction = dbFunction;
    	this._boardID = boardID;

      // get the board table
      DataTable dataTable = this._dbFunction.GetData.board_list(this._boardID);

      if (dataTable.Rows.Count == 0)
      {
        throw new EmptyBoardSettingException("No data for board ID: {0}".FormatWith(this._boardID));
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
				this._dbFunction.Query.registry_save(key, this._reg[key]);
      }

      foreach (string key in this._regBoard.Keys)
      {
				this._dbFunction.Query.registry_save(key, this._regBoard[key], this._boardID);
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
      DataTable dataTable;

      using (dataTable = this._dbFunction.GetData.registry_list())
      {
        // get all the registry settings into our hash table
        foreach (DataRow dr in dataTable.Rows)
        {
          this._reg.Add(dr["Name"].ToString().ToLower(), dr["Value"] == DBNull.Value ? null : dr["Value"]);
        }
      }

      using (dataTable = this._dbFunction.GetData.registry_list(null, this._boardID))
      {
        // get all the registry settings into our hash table
        foreach (DataRow dr in dataTable.Rows)
        {
          this._regBoard.Add(dr["Name"].ToString().ToLower(), dr["Value"] == DBNull.Value ? null : dr["Value"]);
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
      CodeContracts.ArgumentNotNull(board, "board");

      this._membershipAppName = board["MembershipAppName"].ToString().IsNotSet()
                                  ? YafContext.Current.Get<MembershipProvider>().ApplicationName
                                  : board["MembershipAppName"].ToString();

      this._rolesAppName = board["RolesAppName"].ToString().IsNotSet()
                             ? YafContext.Current.Get<RoleProvider>().ApplicationName
                             : board["RolesAppName"].ToString();

      this._legacyBoardSettings = new YafLegacyBoardSettings(
        board["Name"].ToString(), 
        Convert.ToString(board["SQLVersion"]), 
        board["AllowThreaded"].ToType<bool>(), 
        this._membershipAppName, 
        this._rolesAppName);
    }

    #endregion
  }

  public class EmptyBoardSettingException : Exception
  {
    public EmptyBoardSettingException(string message)
      : base(message)
    {
      
    }
  }
}