/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace YAF.Classes
{
	/// <summary>
	/// Implements URL Builder.
	/// </summary>
	public class UrlBuilder : IUrlBuilder
	{
		private static readonly StringDictionary _baseUrls = new StringDictionary();

		/// <summary>
		/// Builds path for calling page with parameter URL as page's escaped parameter.
		/// </summary>
		/// <param name="url">URL to put into parameter.</param>
		/// <returns>URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.</returns>
		public string BuildUrl(string url)
		{
			// escape & to &amp;
			url = url.Replace("&", "&amp;");

			// return URL to current script with URL from parameter as script's parameter
			return String.Format("{0}{1}?{2}", Path, ScriptName, url);
		}

		/// <summary>
		/// Builds Full URL for calling page with parameter URL as page's escaped parameter.
		/// </summary>
		/// <param name="url">URL to put into parameter.</param>
		/// <returns>URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.</returns>
		public string BuildUrlFull( string url )
		{
			// append the full base server url to the beginning of the url (e.g. http://mydomain.com)
			return String.Format( "{0}{1}", BaseUrl, BuildUrl( url ) );
		}

		static public string ScriptName
		{
			get
			{
				string scriptName = HttpContext.Current.Request.FilePath.ToLower();
				return scriptName.Substring(scriptName.LastIndexOf('/') + 1);
			}
		}

		static public string ScriptNamePath
		{
			get
			{
				string scriptName = HttpContext.Current.Request.FilePath.ToLower();
				return scriptName.Substring(0, scriptName.LastIndexOf('/'));
			}
		}

		static public string GetBaseUrlFromVariables()
		{
			StringBuilder url = new StringBuilder();

			long serverPort = long.Parse( HttpContext.Current.Request.ServerVariables["SERVER_PORT"] );
			bool isSecure = (HttpContext.Current.Request.ServerVariables["HTTPS"] == "ON" || serverPort == 443);

			url.Append( "http" );

			if ( isSecure )
			{
				url.Append( "s" );
			}

			url.AppendFormat( "://{0}", HttpContext.Current.Request.ServerVariables["SERVER_NAME"] );

			if ( (!isSecure && serverPort != 80) || (isSecure && serverPort != 443) )
			{
				url.AppendFormat( ":{0}", serverPort.ToString() );
			}

			return url.ToString();
		}

		static public string BaseUrl
		{
			get
			{
				string baseUrl;

				try
				{
					// Lookup the AppRoot based on the current path. 
					baseUrl = _baseUrls[HttpContext.Current.Request.FilePath];

					if ( String.IsNullOrEmpty( baseUrl ) )
					{
						// Each different filepath (multiboard) will specify a AppRoot key in their own web.config in their directory.
						if ( !String.IsNullOrEmpty( Config.BaseUrlMask ) )
						{
							baseUrl = TreatBaseUrl( Config.BaseUrlMask );
						}
						else
						{
							baseUrl = GetBaseUrlFromVariables();
						}

						// save to cache
						_baseUrls[HttpContext.Current.Request.FilePath] = baseUrl;
					}
				}
				catch (Exception)
				{
					baseUrl = GetBaseUrlFromVariables();
				}

				return baseUrl;
			}
		}

		protected static string TreatBaseUrl( string baseUrl )
		{
			if (baseUrl.EndsWith("/"))
			{
				// remove ending slash...
				baseUrl = baseUrl.Remove( baseUrl.Length-1, 1 );
			}

			return baseUrl;
		}

		protected static string TreatPathStr( string altRoot )
		{
			string _path = string.Empty;

			try
			{
				_path = HttpContext.Current.Request.ApplicationPath;

				if ( !_path.EndsWith( "/" ) ) _path += "/";

				if ( !String.IsNullOrEmpty( altRoot ) )
				{
					// use specified root
					_path = altRoot;

					if ( _path.StartsWith( "~" ) )
					{
						// transform with application path...
						_path = _path.Replace( "~", HttpContext.Current.Request.ApplicationPath );
					}

					if ( _path[0] != '/' ) _path = _path.Insert( 0, "/" );
				}
				else if ( Config.IsDotNetNuke )
				{
					_path += "DesktopModules/YetAnotherForumDotNet/";
				}
				else if ( Config.IsRainbow )
				{
					_path += "DesktopModules/Forum/";
				}
				else if ( Config.IsPortal )
				{
					_path += "Modules/Forum/";
				}

				if ( !_path.EndsWith( "/" ) ) _path += "/";

				// remove redundant slashes...
				while ( _path.Contains( "//" ) )
				{
					_path = _path.Replace( "//", "/" );
				}
			}
			catch ( Exception )
			{
				_path = "/";
			}

			return _path;			
		}

		static public string FileRoot
		{
			get
			{
				string altRoot = Config.FileRoot;

				if ( String.IsNullOrEmpty( altRoot ) && !String.IsNullOrEmpty(Config.AppRoot) )
				{
					// default to "AppRoot" if no file root specified and AppRoot specified...
					altRoot = Config.AppRoot;
				}

				return TreatPathStr( altRoot );
			}
		}


		static public string Path
		{
			get
			{
				return TreatPathStr( Config.AppRoot );
			}
		}
	}
}
