// ***********************************************************************
// <copyright file="Inspect.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.IO;
using ServiceStack.Script;
using ServiceStack.Text;

namespace ServiceStack;

/// <summary>
/// Helper utility for inspecting variables
/// </summary>
public static class Inspect
{
    /// <summary>
    /// Class Config.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// The vars name
        /// </summary>
        public const string VarsName = "vars.json";

        /// <summary>
        /// Gets or sets the vars filter.
        /// </summary>
        /// <value>The vars filter.</value>
        public static Action<object> VarsFilter { get; set; } = DefaultVarsFilter;

        /// <summary>
        /// Gets or sets the dump table filter.
        /// </summary>
        /// <value>The dump table filter.</value>
        public static Func<object, string> DumpTableFilter { get; set; }

        /// <summary>
        /// Defaults the vars filter.
        /// </summary>
        /// <param name="anonArgs">The anon arguments.</param>
        public static void DefaultVarsFilter(object anonArgs)
        {
            try
            {
                var inspectVarsPath = Environment.GetEnvironmentVariable("INSPECT_VARS");
                if (string.IsNullOrEmpty(inspectVarsPath)) // Disable
                    return;

                var varsPath = Path.DirectorySeparatorChar == '\\'
                                   ? inspectVarsPath.Replace('/', '\\')
                                   : inspectVarsPath.Replace('\\', '/');

                if (varsPath.IndexOf(Path.DirectorySeparatorChar) >= 0)
                    Path.GetDirectoryName(varsPath).AssertDir();

                File.WriteAllText(varsPath, anonArgs.ToSafeJson());
            }
            catch (Exception ex)
            {
                Tracer.Instance.WriteError("Inspect.vars() Error: " + ex);
            }
        }
    }

    /// <summary>
    /// Dump serialized values to 'vars.json'
    /// </summary>
    /// <param name="anonArgs">Anonymous object with named value</param>
    // ReSharper disable once InconsistentNaming
    public static void vars(object anonArgs) => Config.VarsFilter?.Invoke(anonArgs);

    /// <summary>
    /// Recursively prints the contents of any POCO object in a human-friendly, readable format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    /// <returns>System.String.</returns>
    public static string dump<T>(T instance) => instance.Dump();

    /// <summary>
    /// Print Dump to Console.WriteLine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    public static void printDump<T>(T instance) => PclExport.Instance.WriteLine(dump(instance));

    /// <summary>
    /// Dump object in Ascii Markdown table
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>System.String.</returns>
    public static string dumpTable(object instance) => DefaultScripts.TextDump(instance, null);

    /// <summary>
    /// Print Dump object in Ascii Markdown table
    /// </summary>
    /// <param name="instance">The instance.</param>
    public static void printDumpTable(object instance) => PclExport.Instance.WriteLine(dumpTable(instance));
}