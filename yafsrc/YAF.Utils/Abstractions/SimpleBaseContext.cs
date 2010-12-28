/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Utils
{
  #region Using

  using System.Web;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The simple base context.
  /// </summary>
  public class SimpleBaseContext : IBaseContext
  {
    #region Constants and Fields

    /// <summary>
    ///   The _application.
    /// </summary>
    private readonly HttpApplicationStateBase _application;

    /// <summary>
    ///   The _http context.
    /// </summary>
    private readonly HttpContextBase _httpContext;

    /// <summary>
    ///   The _request.
    /// </summary>
    private readonly HttpRequestBase _request;

    /// <summary>
    ///   The _response.
    /// </summary>
    private readonly HttpResponseBase _response;

    /// <summary>
    ///   The _session.
    /// </summary>
    private readonly HttpSessionStateBase _session;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleBaseContext"/> class.
    /// </summary>
    /// <param name="application">
    /// The application.
    /// </param>
    /// <param name="httpContext">
    /// The http context.
    /// </param>
    /// <param name="request">
    /// The request.
    /// </param>
    /// <param name="response">
    /// The response.
    /// </param>
    /// <param name="session">
    /// The session.
    /// </param>
    protected SimpleBaseContext(
      [NotNull] HttpApplicationStateBase application, 
      [NotNull] HttpContextBase httpContext, 
      [NotNull] HttpRequestBase request, 
      [NotNull] HttpResponseBase response, 
      [NotNull] HttpSessionStateBase session)
    {
      this._application = application;
      this._httpContext = httpContext;
      this._request = request;
      this._response = response;
      this._session = session;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Application.
    /// </summary>
    public HttpApplicationStateBase Application
    {
      get
      {
        return this._application;
      }
    }

    /// <summary>
    ///   Gets HttpContext.
    /// </summary>
    public HttpContextBase HttpContext
    {
      get
      {
        return this._httpContext;
      }
    }

    /// <summary>
    ///   Gets Request.
    /// </summary>
    public HttpRequestBase Request
    {
      get
      {
        return this._request;
      }
    }

    /// <summary>
    ///   Gets Response.
    /// </summary>
    public HttpResponseBase Response
    {
      get
      {
        return this._response;
      }
    }

    /// <summary>
    ///   Gets Session.
    /// </summary>
    public HttpSessionStateBase Session
    {
      get
      {
        return this._session;
      }
    }

    #endregion
  }
}