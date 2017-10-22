// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
    /// <summary>
    /// The core RewriterEngine class.
    /// </summary>
    public class RewriterEngine
    {
        private const char EndChar = (char)65535; // The 'end' char for StringReader.Read() (i.e. -1 as a char).
        private const int MaxRestarts = 10; // Controls the number of restarts so we don't get into an infinite loop

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpContext">The HTTP context facade.</param>
        /// <param param name="configurationManager">The configuration manager facade.</param>
        /// <param name="configuration">The URL rewriter configuration.</param>
        public RewriterEngine(
            IHttpContext httpContext,
            IConfigurationManager configurationManager,
            IRewriterConfiguration configuration)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            if (configurationManager == null)
            {
                throw new ArgumentNullException("configurationManager");
            }
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            _httpContext = httpContext;
            _configurationManager = configurationManager;
            _configuration = configuration;
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
                throw new ArgumentNullException("location");
            }

            string appPath = _httpContext.ApplicationPath;
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
            string originalUrl = _httpContext.RawUrl.Replace("+", " ");
            RawUrl = originalUrl;

            _configuration.Logger.Debug(MessageProvider.FormatString(Message.StartedProcessing, originalUrl));

            // Create the context
            IRewriteContext context = new RewriteContext(this, originalUrl, _httpContext, _configurationManager);

            // Process each rule.
            ProcessRules(context);

            // Append any headers defined.
            AppendHeaders(context);

            // Append any cookies defined.
            AppendCookies(context);

            // Rewrite the path if the location has changed.
            _httpContext.SetStatusCode(context.StatusCode);
            if ((context.Location != originalUrl) && ((int)context.StatusCode < 400))
            {
                if ((int)context.StatusCode < 300)
                {
                    // Successful status if less than 300
                    _configuration.Logger.Info(MessageProvider.FormatString(Message.RewritingXtoY, _httpContext.RawUrl, context.Location));

                    // To verify that the URL exists on this server:
                    //  VerifyResultExists(context);

                    // To ensure that directories are rewritten to their default document:
                    //  HandleDefaultDocument(context);

                    _httpContext.RewritePath(context.Location);
                }
                else
                {
                    // Redirection
                    _configuration.Logger.Info(MessageProvider.FormatString(Message.RedirectingXtoY, _httpContext.RawUrl, context.Location));

                    _httpContext.SetRedirectLocation(context.Location);
                }
            }
            else if ((int)context.StatusCode >= 400)
            {
                HandleError(context);
            }
            // To ensure that directories are rewritten to their default document:
            //  else if (HandleDefaultDocument(context))
            //  {
            //      _contextFacade.RewritePath(context.Location);
            //  }

            // Sets the context items.
            SetContextItems(context);
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
                throw new ArgumentNullException("context");
            }
            if (input == null)
            {
                throw new ArgumentNullException("input");
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

            using (StringReader reader = new StringReader(input))
            {
                using (StringWriter writer = new StringWriter())
                {
                    char ch = (char)reader.Read();
                    while (ch != EndChar)
                    {
                        if (ch == '$')
                        {
                            writer.Write(Reduce(context, reader));
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
            ProcessRules(context, _configuration.Rules, 0);
        }

        private void ProcessRules(IRewriteContext context, IList<IRewriteAction> rewriteRules, int restarts)
        {
            foreach (IRewriteAction action in rewriteRules)
            {
                // If the rule is conditional, ensure the conditions are met.
                IRewriteCondition condition = action as IRewriteCondition;
                if (condition == null || condition.IsMatch(context))
                {
                    // Execute the action.
                    RewriteProcessing processing = action.Execute(context);

                    // If the action is Stop, then break out of the processing loop
                    if (processing == RewriteProcessing.StopProcessing)
                    {
                        _configuration.Logger.Debug(MessageProvider.FormatString(Message.StoppingBecauseOfRule));

                        // Exit the loop.
                        break;
                    }

                    // If the action is Restart, then start again.
                    if (processing == RewriteProcessing.RestartProcessing)
                    {
                        _configuration.Logger.Debug(MessageProvider.FormatString(Message.RestartingBecauseOfRule));

                        // Increment the number of restarts and check that we have not exceeded our max.
                        restarts++;
                        if (restarts > MaxRestarts)
                        {
                            throw new InvalidOperationException(MessageProvider.FormatString(Message.TooManyRestarts));
                        }

                        // Restart again from the first rule by calling this method recursively.
                        ProcessRules(context, rewriteRules, restarts);

                        // Exit the loop.
                        break;
                    }
                }
            }
        }

        private bool HandleDefaultDocument(IRewriteContext context)
        {
            Uri uri = new Uri(_httpContext.RequestUrl, context.Location);
            UriBuilder b = new UriBuilder(uri);
            b.Path += "/";
            uri = b.Uri;
            if (uri.Host == _httpContext.RequestUrl.Host)
            {
                string filename = _httpContext.MapPath(uri.AbsolutePath);
                if (Directory.Exists(filename))
                {
                    foreach (string document in _configuration.DefaultDocuments)
                    {
                        string pathName = Path.Combine(filename, document);
                        if (File.Exists(pathName))
                        {
                            context.Location = new Uri(uri, document).AbsolutePath;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void VerifyResultExists(IRewriteContext context)
        {
            if ((String.Compare(context.Location, _httpContext.RawUrl) != 0) && ((int)context.StatusCode < 300))
            {
                Uri uri = new Uri(_httpContext.RequestUrl, context.Location);
                if (uri.Host == _httpContext.RequestUrl.Host)
                {
                    string filename = _httpContext.MapPath(uri.AbsolutePath);
                    if (!File.Exists(filename))
                    {
                        _configuration.Logger.Debug(MessageProvider.FormatString(Message.ResultNotFound, filename));
                        context.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        HandleDefaultDocument(context);
                    }
                }
            }
        }

        private void HandleError(IRewriteContext context)
        {
            // Return the status code.
            _httpContext.SetStatusCode(context.StatusCode);

            // Get the error handler if there is one.
            if (!_configuration.ErrorHandlers.ContainsKey((int)context.StatusCode))
            {
                // No error handler for this status code?
                // Just throw an HttpException with the appropriate status code.
                throw new HttpException((int)context.StatusCode, context.StatusCode.ToString());
            }

            IRewriteErrorHandler handler = _configuration.ErrorHandlers[(int) context.StatusCode];

            try
            {
                _configuration.Logger.Debug(MessageProvider.FormatString(Message.CallingErrorHandler));

                // Execute the error handler.
                _httpContext.HandleError(handler);
            }
            catch (HttpException)
            {
                // Any HTTP errors that result from executing the error page should be propogated.
                throw;
            }
            catch (Exception exc)
            {
                // Any other error should result in a 500 Internal Server Error.
                _configuration.Logger.Error(exc.Message, exc);

                HttpStatusCode serverError = HttpStatusCode.InternalServerError;
                throw new HttpException((int)serverError, serverError.ToString());
            }
        }

        private void AppendHeaders(IRewriteContext context)
        {
            foreach (string headerKey in context.ResponseHeaders)
            {
                _httpContext.SetResponseHeader(headerKey, context.ResponseHeaders[headerKey]);
            }
        }

        private void AppendCookies(IRewriteContext context)
        {
            for (int i = 0; i < context.ResponseCookies.Count; i++)
            {
                _httpContext.SetResponseCookie(context.ResponseCookies[i]);
            }
        }

        private void SetContextItems(IRewriteContext context)
        {
            OriginalQueryString = new Uri(_httpContext.RequestUrl, _httpContext.RawUrl).Query.Replace("?", "");
            QueryString = new Uri(_httpContext.RequestUrl, context.Location).Query.Replace("?", "");

            // Add in the properties as context items, so these will be accessible to the handler
            foreach (string propertyKey in context.Properties.Keys)
            {
                string itemsKey = String.Format("Rewriter.{0}", propertyKey);
                string itemsValue = context.Properties[propertyKey];

                _httpContext.Items[itemsKey] = itemsValue;
            }
        }

        /// <summary>
        /// The raw URL.
        /// </summary>
        public string RawUrl
        {
            get { return (string)_httpContext.Items[ContextRawUrl]; }
            set { _httpContext.Items[ContextRawUrl] = value; }
        }

        /// <summary>
        /// The original query string.
        /// </summary>
        public string OriginalQueryString
        {
            get { return (string) _httpContext.Items[ContextOriginalQueryString]; }
            set { _httpContext.Items[ContextOriginalQueryString] = value; }
        }

        /// <summary>
        /// The final querystring, after rewriting.
        /// </summary>
        public string QueryString
        {
            get { return (string)_httpContext.Items[ContextQueryString]; }
            set { _httpContext.Items[ContextQueryString] = value; }
        }

        private string Reduce(IRewriteContext context, StringReader reader)
        {
            string result;
            char ch = (char)reader.Read();
            if (Char.IsDigit(ch))
            {
                string num = ch.ToString();
                if (Char.IsDigit((char)reader.Peek()))
                {
                    ch = (char)reader.Read();
                    num += ch.ToString();
                }
                if (context.LastMatch != null)
                {
                    Group group = context.LastMatch.Groups[Convert.ToInt32(num)];
                    result = (group == null) ? String.Empty : group.Value;
                }
                else
                {
                    result = String.Empty;
                }
            }
            else if (ch == '<')
            {
                string expr;

                using (StringWriter writer = new StringWriter())
                {
                    ch = (char)reader.Read();
                    while (ch != '>' && ch != EndChar)
                    {
                        if (ch == '$')
                        {
                            writer.Write(Reduce(context, reader));
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
                    Group group = context.LastMatch.Groups[expr];
                    result = (group == null) ? String.Empty : group.Value;
                }
                else
                {
                    result = String.Empty;
                }
            }
            else if (ch == '{')
            {
                string expr;
                bool isMap = false;
                bool isFunction = false;

                using (StringWriter writer = new StringWriter())
                {
                    ch = (char)reader.Read();
                    while (ch != '}' && ch != EndChar)
                    {
                        if (ch == '$')
                        {
                            writer.Write(Reduce(context, reader));
                        }
                        else
                        {
                            if (ch == ':')
                            {
                                isMap = true;
                            }
                            else if (ch == '(')
                            {
                                isFunction = true;
                            }
                            writer.Write(ch);
                        }
                        ch = (char)reader.Read();
                    }

                    expr = writer.GetStringBuilder().ToString();
                }

                if (isMap)
                {
                    Match match = Regex.Match(expr, @"^([^\:]+)\:([^\|]+)(\|(.+))?$");
                    string mapName = match.Groups[1].Value;
                    string mapArgument = match.Groups[2].Value;
                    string mapDefault = match.Groups[4].Value;

                    IRewriteTransform tx = _configuration.TransformFactory.GetTransform(mapName);
                    if (tx == null)
                    {
                        throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.MappingNotFound, mapName));
                    }

                    result = tx.ApplyTransform(mapArgument) ?? mapDefault;
                }
                else if (isFunction)
                {
                    Match match = Regex.Match(expr, @"^([^\(]+)\((.+)\)$");
                    string functionName = match.Groups[1].Value;
                    string functionArgument = match.Groups[2].Value;

                    IRewriteTransform tx = _configuration.TransformFactory.GetTransform(functionName);
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
        private IRewriterConfiguration _configuration;
        private IHttpContext _httpContext;
        private IConfigurationManager _configurationManager;
    }
}
