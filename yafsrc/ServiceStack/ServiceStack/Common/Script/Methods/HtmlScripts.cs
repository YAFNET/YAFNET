// ***********************************************************************
// <copyright file="HtmlScripts.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Script;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Class HtmlScripts.
    /// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
    /// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptMethods" />
    /// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
    public class HtmlScripts : ScriptMethods, IConfigureScriptContext
    {

        /// <summary>
        /// The evaluate when skipping filter execution
        /// </summary>
        public static List<string> EvaluateWhenSkippingFilterExecution = new()
        {
            nameof(htmlError),
            nameof(htmlErrorMessage),
            nameof(htmlErrorDebug),
        };

        /// <summary>
        /// Configures the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Configure(ScriptContext context)
        {
            EvaluateWhenSkippingFilterExecution.Each(name => context.OnlyEvaluateFiltersWhenSkippingPageFilterExecution.Add(name));
        }

        /// <summary>
        /// HTMLs the list.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlList(IEnumerable target) => HtmlList(target, new HtmlDumpOptions { Defaults = Context.DefaultMethods }).ToRawString();
        /// <summary>
        /// HTMLs the list.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="options">The options.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlList(IEnumerable target, Dictionary<string, object> options) =>
            HtmlList(target, HtmlDumpOptions.Parse(options, Context.DefaultMethods)).ToRawString();

        /// <summary>
        /// HTMLs the dump.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlDump(object target) => HtmlDump(target, new HtmlDumpOptions { Defaults = Context.DefaultMethods }).ToRawString();
        /// <summary>
        /// HTMLs the dump.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="options">The options.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlDump(object target, Dictionary<string, object> options) =>
            HtmlDump(target, HtmlDumpOptions.Parse(options, Context.DefaultMethods)).ToRawString();

        /// <summary>
        /// HTMLs the list.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.String.</returns>
        public static string HtmlList(IEnumerable items, HtmlDumpOptions options)
        {
            if (options == null)
                options = new HtmlDumpOptions();

            if (items is IDictionary<string, object> single)
                items = new[] { single };

            var depth = options.Depth;
            var childDepth = options.ChildDepth;
            options.Depth += 1;

            try
            {
                var parentClass = options.ClassName;
                var childClass = options.ChildClass;
                var className = (depth < childDepth ? parentClass : childClass ?? parentClass)
                                ?? options.Defaults.GetDefaultTableClassName();

                var headerStyle = options.HeaderStyle;
                var headerTag = options.HeaderTag ?? "th";

                var sbHeader = StringBuilderCache.Allocate();
                var sbRows = StringBuilderCacheAlt.Allocate();
                List<string> keys = null;

                foreach (var item in items)
                {
                    if (item is IDictionary<string, object> d)
                    {
                        if (keys == null)
                        {
                            keys = d.Keys.ToList();
                            sbHeader.Append("<tr>");
                            foreach (var key in keys)
                            {
                                sbHeader.Append('<').Append(headerTag).Append('>');
                                sbHeader.Append(ViewUtils.StyleText(key, headerStyle)?.HtmlEncode());
                                sbHeader.Append("</").Append(headerTag).Append('>');
                            }
                            sbHeader.Append("</tr>");
                        }

                        sbRows.Append("<tr>");
                        foreach (var key in keys)
                        {
                            var value = d[key];
                            if (ReferenceEquals(value, items))
                                break; // Prevent cyclical deps like 'it' binding

                            sbRows.Append("<td>");

                            if (!isComplexType(value))
                            {
                                sbRows.Append(GetScalarHtml(value, options.Defaults));
                            }
                            else
                            {
                                var htmlValue = HtmlDump(value, options);
                                sbRows.Append(htmlValue.AsString());
                            }

                            sbRows.Append("</td>");
                        }
                        sbRows.Append("</tr>");
                    }
                }

                var isEmpty = sbRows.Length == 0;
                if (isEmpty && options.CaptionIfEmpty == null)
                    return string.Empty;

                var htmlHeaders = StringBuilderCache.ReturnAndFree(sbHeader);
                var htmlRows = StringBuilderCacheAlt.ReturnAndFree(sbRows);

                var sb = StringBuilderCache.Allocate();
                sb.Append("<table");

                if (options.Id != null)
                    sb.Append(" id=\"").Append(options.Id).Append("\"");
                if (!string.IsNullOrEmpty(className))
                    sb.Append(" class=\"").Append(className).Append("\"");

                sb.Append(">");

                var caption = options.Caption;
                if (isEmpty)
                    caption = options.CaptionIfEmpty;

                if (caption != null && !options.HasCaption)
                {
                    sb.Append("<caption>").Append(caption.HtmlEncode()).Append("</caption>");
                    options.HasCaption = true;
                }

                if (htmlHeaders.Length > 0)
                    sb.Append("<thead>").Append(htmlHeaders).Append("</thead>");
                if (htmlRows.Length > 0)
                    sb.Append("<tbody>").Append(htmlRows).Append("</tbody>");

                sb.Append("</table>");

                var html = StringBuilderCache.ReturnAndFree(sb);
                return html;
            }
            finally
            {
                options.Depth = depth;
            }
        }

        /// <summary>
        /// HTMLs the dump.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.String.</returns>
        public static string HtmlDump(object target, HtmlDumpOptions options)
        {
            if (options == null)
                options = new HtmlDumpOptions();

            var depth = options.Depth;
            var childDepth = options.ChildDepth;
            options.Depth += 1;

            try
            {
                target = DefaultScripts.ConvertDumpType(target);

                if (!isComplexType(target))
                    return GetScalarHtml(target, options.Defaults);

                var parentClass = options.ClassName;
                var childClass = options.ChildClass;
                var className = (depth < childDepth ? parentClass : childClass ?? parentClass)
                                ?? options.Defaults.GetDefaultTableClassName();

                var headerStyle = options.HeaderStyle;
                var headerTag = options.HeaderTag ?? "th";

                if (target is IEnumerable e)
                {
                    var objs = e.Map(x => x);

                    var isEmpty = objs.Count == 0;
                    if (isEmpty && options.CaptionIfEmpty == null)
                        return string.Empty;

                    var first = !isEmpty ? objs[0] : null;
                    if (first is IDictionary && objs.Count > 1 && options.Display != "table")
                        return HtmlList(objs, options);

                    var sb = StringBuilderCacheAlt.Allocate();

                    sb.Append("<table");

                    if (options.Id != null)
                        sb.Append(" id=\"").Append(options.Id).Append("\"");
                    if (!string.IsNullOrEmpty(className))
                        sb.Append(" class=\"").Append(className).Append("\"");

                    sb.Append(">");

                    var caption = options.Caption;
                    if (isEmpty)
                        caption = options.CaptionIfEmpty;

                    var holdCaption = options.HasCaption;
                    if (caption != null && !options.HasCaption)
                    {
                        sb.Append("<caption>").Append(caption.HtmlEncode()).Append("</caption>");
                        options.HasCaption = true;
                    }

                    if (!isEmpty)
                    {
                        sb.Append("<tbody>");

                        if (first is KeyValuePair<string, object>)
                        {
                            foreach (var o in objs)
                            {
                                if (o is KeyValuePair<string, object> kvp)
                                {
                                    if (kvp.Value == target) break; // Prevent cyclical deps like 'it' binding

                                    sb.Append("<tr>");
                                    sb.Append('<').Append(headerTag).Append('>');
                                    sb.Append(ViewUtils.StyleText(kvp.Key, headerStyle)?.HtmlEncode());
                                    sb.Append("</").Append(headerTag).Append('>');
                                    sb.Append("<td>");
                                    if (!isComplexType(kvp.Value))
                                    {
                                        sb.Append(GetScalarHtml(kvp.Value, options.Defaults));
                                    }
                                    else
                                    {
                                        var body = HtmlDump(kvp.Value, options);
                                        sb.Append(body.AsString());
                                    }
                                    sb.Append("</td>");
                                    sb.Append("</tr>");
                                }
                            }
                        }
                        else if (!isComplexType(first))
                        {
                            foreach (var o in objs)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td>");
                                sb.Append(GetScalarHtml(o, options.Defaults));
                                sb.Append("</td>");
                                sb.Append("</tr>");
                            }
                        }
                        else
                        {
                            if (objs.Count > 1 || options.Display == "table")
                            {
                                var rows = objs.Map(x => x.ToObjectDictionary());
                                StringBuilderCache.Free(sb);
                                options.HasCaption = holdCaption;
                                return HtmlList(rows, options);
                            }
                            else
                            {
                                foreach (var o in objs)
                                {
                                    sb.Append("<tr>");

                                    if (!isComplexType(o))
                                    {
                                        sb.Append("<td>");
                                        sb.Append(GetScalarHtml(o, options.Defaults));
                                        sb.Append("</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td>");
                                        var body = HtmlDump(o, options);
                                        sb.Append(body.AsString());
                                        sb.Append("</td>");
                                    }

                                    sb.Append("</tr>");
                                }
                            }
                        }

                        sb.Append("</tbody>");
                    }

                    sb.Append("</table>");

                    var html = StringBuilderCacheAlt.ReturnAndFree(sb);
                    return html;
                }

                return HtmlDump(target.ToObjectDictionary(), options);
            }
            finally
            {
                options.Depth = depth;
            }
        }

        /// <summary>
        /// Gets the scalar HTML.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="defaults">The defaults.</param>
        /// <returns>System.String.</returns>
        private static string GetScalarHtml(object target, DefaultScripts defaults)
        {
            if (target == null || target.ToString() == string.Empty)
                return string.Empty;

            if (target is string s)
                return s.HtmlEncode();

            if (target is decimal dec)
            {
                var isMoney = dec == Math.Floor(dec * 100);
                if (isMoney)
                    return defaults?.currency(dec) ?? dec.ToString(defaults.GetDefaultCulture());
            }

            if (target.GetType().IsNumericType() || target is bool)
                return target.ToString();

            if (target is DateTime d)
                return defaults?.dateFormat(d) ?? d.ToString(defaults.GetDefaultCulture());

            if (target is TimeSpan t)
                return defaults?.timeFormat(t) ?? t.ToString();

            return (target.ToString() ?? "").HtmlEncode();
        }

        /// <summary>
        /// Determines whether [is complex type] [the specified first].
        /// </summary>
        /// <param name="first">The first.</param>
        /// <returns><c>true</c> if [is complex type] [the specified first]; otherwise, <c>false</c>.</returns>
        private static bool isComplexType(object first)
        {
            return !(first == null || first is string || first.GetType().IsValueType);
        }

        /// <summary>
        /// HTMLs the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlError(ScriptScopeContext scope) => htmlError(scope, scope.PageResult.LastFilterError);
        /// <summary>
        /// HTMLs the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ex">The ex.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlError(ScriptScopeContext scope, Exception ex) => htmlError(scope, ex, null);
        /// <summary>
        /// HTMLs the error.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="options">The options.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlError(ScriptScopeContext scope, Exception ex, object options) =>
            Context.DebugMode ? htmlErrorDebug(scope, ex, options) : htmlErrorMessage(ex, options);

        /// <summary>
        /// HTMLs the error message.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlErrorMessage(ScriptScopeContext scope) => htmlErrorMessage(scope.PageResult.LastFilterError);
        /// <summary>
        /// HTMLs the error message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlErrorMessage(Exception ex) => htmlErrorMessage(ex, null);
        /// <summary>
        /// HTMLs the error message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="options">The options.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlErrorMessage(Exception ex, object options)
        {
            if (ex == null)
                return RawString.Empty;

            var scopedParams = options as Dictionary<string, object> ?? TypeConstants.EmptyObjectDictionary;
            var className = (scopedParams.TryGetValue("className", out object oClassName) ? oClassName : null)
                            ?? Context.Args[ScriptConstants.DefaultErrorClassName];

            return $"<div class=\"{className}\">{ex.Message}</div>".ToRawString();
        }

        /// <summary>
        /// HTMLs the error debug.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlErrorDebug(ScriptScopeContext scope) => htmlErrorDebug(scope, scope.PageResult.LastFilterError);
        /// <summary>
        /// HTMLs the error debug.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ex">The ex.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlErrorDebug(ScriptScopeContext scope, object ex) =>
            htmlErrorDebug(scope, ex as Exception ?? scope.PageResult.LastFilterError, ex as Dictionary<string, object>);


        /// <summary>
        /// HTMLs the error debug.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="options">The options.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlErrorDebug(ScriptScopeContext scope, Exception ex, object options)
        {
            if (ex == null)
                return RawString.Empty;

            var scopedParams = options as Dictionary<string, object> ?? TypeConstants.EmptyObjectDictionary;
            var className = (scopedParams.TryGetValue("className", out object oClassName) ? oClassName : null)
                            ?? Context.Args[ScriptConstants.DefaultErrorClassName];

            var sb = StringBuilderCache.Allocate();
            sb.Append($"<pre class=\"{className}\">");
            sb.AppendLine($"{ex.GetType().Name}: {ex.Message}");

            var stackTrace = scope.Context.DefaultMethods.lastErrorStackTrace(scope);
            if (!string.IsNullOrEmpty(stackTrace))
            {
                sb.AppendLine();
                sb.AppendLine("StackTrace:");
                sb.AppendLine(stackTrace);
            }
            else if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                sb.AppendLine();
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);
            }

            if (ex.InnerException != null)
            {
                sb.AppendLine();
                sb.AppendLine("Inner Exceptions:");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    sb.AppendLine($"{innerEx.GetType().Name}: {innerEx.Message}");
                    if (!string.IsNullOrEmpty(innerEx.StackTrace))
                        sb.AppendLine(innerEx.StackTrace);
                    innerEx = innerEx.InnerException;
                }
            }
            sb.AppendLine("</pre>");
            var html = StringBuilderCache.ReturnAndFree(sb);
            return html.ToRawString();
        }

        /// <summary>
        /// HTMLs the attrs list.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>System.String.</returns>
        public string htmlAttrsList(Dictionary<string, object> attrs)
        {
            if (attrs == null || attrs.Count == 0)
                return string.Empty;

            var sb = StringBuilderCache.Allocate();

            var keys = attrs.Keys.OrderBy(x => x);
            foreach (var key in keys)
            {
                if (key == "text" || key == "html")
                    continue;

                var value = attrs[key];
                if (ViewUtils.IsNull(value))
                    continue;

                var useKey = key == "className"
                    ? "class"
                    : key == "htmlFor"
                        ? "for"
                        : key;

                if (value is bool boolAttr)
                {
                    if (boolAttr) // only emit attr name if value == true
                    {
                        sb.Append(' ').Append(useKey);
                    }
                }
                else
                {
                    sb.Append(' ').Append(useKey).Append('=').Append('"').Append(value?.ToString().HtmlEncode()).Append('"');
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// HTMLs the attrs.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlAttrs(object target)
        {
            var attrs = htmlAttrsList(target as Dictionary<string, object>);
            return (attrs.Length > 0 ? attrs : "").ToRawString();
        }

        /// <summary>
        /// HTMLs the class list.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public string htmlClassList(object target)
        {
            if (target == null)
                return null;

            if (target is string clsName)
                return clsName;

            var sb = StringBuilderCache.Allocate();
            if (target is Dictionary<string, object> flags)
            {
                foreach (var entry in flags)
                {
                    if (entry.Value is bool b && b)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ");
                        sb.Append(entry.Key);
                    }
                }
            }
            else if (target is List<object> list)
            {
                foreach (var item in list)
                {
                    if (item is string str && str.Length > 0)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ");
                        sb.Append(str);
                    }
                }
            }
            else if (target != null)
            {
                throw new NotSupportedException($"{nameof(htmlClass)} expects a Dictionary, List or String argument but was '{target.GetType().Name}'");
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// HTMLs the class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlClass(object target)
        {
            var cls = htmlClassList(target);
            return (cls.Length > 0 ? $" class=\"{cls}\"" : "").ToRawString();
        }

        /// <summary>
        /// HTMLs the has class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool htmlHasClass(object target, string name)
        {
            if (target == null)
                return false;

            if (target is Dictionary<string, object> flags)
                return flags.TryGetValue(name, out var oFlags) && oFlags is bool flag && flag;

            if (target is List<object> list)
            {
                foreach (var oCls in list)
                {
                    if (oCls is string cls && cls == name)
                        return true;
                }
                return false;
            }
            if (target is string className)
                return $" {className} ".IndexOf($" {name} ", StringComparison.Ordinal) >= 0;

            return false;
        }

        /// <summary>
        /// HTMLs the add class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string htmlAddClass(object target, string name)
        {
            var className = htmlClassList(target) ?? "";

            if (htmlHasClass(target, name))
                return className;

            return className + " " + name;
        }

        /// <summary>
        /// HTMLs the format.
        /// </summary>
        /// <param name="htmlWithFormat">The HTML with format.</param>
        /// <param name="arg">The argument.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlFormat(string htmlWithFormat, string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return null;

            return string.Format(htmlWithFormat, Context.DefaultMethods.htmlEncode(arg)).ToRawString();
        }

        /// <summary>
        /// HTMLs the link.
        /// </summary>
        /// <param name="href">The href.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlLink(string href) => htmlLink(href, new Dictionary<string, object> { ["text"] = href });
        /// <summary>
        /// HTMLs the link.
        /// </summary>
        /// <param name="href">The href.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlLink(string href, Dictionary<string, object> attrs)
        {
            if (string.IsNullOrEmpty(href))
                return RawString.Empty;

            return htmlA(new Dictionary<string, object>(attrs ?? TypeConstants.EmptyObjectDictionary) { ["href"] = href });
        }

        /// <summary>
        /// HTMLs the image.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlImage(string src) => htmlImage(src, null);
        /// <summary>
        /// HTMLs the image.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlImage(string src, Dictionary<string, object> attrs)
        {
            if (string.IsNullOrEmpty(src))
                return RawString.Empty;

            return htmlImg(new Dictionary<string, object>(attrs ?? TypeConstants.EmptyObjectDictionary) { ["src"] = src });
        }

        /// <summary>
        /// HTMLs the hidden inputs.
        /// </summary>
        /// <param name="inputValues">The input values.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlHiddenInputs(Dictionary<string, object> inputValues) =>
            ViewUtils.HtmlHiddenInputs(inputValues).ToRawString();

        /// <summary>
        /// HTMLs the options.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlOptions(object values) => htmlOptions(values, null);

        /// <summary>
        /// HTMLs the options.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="options">The options.</param>
        /// <returns>IRawString.</returns>
        /// <exception cref="NotSupportedException">Could not convert '{values.GetType().Name}' values into List string</exception>
        public IRawString htmlOptions(object values, object options)
        {
            if (values == null)
                return RawString.Empty;

            var opt = options.AssertOptions(nameof(htmlOptions));
            var selected = opt.TryGetValue("selected", out var oSelected) ? oSelected as string : null;
            var sb = StringBuilderCache.Allocate();

            void appendOption(StringBuilder _sb, string value, string text)
            {
                var selAttr = selected != null && value == selected ? " selected" : "";
                _sb.AppendLine($"<option value=\"{value.HtmlEncode()}\"{selAttr}>{text?.HtmlEncode()}</option>");
            }

            if (values is IEnumerable<KeyValuePair<string, object>> kvps)
            {
                foreach (var kvp in kvps) appendOption(sb, kvp.Key, kvp.Value?.ToString());
            }
            else if (values is IEnumerable<KeyValuePair<string, string>> kvpsStr)
            {
                foreach (var kvp in kvpsStr) appendOption(sb, kvp.Key, kvp.Value);
            }
            else if (values is IEnumerable<object> list)
            {
                foreach (string item in list)
                {
                    var str = item.AsString();
                    var selAttr = selected != null && str == selected ? " selected" : "";
                    sb.AppendLine($"<option{selAttr}>{str?.HtmlEncode()}</option>");
                }
            }
            else throw new NotSupportedException($"Could not convert '{values.GetType().Name}' values into List<string>");

            return StringBuilderCache.ReturnAndFree(sb).ToRawString();
        }

        /// <summary>
        /// Gets the void elements.
        /// </summary>
        /// <value>The void elements.</value>
        public static HashSet<string> VoidElements { get; } = new()
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr"
        };

        /// <summary>
        /// HTMLs the tag.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTag(Dictionary<string, object> attrs, string tag)
        {
            var scopedParams = attrs ?? TypeConstants.EmptyObjectDictionary;

            var innerHtml = scopedParams.TryGetValue("html", out object oInnerHtml)
                ? oInnerHtml.AsString()
                : null;

            if (innerHtml == null)
            {
                innerHtml = scopedParams.TryGetValue("text", out object text)
                    ? text.AsString().HtmlEncode()
                    : null;
            }

            var attrString = htmlAttrsList(attrs);
            return VoidElements.Contains(tag)
                ? $"<{tag}{attrString}>".ToRawString()
                : $"<{tag}{attrString}>{innerHtml}</{tag}>".ToRawString();
        }

        /// <summary>
        /// HTMLs the tag.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTag(string innerHtml, Dictionary<string, object> attrs, string tag)
        {
            return htmlTag(new Dictionary<string, object>(attrs ?? TypeConstants.EmptyObjectDictionary) { ["html"] = innerHtml }, tag);
        }

        /// <summary>
        /// HTMLs the div.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlDiv(Dictionary<string, object> attrs) => htmlTag(attrs, "div");
        /// <summary>
        /// HTMLs the div.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlDiv(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "div");
        /// <summary>
        /// HTMLs the span.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlSpan(Dictionary<string, object> attrs) => htmlTag(attrs, "span");
        /// <summary>
        /// HTMLs the span.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlSpan(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "span");

        /// <summary>
        /// HTMLs a.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlA(Dictionary<string, object> attrs) => htmlTag(attrs, "a");
        /// <summary>
        /// HTMLs a.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlA(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "a");
        /// <summary>
        /// HTMLs the img.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlImg(Dictionary<string, object> attrs) => htmlTag(attrs, "img");
        /// <summary>
        /// HTMLs the img.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlImg(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "img");

        /// <summary>
        /// HTMLs the h1.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH1(Dictionary<string, object> attrs) => htmlTag(attrs, "h1");
        /// <summary>
        /// HTMLs the h1.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH1(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "h1");
        /// <summary>
        /// HTMLs the h2.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH2(Dictionary<string, object> attrs) => htmlTag(attrs, "h2");
        /// <summary>
        /// HTMLs the h2.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH2(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "h2");
        /// <summary>
        /// HTMLs the h3.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH3(Dictionary<string, object> attrs) => htmlTag(attrs, "h3");
        /// <summary>
        /// HTMLs the h3.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH3(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "h3");
        /// <summary>
        /// HTMLs the h4.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH4(Dictionary<string, object> attrs) => htmlTag(attrs, "h4");
        /// <summary>
        /// HTMLs the h4.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH4(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "h4");
        /// <summary>
        /// HTMLs the h5.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH5(Dictionary<string, object> attrs) => htmlTag(attrs, "h5");
        /// <summary>
        /// HTMLs the h5.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH5(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "h5");
        /// <summary>
        /// HTMLs the h6.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH6(Dictionary<string, object> attrs) => htmlTag(attrs, "h6");
        /// <summary>
        /// HTMLs the h6.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlH6(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "h6");

        /// <summary>
        /// HTMLs the em.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlEm(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "em");
        /// <summary>
        /// HTMLs the em.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlEm(string text) => htmlTag(new Dictionary<string, object> { ["text"] = text }, "em");
        /// <summary>
        /// HTMLs the b.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlB(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "b");
        /// <summary>
        /// HTMLs the b.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlB(string text) => htmlTag(new Dictionary<string, object> { ["text"] = text }, "b");

        /// <summary>
        /// HTMLs the ul.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlUl(Dictionary<string, object> attrs) => htmlTag(attrs, "ul");
        /// <summary>
        /// HTMLs the ul.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlUl(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "ul");
        /// <summary>
        /// HTMLs the ol.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlOl(Dictionary<string, object> attrs) => htmlTag(attrs, "ol");
        /// <summary>
        /// HTMLs the ol.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlOl(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "ol");
        /// <summary>
        /// HTMLs the li.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlLi(Dictionary<string, object> attrs) => htmlTag(attrs, "li");
        /// <summary>
        /// HTMLs the li.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlLi(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "li");

        /// <summary>
        /// HTMLs the table.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTable(Dictionary<string, object> attrs) => htmlTag(attrs, "table");
        /// <summary>
        /// HTMLs the table.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTable(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "table");
        /// <summary>
        /// HTMLs the tr.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTr(Dictionary<string, object> attrs) => htmlTag(attrs, "tr");
        /// <summary>
        /// HTMLs the tr.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTr(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "tr");
        /// <summary>
        /// HTMLs the th.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTh(Dictionary<string, object> attrs) => htmlTag(attrs, "th");
        /// <summary>
        /// HTMLs the th.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTh(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "th");
        /// <summary>
        /// HTMLs the td.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTd(Dictionary<string, object> attrs) => htmlTag(attrs, "td");
        /// <summary>
        /// HTMLs the td.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTd(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "td");

        /// <summary>
        /// HTMLs the form.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlForm(Dictionary<string, object> attrs) => htmlTag(attrs, "form");
        /// <summary>
        /// HTMLs the form.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlForm(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "form");
        /// <summary>
        /// HTMLs the label.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlLabel(Dictionary<string, object> attrs) => htmlTag(attrs, "label");
        /// <summary>
        /// HTMLs the label.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlLabel(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "label");
        /// <summary>
        /// HTMLs the input.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlInput(Dictionary<string, object> attrs) => htmlTag(attrs, "input");
        /// <summary>
        /// HTMLs the input.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlInput(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "input");
        /// <summary>
        /// HTMLs the text area.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTextArea(Dictionary<string, object> attrs) => htmlTag(attrs, "textarea");
        /// <summary>
        /// HTMLs the text area.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlTextArea(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "textarea");
        /// <summary>
        /// HTMLs the button.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlButton(Dictionary<string, object> attrs) => htmlTag(attrs, "button");
        /// <summary>
        /// HTMLs the button.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlButton(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "button");
        /// <summary>
        /// HTMLs the select.
        /// </summary>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlSelect(Dictionary<string, object> attrs) => htmlTag(attrs, "select");
        /// <summary>
        /// HTMLs the select.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlSelect(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "select");
        /// <summary>
        /// HTMLs the option.
        /// </summary>
        /// <param name="innerHtml">The inner HTML.</param>
        /// <param name="attrs">The attrs.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlOption(string innerHtml, Dictionary<string, object> attrs) => htmlTag(innerHtml, attrs, "option");
        /// <summary>
        /// HTMLs the option.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>IRawString.</returns>
        public IRawString htmlOption(string text) => htmlTag(new Dictionary<string, object> { ["text"] = text }, "option");
    }
}