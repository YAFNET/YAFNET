// ***********************************************************************
// <copyright file="OrmLiteConflictResolutions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class ConflictResolution.
/// </summary>
public class ConflictResolution
{
    /// <summary>
    /// The ignore
    /// </summary>
    public const string Ignore = "IGNORE";
}

/// <summary>
/// Class OrmLiteConflictResolutions.
/// </summary>
public static class OrmLiteConflictResolutions
{
    /// <param name="dbCmd">The database command.</param>
    extension(IDbCommand dbCmd)
    {
        /// <summary>
        /// Called when [conflict ignore].
        /// </summary>
        public void OnConflictIgnore()
        {
            dbCmd.OnConflict(ConflictResolution.Ignore);
        }

        /// <summary>
        /// Called when [conflict].
        /// </summary>
        /// <param name="conflictResolution">The conflict resolution.</param>
        /// <exception cref="System.NotSupportedException">Cannot specify ON CONFLICT resolution on Invalid SQL starting with: " + dbCmd.CommandText.SubstringWithEllipsis(0, 50)</exception>
        public void OnConflict(string conflictResolution)
        {
            var pos = dbCmd.CommandText?.IndexOf(' ') ?? -1;
            if (pos == -1)
            {
                throw new NotSupportedException("Cannot specify ON CONFLICT resolution on Invalid SQL starting with: " + dbCmd.CommandText.SubstringWithEllipsis(0, 50));
            }

            var sqlConflict = dbCmd.GetDialectProvider().SqlConflict(dbCmd.CommandText, conflictResolution);
            dbCmd.CommandText = sqlConflict;
        }
    }
}