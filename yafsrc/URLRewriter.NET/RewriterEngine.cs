// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Errors;
using Intelligencia.UrlRewriter.Conditions;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
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
            
            ContextFacade = contextFacade;
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

			string appPath = ContextFacade.GetApplicationPath();

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
			string originalUrl = ContextFacade.GetRawUrl().Replace("+", " ");
            RawUrl = originalUrl;

			// Create the context
			RewriteContext context = new RewriteContext(this, originalUrl,
				ContextFacade.GetHttpMethod(), ContextFacade.MapPath,
				ContextFacade.GetServerVariables(), ContextFacade.GetHeaders(), ContextFacade.GetCookies());

			// Process each rule.
			ProcessRules(context);

			// Append any headers defined.
			AppendHeaders(context);

			// Append any cookies defined.
			AppendCookies(context);

			// Rewrite the path if the location has changed.
			ContextFacade.SetStatusCode((int)context.StatusCode);
			if ((context.Location != originalUrl) && ((int)context.StatusCode < 400))
			{
				if ((int)context.StatusCode < 300)
				{
					// Successful status if less than 300
					_configuration.Logger.Info(MessageProvider.FormatString(Message.RewritingXtoY, 
						ContextFacade.GetRawUrl(), context.Location));

					// Verify that the url exists on this server.
                    HandleDefaultDocument(context);// VerifyResultExists(context);

					ContextFacade.RewritePath(context.Location);
				}
				else
				{
					// Redirection
					_configuration.Logger.Info(MessageProvider.FormatString(Message.RedirectingXtoY,
						ContextFacade.GetRawUrl(), context.Location));

					ContextFacade.SetRedirectLocation(context.Location);
				}
			}
			else if ((int)context.StatusCode >= 400)
			{
				HandleError(context);
			}
			else if (HandleDefaultDocument(context))
			{
				ContextFacade.RewritePath(context.Location);
			}

			// Sets the context items.
			SetContextItems(context);
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

			using (StringReader reader = new StringReader(input))
			{
				using (StringWriter writer = new StringWriter())
				{
					char ch = (char)reader.Read();
					while (ch != (char)65535)
					{
						if ((char)ch == '$')
						{
							writer.Write(Reduce(context, reader));
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

			IList rewriteRules = _configuration.Rules;
			int restarts = 0;
			for (int i = 0; i < rewriteRules.Count; i++)
			{
				// If the rule is conditional, ensure the conditions are met.
				IRewriteCondition condition = rewriteRules[i] as IRewriteCondition;
				if (condition == null || condition.IsMatch(context))
				{
					// Execute the action.
					IRewriteAction action = rewriteRules[i] as IRewriteAction;
                    RewriteProcessing processing = action.Execute(context);

					// If the action is Stop, then break out of the processing loop
					if (processing == RewriteProcessing.StopProcessing)
					{
						_configuration.Logger.Debug(MessageProvider.FormatString(Message.StoppingBecauseOfRule));
						break;
					}
					else if (processing == RewriteProcessing.RestartProcessing)
					{
						_configuration.Logger.Debug(MessageProvider.FormatString(Message.RestartingBecauseOfRule));

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

		private bool HandleDefaultDocument(RewriteContext context)
		{
			Uri uri = new Uri(ContextFacade.GetRequestUrl(), context.Location);
            UriBuilder b = new UriBuilder(uri);
            b.Path += "/";
            uri = b.Uri;
			if (uri.Host == ContextFacade.GetRequestUrl().Host)
			{
				string filename = ContextFacade.MapPath(uri.AbsolutePath);
				if (Directory.Exists(filename))
				{
					foreach (string document in RewriterConfiguration.Current.DefaultDocuments)
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

		private void VerifyResultExists(RewriteContext context)
		{
			if ((String.Compare(context.Location, ContextFacade.GetRawUrl()) != 0) && ((int)context.StatusCode < 300))
			{
				Uri uri = new Uri(ContextFacade.GetRequestUrl(), context.Location);
				if (uri.Host == ContextFacade.GetRequestUrl().Host)
				{
					string filename = ContextFacade.MapPath(uri.AbsolutePath);
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

		private void HandleError(RewriteContext context)
		{
			// Return the status code.
			ContextFacade.SetStatusCode((int)context.StatusCode);

			// Get the error handler if there is one.
			IRewriteErrorHandler handler = _configuration.ErrorHandlers[(int)context.StatusCode] as IRewriteErrorHandler;
			if (handler != null)
			{
				try
				{
					_configuration.Logger.Debug(MessageProvider.FormatString(Message.CallingErrorHandler));

					// Execute the error handler.
					ContextFacade.HandleError(handler);
				}
				catch (HttpException)
				{
					throw;
				}
				catch (Exception exc)
				{
					_configuration.Logger.Fatal(exc.Message, exc);
					throw new HttpException((int)HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError.ToString());
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
				ContextFacade.AppendHeader(headerKey, context.Headers[headerKey]);
			}
		}

		private void AppendCookies(RewriteContext context)
		{
			for (int i = 0; i < context.Cookies.Count; i++)
			{
				HttpCookie cookie = context.Cookies[i];
				ContextFacade.AppendCookie(cookie);
			}
		}

		private void SetContextItems(RewriteContext context)
		{
			OriginalQueryString = new Uri(ContextFacade.GetRequestUrl(), ContextFacade.GetRawUrl()).Query.Replace("?", "");
			QueryString = new Uri(ContextFacade.GetRequestUrl(), context.Location).Query.Replace("?", "");

            // Add in the properties as context items, so these will be accessible to the handler
            foreach (string key in context.Properties.Keys)
            {
                ContextFacade.SetItem(String.Format("Rewriter.{0}", key), context.Properties[key]);
            }
		}

        /// <summary>
        /// The raw url.
        /// </summary>
        public string RawUrl
        {
            get
            {
                return (string)ContextFacade.GetItem(ContextRawUrl);
            }
            set
            {
                ContextFacade.SetItem(ContextRawUrl, value);
            }
        }

        /// <summary>
		/// The original query string.
		/// </summary>
		public string OriginalQueryString
		{
			get
			{
				return (string)ContextFacade.GetItem(ContextOriginalQueryString);
			}
			set
			{
				ContextFacade.SetItem(ContextOriginalQueryString, value);
			}
		}

		/// <summary>
		/// The final querystring, after rewriting.
		/// </summary>
		public string QueryString
		{
			get
			{
				return (string)ContextFacade.GetItem(ContextQueryString);
			}
			set
			{
				ContextFacade.SetItem(ContextQueryString, value);
			}
		}		

		private string Reduce(RewriteContext context, StringReader reader)
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
					if (group != null)
					{
						result = group.Value;
					}
					else
					{
						result = String.Empty;
					}
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
					while (ch != '>' && ch != (char)65535)
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
					if (group != null)
					{
						result = group.Value;
					}
					else
					{
						result = String.Empty;
					}
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
					while (ch != '}' && ch != (char)65535)
					{
						if (ch == '$')
						{
							writer.Write(Reduce(context, reader));
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
					Match match = Regex.Match(expr, @"^([^\:]+)\:([^\|]+)(\|(.+))?$");
					string mapName = match.Groups[1].Value;
					string mapArgument = match.Groups[2].Value;
					string mapDefault = match.Groups[4].Value;
					result = _configuration.TransformFactory.GetTransform(mapName).ApplyTransform(mapArgument);
					if (result == null)
					{
						result = mapDefault;
					}
				}
				else if (isFunction)
				{
					Match match = Regex.Match(expr, @"^([^\(]+)\((.+)\)$");
					string functionName = match.Groups[1].Value;
					string functionArgument = match.Groups[2].Value;
                    IRewriteTransform tx = _configuration.TransformFactory.GetTransform(functionName);
                    if (tx != null)
                    {
                        result = tx.ApplyTransform(functionArgument);
                    }
                    else
                    {
                        result = expr;
                    }
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
		private RewriterConfiguration _configuration;
		private IContextFacade ContextFacade;
	}
}
 