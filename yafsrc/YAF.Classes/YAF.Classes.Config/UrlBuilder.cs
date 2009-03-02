/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web;
using System.Collections.Specialized;

namespace YAF.Classes
{
	/// <summary>
	/// Implements URL Builder.
	/// </summary>
	public class UrlBuilder : IUrlBuilder
	{
        private static StringDictionary _baseUrls = null;
		private static string _fileRoot = null;

		/// <summary>
		/// Builds URL for calling page with parameter URL as page's escaped parameter.
		/// </summary>
		/// <param name="url">URL to put into parameter.</param>
		/// <returns>URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.</returns>
		public string BuildUrl(string url)
		{
			// escape & to &amp;
			url = url.Replace( "&", "&amp;" );

			// return URL to current script with URL from parameter as script's parameter
			return String.Format( "{0}{1}?{2}", UrlBuilder.BaseUrl, UrlBuilder.ScriptName, url );
		}

		static public string ScriptName
		{
			get
			{
				string scriptName = HttpContext.Current.Request.FilePath.ToLower();
				return scriptName.Substring( scriptName.LastIndexOf( '/' ) );
			}
		}

		static public string ScriptNamePath
		{
			get
			{
				string scriptName = HttpContext.Current.Request.FilePath.ToLower();
				return scriptName.Substring( 0, scriptName.LastIndexOf( '/' ) );
			}
		}

		static public string BaseUrl
		{
			get
			{
                string baseUrl;

				if ( _baseUrls == null )
				{
                    _baseUrls = new StringDictionary();
				}

                try
                {
                    // Lookup the BaseUrl based on the current path. 
                    baseUrl = _baseUrls[HttpContext.Current.Request.FilePath];

                    if (String.IsNullOrEmpty(baseUrl))
                    {
                        // Each different filepath (multiboard) will specify a BaseUrl key in their own web.config in their directory.
                        if (!String.IsNullOrEmpty(YAF.Classes.Config.BaseUrlFromWCM))
                            baseUrl = YAF.Classes.Config.BaseUrlFromWCM;
                        // If BaseUrl isn't found, use Root.
                        else if (!String.IsNullOrEmpty(YAF.Classes.Config.Root))
                            baseUrl = YAF.Classes.Config.Root;
                        // If Root isn't found, use the current application path.
                        else
                            baseUrl = HttpContext.Current.Request.ApplicationPath;

                        if (baseUrl.StartsWith("~"))
                        {
                            // transform with application path...
                            baseUrl = baseUrl.Replace("~", HttpContext.Current.Request.ApplicationPath);
                        }

                        if (baseUrl.StartsWith("//"))
                        {
                            // remove extra slash
                            baseUrl = baseUrl.Substring(1, baseUrl.Length - 1);
                        }

                        if (baseUrl.EndsWith("/"))
                        {
                            // remove ending slash...
                            baseUrl = baseUrl.Substring(0, baseUrl.LastIndexOf('/'));
                        }

                        // remove redundant slashes...
                        while (baseUrl.Contains("//"))
                        {
                            baseUrl = baseUrl.Replace("//", "/");
                        }

                        // save to cache
                        _baseUrls[HttpContext.Current.Request.FilePath] = baseUrl;
                    }
                }
                catch (Exception)
                {
                    baseUrl = HttpContext.Current.Request.ApplicationPath;
                }

                return baseUrl;
			}
		}

		static public string RootUrl
		{
			get
			{
				if ( _fileRoot != null )
				{
					if ( _fileRoot.Contains( "//" ) )
					{
						_fileRoot = null;
					}
					else
					{
						return _fileRoot;
					}
				}

				try
				{
					_fileRoot = HttpContext.Current.Request.ApplicationPath;

					if ( !_fileRoot.EndsWith( "/" ) ) _fileRoot += "/";

					if ( YAF.Classes.Config.Root != null )
					{
						// use specified root
						_fileRoot = YAF.Classes.Config.Root;

						if ( _fileRoot.StartsWith( "~" ) )
						{
							// transform with application path...
							_fileRoot = _fileRoot.Replace( "~", HttpContext.Current.Request.ApplicationPath );
						}

						if ( _fileRoot [0] != '/' ) _fileRoot = _fileRoot.Insert( 0, "/" );
					}
					else if ( YAF.Classes.Config.IsDotNetNuke )
					{
						_fileRoot += "DesktopModules/YetAnotherForumDotNet/";
					}
					else if ( YAF.Classes.Config.IsRainbow )
					{
						_fileRoot += "DesktopModules/Forum/";
					}
					else if ( YAF.Classes.Config.IsPortal )
					{
						_fileRoot += "Modules/Forum/";
					}

					if ( !_fileRoot.EndsWith( "/" ) ) _fileRoot += "/";

					// remove redundant slashes...
					while ( _fileRoot.Contains( "//" ) )
					{
						_fileRoot = _fileRoot.Replace( "//", "/" );
					}
				}
				catch ( Exception )
				{
					_fileRoot = "/";
				}

				return _fileRoot;
			}
		}
	}
}
