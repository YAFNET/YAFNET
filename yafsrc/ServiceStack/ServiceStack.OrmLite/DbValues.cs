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
    public XmlValue(string xml)
    {
        this.Xml = xml;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return this.Xml;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Equals(XmlValue other)
    {
        return this.Xml == other.Xml;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return obj is XmlValue other && this.Equals(other);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return this.Xml != null ? this.Xml.GetHashCode() : 0;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="string" /> to <see cref="XmlValue" />.
    /// </summary>
    /// <param name="expandedName">Name of the expanded.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator XmlValue(string expandedName)
    {
        return expandedName != null ? new XmlValue(expandedName) : null;
    }
}