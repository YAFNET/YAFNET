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
    /// Whether to show Error Message associated with this control
    /// </summary>
    /// <value><c>true</c> if [show errors]; otherwise, <c>false</c>.</value>
    public bool ShowErrors { get; set; } = true;
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
}

/// <summary>
/// Shared Utils shared between different Template Filters and Razor Views/Helpers
/// </summary>
public static class ViewUtils
{
    /// <summary>
    /// The default scripts
    /// </summary>
    readonly static internal DefaultScripts DefaultScripts = new();

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
    /// Gets the nav items.
    /// </summary>
    /// <value>The nav items.</value>
    public static List<NavItem> NavItems { get; } = [];
    /// <summary>
    /// Gets the nav items map.
    /// </summary>
    /// <value>The nav items map.</value>
    public static Dictionary<string, List<NavItem>> NavItemsMap { get; } = new();


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
}