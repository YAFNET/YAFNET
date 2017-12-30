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
namespace YAF.Types.Interfaces
{
    using System.Data;

    public interface IStyleTransform
    {
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
        void DecodeStyleByRow(DataRow dr, string columnName = "Style", bool colorOnly = false);

        /// <summary>
        /// The decode style by string.
        /// </summary>
        /// <param name="styleStr">The style str.</param>
        /// <param name="colorOnly">The color only.</param>
        /// <returns>
        /// The decode style by string.
        /// </returns>
        string DecodeStyleByString(string styleStr, bool colorOnly = false);

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
        void DecodeStyleByTable(DataTable dt, bool colorOnly = false, string[] styleColumns = null);
    }
}