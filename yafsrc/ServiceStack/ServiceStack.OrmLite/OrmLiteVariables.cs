// ***********************************************************************
// <copyright file="OrmLiteVariables.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite
{
    /// <summary>
    /// Class OrmLiteVariables.
    /// </summary>
    public static class OrmLiteVariables
    {
        /// <summary>
        /// The system UTC
        /// </summary>
        public const string SystemUtc = "{SYSTEM_UTC}";
        /// <summary>
        /// The maximum text
        /// </summary>
        public const string MaxText = "{MAX_TEXT}";
        /// <summary>
        /// The maximum text unicode
        /// </summary>
        public const string MaxTextUnicode = "{NMAX_TEXT}";
        /// <summary>
        /// The true
        /// </summary>
        public const string True = "{TRUE}";
        /// <summary>
        /// The false
        /// </summary>
        public const string False = "{FALSE}";
    }

    /// <summary>
    /// Class Messages.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// The legacy API
        /// </summary>
        public const string LegacyApi = "API is deprecated and will move to ServiceStack.OrmLite.Legacy namespace in future release";
    }
}