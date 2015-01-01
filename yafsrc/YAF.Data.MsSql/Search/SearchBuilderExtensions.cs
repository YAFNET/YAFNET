/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Data.MsSql.Search
{
    using System.Collections.Generic;
    using System.Text;

    using YAF.Types;
    using YAF.Types.Extensions;

    /// <summary>
    /// The search builder extensions.
    /// </summary>
    public static class SearchBuilderExtensions
    {
        #region Public Methods

        /// <summary>
        /// Builds the SQL.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        /// <param name="surroundWithParenthesis">The surround with parenthesis.</param>
        /// <returns>
        /// The build SQL.
        /// </returns>
        [NotNull]
        public static string BuildSql(
            [NotNull] this IEnumerable<SearchCondition> conditions,
            bool surroundWithParenthesis)
        {
            var sb = new StringBuilder();

            conditions.ForEachFirst(
                (item, isFirst) =>
                    {
                        sb.Append(" ");
                        if (!isFirst)
                        {
                            sb.Append(item.ConditionType.GetStringValue());
                            sb.Append(" ");
                        }

                        if (surroundWithParenthesis)
                        {
                            sb.AppendFormat("({0})", item.Condition);
                        }
                        else
                        {
                            sb.Append(item.Condition);
                        }
                    });

            return sb.ToString();
        }

        #endregion
    }
}