/* Farsi Library - Working with Dates, Calendars, and DatePickers
 * http://www.codeproject.com/KB/selection/FarsiLibrary.aspx
 * 
 * Copyright (C) Hadi Eskandari
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace FarsiLibrary.Internals
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// A wrapper around Win32 Theming. Return which theme is
    /// currently active.
    /// </summary>
    public static class ThemeWrapper
    {
        #region Fields

        private static string _themeName;
        
        #endregion

        #region Native

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern bool IsThemeActive();

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        private static extern int GetCurrentThemeName(StringBuilder pszThemeFileName, int dwMaxNameChars, StringBuilder pszColorBuff, int dwMaxColorChars, StringBuilder pszSizeBuff, int cchMaxSizeChars);

        #endregion

        #region Ctor

        static ThemeWrapper()
        {
            CreateThemeInfo();
        }

        #endregion

        #region Props

        public static string CurrentThemeName
        {
            get
            {
                if (string.IsNullOrEmpty(ThemeColor))
                    return ThemeName;

                return string.Format("{0}.{1}", ThemeName, ThemeColor);
            }
        }

        public static bool IsActive
        {
            get; private set;
        }

        private static string ThemeColor
        {
            get; set;
        }

        private static string ThemeName
        {
            get
            {
                if (_themeName == string.Empty)
                    return "classic";
                
                return Path.GetFileNameWithoutExtension(_themeName);
            }
            set { _themeName = value; }
        }

        #endregion

        #region Methods

        private static void EnsureThemeName()
        {
            var sbTheme = new StringBuilder(260);
            var sbColor = new StringBuilder(260);

            if (GetCurrentThemeName(sbTheme, sbTheme.Capacity, sbColor, sbColor.Capacity, null, 0) == 0)
            {
                ThemeName = sbTheme.ToString().ToLower();
                ThemeColor = sbColor.ToString().ToLower();
            }
            else
            {
                ThemeName = ThemeColor = string.Empty;
            }
        }
        
        private static void CreateThemeInfo()
        {
            IsActive = IsThemeActive();
            EnsureThemeName();
        }

        #endregion
    }
}
