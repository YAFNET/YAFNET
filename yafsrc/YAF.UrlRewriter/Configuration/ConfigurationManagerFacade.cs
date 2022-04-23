// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Configuration;

using System.Collections.Specialized;
using System.Configuration;

/// <summary>
/// A naive pass-through implementation of the IConfigurationManager facade that proxys calls to the
/// ASP.NET ConfigurationManager.
/// </summary>
public class ConfigurationManagerFacade : IConfigurationManager
{
    /// <summary>
    /// Retrieves a configuration section from the web application's config file.
    /// </summary>
    /// <param name="sectionName">The configuration section name</param>
    /// <returns>The configuration section class instance</returns>
    public object GetSection(string sectionName)
    {
        return ConfigurationManager.GetSection(sectionName);
    }

    /// <summary>
    /// Gets the AppSettings from the web application's config file.
    /// </summary>
    public NameValueCollection AppSettings => ConfigurationManager.AppSettings;
}