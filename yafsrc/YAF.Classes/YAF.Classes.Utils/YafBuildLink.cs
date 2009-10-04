/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using YAF.Classes;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Enumerates forum info messages.
	/// </summary>
	public enum InfoMessage
	{
		Moderated = 1,				// after posting to moderated forum
		Suspended = 2,				// informs user he's suspended
		RegistrationEmail = 3,		// informs user about registration email being sent
		AccessDenied = 4,			// access was denied
		Disabled = 5,				// informs user about feature being disabled by admin 
		Invalid = 6,				// informs user about invalid input/request
		Failure = 7					// system error
	}

	/// <summary>
	/// Static class with link building functions.
	/// </summary>
	public static class YafBuildLink
	{
		/// <summary>
		/// Gets link to the page.
		/// </summary>
		/// <param name="page">Page to which to create a link.</param>
		/// <returns>URL to the given page.</returns>
		static public string GetLink(ForumPages page)
		{
			return Config.UrlBuilder.BuildUrl(string.Format("g={0}", page));
		}
		/// <summary>
		/// Gets link to the page with given parameters.
		/// </summary>
		/// <param name="page">Page to which to create a link.</param>
		/// <param name="format">Format of parameters.</param>
		/// <param name="args">Array of page parameters.</param>
		/// <returns>URL to the given page with parameters.</returns>
		static public string GetLink(ForumPages page, string format, params object[] args)
		{
			return Config.UrlBuilder.BuildUrl(string.Format("g={0}&{1}", page, string.Format(format, args)));
		}


		/// <summary>
		/// Unescapes ampersands in the link to the given page.
		/// </summary>
		/// <param name="page">Page to which to create a link.</param>
		/// <returns>URL to the given page with unescaped ampersands.</returns>
		static public string GetLinkNotEscaped(ForumPages page)
		{
			return GetLink(page).Replace("&amp;", "&");
		}
		/// <summary>
		/// Unescapes ampersands in the link to the given page with parameters.
		/// </summary>
		/// <param name="page">Page to which to create a link.</param>
		/// <param name="format">Format of parameters.</param>
		/// <param name="args">Array of page parameters.</param>
		/// <returns>URL to the given page with parameters and unescaped ampersands.</returns>
		static public string GetLinkNotEscaped(ForumPages page, string format, params object[] args)
		{
			return GetLink(page, format, args).Replace("&amp;", "&");
		}


		/// <summary>
		/// Redirects to the given page.
		/// </summary>
		/// <param name="page">Page to which to redirect response.</param>
		static public void Redirect(ForumPages page)
		{
			HttpContext.Current.Response.Redirect( GetLinkNotEscaped( page ) );
		}
		/// <summary>
		/// Redirects to the given page with parameters.
		/// </summary>
		/// <param name="page">Page to which to redirect response.</param>
		/// <param name="format">Format of parameters.</param>
		/// <param name="args">Array of page parameters.</param>
		static public void Redirect(ForumPages page, string format, params object[] args)
		{
			HttpContext.Current.Response.Redirect( GetLinkNotEscaped( page, format, args ) );
		}

		/// <summary>
		/// Redirects response to the info page.
		/// </summary>
		static public void RedirectInfoPage( InfoMessage infoMessage )
		{
			Redirect( ForumPages.info, String.Format("i={0}", (int)infoMessage ) );
		}


		/// <summary>
		/// Redirects response to the access denied page.
		/// </summary>
		static public void AccessDenied()
		{
			Redirect(ForumPages.info, "i=4");
		}

		/// <summary>
		/// Gets URL of given smilie.
		/// </summary>
		/// <param name="icon">Name of icon image file.</param>
		/// <returns>URL of a smilie.</returns>
		static public string Smiley(string icon)
		{
			return String.Format("{0}images/emoticons/{1}", YafForumInfo.ForumRoot, icon);
		}
	}
}
