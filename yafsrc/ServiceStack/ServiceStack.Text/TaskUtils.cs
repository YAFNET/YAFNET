// ***********************************************************************
// <copyright file="TaskUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack
{
    /// <summary>
    /// Class TaskUtils.
    /// </summary>
    public static class TaskUtils
    {
        /// <summary>
        /// Froms the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T> FromResult<T>(T result) => Task.FromResult(result);

        /// <summary>
        /// Ins the task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T> InTask<T>(this T result) => Task.FromResult(result);

        /// <summary>
        /// Ins the task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex">The ex.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T> InTask<T>(this Exception ex)
        {
            var taskSource = new TaskCompletionSource<T>();
            taskSource.TrySetException(ex);
            return taskSource.Task;
        }

        /// <summary>
        /// Determines whether the specified task is success.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns><c>true</c> if the specified task is success; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccess(this Task task) => !task.IsFaulted && task.IsCompleted;

        /// <summary>
        /// Casts the specified task.
        /// </summary>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <typeparam name="To">The type of to.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>Task&lt;To&gt;.</returns>
        public static Task<To> Cast<From, To>(this Task<From> task) where To : From => task.Then(x => (To)x);

        /// <summary>
        /// Safes the task scheduler.
        /// </summary>
        /// <returns>TaskScheduler.</returns>
        public static TaskScheduler SafeTaskScheduler()
        {
            //Unit test runner
            if (SynchronizationContext.Current != null)
                return TaskScheduler.FromCurrentSynchronizationContext();

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            return TaskScheduler.FromCurrentSynchronizationContext();
        }

        /// <summary>
        /// Thens the specified function.
        /// </summary>
        /// <typeparam name="From">The type of from.</typeparam>
        /// <typeparam name="To">The type of to.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="fn">The function.</param>
        /// <returns>Task&lt;To&gt;.</returns>
        public static Task<To> Then<From, To>(this Task<From> task, Func<From, To> fn)
        {
            var tcs = new TaskCompletionSource<To>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(fn(t.Result));
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        /// <summary>
        /// Thens the specified function.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="fn">The function.</param>
        /// <returns>Task.</returns>
        public static Task Then(this Task task, Func<Task, Task> fn)
        {
            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(fn(t));
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        //http://stackoverflow.com/a/13904811/85785
        /// <summary>
        /// Eaches the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="fn">The function.</param>
        /// <returns>Task.</returns>
        public static Task EachAsync<T>(this IEnumerable<T> items, Func<T, int, Task> fn)
        {
            var tcs = new TaskCompletionSource<object>();

            var enumerator = items.GetEnumerator();
            var i = 0;

            Action<Task> next = null;
            next = t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    StartNextIteration(tcs, fn, enumerator, ref i, next);
            };

            StartNextIteration(tcs, fn, enumerator, ref i, next);

            tcs.Task.ContinueWith(_ => enumerator.Dispose(), TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        /// <summary>
        /// Starts the next iteration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tcs">The TCS.</param>
        /// <param name="fn">The function.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="i">The i.</param>
        /// <param name="next">The next.</param>
        static void StartNextIteration<T>(TaskCompletionSource<object> tcs,
            Func<T, int, Task> fn,
            IEnumerator<T> enumerator,
            ref int i,
            Action<Task> next)
        {
            bool moveNext;
            try
            {
                moveNext = enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
                return;
            }

            if (!moveNext)
            {
                tcs.SetResult(null);
                return;
            }

            Task iterationTask = null;
            try
            {
                iterationTask = fn(enumerator.Current, i);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            i++;

            iterationTask?.ContinueWith(next, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        /// <summary>
        /// Sleeps the specified time ms.
        /// </summary>
        /// <param name="timeMs">The time ms.</param>
        public static void Sleep(int timeMs)
        {
            Thread.Sleep(timeMs);
        }

        /// <summary>
        /// Sleeps the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        public static void Sleep(TimeSpan time)
        {
            Thread.Sleep(time);
        }
    }
}