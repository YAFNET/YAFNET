// ***********************************************************************
// <copyright file="TypeConstants.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceStack.OrmLite.Base.Text;

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
    public readonly static Task<int> ZeroTask;

    /// <summary>
    /// The true task
    /// </summary>
    public readonly static Task<bool> TrueTask;

    /// <summary>
    /// The false task
    /// </summary>
    public readonly static Task<bool> FalseTask;

    /// <summary>
    /// The empty task
    /// </summary>
    public readonly static Task<object> EmptyTask;

    /// <summary>
    /// The non width white space
    /// </summary>
    public const char NonWidthWhiteSpace = (char)0x200B; //Use zero-width space marker to capture empty string

    /// <summary>
    /// The non width white space chars
    /// </summary>
    public static char[] NonWidthWhiteSpaceChars { get; } = [(char)0x200B];

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
    public static string[] EmptyStringArray = [];

    /// <summary>
    /// The empty byte array
    /// </summary>
    public static byte[] EmptyByteArray = [];

    /// <summary>
    /// The empty object array
    /// </summary>
    public static object[] EmptyObjectArray = [];

    /// <summary>
    /// The empty type array
    /// </summary>
    public static Type[] EmptyTypeArray = Type.EmptyTypes;

    /// <summary>
    /// The empty field information array
    /// </summary>
    public static FieldInfo[] EmptyFieldInfoArray = [];

    /// <summary>
    /// The empty property information array
    /// </summary>
    public static PropertyInfo[] EmptyPropertyInfoArray = [];
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
    public static T[] EmptyArray = [];

    /// <summary>
    /// The empty list
    /// </summary>
    public static List<T> EmptyList = [];
}