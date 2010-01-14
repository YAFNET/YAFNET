/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Xml;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
  /// <summary>
  /// The yaf localization.
  /// </summary>
  public class YafLocalization
  {
    /// <summary>
    /// The _culture.
    /// </summary>
    private CultureInfo _culture = null;

    /// <summary>
    /// The _default locale.
    /// </summary>
    private Localizer _defaultLocale = null;

    /// <summary>
    /// The _language file name.
    /// </summary>
    private string _languageFileName;

    /// <summary>
    /// The _localizer.
    /// </summary>
    private Localizer _localizer = null;

    /// <summary>
    /// The _trans page.
    /// </summary>
    private string _transPage = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="YafLocalization"/> class.
    /// </summary>
    public YafLocalization()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YafLocalization"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    public YafLocalization(string transPage)
      : this()
    {
      TransPage = transPage;
    }

    /// <summary>
    /// Gets a value indicating whether TranslationLoaded.
    /// </summary>
    public bool TranslationLoaded
    {
      get
      {
        return this._localizer != null;
      }
    }

    /// <summary>
    /// What section of the xml is used to translate this page
    /// </summary>
    public string TransPage
    {
      get
      {
        // if ( m_transPage != null )
        return this._transPage;

        // throw new ApplicationException( string.Format( "Missing TransPage property for {0}", GetType() ) );
      }

      set
      {
        this._transPage = value;
      }
    }

    /// <summary>
    /// Gets LanguageCode.
    /// </summary>
    public string LanguageCode
    {
      get
      {
        if (this._localizer != null)
        {
          return this._localizer.LanguageCode;
        }

        return LoadTranslation();
      }
    }

    /// <summary>
    /// Gets Culture.
    /// </summary>
    public CultureInfo Culture
    {
      get
      {
        if (this._culture != null)
        {
          return this._culture;
        }
        else if (this._localizer == null)
        {
          LoadTranslation();
          return this._culture;
        }

        // fall back to current culture if there is some error
        return CultureInfo.CurrentCulture;
      }
    }

    /// <summary>
    /// Gets LanguageFileName.
    /// </summary>
    public string LanguageFileName
    {
      get
      {
        return this._languageFileName;
      }
    }

    /// <summary>
    /// Formats a localized string -- but verifies the parameter count matches
    /// </summary>
    /// <param name="text">
    /// </param>
    /// <param name="args">
    /// </param>
    /// <returns>
    /// The get text formatted.
    /// </returns>
    public string GetTextFormatted(string text, params object[] args)
    {
      string localizedText = GetText(TransPage, text);

      /* get the localization string parameter count...
			int iParamCount = 0;
			for (; iParamCount<10; iParamCount++)
			{
				if (!localizedText.Contains("{" + iParamCount.ToString()))
				{
					break;
				}
			}
#if DEBUG
					localizedText = String.Format( "[INVALID: {1}.{0} -- NEEDS {2} PARAMETERS HAS {3}]", text.ToUpper(), TransPage.ToUpper(), args.Length, i );
#endif
					// inform that the value is wrong to the admin and don't format the string...
					Data.DB.eventlog_create(YafContext.Current.PageUserID, TransPage.ToLower() + ".ascx", String.Format("Not enough parameters for localization entry {1}.{0} -- Needs {2} parameters, has {3}.", text.ToUpper(), TransPage.ToUpper(), args.Length, i), Data.EventLogTypes.Warning);
			*/
      var values = new object[10];

      args.CopyTo(values, 0);
      for (int i = args.Length; i < 10; i++)
      {
        values.SetValue(String.Format("[INVALID: {1}.{0} -- EMPTY PARAM #{2}]", text.ToUpper(), TransPage.ToUpper(), i), i);
      }

      // run format command...
      localizedText = String.Format(localizedText, values);

      return localizedText;
    }

    /// <summary>
    /// The get text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    public string GetText(string text)
    {
      return GetText(TransPage, text);
    }

    /// <summary>
    /// The load translation.
    /// </summary>
    /// <param name="fileName">
    /// The file name.
    /// </param>
    /// <returns>
    /// The load translation.
    /// </returns>
    public string LoadTranslation(string fileName)
    {
      if (this._localizer != null)
      {
        return this._localizer.LanguageCode;
      }

#if !DEBUG
			if ( _localizer == null && HttpContext.Current.Cache ["Localizer." + fileName] != null )
				_localizer = ( Localizer ) HttpContext.Current.Cache ["Localizer." + fileName];
#endif
      if (this._localizer == null)
      {
        this._localizer = new Localizer(HttpContext.Current.Server.MapPath(String.Format("{0}languages/{1}", YafForumInfo.ForumFileRoot, fileName)));
#if !DEBUG
				HttpContext.Current.Cache ["Localizer." + fileName] = _localizer;
#endif
      }

      // If not using default language load that too
      if (fileName.ToLower() != "english.xml")
      {
#if !DEBUG
        if ( _defaultLocale == null && HttpContext.Current.Cache ["DefaultLocale"] != null )
          _defaultLocale = ( Localizer ) HttpContext.Current.Cache ["DefaultLocale"];
#endif

        if (this._defaultLocale == null)
        {
          this._defaultLocale = new Localizer(HttpContext.Current.Server.MapPath(String.Format("{0}languages/english.xml", YafForumInfo.ForumFileRoot)));
#if !DEBUG
          HttpContext.Current.Cache ["DefaultLocale"] = _defaultLocale;
#endif
        }
      }

      try
      {
        // try to load culture info defined in localization file
        this._culture = new CultureInfo(this._localizer.LanguageCode);
      }
      catch
      {
        // if it's wrong, fall back to current culture
        this._culture = CultureInfo.CurrentCulture;
      }

      this._languageFileName = fileName.ToLower();

      return this._localizer.LanguageCode;
    }

    /// <summary>
    /// The load translation.
    /// </summary>
    /// <returns>
    /// The load translation.
    /// </returns>
    public string LoadTranslation()
    {
      if (this._localizer != null)
      {
        return this._localizer.LanguageCode;
      }

      string filename = null;

      if (YafContext.Current.PageIsNull() || YafContext.Current.Page["LanguageFile"] == DBNull.Value || !YafContext.Current.BoardSettings.AllowUserLanguage)
      {
        filename = YafContext.Current.BoardSettings.Language;
      }
      else
      {
        filename = YafContext.Current.LanguageFile;
      }

      if (filename == null)
      {
        filename = "english.xml";
      }

      return LoadTranslation(filename);
    }

    /// <summary>
    /// The get localized text internal.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get localized text internal.
    /// </returns>
    protected string GetLocalizedTextInternal(string page, string tag)
    {
      string localizedText;

      LoadTranslation();

      this._localizer.SetPage(page);
      this._localizer.GetText(tag, out localizedText);

      // If not default language, try to use that instead
      if (localizedText == null && this._defaultLocale != null)
      {
        this._defaultLocale.SetPage(page);
        this._defaultLocale.GetText(tag, out localizedText);
        if (localizedText != null)
        {
          localizedText = '[' + localizedText + ']';
        }
      }

      return localizedText;
    }

    /// <summary>
    /// The get nodes using query.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tagQuery">
    /// The tag query.
    /// </param>
    /// <returns>
    /// </returns>
    public List<XmlNode> GetNodesUsingQuery(string page, string tagQuery)
    {
      LoadTranslation();

      this._localizer.SetPage(page);
      return this._localizer.GetNodesUsingQuery(tagQuery);
    }

    /// <summary>
    /// The get text exists.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get text exists.
    /// </returns>
    public bool GetTextExists(string page, string tag)
    {
      return !String.IsNullOrEmpty(GetLocalizedTextInternal(page, tag));
    }

    /// <summary>
    /// The get text.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    public string GetText(string page, string tag)
    {
      string localizedText = GetLocalizedTextInternal(page, tag);

      if (localizedText == null)
      {
#if !DEBUG
        string filename = string.Empty;

        if ( YafContext.Current.PageIsNull() ||
             YafContext.Current.LanguageFile == string.Empty ||
             !YafContext.Current.BoardSettings.AllowUserLanguage )
        {
          filename = YafContext.Current.BoardSettings.Language;
        }
        else
        {
          filename = YafContext.Current.LanguageFile;
        }

        if ( filename == string.Empty ) filename = "english.xml";

        HttpContext.Current.Cache.Remove( "Localizer." + filename );
#endif
        DB.eventlog_create(
          YafContext.Current.PageUserID, 
          page.ToLower() + ".ascx", 
          String.Format("Missing Translation For {1}.{0}", tag.ToUpper(), page.ToUpper()), 
          EventLogTypes.Error);
        return String.Format("[{1}.{0}]", tag.ToUpper(), page.ToUpper());
      }

      localizedText = localizedText.Replace("[b]", "<b>");
      localizedText = localizedText.Replace("[/b]", "</b>");
      return localizedText;
    }

    /// <summary>
    /// Formats string using current culture.
    /// </summary>
    /// <param name="format">
    /// Format string.
    /// </param>
    /// <param name="args">
    /// Parameters used in format string.
    /// </param>
    /// <returns>
    /// Formatted string.
    /// </returns>
    /// <remarks>
    /// If current localization culture is neutral, it's not used in formatting.
    /// </remarks>
    public string FormatString(string format, params object[] args)
    {
      if (Culture.IsNeutralCulture)
      {
        return String.Format(format, args);
      }
      else
      {
        return String.Format(Culture, format, args);
      }
    }

    /// <summary>
    /// Formats date using given formatting string and current culture.
    /// </summary>
    /// <param name="format">
    /// Format string.
    /// </param>
    /// <param name="date">
    /// Date to format.
    /// </param>
    /// <returns>
    /// Formatted string.
    /// </returns>
    /// <remarks>
    /// If current localization culture is neutral, it's not used in formatting.
    /// </remarks>
    public string FormatDateTime(string format, DateTime date)
    {
      if (Culture.IsNeutralCulture)
      {
        return date.ToString(format);
      }
      else
      {
        return date.ToString(format, Culture);
      }
    }
  }
}