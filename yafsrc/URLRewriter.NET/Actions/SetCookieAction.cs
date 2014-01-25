// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Web;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// Action that sets a cookie.
	/// </summary>
	public class SetCookieAction : IRewriteAction
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="cookieName">The cookie name.</param>
		/// <param name="cookieValue">The cookie value.</param>
		public SetCookieAction(string cookieName, string cookieValue)
		{
            if (cookieName == null)
            {
                throw new ArgumentNullException("cookieName");
            }
            if (cookieValue == null)
            {
                throw new ArgumentNullException("cookieValue");
            }
            _name = cookieName;
			_value = cookieValue;
		}

		/// <summary>
		/// The name of the variable.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// The value of the variable.
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="context">The rewrite context.</param>
        public RewriteProcessing Execute(RewriteContext context)
		{
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            HttpCookie cookie = new HttpCookie(Name, Value);
			context.Cookies.Add(cookie);
            return RewriteProcessing.ContinueProcessing;
		}

		private string _name;
		private string _value;
	}
}
