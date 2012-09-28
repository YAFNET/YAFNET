/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
    using System;
    using System.Data;

    using YAF.Classes;
    using YAF.Types.Interfaces;

    /// <summary>
    /// The user helper.
    /// </summary>
    public static class UserHelper
    {
        #region Public Methods

        /// <summary>
        /// Gets the user language file.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>
        /// language file name. If null -- use default language
        /// </returns>
        public static string GetUserLanguageFile(int userId)
        {
            // get the user information...
            DataRow row = UserMembershipHelper.GetUserRowForID(userId);

            if (row != null && row["LanguageFile"] != DBNull.Value
                && YafContext.Current.Get<YafBoardSettings>().AllowUserLanguage)
            {
                return row["LanguageFile"].ToString();
            }

            return null;
        }

        /// <summary>
        /// Gets the user theme file.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns User theme</returns>
        public static string GetUserThemeFile(int userId)
        {
            DataRow row = UserMembershipHelper.GetUserRowForID(userId);

            string themeFile = row["ThemeFile"] != DBNull.Value && YafContext.Current.Get<YafBoardSettings>().AllowUserTheme
                                   ? row["ThemeFile"].ToString()
                                   : YafContext.Current.Get<YafBoardSettings>().Theme;

            if (!YafTheme.IsValidTheme(themeFile))
            {
                themeFile = StaticDataHelper.Themes().Rows[0][1].ToString();
            }

            return themeFile;
        }

        #endregion
    }
}