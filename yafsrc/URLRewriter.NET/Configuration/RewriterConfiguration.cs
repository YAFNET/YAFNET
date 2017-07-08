// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Xml;

using Intelligencia.UrlRewriter.Logging;
using Intelligencia.UrlRewriter.Parsers;
using Intelligencia.UrlRewriter.Transforms;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Configuration
{
	/// <summary>
	/// Configuration for the URL rewriter.
	/// </summary>
	public class RewriterConfiguration
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		internal RewriterConfiguration()
		{
		    this._xPoweredBy = MessageProvider.FormatString(Message.ProductName, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

		    this._actionParserFactory = new ActionParserFactory();
		    this._actionParserFactory.AddParser(new IfConditionActionParser());
		    this._actionParserFactory.AddParser(new UnlessConditionActionParser());
		    this._actionParserFactory.AddParser(new AddHeaderActionParser());
		    this._actionParserFactory.AddParser(new SetCookieActionParser());
		    this._actionParserFactory.AddParser(new SetPropertyActionParser());
		    this._actionParserFactory.AddParser(new RewriteActionParser());
		    this._actionParserFactory.AddParser(new RedirectActionParser());
		    this._actionParserFactory.AddParser(new SetStatusActionParser());
		    this._actionParserFactory.AddParser(new ForbiddenActionParser());
		    this._actionParserFactory.AddParser(new GoneActionParser());
		    this._actionParserFactory.AddParser(new NotAllowedActionParser());
		    this._actionParserFactory.AddParser(new NotFoundActionParser());
		    this._actionParserFactory.AddParser(new NotImplementedActionParser());

		    this._conditionParserPipeline = new ConditionParserPipeline();
		    this._conditionParserPipeline.AddParser(new AddressConditionParser());
		    this._conditionParserPipeline.AddParser(new HeaderMatchConditionParser());
		    this._conditionParserPipeline.AddParser(new MethodConditionParser());
		    this._conditionParserPipeline.AddParser(new PropertyMatchConditionParser());
		    this._conditionParserPipeline.AddParser(new ExistsConditionParser());
		    this._conditionParserPipeline.AddParser(new UrlMatchConditionParser());

		    this._transformFactory = new TransformFactory();
		    this._transformFactory.AddTransform(new DecodeTransform());
		    this._transformFactory.AddTransform(new EncodeTransform());
		    this._transformFactory.AddTransform(new LowerTransform());
		    this._transformFactory.AddTransform(new UpperTransform());
		    this._transformFactory.AddTransform(new Base64Transform());
		    this._transformFactory.AddTransform(new Base64DecodeTransform());

		    this._defaultDocuments = new StringCollection();
		}

		/// <summary>
		/// The rules.
		/// </summary>
		public IList Rules => this._rules;

	    /// <summary>
		/// The action parser factory.
		/// </summary>
		public ActionParserFactory ActionParserFactory => this._actionParserFactory;

	    /// <summary>
		/// The transform factory.
		/// </summary>
		public TransformFactory TransformFactory => this._transformFactory;

	    /// <summary>
		/// The condition parser pipeline.
		/// </summary>
		public ConditionParserPipeline ConditionParserPipeline => this._conditionParserPipeline;

	    /// <summary>
		/// Dictionary of error handlers.
		/// </summary>
		public IDictionary ErrorHandlers => this._errorHandlers;

	    /// <summary>
		/// Logger to use for logging information.
		/// </summary>
		public IRewriteLogger Logger
		{
			get => this._logger;
	        set => this._logger = value;
	    }

		/// <summary>
		/// Collection of default document names to use if the result of a rewriting
		/// is a directory name.
		/// </summary>
		public StringCollection DefaultDocuments => this._defaultDocuments;

	    internal string XPoweredBy => this._xPoweredBy;

	    /// <summary>
		/// Creates a new configuration with only the default entries.
		/// </summary>
		/// <returns></returns>
		public static RewriterConfiguration Create()
		{
			return new RewriterConfiguration();
		}

		/// <summary>
		/// The current configuration.
		/// </summary>
		public static RewriterConfiguration Current
		{
			get
			{
				var configuration = HttpRuntime.Cache.Get(_cacheName) as RewriterConfiguration;
				if (configuration == null)
				{
					lock (SyncObject)
					{
						configuration = HttpRuntime.Cache.Get(_cacheName) as RewriterConfiguration;
						if (configuration == null)
						{
							configuration = Load();
						}
					}
				}

				return configuration;
			}
		}

		private static object SyncObject = new Object();

		/// <summary>
		/// Loads the configuration from the .config file, with caching.
		/// </summary>
		/// <returns>The configuration.</returns>
		public static RewriterConfiguration Load()
		{
			var section = ConfigurationManager.GetSection(Constants.RewriterNode) as XmlNode;
			RewriterConfiguration config = null;

			var filenameNode = section.Attributes.GetNamedItem(Constants.AttrFile);
			if (filenameNode != null)
			{
				var filename = HttpContext.Current.Server.MapPath(filenameNode.Value);
				config = LoadFromFile(filename);
				if (config != null)
				{
					var fileDependency = new CacheDependency(filename);
					HttpRuntime.Cache.Add(_cacheName, config, fileDependency, DateTime.UtcNow.AddHours(1.0), TimeSpan.Zero, CacheItemPriority.Default, null);
				}
			}

			if (config == null)
			{
				config = LoadFromNode(section);
				HttpRuntime.Cache.Add(_cacheName, config, null, DateTime.UtcNow.AddHours(1.0), TimeSpan.Zero, CacheItemPriority.Default, null);
			}

			return config;
		}

		/// <summary>
		/// Loads the configuration from an external XML file.
		/// </summary>
		/// <param name="filename">The filename of the file to load configuration from.</param>
		/// <returns>The configuration.</returns>
		public static RewriterConfiguration LoadFromFile(string filename)
		{
			if (File.Exists(filename))
			{
				var document = new XmlDocument();
				document.Load(filename);

				return LoadFromNode(document.DocumentElement);
			}

			return null;
		}

		/// <summary>
		/// Loads the configuration from an XML node.
		/// </summary>
		/// <param name="node">The XML node to load configuration from.</param>
		/// <returns>The configuration.</returns>
		public static RewriterConfiguration LoadFromNode(XmlNode node)
		{
            return (RewriterConfiguration)RewriterConfigurationReader.Read(node);
		}

		private IRewriteLogger _logger = new NullLogger();
		private Hashtable _errorHandlers = new Hashtable();
		private ArrayList _rules = new ArrayList();
		private ActionParserFactory _actionParserFactory;
		private ConditionParserPipeline _conditionParserPipeline;
		private TransformFactory _transformFactory;
		private StringCollection _defaultDocuments;
		private string _xPoweredBy;
		private static string _cacheName = typeof(RewriterConfiguration).AssemblyQualifiedName;
	}
}
