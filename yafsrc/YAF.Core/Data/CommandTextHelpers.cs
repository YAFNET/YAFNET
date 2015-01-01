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

namespace YAF.Core.Data
{
    using System;

    using YAF.Classes;
    using YAF.Types;

    public static class CommandTextHelpers
    {
        /// <summary>
        /// A method to handle custom scripts execution tags
        /// </summary>
        /// <param name="scriptChunk">
        /// Input string
        /// </param>
        /// <param name="version">
        /// SQL server version as ushort
        /// </param>
        /// <returns>
        /// Returns an empty string if condition was not and cleanedfrom tags string if was.
        /// </returns>
        [NotNull]
        public static string CleanForServerVersion([NotNull] string scriptChunk, ushort version)
        {
            const string ServerVersionBegin = "#IFSRVVER";

            if (!scriptChunk.Contains(ServerVersionBegin))
            {
                return scriptChunk;
            }

            int indSign = scriptChunk.IndexOf(ServerVersionBegin) + ServerVersionBegin.Length;
            string temp = scriptChunk.Substring(indSign);
            int indEnd = temp.IndexOf("#");
            int indEqual = temp.IndexOf("=");
            int indMore = temp.IndexOf(">");

            if (indEqual >= 0 && indEqual < indEnd)
            {
                ushort indVerEnd = Convert.ToUInt16(temp.Substring(indEqual + 1, indEnd - indEqual - 1).Trim());
                if (version == indVerEnd)
                {
                    return scriptChunk.Substring(indEnd + indSign + 1);
                }
            }

            if (indMore >= 0 && indMore < indEnd)
            {
                ushort indVerEnd = Convert.ToUInt16(temp.Substring(indMore + 1, indEnd - indMore - 1).Trim());
                if (version > indVerEnd)
                {
                    return scriptChunk.Substring(indEnd + indSign + 1);
                }
            }

            return string.Empty;
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
    }
}