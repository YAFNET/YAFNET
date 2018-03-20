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
namespace YAF.Core.Helpers
{
    #region Using

    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Classes;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The Forum Editor helper.
    /// </summary>
    public static class ForumEditorHelper
    {
        #region Public Methods

        /// <summary>
        /// Gets the current forum editor.
        /// </summary>
        /// <returns>
        /// Returns the current forum editor
        /// </returns>
        public static ForumEditor GetCurrentForumEditor()
        {
            // get the forum editor based on the settings
            var editorId = YafContext.Current.BoardSettings.ForumEditor;

            if (YafContext.Current.BoardSettings.AllowUsersTextEditor)
            {
                // Text editor
                editorId = YafContext.Current.TextEditor.IsSet()
                               ? YafContext.Current.TextEditor
                               : YafContext.Current.BoardSettings.ForumEditor;
            }

            // Check if Editor exists, if not fallback to default editorid=1
            var forumEditor = YafContext.Current.Get<IModuleManager<ForumEditor>>().GetBy(editorId, false)
                              ?? YafContext.Current.Get<IModuleManager<ForumEditor>>().GetBy("1");

            // Check if tinymce is used and if it exists
            if (forumEditor.Description.Contains("TinyMCE"))
            {
                if (
                    !File.Exists(
                        HttpContext.Current.Server.MapPath(
                            "{0}{1}Scripts/tinymce/tinymce.min.js".FormatWith(
                                Config.ServerFileRoot,
                                Config.ServerFileRoot.EndsWith("/") ? string.Empty : "/"))))
                {
                    forumEditor = YafContext.Current.Get<IModuleManager<ForumEditor>>().GetBy("1");
                }
            }

            return forumEditor;
        }

        /// <summary>
        /// Gets the filtered editor list.
        /// </summary>
        /// <returns>Returns the filtered editor list.</returns>
        public static DataTable GetFilteredEditorList()
        {
            var editorList = YafContext.Current.Get<IModuleManager<ForumEditor>>().ActiveAsDataTable("Editors");
            
            // Check if TinyMCE exists
            var tinyMceExists = false;

            try
            {
                if (
                    File.Exists(HttpContext.Current.Server.MapPath("~/Scripts/tinymce/tinymce.min.js")))
                {
                    tinyMceExists = true;
                }
            }
            catch (Exception)
            {
                tinyMceExists = false;
            }

            if (tinyMceExists)
            {
                return editorList;
            }

            var filterList = new ArrayList();

            foreach (var drow in editorList.Rows.Cast<DataRow>().Where(drow => drow["Name"].ToString().Contains("TinyMCE")))
            {
                filterList.Add(drow);
            }

            foreach (DataRow row in filterList)
            {
                editorList.Rows.Remove(row);
            }

            editorList.AcceptChanges();
            return editorList;
        }

        #endregion
    }
}