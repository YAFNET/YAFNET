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
    using System.IO;
    using System.Linq;

    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

	/// <summary>
	/// Summary description for Localizer.
	/// </summary>
	public class Localizer
	{
		#region Constants and Fields

		/// <summary>
		/// The _current culture.
		/// </summary>
		private CultureInfo _currentCulture;

		/// <summary>
		///   The _current page.
		/// </summary>
		private string _currentPage = string.Empty;

		/// <summary>
		///   The _file name.
		/// </summary>
		private string _fileName = string.Empty;

		/// <summary>
		/// The _localization resources.
		/// </summary>
		private LanguageResources _localizationLanguageResources;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///   Initializes a new instance of the <see cref = "Localizer" /> class.
		/// </summary>
		public Localizer()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Localizer"/> class.
		/// </summary>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		public Localizer(string fileName)
		{
			this._fileName = fileName;
			this.LoadFile();
			this.InitCulture();
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets LanguageCode.
		/// </summary>
		public CultureInfo CurrentCulture
		{
			get
			{
				return this._currentCulture;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// The get nodes using query.
		/// </summary>
		/// <param name="predicate">
		/// The predicate.
		/// </param>
		/// <returns>
		/// The Nodes.
		/// </returns>
		/// <exception cref="Exception">
		/// </exception>
		public IEnumerable<LanuageResourcesPageResource> GetNodesUsingQuery(Func<LanuageResourcesPageResource, bool> predicate)
		{
			var pagePointer =
				this._localizationLanguageResources.page.FirstOrDefault(p => p.name.ToUpper().Equals(this._currentPage));

			return pagePointer != null ? pagePointer.Resource.Where(predicate) : this._localizationLanguageResources.page.SelectMany(p => p.Resource).Where(predicate);
		}

		/// <summary>
		/// The get nodes using query.
		/// </summary>
		/// <param name="predicate">
		/// The predicate.
		/// </param>
		/// <returns>
		/// The Nodes.
		/// </returns>
		/// <exception cref="Exception">
		/// </exception>
		public IEnumerable<LanuageResourcesPageResource> GetCountryNodesUsingQuery(Func<LanuageResourcesPageResource, bool> predicate)
		{
			var pagePointer =
				this._localizationLanguageResources.page.FirstOrDefault(p => p.name.ToUpper().Equals(this._currentPage));

			return pagePointer != null ? pagePointer.Resource.Where(predicate) : this._localizationLanguageResources.page.SelectMany(p => p.Resource).Where(predicate);
		}

		/// <summary>
		/// The get nodes using query.
		/// </summary>
		/// <param name="predicate">
		/// The predicate.
		/// </param>
		/// <returns>
		/// The Nodes.
		/// </returns>
		/// <exception cref="Exception">
		/// </exception>
		public IEnumerable<LanuageResourcesPageResource> GetRegionNodesUsingQuery(Func<LanuageResourcesPageResource, bool> predicate)
		{
			var pagePointer =
				this._localizationLanguageResources.page.FirstOrDefault(p => p.name.ToUpper().Equals(this._currentPage));

			return pagePointer != null ? pagePointer.Resource.Where(predicate) : this._localizationLanguageResources.page.SelectMany(p => p.Resource).Where(predicate);
		}

		/// <summary>
		/// The get text.
		/// </summary>
		/// <param name="tag">
		/// The tag.
		/// </param>
		/// <param name="localizedText">
		/// The localized text.
		/// </param>
		/// <exception cref="Exception">
		/// </exception>
		public void GetText(string tag, out string localizedText)
		{
			// default the out parameters
			localizedText = string.Empty;

			tag = tag.ToUpper(); //ToUpper(this._currentCulture);

			var pagePointer =
				this._localizationLanguageResources.page.FirstOrDefault(p => p.name.Equals(this._currentPage));

			LanuageResourcesPageResource pageResource = null;

			if (pagePointer != null)
			{
				pageResource = pagePointer.Resource.FirstOrDefault(r => r.tag.Equals(tag));
			}

			if (pageResource == null)
			{
				// attempt to find the tag anywhere...
				pageResource =
					this._localizationLanguageResources.page.SelectMany(p => p.Resource).FirstOrDefault(r => r.tag.Equals(tag));
			}

			if (pageResource != null && pageResource.Value.IsSet())
			{
				localizedText = pageResource.Value;
			}
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
			string text;

			this.SetPage(page);
			this.GetText(tag, out text);

			return text;
		}

		/// <summary>
		/// The load file.
		/// </summary>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		public void LoadFile(string fileName)
		{
			this._fileName = fileName;
			this.LoadFile();
		}

		/// <summary>
		/// The set page.
		/// </summary>
		/// <param name="page">
		/// The page.
		/// </param>
		public void SetPage(string page)
		{
			if (this._currentPage == page)
			{
				return;
			}

			if (page.IsNotSet())
			{
				page = "DEFAULT";
			}

			this._currentPage = page.ToUpper();
		}

		#endregion

		#region Methods

		/// <summary>
		/// The init culture.
		/// </summary>
		private void InitCulture()
		{
			if (!YafContext.Current.Get<StartupInitializeDb>().Initialized)
			{
				return;
			}

			var langCode = this.CurrentCulture.TwoLetterISOLanguageName;

			// vzrus: Culture code is missing for a user until he saved his profile.
			// First set it to board culture
			try
			{
				if (langCode.Equals(YafContext.Current.BoardSettings.Culture.Substring(0, 2)))
				{
					this._currentCulture = new CultureInfo(YafContext.Current.BoardSettings.Culture);
				}
			}
			catch (Exception)
			{
				this._currentCulture = new CultureInfo(YafContext.Current.BoardSettings.Culture);
			}

			string cultureUser = YafContext.Current.CultureUser;

			if (!cultureUser.IsSet())
			{
				return;
			}



			if (cultureUser.Trim().Substring(0, 2).Equals(langCode))
			{
				this._currentCulture = new CultureInfo(cultureUser.Trim().Length > 5 ? cultureUser.Trim().Substring(0, 2) : cultureUser.Trim());
			}
		}

		/// <summary>
		/// The load file.
		/// </summary>
		/// <exception cref="ApplicationException"></exception>
		private void LoadFile()
		{
			if (this._fileName == string.Empty || !File.Exists(this._fileName))
			{
				throw new ApplicationException("Invalid language file {0}".FormatWith(this._fileName));
			}

			this._localizationLanguageResources = new LoadSerializedXmlFile<LanguageResources>().FromFile(
				this._fileName,
				"LOCALIZATIONFILE{0}".FormatWith(this._fileName),
				(r) =>
				{
					// transform the page and tag name ToUpper...
					r.page.ForEach(p => p.name = p.name.ToUpper());
					r.page.ForEach(p => p.Resource.ForEach(i => i.tag = i.tag.ToUpper()));
				});

			var userLanguageCode = this._localizationLanguageResources.code.IsSet()
																	 ? this._localizationLanguageResources.code.Trim()
																	 : "en-US";

			if (userLanguageCode.Length > 5)
			{
				userLanguageCode = this._localizationLanguageResources.code.Trim().Substring(0, 2);
			}

			this._currentCulture =
				new CultureInfo(userLanguageCode);
		}

		#endregion
	}
}