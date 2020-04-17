/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// Transforms the style.
    /// </summary>
    public class StyleTransform : IStyleTransform
    {
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

        /// <summary>
        /// The decode style Group
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <param name="colorOnly">
        /// The color only.
        /// </param>
        public void DecodeStyleByGroup(Group group, bool colorOnly = false)
        {
            if (group.Style.IsSet())
            {
                group.Style = this.DecodeStyleByString(group.Style, colorOnly);
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IStyleTransform

        /// <summary>
        /// Decode Style by String
        /// </summary>
        /// <param name="style">The style string.</param>
        /// <param name="colorOnly">The color only.</param>
        /// <returns>
        /// The decode style by string.
        /// </returns>
        public string DecodeStyleByString(string style, bool colorOnly = false)
        {
            var styleRow = style.Trim().Split('/');

            styleRow.Select(s => s.Split('!')).Where(x => x.Length > 1).ForEach(
                pair => { style = colorOnly ? GetColorOnly(pair[1]) : pair[1]; });
            
            return style;
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
            styleColumns ??= new[] { "Style" };

            dt.Rows.Cast<DataRow>().ForEach(
                row =>
                    {
                        styleColumns.ForEach(
                            t =>
                                {
                                    var dr = row;
                                    this.DecodeStyleByRow(dr, t, colorOnly);
                                });
                    });
        }

        /// <summary>
        /// Decode Groups Styles
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="colorOnly">
        /// The color only.
        /// </param>
        public void DecodeStyleByGroupList(List<Group> list, bool colorOnly = false)
        {
            list.ForEach(group => { this.DecodeStyleByGroup(group, colorOnly); });
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the color only
        /// </summary>
        /// <param name="styleString">
        /// The style string.
        /// </param>
        /// <returns>
        /// The get color only.
        /// </returns>
        private static string GetColorOnly(string styleString)
        {
            var styleArray = styleString.Split(';');
            return styleArray.FirstOrDefault(t => t.ToLower().Contains("color"));
        }

        #endregion
    }
}