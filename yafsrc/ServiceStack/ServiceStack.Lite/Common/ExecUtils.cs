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
        public async static Task ExecAllAsync<T>(this IEnumerable<T> instances, Func<T, Task> action)
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
    }
}
