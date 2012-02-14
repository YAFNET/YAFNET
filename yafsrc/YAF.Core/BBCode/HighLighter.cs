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

namespace YAF.Core.BBCode
{
    #region Using

    using System;
    using System.Text;

    #endregion

    /// <summary>
    /// The high lighter.
    /// </summary>
    public class HighLighter
    {
        /* Ederon : 6/16/2007 - conventions */

        // To Replace Enter with <br />
        #region Constants and Fields

        /// <summary>
        ///   The _replace enter.
        /// </summary>
        private bool _replaceEnter;

        #endregion

        // Default Constructor
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "HighLighter" /> class.
        /// </summary>
        public HighLighter()
        {
            this._replaceEnter = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether ReplaceEnter.
        /// </summary>
        public bool ReplaceEnter
        {
            get
            {
                return this._replaceEnter;
            }

            set
            {
                this._replaceEnter = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The color text.
        /// </summary>
        /// <param name="tmpCode">
        /// The tmp code.
        /// </param>
        /// <param name="pathToDefFile">
        /// The path to def file.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The color text.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public string ColorText(string tmpCode, string pathToDefFile, string language)
        {
            language = language.ToLower();

            language = language.Replace("\"", string.Empty);

            if (language.Equals("cs"))
            {
                language = "csharp";
            }

            var tmpOutput = new StringBuilder();

            // Create Output
            tmpOutput.AppendFormat("<pre class=\"brush:{0};\">{1}", language, Environment.NewLine);
            tmpOutput.Append(tmpCode);
            tmpOutput.AppendFormat("</pre>{0}", Environment.NewLine);

            return tmpOutput.ToString();
        }

        #endregion
    }
}