// ***********************************************************************
// <copyright file="DefaultScripts.ErrorHandling.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceStack.Script
{
    using ServiceStack.Text;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Class DefaultScripts.
    /// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
    /// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptMethods" />
    /// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
    public partial class DefaultScripts
    {
        /// <summary>
        /// Assigns the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="errorBinding">The error binding.</param>
        /// <returns>System.Object.</returns>
        public object assignError(ScriptScopeContext scope, string errorBinding)
        {
            scope.PageResult.AssignExceptionsTo = errorBinding;
            return StopExecution.Value;
        }

        /// <summary>
        /// Catches the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="errorBinding">The error binding.</param>
        /// <returns>System.Object.</returns>
        public object catchError(ScriptScopeContext scope, string errorBinding)
        {
            scope.PageResult.CatchExceptionsIn = errorBinding;
            return StopExecution.Value;
        }

        /// <summary>
        /// Assigns the error and continue executing.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="errorBinding">The error binding.</param>
        /// <returns>System.Object.</returns>
        public object assignErrorAndContinueExecuting(ScriptScopeContext scope, string errorBinding)
        {
            assignError(scope, errorBinding);
            return continueExecutingFiltersOnError(scope);
        }

        /// <summary>
        /// Continues the executing filters on error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ignoreTarget">The ignore target.</param>
        /// <returns>System.Object.</returns>
        public object continueExecutingFiltersOnError(ScriptScopeContext scope, object ignoreTarget) => continueExecutingFiltersOnError(scope);
        /// <summary>
        /// Continues the executing filters on error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public object continueExecutingFiltersOnError(ScriptScopeContext scope)
        {
            scope.PageResult.SkipExecutingFiltersIfError = false;
            return StopExecution.Value;
        }

        /// <summary>
        /// Skips the executing filters on error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ignoreTarget">The ignore target.</param>
        /// <returns>System.Object.</returns>
        public object skipExecutingFiltersOnError(ScriptScopeContext scope, object ignoreTarget) => skipExecutingFiltersOnError(scope);
        /// <summary>
        /// Skips the executing filters on error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public object skipExecutingFiltersOnError(ScriptScopeContext scope)
        {
            scope.PageResult.SkipExecutingFiltersIfError = true;
            return StopExecution.Value;
        }

        /// <summary>
        /// Ends if error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public object endIfError(ScriptScopeContext scope) => scope.PageResult.LastFilterError != null ? (object)StopExecution.Value : IgnoreResult.Value;
        /// <summary>
        /// Ends if error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public object endIfError(ScriptScopeContext scope, object value) => scope.PageResult.LastFilterError != null ? StopExecution.Value : value;

        /// <summary>
        /// Ifs the no error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public object ifNoError(ScriptScopeContext scope) => scope.PageResult.LastFilterError != null ? (object)StopExecution.Value : IgnoreResult.Value;
        /// <summary>
        /// Ifs the no error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public object ifNoError(ScriptScopeContext scope, object value) => scope.PageResult.LastFilterError != null ? StopExecution.Value : value;

        /// <summary>
        /// Ifs the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ignoreTarget">The ignore target.</param>
        /// <returns>System.Object.</returns>
        public object ifError(ScriptScopeContext scope, object ignoreTarget) => ifError(scope);
        /// <summary>
        /// Ifs the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public object ifError(ScriptScopeContext scope) => (object)scope.PageResult.LastFilterError ?? StopExecution.Value;
        /// <summary>
        /// Ifs the debug.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ignoreTarget">The ignore target.</param>
        /// <returns>System.Object.</returns>
        public object ifDebug(ScriptScopeContext scope, object ignoreTarget) => ifDebug(scope);
        /// <summary>
        /// Ifs the debug.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public object ifDebug(ScriptScopeContext scope) => scope.Context.DebugMode ? (object)IgnoreResult.Value : StopExecution.Value;
        /// <summary>
        /// Debugs the mode.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public object debugMode(ScriptScopeContext scope) => scope.Context.DebugMode;

        /// <summary>
        /// Determines whether the specified scope has error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns><c>true</c> if the specified scope has error; otherwise, <c>false</c>.</returns>
        public bool hasError(ScriptScopeContext scope) => scope.PageResult.LastFilterError != null;

        /// <summary>
        /// Lasts the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>Exception.</returns>
        public Exception lastError(ScriptScopeContext scope) => scope.PageResult.LastFilterError;
        /// <summary>
        /// Lasts the error message.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.String.</returns>
        public string lastErrorMessage(ScriptScopeContext scope) => scope.PageResult.LastFilterError?.Message;
        /// <summary>
        /// Lasts the error stack trace.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.String.</returns>
        public string lastErrorStackTrace(ScriptScopeContext scope) => scope.PageResult.LastFilterStackTrace?.Length > 0
            ? scope.PageResult.LastFilterStackTrace.Map(x => "   at " + x).Join(Environment.NewLine)
            : null;

        /// <summary>
        /// Ensures all arguments not null.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public object ensureAllArgsNotNull(ScriptScopeContext scope, object args) => ensureAllArgsNotNull(scope, args, null);
        /// <summary>
        /// Ensures all arguments not null.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAllArgsNotNull)}' expects a non empty Object Dictionary</exception>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAllArgsNotNull)}' expects an Object Dictionary but received a '{args.GetType().Name}'</exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ServiceStack.Script.StopFilterExecutionException"></exception>
        public object ensureAllArgsNotNull(ScriptScopeContext scope, object args, object options)
        {
            try
            {
                var filterArgs = options.AssertOptions(nameof(ensureAllArgsNotNull));
                var message = filterArgs.TryGetValue("message", out object oMessage) ? oMessage as string : null;

                if (args is IDictionary<string, object> argsMap)
                {
                    if (argsMap.Count == 0)
                        throw new NotSupportedException($"'{nameof(ensureAllArgsNotNull)}' expects a non empty Object Dictionary");

                    var keys = argsMap.Keys.OrderBy(x => x);
                    foreach (var key in keys)
                    {
                        var value = argsMap[key];
                        if (!isNull(value))
                            continue;

                        if (message != null)
                            throw new ArgumentException(string.Format(message, key));

                        throw new ArgumentNullException(key);
                    }
                    return args;
                }
                throw new NotSupportedException($"'{nameof(ensureAllArgsNotNull)}' expects an Object Dictionary but received a '{args.GetType().Name}'");
            }
            catch (Exception ex)
            {
                throw new StopFilterExecutionException(scope, options, ex);
            }
        }

        /// <summary>
        /// Ensures any arguments not null.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public object ensureAnyArgsNotNull(ScriptScopeContext scope, object args) => ensureAnyArgsNotNull(scope, args, null);
        /// <summary>
        /// Ensures any arguments not null.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAnyArgsNotNull)}' expects a non empty Object Dictionary</exception>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAnyArgsNotNull)}' expects an Object Dictionary but received a '{args.GetType().Name}'</exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ServiceStack.Script.StopFilterExecutionException"></exception>
        public object ensureAnyArgsNotNull(ScriptScopeContext scope, object args, object options)
        {
            try
            {
                var filterArgs = options.AssertOptions(nameof(ensureAnyArgsNotNull));
                var message = filterArgs.TryGetValue("message", out object oMessage) ? oMessage as string : null;

                if (args is IDictionary<string, object> argsMap)
                {
                    if (argsMap.Count == 0)
                        throw new NotSupportedException($"'{nameof(ensureAnyArgsNotNull)}' expects a non empty Object Dictionary");

                    var keys = argsMap.Keys.OrderBy(x => x);
                    foreach (var key in keys)
                    {
                        var value = argsMap[key];
                        if (!isNull(value))
                            return args;
                    }

                    var firstKey = argsMap.Keys.OrderBy(x => x).First();
                    if (message != null)
                        throw new ArgumentException(string.Format(message, firstKey));

                    throw new ArgumentNullException(firstKey);
                }
                throw new NotSupportedException($"'{nameof(ensureAnyArgsNotNull)}' expects an Object Dictionary but received a '{args.GetType().Name}'");
            }
            catch (Exception ex)
            {
                throw new StopFilterExecutionException(scope, options, ex);
            }
        }

        /// <summary>
        /// Ensures all arguments not empty.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public object ensureAllArgsNotEmpty(ScriptScopeContext scope, object args) => ensureAllArgsNotEmpty(scope, args, null);
        /// <summary>
        /// Ensures all arguments not empty.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAllArgsNotEmpty)}' expects a non empty Object Dictionary</exception>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAllArgsNotEmpty)}' expects an Object Dictionary but received a '{args.GetType().Name}'</exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ServiceStack.Script.StopFilterExecutionException"></exception>
        public object ensureAllArgsNotEmpty(ScriptScopeContext scope, object args, object options)
        {
            try
            {
                var filterArgs = options.AssertOptions(nameof(ensureAllArgsNotEmpty));
                var message = filterArgs.TryGetValue("message", out object oMessage) ? oMessage as string : null;

                if (args is IDictionary<string, object> argsMap)
                {
                    if (argsMap.Count == 0)
                        throw new NotSupportedException($"'{nameof(ensureAllArgsNotEmpty)}' expects a non empty Object Dictionary");

                    var keys = argsMap.Keys.OrderBy(x => x);
                    foreach (var key in keys)
                    {
                        var value = argsMap[key];
                        if (!isEmpty(value))
                            continue;

                        if (message != null)
                            throw new ArgumentException(string.Format(message, key));

                        throw new ArgumentNullException(key);
                    }
                    return args;
                }
                throw new NotSupportedException($"'{nameof(ensureAllArgsNotEmpty)}' expects an Object Dictionary but received a '{args.GetType().Name}'");
            }
            catch (Exception ex)
            {
                throw new StopFilterExecutionException(scope, options, ex);
            }
        }

        /// <summary>
        /// Ensures any arguments not empty.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public object ensureAnyArgsNotEmpty(ScriptScopeContext scope, object args) => ensureAnyArgsNotEmpty(scope, args, null);
        /// <summary>
        /// Ensures any arguments not empty.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAnyArgsNotEmpty)}' expects a non empty Object Dictionary</exception>
        /// <exception cref="System.NotSupportedException">'{nameof(ensureAnyArgsNotEmpty)}' expects an Object Dictionary but received a '{args.GetType().Name}'</exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ServiceStack.Script.StopFilterExecutionException"></exception>
        public object ensureAnyArgsNotEmpty(ScriptScopeContext scope, object args, object options)
        {
            try
            {
                var filterArgs = options.AssertOptions(nameof(ensureAnyArgsNotEmpty));
                var message = filterArgs.TryGetValue("message", out object oMessage) ? oMessage as string : null;

                if (args is IDictionary<string, object> argsMap)
                {
                    if (argsMap.Count == 0)
                        throw new NotSupportedException($"'{nameof(ensureAnyArgsNotEmpty)}' expects a non empty Object Dictionary");

                    var keys = argsMap.Keys.OrderBy(x => x);
                    foreach (var key in keys)
                    {
                        var value = argsMap[key];
                        if (!isEmpty(value))
                            return args;
                    }

                    var firstKey = argsMap.Keys.OrderBy(x => x).First();
                    if (message != null)
                        throw new ArgumentException(string.Format(message, firstKey));

                    throw new ArgumentNullException(firstKey);
                }
                throw new NotSupportedException($"'{nameof(ensureAnyArgsNotEmpty)}' expects an Object Dictionary but received a '{args.GetType().Name}'");
            }
            catch (Exception ex)
            {
                throw new StopFilterExecutionException(scope, options, ex);
            }
        }

        /// <summary>
        /// Ifs the throw.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object ifThrow(ScriptScopeContext scope, bool test, string message) => test
            ? new Exception(message).InStopFilter(scope, null)
            : StopExecution.Value;
        /// <summary>
        /// Ifs the throw.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object ifThrow(ScriptScopeContext scope, bool test, string message, object options) => test
            ? new Exception(message).InStopFilter(scope, options)
            : StopExecution.Value;

        /// <summary>
        /// Throws if.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <returns>System.Object.</returns>
        public object throwIf(ScriptScopeContext scope, string message, bool test) => test
            ? new Exception(message).InStopFilter(scope, null)
            : StopExecution.Value;
        /// <summary>
        /// Throws if.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwIf(ScriptScopeContext scope, string message, bool test, object options) => test
            ? new Exception(message).InStopFilter(scope, options)
            : StopExecution.Value;

        /// <summary>
        /// Ifs the throw argument exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object ifThrowArgumentException(ScriptScopeContext scope, bool test, string message) => test
            ? new ArgumentException(message).InStopFilter(scope, null)
            : StopExecution.Value;

        /// <summary>
        /// Ifs the throw argument exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object ifThrowArgumentException(ScriptScopeContext scope, bool test, string message, object options)
        {
            if (!test)
                return StopExecution.Value;

            if (options is string paramName)
                return new ArgumentException(message, paramName).InStopFilter(scope, null);

            return new ArgumentException(message).InStopFilter(scope, options);
        }

        /// <summary>
        /// Ifs the throw argument exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="message">The message.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object ifThrowArgumentException(ScriptScopeContext scope, bool test, string message, string paramName, object options) => test
            ? new ArgumentException(message, paramName).InStopFilter(scope, options)
            : StopExecution.Value;

        /// <summary>
        /// Ifs the throw argument null exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>System.Object.</returns>
        public object ifThrowArgumentNullException(ScriptScopeContext scope, bool test, string paramName) => test
            ? new ArgumentNullException(paramName).InStopFilter(scope, null)
            : StopExecution.Value;
        /// <summary>
        /// Ifs the throw argument null exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object ifThrowArgumentNullException(ScriptScopeContext scope, bool test, string paramName, object options) => test
            ? new ArgumentNullException(paramName).InStopFilter(scope, options)
            : StopExecution.Value;

        /// <summary>
        /// Throws the argument null exception if.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <returns>System.Object.</returns>
        public object throwArgumentNullExceptionIf(ScriptScopeContext scope, string paramName, bool test) => test
            ? new ArgumentNullException(paramName).InStopFilter(scope, null)
            : StopExecution.Value;
        /// <summary>
        /// Throws the argument null exception if.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwArgumentNullExceptionIf(ScriptScopeContext scope, string paramName, bool test, object options) => test
            ? new ArgumentNullException(paramName).InStopFilter(scope, options)
            : StopExecution.Value;

        /// <summary>
        /// Throws the argument exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object throwArgumentException(ScriptScopeContext scope, string message) => new ArgumentException(message).InStopFilter(scope, null);
        /// <summary>
        /// Throws the argument exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwArgumentException(ScriptScopeContext scope, string message, string options) => ifThrowArgumentException(scope, true, message, options);
        /// <summary>
        /// Throws the argument null exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>System.Object.</returns>
        public object throwArgumentNullException(ScriptScopeContext scope, string paramName) => new ArgumentNullException(paramName).InStopFilter(scope, null);
        /// <summary>
        /// Throws the argument null exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwArgumentNullException(ScriptScopeContext scope, string paramName, object options) => new ArgumentNullException(paramName).InStopFilter(scope, options);
        /// <summary>
        /// Throws the not supported exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object throwNotSupportedException(ScriptScopeContext scope, string message) => new NotSupportedException(message).InStopFilter(scope, null);
        /// <summary>
        /// Throws the not supported exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwNotSupportedException(ScriptScopeContext scope, string message, object options) => new NotSupportedException(message).InStopFilter(scope, options);
        /// <summary>
        /// Throws the not implemented exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object throwNotImplementedException(ScriptScopeContext scope, string message) => new NotImplementedException(message).InStopFilter(scope, null);
        /// <summary>
        /// Throws the not implemented exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwNotImplementedException(ScriptScopeContext scope, string message, object options) => new NotImplementedException(message).InStopFilter(scope, options);
        /// <summary>
        /// Throws the unauthorized access exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object throwUnauthorizedAccessException(ScriptScopeContext scope, string message) => new UnauthorizedAccessException(message).InStopFilter(scope, null);
        /// <summary>
        /// Throws the unauthorized access exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwUnauthorizedAccessException(ScriptScopeContext scope, string message, object options) => new UnauthorizedAccessException(message).InStopFilter(scope, options);
        /// <summary>
        /// Throws the file not found exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object throwFileNotFoundException(ScriptScopeContext scope, string message) => new FileNotFoundException(message).InStopFilter(scope, null);
        /// <summary>
        /// Throws the file not found exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwFileNotFoundException(ScriptScopeContext scope, string message, object options) => new FileNotFoundException(message).InStopFilter(scope, options);
        /// <summary>
        /// Throws the optimistic concurrency exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <returns>System.Object.</returns>
        public object throwOptimisticConcurrencyException(ScriptScopeContext scope, string message) => new Data.OptimisticConcurrencyException(message).InStopFilter(scope, null);
        /// <summary>
        /// Throws the optimistic concurrency exception.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        public object throwOptimisticConcurrencyException(ScriptScopeContext scope, string message, object options) => new Data.OptimisticConcurrencyException(message).InStopFilter(scope, options);

        /// <summary>
        /// Throw as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <returns>A Task&lt;System.Object&gt; representing the asynchronous operation.</returns>
        public async Task<object> @throwAsync(ScriptScopeContext scope, string message)
        {
            await Task.Yield();
            return new Exception(message).InStopFilter(scope, null);
        }

        /// <summary>
        /// Throw as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        /// <returns>A Task&lt;System.Object&gt; representing the asynchronous operation.</returns>
        public async Task<object> @throwAsync(ScriptScopeContext scope, string message, object options)
        {
            await Task.Yield();
            return new Exception(message).InStopFilter(scope, options);
        }
    }
}