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
using System;
using System.Globalization;
using System.Threading;
using YAF.Classes.Data;

namespace YAF.Classes.Core
{
  public interface ILocalizationHandler
  {
    /// <summary>
    /// Gets or sets Localization.
    /// </summary>
    YafLocalization Localization { get; set; }

    /// <summary>
    /// Current TransPage for Localization
    /// </summary>
    string TranslationPage { get; set; }

    /// <summary>
    /// The before init.
    /// </summary>
    event EventHandler<EventArgs> BeforeInit;

    /// <summary>
    /// The after init.
    /// </summary>
    event EventHandler<EventArgs> AfterInit;
  }

  /// <summary>
  /// The localization handler.
  /// </summary>
  public class LocalizationHandler : ILocalizationHandler
  {
    /// <summary>
    /// The _init culture.
    /// </summary>
    private bool _initCulture = false;

    /// <summary>
    /// The _init localization.
    /// </summary>
    private bool _initLocalization = false;

    /// <summary>
    /// The _localization.
    /// </summary>
    private YafLocalization _localization = null;

    /// <summary>
    /// The _trans page.
    /// </summary>
    private string _transPage = string.Empty;

    /// <summary>
    /// Gets or sets Localization.
    /// </summary>
    public YafLocalization Localization
    {
      get
      {
        if (!this._initLocalization)
        {
          InitLocalization();
        }

        if (!this._initCulture)
        {
          InitCulture();
        }

        return this._localization;
      }

      set
      {
        this._localization = value;
        this._initLocalization = value != null;
        this._initCulture = value != null;
      }
    }

    /// <summary>
    /// Current TransPage for Localization
    /// </summary>
    public string TranslationPage
    {
      get
      {
        return this._transPage;
      }

      set
      {
        if (value != this._transPage)
        {
          this._transPage = value;

          if (this._initLocalization)
          {
            // re-init localization
            Localization = null;
          }
        }
      }
    }

    /// <summary>
    /// The before init.
    /// </summary>
    public event EventHandler<EventArgs> BeforeInit;

    /// <summary>
    /// The after init.
    /// </summary>
    public event EventHandler<EventArgs> AfterInit;

    /// <summary>
    /// Set up the localization
    /// </summary>
    protected void InitLocalization()
    {
      if (!this._initLocalization)
      {
        if (BeforeInit != null)
        {
          BeforeInit(this, new EventArgs());
        }

        Localization = new YafLocalization(TranslationPage);

        if (AfterInit != null)
        {
          AfterInit(this, new EventArgs());
        }
      }
    }

    /// <summary>
    /// Set the culture and UI culture to the browser's accept language
    /// </summary>
    protected void InitCulture()
    {
      if (!this._initCulture)
      {
        try
        {
          string cultureCode = string.Empty;

          /*string [] tmp = YafContext.Current.Get<HttpRequestBase>().UserLanguages;
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
          cultureCode = this._localization.LanguageCode;

          Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureCode);
          Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
        }

#if DEBUG
        catch (Exception ex)
        {
          DB.eventlog_create(YafContext.Current.PageUserID, this, ex);
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
        this._initCulture = true;
      }
    }
  }
}