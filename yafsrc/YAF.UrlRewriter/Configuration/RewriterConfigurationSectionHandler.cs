// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Configuration
{
    using System.Configuration;
    using System.Xml;

    /// <summary>
    /// Configuration section handler for the rewriter section.
    /// </summary>
    public sealed class RewriterConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates the settings object.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="configContext">The configuration context.</param>
        /// <param name="section">The section.</param>
        /// <returns>The settings object.</returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}
