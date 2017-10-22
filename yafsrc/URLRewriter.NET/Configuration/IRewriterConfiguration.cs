// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Intelligencia.UrlRewriter.Logging;

namespace Intelligencia.UrlRewriter.Configuration
{
    /// <summary>
    /// Interface for the rewriter's configuration.
    /// Useful for plugging out config in unit tests.
    /// </summary>
    public interface IRewriterConfiguration
    {
        /// <summary>
        /// The rules.
        /// </summary>
        IList<IRewriteAction> Rules { get; }

        /// <summary>
        /// The action parser factory.
        /// </summary>
        ActionParserFactory ActionParserFactory { get; }

        /// <summary>
        /// The transform factory.
        /// </summary>
        TransformFactory TransformFactory { get; }

        /// <summary>
        /// The condition parser pipeline.
        /// </summary>
        ConditionParserPipeline ConditionParserPipeline { get; }

        /// <summary>
        /// Dictionary of error handlers.
        /// </summary>
        IDictionary<int, IRewriteErrorHandler> ErrorHandlers { get; }

        /// <summary>
        /// Logger to use for logging information.
        /// </summary>
        IRewriteLogger Logger { get; set; }

        /// <summary>
        /// Collection of default document names to use if the result of a rewriting
        /// is a directory name.
        /// </summary>
        StringCollection DefaultDocuments { get; }

        /// <summary>
        /// Additional X-Powered-By header.
        /// </summary>
        string XPoweredBy { get; }

        /// <summary>
        /// The configuration manager instance.
        /// </summary>
        IConfigurationManager ConfigurationManager { get; }
    }
}