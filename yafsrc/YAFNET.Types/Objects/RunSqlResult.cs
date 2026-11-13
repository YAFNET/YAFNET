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
namespace YAF.Types.Objects;

using System.Collections.Generic;
using System.Text;

/// <summary>
/// The result of a SQL statement run on the Admin > Run SQL Query page.
/// </summary>
public class RunSqlResult
{
    /// <summary>
    /// The maximum number of rows read from each result set.
    /// </summary>
    public const int MaxRows = 1000;

    /// <summary>
    /// Gets the result sets (one for each select of the statement).
    /// </summary>
    public List<RunSqlResultSet> ResultSets { get; } = [];

    /// <summary>
    /// Gets or sets the number of records affected (-1 when no records were changed).
    /// </summary>
    public int RecordsAffected { get; set; } = -1;

    /// <summary>
    /// Gets or sets the error message, when the statement failed.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Gets the message shown when there is no result set: the records affected or "No Results Returned.".
    /// </summary>
    public string Message =>
        this.RecordsAffected > 0 ? $"{this.RecordsAffected} Record(s) Affected" : "No Results Returned.";

    /// <summary>
    /// Returns the result as text: each result set as CSV, the records affected and the error.
    /// </summary>
    /// <returns>The result as text.</returns>
    public override string ToString()
    {
        var text = new StringBuilder();

        foreach (var resultSet in this.ResultSets)
        {
            text.Append("RowNumber");

            foreach (var column in resultSet.Columns)
            {
                text.Append(',').Append(column);
            }

            text.AppendLine();

            var rowNumber = 1;

            foreach (var row in resultSet.Rows)
            {
                text.Append('"').Append(rowNumber++).Append('"');

                foreach (var value in row)
                {
                    text.Append(",\"").Append(value?.Replace("\"", "\"\"")).Append('"');
                }

                text.AppendLine();
            }

            if (resultSet.Truncated)
            {
                text.AppendLine($"(only the first {MaxRows} rows are shown)");
            }

            text.AppendLine();
        }

        if (this.ResultSets.Count == 0 || this.RecordsAffected > 0)
        {
            text.AppendLine(this.Message);
        }

        if (this.Error is not null)
        {
            text.AppendLine().Append("SQL ERROR: ").AppendLine(this.Error);
        }

        return text.ToString();
    }
}

/// <summary>
/// One result set of a SQL statement.
/// </summary>
public class RunSqlResultSet
{
    /// <summary>
    /// Gets the column names.
    /// </summary>
    public List<string> Columns { get; } = [];

    /// <summary>
    /// Gets the rows (the values as text, null for DBNull).
    /// </summary>
    public List<string[]> Rows { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the result set has more than <see cref="RunSqlResult.MaxRows"/> rows.
    /// </summary>
    public bool Truncated { get; set; }
}