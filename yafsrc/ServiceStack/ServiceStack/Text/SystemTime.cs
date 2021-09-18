// ***********************************************************************
// <copyright file="SystemTime.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.Text
{
    /// <summary>
    /// Class SystemTime.
    /// </summary>
    public static class SystemTime
    {
        /// <summary>
        /// The UTC date time resolver
        /// </summary>
        public static Func<DateTime> UtcDateTimeResolver;

        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
        public static DateTime Now
        {
            get
            {
                var temp = UtcDateTimeResolver;
                return temp == null ? DateTime.Now : temp().ToLocalTime();
            }
        }

        /// <summary>
        /// Gets the UTC now.
        /// </summary>
        /// <value>The UTC now.</value>
        public static DateTime UtcNow
        {
            get
            {
                var temp = UtcDateTimeResolver;
                return temp == null ? DateTime.UtcNow : temp();
            }
        }
    }
}
