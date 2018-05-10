/* UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
*/

namespace Intelligencia.UrlRewriter
{
    using System.Web.UI;
    using System.Web.UI.Adapters;

    /// <summary>
    /// ControlAdapter for rewriting form actions
    /// </summary>
    public class FormRewriterControlAdapter : ControlAdapter
    {
        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="writer">The writer to write to</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(new RewriteFormHtmlTextWriter(writer));
        }
    }
}
