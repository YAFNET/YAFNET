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
namespace YAF.Core.Data
{
    using System;
    using System.Data;

    /// <summary>
    /// The data reader filter -- initially strikes me as a terrible idea.
    /// </summary>
    public class DataReaderFilter : IDataReader
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderFilter"/> class.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        public DataReaderFilter(IDataReader dataReader)
        {
            this.DataReader = dataReader;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the data reader.
        /// </summary>
        protected IDataReader DataReader { get; set; }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        public virtual int Depth
        {
            get
            {
                return this.DataReader.Depth;
            }
        }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        public virtual int FieldCount
        {
            get
            {
                return this.DataReader.FieldCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is closed.
        /// </summary>
        public virtual bool IsClosed
        {
            get
            {
                return this.DataReader.IsClosed;
            }
        }

        /// <summary>
        /// Gets the records affected.
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return this.DataReader.RecordsAffected;
            }
        }

        #endregion

        #region Explicit Interface Indexers

        /// <summary>
        /// The i data record.this.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object this[int i]
        {
            get
            {
                return this.DataReader[i];
            }
        }

        /// <summary>
        /// The i data record.this.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object this[string name]
        {
            get
            {
                return this.DataReader[name];
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The close.
        /// </summary>
        public virtual void Close()
        {
            this.DataReader.Close();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public virtual void Dispose()
        {
            this.DataReader.Dispose();
        }

        /// <summary>
        /// The get boolean.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool GetBoolean(int i)
        {
            return this.DataReader.GetBoolean(i);
        }

        /// <summary>
        /// The get byte.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public virtual byte GetByte(int i)
        {
            return this.DataReader.GetByte(i);
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="fieldOffset">
        /// The field offset.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="bufferoffset">
        /// The bufferoffset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this.DataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// The get char.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="char"/>.
        /// </returns>
        public virtual char GetChar(int i)
        {
            return this.DataReader.GetChar(i);
        }

        /// <summary>
        /// The get chars.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="fieldoffset">
        /// The fieldoffset.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="bufferoffset">
        /// The bufferoffset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this.DataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        public virtual IDataReader GetData(int i)
        {
            return this.DataReader.GetData(i);
        }

        /// <summary>
        /// The get data type name.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetDataTypeName(int i)
        {
            return this.DataReader.GetDataTypeName(i);
        }

        /// <summary>
        /// The get date time.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public virtual DateTime GetDateTime(int i)
        {
            return this.DataReader.GetDateTime(i);
        }

        /// <summary>
        /// The get decimal.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public virtual decimal GetDecimal(int i)
        {
            return this.DataReader.GetDecimal(i);
        }

        /// <summary>
        /// The get double.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public virtual double GetDouble(int i)
        {
            return this.DataReader.GetDouble(i);
        }

        /// <summary>
        /// The get field type.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public virtual Type GetFieldType(int i)
        {
            return this.DataReader.GetFieldType(i);
        }

        /// <summary>
        /// The get float.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public virtual float GetFloat(int i)
        {
            return this.DataReader.GetFloat(i);
        }

        /// <summary>
        /// The get guid.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public virtual Guid GetGuid(int i)
        {
            return this.DataReader.GetGuid(i);
        }

        /// <summary>
        /// The get int 16.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="short"/>.
        /// </returns>
        public virtual short GetInt16(int i)
        {
            return this.DataReader.GetInt16(i);
        }

        /// <summary>
        /// The get int 32.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int GetInt32(int i)
        {
            return this.DataReader.GetInt32(i);
        }

        /// <summary>
        /// The get int 64.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public virtual long GetInt64(int i)
        {
            return this.DataReader.GetInt64(i);
        }

        /// <summary>
        /// The get name.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetName(int i)
        {
            return this.DataReader.GetName(i);
        }

        /// <summary>
        /// The get ordinal.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int GetOrdinal(string name)
        {
            return this.DataReader.GetOrdinal(name);
        }

        /// <summary>
        /// The get schema table.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public virtual DataTable GetSchemaTable()
        {
            return this.DataReader.GetSchemaTable();
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetString(int i)
        {
            return this.DataReader.GetString(i);
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object GetValue(int i)
        {
            return this.DataReader.GetValue(i);
        }

        /// <summary>
        /// The get values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public virtual int GetValues(object[] values)
        {
            return this.DataReader.GetValues(values);
        }

        /// <summary>
        /// The is db null.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsDBNull(int i)
        {
            return this.DataReader.IsDBNull(i);
        }

        /// <summary>
        /// The next result.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool NextResult()
        {
            return this.DataReader.NextResult();
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool Read()
        {
            return this.DataReader.Read();
        }

        #endregion
    }
}