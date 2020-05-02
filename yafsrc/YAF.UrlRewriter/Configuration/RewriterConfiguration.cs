// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Reflection;
    using System.Xml;

    using YAF.UrlRewriter.Actions;
    using YAF.UrlRewriter.Errors;
    using YAF.UrlRewriter.Logging;
    using YAF.UrlRewriter.Parsers;
    using YAF.UrlRewriter.Transforms;
    using YAF.UrlRewriter.Utilities;

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
            this.ConfigurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));

            this.XPoweredBy = MessageProvider.FormatString(Message.ProductName, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            // Initialise the action parser factory with all the standard actions.
            this.ActionParserFactory = new ActionParserFactory();
            this.ActionParserFactory.Add(new IfConditionActionParser());
            this.ActionParserFactory.Add(new UnlessConditionActionParser());
            this.ActionParserFactory.Add(new AddHeaderActionParser());
            this.ActionParserFactory.Add(new SetCookieActionParser());
            this.ActionParserFactory.Add(new SetPropertyActionParser());
            this.ActionParserFactory.Add(new SetAppSettingPropertyActionParser());
            this.ActionParserFactory.Add(new RewriteActionParser());
            this.ActionParserFactory.Add(new RedirectActionParser());
            this.ActionParserFactory.Add(new SetStatusActionParser());
            this.ActionParserFactory.Add(new ForbiddenActionParser());
            this.ActionParserFactory.Add(new GoneActionParser());
            this.ActionParserFactory.Add(new NotAllowedActionParser());
            this.ActionParserFactory.Add(new NotFoundActionParser());
            this.ActionParserFactory.Add(new NotImplementedActionParser());

            // Initialise the condition parser pipeline with our standard conditions.
            this.ConditionParserPipeline = new ConditionParserPipeline
                                               {
                                                   new AddressConditionParser(),
                                                   new HeaderMatchConditionParser(),
                                                   new MethodConditionParser(),
                                                   new PropertyMatchConditionParser(),
                                                   new ExistsConditionParser(),
                                                   new UrlMatchConditionParser()
                                               };

            // Initialise the transform factory with our standard transforms.
            this.TransformFactory = new TransformFactory();
            this.TransformFactory.Add(new DecodeTransform());
            this.TransformFactory.Add(new EncodeTransform());
            this.TransformFactory.Add(new LowerTransform());
            this.TransformFactory.Add(new UpperTransform());
            this.TransformFactory.Add(new Base64Transform());
            this.TransformFactory.Add(new Base64DecodeTransform());

            // The default documents collection is initially empty.
            // Should we read this from IIS config?
            this.DefaultDocuments = new StringCollection();

            // Load the rewriter configuration from the rules as specified in the web.config file.
            this.LoadFromConfig();
        }

        /// <summary>
        /// The rules.
        /// </summary>
        public IList<IRewriteAction> Rules { get; } = new List<IRewriteAction>();

        /// <summary>
        /// The action parser factory.
        /// </summary>
        public ActionParserFactory ActionParserFactory { get; }

        /// <summary>
        /// The transform factory.
        /// </summary>
        public TransformFactory TransformFactory { get; }

        /// <summary>
        /// The condition parser pipeline.
        /// </summary>
        public ConditionParserPipeline ConditionParserPipeline { get; }

        /// <summary>
        /// Dictionary of error handlers.
        /// </summary>
        public IDictionary<int, IRewriteErrorHandler> ErrorHandlers { get; } = new Dictionary<int, IRewriteErrorHandler>();

        /// <summary>
        /// Logger to use for logging information.
        /// </summary>
        public IRewriteLogger Logger { get; set; } = new NullLogger();

        /// <summary>
        /// Collection of default document names to use if the result of a rewriting
        /// is a directory name.
        /// </summary>
        public StringCollection DefaultDocuments { get; }

        /// <summary>
        /// Additional X-Powered-By header.
        /// </summary>
        public string XPoweredBy { get; }

        /// <summary>
        /// The configuration manager instance.
        /// </summary>
        public IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Loads the rewriter configuration from the web.config file.
        /// </summary>
        private void LoadFromConfig()
        {
            var section = this.ConfigurationManager.GetSection(Constants.RewriterNode) as XmlNode;
            if (section == null)
            {
                throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.MissingConfigFileSection, Constants.RewriterNode), (XmlNode)null);
            }

            RewriterConfigurationReader.Read(this, section);
        }
    }
}
