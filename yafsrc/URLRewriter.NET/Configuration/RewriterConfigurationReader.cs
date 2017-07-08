// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Xml;

    using Intelligencia.UrlRewriter.Errors;
    using Intelligencia.UrlRewriter.Logging;
    using Intelligencia.UrlRewriter.Transforms;
    using Intelligencia.UrlRewriter.Utilities;

    /// <summary>
    /// Reads configuration from an XML Node.
    /// </summary>
    public sealed class RewriterConfigurationReader
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        private RewriterConfigurationReader()
        {
        }

        /// <summary>
        /// Reads configuration information from the given XML Node.
        /// </summary>
        /// <param name="section">The XML node to read configuration from.</param>
        /// <returns>The configuration information.</returns>
        public static object Read(XmlNode section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }

            var config = RewriterConfiguration.Create();

            foreach (XmlNode node in section.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node.LocalName == Constants.ElementErrorHandler)
                    {
                        ReadErrorHandler(node, config);
                    }
                    else if (node.LocalName == Constants.ElementDefaultDocuments)
                    {
                        ReadDefaultDocuments(node, config);
                    }
                    else if (node.LocalName == Constants.ElementRegister)
                    {
                        if (node.Attributes[Constants.AttrParser] != null)
                        {
                            ReadRegisterParser(node, config);
                        }
                        else if (node.Attributes[Constants.AttrTransform] != null)
                        {
                            ReadRegisterTransform(node, config);
                        }
                        else if (node.Attributes[Constants.AttrLogger] != null)
                        {
                            ReadRegisterLogger(node, config);
                        }
                    }
                    else if (node.LocalName == Constants.ElementMapping)
                    {
                        ReadMapping(node, config);
                    }
                    else
                    {
                        ReadRule(node, config);
                    }
                }
            }

            return config;
        }

        private static void ReadRegisterTransform(XmlNode node, RewriterConfiguration config)
        {
            // Type attribute.
            XmlNode typeNode = node.Attributes[Constants.AttrTransform];
            if (typeNode == null)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrTransform),
                    node);
            }

            if (node.ChildNodes.Count > 0)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.ElementNoElements, Constants.ElementRegister),
                    node);
            }

            // Transform type specified.  Create an instance and add it
            // as the mapper handler for this map.
            var handler = TypeHelper.Activate(typeNode.Value, null) as IRewriteTransform;
            if (handler != null)
            {
                config.TransformFactory.AddTransform(handler);
            }
        }

        private static void ReadRegisterLogger(XmlNode node, RewriterConfiguration config)
        {
            // Type attribute.
            XmlNode typeNode = node.Attributes[Constants.AttrLogger];
            if (typeNode == null)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrLogger),
                    node);
            }

            if (node.ChildNodes.Count > 0)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.ElementNoElements, Constants.ElementRegister),
                    node);
            }

            // Logger type specified.  Create an instance and add it
            // as the mapper handler for this map.
            var logger = TypeHelper.Activate(typeNode.Value, null) as IRewriteLogger;
            if (logger != null)
            {
                config.Logger = logger;
            }
        }

        private static void ReadRegisterParser(XmlNode node, RewriterConfiguration config)
        {
            XmlNode typeNode = node.Attributes[Constants.AttrParser];
            if (typeNode == null)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrParser),
                    node);
            }

            if (node.ChildNodes.Count > 0)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.ElementNoElements, Constants.ElementRegister),
                    node);
            }

            var parser = TypeHelper.Activate(typeNode.Value, null);
            var actionParser = parser as IRewriteActionParser;
            if (actionParser != null)
            {
                config.ActionParserFactory.AddParser(actionParser);
            }

            var conditionParser = parser as IRewriteConditionParser;
            if (conditionParser != null)
            {
                config.ConditionParserPipeline.AddParser(conditionParser);
            }
        }

        private static void ReadDefaultDocuments(XmlNode node, RewriterConfiguration config)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element && childNode.LocalName == Constants.ElementDocument)
                {
                    config.DefaultDocuments.Add(childNode.InnerText);
                }
            }
        }

        private static void ReadErrorHandler(XmlNode node, RewriterConfiguration config)
        {
            XmlNode codeNode = node.Attributes[Constants.AttrCode];
            if (codeNode == null)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrCode),
                    node);
            }

            XmlNode typeNode = node.Attributes[Constants.AttrType];
            XmlNode urlNode = node.Attributes[Constants.AttrUrl];
            if (typeNode == null && urlNode == null)
            {
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrUrl),
                    node);
            }

            IRewriteErrorHandler handler = null;
            if (typeNode != null)
            {
                // <error-handler code="500" url="/oops.aspx" />
                handler = TypeHelper.Activate(typeNode.Value, null) as IRewriteErrorHandler;
                if (handler == null)
                {
                    throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.InvalidTypeSpecified));
                }
            }
            else
            {
                handler = new DefaultErrorHandler(urlNode.Value);
            }

            config.ErrorHandlers.Add(Convert.ToInt32(codeNode.Value), handler);
        }

        private static void ReadMapping(XmlNode node, RewriterConfiguration config)
        {
            // Name attribute.
            XmlNode nameNode = node.Attributes[Constants.AttrName];

            // Mapper type not specified.  Load in the hash map.
            var map = new StringDictionary();
            foreach (XmlNode mapNode in node.ChildNodes)
            {
                if (mapNode.NodeType == XmlNodeType.Element)
                {
                    if (mapNode.LocalName == Constants.ElementMap)
                    {
                        XmlNode fromValueNode = mapNode.Attributes[Constants.AttrFrom];
                        if (fromValueNode == null)
                        {
                            throw new ConfigurationErrorsException(
                                MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrFrom),
                                node);
                        }

                        XmlNode toValueNode = mapNode.Attributes[Constants.AttrTo];
                        if (toValueNode == null)
                        {
                            throw new ConfigurationErrorsException(
                                MessageProvider.FormatString(Message.AttributeRequired, Constants.AttrTo),
                                node);
                        }

                        map.Add(fromValueNode.Value, toValueNode.Value);
                    }
                    else
                    {
                        throw new ConfigurationErrorsException(
                            MessageProvider.FormatString(Message.ElementNotAllowed, mapNode.LocalName),
                            node);
                    }
                }
            }

            config.TransformFactory.AddTransform(new StaticMappingTransform(nameNode.Value, map));
        }

        private static void ReadRule(XmlNode node, RewriterConfiguration config)
        {
            var parsed = false;
            var parsers = config.ActionParserFactory.GetParsers(node.LocalName);
            if (parsers != null)
            {
                foreach (IRewriteActionParser parser in parsers)
                {
                    if (!parser.AllowsNestedActions && node.ChildNodes.Count > 0)
                    {
                        throw new ConfigurationErrorsException(
                            MessageProvider.FormatString(Message.ElementNoElements, parser.Name),
                            node);
                    }

                    if (!parser.AllowsAttributes && node.Attributes.Count > 0)
                    {
                        throw new ConfigurationErrorsException(
                            MessageProvider.FormatString(Message.ElementNoAttributes, parser.Name),
                            node);
                    }

                    var rule = parser.Parse(node, config);
                    if (rule != null)
                    {
                        config.Rules.Add(rule);
                        parsed = true;
                        break;
                    }
                }
            }

            if (!parsed)
            {
                // No parsers recognised to handle this node.
                throw new ConfigurationErrorsException(
                    MessageProvider.FormatString(Message.ElementNotAllowed, node.LocalName),
                    node);
            }
        }
    }
}