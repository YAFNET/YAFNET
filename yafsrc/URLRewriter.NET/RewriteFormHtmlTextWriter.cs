using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Adapters;

namespace Intelligencia.UrlRewriter
{
    /// <summary>
    /// The HTML Text Writer to use for rewriting form actions.
    /// </summary>
    public class RewriteFormHtmlTextWriter : HtmlTextWriter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        public RewriteFormHtmlTextWriter(HtmlTextWriter writer) : base(writer)
        {
            InnerWriter = writer.InnerWriter;
        }

        /// <summary>
        /// Writes an attribute
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <param name="fEncode"></param>
        public override void WriteAttribute(string name, string value, bool fEncode)
        {
            // If the attribute we are writing is the "action" attribute, and we are not on a sub-control, 
            // then replace the value to write with the raw URL of the request - which ensures that we'll
            // preserve the PathInfo value on postback scenarios
            if (name == "action")
            {
                if (HttpContext.Current.Items["ActionAlreadyWritten"] == null)
                {
                    value = RewriterHttpModule.RawUrl;

                    // Indicate that we've already rewritten the <form>'s action attribute to prevent
                    // us from rewriting a sub-control under the <form> control
                    HttpContext.Current.Items["ActionAlreadyWritten"] = true;
                }
            }

            base.WriteAttribute(name, value, fEncode);
        }
    }
}
