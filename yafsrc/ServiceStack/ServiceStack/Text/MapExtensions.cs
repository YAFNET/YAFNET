// ***********************************************************************
// <copyright file="MapExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text
{
    using ServiceStack.Text.Common;

    using System.Collections.Generic;

    /// <summary>
    /// Class MapExtensions.
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// Joins the specified values.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns>System.String.</returns>
        public static string Join<K, V>(this Dictionary<K, V> values)
        {
            return Join(values, JsWriter.ItemSeperatorString, JsWriter.MapKeySeperatorString);
        }

        /// <summary>
        /// Joins the specified item seperator.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="itemSeperator">The item seperator.</param>
        /// <param name="keySeperator">The key seperator.</param>
        /// <returns>System.String.</returns>
        public static string Join<K, V>(this Dictionary<K, V> values, string itemSeperator, string keySeperator)
        {
            var sb = StringBuilderThreadStatic.Allocate();
            foreach (var entry in values)
            {
                if (sb.Length > 0)
                    sb.Append(itemSeperator);

                sb.Append(entry.Key).Append(keySeperator).Append(entry.Value);
            }
            return StringBuilderThreadStatic.ReturnAndFree(sb);
        }
    }
}