// ***********************************************************************
// <copyright file="TypeConstants.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Class TypeConstants.
    /// </summary>
    public static class TypeConstants
    {
        /// <summary>
        /// Cctors this instance.
        /// </summary>
        static TypeConstants()
        {
            ZeroTask = InTask(0);
            TrueTask = InTask(true);
            FalseTask = InTask(false);
            EmptyTask = InTask((object)null);
        }

        /// <summary>
        /// Ins the task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <returns>System.Threading.Tasks.Task&lt;T&gt;.</returns>
        private static Task<T> InTask<T>(this T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>
        /// The zero task
        /// </summary>
        public static readonly Task<int> ZeroTask;
        /// <summary>
        /// The true task
        /// </summary>
        public static readonly Task<bool> TrueTask;
        /// <summary>
        /// The false task
        /// </summary>
        public static readonly Task<bool> FalseTask;
        /// <summary>
        /// The empty task
        /// </summary>
        public static readonly Task<object> EmptyTask;

        /// <summary>
        /// The non width white space
        /// </summary>
        public const char NonWidthWhiteSpace = (char)0x200B; //Use zero-width space marker to capture empty string
        /// <summary>
        /// The non width white space chars
        /// </summary>
        public static char[] NonWidthWhiteSpaceChars = { (char)0x200B };

        /// <summary>
        /// Gets the null string span.
        /// </summary>
        /// <value>The null string span.</value>
        public static ReadOnlySpan<char> NullStringSpan => default;
        /// <summary>
        /// Gets the empty string span.
        /// </summary>
        /// <value>The empty string span.</value>
        public static ReadOnlySpan<char> EmptyStringSpan => new(NonWidthWhiteSpaceChars);

        /// <summary>
        /// Gets the null string memory.
        /// </summary>
        /// <value>The null string memory.</value>
        public static ReadOnlyMemory<char> NullStringMemory => default;
        /// <summary>
        /// Gets the empty string memory.
        /// </summary>
        /// <value>The empty string memory.</value>
        public static ReadOnlyMemory<char> EmptyStringMemory => "".AsMemory();

        /// <summary>
        /// The empty string array
        /// </summary>
        public static string[] EmptyStringArray = Array.Empty<string>();
        /// <summary>
        /// The empty long array
        /// </summary>
        public static long[] EmptyLongArray = Array.Empty<long>();
        /// <summary>
        /// The empty int array
        /// </summary>
        public static int[] EmptyIntArray = Array.Empty<int>();
        /// <summary>
        /// The empty character array
        /// </summary>
        public static char[] EmptyCharArray = Array.Empty<char>();
        /// <summary>
        /// The empty bool array
        /// </summary>
        public static bool[] EmptyBoolArray = Array.Empty<bool>();
        /// <summary>
        /// The empty byte array
        /// </summary>
        public static byte[] EmptyByteArray = Array.Empty<byte>();
        /// <summary>
        /// The empty object array
        /// </summary>
        public static object[] EmptyObjectArray = Array.Empty<object>();
        /// <summary>
        /// The empty type array
        /// </summary>
        public static Type[] EmptyTypeArray = Type.EmptyTypes;
        /// <summary>
        /// The empty field information array
        /// </summary>
        public static FieldInfo[] EmptyFieldInfoArray = Array.Empty<FieldInfo>();
        /// <summary>
        /// The empty property information array
        /// </summary>
        public static PropertyInfo[] EmptyPropertyInfoArray = Array.Empty<PropertyInfo>();
        /// <summary>
        /// The empty object dictionary
        /// </summary>
        public static Dictionary<string, object> EmptyObjectDictionary = new();

        /// <summary>
        /// The empty string list
        /// </summary>
        public static List<string> EmptyStringList = new(0);
        /// <summary>
        /// The empty long list
        /// </summary>
        public static List<long> EmptyLongList = new(0);
        /// <summary>
        /// The empty int list
        /// </summary>
        public static List<int> EmptyIntList = new(0);
        /// <summary>
        /// The empty character list
        /// </summary>
        public static List<char> EmptyCharList = new(0);
        /// <summary>
        /// The empty bool list
        /// </summary>
        public static List<bool> EmptyBoolList = new(0);
        /// <summary>
        /// The empty object list
        /// </summary>
        public static List<object> EmptyObjectList = new(0);
        /// <summary>
        /// The empty type list
        /// </summary>
        public static List<Type> EmptyTypeList = new(0);
    }

    /// <summary>
    /// Class TypeConstants.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TypeConstants<T>
    {
        /// <summary>
        /// The empty array
        /// </summary>
        public static T[] EmptyArray = Array.Empty<T>();
        /// <summary>
        /// The empty list
        /// </summary>
        public static List<T> EmptyList = new(0);
        /// <summary>
        /// The empty hash set
        /// </summary>
        public static HashSet<T> EmptyHashSet = new();
    }
}