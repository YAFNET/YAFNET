using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using YAF.Classes;

namespace YAF.Classes.Utils
{
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
			HttpContext.Current.Response.Redirect(GetLinkNotEscaped(page));
		}
		/// <summary>
		/// Redirects to the given page with parameters.
		/// </summary>
		/// <param name="page">Page to which to redirect response.</param>
		/// <param name="format">Format of parameters.</param>
		/// <param name="args">Array of page parameters.</param>
		static public void Redirect(ForumPages page, string format, params object[] args)
		{
			HttpContext.Current.Response.Redirect(GetLinkNotEscaped(page, format, args));
		}


		/// <summary>
		/// Redirects response to the access denied page.
		/// </summary>
		static public void AccessDenied()
		{
			Redirect(ForumPages.info, "i=4");
		}


		/// <summary>
		/// Gets full path to the given theme file.
		/// </summary>
		/// <param name="filename">Short name of theme file.</param>
		/// <returns></returns>
		static public string ThemeFile(string filename)
		{
			return YafContext.Current.Theme.ThemeDir + filename;
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
