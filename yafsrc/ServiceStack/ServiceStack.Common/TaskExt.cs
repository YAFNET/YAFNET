// ***********************************************************************
// <copyright file="TaskExt.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack
{
    /// <summary>
    /// Class TaskExt.
    /// </summary>
    public static class TaskExt
    {
        /// <summary>
        /// Ases the task exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public static Task<object> AsTaskException(this Exception ex)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        /// <summary>
        /// Ases the task exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex">The ex.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<T> AsTaskException<T>(this Exception ex)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        /// <summary>
        /// Ases the task result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<T> AsTaskResult<T>(this T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

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

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns>T.</returns>
        public static T GetResult<T>(this Task<T> task)
        {
            return (T)((Task)task).GetResult();
        }

        /// <summary>
        /// The synchronize task factory
        /// </summary>
        private static readonly TaskFactory SyncTaskFactory = new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
        /// <summary>
        /// Runs the synchronize.
        /// </summary>
        /// <param name="task">The task.</param>
        public static void RunSync(Func<Task> task) => SyncTaskFactory.StartNew(task).Unwrap().GetAwaiter().GetResult();
        /// <summary>
        /// Runs the synchronize.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>TResult.</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task) => SyncTaskFactory.StartNew(task).Unwrap().GetAwaiter().GetResult();

#if NET48 || NET5_0_OR_GREATER
        /// <summary>
        /// Ases the value task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>ValueTask.</returns>
        public static ValueTask AsValueTask(this Task task) => new ValueTask(task);
        /// <summary>
        /// Ases the value task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns>ValueTask&lt;T&gt;.</returns>
        public static ValueTask<T> AsValueTask<T>(this Task<T> task) => new ValueTask<T>(task);
#endif
    }
}