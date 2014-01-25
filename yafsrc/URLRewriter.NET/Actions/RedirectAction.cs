// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Net;
using System.Web;
using System.Collections;
using Intelligencia.UrlRewriter.Conditions;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// Redirect using 302 temporary redirect.
	/// </summary>
	public sealed class RedirectAction : SetLocationAction, IRewriteCondition
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="location">The location to set.</param>
		/// <param name="permanent">Whether the redirection is permanent.</param>
		public RedirectAction(string location, bool permanent) : base(location)
		{
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            _permanent = permanent;
		}

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="context">The rewriting context.</param>
        public override RewriteProcessing Execute(RewriteContext context)
		{
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            base.Execute(context);
			if (_permanent)
			{
				context.StatusCode = HttpStatusCode.Moved;
			}
			else
			{
				context.StatusCode = HttpStatusCode.Found;
			}

            return RewriteProcessing.StopProcessing;
		}

		/// <summary>
		/// Determines if the rewrite rule matches.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public bool IsMatch(RewriteContext context)
		{
			// Ensure the conditions are met.
			foreach (IRewriteCondition condition in Conditions)
			{
				if (!condition.IsMatch(context))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Conditions that must hold for the rule to fire.
		/// </summary>
		public IList Conditions
		{
			get
			{
				return _conditions;
			}
		}

		private ArrayList _conditions = new ArrayList();
		private bool _permanent;
	}
}
