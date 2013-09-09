using System;
using System.Data;
using System.Data.SqlClient;
using Autofac;
using YAF.Core;
using YAF.Types;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Data;

namespace YAF.Classes.Data
{
    using System.Data.Common;

    public static class DbHelpers
    {
        /// <summary>
        /// Gets LargeForumTree optimization setting.
        /// </summary>
        public static bool LargeForumTree
        {
            get
            {
                return Config.LargeForumTree;
            }
        }

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
        public static string CleanForSQLServerVersion([NotNull] string scriptChunk, ushort versionSQL)
        {
            if (scriptChunk == null) throw new ArgumentNullException("scriptChunk");

            const string serverVersionString = "#IFSRVVER";

            if (!scriptChunk.Contains(serverVersionString))
            {
                return scriptChunk;
            }

            int signIndex = scriptChunk.IndexOf(serverVersionString, System.StringComparison.Ordinal) + serverVersionString.Length;
            string temp = scriptChunk.Substring(signIndex);
            int endIndex = temp.IndexOf("#", System.StringComparison.Ordinal);
            int equalIndex = temp.IndexOf("=", System.StringComparison.Ordinal);
            int moreIndex = temp.IndexOf(">", System.StringComparison.Ordinal);

            ushort versionEnd = 0;

            if (equalIndex >= 0 && equalIndex < endIndex)
            {
                versionEnd = Convert.ToUInt16(temp.Substring(equalIndex + 1, endIndex - equalIndex - 1).Trim());
                if (versionSQL == versionEnd)
                {
                    return scriptChunk.Substring(endIndex + signIndex + 1);
                }
            }

            if (moreIndex < 0 || moreIndex >= endIndex) return String.Empty;

            versionEnd = Convert.ToUInt16(temp.Substring(moreIndex + 1, endIndex - moreIndex - 1).Trim());

            return versionSQL > versionEnd ? scriptChunk.Substring(endIndex + signIndex + 1) : String.Empty;
        }

        /// <summary>
        /// Creates new SqlCommand based on command text applying all qualifiers to the name.
        /// </summary>
        /// <param name="commandText">
        /// Command text to qualify.
        /// </param>
        /// <param name="isText">
        /// Determines whether command text is text or stored procedure.
        /// </param>
        /// <returns>
        /// New SqlCommand
        /// </returns>
        public static SqlCommand GetCommand([NotNull] string commandText, bool isText)
        {
            return GetCommand(commandText, isText, null);
        }

        /// <summary>
        /// Creates new SqlCommand based on command text applying all qualifiers to the name.
        /// </summary>
        /// <param name="commandText">
        /// Command text to qualify.
        /// </param>
        /// <param name="isText">
        /// Determines whether command text is text or stored procedure.
        /// </param>
        /// <param name="connection">
        /// Connection to use with command.
        /// </param>
        /// <returns>
        /// New SqlCommand
        /// </returns>
        public static SqlCommand GetCommand(
            [NotNull] string commandText,
            bool isText,
            [NotNull] IDbConnection connection)
        {
            return isText
                ? new SqlCommand
                  {
                      CommandType = CommandType.Text,
                      CommandText = GetCommandTextReplaced(commandText),
                      Connection = connection as SqlConnection,
                      CommandTimeout = Config.SqlCommandTimeout.ToType<int>()
                  }
                : GetCommand(commandText);
        }

        /// <summary>
        /// Creates new SqlCommand calling stored procedure applying all qualifiers to the name.
        /// </summary>
        /// <param name="storedProcedure">
        /// Base of stored procedure name.
        /// </param>
        /// <returns>
        /// New SqlCommand
        /// </returns>
        [NotNull]
        public static SqlCommand GetCommand([NotNull] string storedProcedure)
        {
            return GetCommand(storedProcedure, null);
        }

        /// <summary>
        /// Creates new SqlCommand calling stored procedure applying all qualifiers to the name.
        /// </summary>
        /// <param name="storedProcedure">
        /// Base of stored procedure name.
        /// </param>
        /// <param name="connection">
        /// Connection to use with command.
        /// </param>
        /// <returns>
        /// New SqlCommand
        /// </returns>
        [NotNull]
        public static SqlCommand GetCommand([NotNull] string storedProcedure, [NotNull] DbConnection connection)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = GetObjectName(storedProcedure),
                Connection = connection as SqlConnection,
                CommandTimeout = Int32.Parse(Config.SqlCommandTimeout)
            };

            return cmd;
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

        /// <summary>
        /// Creates a Connection String from the parameters.
        /// </summary>
        /// <param name="parm1">
        /// </param>
        /// <param name="parm2">
        /// </param>
        /// <param name="parm3">
        /// </param>
        /// <param name="parm4">
        /// </param>
        /// <param name="parm5">
        /// </param>
        /// <param name="parm6">
        /// </param>
        /// <param name="parm7">
        /// </param>
        /// <param name="parm8">
        /// </param>
        /// <param name="parm9">
        /// </param>
        /// <param name="parm10">
        /// </param>
        /// <param name="parm11">
        /// </param>
        /// <param name="parm12">
        /// </param>
        /// <param name="parm13">
        /// </param>
        /// <param name="parm14">
        /// </param>
        /// <param name="parm15">
        /// </param>
        /// <param name="parm16">
        /// </param>
        /// <param name="parm17">
        /// </param>
        /// <param name="parm18">
        /// </param>
        /// <param name="parm19">
        /// </param>
        /// <param name="userID">
        /// </param>
        /// <param name="userPassword">
        /// </param>
        /// <returns>
        /// The get connection string.
        /// </returns>
        public static string GetConnectionString(
            [NotNull] string parm1,
            [NotNull] string parm2,
            [NotNull] string parm3,
            [NotNull] string parm4,
            [NotNull] string parm5,
            [NotNull] string parm6,
            [NotNull] string parm7,
            [NotNull] string parm8,
            [NotNull] string parm9,
            [NotNull] string parm10,
            bool parm11,
            bool parm12,
            bool parm13,
            bool parm14,
            bool parm15,
            bool parm16,
            bool parm17,
            bool parm18,
            bool parm19,
            [NotNull] string userID,
            [NotNull] string userPassword)
        {
            // TODO: Parameters should be in a List<ConnectionParameters>
            var connBuilder = new SqlConnectionStringBuilder { DataSource = parm1, InitialCatalog = parm2 };

            if (parm11)
            {
                connBuilder.IntegratedSecurity = true;
            }
            else
            {
                connBuilder.UserID = userID;
                connBuilder.Password = userPassword;
            }

            return connBuilder.ConnectionString;
        }

        /// <summary>
        /// Gets qualified object name
        /// </summary>
        /// <param name="name">
        /// Base name of an object
        /// </param>
        /// <returns>
        /// Returns qualified object name of format {databaseOwner}.{objectQualifier}name
        /// </returns>
        public static string GetObjectName([NotNull] string name)
        {
            return "[{0}].[{1}{2}]".FormatWith(Config.DatabaseOwner, Config.DatabaseObjectQualifier, name);
        }
    }
}