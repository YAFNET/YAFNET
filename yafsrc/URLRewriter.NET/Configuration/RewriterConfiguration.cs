// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.Caching;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
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
	public class RewriterConfiguration
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		internal RewriterConfiguration()
		{
			_xPoweredBy = MessageProvider.FormatString(Message.ProductName, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

			_actionParserFactory = new ActionParserFactory();
			_actionParserFactory.AddParser(new IfConditionActionParser());
            _actionParserFactory.AddParser(new UnlessConditionActionParser());
			_actionParserFactory.AddParser(new AddHeaderActionParser());
			_actionParserFactory.AddParser(new SetCookieActionParser());
			_actionParserFactory.AddParser(new SetPropertyActionParser());
			_actionParserFactory.AddParser(new RewriteActionParser());
			_actionParserFactory.AddParser(new RedirectActionParser());
			_actionParserFactory.AddParser(new SetStatusActionParser());
			_actionParserFactory.AddParser(new ForbiddenActionParser());
			_actionParserFactory.AddParser(new GoneActionParser());
			_actionParserFactory.AddParser(new NotAllowedActionParser());
			_actionParserFactory.AddParser(new NotFoundActionParser());
			_actionParserFactory.AddParser(new NotImplementedActionParser());

			_conditionParserPipeline = new ConditionParserPipeline();
			_conditionParserPipeline.AddParser(new AddressConditionParser());
			_conditionParserPipeline.AddParser(new HeaderMatchConditionParser());
			_conditionParserPipeline.AddParser(new MethodConditionParser());
			_conditionParserPipeline.AddParser(new PropertyMatchConditionParser());
			_conditionParserPipeline.AddParser(new ExistsConditionParser());
			_conditionParserPipeline.AddParser(new UrlMatchConditionParser());

			_transformFactory = new TransformFactory();
			_transformFactory.AddTransform(new DecodeTransform());
			_transformFactory.AddTransform(new EncodeTransform());
			_transformFactory.AddTransform(new LowerTransform());
			_transformFactory.AddTransform(new UpperTransform());
			_transformFactory.AddTransform(new Base64Transform());
			_transformFactory.AddTransform(new Base64DecodeTransform());

			_defaultDocuments = new StringCollection();
		}

		/// <summary>
		/// The rules.
		/// </summary>
		public IList Rules
		{
			get
			{
				return _rules;
			}
		}

		/// <summary>
		/// The action parser factory.
		/// </summary>
		public ActionParserFactory ActionParserFactory
		{
			get
			{
				return _actionParserFactory;
			}
		}

		/// <summary>
		/// The transform factory.
		/// </summary>
		public TransformFactory TransformFactory
		{
			get
			{
				return _transformFactory;
			}
		}

		/// <summary>
		/// The condition parser pipeline.
		/// </summary>
		public ConditionParserPipeline ConditionParserPipeline
		{
			get
			{
				return _conditionParserPipeline;
			}
		}

		/// <summary>
		/// Dictionary of error handlers.
		/// </summary>
		public IDictionary ErrorHandlers
		{
			get
			{
				return _errorHandlers;
			}
		}

		/// <summary>
		/// Logger to use for logging information.
		/// </summary>
		public IRewriteLogger Logger
		{
			get
			{
				return _logger;
			}
			set
			{
				_logger = value;
			}
		}

		/// <summary>
		/// Collection of default document names to use if the result of a rewriting
		/// is a directory name.
		/// </summary>
		public StringCollection DefaultDocuments
		{
			get
			{
				return _defaultDocuments;
			}
		}

		internal string XPoweredBy
		{
			get
			{
				return _xPoweredBy;
			}
		}

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
				RewriterConfiguration configuration = HttpRuntime.Cache.Get(_cacheName) as RewriterConfiguration;
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
			XmlNode section = ConfigurationManager.GetSection(Constants.RewriterNode) as XmlNode;
			RewriterConfiguration config = null;

			XmlNode filenameNode = section.Attributes.GetNamedItem(Constants.AttrFile);
			if (filenameNode != null)
			{
				string filename = HttpContext.Current.Server.MapPath(filenameNode.Value);
				config = LoadFromFile(filename);
				if (config != null)
				{
					CacheDependency fileDependency = new CacheDependency(filename);
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
				XmlDocument document = new XmlDocument();
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
