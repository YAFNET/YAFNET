// ***********************************************************************
// <copyright file="UtilExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Extensions
{
    using System;

    /// <summary>
    /// Move conflicting extension methods to ServiceStack.Extensions namespace
    /// </summary>
    public static class UtilExtensions
    {
        /// <summary>
        /// Gets the inner most exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>Exception.</returns>
        public static Exception GetInnerMostException(this Exception ex)
        {
            //Extract true exception from static initializers (e.g. LicenseException)
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }
    }
}