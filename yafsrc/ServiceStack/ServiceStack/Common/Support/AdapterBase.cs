// ***********************************************************************
// <copyright file="AdapterBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Support
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using ServiceStack.Logging;

    /// <summary>
    /// Common functionality when creating adapters
    /// </summary>
    public abstract class AdapterBase
    {
        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
        protected abstract ILog Log { get; }

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>T.</returns>
        protected T Execute<T>(Func<T> action)
        {
            DateTime before = DateTime.UtcNow;
            if (Log.IsDebugEnabled)
                Log.Debug($"Executing action '{action.Method.Name}'");
            try
            {
                T result = action();
                if (Log.IsDebugEnabled)
                {
                    var timeTaken = DateTime.UtcNow - before;
                    this.Log.Debug($"Action '{action.Method.Name}' executed. Took {timeTaken.TotalMilliseconds} ms.");
                }
                return result;
            }
            catch (Exception ex)
            {
                this.Log.Error($"There was an error executing Action '{action.Method.Name}'. Message: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            var before = DateTime.UtcNow;
            if (Log.IsDebugEnabled)
                Log.Debug($"Executing action '{action.Method.Name}'");
            try
            {
                var result = await action();
                if (Log.IsDebugEnabled)
                {
                    var timeTaken = DateTime.UtcNow - before;
                    this.Log.Debug($"Action '{action.Method.Name}' executed. Took {timeTaken.TotalMilliseconds} ms.");
                }
                return result;
            }
            catch (Exception ex)
            {
                this.Log.Error($"There was an error executing Action '{action.Method.Name}'. Message: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        protected async Task<T> ExecuteAsync<T>(Func<CancellationToken,Task<T>> action, CancellationToken token)
        {
            var before = DateTime.UtcNow;
            if (Log.IsDebugEnabled)
                Log.Debug($"Executing action '{action.Method.Name}'");
            try
            {
                var result = await action(token);
                if (Log.IsDebugEnabled)
                {
                    var timeTaken = DateTime.UtcNow - before;
                    this.Log.Debug($"Action '{action.Method.Name}' executed. Took {timeTaken.TotalMilliseconds} ms.");
                }
                return result;
            }
            catch (Exception ex)
            {
                this.Log.Error($"There was an error executing Action '{action.Method.Name}'. Message: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Executes the specified action (for void methods).
        /// </summary>
        /// <param name="action">The action.</param>
        protected void Execute(Action action)
        {
            DateTime before = DateTime.UtcNow;
            if (Log.IsDebugEnabled)
                Log.Debug($"Executing action '{action.Method.Name}'");
            try
            {
                action();
                if (Log.IsDebugEnabled)
                {
                    var timeTaken = DateTime.UtcNow - before;
                    this.Log.Debug($"Action '{action.Method.Name}' executed. Took {timeTaken.TotalMilliseconds} ms.");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error($"There was an error executing Action '{action.Method.Name}'. Message: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Executes the specified action (for void methods).
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected async Task ExecuteAsync(Func<Task> action)
        {
            DateTime before = DateTime.UtcNow;
            if (Log.IsDebugEnabled)
                Log.Debug($"Executing action '{action.Method.Name}'");
            try
            {
                await action();
                if (Log.IsDebugEnabled)
                {
                    var timeTaken = DateTime.UtcNow - before;
                    this.Log.Debug($"Action '{action.Method.Name}' executed. Took {timeTaken.TotalMilliseconds} ms.");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error($"There was an error executing Action '{action.Method.Name}'. Message: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Executes the specified action (for void methods).
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected async Task ExecuteAsync(Func<CancellationToken,Task> action, CancellationToken token)
        {
            DateTime before = DateTime.UtcNow;
            if (Log.IsDebugEnabled)
                Log.Debug($"Executing action '{action.Method.Name}'");
            try
            {
                await action(token);
                if (Log.IsDebugEnabled)
                {
                    var timeTaken = DateTime.UtcNow - before;
                    this.Log.Debug($"Action '{action.Method.Name}' executed. Took {timeTaken.TotalMilliseconds} ms.");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error($"There was an error executing Action '{action.Method.Name}'. Message: {ex.Message}", ex);
                throw;
            }
        }
    }
}