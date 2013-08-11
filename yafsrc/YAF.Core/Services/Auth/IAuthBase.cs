/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Björnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF.Core.Services.Auth
{
    using System.Web;

    /// <summary>
    /// Interface For oAUTH
    /// </summary>
    public interface IAuthBase
    {
        /// <summary>
        /// Generates the login URL.
        /// </summary>
        /// <param name="generatePopUpUrl">if set to <c>true</c> [generate pop up URL].</param>
        /// <param name="connectCurrentUser">if set to <c>true</c> [connect current user].</param>
        /// <returns>Returns the Login URL</returns>
        string GenerateLoginUrl(bool generatePopUpUrl, bool connectCurrentUser = false);

        /// <summary>
        /// Logins the or create user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="message">The message.</param>
        /// <returns>Returns if Login was successful or not</returns>
        bool LoginOrCreateUser(HttpRequest request, string parameters, out string message);

        /// <summary>
        /// Connects the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="message">The message.</param>
        /// <returns>Returns if the connect was successful or not</returns>
        bool ConnectUser(HttpRequest request, string parameters, out string message);
    }
}