// ***********************************************************************
// <copyright file="DefaultScriptsView.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections;
using System.Collections.Generic;

namespace ServiceStack.Script;

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
    /// Navs the items.
    /// </summary>
    /// <returns>List&lt;NavItem&gt;.</returns>
    public List<NavItem> navItems() => ViewUtils.NavItems;
    /// <summary>
    /// Navs the items.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>List&lt;NavItem&gt;.</returns>
    public List<NavItem> navItems(string key) => ViewUtils.GetNavItems(key);

    /// <summary>
    /// CSSs the includes.
    /// </summary>
    /// <param name="cssFiles">The CSS files.</param>
    /// <returns>IRawString.</returns>
    public IRawString cssIncludes(IEnumerable cssFiles) =>
        ViewUtils.CssIncludes(Context.VirtualFiles, ViewUtils.SplitStringList(cssFiles)).ToRawString();

    /// <summary>
    /// Jses the includes.
    /// </summary>
    /// <param name="jsFiles">The js files.</param>
    /// <returns>IRawString.</returns>
    public IRawString jsIncludes(IEnumerable jsFiles) =>
        ViewUtils.JsIncludes(Context.VirtualFiles, ViewUtils.SplitStringList(jsFiles)).ToRawString();
}