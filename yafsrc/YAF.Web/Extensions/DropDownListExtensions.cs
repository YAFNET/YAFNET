/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Web.Extensions
{
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Types;
    using YAF.Types.Extensions;

    /// <summary>
    /// The DropDown Extensions
    /// </summary>
    public static class DropDownListExtensions
    {
        /// <summary>
        /// Add forum and category icons to dropdown list.
        /// </summary>
        /// <param name="dropDownList">
        /// The drop down list.
        /// </param>
        /// <param name="forumList">
        /// The forum list.
        /// </param>
        public static void AddForumAndCategoryIcons(this DropDownList dropDownList, [NotNull] DataTable forumList)
        {
            CodeContracts.VerifyNotNull(dropDownList, "dropDownList");

            CodeContracts.VerifyNotNull(forumList, "forumList");

            forumList.Rows.Cast<DataRow>().ForEach(
                row =>
                    {
                        var item = new ListItem { Value = row["ForumID"].ToString(), Text = row["Title"].ToString() };

                        item.Attributes.Add(
                            "data-content",
                            $"<span class=\"select2-image-select-icon\"><i class=\"fas fa-{row["Icon"]} fa-fw text-secondary\"></i>&nbsp;{row["Title"]}</span>");

                        dropDownList.Items.Add(item);
                    });
        }
    }
}