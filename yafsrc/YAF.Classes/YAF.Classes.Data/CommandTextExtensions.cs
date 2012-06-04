namespace YAF.Classes.Data
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using YAF.Types;
	using YAF.Utils;

	public static class DbStringExtensions
	{
		/// <summary>
		/// A method to handle custom scripts execution tags
		/// </summary>
		/// <param name="scriptChunk">
		/// Input string
		/// </param>
		/// <param name="versionSQL">
		/// SQL server version as ushort
		/// </param>
		/// <returns>
		/// Returns an empty string if condition was not and cleanedfrom tags string if was.
		/// </returns>
		[NotNull]
		public static string CleanForSQLServerVersion([NotNull] this string scriptChunk, ushort versionSQL)
		{
			if (!scriptChunk.Contains("#IFSRVVER"))
			{
				return scriptChunk;
			}
			else
			{
				int indSign = scriptChunk.IndexOf("#IFSRVVER") + 9;
				string temp = scriptChunk.Substring(indSign);
				int indEnd = temp.IndexOf("#");
				int indEqual = temp.IndexOf("=");
				int indMore = temp.IndexOf(">");

				if (indEqual >= 0 && indEqual < indEnd)
				{
					ushort indVerEnd = Convert.ToUInt16(temp.Substring(indEqual + 1, indEnd - indEqual - 1).Trim());
					if (versionSQL == indVerEnd)
					{
						return scriptChunk.Substring(indEnd + indSign + 1);
					}
				}

				if (indMore >= 0 && indMore < indEnd)
				{
					ushort indVerEnd = Convert.ToUInt16(temp.Substring(indMore + 1, indEnd - indMore - 1).Trim());
					if (versionSQL > indVerEnd)
					{
						return scriptChunk.Substring(indEnd + indSign + 1);
					}
				}

				return string.Empty;
			}
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
		public static string GetCommandTextReplaced([NotNull] this string commandText)
		{
			commandText = commandText.Replace("{databaseOwner}", Config.DatabaseOwner);
			commandText = commandText.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

			return commandText;
		}		
	}
}