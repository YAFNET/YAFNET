// ***********************************************************************
// <copyright file="TableValuedParameter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Used to pass a DataTable as a TableValuedParameter
    /// </summary>
    internal sealed class TableValuedParameter : SqlMapper.ICustomQueryParameter
    {
        /// <summary>
        /// The table
        /// </summary>
        private readonly DataTable table;
        /// <summary>
        /// The type name
        /// </summary>
        private readonly string typeName;

        /// <summary>
        /// Create a new instance of <see cref="TableValuedParameter" />.
        /// </summary>
        /// <param name="table">The <see cref="DataTable" /> to create this parameter for</param>
        public TableValuedParameter(DataTable table) : this(table, null) { /* run base */ }

        /// <summary>
        /// Create a new instance of <see cref="TableValuedParameter" />.
        /// </summary>
        /// <param name="table">The <see cref="DataTable" /> to create this parameter for.</param>
        /// <param name="typeName">The name of the type this parameter is for.</param>
        public TableValuedParameter(DataTable table, string typeName)
        {
            this.table = table;
            this.typeName = typeName;
        }

        /// <summary>
        /// Add the parameter needed to the command before it executes
        /// </summary>
        /// <param name="command">The raw command prior to execution</param>
        /// <param name="name">Parameter name</param>
        void SqlMapper.ICustomQueryParameter.AddParameter(IDbCommand command, string name)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            Set(param, table, typeName);
            command.Parameters.Add(param);
        }

        /// <summary>
        /// Sets the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="table">The table.</param>
        /// <param name="typeName">Name of the type.</param>
        internal static void Set(IDbDataParameter parameter, DataTable table, string typeName)
        {
#pragma warning disable 0618
            parameter.Value = SqlMapper.SanitizeParameterValue(table);
#pragma warning restore 0618
            if (string.IsNullOrEmpty(typeName) && table != null)
            {
                typeName = table.GetTypeName();
            }
            if (!string.IsNullOrEmpty(typeName)) StructuredHelper.ConfigureTVP(parameter, typeName);
        }
    }
}
