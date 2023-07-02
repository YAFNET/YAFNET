// ***********************************************************************
// <copyright file="DbValues.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite;

/// <summary>
/// Struct XmlValue
/// </summary>
public struct XmlValue
{
    /// <summary>
    /// Gets the XML.
    /// </summary>
    /// <value>The XML.</value>
    public string Xml { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlValue" /> struct.
    /// </summary>
    /// <param name="xml">The XML.</param>
    public XmlValue(string xml) => Xml = xml;
    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => Xml;

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Equals(XmlValue other) => Xml == other.Xml;

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) => obj is XmlValue other && Equals(other);

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => Xml != null ? Xml.GetHashCode() : 0;

    /// <summary>
    /// Performs an implicit conversion from <see cref="System.String" /> to <see cref="XmlValue" />.
    /// </summary>
    /// <param name="expandedName">Name of the expanded.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator XmlValue(string expandedName) =>
        expandedName != null ? new XmlValue(expandedName) : null;
}