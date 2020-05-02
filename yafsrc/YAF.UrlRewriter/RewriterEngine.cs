// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="RewriterEngine.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace YAF.UrlRewriter
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;
    using YAF.Types.Extensions;
    using YAF.UrlRewriter.Actions;
    using YAF.UrlRewriter.Conditions;
    using YAF.UrlRewriter.Configuration;
    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// The core Rewriter Engine class.
    /// </summary>
    public class RewriterEngine
    {
        private const char EndChar = (char)65535; // The 'end' char for StringReader.Read() (i.e. -1 as a char).
        private const int MaxRestarts = 10; // Controls the number of restarts so we don't get into an infinite loop

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpContext">The HTTP context facade.</param>
        /// <param name="configurationManager">The configuration manager facade.</param>
        /// <param name="configuration">The URL rewriter configuration.</param>
        public RewriterEngine(
            IHttpContext httpContext,
            IConfigurationManager configurationManager,
            IRewriterConfiguration configuration)
        {
            this._httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            this._configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Resolves an Application-path relative location
        /// </summary>
        /// <param name="location">The location</param>
        /// <returns>The absolute location.</returns>
        public string ResolveLocation(string location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var appPath = this._httpContext.ApplicationPath;
            if (appPath.Length > 1)
            {
                appPath += "/";
            }

            return location.Replace("~/", appPath);
        }

        /// <summary>
        /// Performs the rewriting.
        /// </summary>
        public void Rewrite()
        {
            var originalUrl = this._httpContext.RequestUrl.PathAndQuery.Replace("+", " ");
            this.RawUrl = originalUrl;

            this._configuration.Logger.Debug(MessageProvider.FormatString(Message.StartedProcessing, originalUrl));

            // Create the context
            IRewriteContext context = new RewriteContext(
                this,
                originalUrl,
                this._httpContext,
                this._configurationManager);

            // Process each rule.
            this.ProcessRules(context);

            // Append any headers defined.
            this.AppendHeaders(context);

            // Append any cookies defined.
            this.AppendCookies(context);

            // Rewrite the path if the location has changed.
            this._httpContext.SetStatusCode(context.StatusCode);
            if (context.Location != originalUrl && (int)context.StatusCode < 400)
            {
                if ((int)context.StatusCode < 300)
                {
                    // Successful status if less than 300
                    this._configuration.Logger.Info(
                        MessageProvider.FormatString(
                            Message.RewritingXtoY,
                            this._httpContext.RawUrl,
                            context.Location));

                    // To verify that the URL exists on this server:
                    // VerifyResultExists(context);

                    // To ensure that directories are rewritten to their default document:
                    // HandleDefaultDocument(context);
                    this._httpContext.RewritePath(context.Location);
                }
                else
                {
                    // Redirection
                    this._configuration.Logger.Info(
                        MessageProvider.FormatString(
                            Message.RedirectingXtoY,
                            this._httpContext.RawUrl,
                            context.Location));

                    this._httpContext.SetRedirectLocation(context.Location);
                }
            }
            else if ((int)context.StatusCode >= 400)
            {
                this.HandleError(context);
            }

            // To ensure that directories are rewritten to their default document:
            // else if (HandleDefaultDocument(context))
            // {
            // _contextFacade.RewritePath(context.Location);
            // }

            // Sets the context items.
            this.SetContextItems(context);
        }

        /// <summary>
        /// Expands the given input based on the current context.
        /// </summary>
        /// <param name="context">The current context</param>
        /// <param name="input">The input to expand.</param>
        /// <returns>The expanded input</returns>
        public string Expand(IRewriteContext context, string input)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            /* replacement :- $n
                         * |	${[a-zA-Z0-9\-]+}
                         * |	${fn( <replacement> )}
                         * |	${<replacement-or-id>:<replacement-or-value>:<replacement-or-value>}
                         * 
                         * replacement-or-id :- <replacement> | <id>
                         * replacement-or-value :- <replacement> | <value>
                         */

            /* $1 - regex replacement
             * ${propertyname}
             * ${map-name:value}				map-name is replacement, value is replacement
             * ${map-name:value|default-value}	map-name is replacement, value is replacement, default-value is replacement
             * ${fn(value)}						value is replacement
             */
            using (var reader = new StringReader(input))
            {
                using (var writer = new StringWriter())
                {
                    var ch = (char)reader.Read();
                    while (ch != EndChar)
                    {
                        if (ch == '$')
                        {
                            writer.Write(this.Reduce(context, reader));
                        }
                        else
                        {
                            writer.Write(ch);
                        }

                        ch = (char)reader.Read();
                    }

                    return writer.GetStringBuilder().ToString();
                }
            }
        }

        private void ProcessRules(IRewriteContext context)
        {
            this.ProcessRules(context, this._configuration.Rules, 0);
        }

        private void ProcessRules(IRewriteContext context, IList<IRewriteAction> rewriteRules, int restarts)
        {
            foreach (var action in rewriteRules)
            {
                // If the rule is conditional, ensure the conditions are met.
                if (action is IRewriteCondition condition && !condition.IsMatch(context))
                {
                    continue;
                }

                // Execute the action.
                var processing = action.Execute(context);

                // If the action is Stop, then break out of the processing loop
                if (processing == RewriteProcessing.StopProcessing)
                {
                    this._configuration.Logger.Debug(MessageProvider.FormatString(Message.StoppingBecauseOfRule));

                    // Exit the loop.
                    break;
                }

                // If the action is Restart, then start again.
                if (processing == RewriteProcessing.RestartProcessing)
                {
                    this._configuration.Logger.Debug(MessageProvider.FormatString(Message.RestartingBecauseOfRule));

                    // Increment the number of restarts and check that we have not exceeded our max.
                    restarts++;
                    if (restarts > MaxRestarts)
                    {
                        throw new InvalidOperationException(MessageProvider.FormatString(Message.TooManyRestarts));
                    }

                    // Restart again from the first rule by calling this method recursively.
                    this.ProcessRules(context, rewriteRules, restarts);

                    // Exit the loop.
                    break;
                }
            }
        }

        private bool HandleDefaultDocument(IRewriteContext context)
        {
            var uri = new Uri(this._httpContext.RequestUrl, context.Location);
            var b = new UriBuilder(uri);
            b.Path += "/";
            uri = b.Uri;
            
            if (uri.Host != this._httpContext.RequestUrl.Host)
            {
                return false;
            }
            
            var filename = this._httpContext.MapPath(uri.AbsolutePath);

            if (!Directory.Exists(filename))
            {
                return false;
            }

            foreach (var document in this._configuration.DefaultDocuments)
            {
                var pathName = Path.Combine(filename, document);

                if (!File.Exists(pathName))
                {
                    continue;
                }

                context.Location = new Uri(uri, document).AbsolutePath;
                return true;
            }

            return false;
        }

        private void HandleError(IRewriteContext context)
        {
            // Return the status code.
            this._httpContext.SetStatusCode(context.StatusCode);

            // Get the error handler if there is one.
            if (!this._configuration.ErrorHandlers.ContainsKey((int)context.StatusCode))
            {
                // No error handler for this status code?
                // Just throw an HttpException with the appropriate status code.
                throw new HttpException((int)context.StatusCode, context.StatusCode.ToString());
            }

            var handler = this._configuration.ErrorHandlers[(int) context.StatusCode];

            try
            {
                this._configuration.Logger.Debug(MessageProvider.FormatString(Message.CallingErrorHandler));

                // Execute the error handler.
                this._httpContext.HandleError(handler);
            }
            catch (HttpException)
            {
                // Any HTTP errors that result from executing the error page should be propogated.
                throw;
            }
            catch (Exception exc)
            {
                // Any other error should result in a 500 Internal Server Error.
                this._configuration.Logger.Error(exc.Message, exc);

                var serverError = HttpStatusCode.InternalServerError;
                throw new HttpException((int)serverError, serverError.ToString());
            }
        }

        private void AppendHeaders(IRewriteContext context)
        {
            foreach (string headerKey in context.ResponseHeaders)
            {
                this._httpContext.SetResponseHeader(headerKey, context.ResponseHeaders[headerKey]);
            }
        }

        private void AppendCookies(IRewriteContext context)
        {
            for (var i = 0; i < context.ResponseCookies.Count; i++)
            {
                this._httpContext.SetResponseCookie(context.ResponseCookies[i]);
            }
        }

        private void SetContextItems(IRewriteContext context)
        {
            this.OriginalQueryString = new Uri(this._httpContext.RequestUrl, this._httpContext.RawUrl).Query.Replace("?", string.Empty);
            this.QueryString = new Uri(this._httpContext.RequestUrl, context.Location).Query.Replace("?", string.Empty);

            // Add in the properties as context items, so these will be accessible to the handler
            foreach (string propertyKey in context.Properties.Keys)
            {
                var itemsKey = $"Rewriter.{propertyKey}";
                var itemsValue = context.Properties[propertyKey];

                this._httpContext.Items[itemsKey] = itemsValue;
            }
        }

        /// <summary>
        /// The raw URL.
        /// </summary>
        public string RawUrl
        {
            get => (string)this._httpContext.Items[ContextRawUrl];
            set => this._httpContext.Items[ContextRawUrl] = value;
        }

        /// <summary>
        /// The original query string.
        /// </summary>
        public string OriginalQueryString
        {
            get => (string)this._httpContext.Items[ContextOriginalQueryString];
            set => this._httpContext.Items[ContextOriginalQueryString] = value;
        }

        /// <summary>
        /// The final querystring, after rewriting.
        /// </summary>
        public string QueryString
        {
            get => (string)this._httpContext.Items[ContextQueryString];
            set => this._httpContext.Items[ContextQueryString] = value;
        }

        private string Reduce(IRewriteContext context, TextReader reader)
        {
            string result;
            var ch = (char)reader.Read();
            if (char.IsDigit(ch))
            {
                var num = ch.ToString();
                if (char.IsDigit((char)reader.Peek()))
                {
                    ch = (char)reader.Read();
                    num += ch.ToString();
                }

                if (context.LastMatch != null)
                {
                    var group = context.LastMatch.Groups[num.ToType<int>()];
                    result = group.Value;
                }
                else
                {
                    result = string.Empty;
                }
            }
            else if (ch == '<')
            {
                string expr;

                using (var writer = new StringWriter())
                {
                    ch = (char)reader.Read();
                    while (ch != '>' && ch != EndChar)
                    {
                        if (ch == '$')
                        {
                            writer.Write(this.Reduce(context, reader));
                        }
                        else
                        {
                            writer.Write(ch);
                        }

                        ch = (char)reader.Read();
                    }

                    expr = writer.GetStringBuilder().ToString();
                }

                if (context.LastMatch != null)
                {
                    var group = context.LastMatch.Groups[expr];
                    result = group == null ? string.Empty : group.Value;
                }
                else
                {
                    result = string.Empty;
                }
            }
            else if (ch == '{')
            {
                string expr;
                var isMap = false;
                var isFunction = false;

                using (var writer = new StringWriter())
                {
                    ch = (char)reader.Read();
                    while (ch != '}' && ch != EndChar)
                    {
                        if (ch == '$')
                        {
                            writer.Write(this.Reduce(context, reader));
                        }
                        else
                        {
                            switch (ch)
                            {
                                case ':':
                                    isMap = true;
                                    break;
                                case '(':
                                    isFunction = true;
                                    break;
                            }

                            writer.Write(ch);
                        }

                        ch = (char)reader.Read();
                    }

                    expr = writer.GetStringBuilder().ToString();
                }

                if (isMap)
                {
                    var match = Regex.Match(expr, @"^([^\:]+)\:([^\|]+)(\|(.+))?$");
                    var mapName = match.Groups[1].Value;
                    var mapArgument = match.Groups[2].Value;
                    var mapDefault = match.Groups[4].Value;

                    var tx = this._configuration.TransformFactory.GetTransform(mapName);
                    if (tx == null)
                    {
                        throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.MappingNotFound, mapName));
                    }

                    result = tx.ApplyTransform(mapArgument) ?? mapDefault;
                }
                else if (isFunction)
                {
                    var match = Regex.Match(expr, @"^([^\(]+)\((.+)\)$");
                    var functionName = match.Groups[1].Value;
                    var functionArgument = match.Groups[2].Value;

                    var tx = this._configuration.TransformFactory.GetTransform(functionName);
                    if (tx == null)
                    {
                        throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.TransformFunctionNotFound, functionName));
                    }

                    result = tx.ApplyTransform(functionArgument);
                }
                else
                {
                    result = context.Properties[expr];
                }
            }
            else
            {
                result = ch.ToString();
            }

            return result;
        }

        private const string ContextQueryString = "UrlRewriter.NET.QueryString";
        private const string ContextOriginalQueryString = "UrlRewriter.NET.OriginalQueryString";
        private const string ContextRawUrl = "UrlRewriter.NET.RawUrl";
        private readonly IRewriterConfiguration _configuration;
        private readonly IHttpContext _httpContext;
        private readonly IConfigurationManager _configurationManager;
    }
}
