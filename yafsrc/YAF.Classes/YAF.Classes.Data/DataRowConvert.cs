namespace YAF.Classes.Data
{
  #region Using

  using System;
  using System.Data;

  #endregion

  /// <summary>
  /// Helper class to do basic data conversion for a DataRow.
  /// </summary>
  public class DataRowConvert
  {
    #region Constants and Fields

    /// <summary>
    ///   The _db row.
    /// </summary>
    private readonly DataRow _dbRow;

    #endregion

    #region Constructors and Destructors

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

    #endregion

    #region Public Methods

    /// <summary>
    /// The as bool.
    /// </summary>
    /// <param name="columnName">
    /// The column name.
    /// </param>
    /// <returns>
    /// The as bool.
    /// </returns>
    public bool? AsBool(string columnName)
    {
      if (this._dbRow[columnName] == DBNull.Value)
      {
        return null;
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

    #endregion
  }
}