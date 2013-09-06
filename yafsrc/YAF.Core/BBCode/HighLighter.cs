/* Yet Another Forum.net
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

namespace YAF.Core.BBCode
{
    #region Using

    using System;
    using System.Text;

    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The high lighter.
    /// </summary>
    public class HighLighter
    {
        // Default Constructor
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "HighLighter" /> class.
        /// </summary>
        public HighLighter()
        {
            this.ReplaceEnter = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether ReplaceEnter.
        /// </summary>
        public bool ReplaceEnter { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Colors the text.
        /// </summary>
        /// <param name="tmpCode">The tmp code.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// The color text.
        /// </returns>
        public string ColorText(string tmpCode, string language)
        {
            language = language.ToLower();

            language = language.Replace("\"", string.Empty);

            if (language.Equals("cs"))
            {
                language = "csharp";
            }

            var tmpOutput = new StringBuilder();

            var highlight = string.Empty;

            // extract highlight
            if (language.Contains(";"))
            {
                highlight = language.Substring(language.IndexOf(";") + 1);
                language = language.Remove(language.IndexOf(";"));
            }

            // Create Output
            tmpOutput.AppendFormat(
                "<pre class=\"brush:{0}{1}\">{2}",
                language,
                !string.IsNullOrEmpty(highlight) ? ";highlight: [{0}];".FormatWith(highlight) : string.Empty,
                Environment.NewLine);

            tmpOutput.Append(tmpCode);
            tmpOutput.AppendFormat("</pre>{0}", Environment.NewLine);

            return tmpOutput.ToString();
        }

        #endregion
    }
}