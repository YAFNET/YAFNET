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
  #region Using

  using System.Web;

  #endregion

  /// <summary>
  /// The empty http context.
  /// </summary>
  public class EmptyHttpContext : HttpContextBase
  {
  }

  /// <summary>
  /// The empty http session state.
  /// </summary>
  public class EmptyHttpSessionState : HttpSessionStateBase
  {
  }

  /// <summary>
  /// The empty http request.
  /// </summary>
  public class EmptyHttpRequest : HttpRequestBase
  {
  }

  /// <summary>
  /// The empty http response.
  /// </summary>
  public class EmptyHttpResponse : HttpResponseBase
  {
  }

  /// <summary>
  /// The empty http server utility.
  /// </summary>
  public class EmptyHttpServerUtility : HttpServerUtilityBase
  {
  }

  /// <summary>
  /// The empty http application state.
  /// </summary>
  public class EmptyHttpApplicationState : HttpApplicationStateBase
  {
  }
}