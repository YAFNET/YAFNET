/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Data.MySql;

/// <summary>
/// The YAF naming strategy base override.
/// </summary>
public class YafNamingStrategyBaseOverride : INamingStrategy
{
    public Dictionary<string, string> SchemaAliases { get; } = [];
    public Dictionary<string, string> TableAliases { get; } = [];
    public Dictionary<string, string> ColumnAliases { get; } = [];

    /// <summary>
    /// The get column name.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetColumnName(string name)
    {
        return name;
    }

    /// <summary>
    /// Gets the name of the sequence.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>Returns the sequence name</returns>
    public virtual string GetSequenceName(string modelName, string fieldName)
    {
        return $"SEQ_{modelName}_{fieldName}";
    }

    /// <summary>
    /// Applies the name restrictions.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Returns the name</returns>
    public string ApplyNameRestrictions(string name)
    {
        return name;
    }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Returns the name of the table.</returns>
    public string GetTableName(ModelDefinition modelDef)
    {
        return this.GetTableName(modelDef.ModelName);
    }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>Returns the name of the table.</returns>
    public string GetTableName(string name)
    {
        return $"{Config.DatabaseObjectQualifier}{name}";
    }

    /// <summary>
    /// Gets the table alias.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public string GetTableAlias(string name)
    {
        return name is "UserRoles" or "Roles"
            ? name
            : $"{Config.DatabaseObjectQualifier}{name}";
    }

    /// <summary>
    /// Gets the column alias.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public string GetColumnAlias(string name)
    {
        return name;
    }

    /// <summary>
    /// Gets the name of the schema.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Returns the name of the schema</returns>
    public string GetSchemaName(string name)
    {
        return Config.DatabaseOwner;
    }

    /// <summary>
    /// Gets the name of the schema.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Returns the name of the schema</returns>
    public string GetSchemaName(ModelDefinition modelDef)
    {
        return this.GetSchemaName(modelDef.Schema);
    }
}