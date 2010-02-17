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
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SqlClient;
  using YAF.Classes.Pattern;

  /// <summary>
  /// Helper class to do basic data conversion for a DataRow.	
  /// </summary>
  public class DataRowConvert
  {
    /// <summary>
    /// The _db row.
    /// </summary>
    private DataRow _dbRow;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataRowConvert"/> class.
    /// </summary>
    /// <param name="dbRow">
    /// The db row.
    /// </param>
    public DataRowConvert(DataRow dbRow)
    {
      this._dbRow = dbRow;
    }

    /// <summary>
    /// The as string.
    /// </summary>
    /// <param name="columnName">
    /// The column name.
    /// </param>
    /// <returns>
    /// The as string.
    /// </returns>
    public string AsString(string columnName)
    {
      if (this._dbRow[columnName] == DBNull.Value)
      {
        return null;
      }

      return this._dbRow[columnName].ToString();
    }

    /// <summary>
    /// The as bool.
    /// </summary>
    /// <param name="columnName">
    /// The column name.
    /// </param>
    /// <returns>
    /// The as bool.
    /// </returns>
    public bool AsBool(string columnName)
    {
      if (this._dbRow[columnName] == DBNull.Value)
      {
        return false;
      }

      return Convert.ToBoolean(this._dbRow[columnName]);
    }

    /// <summary>
    /// The as date time.
    /// </summary>
    /// <param name="columnName">
    /// The column name.
    /// </param>
    /// <returns>
    /// </returns>
    public DateTime? AsDateTime(string columnName)
    {
      if (this._dbRow[columnName] == DBNull.Value)
      {
        return null;
      }

      return Convert.ToDateTime(this._dbRow[columnName]);
    }

    /// <summary>
    /// The as int 32.
    /// </summary>
    /// <param name="columnName">
    /// The column name.
    /// </param>
    /// <returns>
    /// </returns>
    public int? AsInt32(string columnName)
    {
      if (this._dbRow[columnName] == DBNull.Value)
      {
        return null;
      }

      return Convert.ToInt32(this._dbRow[columnName]);
    }

    /// <summary>
    /// The as int 64.
    /// </summary>
    /// <param name="columnName">
    /// The column name.
    /// </param>
    /// <returns>
    /// </returns>
    public long? AsInt64(string columnName)
    {
      if (this._dbRow[columnName] == DBNull.Value)
      {
        return null;
      }

      return Convert.ToInt64(this._dbRow[columnName]);
    }
  }

  /// <summary>
  /// The sql data layer converter.
  /// </summary>
  public static class SqlDataLayerConverter
  {
    /// <summary>
    /// The verify int 32.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The verify int 32.
    /// </returns>
    public static int VerifyInt32(object o)
    {
      return Convert.ToInt32(o);
    }

    /// <summary>
    /// The verify bool.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The verify bool.
    /// </returns>
    public static bool VerifyBool(object o)
    {
      return Convert.ToBoolean(o);
    }
  }

  /// <summary>
  /// Class to hold polls and questions data to save
  /// </summary>
  public class PollSaveList
  {
      public PollSaveList()
      {

      }
      public PollSaveList(string pollQuestion, string[] pollChoices, object pollCloses)
      {

          question = pollQuestion;
          choice = pollChoices;
          pollCloses = closes;
      }

      #region Constants and Fields

      /// <summary>
      /// The poll question.
      /// </summary>
      private string question;
      /// <summary>
      /// The Reply options.
      /// </summary>
      private string[] choice;
      /// <summary>
      /// TData whem poll will be closed.
      /// </summary>
      private DateTime closes;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or Sets value for Question text
      /// </summary>
      public string Question
      {
          get
          {
              return question;
          }
          set
          {
              question = value;
          }
      }

      /// <summary>
      /// Gets or Sets value for Question text
      /// </summary>
      public string[] Choice
      {
          get
          {
              return choice;
          }
          set
          {
              choice = value;
          }
      }

      /// <summary>
      /// Gets or Sets value indicatiing when a poll (question) closes
      /// </summary>
      public DateTime Closes
      {
          get
          {
              return closes;
          }
          set
          {
              closes = value;
          }
      }

      #endregion
  }
}
