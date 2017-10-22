using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
    public interface IRewriteContext
    {
        /// <summary>
        /// The configuration manager.
        /// </summary>
        IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// The current HTTP context.
        /// </summary>
        IHttpContext HttpContext { get; }

        /// <summary>
        /// The current location being rewritten.
        /// </summary>
        /// <remarks>
        /// This property starts out as Request.RawUrl and is altered by various rewrite actions.
        /// </remarks>
        string Location { get; set; }

        /// <summary>
        /// The properties for the context, including headers and cookie values.
        /// </summary>
        NameValueCollection Properties { get; }

        /// <summary>
        /// Output response headers.
        /// </summary>
        /// <remarks>
        /// This collection is the collection of headers to add to the response.
        /// For the headers sent in the request, use the <see cref="IRewriteContext.Properties">Properties</see> property.
        /// </remarks>
        NameValueCollection ResponseHeaders { get; }

        /// <summary>
        /// The status code to send in the response.
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Collection of output cookies.
        /// </summary>
        /// <remarks>
        /// This is the collection of cookies to send in the response.  For the cookies
        /// received in the request, use the <see cref="IRewriteContext.Properties">Properties</see> property.
        /// </remarks>
        HttpCookieCollection ResponseCookies { get; }

        /// <summary>
        /// Last matching pattern from a match (if any).
        /// </summary>
        Match LastMatch { get; set; }

        /// <summary>
        /// Expands the given input using the last match, properties, maps and transforms.
        /// </summary>
        /// <param name="input">The input to expand.</param>
        /// <returns>The expanded form of the input.</returns>
        string Expand(string input);

        /// <summary>
        /// Resolves the location to an absolute reference.
        /// </summary>
        /// <param name="location">The application-referenced location.</param>
        /// <returns>The absolute location.</returns>
        string ResolveLocation(string location);
    }
}