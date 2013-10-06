/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Core.Services.Localization
{
  #region Using

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

  /// <summary>
  /// The yaf localization.
  /// </summary>
  public class YafLocalization : ILocalization
  {
    #region Constants and Fields

    /// <summary>
    ///   The _culture.
    /// </summary>
    private CultureInfo _culture;

    /// <summary>
    ///   The _default locale.
    /// </summary>
    private Localizer _defaultLocale;

    /// <summary>
    ///   The _language file name.
    /// </summary>
    private string _languageFileName;

    /// <summary>
    ///   The _localizer.
    /// </summary>
    private Localizer _localizer;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafLocalization"/> class. 
    ///   Initializes a new instance of the <see cref="YAF.Core"/> class.
    /// </summary>
    public YafLocalization()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YafLocalization"/> class. 
    /// Initializes a new instance of the <see cref="YAF.Core"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    public YafLocalization([NotNull] string transPage)
      : this()
    {
      this.TransPage = transPage;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Culture.
    /// </summary>
    public CultureInfo Culture
    {
      get
      {
        if (this._culture != null)
        {
          return this._culture;
        }

        if (this._localizer == null)
        {
          this._culture = this.LoadTranslation();
          return this._culture;
        }

        // fall back to current culture if there is some error
        return CultureInfo.CurrentCulture;
      }
    }

    /// <summary>
    ///   Gets LanguageCode.
    /// </summary>
    [NotNull]
    public string LanguageCode
    {
      get
      {
        return this._localizer != null
                 ? this._localizer.CurrentCulture.TwoLetterISOLanguageName
                 : this.LoadTranslation().TwoLetterISOLanguageName;
      }
    }

    /// <summary>
    ///   Gets LanguageFileName.
    /// </summary>
    public string LanguageFileName
    {
      get
      {
        return this._languageFileName;
      }
    }

    /// <summary>
    ///   Gets or sets What section of the xml is used to translate this page
    /// </summary>
    public string TransPage { get; set; }

    /// <summary>
    ///   Gets a value indicating whether TranslationLoaded.
    /// </summary>
    public bool TranslationLoaded
    {
      get
      {
        return this._localizer != null;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region ILocalization

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
    [NotNull]
    public string FormatDateTime([NotNull] string format, DateTime date)
    {
      return this.Culture.IsNeutralCulture ? date.ToString(format) : date.ToString(format, this.Culture);
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
    public string FormatString([NotNull] string format, [NotNull] params object[] args)
    {
      return this.Culture.IsNeutralCulture ? format.FormatWith(args) : string.Format(this.Culture, format, args);
    }

    /// <summary>
    /// The get nodes using query.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="predicate">
    /// The predicate.
    /// </param>
    /// <returns>
    /// The Nodes
    /// </returns>
    public IEnumerable<LanuageResourcesPageResource> GetNodesUsingQuery([NotNull] string page, [NotNull] Func<LanuageResourcesPageResource, bool> predicate)
    {
      this.LoadTranslation();

      this._localizer.SetPage(page);
      return this._localizer.GetNodesUsingQuery(predicate);
    }

    /// <summary>
    /// The get nodes using query.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="predicate">
    /// The predicate.
    /// </param>
    /// <returns>
    /// The Nodes
    /// </returns>
    public IEnumerable<LanuageResourcesPageResource> GetCountryNodesUsingQuery([NotNull] string page, [NotNull] Func<LanuageResourcesPageResource, bool> predicate)
    {
        this.LoadTranslation();

        this._localizer.SetPage(page);
        return this._localizer.GetCountryNodesUsingQuery(predicate);
    }

    /// <summary>
    /// The get nodes using query.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="predicate">
    /// The predicate.
    /// </param>
    /// <returns>
    /// The Nodes
    /// </returns>
    public IEnumerable<LanuageResourcesPageResource> GetRegionNodesUsingQuery([NotNull] string page, [NotNull] Func<LanuageResourcesPageResource, bool> predicate)
    {
        this.LoadTranslation();

        this._localizer.SetPage(page);
        return this._localizer.GetCountryNodesUsingQuery(predicate);
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
    public string GetText([NotNull] string text)
    {
      CodeContracts.VerifyNotNull(text, "text");

      return this.GetText(this.TransPage, text);
    }

		private static readonly Regex _rgxBegin = new Regex(@"(?<!\[noparse\])(?<inner>\[b\])", RegexOptions.Compiled);
		private static readonly Regex _rgxEnd = new Regex(@"(?<inner>\[/b\])(?!\[/noparse\])", RegexOptions.Compiled);

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
    public string GetText([NotNull] string page, [NotNull] string tag)
    {
      string localizedText = this.GetLocalizedTextInternal(page, tag);

      if (localizedText == null)
      {
#if !DEBUG
        string filename;

        if ( YafContext.Current.PageIsNull() ||
             YafContext.Current.LanguageFile == string.Empty ||
             YafContext.Current.LanguageFile == string.Empty ||
             !YafContext.Current.Get<YafBoardSettings>().AllowUserLanguage)
        {
            filename = YafContext.Current.Get<YafBoardSettings>().Language;
        }
        else
        {
          filename = YafContext.Current.LanguageFile;
        }

        if ( filename == string.Empty ) filename = "english.xml";

        HttpContext.Current.Cache.Remove( "Localizer.{0}".FormatWith(filename) );
#endif
          YafContext.Current.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        page.ToLower() + ".ascx",
                        "Missing Translation For {1}.{0}".FormatWith(tag.ToUpper(), page.ToUpper()),
                        EventLogTypes.Error);

        return "[{1}.{0}]".FormatWith(tag.ToUpper(), page.ToUpper());
      }
      
      localizedText = _rgxBegin.Replace(localizedText, "<strong>");
      localizedText = _rgxEnd.Replace(localizedText, "</strong>");

      localizedText = localizedText.Replace("[noparse]", string.Empty);
      localizedText = localizedText.Replace("[/noparse]", string.Empty);

      return localizedText;
    }

    /// <summary>
    /// The get text, with a Specific Language.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="languageFile">
    /// The Language file
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    public string GetText([NotNull] string page, [NotNull] string tag, [NotNull] string languageFile)
    {
      string localizedText;

      if (!string.IsNullOrEmpty(languageFile))
      {
        var localization = new YafLocalization();
        localization.LoadTranslation(languageFile);
        localizedText = localization.GetText(page, tag);
      }
      else
      {
        localizedText = this.GetLocalizedTextInternal(page, tag);
      }

      if (localizedText == null)
      {
#if !DEBUG
            string filename;

            if (!string.IsNullOrEmpty(languageFile))
            {
                filename = languageFile;
            }
            else
            {
                if (YafContext.Current.PageIsNull() ||
                 YafContext.Current.LanguageFile == string.Empty ||
                 YafContext.Current.LanguageFile == string.Empty ||
                 !YafContext.Current.Get<YafBoardSettings>().AllowUserLanguage)
                {
                    filename = YafContext.Current.Get<YafBoardSettings>().Language;
                }
                else
                {
                    filename = YafContext.Current.LanguageFile;
                }
            }
            

            if (filename == string.Empty) filename = "english.xml";

            HttpContext.Current.Cache.Remove("Localizer." + filename);
#endif
          YafContext.Current.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        page.ToLower() + ".ascx",
                        "Missing Translation For {1}.{0}".FormatWith(tag.ToUpper(), page.ToUpper()),
                        EventLogTypes.Error);

        return "[{1}.{0}]".FormatWith(tag.ToUpper(), page.ToUpper());
      }

      localizedText = localizedText.Replace("[b]", "<b>");
      localizedText = localizedText.Replace("[/b]", "</b>");

      return localizedText;
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
    /// Returns value if text exists.
    /// </returns>
    public bool GetTextExists([NotNull] string page, [NotNull] string tag)
    {
      return this.GetLocalizedTextInternal(page, tag).IsSet();
    }

    /// <summary>
    /// Formats a localized string -- but verifies the parameter count matches
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// Returns the Formatted Text
    /// </returns>
    public string GetTextFormatted([NotNull] string text, [NotNull] params object[] args)
    {
      CodeContracts.VerifyNotNull(text, "text");
      CodeContracts.VerifyNotNull(args, "args");

      string localizedText = this.GetText(this.TransPage, text);

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
					 inform that the value is wrong to the admin and don't format the string...
					Data.DB.eventlog_create(YafContext.Current.PageUserID, TransPage.ToLower() + ".ascx", String.Format("Not enough parameters for localization entry {1}.{0} -- Needs {2} parameters, has {3}.", text.ToUpper(), TransPage.ToUpper(), args.Length, i), Data.EventLogTypes.Warning);
			*/

      int arraySize = Math.Max(args.Length, 10);
      var copiedArgs = new object[arraySize];
      args.CopyTo(copiedArgs, 0);

      for (int arrayIndex = args.Length; arrayIndex < arraySize; arrayIndex++)
      {
        copiedArgs[arrayIndex] = "[INVALID: {1}.{0} -- EMPTY PARAM #{2}]".FormatWith(
          text.ToUpper(), this.TransPage.IsNotSet() ? "NULL" : this.TransPage.ToUpper(), arrayIndex);
      }

      // run format command...
      localizedText = localizedText.FormatWith(copiedArgs);

      return localizedText;
    }

    /// <summary>
    /// The load translation.
    /// </summary>
    /// <param name="fileName">
    /// The file name.
    /// </param>
    /// <returns>
    /// Returns the translation Culture Info.
    /// </returns>
    public CultureInfo LoadTranslation([NotNull] string fileName)
    {
      CodeContracts.VerifyNotNull(fileName, "fileName");

      if (this._localizer != null)
      {
        return this._localizer.CurrentCulture;
      }

#if !DEBUG
			if ( _localizer == null && HttpContext.Current.Cache ["Localizer." + fileName] != null )
				_localizer = ( Localizer ) HttpContext.Current.Cache ["Localizer." + fileName];
#endif
      if (this._localizer == null)
      {
        this._localizer =
          new Localizer(
            HttpContext.Current.Server.MapPath(
              "{0}languages/{1}".FormatWith(YafForumInfo.ForumServerFileRoot, fileName)));

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
          this._defaultLocale =
            new Localizer(
              HttpContext.Current.Server.MapPath(
                "{0}languages/english.xml".FormatWith(YafForumInfo.ForumServerFileRoot)));
#if !DEBUG
          HttpContext.Current.Cache ["DefaultLocale"] = _defaultLocale;
#endif
        }
      }

      try
      {
        // try to load culture info defined in localization file
        this._culture = this._localizer.CurrentCulture;
      }
      catch
      {
        // if it's wrong, fall back to current culture
        this._culture = CultureInfo.CurrentCulture;
      }

      this._languageFileName = fileName.ToLower();

      return this._culture;
    }

    /// <summary>
    /// The load translation.
    /// </summary>
    /// <returns>
    /// The load translation.
    /// </returns>
    public CultureInfo LoadTranslation()
    {
      if (this._localizer != null)
      {
        return this._localizer.CurrentCulture;
      }

      string filename;

      if (YafContext.Current.PageIsNull() || YafContext.Current.Page["LanguageFile"] == null ||
          !YafContext.Current.Get<YafBoardSettings>().AllowUserLanguage)
      {
          filename = YafContext.Current.Get<YafBoardSettings>().Language;
      }
      else
      {
        filename = YafContext.Current.LanguageFile;
      }

      if (filename == null)
      {
        filename = "english.xml";
      }

      return this.LoadTranslation(filename);
    }

    #endregion

    #endregion

    #region Methods

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
    /// Returns the localized text internal.
    /// </returns>
    protected string GetLocalizedTextInternal([NotNull] string page, [NotNull] string tag)
    {
      string localizedText;

      this.LoadTranslation();

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

    #endregion
  }
}