/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
  using System;
  using System.Data;
  using System.Data.SqlClient;
  using YAF.Classes.Pattern;

  #region Events

  /// <summary>
  /// The yaf db conn info message event handler.
  /// </summary>
  /// <param name="sender">
  /// The sender.
  /// </param>
  /// <param name="e">
  /// The e.
  /// </param>
  public delegate void YafDBConnInfoMessageEventHandler(object sender, YafDBConnInfoMessageEventArgs e);

  /// <summary>
  /// The yaf db conn info message event args.
  /// </summary>
  public class YafDBConnInfoMessageEventArgs : EventArgs
  {
    /// <summary>
    /// The _message.
    /// </summary>
    private string _message;

    /// <summary>
    /// Initializes a new instance of the <see cref="YafDBConnInfoMessageEventArgs"/> class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    public YafDBConnInfoMessageEventArgs(string message)
    {
      Message = message;
    }

    /// <summary>
    /// Gets or sets Message.
    /// </summary>
    public string Message
    {
      get
      {
        return this._message;
      }

      set
      {
        this._message = value;
      }
    }
  }

  #endregion

  public interface IYafDBConnManager
  {
    /// <summary>
    /// Gets ConnectionString.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the current DB Connection in any state.
    /// </summary>
    SqlConnection DBConnection { get; }

    /// <summary>
    /// Gets an open connection to the DB. Can be called any number of times.
    /// </summary>
    SqlConnection OpenDBConnection { get; }

    /// <summary>
    /// The info message.
    /// </summary>
    event YafDBConnInfoMessageEventHandler InfoMessage;

    /// <summary>
    /// The init connection.
    /// </summary>
    void InitConnection();

    /// <summary>
    /// The close connection.
    /// </summary>
    void CloseConnection();
  }

  /// <summary>
  /// Provides open/close management for DB Connections
  /// </summary>
  public class YafDBConnManager : IDisposable, IYafDBConnManager
  {
    /// <summary>
    /// The _connection.
    /// </summary>
    protected SqlConnection _connection = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="YafDBConnManager"/> class.
    /// </summary>
    public YafDBConnManager()
    {
      // just initalize it (not open)
      InitConnection();
    }

    /// <summary>
    /// Gets ConnectionString.
    /// </summary>
    public virtual string ConnectionString
    {
      get
      {
        return Config.ConnectionString;
      }
    }

    /// <summary>
    /// Gets the current DB Connection in any state.
    /// </summary>
    public SqlConnection DBConnection
    {
      get
      {
        InitConnection();
        return this._connection;
      }
    }

    /// <summary>
    /// Gets an open connection to the DB. Can be called any number of times.
    /// </summary>
    public SqlConnection OpenDBConnection
    {
      get
      {
        InitConnection();

        if (this._connection.State != ConnectionState.Open)
        {
          // open it up...
          this._connection.Open();
        }

        return this._connection;
      }
    }

    #region IDisposable Members

    /// <summary>
    /// The dispose.
    /// </summary>
    public virtual void Dispose()
    {
      // close and delete connection
      CloseConnection();
      this._connection = null;
    }

    #endregion

    /// <summary>
    /// The info message.
    /// </summary>
    public event YafDBConnInfoMessageEventHandler InfoMessage;

    /// <summary>
    /// The init connection.
    /// </summary>
    public void InitConnection()
    {
      if (this._connection == null)
      {
        // create the connection
        this._connection = new SqlConnection();
        this._connection.InfoMessage += new SqlInfoMessageEventHandler(Connection_InfoMessage);
        this._connection.ConnectionString = ConnectionString;
      }
      else if (this._connection.State != ConnectionState.Open)
      {
        // verify the connection string is in there...
        this._connection.ConnectionString = ConnectionString;
      }
    }

    /// <summary>
    /// The close connection.
    /// </summary>
    public void CloseConnection()
    {
      if (this._connection != null && this._connection.State != ConnectionState.Closed)
      {
        this._connection.Close();
      }
    }

    /// <summary>
    /// The connection_ info message.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      if (InfoMessage != null)
      {
        InfoMessage(this, new YafDBConnInfoMessageEventArgs(e.Message));
      }
    }
  }
}
