/* Yet Another Forum.NET
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
  using System.Web;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #region Using


  #endregion

  /// <summary>
  /// The http request is secure.
  /// </summary>
  public class HttpRequestIsSecure : IRequestSecure
  {
    #region Constants and Fields

    /// <summary>
    /// The _http request base.
    /// </summary>
    private readonly HttpRequestBase _httpRequestBase;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestIsSecure"/> class.
    /// </summary>
    /// <param name="httpRequestBase">
    /// The http request base.
    /// </param>
    public HttpRequestIsSecure([NotNull] HttpRequestBase httpRequestBase)
    {
      this._httpRequestBase = httpRequestBase;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsSecure.
    /// </summary>
    public bool IsSecure
    {
      get
      {
        return this._httpRequestBase != null && this._httpRequestBase.IsSecureConnection;
      }
    }

    #endregion
  }
}