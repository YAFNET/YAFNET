// ***********************************************************************
// <copyright file="PerfUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
#if !SL5 && !IOS && !XBOX

using System;
using System.Diagnostics;

namespace ServiceStack
{
    /// <summary>
    /// Class PerfUtils.
    /// </summary>
    public static class PerfUtils
    {
        /// <summary>
        /// Converts to timespan.
        /// </summary>
        /// <param name="fromTicks">From ticks.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan ToTimeSpan(this long fromTicks)
        {
            return TimeSpan.FromSeconds(fromTicks * 1d / Stopwatch.Frequency);
        }

        /// <summary>
        /// Runs an action for a minimum of runForMs
        /// </summary>
        /// <param name="fn">What to run</param>
        /// <param name="runForMs">Minimum ms to run for</param>
        /// <returns>time elapsed in micro seconds</returns>
        public static double MeasureFor(Action fn, int runForMs)
        {
            int iter = 0;
            var watch = new Stopwatch();
            watch.Start();
            long elapsed = 0;
            while (elapsed < runForMs)
            {
                fn();
                elapsed = watch.ElapsedMilliseconds;
                iter++;
            }

            return 1000.0 * elapsed / iter;
        }

        /// <summary>
        /// Returns average microseconds an action takes when run for the specified runForMs
        /// </summary>
        /// <param name="fn">What to run</param>
        /// <param name="times">How many times to run for each iteration</param>
        /// <param name="runForMs">Minimum ms to run for</param>
        /// <param name="setup">The setup.</param>
        /// <param name="warmup">The warmup.</param>
        /// <param name="teardown">The teardown.</param>
        /// <returns>System.Double.</returns>
        public static double Measure(Action fn,
            int times = 1,
            int runForMs = 2000,
            Action setup = null,
            Action warmup = null,
            Action teardown = null)
        {
            setup?.Invoke();

            // Warmup for at least 100ms. Discard result.
            if (warmup == null)
                warmup = fn;

            GC.Collect();

            MeasureFor(() => warmup(), 100);

            // Run the benchmark for at least 2000ms.
            double result = MeasureFor(() =>
            {
                for (var i = 0; i < times; i++)
                    fn();
            }, runForMs);

            teardown?.Invoke();

            return result;
        }
    }
}

#endif