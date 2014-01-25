// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Xml;
using System.Configuration;
using Intelligencia.UrlRewriter.Conditions;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Parsers
{
	/// <summary>
	/// Parser for property match conditions.
	/// </summary>
	public sealed class PropertyMatchConditionParser : IRewriteConditionParser
	{
		/// <summary>
		/// Parses the condition.
		/// </summary>
		/// <param name="node">The node to parse.</param>
		/// <returns>The condition parsed, or null if nothing parsed.</returns>
		public IRewriteCondition Parse(XmlNode node)
		{
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            
            XmlNode propertyAttr = node.Attributes.GetNamedItem(Constants.AttrProperty);
			if (propertyAttr != null)
			{
				string propertyName = propertyAttr.Value;

				XmlNode matchAttr = node.Attributes.GetNamedItem(Constants.AttrMatch);
				if (matchAttr != null)
				{
					return new PropertyMatchCondition(propertyName, matchAttr.Value);
				}
				else
				{
                    throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrMatch), node);
				}
			}

			return null;
		}
	}
}
