// ***********************************************************************
// <copyright file="ViewUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using ServiceStack.Configuration;
using ServiceStack.IO;
using ServiceStack.Script;
using ServiceStack.Text;
using ServiceStack.Web;

namespace ServiceStack;

/// <summary>
/// High-level Input options for rendering HTML Input controls
/// </summary>
public class InputOptions
{
    /// <summary>
    /// Display the Control inline
    /// </summary>
    /// <value><c>true</c> if inline; otherwise, <c>false</c>.</value>
    public bool Inline { get; set; }

    /// <summary>
    /// Label for the control
    /// </summary>
    /// <value>The label.</value>
    public string Label { get; set; }

    /// <summary>
    /// Class for Label
    /// </summary>
    /// <value>The label class.</value>
    public string LabelClass { get; set; }

    /// <summary>
    /// Override the class on the error message (default: invalid-feedback)
    /// </summary>
    /// <value>The error class.</value>
    public string ErrorClass { get; set; }

    /// <summary>
    /// Small Help Text displayed with the control
    /// </summary>
    /// <value>The help.</value>
    public string Help { get; set; }

    /// <summary>
    /// Bootstrap Size of the Control: sm, lg
    /// </summary>
    /// <value>The size.</value>
    public string Size { get; set; }

    /// <summary>
    /// Multiple Value Data Source for Checkboxes, Radio boxes and Select Controls
    /// </summary>
    /// <value>The values.</value>
    public object Values { get; set; }

    /// <summary>
    /// Typed setter of Multi Input Values
    /// </summary>
    /// <value>The input values.</value>
    public IEnumerable<KeyValuePair<string, string>> InputValues
    {
        set => Values = value;
    }

    /// <summary>
    /// Whether to preserve value state after post back
    /// </summary>
    /// <value><c>true</c> if [preserve value]; otherwise, <c>false</c>.</value>
    public bool PreserveValue { get; set; } = true;

    /// <summary>
    /// Whether to show Error Message associated with this control
    /// </summary>
    /// <value><c>true</c> if [show errors]; otherwise, <c>false</c>.</value>
    public bool ShowErrors { get; set; } = true;
}

/// <summary>
/// Customize JS/CSS/HTML bundles
/// </summary>
public class BundleOptions
{
    /// <summary>
    /// List of file and directory sources to include in this bundle, directory sources must end in `/`.
    /// Sources can include prefixes to specify which Virtual File System Source to use, options:
    /// 'content:' (ContentRoot HostContext.VirtualFiles), 'filesystem:' (WebRoot FileSystem), 'memory:' (WebRoot Memory)
    /// </summary>
    /// <value>The sources.</value>
    public List<string> Sources { get; set; } = new();

    /// <summary>
    /// Write bundled file to this Virtual Path
    /// </summary>
    /// <value>The output to.</value>
    public string OutputTo { get; set; }
    /// <summary>
    /// If needed, use alternative OutputTo Virtual Path in html tag
    /// </summary>
    /// <value>The output web path.</value>
    public string OutputWebPath { get; set; }
    /// <summary>
    /// If needed, include PathBase prefix in output tag
    /// </summary>
    /// <value>The path base.</value>
    public string PathBase { get; set; }
    /// <summary>
    /// Whether to minify sources in bundle (default true)
    /// </summary>
    /// <value><c>true</c> if minify; otherwise, <c>false</c>.</value>
    public bool Minify { get; set; } = true;
    /// <summary>
    /// Whether to save to disk or Memory File System (default Memory)
    /// </summary>
    /// <value><c>true</c> if [save to disk]; otherwise, <c>false</c>.</value>
    public bool SaveToDisk { get; set; }
    /// <summary>
    /// Whether to return cached bundle if exists (default true)
    /// </summary>
    /// <value><c>true</c> if cache; otherwise, <c>false</c>.</value>
    public bool Cache { get; set; } = true;
    /// <summary>
    /// Whether to bundle and emit single or not bundle and emit multiple html tags
    /// </summary>
    /// <value><c>true</c> if bundle; otherwise, <c>false</c>.</value>
    public bool Bundle { get; set; } = true;
    /// <summary>
    /// Whether to call AMD define for CommonJS modules
    /// </summary>
    /// <value><c>true</c> if [register module in amd]; otherwise, <c>false</c>.</value>
    public bool RegisterModuleInAmd { get; set; }
    /// <summary>
    /// Whether to wrap JS scripts in an Immediately-Invoked Function Expression
    /// </summary>
    /// <value><c>true</c> if iife; otherwise, <c>false</c>.</value>
    public bool IIFE { get; set; }
}

/// <summary>
/// Class TextDumpOptions.
/// </summary>
public class TextDumpOptions
{
    /// <summary>
    /// Gets or sets the header style.
    /// </summary>
    /// <value>The header style.</value>
    public TextStyle HeaderStyle { get; set; }
    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    /// <value>The caption.</value>
    public string Caption { get; set; }
    /// <summary>
    /// Gets or sets the caption if empty.
    /// </summary>
    /// <value>The caption if empty.</value>
    public string CaptionIfEmpty { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [include row numbers].
    /// </summary>
    /// <value><c>true</c> if [include row numbers]; otherwise, <c>false</c>.</value>
    public bool IncludeRowNumbers { get; set; } = true;

    /// <summary>
    /// Gets or sets the defaults.
    /// </summary>
    /// <value>The defaults.</value>
    public DefaultScripts Defaults { get; set; } = ViewUtils.DefaultScripts;

    /// <summary>
    /// Gets or sets the depth.
    /// </summary>
    /// <value>The depth.</value>
    internal int Depth { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance has caption.
    /// </summary>
    /// <value><c>true</c> if this instance has caption; otherwise, <c>false</c>.</value>
    internal bool HasCaption { get; set; }

    /// <summary>
    /// Parses the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="defaults">The defaults.</param>
    /// <returns>TextDumpOptions.</returns>
    public static TextDumpOptions Parse(Dictionary<string, object> options, DefaultScripts defaults = null)
    {
        return new TextDumpOptions
                   {
                       HeaderStyle = options.TryGetValue("headerStyle", out var oHeaderStyle)
                                         ? oHeaderStyle.ConvertTo<TextStyle>()
                                         : TextStyle.SplitCase,
                       Caption = options.TryGetValue("caption", out var caption)
                                     ? caption?.ToString()
                                     : null,
                       CaptionIfEmpty = options.TryGetValue("captionIfEmpty", out var captionIfEmpty)
                                            ? captionIfEmpty?.ToString()
                                            : null,
                       IncludeRowNumbers = !options.TryGetValue("rowNumbers", out var rowNumbers) || rowNumbers is not bool b || b,
                       Defaults = defaults ?? ViewUtils.DefaultScripts,
                   };
    }
}

/// <summary>
/// Class HtmlDumpOptions.
/// </summary>
public class HtmlDumpOptions
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public string Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the class.
    /// </summary>
    /// <value>The name of the class.</value>
    public string ClassName { get; set; }
    /// <summary>
    /// Gets or sets the child class.
    /// </summary>
    /// <value>The child class.</value>
    public string ChildClass { get; set; }

    /// <summary>
    /// Gets or sets the header style.
    /// </summary>
    /// <value>The header style.</value>
    public TextStyle HeaderStyle { get; set; }
    /// <summary>
    /// Gets or sets the header tag.
    /// </summary>
    /// <value>The header tag.</value>
    public string HeaderTag { get; set; }

    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    /// <value>The caption.</value>
    public string Caption { get; set; }
    /// <summary>
    /// Gets or sets the caption if empty.
    /// </summary>
    /// <value>The caption if empty.</value>
    public string CaptionIfEmpty { get; set; }

    /// <summary>
    /// Gets or sets the defaults.
    /// </summary>
    /// <value>The defaults.</value>
    public DefaultScripts Defaults { get; set; } = ViewUtils.DefaultScripts;

    /// <summary>
    /// Gets or sets the display.
    /// </summary>
    /// <value>The display.</value>
    public string Display { get; set; }
    /// <summary>
    /// Gets or sets the depth.
    /// </summary>
    /// <value>The depth.</value>
    internal int Depth { get; set; }
    /// <summary>
    /// Gets or sets the child depth.
    /// </summary>
    /// <value>The child depth.</value>
    internal int ChildDepth { get; set; } = 1;
    /// <summary>
    /// Gets or sets a value indicating whether this instance has caption.
    /// </summary>
    /// <value><c>true</c> if this instance has caption; otherwise, <c>false</c>.</value>
    internal bool HasCaption { get; set; }

    /// <summary>
    /// Parses the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="defaults">The defaults.</param>
    /// <returns>HtmlDumpOptions.</returns>
    public static HtmlDumpOptions Parse(Dictionary<string, object> options, DefaultScripts defaults = null)
    {
        return new HtmlDumpOptions
                   {
                       Id = options.TryGetValue("id", out var oId)
                                ? (string)oId
                                : null,
                       ClassName = options.TryGetValue("className", out var oClassName)
                                       ? (string)oClassName
                                       : null,
                       ChildClass = options.TryGetValue("childClass", out var oChildClass)
                                        ? (string)oChildClass
                                        : null,

                       HeaderStyle = options.TryGetValue("headerStyle", out var oHeaderStyle)
                                         ? oHeaderStyle.ConvertTo<TextStyle>()
                                         : TextStyle.SplitCase,
                       HeaderTag = options.TryGetValue("headerTag", out var oHeaderTag)
                                       ? (string)oHeaderTag
                                       : null,

                       Caption = options.TryGetValue("caption", out var caption)
                                     ? caption?.ToString()
                                     : null,
                       CaptionIfEmpty = options.TryGetValue("captionIfEmpty", out var captionIfEmpty)
                                            ? captionIfEmpty?.ToString()
                                            : null,
                       Display = options.TryGetValue("display", out var display)
                                     ? display?.ToString()
                                     : null,
                       Defaults = defaults ?? ViewUtils.DefaultScripts,
                   };
    }
}

/// <summary>
/// Enum TextStyle
/// </summary>
public enum TextStyle
{
    /// <summary>
    /// The none
    /// </summary>
    None,
    /// <summary>
    /// The split case
    /// </summary>
    SplitCase,
    /// <summary>
    /// The humanize
    /// </summary>
    Humanize,
    /// <summary>
    /// The title case
    /// </summary>
    TitleCase,
    /// <summary>
    /// The pascal case
    /// </summary>
    PascalCase,
    /// <summary>
    /// The camel case
    /// </summary>
    CamelCase,
}

/// <summary>
/// Generic collection of Nav Links
/// </summary>
public static class NavDefaults
{
    /// <summary>
    /// Gets or sets the nav class.
    /// </summary>
    /// <value>The nav class.</value>
    public static string NavClass { get; set; } = "nav";
    /// <summary>
    /// Gets or sets the nav item class.
    /// </summary>
    /// <value>The nav item class.</value>
    public static string NavItemClass { get; set; } = "nav-item";
    /// <summary>
    /// Gets or sets the nav link class.
    /// </summary>
    /// <value>The nav link class.</value>
    public static string NavLinkClass { get; set; } = "nav-link";

    /// <summary>
    /// Gets or sets the child nav item class.
    /// </summary>
    /// <value>The child nav item class.</value>
    public static string ChildNavItemClass { get; set; } = "nav-item dropdown";
    /// <summary>
    /// Gets or sets the child nav link class.
    /// </summary>
    /// <value>The child nav link class.</value>
    public static string ChildNavLinkClass { get; set; } = "nav-link dropdown-toggle";
    /// <summary>
    /// Gets or sets the child nav menu class.
    /// </summary>
    /// <value>The child nav menu class.</value>
    public static string ChildNavMenuClass { get; set; } = "dropdown-menu";
    /// <summary>
    /// Gets or sets the child nav menu item class.
    /// </summary>
    /// <value>The child nav menu item class.</value>
    public static string ChildNavMenuItemClass { get; set; } = "dropdown-item";

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>NavOptions.</returns>
    public static NavOptions Create() => new()
                                             {
                                                 NavClass = NavClass,
                                                 NavItemClass = NavItemClass,
                                                 NavLinkClass = NavLinkClass,
                                                 ChildNavItemClass = ChildNavItemClass,
                                                 ChildNavLinkClass = ChildNavLinkClass,
                                                 ChildNavMenuClass = ChildNavMenuClass,
                                                 ChildNavMenuItemClass = ChildNavMenuItemClass,
                                             };
    /// <summary>
    /// Fors the nav.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>NavOptions.</returns>
    public static NavOptions ForNav(this NavOptions options) => options; //Already uses NavDefaults
    /// <summary>
    /// Overrides the defaults.
    /// </summary>
    /// <param name="targets">The targets.</param>
    /// <param name="source">The source.</param>
    /// <returns>NavOptions.</returns>
    public static NavOptions OverrideDefaults(NavOptions targets, NavOptions source)
    {
        if (targets == null)
            return source;
        if (targets.NavClass == NavClass && source.NavClass != null)
            targets.NavClass = source.NavClass;
        if (targets.NavItemClass == NavItemClass && source.NavItemClass != null)
            targets.NavItemClass = source.NavItemClass;
        if (targets.NavLinkClass == NavLinkClass && source.NavLinkClass != null)
            targets.NavLinkClass = source.NavLinkClass;
        if (targets.ChildNavItemClass == ChildNavItemClass && source.ChildNavItemClass != null)
            targets.ChildNavItemClass = source.ChildNavItemClass;
        if (targets.ChildNavLinkClass == ChildNavLinkClass && source.ChildNavLinkClass != null)
            targets.ChildNavLinkClass = source.ChildNavLinkClass;
        if (targets.ChildNavMenuClass == ChildNavMenuClass && source.ChildNavMenuClass != null)
            targets.ChildNavMenuClass = source.ChildNavMenuClass;
        if (targets.ChildNavMenuItemClass == ChildNavMenuItemClass && source.ChildNavMenuItemClass != null)
            targets.ChildNavMenuItemClass = source.ChildNavMenuItemClass;
        return targets;
    }
}
/// <summary>
/// Single NavLink List Item
/// </summary>
public static class NavLinkDefaults
{
    /// <summary>
    /// Fors the nav link.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>NavOptions.</returns>
    public static NavOptions ForNavLink(this NavOptions options) => options; //Already uses NavDefaults
}
/// <summary>
/// Navigation Bar Menu Items
/// </summary>
public static class NavbarDefaults
{
    /// <summary>
    /// Gets or sets the nav class.
    /// </summary>
    /// <value>The nav class.</value>
    public static string NavClass { get; set; } = "navbar-nav";
    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>NavOptions.</returns>
    public static NavOptions Create() => new() { NavClass = NavClass };
    /// <summary>
    /// Fors the navbar.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>NavOptions.</returns>
    public static NavOptions ForNavbar(this NavOptions options) => NavDefaults.OverrideDefaults(options, Create());
}
/// <summary>
/// Collection of Link Buttons (e.g. used to render /auth buttons)
/// </summary>
public static class NavButtonGroupDefaults
{
    /// <summary>
    /// Gets or sets the nav class.
    /// </summary>
    /// <value>The nav class.</value>
    public static string NavClass { get; set; } = "btn-group";
    /// <summary>
    /// Gets or sets the nav item class.
    /// </summary>
    /// <value>The nav item class.</value>
    public static string NavItemClass { get; set; } = "btn btn-primary";
    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>NavOptions.</returns>
    public static NavOptions Create() => new() { NavClass = NavClass, NavItemClass = NavItemClass };
    /// <summary>
    /// Fors the nav button group.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>NavOptions.</returns>
    public static NavOptions ForNavButtonGroup(this NavOptions options) => NavDefaults.OverrideDefaults(options, Create());
}

/// <summary>
/// Class NavOptions.
/// </summary>
public class NavOptions
{
    /// <summary>
    /// User Attributes for conditional rendering, e.g:
    /// - auth - User is Authenticated
    /// - role:name - User Role
    /// - perm:name - User Permission
    /// </summary>
    /// <value>The attributes.</value>
    public HashSet<string> Attributes { get; set; }

    /// <summary>
    /// Path Info that should set as active
    /// </summary>
    /// <value>The active path.</value>
    public string ActivePath { get; set; }


    /// <summary>
    /// Prefix to include before NavItem.Path (if any)
    /// </summary>
    /// <value>The base href.</value>
    public string BaseHref { get; set; }

    /// <summary>
    /// Custom classes applied to different navigation elements (defaults to Bootstrap classes)
    /// </summary>
    /// <value>The nav class.</value>
    public string NavClass { get; set; } = NavDefaults.NavClass;
    /// <summary>
    /// Gets or sets the nav item class.
    /// </summary>
    /// <value>The nav item class.</value>
    public string NavItemClass { get; set; } = NavDefaults.NavItemClass;
    /// <summary>
    /// Gets or sets the nav link class.
    /// </summary>
    /// <value>The nav link class.</value>
    public string NavLinkClass { get; set; } = NavDefaults.NavLinkClass;

    /// <summary>
    /// Gets or sets the child nav item class.
    /// </summary>
    /// <value>The child nav item class.</value>
    public string ChildNavItemClass { get; set; } = NavDefaults.ChildNavItemClass;
    /// <summary>
    /// Gets or sets the child nav link class.
    /// </summary>
    /// <value>The child nav link class.</value>
    public string ChildNavLinkClass { get; set; } = NavDefaults.ChildNavLinkClass;
    /// <summary>
    /// Gets or sets the child nav menu class.
    /// </summary>
    /// <value>The child nav menu class.</value>
    public string ChildNavMenuClass { get; set; } = NavDefaults.ChildNavMenuClass;
    /// <summary>
    /// Gets or sets the child nav menu item class.
    /// </summary>
    /// <value>The child nav menu item class.</value>
    public string ChildNavMenuItemClass { get; set; } = NavDefaults.ChildNavMenuItemClass;
}

/// <summary>
/// Public API for ViewUtils
/// </summary>
public static class View
{
    /// <summary>
    /// Gets the nav items.
    /// </summary>
    /// <value>The nav items.</value>
    public static List<NavItem> NavItems => ViewUtils.NavItems;
    /// <summary>
    /// Gets the nav items map.
    /// </summary>
    /// <value>The nav items map.</value>
    public static Dictionary<string, List<NavItem>> NavItemsMap => ViewUtils.NavItemsMap;
    /// <summary>
    /// Loads the specified settings.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public static void Load(IAppSettings settings) => ViewUtils.Load(settings);

    /// <summary>
    /// Gets the nav items.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>List&lt;NavItem&gt;.</returns>
    public static List<NavItem> GetNavItems(string key) => ViewUtils.GetNavItems(key);
}

/// <summary>
/// Shared Utils shared between different Template Filters and Razor Views/Helpers
/// </summary>
public static class ViewUtils
{
    /// <summary>
    /// The default scripts
    /// </summary>
    internal static readonly DefaultScripts DefaultScripts = new();
    /// <summary>
    /// The HTML scripts
    /// </summary>
    private static readonly HtmlScripts HtmlScripts = new();

    /// <summary>
    /// Gets or sets the nav items key.
    /// </summary>
    /// <value>The nav items key.</value>
    public static string NavItemsKey { get; set; } = "NavItems";
    /// <summary>
    /// Gets or sets the nav items map key.
    /// </summary>
    /// <value>The nav items map key.</value>
    public static string NavItemsMapKey { get; set; } = "NavItemsMap";

    /// <summary>
    /// Loads the specified settings.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public static void Load(IAppSettings settings)
    {
        var navItems = settings?.Get<List<NavItem>>(NavItemsKey);
        if (navItems != null)
        {
            NavItems.AddRange(navItems);
        }

        var navItemsMap = settings?.Get<Dictionary<string, List<NavItem>>>(NavItemsMapKey);
        if (navItemsMap != null)
        {
            foreach (var entry in navItemsMap)
            {
                NavItemsMap[entry.Key] = entry.Value;
            }
        }
    }

    /// <summary>
    /// Shows the nav.
    /// </summary>
    /// <param name="navItem">The nav item.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool ShowNav(this NavItem navItem, HashSet<string> attributes)
    {
        if (attributes.IsEmpty())
            return navItem.Show == null;
        if (navItem.Show != null && !attributes.Contains(navItem.Show))
            return false;
        if (navItem.Hide != null && attributes.Contains(navItem.Hide))
            return false;
        return true;
    }

    /// <summary>
    /// Gets the nav items.
    /// </summary>
    /// <value>The nav items.</value>
    public static List<NavItem> NavItems { get; } = new();
    /// <summary>
    /// Gets the nav items map.
    /// </summary>
    /// <value>The nav items map.</value>
    public static Dictionary<string, List<NavItem>> NavItemsMap { get; } = new();

    /// <summary>
    /// Gets the nav items.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>List&lt;NavItem&gt;.</returns>
    public static List<NavItem> GetNavItems(string key) => NavItemsMap.TryGetValue(key, out var navItems)
                                                               ? navItems
                                                               : TypeConstants<NavItem>.EmptyList;

    /// <summary>
    /// CSSs the includes.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="cssFiles">The CSS files.</param>
    /// <returns>System.String.</returns>
    public static string CssIncludes(IVirtualPathProvider vfs, List<string> cssFiles)
    {
        if (vfs == null || cssFiles == null || cssFiles.Count == 0)
            return null;


        var sb = StringBuilderCache.Allocate();
        sb.AppendLine("<style>");

        foreach (var file in cssFiles.Select(cssFile => !cssFile.StartsWith("/")
                                                            ? "/css/" + cssFile + ".css"
                                                            : cssFile).Select(virtualPath => vfs.GetFile(virtualPath.TrimStart('/'))).Where(file => file != null))
        {
            using var reader = file.OpenText();
            while (reader.ReadLine() is { } line)
            {
                sb.AppendLine(line);
            }
        }

        sb.AppendLine("</style>");
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Jses the includes.
    /// </summary>
    /// <param name="vfs">The VFS.</param>
    /// <param name="jsFiles">The js files.</param>
    /// <returns>System.String.</returns>
    public static string JsIncludes(IVirtualPathProvider vfs, List<string> jsFiles)
    {
        if (vfs == null || jsFiles == null || jsFiles.Count == 0)
            return null;


        var sb = StringBuilderCache.Allocate();
        sb.AppendLine("<script>");

        foreach (var file in jsFiles.Select(jsFile => !jsFile.StartsWith("/")
                                                          ? "/js/" + jsFile + ".js"
                                                          : jsFile).Select(virtualPath => vfs.GetFile(virtualPath.TrimStart('/'))).Where(file => file != null))
        {
            using var reader = file.OpenText();
            while (reader.ReadLine() is { } line)
            {
                sb.AppendLine(line);
            }
        }

        sb.AppendLine("</script>");
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Display a list of NavItem's
    /// </summary>
    /// <param name="navItems">The nav items.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string Nav(List<NavItem> navItems, NavOptions options)
    {
        if (navItems.IsEmpty())
            return string.Empty;

        var sb = StringBuilderCache.Allocate();
        sb.Append("<div class=\"")
            .Append(options.NavClass)
            .AppendLine("\">");

        foreach (var navItem in navItems)
        {
            NavLink(sb, navItem, options);
        }

        sb.AppendLine("</div>");
        return sb.ToString();
    }

    /// <summary>
    /// Display a `nav-link` nav-item
    /// </summary>
    /// <param name="navItem">The nav item.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string NavLink(NavItem navItem, NavOptions options)
    {
        var sb = StringBuilderCache.Allocate();
        NavLink(sb, navItem, options);
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Actives the class.
    /// </summary>
    /// <param name="navItem">The nav item.</param>
    /// <param name="activePath">The active path.</param>
    /// <returns>System.String.</returns>
    static string ActiveClass(NavItem navItem, string activePath) =>
        navItem.Href != null && (navItem.Exact == true || activePath.Length <= 1
                                     ? activePath?.TrimEnd('/').EqualsIgnoreCase(navItem.Href?.TrimEnd('/')) == true
                                     : activePath.TrimEnd('/').StartsWithIgnoreCase(navItem.Href?.TrimEnd('/')))
            ? " active"
            : "";

    /// <summary>
    /// Display a `nav-link` nav-item
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="navItem">The nav item.</param>
    /// <param name="options">The options.</param>
    public static void NavLink(StringBuilder sb, NavItem navItem, NavOptions options)
    {
        if (!navItem.ShowNav(options.Attributes))
            return;

        var hasChildren = navItem.Children?.Count > 0;
        var navItemCls = hasChildren
                             ? options.ChildNavItemClass
                             : options.NavItemClass;
        var navLinkCls = hasChildren
                             ? options.ChildNavLinkClass
                             : options.NavLinkClass;
        var id = navItem.Id;
        if (hasChildren && id == null)
            id = navItem.Label.SafeVarName() + "MenuLink";

        sb.Append("<li class=\"")
            .Append(navItem.ClassName).Append(navItem.ClassName != null ? " " : "")
            .Append(navItemCls)
            .AppendLine("\">");

        sb.Append("  <a href=\"")
            .Append(options.BaseHref?.TrimEnd('/'))
            .Append(navItem.Href)
            .Append("\"");

        sb.Append(" class=\"")
            .Append(navLinkCls).Append(ActiveClass(navItem, options.ActivePath))
            .Append("\"");

        if (id != null)
            sb.Append(" id=\"").Append(id).Append("\"");

        if (hasChildren)
        {
            sb.Append(" role=\"button\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"");
        }

        sb.Append(">")
            .Append(navItem.Label)
            .AppendLine("</a>");

        if (hasChildren)
        {
            sb.Append("  <div class=\"")
                .Append(options.ChildNavMenuClass)
                .Append("\" aria-labelledby=\"").Append(id).AppendLine("\">");

            foreach (var childNav in navItem.Children)
            {
                if (childNav.Label == "-")
                {
                    sb.AppendLine("    <div class=\"dropdown-divider\"></div>");
                }
                else
                {
                    sb.Append("    <a class=\"")
                        .Append(options.ChildNavMenuItemClass)
                        .Append(ActiveClass(childNav, options.ActivePath))
                        .Append("\"")
                        .Append(" href=\"")
                        .Append(options.BaseHref?.TrimEnd('/'))
                        .Append(childNav.Href)
                        .Append("\">")
                        .Append(childNav.Label)
                        .AppendLine("</a>");
                }
            }
            sb.AppendLine("</div");
        }

        sb.Append("</lI>");
    }

    /// <summary>
    /// Navs the button group.
    /// </summary>
    /// <param name="navItems">The nav items.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string NavButtonGroup(List<NavItem> navItems, NavOptions options)
    {
        if (navItems.IsEmpty())
            return string.Empty;

        var sb = StringBuilderCache.Allocate();
        sb.Append("<div class=\"")
            .Append(options.NavClass)
            .AppendLine("\">");

        foreach (var navItem in navItems)
        {
            NavLinkButton(sb, navItem, options);
        }

        sb.AppendLine("</div>");
        return sb.ToString();
    }

    /// <summary>
    /// Navs the button group.
    /// </summary>
    /// <param name="navItem">The nav item.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string NavButtonGroup(NavItem navItem, NavOptions options)
    {
        var sb = StringBuilderCache.Allocate();
        NavLinkButton(sb, navItem, options);
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Navs the link button.
    /// </summary>
    /// <param name="sb">The sb.</param>
    /// <param name="navItem">The nav item.</param>
    /// <param name="options">The options.</param>
    public static void NavLinkButton(StringBuilder sb, NavItem navItem, NavOptions options)
    {
        if (!navItem.ShowNav(options.Attributes))
            return;

        sb.Append("<a href=\"")
            .Append(options.BaseHref?.TrimEnd('/'))
            .Append(navItem.Href)
            .Append("\"");

        sb.Append(" class=\"")
            .Append(navItem.ClassName).Append(navItem.ClassName != null ? " " : "")
            .Append(options.NavItemClass).Append(ActiveClass(navItem, options.ActivePath))
            .Append("\"");

        if (navItem.Id != null)
            sb.Append(" id=\"").Append(navItem.Id).Append("\"");

        sb.Append(">")
            .Append(!string.IsNullOrEmpty(navItem.IconClass)
                        ? $"<i class=\"{navItem.IconClass}\"></i>" : "")
            .Append(navItem.Label)
            .AppendLine("</a>");
    }


    /// <summary>
    /// Determines whether the specified test is null.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <returns><c>true</c> if the specified test is null; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull(object test) => test == null || test == JsNull.Value;

    /// <summary>
    /// Gets the default culture.
    /// </summary>
    /// <param name="defaultScripts">The default scripts.</param>
    /// <returns>CultureInfo.</returns>
    public static CultureInfo GetDefaultCulture(this DefaultScripts defaultScripts) =>
        defaultScripts?.Context?.Args[ScriptConstants.DefaultCulture] as CultureInfo ?? ScriptConfig.DefaultCulture;

    /// <summary>
    /// Gets the default name of the table class.
    /// </summary>
    /// <param name="defaultScripts">The default scripts.</param>
    /// <returns>System.String.</returns>
    public static string GetDefaultTableClassName(this DefaultScripts defaultScripts) =>
        defaultScripts?.Context?.Args[ScriptConstants.DefaultTableClassName] as string;

    /// <summary>
    /// Texts the dump.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public static string TextDump(this object target) => DefaultScripts.TextDump(target, null);
    /// <summary>
    /// Texts the dump.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string TextDump(this object target, TextDumpOptions options) => DefaultScripts.TextDump(target, options);
    /// <summary>
    /// Dumps the table.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public static string DumpTable(this object target) => DefaultScripts.TextDump(target, null);
    /// <summary>
    /// Prints the dump table.
    /// </summary>
    /// <param name="target">The target.</param>
    public static void PrintDumpTable(this object target) => DumpTable(target).Print();
    /// <summary>
    /// Dumps the table.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string DumpTable(this object target, TextDumpOptions options) => DefaultScripts.TextDump(target, options);
    /// <summary>
    /// Prints the dump table.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="options">The options.</param>
    public static void PrintDumpTable(this object target, TextDumpOptions options) => DumpTable(target, options).Print();

    /// <summary>
    /// HTMLs the dump.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public static string HtmlDump(object target) => HtmlScripts.HtmlDump(target, null);
    /// <summary>
    /// HTMLs the dump.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string HtmlDump(object target, HtmlDumpOptions options) => HtmlScripts.HtmlDump(target, options);

    /// <summary>
    /// Styles the text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="textStyle">The text style.</param>
    /// <returns>System.String.</returns>
    public static string StyleText(string text, TextStyle textStyle)
    {
        if (text == null) return null;
        switch (textStyle)
        {
            case TextStyle.SplitCase:
                return DefaultScripts.splitCase(text);
            case TextStyle.Humanize:
                return DefaultScripts.humanize(text);
            case TextStyle.TitleCase:
                return DefaultScripts.titleCase(text);
            case TextStyle.PascalCase:
                return DefaultScripts.pascalCase(text);
            case TextStyle.CamelCase:
                return DefaultScripts.camelCase(text);
        }
        return text;
    }

    /// <summary>
    /// Emit HTML hidden input field for each specified Key/Value pair entry
    /// </summary>
    /// <param name="inputValues">The input values.</param>
    /// <returns>System.String.</returns>
    public static string HtmlHiddenInputs(IEnumerable<KeyValuePair<string, object>> inputValues)
    {
        if (inputValues != null)
        {
            var sb = StringBuilderCache.Allocate();
            foreach (var entry in inputValues)
            {
                sb.AppendLine($"<input type=\"hidden\" name=\"{entry.Key.HtmlEncode()}\" value=\"{entry.Value?.ToString().HtmlEncode()}\">");
            }
            return StringBuilderCache.ReturnAndFree(sb);
        }
        return null;
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <param name="httpReq">The HTTP req.</param>
    /// <param name="key">The key.</param>
    /// <returns>System.Object.</returns>
    internal static object GetItem(this IRequest httpReq, string key)
    {
        if (httpReq == null) return null;

        httpReq.Items.TryGetValue(key, out var value);
        return value;
    }

    /// <summary>
    /// Gets the error status.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <returns>ResponseStatus.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ResponseStatus GetErrorStatus(IRequest req) =>
        req.GetItem("__errorStatus") as ResponseStatus; // Keywords.ErrorStatus

    /// <summary>
    /// Determines whether [has error status] [the specified req].
    /// </summary>
    /// <param name="req">The req.</param>
    /// <returns><c>true</c> if [has error status] [the specified req]; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasErrorStatus(IRequest req) => GetErrorStatus(req) != null;

    /// <summary>
    /// Forms the query.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormQuery(IRequest req, string name) => req.FormData[name] ?? req.QueryString[name];
    /// <summary>
    /// Forms the query values.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String[].</returns>
    public static string[] FormQueryValues(IRequest req, string name)
    {
        var values = req.Verb == HttpMethods.Post
                         ? req.FormData.GetValues(name)
                         : req.QueryString.GetValues(name);

        return values?.Length == 1 // if it's only a single item can be returned in comma-delimited list
                   ? values[0].Split(',')
                   : values ?? TypeConstants.EmptyStringArray;
    }

    /// <summary>
    /// Forms the value.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormValue(IRequest req, string name) => FormValue(req, name, null);

    /// <summary>
    /// Forms the value.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>System.String.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FormValue(IRequest req, string name, string defaultValue) => HasErrorStatus(req)
        ? FormQuery(req, name)
        : defaultValue;

    /// <summary>
    /// Forms the values.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String[].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] FormValues(IRequest req, string name) => HasErrorStatus(req)
                                                                        ? FormQueryValues(req, name)
                                                                        : null;

    /// <summary>
    /// Forms the check value.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FormCheckValue(IRequest req, string name)
    {
        var value = FormValue(req, name);
        return value == "true" || value == "True" || value == "t" || value == "on" || value == "1";
    }

    /// <summary>
    /// Gets the parameter.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public static string GetParam(IRequest req, string name) //sync with IRequest.GetParam()
    {
        string value;
        if ((value = req.Headers[HttpHeaders.XParamOverridePrefix + name]) != null) return value;
        if ((value = req.QueryString[name]) != null) return value;
        if ((value = req.FormData[name]) != null) return value;

        //IIS will assign null to params without a name: .../?some_value can be retrieved as req.Params[null]
        //TryGetValue is not happy with null dictionary keys, so we should bail out here
        if (string.IsNullOrEmpty(name)) return null;

        if (req.Cookies.TryGetValue(name, out var cookie)) return cookie.Value;

        if (req.Items.TryGetValue(name, out var oValue)) return oValue.ToString();

        return null;
    }

    /// <summary>
    /// Comma delimited field names
    /// </summary>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> ToVarNames(string fieldNames) =>
        fieldNames.Split(',').Map(x => x.Trim());

    /// <summary>
    /// Converts to strings.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="arg">The argument.</param>
    /// <returns>IEnumerable&lt;System.String&gt;.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public static IEnumerable<string> ToStrings(string filterName, object arg)
    {
        if (arg == null)
            return TypeConstants.EmptyStringArray;

        var strings = arg as IEnumerable<string> ?? (arg is string s
                                                         ? new[] { s }
                                                         : arg is IEnumerable<object> e
                                                             ? e.Map(x => x.AsString())
                                                             : throw new NotSupportedException($"{filterName} expected a collection of strings but was '{arg.GetType().Name}'"));

        return strings;
    }

    /// <summary>
    /// Show validation summary error message unless there's an error in exceptFor list of fields
    /// as validation errors will be displayed along side the field instead
    /// </summary>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="exceptFor">The except for.</param>
    /// <returns>System.String.</returns>
    public static string ValidationSummary(ResponseStatus errorStatus, string exceptFor) =>
        ValidationSummary(errorStatus, ToVarNames(exceptFor), null);
    /// <summary>
    /// Validations the summary.
    /// </summary>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="exceptFields">The except fields.</param>
    /// <param name="divAttrs">The div attrs.</param>
    /// <returns>System.String.</returns>
    public static string ValidationSummary(ResponseStatus errorStatus, ICollection<string> exceptFields, Dictionary<string, object> divAttrs)
    {
        var errorSummaryMsg = exceptFields != null
                                  ? ErrorResponseExcept(errorStatus, exceptFields)
                                  : ErrorResponseSummary(errorStatus);

        if (string.IsNullOrEmpty(errorSummaryMsg))
            return null;

        divAttrs ??= new Dictionary<string, object>();

        if (!divAttrs.ContainsKey("class") && !divAttrs.ContainsKey("className"))
            divAttrs["class"] = ValidationSummaryCssClassNames;

        return HtmlScripts.htmlDiv(errorSummaryMsg, divAttrs).ToRawString();
    }

    /// <summary>
    /// The validation summary CSS class names
    /// </summary>
    public static string ValidationSummaryCssClassNames = "alert alert-danger";
    /// <summary>
    /// The validation success CSS class names
    /// </summary>
    public static string ValidationSuccessCssClassNames = "alert alert-success";

    /// <summary>
    /// Display a "Success Alert Box"
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="divAttrs">The div attrs.</param>
    /// <returns>System.String.</returns>
    public static string ValidationSuccess(string message, Dictionary<string, object> divAttrs)
    {
        divAttrs ??= new Dictionary<string, object>();

        if (!divAttrs.ContainsKey("class") && !divAttrs.ContainsKey("className"))
            divAttrs["class"] = ValidationSuccessCssClassNames;

        return HtmlScripts.htmlDiv(message, divAttrs).ToRawString();
    }

    /// <summary>
    /// Return an error message unless there's an error in fieldNames
    /// </summary>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>System.String.</returns>
    public static string ErrorResponseExcept(ResponseStatus errorStatus, string fieldNames) =>
        ErrorResponseExcept(errorStatus, ToVarNames(fieldNames));
    /// <summary>
    /// Errors the response except.
    /// </summary>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="fieldNames">The field names.</param>
    /// <returns>System.String.</returns>
    public static string ErrorResponseExcept(ResponseStatus errorStatus, ICollection<string> fieldNames)
    {
        if (errorStatus == null)
            return null;

        var fieldNamesLookup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var fieldName in fieldNames)
        {
            fieldNamesLookup.Add(fieldName);
        }

        if (!fieldNames.IsEmpty() && !errorStatus.Errors.IsEmpty())
        {
            foreach (var fieldError in errorStatus.Errors)
            {
                if (fieldNamesLookup.Contains(fieldError.FieldName))
                    return null;
            }

            var firstFieldError = errorStatus.Errors[0];
            return firstFieldError.Message ?? firstFieldError.ErrorCode;
        }

        return errorStatus.Message ?? errorStatus.ErrorCode;
    }

    /// <summary>
    /// Return an error message unless there are field errors
    /// </summary>
    /// <param name="errorStatus">The error status.</param>
    /// <returns>System.String.</returns>
    public static string ErrorResponseSummary(ResponseStatus errorStatus)
    {
        if (errorStatus == null)
            return null;

        return errorStatus.Errors.IsEmpty()
                   ? errorStatus.Message ?? errorStatus.ErrorCode
                   : null;
    }

    /// <summary>
    /// Return an error for the specified field (if any)
    /// </summary>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    public static string ErrorResponse(ResponseStatus errorStatus, string fieldName)
    {
        if (fieldName == null)
            return ErrorResponseSummary(errorStatus);
        if (errorStatus == null || errorStatus.Errors.IsEmpty())
            return null;

        foreach (var fieldError in errorStatus.Errors)
        {
            if (fieldName.EqualsIgnoreCase(fieldError.FieldName))
                return fieldError.Message ?? fieldError.ErrorCode;
        }

        return null;
    }

    /// <summary>
    /// Converts to keyvalues.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
    public static List<KeyValuePair<string, string>> ToKeyValues(object values)
    {
        var to = new List<KeyValuePair<string, string>>();
        if (values != null)
        {
            if (values is IEnumerable<KeyValuePair<string, object>> kvps)
                foreach (var kvp in kvps) to.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value?.ToString()));
            else if (values is IEnumerable<KeyValuePair<string, string>> kvpsStr)
                foreach (var kvp in kvpsStr) to.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value));
            else if (values is IEnumerable<object> list)
                to.AddRange(from string item in list select item.AsString() into s select new KeyValuePair<string, string>(s, s));
        }
        return to;
    }

    /// <summary>
    /// Splits the string list.
    /// </summary>
    /// <param name="strings">The strings.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> SplitStringList(IEnumerable strings) => strings is null
                                                                           ? TypeConstants.EmptyStringList
                                                                           : strings as List<string> ?? (strings switch
                                                                                     {
                                                                                         IEnumerable<string> strEnum => strEnum.ToList(),
                                                                                         IEnumerable<object> objEnum => objEnum.Map(x => x.AsString()),
                                                                                         string strFields => strFields.Split(',').Map(x => x.Trim()),
                                                                                         _ => throw new NotSupportedException($"Cannot convert '{strings.GetType().Name}' to List<string>")
                                                                                     });

    /// <summary>
    /// Converts to stringlist.
    /// </summary>
    /// <param name="strings">The strings.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> ToStringList(IEnumerable strings) => strings as List<string> ?? (strings is string s
                                                                                                    ? new List<string> { s }
                                                                                                    : strings is IEnumerable<string> e
                                                                                                        ? new List<string>(e)
                                                                                                        : strings.Map(x => x.AsString()));

    /// <summary>
    /// Forms the control.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="inputOptions">The input options.</param>
    /// <returns>System.String.</returns>
    public static string FormControl(IRequest req, Dictionary<string, object> args, string tagName, InputOptions inputOptions)
    {
        tagName ??= "input";

        var options = inputOptions ?? new InputOptions();

        string id = null;
        string type = null;
        string name = null;
        string label = null;
        string size = options.Size;
        bool inline = options.Inline;

        if (args.TryGetValue("type", out var oType))
            type = oType as string;
        else
            args["type"] = type = "text";

        var notInput = tagName != "input";
        if (notInput)
        {
            type = tagName;
            args.RemoveKey("type");
        }

        var inputClass = "form-control";
        var labelClass = "form-label";
        var helpClass = "text-muted";
        var isCheck = type == "checkbox" || type == "radio";
        if (isCheck)
        {
            inputClass = "form-check-input";
            labelClass = "form-check-label";
            if (!args.ContainsKey("value"))
                args["value"] = "true";
        }
        else if (type == "range")
        {
            inputClass = "form-control-range";
        }
        if (options.LabelClass != null)
            labelClass = options.LabelClass;

        if (args.TryGetValue("id", out var oId))
        {
            if (!args.ContainsKey("name"))
                args["name"] = id = oId as string;

            if (args.TryGetValue("name", out var oName))
                name = oName as string;
        }

        string help = options.Help;
        string helpId = help != null ? (id ?? name) + "-help" : null;
        if (helpId != null)
            args["aria-describedby"] = helpId;

        if (options.Label != null)
        {
            label = options.Label;
            if (!args.ContainsKey("placeholder"))
                args["placeholder"] = label;
        }

        var values = options.Values;
        var isSingleCheck = isCheck && values == null;

        object oValue = null;
        string formValue = null;
        var isGet = req.Verb == HttpMethods.Get;
        var preserveValue = options.PreserveValue;
        if (preserveValue)
        {
            var strValue = args.TryGetValue("value", out oValue) ? oValue as string : null;
            formValue = FormValue(req, name, strValue);
            if (!isGet || !string.IsNullOrEmpty(formValue)) //only override value if POST or GET queryString has value
            {
                if (!isCheck)
                    args["value"] = formValue;
                else if (isSingleCheck)
                    args["checked"] = formValue == "true";
            }
        }
        else if (!isGet)
        {
            if (!isCheck)
            {
                args["value"] = null;
            }
        }

        var className = args.TryGetValue("class", out var oCls) || args.TryGetValue("className", out oCls)
                            ? HtmlScripts.htmlClassList(oCls)
                            : "";

        className = HtmlScripts.htmlAddClass(className, inputClass);

        if (size != null)
            className = HtmlScripts.htmlAddClass(className, inputClass + "-" + size);

        var errorMsg = ErrorResponse(GetErrorStatus(req), name);
        if (errorMsg != null)
            className = HtmlScripts.htmlAddClass(className, "is-invalid");

        args["class"] = className;

        string inputHtml = null, labelHtml = null;
        var sb = StringBuilderCache.Allocate();

        if (label != null)
        {
            var labelArgs = new Dictionary<string, object>
                                {
                                    ["html"] = label,
                                    ["class"] = labelClass,
                                };
            if (id != null)
                labelArgs["for"] = id;

            labelHtml = HtmlScripts.htmlLabel(labelArgs).AsString();
        }

        var value = (args.TryGetValue("value", out oValue)
                         ? oValue as string
                         : null)
                    ?? (oValue?.GetType().IsValueType == true
                            ? oValue.ToString()
                            : null);

        if (type == "radio")
        {
            if (values != null)
            {
                var sbInput = StringBuilderCacheAlt.Allocate();
                var kvps = ToKeyValues(values);
                foreach (var kvp in kvps)
                {
                    var cls = inline ? " custom-control-inline" : "";
                    sbInput.AppendLine($"<div class=\"custom-control custom-radio{cls}\">");
                    var inputId = name + "-" + kvp.Key;
                    var selected = kvp.Key == formValue || kvp.Key == value ? " checked" : "";
                    sbInput.AppendLine($"  <input type=\"radio\" id=\"{inputId}\" name=\"{name}\" value=\"{kvp.Key}\" class=\"custom-control-input\"{selected}>");
                    sbInput.AppendLine($"  <label class=\"custom-control-label\" for=\"{inputId}\">{kvp.Value}</label>");
                    sbInput.AppendLine("</div>");
                }
                inputHtml = StringBuilderCacheAlt.ReturnAndFree(sbInput);
            }
            else
            {
                throw new NotSupportedException("input type=radio requires 'values' inputOption containing a collection of Key/Value Pairs");
            }
        }
        else if (type == "checkbox")
        {
            if (values != null)
            {
                var sbInput = StringBuilderCacheAlt.Allocate();
                var kvps = ToKeyValues(values);

                var selectedValues = value != null && value != "true"
                                         ? new HashSet<string> { value }
                                         : oValue == null
                                             ? TypeConstants<string>.EmptyHashSet
                                             : (FormValues(req, name) ?? ToStringList(oValue as IEnumerable).ToArray())
                                             .ToSet();

                foreach (var kvp in kvps)
                {
                    var cls = inline ? " custom-control-inline" : "";
                    sbInput.AppendLine($"<div class=\"custom-control custom-checkbox{cls}\">");
                    var inputId = name + "-" + kvp.Key;
                    var selected = kvp.Key == formValue || selectedValues.Contains(kvp.Key) ? " checked" : "";
                    sbInput.AppendLine($"  <input type=\"checkbox\" id=\"{inputId}\" name=\"{name}\" value=\"{kvp.Key}\" class=\"form-check-input\"{selected}>");
                    sbInput.AppendLine($"  <label class=\"form-check-label\" for=\"{inputId}\">{kvp.Value}</label>");
                    sbInput.AppendLine("</div>");
                }
                inputHtml = StringBuilderCacheAlt.ReturnAndFree(sbInput);
            }
        }
        else if (type == "select")
        {
            if (values != null)
            {
                args["html"] = HtmlScripts.htmlOptions(values,
                    new Dictionary<string, object> { { "selected", formValue ?? value } });
            }
            else if (!args.ContainsKey("html"))
                throw new NotSupportedException("<select> requires either 'values' inputOption containing a collection of Key/Value Pairs or 'html' argument containing innerHTML <option>'s");
        }

        inputHtml ??= HtmlScripts.htmlTag(args, tagName).AsString();

        if (isCheck)
        {
            sb.AppendLine(inputHtml);
            if (isSingleCheck)
                sb.AppendLine(labelHtml);
        }
        else
        {
            sb.AppendLine(labelHtml).AppendLine(inputHtml);
        }

        if (help != null)
        {
            sb.AppendLine($"<small id='{helpId}' class='{helpClass}'>{help.HtmlEncode()}</small>");
        }

        string htmlError = null;
        if (options.ShowErrors && errorMsg != null)
        {
            var errorClass = "invalid-feedback";
            if (options.ErrorClass != null)
                errorClass = options.ErrorClass ?? "";
            htmlError = $"<div class='{errorClass}'>{errorMsg.HtmlEncode()}</div>";
        }

        if (!isCheck)
        {
            sb.AppendLine(htmlError);
        }
        else
        {
            var cls = htmlError != null ? " is-invalid form-control" : "";
            sb.Insert(0, $"<div class=\"form-check{cls}\">");
            sb.AppendLine("</div>");
            if (htmlError != null)
                sb.AppendLine(htmlError);
        }

        if (isCheck && !isSingleCheck) // multi-value checkbox/radio
            sb.Insert(0, labelHtml);

        var html = StringBuilderCache.ReturnAndFree(sb);
        return html;
    }

    /// <summary>
    /// Resolves the write VFS.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="webVfs">The web VFS.</param>
    /// <param name="contentVfs">The content VFS.</param>
    /// <param name="outFile">The out file.</param>
    /// <param name="toDisk">if set to <c>true</c> [to disk].</param>
    /// <param name="useOutFile">The use out file.</param>
    /// <returns>IVirtualFiles.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    /// <exception cref="System.NotSupportedException"></exception>
    private static IVirtualFiles ResolveWriteVfs(string filterName, IVirtualPathProvider webVfs, IVirtualPathProvider contentVfs, string outFile, bool toDisk, out string useOutFile)
    {
        if (outFile.IndexOf(':') >= 0)
        {
            ResolveVfsAndSource(filterName, webVfs, contentVfs, outFile, out var useVfs, out useOutFile);
            return (IVirtualFiles)useVfs;
        }

        useOutFile = outFile;
        var vfs = !toDisk
                      ? (IVirtualFiles)webVfs.GetMemoryVirtualFiles() ??
                        throw new NotSupportedException($"{nameof(MemoryVirtualFiles)} is required in {filterName} when disk=false")
                      : webVfs.GetFileSystemVirtualFiles() ??
                        throw new NotSupportedException($"{nameof(FileSystemVirtualFiles)} is required in {filterName} when disk=true");
        return vfs;
    }

    /// <summary>
    /// Resolves the VFS and source.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="webVfs">The web VFS.</param>
    /// <param name="contentVfs">The content VFS.</param>
    /// <param name="source">The source.</param>
    /// <param name="useVfs">The use VFS.</param>
    /// <param name="useSource">The use source.</param>
    /// <exception cref="System.NotSupportedException">Unknown Virtual File System provider '{name}' used in '{filterName}'. Valid providers: web,content,filesystem,memory</exception>
    private static void ResolveVfsAndSource(string filterName, IVirtualPathProvider webVfs, IVirtualPathProvider contentVfs, string source, out IVirtualPathProvider useVfs, out string useSource)
    {
        useVfs = webVfs;
        useSource = source;

        var parts = source.SplitOnFirst(':');
        if (parts.Length != 2)
            return;

        useSource = parts[1];
        var name = parts[0];
        useVfs = name == "content"
                     ? contentVfs
                     : name == "web"
                         ? webVfs
                         : name == "filesystem"
                             ? webVfs.GetFileSystemVirtualFiles()
                             : name == "memory"
                                 ? webVfs.GetMemoryVirtualFiles()
                                 : throw new NotSupportedException($"Unknown Virtual File System provider '{name}' used in '{filterName}'. Valid providers: web,content,filesystem,memory");
    }

    /// <summary>
    /// Gets the bundle files.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="webVfs">The web VFS.</param>
    /// <param name="contentVfs">The content VFS.</param>
    /// <param name="virtualPaths">The virtual paths.</param>
    /// <param name="assetExt">The asset ext.</param>
    /// <returns>IEnumerable&lt;IVirtualFile&gt;.</returns>
    /// <exception cref="System.NotSupportedException">Could not find resource at virtual path '{source}' in '{filterName}'</exception>
    public static IEnumerable<IVirtualFile> GetBundleFiles(string filterName, IVirtualPathProvider webVfs, IVirtualPathProvider contentVfs, IEnumerable<string> virtualPaths, string assetExt)
    {
        var excludeFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var source in virtualPaths)
        {
            ResolveVfsAndSource(filterName, webVfs, contentVfs, source, out var vfs, out var virtualPath);

            if (virtualPath.StartsWith("!"))
            {
                excludeFiles.Add(virtualPath.Substring(1).TrimStart('/'));
                continue;
            }

            var dir = vfs.GetDirectory(virtualPath);
            if (dir != null)
            {
                var files = dir.GetAllFiles();
                foreach (var dirFile in files)
                {
                    if (!assetExt.EqualsIgnoreCase(dirFile.Extension))
                        continue;
                    if (excludeFiles.Contains(dirFile.VirtualPath))
                        continue;

                    yield return dirFile;
                }
                continue;
            }

            var file = vfs.GetFile(virtualPath);
            if (file != null)
            {
                if (excludeFiles.Contains(file.VirtualPath))
                    continue;

                yield return file;
            }
            else
            {
                throw new NotSupportedException($"Could not find resource at virtual path '{source}' in '{filterName}'");
            }
        }
    }

    /// <summary>
    /// Bundles the js.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="webVfs">The web VFS.</param>
    /// <param name="contentVfs">The content VFS.</param>
    /// <param name="jsCompressor">The js compressor.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string BundleJs(string filterName,
                                  IVirtualPathProvider webVfs,
                                  IVirtualPathProvider contentVfs,
                                  ICompressor jsCompressor,
                                  BundleOptions options)
    {
        var assetExt = "js";
        var outFile = options.OutputTo ?? (options.Minify
                                               ? $"/{assetExt}/bundle.min.{assetExt}" : $"/{assetExt}/bundle.{assetExt}");
        var htmlTagFmt = "<script src=\"{0}\"></script>";

        return BundleAsset(filterName,
            webVfs,
            contentVfs,
            jsCompressor, options, outFile, options.OutputWebPath, htmlTagFmt, assetExt, options.PathBase);
    }

    /// <summary>
    /// Bundles the CSS.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="webVfs">The web VFS.</param>
    /// <param name="contentVfs">The content VFS.</param>
    /// <param name="cssCompressor">The CSS compressor.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string BundleCss(string filterName,
                                   IVirtualPathProvider webVfs,
                                   IVirtualPathProvider contentVfs,
                                   ICompressor cssCompressor,
                                   BundleOptions options)
    {
        var assetExt = "css";
        var outFile = options.OutputTo ?? (options.Minify
                                               ? $"/{assetExt}/bundle.min.{assetExt}" : $"/{assetExt}/bundle.{assetExt}");
        var htmlTagFmt = "<link rel=\"stylesheet\" href=\"{0}\">";

        return BundleAsset(filterName,
            webVfs,
            contentVfs,
            cssCompressor, options, outFile, options.OutputWebPath, htmlTagFmt, assetExt, options.PathBase);
    }

    /// <summary>
    /// Bundles the HTML.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="webVfs">The web VFS.</param>
    /// <param name="contentVfs">The content VFS.</param>
    /// <param name="htmlCompressor">The HTML compressor.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.String.</returns>
    public static string BundleHtml(string filterName,
                                    IVirtualPathProvider webVfs,
                                    IVirtualPathProvider contentVfs,
                                    ICompressor htmlCompressor,
                                    BundleOptions options)
    {
        var assetExt = "html";
        var outFile = options.OutputTo ?? (options.Minify
                                               ? $"/{assetExt}/bundle.min.{assetExt}" : $"/{assetExt}/bundle.{assetExt}");
        var id = options.OutputTo != null
                     ? $" id=\"{options.OutputTo.LastRightPart('/').LeftPart('.')}\"" : "";
        var htmlTagFmt = "<link rel=\"import\" href=\"{0}\"" + id + ">";

        return BundleAsset(filterName,
            webVfs,
            contentVfs,
            htmlCompressor, options, outFile, options.OutputWebPath, htmlTagFmt, assetExt, options.PathBase);
    }

    /// <summary>
    /// Bundles the asset.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="webVfs">The web VFS.</param>
    /// <param name="contentVfs">The content VFS.</param>
    /// <param name="jsCompressor">The js compressor.</param>
    /// <param name="options">The options.</param>
    /// <param name="origOutFile">The original out file.</param>
    /// <param name="outWebPath">The out web path.</param>
    /// <param name="htmlTagFmt">The HTML tag FMT.</param>
    /// <param name="assetExt">The asset ext.</param>
    /// <param name="pathBase">The path base.</param>
    /// <returns>System.String.</returns>
    private static string BundleAsset(string filterName,
                                      IVirtualPathProvider webVfs,
                                      IVirtualPathProvider contentVfs,
                                      ICompressor jsCompressor,
                                      BundleOptions options,
                                      string origOutFile,
                                      string outWebPath,
                                      string htmlTagFmt,
                                      string assetExt,
                                      string pathBase)
    {
        try
        {
            var writeVfs = ResolveWriteVfs(filterName, webVfs, contentVfs, origOutFile, options.SaveToDisk, out var outFilePath);

            var outHtmlTag = htmlTagFmt.Replace("{0}", pathBase == null ? outFilePath : pathBase.CombineWith(outFilePath));

            var maxDate = DateTime.MinValue;
            var hasHash = outFilePath.IndexOf("[hash]", StringComparison.Ordinal) >= 0;

            if (!options.Sources.IsEmpty() && options.Bundle && options.Cache)
            {
                if (hasHash)
                {
                    var memFs = webVfs.GetMemoryVirtualFiles();

                    var existingBundleTag = webVfs.GetFile(outFilePath);
                    if (existingBundleTag == null)
                    {
                        // use existing bundle if file with matching hash pattern is found
                        var outDirPath = outFilePath.LastLeftPart('/');
                        var outFileName = outFilePath.LastRightPart('/');
                        var outGlobFile = outFileName.Replace("[hash]", ".*");

                        // use glob search to avoid unnecessary file scans
                        var outDir = webVfs.GetDirectory(outDirPath);
                        if (outDir != null)
                        {
                            var outDirFiles = outDir.GetFiles();
                            foreach (var file in outDirFiles)
                            {
                                if (file.Name.Glob(outGlobFile))
                                {
                                    outHtmlTag = htmlTagFmt.Replace("{0}", "/" + file.VirtualPath);
                                    memFs.WriteFile(outFilePath, outHtmlTag); //cache lookup
                                    return outHtmlTag;
                                }
                            }
                        }
                    }
                    else
                    {
                        return existingBundleTag.ReadAllText();
                    }
                }
                else if (webVfs.FileExists(outWebPath ?? outFilePath))
                {
                    return outHtmlTag;
                }
            }

            var sources = GetBundleFiles(filterName, webVfs, contentVfs, options.Sources, assetExt);

            var existing = new HashSet<string>();
            var sb = StringBuilderCache.Allocate();
            var sbLog = StringBuilderCacheAlt.Allocate();

            void LogWarning(string msg)
            {
                sbLog.AppendLine()
                    .Append(assetExt == "html" ? "<!--" : "/*")
                    .Append(" WARNING: ")
                    .Append(msg)
                    .Append(assetExt == "html" ? "-->" : "*/");
            }


            var minExt = ".min." + assetExt;
            if (options.Bundle)
            {
                foreach (var file in sources)
                {
                    if (hasHash)
                    {
                        file.Refresh();
                        if (file.LastModified > maxDate)
                            maxDate = file.LastModified;
                    }

                    string src;
                    try
                    {
                        src = file.ReadAllText();
                    }
                    catch (Exception e)
                    {
                        LogWarning($"Could not read '{file.VirtualPath}': {e.Message}");
                        continue;
                    }

                    if (file.Name.EndsWith("bundle." + assetExt) ||
                        file.Name.EndsWith("bundle.min." + assetExt) ||
                        existing.Contains(file.VirtualPath))
                        continue;

                    if (options.IIFE) sb.AppendLine("(function(){");

                    if (options.Minify && !file.Name.EndsWith(minExt))
                    {
                        string minified;
                        try
                        {
                            minified = jsCompressor.Compress(src);
                        }
                        catch (Exception e)
                        {
                            LogWarning($"Could not Compress '{file.VirtualPath}': {e.Message}");
                            minified = src;
                        }

                        sb.Append(minified).Append(assetExt == "js" ? ";" : "").AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(src);
                    }

                    if (options.IIFE) sb.AppendLine("})();");

                    // Also define ES6 module in AMD's define(), required by /js/ss-require.js
                    if (options.RegisterModuleInAmd && assetExt == "js")
                    {
                        sb.AppendLine("if (typeof define === 'function' && define.amd && typeof module !== 'undefined') define('" +
                                      file.Name.WithoutExtension() + "', [], function(){ return module.exports; });");
                    }

                    existing.Add(file.VirtualPath);
                }

                var bundled = StringBuilderCache.ReturnAndFree(sb);
                if (hasHash)
                {
                    var hash = "." + maxDate.ToUnixTimeMs();
                    outHtmlTag = outHtmlTag.Replace("[hash]", hash);
                    webVfs.GetMemoryVirtualFiles().WriteFile(outFilePath, outHtmlTag); //have bundle[hash].ext return rendered html

                    outFilePath = outFilePath.Replace("[hash]", hash);
                }

                try
                {
                    writeVfs.WriteFile(outFilePath, bundled);
                }
                catch (Exception e)
                {
                    LogWarning($"Could not write to '{origOutFile}': {e.Message}");
                }

                if (sbLog.Length != 0)
                    return outHtmlTag + StringBuilderCacheAlt.ReturnAndFree(sbLog);

                StringBuilderCacheAlt.Free(sbLog);
                return outHtmlTag;
            }
            else
            {
                var filePaths = new List<string>();

                foreach (var file in sources)
                {
                    if (file.Name.EndsWith("bundle." + assetExt) ||
                        file.Name.EndsWith("bundle.min." + assetExt) ||
                        existing.Contains(file.VirtualPath))
                        continue;

                    filePaths.Add("/".CombineWith(file.VirtualPath));
                    existing.Add(file.VirtualPath);
                }

                foreach (var filePath in filePaths)
                {
                    if (filePath.EndsWith(minExt))
                    {
                        var withoutMin = filePath.Substring(0, filePath.Length - minExt.Length) + "." + assetExt;
                        if (filePaths.Contains(withoutMin))
                            continue;
                    }

                    sb.AppendLine(htmlTagFmt.Replace("{0}", filePath));
                }

                return StringBuilderCache.ReturnAndFree(sb);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}