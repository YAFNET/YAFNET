// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// Sets the Location.
	/// </summary>
	public abstract class SetLocationAction : IRewriteAction
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="location">The location (pattern) to set.</param>
		protected SetLocationAction(string location)
		{
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            _location = location;
		}

		/// <summary>
		/// The location to set.  This can include replacements referencing the matched pattern,
		/// for example $1, $2, ... $n and ${group} as well as ${ServerVariable} and mapping, e.g., 
		/// ${MapName:$1}.
		/// </summary>
		public string Location
		{
			get
			{
				return _location;
			}
		}

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="context">The rewriting context.</param>
        public virtual RewriteProcessing Execute(RewriteContext context)
		{
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.Location = context.ResolveLocation(context.Expand(Location));
            return RewriteProcessing.StopProcessing;
		}

		private string _location;
	}
}
