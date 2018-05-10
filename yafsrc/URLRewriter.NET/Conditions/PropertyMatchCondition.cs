// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Conditions
{
	/// <summary>
	/// Performs a property match.
	/// </summary>
	public sealed class PropertyMatchCondition : MatchCondition
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="pattern"></param>
		public PropertyMatchCondition(string propertyName, string pattern) : base(pattern)
		{
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

		    this._propertyName = propertyName;
		}

		/// <summary>
		/// The property name.
		/// </summary>
		public string PropertyName => this._propertyName;

	    /// <summary>
		/// Determines if the condition is matched.
		/// </summary>
		/// <param name="context">The rewriting context.</param>
		/// <returns>True if the condition is met.</returns>
		public override bool IsMatch(RewriteContext context)
		{
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var property = context.Properties[this.PropertyName];
			if (property != null)
			{
				var match = this.Pattern.Match(property);
				if (match.Success)
				{
					context.LastMatch = match;
					return true;
				}

			    return false;
			}

		    return false;
		}

		private string _propertyName = string.Empty;
	}
}
