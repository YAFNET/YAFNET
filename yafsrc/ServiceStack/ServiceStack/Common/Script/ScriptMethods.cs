// ***********************************************************************
// <copyright file="ScriptMethods.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ServiceStack.Model;
using ServiceStack.Text;
using ServiceStack.Web;

namespace ServiceStack.Script
{
    /// <summary>
    /// Enum InvokerType
    /// </summary>
    public enum InvokerType
    {
        /// <summary>
        /// The filter
        /// </summary>
        Filter,
        /// <summary>
        /// The context filter
        /// </summary>
        ContextFilter,
        /// <summary>
        /// The context block
        /// </summary>
        ContextBlock,
    }

    /// <summary>
    /// Interface IResultInstruction
    /// </summary>
    public interface IResultInstruction { }
    /// <summary>
    /// Class IgnoreResult.
    /// Implements the <see cref="ServiceStack.Script.IResultInstruction" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.IResultInstruction" />
    public class IgnoreResult : IResultInstruction
    {
        /// <summary>
        /// The value
        /// </summary>
        public static readonly IgnoreResult Value = new();
        /// <summary>
        /// Prevents a default instance of the <see cref="IgnoreResult"/> class from being created.
        /// </summary>
        private IgnoreResult() { }
    }

    /// <summary>
    /// Class StopExecution.
    /// Implements the <see cref="ServiceStack.Script.IResultInstruction" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.IResultInstruction" />
    public class StopExecution : IResultInstruction
    {
        /// <summary>
        /// The value
        /// </summary>
        public static StopExecution Value = new();
        /// <summary>
        /// Prevents a default instance of the <see cref="StopExecution"/> class from being created.
        /// </summary>
        private StopExecution() { }
    }

    /// <summary>
    /// Class StopFilterExecutionException.
    /// Implements the <see cref="ServiceStack.StopExecutionException" />
    /// Implements the <see cref="ServiceStack.Model.IResponseStatusConvertible" />
    /// </summary>
    /// <seealso cref="ServiceStack.StopExecutionException" />
    /// <seealso cref="ServiceStack.Model.IResponseStatusConvertible" />
    public class StopFilterExecutionException : StopExecutionException, IResponseStatusConvertible
    {
        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public ScriptScopeContext Scope { get; }
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public object Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopFilterExecutionException"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="options">The options.</param>
        /// <param name="innerException">The inner exception.</param>
        public StopFilterExecutionException(ScriptScopeContext scope, object options, Exception innerException)
            : base(nameof(StopFilterExecutionException), innerException)
        {
            Scope = scope;
            Options = options;
        }

        /// <summary>
        /// Converts to responsestatus.
        /// </summary>
        /// <returns>ResponseStatus.</returns>
        public ResponseStatus ToResponseStatus()
        {
            var ex = InnerException ?? this;
            return new ResponseStatus
            {
                ErrorCode = ex.GetType().Name,
                Message = ex.Message,
                StackTrace = ex.StackTrace.LeftPart('\n'),
                Meta = new Dictionary<string, string>
                {
                    [GetType().Name] = Message + "\n" + this.StackTrace.LeftPart('\n')
                }
            };
        }
    }

    /// <summary>
    /// Class ScriptException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ScriptException : Exception
    {
        /// <summary>
        /// Gets the page result.
        /// </summary>
        /// <value>The page result.</value>
        public PageResult PageResult { get; }
        /// <summary>
        /// Gets the page stack trace.
        /// </summary>
        /// <value>The page stack trace.</value>
        public string PageStackTrace => PageResult.LastFilterStackTrace.Map(x => "   at " + x).Join(Environment.NewLine);

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        public ScriptException(PageResult pageResult) : base(
            pageResult.LastFilterError?.Message ?? throw new ArgumentNullException(nameof(pageResult)),
            pageResult.LastFilterError)
        {
            PageResult = pageResult;
        }
    }

    /// <summary>
    /// Class ScriptMethods.
    /// </summary>
    public class ScriptMethods
    {
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
        /// The lookup index
        /// </summary>
        private readonly Dictionary<string, MethodInfo> lookupIndex = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptMethods"/> class.
        /// </summary>
        public ScriptMethods()
        {
            var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                var paramType = method.GetParameters().FirstOrDefault()?.ParameterType;
                var hasScope = paramType == typeof(ScriptScopeContext);
                var hasTaskReturnType = method.ReturnType == typeof(Task);
                var isFilter = !hasScope && !hasTaskReturnType;
                var isContextFilter = hasScope && !hasTaskReturnType;
                var isBlockFilter = hasScope && hasTaskReturnType;
                if (!isFilter && !isContextFilter && !isBlockFilter)
                    continue;

                var filterType = isBlockFilter
                    ? InvokerType.ContextBlock
                    : isContextFilter
                        ? InvokerType.ContextFilter
                        : InvokerType.Filter;

                var key = CacheKey(filterType, method.Name, method.GetParameters().Length);
                lookupIndex[key] = method;
            }
        }

        /// <summary>
        /// Caches the key.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="argsCount">The arguments count.</param>
        /// <returns>System.String.</returns>
        private string CacheKey(InvokerType type, string methodName, int argsCount) =>
            type + "::" + methodName + "`" + argsCount;

        /// <summary>
        /// Gets the filter method.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>MethodInfo.</returns>
        private MethodInfo GetFilterMethod(string cacheKey) => lookupIndex.TryGetValue(cacheKey, out MethodInfo method) ? method : null;

        /// <summary>
        /// Queries the filters.
        /// </summary>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>List&lt;MethodInfo&gt;.</returns>
        public List<MethodInfo> QueryFilters(string filterName)
        {
            var filters = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.Name.IndexOf(filterName, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            return filters;
        }

        /// <summary>
        /// Gets the invoker cache.
        /// </summary>
        /// <value>The invoker cache.</value>
        public ConcurrentDictionary<string, MethodInvoker> InvokerCache { get; } = new();

        /// <summary>
        /// Gets the invoker.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="argsCount">The arguments count.</param>
        /// <param name="type">The type.</param>
        /// <returns>MethodInvoker.</returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public MethodInvoker GetInvoker(string name, int argsCount, InvokerType type)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var key = CacheKey(type, name, argsCount);
            if (InvokerCache.TryGetValue(key, out MethodInvoker invoker))
                return invoker;

            var method = GetFilterMethod(key);
            if (method == null)
                return null;

            return InvokerCache[key] = TypeExtensions.GetInvokerToCache(method);
        }
    }

    /// <summary>
    /// Class TemplateFilterUtils.
    /// </summary>
    public static class TemplateFilterUtils
    {
        /// <summary>
        /// Asserts the options.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="scopedParams">The scoped parameters.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static Dictionary<string, object> AssertOptions(this ScriptScopeContext scope, string filterName, object scopedParams)
        {
            var pageParams = scopedParams as Dictionary<string, object>;
            if (pageParams == null && scopedParams != null)
                throw new ArgumentException(
                    $"{filterName} in '{scope.PageResult.VirtualPath}' only accepts an Object dictionary as an argument but received a '{scopedParams.GetType().Name}' instead");

            return pageParams ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Asserts the options.
        /// </summary>
        /// <param name="scopedParams">The scoped parameters.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static Dictionary<string, object> AssertOptions(this object scopedParams, string filterName)
        {
            var pageParams = scopedParams as Dictionary<string, object>;
            if (pageParams == null && scopedParams != null)
                throw new ArgumentException(
                    $"{filterName} only accepts an Object dictionary as an argument but received a '{scopedParams.GetType().Name}' instead");

            return pageParams ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Asserts the no circular deps.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotSupportedException">Cannot serialize type '{value.GetType().Name}' with cyclical dependencies</exception>
        public static object AssertNoCircularDeps(this object value)
        {
            if (value != null && TypeSerializer.HasCircularReferences(value))
                throw new NotSupportedException($"Cannot serialize type '{value.GetType().Name}' with cyclical dependencies");
            return value;
        }

        /// <summary>
        /// Asserts the enumerable.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static IEnumerable<object> AssertEnumerable(this object items, string filterName)
        {
            var enumObjects = items as IEnumerable<object>;
            if (enumObjects == null && items != null)
            {
                if (items is IEnumerable enumItems)
                {
                    var to = new List<object>();
                    foreach (var item in enumItems)
                    {
                        to.Add(item);
                    }
                    return to;
                }

                throw new ArgumentException(
                    $"{filterName} expects an IEnumerable but received a '{items.GetType().Name}' instead");
            }

            return enumObjects ?? TypeConstants.EmptyObjectArray;
        }

        /// <summary>
        /// Asserts the expression.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotSupportedException">'{filterName}' in '{scope.PageResult.VirtualPath}' requires a string Expression but received a '{expression?.GetType()?.Name}' instead</exception>
        public static string AssertExpression(this ScriptScopeContext scope, string filterName, object expression)
        {
            if (expression is not string literal)
                throw new NotSupportedException($"'{filterName}' in '{scope.PageResult.VirtualPath}' requires a string Expression but received a '{expression?.GetType()?.Name}' instead");
            return literal;
        }

        /// <summary>
        /// Asserts the expression.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="scopeOptions">The scope options.</param>
        /// <param name="itemBinding">The item binding.</param>
        /// <returns>JsToken.</returns>
        public static JsToken AssertExpression(this ScriptScopeContext scope, string filterName, object expression, object scopeOptions, out string itemBinding)
        {
            if (expression is JsArrowFunctionExpression arrowExpr)
            {
                itemBinding = arrowExpr.Params[0].Name;
                return arrowExpr.Body;
            }

            var literal = scope.AssertExpression(filterName, expression);
            var scopedParams = scope.GetParamsWithItemBinding(filterName, scopeOptions, out itemBinding);

            var token = literal.GetCachedJsExpression(scope);
            return token;
        }

        /// <summary>
        /// Gets the parameters with item binding.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="scopedParams">The scoped parameters.</param>
        /// <param name="itemBinding">The item binding.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public static Dictionary<string, object> GetParamsWithItemBinding(this ScriptScopeContext scope, string filterName, object scopedParams, out string itemBinding) =>
            GetParamsWithItemBinding(scope, filterName, null, scopedParams, out itemBinding);

        /// <summary>
        /// Gets the parameters with item binding.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="page">The page.</param>
        /// <param name="scopedParams">The scoped parameters.</param>
        /// <param name="itemBinding">The item binding.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public static Dictionary<string, object> GetParamsWithItemBinding(this ScriptScopeContext scope, string filterName, SharpPage page, object scopedParams, out string itemBinding)
        {
            var scopeParams = scope.GetParamsWithItemBindingOnly(filterName, page, scopedParams, out itemBinding);
            scopeParams.Each((key, val) => scope.ScopedParams[key] = val);
            return scopeParams;
        }

        /// <summary>
        /// Gets the parameters with item binding only.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="page">The page.</param>
        /// <param name="scopedParams">The scoped parameters.</param>
        /// <param name="itemBinding">The item binding.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        /// <exception cref="System.NotSupportedException">'it' option in filter '{filterName}' should contain the name to bind to but contained a '{bindingName.GetType().Name}' instead</exception>
        public static Dictionary<string, object> GetParamsWithItemBindingOnly(this ScriptScopeContext scope, string filterName, SharpPage page, object scopedParams, out string itemBinding)
        {
            var pageParams = scope.AssertOptions(filterName, scopedParams);
            itemBinding = pageParams.TryGetValue("it", out object bindingName) && bindingName is string binding
                ? binding
                : "it";

            if (bindingName != null && bindingName is not string)
                throw new NotSupportedException($"'it' option in filter '{filterName}' should contain the name to bind to but contained a '{bindingName.GetType().Name}' instead");

            // page vars take precedence
            if (page != null && page.Args.TryGetValue("it", out object pageBinding))
                itemBinding = (string)pageBinding;

            return pageParams;
        }

        /// <summary>
        /// Adds the item to scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="itemBinding">The item binding.</param>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <returns>ScriptScopeContext.</returns>
        public static ScriptScopeContext AddItemToScope(this ScriptScopeContext scope, string itemBinding, object item, int index)
        {
            scope.ScopedParams[ScriptConstants.Index] = index;
            return SetItemInScope(itemBinding, item, scope);
        }

        /// <summary>
        /// Adds the item to scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="itemBinding">The item binding.</param>
        /// <param name="item">The item.</param>
        /// <returns>ScriptScopeContext.</returns>
        public static ScriptScopeContext AddItemToScope(this ScriptScopeContext scope, string itemBinding, object item)
        {
            return SetItemInScope(itemBinding, item, scope);
        }

        /// <summary>
        /// Sets the item in scope.
        /// </summary>
        /// <param name="itemBinding">The item binding.</param>
        /// <param name="item">The item.</param>
        /// <param name="newScope">The new scope.</param>
        /// <returns>ScriptScopeContext.</returns>
        private static ScriptScopeContext SetItemInScope(string itemBinding, object item, ScriptScopeContext newScope)
        {
            newScope.ScopedParams[itemBinding] = item;

            if (item is ScopeVars explodeBindings)
            {
                foreach (var entry in explodeBindings)
                {
                    newScope.ScopedParams[entry.Key] = entry.Value;
                }
            }

            return newScope;
        }

        /// <summary>
        /// Gets the value or evaluate binding.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope">The scope.</param>
        /// <param name="valueOrBinding">The value or binding.</param>
        /// <returns>T.</returns>
        public static T GetValueOrEvaluateBinding<T>(this ScriptScopeContext scope, object valueOrBinding) =>
            (T)GetValueOrEvaluateBinding(scope, valueOrBinding, typeof(T));

        /// <summary>
        /// Gets the value or evaluate binding.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="valueOrBinding">The value or binding.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <returns>System.Object.</returns>
        public static object GetValueOrEvaluateBinding(this ScriptScopeContext scope, object valueOrBinding, Type returnType)
        {
            if (valueOrBinding is string literal)
            {
                literal.ParseJsExpression(out var token);
                var oValue = token.Evaluate(scope);
                return oValue.ConvertTo(returnType);
            }

            return valueOrBinding.ConvertTo(returnType);
        }

        /// <summary>
        /// Tries the get page.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="page">The page.</param>
        /// <param name="codePage">The code page.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryGetPage(this ScriptScopeContext scope, string virtualPath, out SharpPage page, out SharpCodePage codePage)
        {
            if (scope.PageResult.Partials.TryGetValue(virtualPath, out page))
            {
                codePage = null;
                return true;
            }

            if (!scope.Context.TryGetPage(scope.PageResult.VirtualPath, virtualPath, out page, out codePage))
                return false;

            codePage?.Init();

            if (codePage is IRequiresRequest requiresRequest)
            {
                if (scope.GetValue(ScriptConstants.Request) is IRequest request)
                    requiresRequest.Request = request;
            }

            return true;
        }

        /// <summary>
        /// Creates the new context.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>ScriptContext.</returns>
        /// <exception cref="System.NotSupportedException">Plugin '{name}' is not registered in parent context</exception>
        /// <exception cref="System.NotSupportedException">Filter '{name}' is not registered in parent context</exception>
        /// <exception cref="System.NotSupportedException">Block '{name}' is not registered in parent context</exception>
        public static ScriptContext CreateNewContext(this ScriptScopeContext scope, Dictionary<string, object> args)
        {
            if (args == null)
                return new ScriptContext().Init();

            var context = new ScriptContext();
            if (args.TryGetValue("use", out var oUse))
            {
                var use = (Dictionary<string, object>)oUse;
                if (use.TryGetValue("context", out var oContext) && oContext is bool useContext && useContext)
                {
                    return scope.Context;
                }

                // Use same ThreadSafe plugin instance to preserve configuration 
                var plugins = use.TryGetValue("plugins", out var oPlugins)
                    ? ViewUtils.ToStrings("plugins", oPlugins)
                    : null;
                if (plugins != null)
                {
                    foreach (var name in plugins)
                    {
                        var plugin = scope.Context.Plugins.FirstOrDefault(x => x.GetType().Name == name);
                        if (plugin == null)
                            throw new NotSupportedException($"Plugin '{name}' is not registered in parent context");

                        context.Plugins.Add(plugin);
                    }
                }

                // Use new filter and block instances which cannot be shared between contexts
                var methods = use.TryGetValue("methods", out var oMethods)
                    ? ViewUtils.ToStrings("methods", oMethods)
                    : use.TryGetValue("filters", out var oFilters)
                        ? ViewUtils.ToStrings("filters", oFilters)
                        : null;
                if (methods != null)
                {
                    foreach (var name in methods)
                    {
                        var filter = scope.Context.ScriptMethods.FirstOrDefault(x => x.GetType().Name == name);
                        if (filter == null)
                            throw new NotSupportedException($"Filter '{name}' is not registered in parent context");

                        context.ScriptMethods.Add(filter.GetType().CreateInstance<ScriptMethods>());
                    }
                }

                var blocks = use.TryGetValue("blocks", out var oBlocks)
                    ? ViewUtils.ToStrings("blocks", oBlocks)
                    : null;
                if (blocks != null)
                {
                    foreach (var name in blocks)
                    {
                        var useBlock = scope.Context.ScriptBlocks.FirstOrDefault(x => x.GetType().Name == name);
                        if (useBlock == null)
                            throw new NotSupportedException($"Block '{name}' is not registered in parent context");

                        context.ScriptBlocks.Add(useBlock.GetType().CreateInstance<ScriptBlock>());
                    }
                }

                args.Remove(nameof(use));
            }
            context.Init();

            return context;
        }

    }
}