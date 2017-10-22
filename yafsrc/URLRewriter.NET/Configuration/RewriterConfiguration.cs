// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;
using Intelligencia.UrlRewriter.Parsers;
using Intelligencia.UrlRewriter.Transforms;
using Intelligencia.UrlRewriter.Utilities;
using Intelligencia.UrlRewriter.Logging;

namespace Intelligencia.UrlRewriter.Configuration
{
    /// <summary>
    /// Configuration for the URL rewriter.
    /// </summary>
    public class RewriterConfiguration : IRewriterConfiguration
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RewriterConfiguration()
            : this(new ConfigurationManagerFacade())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configurationManager">The configuration manager instance</param>
        public RewriterConfiguration(IConfigurationManager configurationManager)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException("configurationManager");
            }

            _configurationManager = configurationManager;

            _xPoweredBy = MessageProvider.FormatString(Message.ProductName, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            // Initialise the action parser factory with all the standard actions.
            _actionParserFactory = new ActionParserFactory();
            _actionParserFactory.Add(new IfConditionActionParser());
            _actionParserFactory.Add(new UnlessConditionActionParser());
            _actionParserFactory.Add(new AddHeaderActionParser());
            _actionParserFactory.Add(new SetCookieActionParser());
            _actionParserFactory.Add(new SetPropertyActionParser());
            _actionParserFactory.Add(new SetAppSettingPropertyActionParser());
            _actionParserFactory.Add(new RewriteActionParser());
            _actionParserFactory.Add(new RedirectActionParser());
            _actionParserFactory.Add(new SetStatusActionParser());
            _actionParserFactory.Add(new ForbiddenActionParser());
            _actionParserFactory.Add(new GoneActionParser());
            _actionParserFactory.Add(new NotAllowedActionParser());
            _actionParserFactory.Add(new NotFoundActionParser());
            _actionParserFactory.Add(new NotImplementedActionParser());

            // Initialise the condition parser pipeline with our standard conditions.
            _conditionParserPipeline = new ConditionParserPipeline();
            _conditionParserPipeline.Add(new AddressConditionParser());
            _conditionParserPipeline.Add(new HeaderMatchConditionParser());
            _conditionParserPipeline.Add(new MethodConditionParser());
            _conditionParserPipeline.Add(new PropertyMatchConditionParser());
            _conditionParserPipeline.Add(new ExistsConditionParser());
            _conditionParserPipeline.Add(new UrlMatchConditionParser());

            // Initialise the transform factory with our standard transforms.
            _transformFactory = new TransformFactory();
            _transformFactory.Add(new DecodeTransform());
            _transformFactory.Add(new EncodeTransform());
            _transformFactory.Add(new LowerTransform());
            _transformFactory.Add(new UpperTransform());
            _transformFactory.Add(new Base64Transform());
            _transformFactory.Add(new Base64DecodeTransform());

            // The default documents collection is initially empty.
            // Should we read this from IIS config?
            _defaultDocuments = new StringCollection();

            // Load the rewriter configuration from the rules as specified in the web.config file.
            LoadFromConfig();
        }

        /// <summary>
        /// The rules.
        /// </summary>
        public IList<IRewriteAction> Rules
        {
            get { return _rules; }
        }

        /// <summary>
        /// The action parser factory.
        /// </summary>
        public ActionParserFactory ActionParserFactory
        {
            get { return _actionParserFactory; }
        }

        /// <summary>
        /// The transform factory.
        /// </summary>
        public TransformFactory TransformFactory
        {
            get { return _transformFactory; }
        }

        /// <summary>
        /// The condition parser pipeline.
        /// </summary>
        public ConditionParserPipeline ConditionParserPipeline
        {
            get { return _conditionParserPipeline; }
        }

        /// <summary>
        /// Dictionary of error handlers.
        /// </summary>
        public IDictionary<int, IRewriteErrorHandler> ErrorHandlers
        {
            get { return _errorHandlers; }
        }

        /// <summary>
        /// Logger to use for logging information.
        /// </summary>
        public IRewriteLogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        /// <summary>
        /// Collection of default document names to use if the result of a rewriting
        /// is a directory name.
        /// </summary>
        public StringCollection DefaultDocuments
        {
            get { return _defaultDocuments; }
        }

        /// <summary>
        /// Additional X-Powered-By header.
        /// </summary>
        public string XPoweredBy
        {
            get { return _xPoweredBy; }
        }

        /// <summary>
        /// The configuration manager instance.
        /// </summary>
        public IConfigurationManager ConfigurationManager
        {
            get { return _configurationManager; }
        }

        /// <summary>
        /// Loads the rewriter configuration from the web.config file.
        /// </summary>
        private void LoadFromConfig()
        {
            XmlNode section = _configurationManager.GetSection(Constants.RewriterNode) as XmlNode;
            if (section == null)
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.MissingConfigFileSection, Constants.RewriterNode), section);
            }

            RewriterConfigurationReader.Read(this, section);
        }

        private IConfigurationManager _configurationManager;
        private IRewriteLogger _logger = new NullLogger();
        private IDictionary<int, IRewriteErrorHandler> _errorHandlers = new Dictionary<int, IRewriteErrorHandler>();
        private IList<IRewriteAction> _rules = new List<IRewriteAction>();
        private ActionParserFactory _actionParserFactory;
        private ConditionParserPipeline _conditionParserPipeline;
        private TransformFactory _transformFactory;
        private StringCollection _defaultDocuments;
        private string _xPoweredBy;
    }
}
