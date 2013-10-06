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

namespace YAF.Core
{
    #region Using

    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The yaf theme.
    /// </summary>
    public class YafTheme : ITheme
    {
        #region Constants and Fields

        /// <summary>
        ///   The _theme file.
        /// </summary>
        private string _themeFile;

        /// <summary>
        /// The _theme resources.
        /// </summary>
        private ThemeResources _themeResources;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="YafTheme" /> class.
        /// </summary>
        public YafTheme()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YafTheme"/> class.
        /// </summary>
        /// <param name="newThemeFile">
        /// The new theme file. 
        /// </param>
        public YafTheme([NotNull] string newThemeFile)
        {
            CodeContracts.VerifyNotNull(newThemeFile, "newThemeFile");

            this.ThemeFile = newThemeFile;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether LogMissingThemeItem.
        /// </summary>
        public bool LogMissingThemeItem { get; set; }

        /// <summary>
        ///   Gets ThemeDir.
        /// </summary>
        public string ThemeDir
        {
            get
            {
                this.LoadThemeFile();
                return "{0}{1}/{2}/".FormatWith(
                    YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Themes, this._themeResources.dir);
            }
        }

        /// <summary>
        ///   Gets or sets the current Theme File
        /// </summary>
        public string ThemeFile
        {
            get
            {
                return this._themeFile;
            }

            set
            {
                if (this._themeFile == value)
                {
                    return;
                }

                if (!IsValidTheme(value))
                {
                    return;
                }

                this._themeFile = value;
                this._themeResources = null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Basic testing of the theme's validity...
        /// </summary>
        /// <param name="themeFile">The theme file.</param>
        /// <returns>
        /// The is valid theme.
        /// </returns>
        public static bool IsValidTheme([NotNull] string themeFile)
        {
            CodeContracts.VerifyNotNull(themeFile, "themeFile");

            if (themeFile.IsNotSet())
            {
                return false;
            }

            themeFile = themeFile.Trim().ToLower();

            return themeFile.Length != 0 && (themeFile.EndsWith(".xml") && File.Exists(GetMappedThemeFile(themeFile)));
        }

        /// <summary>
        /// Gets full path to the given theme file.
        /// </summary>
        /// <param name="filename">
        /// Short name of theme file. 
        /// </param>
        /// <returns>
        /// The build theme path. 
        /// </returns>
        public string BuildThemePath([NotNull] string filename)
        {
            CodeContracts.VerifyNotNull(filename, "filename");

            return this.ThemeDir + filename;
        }

        /// <summary>
        /// Get the Item.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The item.
        /// </returns>
        public string GetItem([NotNull] string page, [NotNull] string tag)
        {
            CodeContracts.VerifyNotNull(page, "page");
            CodeContracts.VerifyNotNull(tag, "tag");

            return this.GetItem(page, tag, "[{0}.{1}]".FormatWith(page.ToUpper(), tag.ToUpper()));
        }

        /// <summary>
        /// Get the Item.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The item.
        /// </returns>
        public string GetItem([NotNull] string page, [NotNull] string tag, [CanBeNull] string defaultValue)
        {
            CodeContracts.VerifyNotNull(page, "page");
            CodeContracts.VerifyNotNull(tag, "tag");

            string item = string.Empty;

            this.LoadThemeFile();

            if (this._themeResources != null)
            {
                string langCode = YafContext.Current.Get<ILocalization>().LanguageCode.ToUpper();
                var selectedPage = this._themeResources.page.FirstOrDefault(x => x.name == page.ToUpper());

                if (selectedPage != null)
                {
                    var selectedItem =
                        selectedPage.Resource.FirstOrDefault(
                            x =>
                            x.tag == tag.ToUpper()
                            && (x.language.IsNotSet() || (x.language.IsSet() && x.language == langCode.ToUpper())));

                    if (selectedItem == null)
                    {
                        if (this.LogMissingThemeItem)
                        {
                            YafContext.Current.Get<ILogger>()
                                      .Error("Missing Theme Item: {0}.{1} in {2}.ascx".FormatWith(page.ToUpper(), tag.ToUpper(), page.ToLower()));
                        }

                        return defaultValue;
                    }

                    return selectedItem.Value.IsSet()
                               ? selectedItem.Value.Replace(
                                   "~",
                                   "{0}{1}/{2}".FormatWith(
                                       YafForumInfo.ForumServerFileRoot,
                                       YafBoardFolders.Current.Themes,
                                       this._themeResources.dir))
                               : defaultValue;
                }

                return defaultValue;
            }

            return item;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the mapped theme file.
        /// </summary>
        /// <param name="themeFile">The theme file.</param>
        /// <returns>
        /// The get mapped theme file.
        /// </returns>
        private static string GetMappedThemeFile([NotNull] string themeFile)
        {
            CodeContracts.VerifyNotNull(themeFile, "themeFile");

            return
                HttpContext.Current.Server.MapPath(
                    string.Concat(
                        YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Themes, "/", themeFile.Trim()));
        }

        /// <summary>
        /// Loads the theme file.
        /// </summary>
        private void LoadThemeFile()
        {
            if (this.ThemeFile != null && this._themeResources == null)
            {
                this._themeResources =
                    new LoadSerializedXmlFile<ThemeResources>().FromFile(
                        GetMappedThemeFile(this.ThemeFile),
                        "THEMEFILE{0}".FormatWith(this.ThemeFile),
                        r =>
                            {
                                // transform the page and tag name ToUpper...
                                r.page.ForEach(p => p.name = p.name.ToUpper());
                                r.page.Where(p => p.Resource == null).ForEach(
                                    p => p.Resource = new ResourcesPageResource[0]);
                                r.page.ForEach(p => p.Resource.ForEach(i => i.tag = i.tag.ToUpper()));
                            });
            }
        }

        #endregion
    }
}