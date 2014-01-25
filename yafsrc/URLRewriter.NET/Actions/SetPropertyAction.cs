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
	/// Action that sets properties in the context.
	/// </summary>
	public class SetPropertyAction : IRewriteAction
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The name of the value.</param>
		public SetPropertyAction(string name, string value)
		{
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            _name = name;
			_value = value;
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
            context.Properties.Set(Name, context.Expand(Value));
            return RewriteProcessing.ContinueProcessing;
		}

		private string _name;
		private string _value;
	}
}
