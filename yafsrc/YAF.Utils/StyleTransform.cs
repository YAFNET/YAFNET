/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;

    using YAF.Types.Interfaces;

    #endregion

    /* Created by vzrus(c) for Yet Another Forum and can be use with any Yet Another Forum licence and modified in any way.*/

    /// <summary>
    /// Transforms the style.
    /// </summary>
    public class StyleTransform : IStyleTransform
    {
        #region Constants and Fields

        /// <summary>
        /// The _theme.
        /// </summary>
        private readonly ITheme _theme;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleTransform"/> class.
        /// </summary>
        /// <param name="theme">
        /// The theme.
        /// </param>
        public StyleTransform(ITheme theme)
        {
            this._theme = theme;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets CurrentThemeFile.
        /// </summary>
        public string CurrentThemeFile
        {
            get
            {
                return this._theme != null ? this._theme.ThemeFile.ToLower().Trim() : string.Empty;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The decode style by row.
        /// </summary>
        /// <param name="dr">
        /// The dr.
        /// </param>
        /// <param name="columnName">
        /// the style column name
        /// </param>
        /// <param name="colorOnly">
        /// The color only.
        /// </param>
        public void DecodeStyleByRow(DataRow dr, string columnName = "Style", bool colorOnly = false)
        {
            dr[columnName] = this.DecodeStyleByString(dr[columnName].ToString(), colorOnly);
        }

        #endregion

        #region Implemented Interfaces

        #region IStyleTransform

        /// <summary>
        /// The decode style by string.
        /// </summary>
        /// <param name="styleStr">The style str.</param>
        /// <param name="colorOnly">The color only.</param>
        /// <returns>
        /// The decode style by string.
        /// </returns>
        public string DecodeStyleByString(string styleStr, bool colorOnly = false)
        {
            string[] styleRow = styleStr.Trim().Split('/');

            foreach (string[] pair in styleRow.Select(s => s.Split('!')).Where(x => x.Length > 1))
            {
                if (pair[0].ToLowerInvariant().Trim() == "default")
                {
                    styleStr = colorOnly ? this.GetColorOnly(pair[1]) : pair[1];
                }

                styleStr = this.DecodeStyleByString(styleStr, colorOnly, pair);
            }

            return styleStr;
        }

        /// <summary>
        /// The decode style by table.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <param name="colorOnly">
        /// The color only.
        /// </param>
        /// <param name="styleColumns">
        /// The styleColumns can contain param array to handle several style columns.
        /// </param>
        public void DecodeStyleByTable(DataTable dt, bool colorOnly = false, string[] styleColumns = null)
        {
            styleColumns = styleColumns ?? new string[] { "Style" }; 

            foreach (var row in dt.Rows.Cast<DataRow>())
            {
                foreach (string t in styleColumns)
                {
                    DataRow dr = row;
                    this.DecodeStyleByRow(dr, t, colorOnly);
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The decode style by string.
        /// </summary>
        /// <param name="styleStr">
        /// The style str.
        /// </param>
        /// <param name="colorOnly">
        /// The color only.
        /// </param>
        /// <param name="pair">
        /// The pair.
        /// </param>
        /// <returns>
        /// The decode style by string.
        /// </returns>
        private string DecodeStyleByString(string styleStr, bool colorOnly, string[] pair)
        {
            string styleStrResult = styleStr;

            if (pair.Select(t => string.Format("{0}.xml", pair[0])).Any(filename => filename.Trim().Equals(this.CurrentThemeFile, StringComparison.CurrentCultureIgnoreCase)))
            {
                styleStrResult = colorOnly ? this.GetColorOnly(pair[1]) : pair[1];
            }

            return styleStrResult;
        }

        /// <summary>
        /// The get color only.
        /// </summary>
        /// <param name="styleString">
        /// The style string.
        /// </param>
        /// <returns>
        /// The get color only.
        /// </returns>
        private string GetColorOnly(string styleString)
        {
            string[] styleArray = styleString.Split(';');
            return styleArray.FirstOrDefault(t => t.ToLower().Contains("color"));
        }

        #endregion
    }
}