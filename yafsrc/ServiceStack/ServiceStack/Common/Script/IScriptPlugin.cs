// ***********************************************************************
// <copyright file="IScriptPlugin.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Script
{
    /// <summary>
    /// Interface IScriptPlugin
    /// </summary>
    public interface IScriptPlugin
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Register(ScriptContext context);
    }

    /// <summary>
    /// Interface IScriptPluginBefore
    /// </summary>
    public interface IScriptPluginBefore
    {
        /// <summary>
        /// Befores the plugins loaded.
        /// </summary>
        /// <param name="context">The context.</param>
        void BeforePluginsLoaded(ScriptContext context);
    }

    /// <summary>
    /// Interface IScriptPluginAfter
    /// </summary>
    public interface IScriptPluginAfter
    {
        /// <summary>
        /// Afters the plugins loaded.
        /// </summary>
        /// <param name="context">The context.</param>
        void AfterPluginsLoaded(ScriptContext context);
    }
}