// ***********************************************************************
// <copyright file="SqliteGuidConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Sqlite.Converters
{
    using System;
    using System.Data;

    using ServiceStack.OrmLite.Converters;

    using System.Collections.Generic;

    /// <summary>
    /// Class SqliteGuidConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.GuidConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.GuidConverter" />
    public class SqliteGuidConverter : GuidConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "CHAR(36)";

        /// <summary>
        /// Quoted Value in SQL Statement
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string ToQuotedString(Type fieldType, object value)
        {
            var guid = (Guid)value;
            var bytes = guid.ToByteArray();
            var fmt = "x'" + BitConverter.ToString(bytes).Replace("-", "") + "'";
            return fmt;
        }

        /// <summary>
        /// Retrieve Value from ADO.NET IDataReader. Defaults to reader.GetValue()
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="values">The values.</param>
        /// <returns>System.Object.</returns>
        public override object GetValue(IDataReader reader, int columnIndex, object[] values)
        {
            if (values != null)
            {
                if (values[columnIndex] == DBNull.Value)
                    return null;
            }
            else
            {
                if (reader.IsDBNull(columnIndex))
                    return null;
            }

            return reader.GetGuid(columnIndex);
        }
    }

    /// <summary>
    /// Class SqliteDataGuidConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.GuidConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.GuidConverter" />
    public class SqliteDataGuidConverter : GuidConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "CHAR(36)";

        /// <summary>
        /// Quoted Value in SQL Statement
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string ToQuotedString(Type fieldType, object value)
        {
            var guid = (Guid)value;
            var bytes = guid.ToByteArray();
            SwapEndian(bytes);
            var p = BitConverter.ToString(bytes).Replace("-","");
            var fmt = "'" + p.Substring(0,8) + "-" 
                + p.Substring(8,4) + "-" 
                + p.Substring(12,4) + "-" 
                + p.Substring(16,4) + "-"
                + p.Substring(20) + "'";
            return fmt;
        }

        /// <summary>
        /// Swaps the endian.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <exception cref="System.ArgumentNullException">guid</exception>
        public static void SwapEndian(byte[] guid)
        {
            _ = guid ?? throw new ArgumentNullException(nameof(guid));
            Swap(guid, 0, 3);
            Swap(guid, 1, 2);
            Swap(guid, 4, 5);
            Swap(guid, 6, 7);
        }

        /// <summary>
        /// Swaps the specified array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index1">The index1.</param>
        /// <param name="index2">The index2.</param>
        private static void Swap(IList<byte> array, int index1, int index2)
        {
            var temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="values">The values.</param>
        /// <returns>System.Object.</returns>
        public override object GetValue(IDataReader reader, int columnIndex, object[] values)
        {
            if (values != null)
            {
                if (values[columnIndex] == DBNull.Value)
                {
                    return null;
                }
            }
            else
            {
                if (reader.IsDBNull(columnIndex))
                {
                    return null;
                }
            }

            return reader.GetGuid(columnIndex);
        }
    }
    
}