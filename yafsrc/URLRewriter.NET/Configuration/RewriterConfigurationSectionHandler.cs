// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Xml;
using System.Configuration;

namespace Intelligencia.UrlRewriter.Configuration
{
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
