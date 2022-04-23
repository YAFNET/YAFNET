// ***********************************************************************
// <copyright file="NavItem.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace ServiceStack;

/// <summary>
/// NavItem in ViewUtils.NavItems and ViewUtils.NavItemsMap
/// </summary>
public class NavItem : IMeta
{
    /// <summary>
    /// Link Label
    /// </summary>
    /// <value>The label.</value>
    public string Label { get; set; }

    /// <summary>
    /// Link href
    /// </summary>
    /// <value>The href.</value>
    public string Href { get; set; }

    /// <summary>
    /// Whether NavItem should only be considered active when paths
    /// are an exact match otherwise checks if ActivePath starts with Path
    /// </summary>
    /// <value><c>null</c> if [exact] contains no value, <c>true</c> if [exact]; otherwise, <c>false</c>.</value>
    public bool? Exact { get; set; }

    /// <summary>
    /// Emit id="{Id}"
    /// </summary>
    /// <value>The identifier.</value>
    public string Id { get; set; }

    /// <summary>
    /// Override class="{Class}"
    /// </summary>
    /// <value>The name of the class.</value>
    public string ClassName { get; set; }

    /// <summary>
    /// Icon class (if any)
    /// </summary>
    /// <value>The icon class.</value>
    public string IconClass { get; set; }

    /// <summary>
    /// Only show if NavOptions.Attributes.Contains(Show)
    /// </summary>
    /// <value>The show.</value>
    public string Show { get; set; }

    /// <summary>
    /// Do not show if NavOptions.Attributes.Contains(Hide)
    /// </summary>
    /// <value>The hide.</value>
    public string Hide { get; set; }

    /// <summary>
    /// Sub Menu Child NavItems
    /// </summary>
    /// <value>The children.</value>
    public List<NavItem> Children { get; set; }

    /// <summary>
    /// Attach additional custom metadata to this NavItem
    /// </summary>
    /// <value>The meta.</value>
    public Dictionary<string, string> Meta { get; set; }
}