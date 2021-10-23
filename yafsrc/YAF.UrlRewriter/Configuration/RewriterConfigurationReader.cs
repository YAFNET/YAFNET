// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;
    using System.Xml;

    using YAF.Types.Extensions;
    using YAF.UrlRewriter.Errors;
    using YAF.UrlRewriter.Extensions;
    using YAF.UrlRewriter.Logging;
    using YAF.UrlRewriter.Parsers;
    using YAF.UrlRewriter.Transforms;
    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Reads configuration from an XML Node.
    /// </summary>
    public static class RewriterConfigurationReader
    {
        /// <summary>
        /// Reads configuration information from the given XML Node.
        /// </summary>
        /// <param name="config">The rewriter configuration object to populate.</param>
        /// <param name="section">The XML node to read configuration from.</param>
        /// <returns>The configuration information.</returns>
        public static void Read(IRewriterConfiguration config, XmlNode section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            foreach (XmlNode node in section.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                switch (node.LocalName)
                {
                    case Constants.ElementErrorHandler:
                        ReadErrorHandler(node, config);
                        break;
                    case Constants.ElementDefaultDocuments:
                        ReadDefaultDocuments(node, config);
                        break;
                    case Constants.ElementRegister when node.Attributes[Constants.AttrParser] != null:
                        ReadRegisterParser(node, config);
                        break;
                    case Constants.ElementRegister when node.Attributes[Constants.AttrTransform] != null:
                        ReadRegisterTransform(node, config);
                        break;
                    case Constants.ElementRegister:
                        {
                            if (node.Attributes[Constants.AttrLogger] != null)
                            {
                                ReadRegisterLogger(node, config);
                            }

                            break;
                        }

                    case Constants.ElementMapping:
                        ReadMapping(node, config);
                        break;
                    default:
                        ReadRule(node, config);
                        break;
                }
            }
        }

        private static void ReadRegisterTransform(XmlNode node, IRewriterConfiguration config)
        {
            if (node.ChildNodes.Count > 0)
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNoElements, Constants.ElementRegister), node);
            }

            var type = node.GetRequiredAttribute(Constants.AttrTransform);

            // Transform type specified.
            // Create an instance and add it as the mapper handler for this map.
            if (!(TypeHelper.Activate(type, null) is IRewriteTransform transform))
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.InvalidTypeSpecified, type, typeof(IRewriteTransform)), node);
            }

            config.TransformFactory.Add(transform);
        }

        private static void ReadRegisterLogger(XmlNode node, IRewriterConfiguration config)
        {
            if (node.ChildNodes.Count > 0)
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNoElements, Constants.ElementRegister), node);
            }

            var type = node.GetRequiredAttribute(Constants.AttrLogger);

            // Logger type specified.  Create an instance and add it
            // as the mapper handler for this map.
            if (TypeHelper.Activate(type, null) is IRewriteLogger logger)
            {
                config.Logger = logger;
            }
        }

        private static void ReadRegisterParser(XmlNode node, IRewriterConfiguration config)
        {
            if (node.ChildNodes.Count > 0)
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNoElements, Constants.ElementRegister), node);
            }

            var type = node.GetRequiredAttribute(Constants.AttrParser);

            var parser = TypeHelper.Activate(type, null);

            switch (parser)
            {
                case IRewriteActionParser actionParser:
                    config.ActionParserFactory.Add(actionParser);
                    break;
                case IRewriteConditionParser conditionParser:
                    config.ConditionParserPipeline.Add(conditionParser);
                    break;
            }
        }

        private static void ReadDefaultDocuments(XmlNode node, IRewriterConfiguration config)
        {
            node.ChildNodes.Cast<XmlNode>().ForEach(childNode =>
            {
                if (childNode.NodeType == XmlNodeType.Element && childNode.LocalName == Constants.ElementDocument)
                {
                    config.DefaultDocuments.Add(childNode.InnerText);
                }
            });
        }

        private static void ReadErrorHandler(XmlNode node, IRewriterConfiguration config)
        {
            var code = node.GetRequiredAttribute(Constants.AttrCode);

            XmlNode typeNode = node.Attributes[Constants.AttrType];
            XmlNode urlNode = node.Attributes[Constants.AttrUrl];
            if (typeNode == null && urlNode == null)
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrUrl), node);
            }

            IRewriteErrorHandler handler;
            if (typeNode != null)
            {
                // <error-handler code="500" url="/oops.aspx" />
                handler = TypeHelper.Activate(typeNode.Value, null) as IRewriteErrorHandler;
                if (handler == null)
                {
                    throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.InvalidTypeSpecified, typeNode.Value, typeof(IRewriteErrorHandler)), node);
                }
            }
            else
            {
                handler = new DefaultErrorHandler(urlNode.Value);
            }

            if (!int.TryParse(code, out var statusCode))
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.InvalidHttpStatusCode, code), node);
            }

            config.ErrorHandlers.Add(statusCode, handler);
        }

        private static void ReadMapping(XmlNode node, IRewriterConfiguration config)
        {
            // Name attribute.
            var mappingName = node.GetRequiredAttribute(Constants.AttrName);

            // Mapper type not specified.  Load in the hash map.
            var map = new StringDictionary();
            foreach (XmlNode mapNode in node.ChildNodes)
            {
                if (mapNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (mapNode.LocalName == Constants.ElementMap)
                {
                    var fromValue = mapNode.GetRequiredAttribute(Constants.AttrFrom, true);
                    var toValue = mapNode.GetRequiredAttribute(Constants.AttrTo, true);

                    map.Add(fromValue, toValue);
                }
                else
                {
                    throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNotAllowed, mapNode.LocalName), node);
                }
            }

            IRewriteTransform mapping = new StaticMappingTransform(mappingName, map);

            config.TransformFactory.Add(mapping);
        }

        private static void ReadRule(XmlNode node, IRewriterConfiguration config)
        {
            var parsed = false;
            var parsers = config.ActionParserFactory.GetParsers(node.LocalName);
            if (parsers != null)
            {
                foreach (var parser in parsers)
                {
                    if (!parser.AllowsNestedActions && node.ChildNodes.Count > 0)
                    {
                        throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNoElements, parser.Name), node);
                    }

                    if (!parser.AllowsAttributes && node.Attributes.Count > 0)
                    {
                        throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNoAttributes, parser.Name), node);
                    }

                    var rule = parser.Parse(node, config);

                    if (rule == null)
                    {
                        continue;
                    }

                    config.Rules.Add(rule);
                    parsed = true;
                    break;
                }
            }

            if (!parsed)
            {
                // No parsers recognized to handle this node.
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.ElementNotAllowed, node.LocalName), node);
            }
        }
    }
}
