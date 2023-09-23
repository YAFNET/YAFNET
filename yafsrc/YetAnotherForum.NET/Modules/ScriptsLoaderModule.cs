/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Modules;

using System.Web.UI;

using YAF.Types.Attributes;

/// <summary>
/// Automatic JavaScript Loading Module
/// </summary>
[Module("JavaScript Loading Module", "Ingo Herbote", 1)]
public class ScriptsLoaderModule : SimpleBaseForumModule
{
    /// <summary>
    /// The initializes after page.
    /// </summary>
    public override void InitAfterPage()
    {
        this.CurrentForumPage.PreRender += this.CurrentForumPagePreRender;
        this.CurrentForumPage.Load += this.CurrentForumPageLoad;
    }

    /// <summary>
    /// Handles the Load event of the ForumPage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void CurrentForumPageLoad([NotNull] object sender, [NotNull] EventArgs e)
    {
        // Load CSS First
        this.RegisterCssFiles(this.PageBoardContext.BoardSettings.CdvVersion);
    }

    /// <summary>
    /// Handles the PreRender event of the CurrentForumPage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void CurrentForumPagePreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.PageBoardContext.Vars.ContainsKey("forumExtensions"))
        {
            return;
        }

        var version = this.PageBoardContext.BoardSettings.CdvVersion;

        var forumJsName = Config.IsDotNetNuke ? "forumExtensionsDnn" : "forumExtensions";
        var adminForumJsName = Config.IsDotNetNuke ? "forumAdminExtensionsDnn" : "forumAdminExtensions";

        ScriptManager.ScriptResourceMapping.AddDefinition(
            "forumAdminExtensions",
            new ScriptResourceDefinition
                {
                    Path = BoardInfo.GetURLToScripts($"{adminForumJsName}.min.js?v={version}"),
                    DebugPath = BoardInfo.GetURLToScripts($"{adminForumJsName}.js?v={version}")
                });

        ScriptManager.ScriptResourceMapping.AddDefinition(
            "forumExtensions",
            new ScriptResourceDefinition
                {
                    Path = BoardInfo.GetURLToScripts($"{forumJsName}.min.js?v={version}"),
                    DebugPath = BoardInfo.GetURLToScripts($"{forumJsName}.js?v={version}")
                });

        ScriptManager.ScriptResourceMapping.AddDefinition(
            "FileUploadScript",
            new ScriptResourceDefinition
                {
                    Path = BoardInfo.GetURLToScripts("fileUploader.min.js"),
                    DebugPath = BoardInfo.GetURLToScripts("fileUploader.js")
                });

        this.PageBoardContext.PageElements.AddScriptReference(
            this.PageBoardContext.CurrentForumPage.IsAdminPage ? "forumAdminExtensions" : "forumExtensions");

        this.PageBoardContext.Vars["forumExtensions"] = true;
    }

    /// <summary>
    /// Register the CSS Files in the header.
    /// </summary>
    /// <param name="version">
    /// The version.
    /// </param>
    private void RegisterCssFiles(int version)
    {
        var element = this.PageBoardContext.CurrentForumPage.TopPageControl;

        element.Controls.Add(
            ControlHelper.MakeCssIncludeControl(
                $"{this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")}?v={version}"));

        // make the style sheet link controls.
        element.Controls.Add(
            ControlHelper.MakeCssIncludeControl(
                BoardInfo.GetURLToContent(
                    this.PageBoardContext.CurrentForumPage.IsAdminPage
                        ? $"forum-admin.min.css?v={version}"
                        : $"forum.min.css?v={version}")));
    }
}