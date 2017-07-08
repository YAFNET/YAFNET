// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using Intelligencia.UrlRewriter.Configuration;
    using Intelligencia.UrlRewriter.Utilities;

    /// <summary>
    /// The core RewriterEngine class.
    /// </summary>
    public class RewriterEngine
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contextFacade">The context facade to use.</param>
        /// <param name="configuration">The configuration to use.</param>
        public RewriterEngine(IContextFacade contextFacade, RewriterConfiguration configuration)
        {
            if (contextFacade == null)
            {
                throw new ArgumentNullException("contextFacade");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.ContextFacade = contextFacade;
            this._configuration = configuration;
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

            var appPath = this.ContextFacade.GetApplicationPath();

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
            var originalUrl = this.ContextFacade.GetRawUrl().Replace("+", " ");
            this.RawUrl = originalUrl;

            // Create the context
            var context = new RewriteContext(
                this,
                originalUrl,
                this.ContextFacade.GetHttpMethod(),
                this.ContextFacade.MapPath,
                this.ContextFacade.GetServerVariables(),
                this.ContextFacade.GetHeaders(),
                this.ContextFacade.GetCookies());

            // Process each rule.
            this.ProcessRules(context);

            // Append any headers defined.
            this.AppendHeaders(context);

            // Append any cookies defined.
            this.AppendCookies(context);

            // Rewrite the path if the location has changed.
            this.ContextFacade.SetStatusCode((int)context.StatusCode);
            if ((context.Location != originalUrl) && ((int)context.StatusCode < 400))
            {
                if ((int)context.StatusCode < 300)
                {
                    // Successful status if less than 300
                    this._configuration.Logger.Info(
                        MessageProvider.FormatString(Message.RewritingXtoY, this.ContextFacade.GetRawUrl(), context.Location));

                    // Verify that the url exists on this server.
                    this.HandleDefaultDocument(context); // VerifyResultExists(context);

                    if (context.Location.Contains(@"&"))
                    {
                        var queryStringCollection = HttpUtility.ParseQueryString(new Uri(this.ContextFacade.GetRequestUrl(), context.Location).Query);

                        var builder = new StringBuilder();
                        foreach (var value in queryStringCollection.AllKeys.Distinct())
                        {
                            var argument = queryStringCollection.GetValues(value).FirstOrDefault();
                            if (value == "g")
                            {
                                if (context.Location != argument)
                                {
                                    builder.AppendFormat(
                                        "{0}={1}&",
                                        value,
                                        HttpUtility.UrlEncode(argument));
                                }
                            }
                            else
                            {
                                builder.AppendFormat(
                                   "{0}={1}&",
                                   value,
                                   HttpUtility.UrlEncode(argument));
                            }
                        }

                        context.Location = context.Location.Remove(context.Location.IndexOf("?") + 1);
                        context.Location = context.Location + builder;

                        if (context.Location.EndsWith(@"&"))
                        {
                            context.Location = context.Location.Remove(context.Location.Length - 1);
                        }
                    }

                    this.ContextFacade.RewritePath(context.Location);
                }
                else
                {
                    // Redirection
                    this._configuration.Logger.Info(
                        MessageProvider.FormatString(
                            Message.RedirectingXtoY,
                            this.ContextFacade.GetRawUrl(), context.Location));

                    this.ContextFacade.SetRedirectLocation(context.Location);
                }
            }
            else if ((int)context.StatusCode >= 400)
            {
                this.HandleError(context);
            }
            else if (this.HandleDefaultDocument(context))
            {
                this.ContextFacade.RewritePath(context.Location);
            }

            // Sets the context items.
            this.SetContextItems(context);
        }

        /// <summary>
        /// Expands the given input based on the current context.
        /// </summary>
        /// <param name="context">The current context</param>
        /// <param name="input">The input to expand.</param>
        /// <returns>The expanded input</returns>
        public string Expand(RewriteContext context, string input)
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
            using (var reader = new StringReader(input))
            {
                using (var writer = new StringWriter())
                {
                    var ch = (char)reader.Read();
                    while (ch != (char)65535)
                    {
                        if ((char)ch == '$')
                        {
                            writer.Write(this.Reduce(context, reader));
                        }
                        else
                        {
                            writer.Write((char)ch);
                        }

                        ch = (char)reader.Read();
                    }

                    return writer.GetStringBuilder().ToString();
                }
            }
        }

        private void ProcessRules(RewriteContext context)
        {
            const int MaxRestart = 10; // Controls the number of restarts so we don't get into an infinite loop

            var rewriteRules = this._configuration.Rules;
            var restarts = 0;
            for (var i = 0; i < rewriteRules.Count; i++)
            {
                // If the rule is conditional, ensure the conditions are met.
                var condition = rewriteRules[i] as IRewriteCondition;
                if (condition == null || condition.IsMatch(context))
                {
                    // Execute the action.
                    var action = rewriteRules[i] as IRewriteAction;
                    var processing = action.Execute(context);

                    // If the action is Stop, then break out of the processing loop
                    if (processing == RewriteProcessing.StopProcessing)
                    {
                        this._configuration.Logger.Debug(MessageProvider.FormatString(Message.StoppingBecauseOfRule));
                        break;
                    }

                    if (processing == RewriteProcessing.RestartProcessing)
                    {
                        this._configuration.Logger.Debug(MessageProvider.FormatString(Message.RestartingBecauseOfRule));

                        // Restart from the first rule.
                        i = 0;

                        if (++restarts > MaxRestart)
                        {
                            throw new InvalidOperationException(MessageProvider.FormatString(Message.TooManyRestarts));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the default document.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private bool HandleDefaultDocument(RewriteContext context)
        {
            var uri = new Uri(this.ContextFacade.GetRequestUrl(), context.Location);
            var b = new UriBuilder(uri);
            b.Path += "/";
            uri = b.Uri;
            if (uri.Host == this.ContextFacade.GetRequestUrl().Host)
            {
                try
                {
                    var filename = this.ContextFacade.MapPath(uri.AbsolutePath);

                    if (Directory.Exists(filename))
                    {
                        foreach (
                            var document in from string document in RewriterConfiguration.Current.DefaultDocuments
                                               let pathName = Path.Combine(filename, document)
                                               where File.Exists(pathName)
                                               select document)
                        {
                            context.Location = new Uri(uri, document).AbsolutePath;
                            return true;
                        }
                    }
                }
                catch (PathTooLongException)
                {
                    // ignore if path to long
                    return false;
                }
            }

            return false;
        }

        private void VerifyResultExists(RewriteContext context)
        {
            if ((string.Compare(context.Location, this.ContextFacade.GetRawUrl()) != 0) && ((int)context.StatusCode < 300))
            {
                var uri = new Uri(this.ContextFacade.GetRequestUrl(), context.Location);
                if (uri.Host == this.ContextFacade.GetRequestUrl().Host)
                {
                    var filename = this.ContextFacade.MapPath(uri.AbsolutePath);
                    if (!File.Exists(filename))
                    {
                        this._configuration.Logger.Debug(MessageProvider.FormatString(Message.ResultNotFound, filename));
                        context.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        this.HandleDefaultDocument(context);
                    }
                }
            }
        }

        private void HandleError(RewriteContext context)
        {
            // Return the status code.
            this.ContextFacade.SetStatusCode((int)context.StatusCode);

            // Get the error handler if there is one.
            var handler = this._configuration.ErrorHandlers[(int)context.StatusCode] as IRewriteErrorHandler;
            if (handler != null)
            {
                try
                {
                    this._configuration.Logger.Debug(MessageProvider.FormatString(Message.CallingErrorHandler));

                    // Execute the error handler.
                    this.ContextFacade.HandleError(handler);
                }
                catch (HttpException)
                {
                    throw;
                }
                catch (Exception exc)
                {
                    this._configuration.Logger.Fatal(exc.Message, exc);
                    throw new HttpException(
                        (int)HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError.ToString());
                }
            }
            else
            {
                throw new HttpException((int)context.StatusCode, context.StatusCode.ToString());
            }
        }

        private void AppendHeaders(RewriteContext context)
        {
            foreach (string headerKey in context.Headers)
            {
                this.ContextFacade.AppendHeader(headerKey, context.Headers[headerKey]);
            }
        }

        private void AppendCookies(RewriteContext context)
        {
            for (var i = 0; i < context.Cookies.Count; i++)
            {
                var cookie = context.Cookies[i];
                this.ContextFacade.AppendCookie(cookie);
            }
        }

        private void SetContextItems(RewriteContext context)
        {
            this.OriginalQueryString =
                new Uri(this.ContextFacade.GetRequestUrl(), this.ContextFacade.GetRawUrl()).Query.Replace("?", string.Empty);

            this.QueryString = new Uri(this.ContextFacade.GetRequestUrl(), context.Location).Query.Replace("?", string.Empty);

            // Add in the properties as context items, so these will be accessible to the handler
            foreach (string key in context.Properties.Keys)
            {
                this.ContextFacade.SetItem(string.Format("Rewriter.{0}", key), context.Properties[key]);
            }
        }

        /// <summary>
        /// The raw url.
        /// </summary>
        public string RawUrl
        {
            get => (string)this.ContextFacade.GetItem(ContextRawUrl);
            set => this.ContextFacade.SetItem(ContextRawUrl, value);
        }

        /// <summary>
        /// The original query string.
        /// </summary>
        public string OriginalQueryString
        {
            get => (string)this.ContextFacade.GetItem(ContextOriginalQueryString);
            set => this.ContextFacade.SetItem(ContextOriginalQueryString, value);
        }

        /// <summary>
        /// The final querystring, after rewriting.
        /// </summary>
        public string QueryString
        {
            get => (string)this.ContextFacade.GetItem(ContextQueryString);
            set => this.ContextFacade.SetItem(ContextQueryString, value);
        }

        private string Reduce(RewriteContext context, StringReader reader)
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
                    var group = context.LastMatch.Groups[Convert.ToInt32(num)];

                    result = @group != null ? @group.Value : string.Empty;
                }
                else
                {
                    result = string.Empty;
                }
            }
            else
                switch (ch)
                {
                    case '<':
                        {
                            string expr;

                            using (var writer = new StringWriter())
                            {
                                ch = (char)reader.Read();
                                while (ch != '>' && ch != (char)65535)
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
                                if (@group != null)
                                {
                                    result = @group.Value;
                                }
                                else
                                {
                                    result = string.Empty;
                                }
                            }
                            else
                            {
                                result = string.Empty;
                            }
                        }

                        break;
                    case '{':
                        {
                            string expr;
                            var isMap = false;
                            var isFunction = false;

                            using (var writer = new StringWriter())
                            {
                                ch = (char)reader.Read();
                                while (ch != '}' && ch != (char)65535)
                                {
                                    if (ch == '$')
                                    {
                                        writer.Write(this.Reduce(context, reader));
                                    }
                                    else
                                    {
                                        if (ch == ':') isMap = true;
                                        else if (ch == '(') isFunction = true;
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
                                result =
                                    this._configuration.TransformFactory.GetTransform(mapName).ApplyTransform(
                                        mapArgument) ?? mapDefault;
                            }
                            else if (isFunction)
                            {
                                var match = Regex.Match(expr, @"^([^\(]+)\((.+)\)$");
                                var functionName = match.Groups[1].Value;
                                var functionArgument = match.Groups[2].Value;
                                var tx = this._configuration.TransformFactory.GetTransform(functionName);

                                result = tx != null ? tx.ApplyTransform(functionArgument) : expr;
                            }
                            else
                            {
                                result = context.Properties[expr];
                            }
                        }

                        break;
                    default:
                        result = ch.ToString();
                        break;
                }

            return result;
        }

        private const string ContextQueryString = "UrlRewriter.NET.QueryString";

        private const string ContextOriginalQueryString = "UrlRewriter.NET.OriginalQueryString";

        private const string ContextRawUrl = "UrlRewriter.NET.RawUrl";

        private RewriterConfiguration _configuration;

        private IContextFacade ContextFacade;
    }
}