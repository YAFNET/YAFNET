// ***********************************************************************
// <copyright file="ModelConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack
{
    /// <summary>
    /// Class ModelConfig.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelConfig<T>
    {
        /// <summary>
        /// Identifiers the specified get identifier function.
        /// </summary>
        /// <param name="getIdFn">The get identifier function.</param>
        public static void Id(GetMemberDelegate<T> getIdFn)
        {
            IdUtils<T>.CanGetId = getIdFn;
        }
    }
}