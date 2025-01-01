﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

/// <summary>
/// The Page Popup Module
/// </summary>
[Module("Page Popup Module", "Tiny Gecko", 1)]
public class PagePopupForumModule : SimpleBaseForumModule
{
    /// <summary>
    ///   The _error popup.
    /// </summary>
    private PopupDialogNotification errorPopup;

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
        if (this.errorPopup == null)
        {
            this.AddErrorPopup();
        }

        this.CurrentForumPage.PreRender += this.CurrentForumPage_PreRender;
    }

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
        this.ForumControl.Init += this.ForumControl_Init;
    }

    /// <summary>
    /// The register load string.
    /// </summary>
    protected void RegisterLoadString()
    {
        var message = this.PageBoardContext.LoadMessage.GetMessage();

        if (message == null)
        {
            return;
        }

        // Get the clean JS string.
        message.Message = message.Message.ToJsString();

        if (message.Message.IsNotSet())
        {
            return;
        }

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            this.ForumControl.Page,
            "modalNotification",
            string
                .Format(
                    "var fpModal = function() {{ {2}(\"{0}\", \"{1}\",\"{3}\"); Sys.Application.remove_load(fpModal); }}; Sys.Application.add_load(fpModal);",
                    message.Message,
                    message.MessageType.ToString().ToLower(),
                    this.errorPopup.ShowModalFunction,
                    message.Script.IsSet() ? message.Script : string.Empty));
    }

    /// <summary>
    /// Sets up the Modal Error Popup Dialog
    /// </summary>
    private void AddErrorPopup()
    {
        if (this.ForumControl.FindControl("YafForumPageErrorPopup1") == null)
        {
            // add error control...
            this.errorPopup = new PopupDialogNotification
                                  {
                                      ID = "YafForumPageErrorPopup1"
                                  };

            this.ForumControl.Controls.Add(this.errorPopup);
        }
        else
        {
            // reference existing control...
            this.errorPopup = (PopupDialogNotification)this.ForumControl.FindControl("YafForumPageErrorPopup1");
        }
    }

    /// <summary>
    /// Handles the PreRender event of the CurrentForumPage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void CurrentForumPage_PreRender(object sender, EventArgs e)
    {
        this.RegisterLoadString();
    }

    /// <summary>
    /// Handles the Init event of the ForumControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void ForumControl_Init(object sender, EventArgs e)
    {
        // at this point, init has already been called...
        this.AddErrorPopup();
    }
}