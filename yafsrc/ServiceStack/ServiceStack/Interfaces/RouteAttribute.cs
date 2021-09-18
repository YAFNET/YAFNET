// ***********************************************************************
// <copyright file="RouteAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ServiceStack
{
    using System.IO;

    /// <summary>
    /// Used to decorate Request DTO's to associate a RESTful request
    /// path mapping with a service.  Multiple attributes can be applied to
    /// each request DTO, to map multiple paths to the service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RouteAttribute : AttributeBase, IReflectAttributeConverter
    {
        /// <summary>
        /// Initializes an instance of the <see cref="RouteAttribute" /> class.
        /// </summary>
        /// <param name="path">The path template to map to the request.  See
        /// <see cref="Path">RouteAttribute.Path</see>
        /// for details on the correct format.</param>
        public RouteAttribute(string path)
            : this(path, null)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="RouteAttribute" /> class.
        /// </summary>
        /// <param name="path">The path template to map to the request.  See
        /// <see cref="Path">RouteAttribute.Path</see>
        /// for details on the correct format.</param>
        /// <param name="verbs">A comma-delimited list of HTTP verbs supported by the
        /// service.  If unspecified, all verbs are assumed to be supported.</param>
        public RouteAttribute(string path, string verbs)
        {
            Path = path;
            Verbs = verbs;
        }

        /// <summary>
        /// Gets or sets the path template to be mapped to the request.
        /// </summary>
        /// <value>A <see cref="string" /> value providing the path mapped to
        /// the request.  Never <see langword="null" />.</value>
        /// <remarks><para>Some examples of valid paths are:</para>
        /// <list>
        ///   <item>"/Inventory"</item>
        ///   <item>"/Inventory/{Category}/{ItemId}"</item>
        ///   <item>"/Inventory/{ItemPath*}"</item>
        /// </list>
        /// <para>Variables are specified within "{}"
        /// brackets.  Each variable in the path is mapped to the same-named property
        /// on the request DTO.  At runtime, ServiceStack will parse the
        /// request URL, extract the variable values, instantiate the request DTO,
        /// and assign the variable values into the corresponding request properties,
        /// prior to passing the request DTO to the service object for processing.</para>
        /// <para>It is not necessary to specify all request properties as
        /// variables in the path.  For unspecified properties, callers may provide
        /// values in the query string.  For example: the URL
        /// "http://services/Inventory?Category=Books&amp;ItemId=12345" causes the same
        /// request DTO to be processed as "http://services/Inventory/Books/12345",
        /// provided that the paths "/Inventory" (which supports the first URL) and
        /// "/Inventory/{Category}/{ItemId}" (which supports the second URL)
        /// are both mapped to the request DTO.</para>
        /// <para>Please note that while it is possible to specify property values
        /// in the query string, it is generally considered to be less RESTful and
        /// less desirable than to specify them as variables in the path.  Using the
        /// query string to specify property values may also interfere with HTTP
        /// caching.</para>
        /// <para>The final variable in the path may contain a "*" suffix
        /// to grab all remaining segments in the path portion of the request URL and assign
        /// them to a single property on the request DTO.
        /// For example, if the path "/Inventory/{ItemPath*}" is mapped to the request DTO,
        /// then the request URL "http://services/Inventory/Books/12345" will result
        /// in a request DTO whose ItemPath property contains "Books/12345".
        /// You may only specify one such variable in the path, and it must be positioned at
        /// the end of the path.</para></remarks>
        public string Path { get; }

        /// <summary>
        /// Gets or sets short summary of what the route does.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets longer text to explain the behaviour of the route.
        /// </summary>
        /// <value>The notes.</value>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets a comma-delimited list of HTTP verbs supported by the service, such as
        /// "GET,PUT,POST,DELETE".
        /// </summary>
        /// <value>A <see cref="String" /> providing a comma-delimited list of HTTP verbs supported
        /// by the service, <see langword="null" /> or empty if all verbs are supported.</value>
        public string Verbs { get; set; }

        /// <summary>
        /// Used to rank the precedences of route definitions in reverse routing.
        /// i.e. Priorities below 0 are auto-generated have less precedence.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Must match rule defined in Config.RequestRules or Regex expression with format:
        /// "{IHttpRequest.Field} =~ {pattern}", e.g "PathInfo =~ \/[0-9]+$"
        /// </summary>
        /// <value>The matches.</value>
        public string Matches { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>bool.</returns>
        protected bool Equals(RouteAttribute other)
        {
            return base.Equals(other)
                && string.Equals(Path, other.Path)
                && string.Equals(Summary, other.Summary)
                && string.Equals(Notes, other.Notes)
                && string.Equals(Verbs, other.Verbs)
                && Priority == other.Priority;
        }

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((RouteAttribute)obj);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>int.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Summary != null ? Summary.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Notes != null ? Notes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Verbs != null ? Verbs.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Priority;
                return hashCode;
            }
        }

        /// <summary>
        /// Converts to reflectattribute.
        /// </summary>
        /// <returns>ReflectAttribute.</returns>
        public ReflectAttribute ToReflectAttribute()
        {
            if (Summary == null && Notes == null && Matches == null && Priority == default)
            {
                // Return ideal Constructor Args 
                if (Path != null && Verbs != null)
                {
                    return new ReflectAttribute
                    {
                        ConstructorArgs = new List<KeyValuePair<PropertyInfo, object>> {
                            new(GetType().GetProperty(nameof(Path)), Path),
                            new(GetType().GetProperty(nameof(Verbs)), Verbs),
                        }
                    };
                }

                return new ReflectAttribute
                {
                    ConstructorArgs = new List<KeyValuePair<PropertyInfo, object>> {
                        new(GetType().GetProperty(nameof(Path)), Path),
                    }
                };
            }

            // Otherwise return Property Args
            var to = new ReflectAttribute
            {
                PropertyArgs = new List<KeyValuePair<PropertyInfo, object>> {
                    new(GetType().GetProperty(nameof(Path)), Path),
                }
            };
            if (Verbs != null)
                to.PropertyArgs.Add(new KeyValuePair<PropertyInfo, object>(GetType().GetProperty(nameof(Verbs)), Verbs));
            if (Summary != null)
                to.PropertyArgs.Add(new KeyValuePair<PropertyInfo, object>(GetType().GetProperty(nameof(Summary)), Summary));
            if (Notes != null)
                to.PropertyArgs.Add(new KeyValuePair<PropertyInfo, object>(GetType().GetProperty(nameof(Notes)), Notes));
            if (Matches != null)
                to.PropertyArgs.Add(new KeyValuePair<PropertyInfo, object>(GetType().GetProperty(nameof(Matches)), Matches));
            if (Priority != default)
                to.PropertyArgs.Add(new KeyValuePair<PropertyInfo, object>(GetType().GetProperty(nameof(Priority)), Priority));
            return to;
        }
    }

    /// <summary>
    /// Fallback routes have the lowest precedence, i.e. after normal Routes, static files or any matching Catch All Handlers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FallbackRouteAttribute : RouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackRouteAttribute"/> class.
        /// </summary>
        /// <param name="path">The path template to map to the request.  See
        /// <see cref="Path">RouteAttribute.Path</see>
        /// for details on the correct format.</param>
        public FallbackRouteAttribute(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackRouteAttribute"/> class.
        /// </summary>
        /// <param name="path">The path template to map to the request.  See
        /// <see cref="Path">RouteAttribute.Path</see>
        /// for details on the correct format.</param>
        /// <param name="verbs">A comma-delimited list of HTTP verbs supported by the
        /// service.  If unspecified, all verbs are assumed to be supported.</param>
        public FallbackRouteAttribute(string path, string verbs) : base(path, verbs) { }
    }
}
