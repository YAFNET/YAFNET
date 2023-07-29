// ***********************************************************************
// <copyright file="Tracer.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.Text;

/// <summary>
/// Class Tracer.
/// </summary>
public class Tracer
{
    /// <summary>
    /// The instance
    /// </summary>
    public static ITracer Instance = new NullTracer();

    /// <summary>
    /// Class NullTracer.
    /// Implements the <see cref="ServiceStack.Text.ITracer" />
    /// </summary>
    /// <seealso cref="ServiceStack.Text.ITracer" />
    public class NullTracer : ITracer
    {
        /// <summary>
        /// Writes the debug.
        /// </summary>
        /// <param name="error">The error.</param>
        public void WriteDebug(string error) { }

        /// <summary>
        /// Writes the debug.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void WriteDebug(string format, params object[] args) { }

        /// <summary>
        /// Writes the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void WriteWarning(string warning) { }

        /// <summary>
        /// Writes the warning.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void WriteWarning(string format, params object[] args) { }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public void WriteError(Exception ex)
        {
            if (JsConfig.ThrowOnError)
                throw ex;
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="System.Exception"></exception>
        public void WriteError(string error)
        {
            if (JsConfig.ThrowOnError)
                throw new Exception(error);
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <exception cref="System.Exception"></exception>
        public void WriteError(string format, params object[] args)
        {
            if (JsConfig.ThrowOnError)
                throw new Exception(string.Format(format, args));
        }
    }

    /// <summary>
    /// Class ConsoleTracer.
    /// Implements the <see cref="ServiceStack.Text.ITracer" />
    /// </summary>
    /// <seealso cref="ServiceStack.Text.ITracer" />
    public class ConsoleTracer : ITracer
    {
        /// <summary>
        /// Writes the debug.
        /// </summary>
        /// <param name="error">The error.</param>
        public void WriteDebug(string error)
        {
            PclExport.Instance.WriteLine(error);
        }

        /// <summary>
        /// Writes the debug.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void WriteDebug(string format, params object[] args)
        {
            PclExport.Instance.WriteLine(format, args);
        }

        /// <summary>
        /// Writes the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void WriteWarning(string warning)
        {
            PclExport.Instance.WriteLine(warning);
        }

        /// <summary>
        /// Writes the warning.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void WriteWarning(string format, params object[] args)
        {
            PclExport.Instance.WriteLine(format, args);
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public void WriteError(Exception ex)
        {
            PclExport.Instance.WriteLine(ex.ToString());
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void WriteError(string error)
        {
            PclExport.Instance.WriteLine(error);
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void WriteError(string format, params object[] args)
        {
            PclExport.Instance.WriteLine(format, args);
        }
    }
}

/// <summary>
/// Class TracerExceptions.
/// </summary>
public static class TracerExceptions
{
    /// <summary>
    /// Traces the specified ex.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ex">The ex.</param>
    /// <returns>T.</returns>
    public static T Trace<T>(this T ex) where T : Exception
    {
        Tracer.Instance.WriteError(ex);
        return ex;
    }
}