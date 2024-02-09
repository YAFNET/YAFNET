// ***********************************************************************
// <copyright file="IntExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System;
using System.Collections.Generic;

namespace ServiceStack
{
    /// <summary>
    /// Class IntExtensions.
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Timeses the specified times.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <returns>IEnumerable&lt;System.Int32&gt;.</returns>
        public static IEnumerable<int> Times(this int times)
        {
            for (var i = 0; i < times; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action<int> actionFn)
        {
            for (var i = 0; i < times; i++)
            {
                actionFn(i);
            }
        }

        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        public static void Times(this int times, Action actionFn)
        {
            for (var i = 0; i < times; i++)
            {
                actionFn();
            }
        }

        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public static List<T> Times<T>(this int times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (var i = 0; i < times; i++)
            {
                list.Add(actionFn());
            }
            return list;
        }

        /// <summary>
        /// Timeses the specified action function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="times">The times.</param>
        /// <param name="actionFn">The action function.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public static List<T> Times<T>(this int times, Func<int, T> actionFn)
        {
            var list = new List<T>();
            for (var i = 0; i < times; i++)
            {
                list.Add(actionFn(i));
            }
            return list;
        }
    }
}