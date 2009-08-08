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
using System.Globalization;
using System.Threading;

namespace YAF.Classes.Core
{
	public class LocalizationHandler
	{
		private YafLocalization _localization = null;
		private bool _initCulture = false;
		private bool _initLocalization = false;

		public YafLocalization Localization
		{
			get
			{
				if (!_initLocalization) InitLocalization();
				if (!_initCulture) InitCulture();
				return _localization;
			}
			set
			{
				_localization = value;
				_initLocalization = (value != null);
				_initCulture = (value != null);
			}
		}

		private string _transPage = string.Empty;
		/// <summary>
		/// Current TransPage for Localization
		/// </summary>
		public string TranslationPage
		{
			get
			{
				return _transPage;
			}
			set
			{
				if (value != _transPage)
				{
					_transPage = value;

					if (_initLocalization)
					{
						// re-init localization
						this.Localization = null;
					}
				}
			}
		}

		public event EventHandler<EventArgs> BeforeInit;
		public event EventHandler<EventArgs> AfterInit;

		/// <summary>
		/// Set up the localization
		/// </summary>
		protected void InitLocalization()
		{
			if (!_initLocalization)
			{
				if (BeforeInit != null) BeforeInit(this, new EventArgs());

				this.Localization = new YafLocalization(this.TranslationPage);

				if (AfterInit != null) AfterInit(this, new EventArgs());
			}
		}

		/// <summary>
		/// Set the culture and UI culture to the browser's accept language
		/// </summary>
		protected void InitCulture()
		{
			if (!_initCulture)
			{
				try
				{
					string cultureCode = "";

					/*string [] tmp = HttpContext.Current.Request.UserLanguages;
					if ( tmp != null )
					{
						cultureCode = tmp [0];
						if ( cultureCode.IndexOf( ';' ) >= 0 )
						{
							cultureCode = cultureCode.Substring( 0, cultureCode.IndexOf( ';' ) ).Replace( '_', '-' );
						}
					}
					else
					{
						cultureCode = "en-US";
					}*/

					cultureCode = _localization.LanguageCode;

					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureCode);
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
				}
#if DEBUG
				catch (Exception ex)
				{
					Data.DB.eventlog_create(YafContext.Current.PageUserID, this, ex);
					throw new ApplicationException("Error getting User Language." + Environment.NewLine + ex.ToString());
				}
#else
				catch ( Exception )
				{
					// set to default...
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture( "en-US" );
					Thread.CurrentThread.CurrentUICulture = new CultureInfo( "en-US" );
				}
#endif
				// mark as setup...
				_initCulture = true;
			}
		}
	}
}
