// ***********************************************************************
// <copyright file="FuncUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using ServiceStack.Logging;

namespace ServiceStack;

using ServiceStack.Text;

/// <summary>
/// Class FuncUtils.
/// </summary>
public static class FuncUtils
{
    /// <summary>
    /// The log
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(FuncUtils));

    /// <summary>
    /// Invokes the action provided and returns true if no excpetion was thrown.
    /// Otherwise logs the exception and returns false if an exception was thrown.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryExec(Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
        }
        return false;
    }

    /// <summary>
    /// Tries the execute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func">The function.</param>
    /// <returns>T.</returns>
    public static T TryExec<T>(Func<T> func)
    {
        return TryExec(func, default(T));
    }

    /// <summary>
    /// Tries the execute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func">The function.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>T.</returns>
    public static T TryExec<T>(Func<T> func, T defaultValue)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
        }
        return default(T);
    }

#if !SL5 //SL5 - No Stopwatch, Net Standard 1.1 - no Thread 
    /// <summary>
    /// Waits the while.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="millisecondTimeout">The millisecond timeout.</param>
    /// <param name="millsecondPollPeriod">The millsecond poll period.</param>
    /// <exception cref="System.TimeoutException">Timed out waiting for condition function.</exception>
    public static void WaitWhile(Func<bool> condition, int millisecondTimeout, int millsecondPollPeriod = 10)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (condition())
        {
            TaskUtils.Sleep(millsecondPollPeriod);

            if (timer.ElapsedMilliseconds > millisecondTimeout)
                throw new TimeoutException("Timed out waiting for condition function.");
        }
    }
#endif

}