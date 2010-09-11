using System;
using System.Data;
using YAF.Classes.Data;

namespace YAF.Classes.Core
{
  using YAF.Classes.Utils;

  /// <summary>
  /// The yaf load board settings.
  /// </summary>
  public class YafLoadBoardSettings : YafBoardSettings
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="YafLoadBoardSettings"/> class.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public YafLoadBoardSettings(object boardID)
    {
      _boardID = boardID;

      // get the board table
      DataTable dataTable = DB.board_list(_boardID);

      if (dataTable.Rows.Count == 0)
      {
        throw new Exception("No data for board with id: " + _boardID);
      }

      // setup legacy board settings...
      SetupLegacyBoardSettings(dataTable.Rows[0]);

      // get all the registry values for the forum
      LoadBoardSettingsFromDB();
    }

    /// <summary>
    /// The setup legacy board settings.
    /// </summary>
    /// <param name="board">
    /// The board.
    /// </param>
    private void SetupLegacyBoardSettings(DataRow board)
    {
      _membershipAppName = board["MembershipAppName"].ToString().IsNotSet()
                             ? YafContext.Current.CurrentMembership.ApplicationName
                             : board["MembershipAppName"].ToString();

      _rolesAppName = board["RolesAppName"].ToString().IsNotSet()
                        ? YafContext.Current.CurrentRoles.ApplicationName
                        : board["RolesAppName"].ToString();

      _legacyBoardSettings = new YafLegacyBoardSettings(
        board["Name"].ToString(), 
        Convert.ToString(board["SQLVersion"]), 
        SqlDataLayerConverter.VerifyBool(board["AllowThreaded"].ToString()), 
        _membershipAppName, 
        _rolesAppName);
    }

    /// <summary>
    /// The load board settings from db.
    /// </summary>
    /// <exception cref="Exception">
    /// </exception>
    protected void LoadBoardSettingsFromDB()
    {
      // verify DB is initialized...
      if (!YafServices.InitializeDb.Initialized)
      {
        throw new Exception("Database is not initialized.");
      }

      DataTable dataTable;
      using (dataTable = DB.registry_list())
      {
        // get all the registry settings into our hash table
        foreach (DataRow dr in dataTable.Rows)
        {
          if (dr["Value"] == DBNull.Value)
          {
            _reg.Add(dr["Name"].ToString().ToLower(), null);
          }
          else
          {
            _reg.Add(dr["Name"].ToString().ToLower(), dr["Value"]);
          }
        }
      }

      using (dataTable = DB.registry_list(null, _boardID))
      {
        // get all the registry settings into our hash table
        foreach (DataRow dr in dataTable.Rows)
        {
          if (dr["Value"] == DBNull.Value)
          {
            _regBoard.Add(dr["Name"].ToString().ToLower(), null);
          }
          else
          {
            _regBoard.Add(dr["Name"].ToString().ToLower(), dr["Value"]);
          }
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
        DB.registry_save(key, _reg[key]);
      }

      foreach (string key in _regBoard.Keys)
      {
        DB.registry_save(key, _regBoard[key], _boardID);
      }
    }
  }
}