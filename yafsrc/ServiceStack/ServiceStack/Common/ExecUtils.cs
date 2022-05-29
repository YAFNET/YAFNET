// ***********************************************************************
// <copyright file="ExecUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Logging;
using ServiceStack.Script;
using ServiceStack.Text;

#if NETCORE
using System.Threading.Tasks;
#endif

namespace ServiceStack
{
    /// <summary>
    /// Class ExecUtils.
    /// </summary>
    public static class ExecUtils
    {
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="clientMethodName">Name of the client method.</param>
        /// <param name="ex">The ex.</param>
        public static void LogError(Type declaringType, string clientMethodName, Exception ex)
        {
            var log = LogManager.GetLogger(declaringType);
            log.Error($"'{declaringType.FullName}' threw an error on {clientMethodName}: {ex.Message}", ex);
        }

        /// <summary>
        /// Executes all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The action.</param>
        public static void ExecAll<T>(this IEnumerable<T> instances, Action<T> action)
        {
            foreach (var instance in instances)
            {
                try
                {
                    action(instance);
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }
        }

        /// <summary>
        /// Execute all as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The action.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task ExecAllAsync<T>(this IEnumerable<T> instances, Func<T, Task> action)
        {
            foreach (var instance in instances)
            {
                try
                {
                    await action(instance).ConfigAwait();
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }
        }

        /// <summary>
        /// Execute all return first as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The action.</param>
        /// <returns>A Task&lt;TReturn&gt; representing the asynchronous operation.</returns>
        public static async Task<TReturn> ExecAllReturnFirstAsync<T, TReturn>(this IEnumerable<T> instances, Func<T, Task<TReturn>> action)
        {
            TReturn firstResult = default;
            var i = 0;
            foreach (var instance in instances)
            {
                try
                {
                    var ret = await action(instance).ConfigAwait();
                    if (i++ == 0)
                        firstResult = ret;
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }
            return firstResult;
        }

        /// <summary>
        /// Executes all with first out.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The action.</param>
        /// <param name="firstResult">The first result.</param>
        public static void ExecAllWithFirstOut<T, TReturn>(this IEnumerable<T> instances, Func<T, TReturn> action, ref TReturn firstResult)
        {
            foreach (var instance in instances)
            {
                try
                {
                    var result = action(instance);
                    if (!Equals(firstResult, default(TReturn)))
                    {
                        firstResult = result;
                    }
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }
        }

        /// <summary>
        /// Executes the return first with result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The action.</param>
        /// <returns>TReturn.</returns>
        public static TReturn ExecReturnFirstWithResult<T, TReturn>(this IEnumerable<T> instances, Func<T, TReturn> action)
        {
            foreach (var instance in instances)
            {
                try
                {
                    var result = action(instance);
                    if (!Equals(result, default(TReturn)))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }

            return default;
        }

        /// <summary>
        /// Execute return first with result as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The action.</param>
        /// <returns>A Task&lt;TReturn&gt; representing the asynchronous operation.</returns>
        public static async Task<TReturn> ExecReturnFirstWithResultAsync<T, TReturn>(this IEnumerable<T> instances, Func<T, Task<TReturn>> action)
        {
            foreach (var instance in instances)
            {
                try
                {
                    var result = await action(instance).ConfigAwait();
                    if (!Equals(result, default(TReturn)))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    LogError(instance.GetType(), action.GetType().Name, ex);
                }
            }

            return default;
        }

        /// <summary>
        /// Retries the until true.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="timeOut">The time out.</param>
        /// <exception cref="System.TimeoutException">Exceeded timeout of {timeOut.Value}</exception>
        public static void RetryUntilTrue(Func<bool> action, TimeSpan? timeOut = null)
        {
            var i = 0;
            var firstAttempt = DateTime.UtcNow;

            while (timeOut == null || DateTime.UtcNow - firstAttempt < timeOut.Value)
            {
                i++;
                if (action())
                {
                    return;
                }
                SleepBackOffMultiplier(i);
            }

            throw new TimeoutException($"Exceeded timeout of {timeOut.Value}");
        }

        /// <summary>
        /// Retry until true as an asynchronous operation.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="timeOut">The time out.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="System.TimeoutException">Exceeded timeout of {timeOut.Value}</exception>
        public static async Task RetryUntilTrueAsync(Func<Task<bool>> action, TimeSpan? timeOut = null)
        {
            var i = 0;
            var firstAttempt = DateTime.UtcNow;

            while (timeOut == null || DateTime.UtcNow - firstAttempt < timeOut.Value)
            {
                i++;
                if (await action().ConfigAwait())
                {
                    return;
                }
                await DelayBackOffMultiplierAsync(i).ConfigAwait();
            }

            throw new TimeoutException($"Exceeded timeout of {timeOut.Value}");
        }

        /// <summary>
        /// Retries the on exception.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="timeOut">The time out.</param>
        /// <exception cref="System.TimeoutException">Exceeded timeout of {timeOut.Value}</exception>
        public static void RetryOnException(Action action, TimeSpan? timeOut)
        {
            var i = 0;
            Exception lastEx = null;
            var firstAttempt = DateTime.UtcNow;

            while (timeOut == null || DateTime.UtcNow - firstAttempt < timeOut.Value)
            {
                i++;
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    lastEx = ex;

                    SleepBackOffMultiplier(i);
                }
            }

            throw new TimeoutException($"Exceeded timeout of {timeOut.Value}", lastEx);
        }

        /// <summary>
        /// Retry on exception as an asynchronous operation.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="timeOut">The time out.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="System.TimeoutException">Exceeded timeout of {timeOut.Value}</exception>
        public static async Task RetryOnExceptionAsync(Func<Task> action, TimeSpan? timeOut)
        {
            var i = 0;
            Exception lastEx = null;
            var firstAttempt = DateTime.UtcNow;

            while (timeOut == null || DateTime.UtcNow - firstAttempt < timeOut.Value)
            {
                i++;
                try
                {
                    await action().ConfigAwait();
                    return;
                }
                catch (Exception ex)
                {
                    lastEx = ex;

                    await DelayBackOffMultiplierAsync(i).ConfigAwait();
                }
            }

            throw new TimeoutException($"Exceeded timeout of {timeOut.Value}", lastEx);
        }

        /// <summary>
        /// Retries the on exception.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        public static void RetryOnException(Action action, int maxRetries)
        {
            for (var i = 0; i < maxRetries; i++)
            {
                try
                {
                    action();
                    break;
                }
                catch
                {
                    if (i == maxRetries - 1) throw;

                    SleepBackOffMultiplier(i);
                }
            }
        }

        /// <summary>
        /// Retry on exception as an asynchronous operation.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task RetryOnExceptionAsync(Func<Task> action, int maxRetries)
        {
            for (var i = 0; i < maxRetries; i++)
            {
                try
                {
                    await action().ConfigAwait();
                    break;
                }
                catch
                {
                    if (i == maxRetries - 1) throw;

                    await DelayBackOffMultiplierAsync(i).ConfigAwait();
                }
            }
        }

        /// <summary>
        /// Default base sleep time (milliseconds).
        /// </summary>
        /// <value>The base delay ms.</value>
        public static int BaseDelayMs { get; set; } = 100;

        /// <summary>
        /// Default maximum back-off time before retrying a request
        /// </summary>
        /// <value>The maximum back off ms.</value>
        public static int MaxBackOffMs { get; set; } = 1000 * 20;

        /// <summary>
        /// Maximum retry limit. Avoids integer overflow issues.
        /// </summary>
        /// <value>The maximum retries.</value>
        public static int MaxRetries { get; set; } = 30;

        /// <summary>
        /// How long to sleep before next retry using Exponential BackOff delay with Full Jitter.
        /// </summary>
        /// <param name="retriesAttempted">The retries attempted.</param>
        public static void SleepBackOffMultiplier(int retriesAttempted) => TaskUtils.Sleep(CalculateFullJitterBackOffDelay(retriesAttempted));

        /// <summary>
        /// How long to wait before next retry using Exponential BackOff delay with Full Jitter.
        /// </summary>
        /// <param name="retriesAttempted">The retries attempted.</param>
        /// <returns>Task.</returns>
        public static Task DelayBackOffMultiplierAsync(int retriesAttempted) => Task.Delay(CalculateFullJitterBackOffDelay(retriesAttempted));

        /// <summary>
        /// Exponential BackOff Delay with Full Jitter
        /// </summary>
        /// <param name="retriesAttempted">The retries attempted.</param>
        /// <returns>System.Int32.</returns>
        public static int CalculateFullJitterBackOffDelay(int retriesAttempted) => CalculateFullJitterBackOffDelay(retriesAttempted, BaseDelayMs, MaxBackOffMs);

        /// <summary>
        /// Exponential BackOff Delay with Full Jitter from:
        /// https://github.com/aws/aws-sdk-java/blob/master/aws-java-sdk-core/src/main/java/com/amazonaws/retry/PredefinedBackoffStrategies.java
        /// </summary>
        /// <param name="retriesAttempted">The retries attempted.</param>
        /// <param name="baseDelay">The base delay.</param>
        /// <param name="maxBackOffMs">The maximum back off ms.</param>
        /// <returns>System.Int32.</returns>
        public static int CalculateFullJitterBackOffDelay(int retriesAttempted, int baseDelay, int maxBackOffMs)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var ceil = CalculateExponentialDelay(retriesAttempted, baseDelay, maxBackOffMs);
            return random.Next(ceil);
        }

        /// <summary>
        /// Calculate exponential retry back-off.
        /// </summary>
        /// <param name="retriesAttempted">The retries attempted.</param>
        /// <returns>System.Int32.</returns>
        public static int CalculateExponentialDelay(int retriesAttempted) => CalculateExponentialDelay(retriesAttempted, BaseDelayMs, MaxBackOffMs);

        /// <summary>
        /// Calculate exponential retry back-off.
        /// </summary>
        /// <param name="retriesAttempted">The retries attempted.</param>
        /// <param name="baseDelay">The base delay.</param>
        /// <param name="maxBackOffMs">The maximum back off ms.</param>
        /// <returns>System.Int32.</returns>
        public static int CalculateExponentialDelay(int retriesAttempted, int baseDelay, int maxBackOffMs)
        {
            if (retriesAttempted <= 0)
                return baseDelay;

            var retries = Math.Min(retriesAttempted, MaxRetries);
            return (int)Math.Min((1L << retries) * baseDelay, maxBackOffMs);
        }

        /// <summary>
        /// Calculate back-off logic for obtaining an in memory lock
        /// </summary>
        /// <param name="retries">The retries.</param>
        /// <returns>System.Int32.</returns>
        public static int CalculateMemoryLockDelay(int retries) => retries < 10
            ? CalculateExponentialDelay(retries, baseDelay: 5, maxBackOffMs: 1000)
            : CalculateFullJitterBackOffDelay(retries, baseDelay: 10, maxBackOffMs: 10000);

        /// <summary>
        /// Shells the execute.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string ShellExec(string command, Dictionary<string, object> args = null) =>
            new ProtectedScripts().sh(default, command, args);
    }
}
