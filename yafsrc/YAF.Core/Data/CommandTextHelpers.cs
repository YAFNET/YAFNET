/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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