/* Yet Another Forum.NET
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
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Class provides misc helper functions and forum version information
	/// </summary>
	public static class YafForumInfo
	{
		/// <summary>
		/// The forum path (external).
		/// May not be the actual URL of the forum.
		/// </summary>
		static public string ForumRoot
		{
			get
			{
				string _forumRoot = null;

				try
				{
					_forumRoot = UrlBuilder.BaseUrl;
					if (!_forumRoot.EndsWith("/")) _forumRoot += "/";
				}
				catch (Exception)
				{
					_forumRoot = "/";
				}

				return _forumRoot;
			}
		}

		/// <summary>
		/// The forum path (internal).
		/// May not be the actual URL of the forum.
		/// </summary>
		static public string ForumFileRoot
		{
			get
			{
				return UrlBuilder.RootUrl;
			}
		}

		/// <summary>
		/// Server URL based on the server variables. May not actually be 
		/// the URL of the forum.
		/// </summary>
		static public string ServerURL
		{
			get
			{
				StringBuilder url = new StringBuilder();

				if (!Config.BaseUrlOverrideDomain)
				{
					long serverPort = long.Parse(HttpContext.Current.Request.ServerVariables["SERVER_PORT"]);
					bool isSecure = (HttpContext.Current.Request.ServerVariables["HTTPS"] == "ON" || serverPort == 443);

					url.Append("http");

					if (isSecure)
					{
						url.Append("s");
					}

					url.AppendFormat("://{0}", HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);

					if ((!isSecure && serverPort != 80) || (isSecure && serverPort != 443))
					{
						url.AppendFormat(":{0}", serverPort.ToString());
					}
				}
				else
				{
					// pull the domain from BaseUrl...
					string[] sections = UrlBuilder.BaseUrl.Split(new char[] { '/' });

					// add the necessary sections...
					// http(s)
					url.Append(sections[0]);
					url.Append("//");
					url.Append(sections[1]);
				}

				return url.ToString();
			}
		}

		/// <summary>
		/// Complete external URL of the forum.
		/// </summary>
		static public string ForumBaseUrl
		{
			get
			{
				if (!Config.BaseUrlOverrideDomain)
				{
					return ServerURL + ForumRoot;
				}
				else
				{
					// just return the base url...
					return UrlBuilder.BaseUrl;
				}
			}
		}

		static public string ForumURL
		{
			get
			{
				if (!Config.BaseUrlOverrideDomain)
				{
					return string.Format("{0}{1}", YafForumInfo.ServerURL, YafBuildLink.GetLink(ForumPages.forum));
				}
				else
				{
					// link will include the url and domain...
					return YafBuildLink.GetLink(ForumPages.forum);
				}
			}
		}

		static public bool IsLocal
		{
			get
			{
				string s = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
				return s != null && s.ToLower() == "localhost";
			}
		}

		/// <summary>
		/// Helper function that creates the the url of a resource.
		/// </summary>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		static public string GetURLToResource(string resourceName)
		{
			return string.Format("{1}resources/{0}", resourceName, YafForumInfo.ForumRoot);
		}

		#region Version Information
		static public string AppVersionNameFromCode(long code)
		{
			string version;

			if ((code & 0xF0) > 0 || (code & 0x0F) == 1)
			{
				version = String.Format("{0}.{1}.{2}{3}", (code >> 24) & 0xFF, (code >> 16) & 0xFF, (code >> 8) & 0xFF, Convert.ToInt32(((code >> 4) & 0x0F)).ToString("00"));
			}
			else
			{
				version = String.Format("{0}.{1}.{2}", (code >> 24) & 0xFF, (code >> 16) & 0xFF, (code >> 8) & 0xFF);
			}

			if ((code & 0x0F) > 0)
			{
				if ((code & 0x0F) == 1)
				{
					// alpha release...
					version += " alpha";
				}
				else if ((code & 0x0F) == 2)
				{
					version += " beta";
				}
				else
				{
					// Add Release Candidate
					version += string.Format(" RC{0}", (code & 0x0F) - 2);
				}
			}

			return version;
		}
		static public string AppVersionName
		{
			get
			{
				return AppVersionNameFromCode(AppVersionCode);
			}
		}
		static public int AppVersion
		{
			get
			{
				return 34;
			}
		}
		static public long AppVersionCode
		{
			get
			{
				return 0x01090402;
			}
		}
		static public DateTime AppVersionDate
		{
			get
			{
				return new DateTime(2009, 9, 26);
			}
		}
		#endregion
	}
}
