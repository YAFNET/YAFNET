// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;
using Intelligencia.UrlRewriter.Actions;
using Intelligencia.UrlRewriter.Conditions;
using Intelligencia.UrlRewriter.Utilities;
using Intelligencia.UrlRewriter.Configuration;

namespace Intelligencia.UrlRewriter.Parsers
{
    /// <summary>
    /// Parses the IFNOT node.
    /// </summary>
    public class UnlessConditionActionParser : IfConditionActionParser
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public override string Name
        {
            get
            {
                return Constants.ElementUnless;
            }
        }
    }
}
