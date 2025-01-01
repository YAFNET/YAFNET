/* Yet Another Forum.NET
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

namespace YAF.Pages;

/// <summary>
/// Forum Rules Page.
/// </summary>
public partial class RulesAndPrivacy : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "RulesAndPrivacy" /> class.
    /// </summary>
    public RulesAndPrivacy()
        : base("RULES", ForumPages.RulesAndPrivacy)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// The accept_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Accept_Click(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Account_Register);
    }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Board);
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"));
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="EventArgs"/> instance containing the event data.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.RulesText.Param0 = Config.GDPRControllerAddress.IsSet()
                                    ? Config.GDPRControllerAddress
                                    : this.PageBoardContext.BoardSettings.ForumEmail;

        this.Footer.Visible = this.PageBoardContext.IsGuest;
    }
}