// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Specialized;

namespace Intelligencia.UrlRewriter.Configuration
{
    /// <summary>
    /// Interface for a facade to the ASP.NET ConfigurationManager.
    /// Useful for plugging out the ConfigurationManager in unit tests.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Retrieves a configuration section from the web application's config file.
        /// </summary>
        /// <param name="sectionName">The configuration section name</param>
        /// <returns>The configuration section class instance</returns>
        object GetSection(string sectionName);

        /// <summary>
        /// Gets the AppSettings from the web application's config file.
        /// </summary>
        NameValueCollection AppSettings { get; }
    }
}
