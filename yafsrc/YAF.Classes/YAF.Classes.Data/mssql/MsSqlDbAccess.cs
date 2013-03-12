/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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
  using System.Data.SqlClient;
  using System.Diagnostics;
  using System.Linq;

  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Interfaces.Data;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The yaf db access for SQL Server.
  /// </summary>
  public class MsSqlDbAccess : IDbAccess
  {
    #region Constants and Fields

    /// <summary>
    ///   Result filter list
    /// </summary>
    private readonly IList<IDataTableResultFilter> _resultFilterList = new List<IDataTableResultFilter>();

    /// <summary>
    ///   The _isolation level.
    /// </summary>
    private static IsolationLevel _isolationLevel = IsolationLevel.ReadUncommitted;

    /// <summary>
    ///   The _connection manager type.
    /// </summary>
    private Type _connectionManagerType = typeof(MsSqlDbConnectionManager);

    #endregion

    #region Properties
    
    /// <summary>
    /// Gets LargeForumTree optimization setting.
    /// </summary>
    public static bool LargeForumTree
    {
        get
        {
            return Config.LargeForumTree;
        }
    }
    /// <summary>
    ///   Gets Current IDbAccess -- needs to be switched to direct injection into all DB classes.
    /// </summary>
    public static IDbAccess Current
    {
      get
      {
        return ServiceLocatorAccess.CurrentServiceProvider.Get<IDbAccess>();
      }
    }

    /// <summary>
    ///   Gets IsolationLevel.
    /// </summary>
    public static IsolationLevel IsolationLevel
    {
      get
      {
        return _isolationLevel;
      }
    }

    /// <summary>
    ///   Gets the Result Filter List.
    /// </summary>
    /// <exception cref = "NotImplementedException">
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
    /// A method to handle custom scripts execution tags
    /// </summary>
    /// <param name="scriptChunk">
    /// Input string
    /// </param>
    /// <param name="versionSQL">
    /// SQL server version as ushort
    /// </param>
    /// <returns>
    /// Returns an empty string if condition was not and cleanedfrom tags string if was.
    /// </returns>
    [NotNull]
    public static string CleanForSQLServerVersion([NotNull] string scriptChunk, ushort versionSQL)
    {
      if (!scriptChunk.Contains("#IFSRVVER"))
      {
        return scriptChunk;
      }
      else
      {
        int indSign = scriptChunk.IndexOf("#IFSRVVER") + 9;
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
    public static SqlCommand GetCommand([NotNull] string commandText, bool isText)
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
    public static SqlCommand GetCommand([NotNull] string commandText, bool isText, [NotNull] SqlConnection connection)
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
    [NotNull]
    public static SqlCommand GetCommand([NotNull] string storedProcedure)
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
    [NotNull]
    public static SqlCommand GetCommand([NotNull] string storedProcedure, [NotNull] SqlConnection connection)
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
    [NotNull]
    public static string GetCommandTextReplaced([NotNull] string commandText)
    {
      commandText = commandText.Replace("{databaseOwner}", Config.DatabaseOwner);
      commandText = commandText.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

      return commandText;
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
      [NotNull] string parm1, 
      [NotNull] string parm2, 
      [NotNull] string parm3, 
      [NotNull] string parm4, 
      [NotNull] string parm5, 
      [NotNull] string parm6, 
      [NotNull] string parm7, 
      [NotNull] string parm8, 
      [NotNull] string parm9, 
      [NotNull] string parm10, 
      bool parm11, 
      bool parm12, 
      bool parm13, 
      bool parm14, 
      bool parm15, 
      bool parm16, 
      bool parm17, 
      bool parm18, 
      bool parm19, 
      [NotNull] string userID, 
      [NotNull] string userPassword)
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
    public static string GetObjectName([NotNull] string name)
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
    public static bool TestConnection([NotNull] out string exceptionMessage)
    {
      exceptionMessage = string.Empty;
      bool success = false;

      try
      {
        using (var connection = Current.GetConnectionManager())
        {
          // attempt to connect to the db...
          var conn = connection.OpenDBConnection;
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
    /// Change the Connection Manager used in all DB operations.
    /// </summary>
    /// <typeparam name="TManager">
    /// </typeparam>
    public void SetConnectionManagerAdapter<TManager>()
      where TManager : IDbConnectionManager
    {
      Type newConnectionManager = typeof(TManager);

      if (typeof(IDbConnectionManager).IsAssignableFrom(newConnectionManager))
      {
        this._connectionManagerType = newConnectionManager;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IDbAccess

    /// <summary>
    /// The execute non query.
    /// </summary>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <param name="transaction">
    /// The transaction.
    /// </param>
    public void ExecuteNonQuery([NotNull] IDbCommand cmd, bool transaction)
    {
      using (var qc = new QueryCounter(cmd.CommandText))
      {
        using (var connectionManager = this.GetConnectionManager())
        {
          // get an open connection
          cmd.Connection = connectionManager.OpenDBConnection;

          Trace.WriteLine(cmd.ToDebugString(), "DbAccess");

          if (transaction)
          {
            // execute using a transaction
            using (var trans = connectionManager.OpenDBConnection.BeginTransaction(_isolationLevel))
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
    /// <param name="transaction">
    /// The transaction.
    /// </param>
    /// <returns>
    /// The execute scalar.
    /// </returns>
    public object ExecuteScalar([NotNull] IDbCommand cmd, bool transaction)
    {
      using (var qc = new QueryCounter(cmd.CommandText))
      {
        using (var connectionManager = this.GetConnectionManager())
        {
          // get an open connection
          cmd.Connection = connectionManager.OpenDBConnection;

          Trace.WriteLine(cmd.ToDebugString(), "DbAccess");

          if (transaction)
          {
            // get scalar using a transaction
            using (var trans = connectionManager.OpenDBConnection.BeginTransaction(_isolationLevel))
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
    /// The get connection manager.
    /// </summary>
    /// <returns>
    /// </returns>
    [CanBeNull]
    public IDbConnectionManager GetConnectionManager()
    {
      return Activator.CreateInstance(this._connectionManagerType).ToClass<IDbConnectionManager>();
    }

    // vzrus: The methods should not be implemented and used for other data layers compatability.
    public DataTable AddValuesToDataTableFromReader([NotNull] IDbCommand cmd, DataTable dt, bool transaction, bool acceptChanges,
                                             int firstColumnIndex)
    {
        throw new Exception("Not in use for the data layer.");
    }

    public DataTable AddValuesToDataTableFromReader([NotNull] IDbCommand cmd, DataTable dt, bool transaction, bool acceptChanges,
                                             int firstColumnIndex, int currentRow)
    {
        throw new Exception("Not in use for the data layer.");
    }

    public DataTable GetDataTableFromReader([NotNull] IDbCommand cmd, bool transaction, bool acceptChanges)
    {
        throw new Exception("Not in use for the data layer.");
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
    public DataTable GetData([NotNull] IDbCommand cmd, bool transaction)
    {
      using (var qc = new QueryCounter(cmd.CommandText))
      {
        return this.ProcessUsingResultFilters(this.GetDatasetBasic(cmd, transaction).Tables[0], cmd.CommandText);
      }
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
    public DataTable GetData([NotNull] string commandText, bool transaction)
    {
      using (var qc = new QueryCounter(commandText))
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
    [NotNull]
    public DataSet GetDataset([NotNull] IDbCommand cmd, bool transaction)
    {
      using (var qc = new QueryCounter(cmd.CommandText))
      {
        return this.GetDatasetBasic(cmd, transaction);
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Used internally to get data for all the other functions
    /// </summary>
    /// <param name="cmd">
    /// </param>
    /// <param name="transaction">
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private DataSet GetDatasetBasic([NotNull] IDbCommand cmd, bool transaction)
    {
      using (var connectionManager = this.GetConnectionManager())
      {
        // see if an existing connection is present
        if (cmd.Connection == null)
        {
          cmd.Connection = connectionManager.OpenDBConnection;
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
            da.SelectCommand = (SqlCommand)cmd;
            da.SelectCommand.Connection = (SqlConnection)cmd.Connection;
            Trace.WriteLine(cmd.ToDebugString(), "DbAccess");

            // use a transaction
            if (transaction)
            {
              using (var trans = connectionManager.OpenDBConnection.BeginTransaction(_isolationLevel))
              {
                try
                {
                  da.SelectCommand.Transaction = (SqlTransaction)trans;
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

    /// <summary>
    /// Process a <see cref="DataTable"/> using Result Filters.
    /// </summary>
    /// <param name="dataTable">
    /// data table to process
    /// </param>
    /// <param name="sqlCommand">
    /// </param>
    /// <returns>
    /// </returns>
    private DataTable ProcessUsingResultFilters([NotNull] DataTable dataTable, [NotNull] string sqlCommand)
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

    #endregion
  }
}