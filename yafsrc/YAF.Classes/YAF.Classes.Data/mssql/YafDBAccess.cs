/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Data.SqlClient;

  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The yaf db access for SQL Server.
  /// </summary>
  public class YafDBAccess : IYafDBAccess
  {
    #region Constants and Fields

    /// <summary>
    /// The _isolation level.
    /// </summary>
    private static IsolationLevel _isolationLevel = IsolationLevel.ReadUncommitted;

    /// <summary>
    /// The _connection manager type.
    /// </summary>
    private Type _connectionManagerType = typeof(YafDBConnManager);

    /// <summary>
    /// Result filter list
    /// </summary>
    private IList<IDataTableResultFilter> _resultFilterList = new List<IDataTableResultFilter>();

    #endregion

    #region Properties

    /// <summary>
    /// Gets Current.
    /// </summary>
    public static YafDBAccess Current
    {
      get
      {
        return PageSingleton<YafDBAccess>.Instance;
      }
    }

    /// <summary>
    /// Gets IsolationLevel.
    /// </summary>
    public static IsolationLevel IsolationLevel
    {
      get
      {
        return _isolationLevel;
      }
    }

    /// <summary>
    /// Gets the Result Filter List.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public IList<IDataTableResultFilter> ResultFilterList
    {
      get
      {
        return this._resultFilterList;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates new SqlCommand based on command text applying all qualifiers to the name.
    /// </summary>
    /// <param name="commandText">
    /// Command text to qualify.
    /// </param>
    /// <param name="isText">
    /// Determines whether command text is text or stored procedure.
    /// </param>
    /// <returns>
    /// New SqlCommand
    /// </returns>
    public static SqlCommand GetCommand(string commandText, bool isText)
    {
      return GetCommand(commandText, isText, null);
    }

    /// <summary>
    /// Creates new SqlCommand based on command text applying all qualifiers to the name.
    /// </summary>
    /// <param name="commandText">
    /// Command text to qualify.
    /// </param>
    /// <param name="isText">
    /// Determines whether command text is text or stored procedure.
    /// </param>
    /// <param name="connection">
    /// Connection to use with command.
    /// </param>
    /// <returns>
    /// New SqlCommand
    /// </returns>
    public static SqlCommand GetCommand(string commandText, bool isText, SqlConnection connection)
    {
      return isText
               ? new SqlCommand
                 {
                   CommandType = CommandType.Text, 
                   CommandText = GetCommandTextReplaced(commandText), 
                   Connection = connection
                 }
               : GetCommand(commandText);
    }

    /// <summary>
    /// Creates new SqlCommand calling stored procedure applying all qualifiers to the name.
    /// </summary>
    /// <param name="storedProcedure">
    /// Base of stored procedure name.
    /// </param>
    /// <returns>
    /// New SqlCommand
    /// </returns>
    public static SqlCommand GetCommand(string storedProcedure)
    {
      return GetCommand(storedProcedure, null);
    }

    /// <summary>
    /// Creates new SqlCommand calling stored procedure applying all qualifiers to the name.
    /// </summary>
    /// <param name="storedProcedure">
    /// Base of stored procedure name.
    /// </param>
    /// <param name="connection">
    /// Connection to use with command.
    /// </param>
    /// <returns>
    /// New SqlCommand
    /// </returns>
    public static SqlCommand GetCommand(string storedProcedure, SqlConnection connection)
    {
      var cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = GetObjectName(storedProcedure),
                        Connection = connection,
                        CommandTimeout = int.Parse(Config.SqlCommandTimeout)
                    };

        return cmd;
    }

    /// <summary>
    /// Gets command text replaced with {databaseOwner} and {objectQualifier}.
    /// </summary>
    /// <param name="commandText">
    /// Test to transform.
    /// </param>
    /// <returns>
    /// The get command text replaced.
    /// </returns>
    public static string GetCommandTextReplaced(string commandText)
    {
      commandText = commandText.Replace("{databaseOwner}", Config.DatabaseOwner);
      commandText = commandText.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

      return commandText;
    }

    /// <summary>
    /// A method to handle custom scripts execution tags 
    /// </summary>
    /// <param name="scriptChunk">Input string</param>
    /// <param name="versionSQL">SQL server version as ushort</param>
    /// <returns>Returns an empty string if condition was not and cleanedfrom tags string if was.</returns>
    public static string CleanForSQLServerVersion(string scriptChunk, ushort versionSQL)
    {
        if (!scriptChunk.Contains("#IFSRVVER")) 
        {
            return scriptChunk;
        }
        else
        {
            int indSign =  scriptChunk.IndexOf("#IFSRVVER") + 9;
            string temp = scriptChunk.Substring(indSign);
            int indEnd = temp.IndexOf("#");
            int indEqual = temp.IndexOf("=");
            int indMore = temp.IndexOf(">");

            if (indEqual >= 0 && indEqual < indEnd)
            {            
                ushort indVerEnd = Convert.ToUInt16(temp.Substring(indEqual + 1, indEnd - indEqual - 1).Trim());
                if (versionSQL == indVerEnd)
                {
                    return scriptChunk.Substring(indEnd + indSign + 1);
                }             
               
            }
            if (indMore >= 0 && indMore < indEnd)
            {

                ushort indVerEnd = Convert.ToUInt16(temp.Substring(indMore + 1, indEnd - indMore - 1).Trim());
                if (versionSQL > indVerEnd)
                {
                    return scriptChunk.Substring(indEnd + indSign + 1);
                }

            }
            
            return string.Empty;

        }       

       
    }

    /// <summary>
    /// Creates a Connection String from the parameters. 
    /// </summary>
    /// <param name="parm1">
    /// </param>
    /// <param name="parm2">
    /// </param>
    /// <param name="parm3">
    /// </param>
    /// <param name="parm4">
    /// </param>
    /// <param name="parm5">
    /// </param>
    /// <param name="parm6">
    /// </param>
    /// <param name="parm7">
    /// </param>
    /// <param name="parm8">
    /// </param>
    /// <param name="parm9">
    /// </param>
    /// <param name="parm10">
    /// </param>
    /// <param name="parm11">
    /// </param>
    /// <param name="parm12">
    /// </param>
    /// <param name="parm13">
    /// </param>
    /// <param name="parm14">
    /// </param>
    /// <param name="parm15">
    /// </param>
    /// <param name="parm16">
    /// </param>
    /// <param name="parm17">
    /// </param>
    /// <param name="parm18">
    /// </param>
    /// <param name="parm19">
    /// </param>
    /// <param name="userID">
    /// </param>
    /// <param name="userPassword">
    /// </param>
    /// <returns>
    /// The get connection string.
    /// </returns>
    public static string GetConnectionString(
      string parm1, 
      string parm2, 
      string parm3, 
      string parm4, 
      string parm5, 
      string parm6, 
      string parm7, 
      string parm8, 
      string parm9, 
      string parm10, 
      bool parm11, 
      bool parm12, 
      bool parm13, 
      bool parm14, 
      bool parm15, 
      bool parm16, 
      bool parm17, 
      bool parm18, 
      bool parm19, 
      string userID, 
      string userPassword)
    {
      // TODO: Parameters should be in a List<ConnectionParameters>
      var connBuilder = new SqlConnectionStringBuilder { DataSource = parm1, InitialCatalog = parm2 };

      if (parm11)
      {
        connBuilder.IntegratedSecurity = true;
      }
      else
      {
        connBuilder.UserID = userID;
        connBuilder.Password = userPassword;
      }

      return connBuilder.ConnectionString;
    }

    /// <summary>
    /// Gets qualified object name
    /// </summary>
    /// <param name="name">
    /// Base name of an object
    /// </param>
    /// <returns>
    /// Returns qualified object name of format {databaseOwner}.{objectQualifier}name
    /// </returns>
    public static string GetObjectName(string name)
    {
      return "[{0}].[{1}{2}]".FormatWith(Config.DatabaseOwner, Config.DatabaseObjectQualifier, name);
    }

    /// <summary>
    /// Test the DB Connection.
    /// </summary>
    /// <param name="exceptionMessage">
    /// outbound ExceptionMessage
    /// </param>
    /// <returns>
    /// true if successfully connected
    /// </returns>
    public static bool TestConnection(out string exceptionMessage)
    {
      exceptionMessage = string.Empty;
      bool success = false;

      try
      {
        using (YafDBConnManager connection = Current.GetConnectionManager())
        {
          // attempt to connect to the db...
          SqlConnection conn = connection.OpenDBConnection;
        }

        // success
        success = true;
      }
      catch (Exception x)
      {
        // unable to connect...
        exceptionMessage = x.Message;
      }

      return success;
    }

    /// <summary>
    /// The get connection manager.
    /// </summary>
    /// <returns>
    /// </returns>
    public YafDBConnManager GetConnectionManager()
    {
      return (YafDBConnManager)Activator.CreateInstance(this._connectionManagerType);
    }

    /// <summary>
    /// Change the Connection Manager used in all DB operations.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public void SetConnectionManagerAdapter<T>()
    {
      Type newConnectionManager = typeof(T);

      if (typeof(IYafDBConnManager).IsAssignableFrom(newConnectionManager))
      {
        this._connectionManagerType = newConnectionManager;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IYafDBAccess

    /// <summary>
    /// Executes a NonQuery
    /// </summary>
    /// <param name="cmd">
    /// NonQuery to execute
    /// </param>
    /// <remarks>
    /// Without transaction
    /// </remarks>
    public void ExecuteNonQuery(SqlCommand cmd)
    {
      // defaults to using a transaction for non-queries
      this.ExecuteNonQuery(cmd, true);
    }

    /// <summary>
    /// The execute non query.
    /// </summary>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <param name="transaction">
    /// The transaction.
    /// </param>
    public void ExecuteNonQuery(SqlCommand cmd, bool transaction)
    {
      using (QueryCounter qc = new QueryCounter(cmd.CommandText))
      {
        using (YafDBConnManager connMan = this.GetConnectionManager())
        {
          // get an open connection
          cmd.Connection = connMan.OpenDBConnection;

          if (transaction)
          {
            // execute using a transaction
            using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(_isolationLevel))
            {
              cmd.Transaction = trans;
              cmd.ExecuteNonQuery();
              trans.Commit();
            }
          }
          else
          {
            // don't use a transaction
            cmd.ExecuteNonQuery();
          }
        }
      }
    }

    /// <summary>
    /// The execute scalar.
    /// </summary>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <returns>
    /// The execute scalar.
    /// </returns>
    public object ExecuteScalar(SqlCommand cmd)
    {
      // default to using a transaction for scaler commands
      return this.ExecuteScalar(cmd, true);
    }

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
    public object ExecuteScalar(SqlCommand cmd, bool transaction)
    {
      using (QueryCounter qc = new QueryCounter(cmd.CommandText))
      {
        using (YafDBConnManager connMan = this.GetConnectionManager())
        {
          // get an open connection
          cmd.Connection = connMan.OpenDBConnection;

          if (transaction)
          {
            // get scalar using a transaction
            using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(_isolationLevel))
            {
              cmd.Transaction = trans;
              object results = cmd.ExecuteScalar();
              trans.Commit();
              return results;
            }
          }
          else
          {
            // get scalar regular
            return cmd.ExecuteScalar();
          }
        }
      }
    }

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
    public DataTable GetData(SqlCommand cmd)
    {
      return GetData(cmd, false);
    }

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
    public DataTable GetData(SqlCommand cmd, bool transaction)
    {
      using (QueryCounter qc = new QueryCounter(cmd.CommandText))
      {
        return this.ProcessUsingResultFilters(this.GetDatasetBasic(cmd, transaction).Tables[0], cmd.CommandText);
      }
    }

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
    public DataTable GetData(string commandText)
    {
      return GetData(commandText, false);
    }

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
    public DataTable GetData(string commandText, bool transaction)
    {
      using (QueryCounter qc = new QueryCounter(commandText))
      {
        using (var cmd = new SqlCommand())
        {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = commandText;
          return this.ProcessUsingResultFilters(this.GetDatasetBasic(cmd, transaction).Tables[0], commandText);
        }
      }
    }

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
    public DataSet GetDataset(SqlCommand cmd)
    {
      return this.GetDataset(cmd, false);
    }

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
    public DataSet GetDataset(SqlCommand cmd, bool transaction)
    {
      using (QueryCounter qc = new QueryCounter(cmd.CommandText))
      {
        return this.GetDatasetBasic(cmd, transaction);
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Process a <see cref="DataTable"/> using Result Filters.
    /// </summary>
    /// <param name="dataTable">data table to process</param>
    /// <param name="sqlCommand"></param>
    /// <returns></returns>
    private DataTable ProcessUsingResultFilters(DataTable dataTable, string sqlCommand)
    {
      string commandCleaned =
        sqlCommand.Replace("[{0}].[{1}".FormatWith(Config.DatabaseOwner, Config.DatabaseObjectQualifier), String.Empty);

      if (commandCleaned.EndsWith("]"))
      {
        // remove last character
        commandCleaned = commandCleaned.Substring(0, commandCleaned.Length - 1);
      }

      // sort filters and process each one...
      this.ResultFilterList.OrderBy(x => x.Rank).ToList().ForEach(i => i.Process(ref dataTable, commandCleaned));

      // return possibility modified dataTable
      return dataTable;
    }

    /// <summary>
    /// Used internally to get data for all the other functions
    /// </summary>
    /// <param name="cmd">
    /// </param>
    /// <param name="transaction">
    /// </param>
    /// <returns>
    /// </returns>
    private DataSet GetDatasetBasic(SqlCommand cmd, bool transaction)
    {
      using (YafDBConnManager connMan = this.GetConnectionManager())
      {
        // see if an existing connection is present
        if (cmd.Connection == null)
        {
          cmd.Connection = connMan.OpenDBConnection;
        }
        else if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Open)
        {
          cmd.Connection.Open();
        }

        // create the adapters
        using (var ds = new DataSet())
        {
          using (var da = new SqlDataAdapter())
          {
            da.SelectCommand = cmd;
            da.SelectCommand.Connection = cmd.Connection;

            // use a transaction
            if (transaction)
            {
              using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(_isolationLevel))
              {
                try
                {
                  da.SelectCommand.Transaction = trans;
                  da.Fill(ds);
                }
                finally
                {
                  trans.Commit();
                }
              }
            }
            else
            {
              // no transaction
              da.Fill(ds);
            }

            // return the dataset
            return ds;
          }
        }
      }
    }

    #endregion
  }
}