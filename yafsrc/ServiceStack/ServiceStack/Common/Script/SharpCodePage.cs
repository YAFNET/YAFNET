// ***********************************************************************
// <copyright file="SharpCodePage.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceStack.Script
{
    using ServiceStack.Text;

    /// <summary>
    /// Class SharpCodePage.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public abstract class SharpCodePage : IDisposable
    {
        /// <summary>
        /// Gets or sets the virtual path.
        /// </summary>
        /// <value>The virtual path.</value>
        public string VirtualPath { get; set; }
        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        public string Layout { get; set; }
        /// <summary>
        /// Gets or sets the layout page.
        /// </summary>
        /// <value>The layout page.</value>
        public SharpPage LayoutPage { get; set; }
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public PageFormat Format { get; set; }
        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public Dictionary<string, object> Args { get; } = new Dictionary<string, object>();
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public ScriptContext Context { get; set; }
        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public ISharpPages Pages { get; set; }
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public ScriptScopeContext Scope { get; set; }

        /// <summary>
        /// The render method
        /// </summary>
        private MethodInfo renderMethod;
        /// <summary>
        /// The render invoker
        /// </summary>
        private MethodInvoker renderInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpCodePage"/> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        protected SharpCodePage(string layout = null)
        {
            Layout = layout;
        }

        /// <summary>
        /// Write as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="System.Reflection.TargetInvocationException">Failed to invoke render method on '{GetType().Name}': {ex.Message}</exception>
        public async Task WriteAsync(ScriptScopeContext scope)
        {
            var renderParams = renderMethod.GetParameters();
            var args = new object[renderParams.Length];

            Dictionary<string, string> requestParams = null;

            for (var i = 0; i < renderParams.Length; i++)
            {
                var renderParam = renderParams[i];
                var arg = scope.GetValue(renderParam.Name);
                if (arg == null)
                {
                    if (requestParams == null)
                        requestParams = (scope.GetValue("Request") as Web.IRequest)?.GetRequestParams();

                    if (requestParams != null && requestParams.TryGetValue(renderParam.Name, out var reqParam))
                        arg = reqParam;
                }

                args[i] = arg;
            }

            try
            {
                var result = renderInvoker(this, args);
                if (result != null)
                {
                    var str = result.ToString();
                    await scope.OutputStream.WriteAsync(str);
                }
            }
            catch (Exception ex)
            {
                throw new TargetInvocationException($"Failed to invoke render method on '{GetType().Name}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has initialize.
        /// </summary>
        /// <value><c>true</c> if this instance has initialize; otherwise, <c>false</c>.</value>
        public bool HasInit { get; private set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>SharpCodePage.</returns>
        /// <exception cref="System.NotSupportedException">Template Code Page '{GetType().Name}' does not have a 'render' method</exception>
        public virtual SharpCodePage Init()
        {
            if (!HasInit)
            {
                HasInit = true;
                var type = GetType();

                if (Format == null)
                    Format = Context.PageFormats.First();

                var pageAttr = type.FirstAttribute<PageAttribute>();
                VirtualPath = pageAttr.VirtualPath;
                if (Layout == null)
                    Layout = pageAttr?.Layout;

                LayoutPage = Pages.ResolveLayoutPage(this, Layout);

                var pageArgs = type.AllAttributes<PageArgAttribute>();
                foreach (var pageArg in pageArgs)
                {
                    Args[pageArg.Name] = pageArg.Value;
                }

                if (!Context.CodePageInvokers.TryGetValue(type, out var tuple))
                {
                    var method = type.GetInstanceMethods().FirstOrDefault(x => x.Name.EndsWithIgnoreCase("render"));
                    if (method == null)
                        throw new NotSupportedException($"Template Code Page '{GetType().Name}' does not have a 'render' method");

                    var invoker = TypeExtensions.GetInvokerToCache(method);
                    Context.CodePageInvokers[type] = tuple = Tuple.Create(method, invoker);
                }

                renderMethod = tuple.Item1;
                renderInvoker = tuple.Item2;
            }

            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}