// ***********************************************************************
// <copyright file="XLinqExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using static System.String;

namespace ServiceStack
{
    /// <summary>
    /// Class XLinqExtensions.
    /// </summary>
    public static class XLinqExtensions
    {
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string GetString(this XElement el, string name)
        {
            return el == null ? null : GetElementValueOrDefault(el, name, x => x.Value);
        }

        /// <summary>
        /// Gets the string attribute or default.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string GetStringAttributeOrDefault(this XElement element, string name)
        {
            var attr = AnyAttribute(element, name);
            return attr == null ? null : GetAttributeValueOrDefault(attr, name, x => x.Value);
        }

        /// <summary>
        /// Gets the attribute value or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attr">The attribute.</param>
        /// <param name="name">The name.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentNullException">converter</exception>
        public static T GetAttributeValueOrDefault<T>(this XAttribute attr, string name, Func<XAttribute, T> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            return IsNullOrEmpty(attr?.Value) ? default(T) : converter(attr);
        }

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetBool(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (bool)GetElement(el, name);
        }

        /// <summary>
        /// Gets the bool or default.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetBoolOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (bool)x);
        }

        /// <summary>
        /// Gets the nullable bool.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool? GetNullableBool(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return IsNullOrEmpty(childEl?.Value) ? null : (bool?)childEl;
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        public static int GetInt(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (int)GetElement(el, name);
        }

        /// <summary>
        /// Gets the int or default.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        public static int GetIntOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (int)x);
        }

        /// <summary>
        /// Gets the nullable int.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        public static int? GetNullableInt(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return IsNullOrEmpty(childEl?.Value) ? null : (int?)childEl;
        }

        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Int64.</returns>
        public static long GetLong(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (long)GetElement(el, name);
        }

        /// <summary>
        /// Gets the long or default.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Int64.</returns>
        public static long GetLongOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (long)x);
        }

        /// <summary>
        /// Gets the nullable long.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        public static long? GetNullableLong(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return IsNullOrEmpty(childEl?.Value) ? null : (long?)childEl;
        }

        /// <summary>
        /// Gets the decimal.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal GetDecimal(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (decimal)GetElement(el, name);
        }

        /// <summary>
        /// Gets the decimal or default.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal GetDecimalOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (decimal)x);
        }

        /// <summary>
        /// Gets the nullable decimal.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Nullable&lt;System.Decimal&gt;.</returns>
        public static decimal? GetNullableDecimal(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return IsNullOrEmpty(childEl?.Value) ? null : (decimal?)childEl;
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>DateTime.</returns>
        public static DateTime GetDateTime(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (DateTime)GetElement(el, name);
        }

        /// <summary>
        /// Gets the date time or default.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>DateTime.</returns>
        public static DateTime GetDateTimeOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (DateTime)x);
        }

        /// <summary>
        /// Gets the nullable date time.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Nullable&lt;DateTime&gt;.</returns>
        public static DateTime? GetNullableDateTime(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return IsNullOrEmpty(childEl?.Value) ? null : (DateTime?)childEl;
        }

        /// <summary>
        /// Gets the time span.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan GetTimeSpan(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (TimeSpan)GetElement(el, name);
        }

        /// <summary>
        /// Gets the time span or default.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan GetTimeSpanOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (TimeSpan)x);
        }

        /// <summary>
        /// Gets the nullable time span.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Nullable&lt;TimeSpan&gt;.</returns>
        public static TimeSpan? GetNullableTimeSpan(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return IsNullOrEmpty(childEl?.Value) ? null : (TimeSpan?)childEl;
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>Guid.</returns>
        public static Guid GetGuid(this XElement el, string name)
        {
            AssertElementHasValue(el, name);
            return (Guid)GetElement(el, name);
        }

        /// <summary>
        /// Gets the unique identifier or default.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>Guid.</returns>
        public static Guid GetGuidOrDefault(this XElement el, string name)
        {
            return GetElementValueOrDefault(el, name, x => (Guid)x);
        }

        /// <summary>
        /// Gets the nullable unique identifier.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Nullable&lt;Guid&gt;.</returns>
        public static Guid? GetNullableGuid(this XElement el, string name)
        {
            var childEl = GetElement(el, name);
            return IsNullOrEmpty(childEl?.Value) ? null : (Guid?)childEl;
        }

        /// <summary>
        /// Gets the element value or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentNullException">converter</exception>
        public static T GetElementValueOrDefault<T>(this XElement element, string name, Func<XElement, T> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            var el = GetElement(element, name);
            return IsNullOrEmpty(el?.Value) ? default(T) : converter(el);
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns>XElement.</returns>
        /// <exception cref="System.ArgumentNullException">element</exception>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public static XElement GetElement(this XElement element, string name)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return element.AnyElement(name);
        }

        /// <summary>
        /// Asserts the element has value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">element</exception>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void AssertElementHasValue(this XElement element, string name)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var childEl = element.AnyElement(name);
            if (childEl == null || IsNullOrEmpty(childEl.Value))
            {
                throw new ArgumentNullException(name, $"{name} is required");
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="els">The els.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetValues(this IEnumerable<XElement> els)
        {
            var values = new List<string>();
            foreach (var el in els)
            {
                values.Add(el.Value);
            }
            return values;
        }

        /// <summary>
        /// Anies the attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns>XAttribute.</returns>
        public static XAttribute AnyAttribute(this XElement element, string name)
        {
            if (element == null) return null;
            foreach (var attribute in element.Attributes())
            {
                if (attribute.Name.LocalName == name)
                {
                    return attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// Alls the elements.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;XElement&gt;.</returns>
        public static IEnumerable<XElement> AllElements(this XElement element, string name)
        {
            var els = new List<XElement>();
            if (element == null) return els;
            foreach (var node in element.Nodes())
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                var childEl = (XElement)node;
                if (childEl.Name.LocalName == name)
                {
                    els.Add(childEl);
                }
            }
            return els;
        }

        /// <summary>
        /// Anies the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns>XElement.</returns>
        public static XElement AnyElement(this XElement element, string name)
        {
            if (element == null) return null;
            foreach (var node in element.Nodes())
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                var childEl = (XElement)node;
                if (childEl.Name.LocalName == name)
                {
                    return childEl;
                }
            }
            return null;
        }

        /// <summary>
        /// Anies the element.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="name">The name.</param>
        /// <returns>XElement.</returns>
        public static XElement AnyElement(this IEnumerable<XElement> elements, string name)
        {
            foreach (var element in elements)
            {
                if (element.Name.LocalName == name)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Alls the elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;XElement&gt;.</returns>
        public static IEnumerable<XElement> AllElements(this IEnumerable<XElement> elements, string name)
        {
            var els = new List<XElement>();
            foreach (var element in elements)
            {
                els.AddRange(AllElements(element, name));
            }
            return els;
        }

        /// <summary>
        /// Firsts the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>XElement.</returns>
        public static XElement FirstElement(this XElement element)
        {
            if (element.FirstNode.NodeType == XmlNodeType.Element)
            {
                return (XElement)element.FirstNode;
            }
            return null;
        }

        /// <summary>
        /// Nexts the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>XElement.</returns>
        public static XElement NextElement(this XElement element)
        {
            var node = element.NextNode;
            while (node != null)
            {
                if (node.NodeType == XmlNodeType.Element)
                    return (XElement) node;

                node = node.NextNode;
            }
            return null;
        }

    }
}
