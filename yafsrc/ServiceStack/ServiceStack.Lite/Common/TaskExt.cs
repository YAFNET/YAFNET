// ***********************************************************************
// <copyright file="TaskExt.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Threading.Tasks;

using ServiceStack.Text;

/// <summary>
/// Class TaskExt.
/// </summary>
public static class TaskExt
{
    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>System.Object.</returns>
    public static object GetResult(this Task task)
    {
        try
        {
            if (!task.IsCompleted)
                task.Wait();

            if (task is Task<object> taskObj)
                return taskObj.Result;

            var taskType = task.GetType();
            if (!taskType.IsGenericType || taskType.FullName.Contains("VoidTaskResult"))
                return null;

            var props = TypeProperties.Get(taskType);
            var fn = props.GetPublicGetter("Result");
            return fn?.Invoke(task);
        }
        catch (TypeAccessException)
        {
            return null; //return null for void Task's
        }
        catch (Exception ex)
        {
            throw ex.UnwrapIfSingleException();
        }
    }
}