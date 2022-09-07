/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Pages.Profile;

using YAF.Types.Models;

/// <summary>
/// The attachments Page Class.
/// </summary>
public partial class Attachments : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Attachments" /> class.
    /// </summary>
    public Attachments()
        : base("ATTACHMENTS", ForumPages.Profile_Attachments)
    {
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">
    /// The source of the event. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data. 
    /// </param>
    protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.PageBoardContext.UploadAccess)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
        }

        if (this.IsPostBack)
        {
            return;
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        this.BindData();
    }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        var displayName = this.PageBoardContext.PageUser.DisplayOrUserName();

        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddUser(this.PageBoardContext.PageUserID, displayName);
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// Gets the preview image.
    /// </summary>
    /// <param name="o">The Data Row object.</param>
    /// <returns>Returns the Preview Image</returns>
    protected string GetPreviewImage([NotNull] object o)
    {
        var attach = o.ToType<Attachment>();

        var fileName = attach.FileName;
        var isImage = fileName.IsImageName();
        var url =
            $"{BoardInfo.ForumClientFileRoot}resource.ashx?i={attach.ID}&editor=true";

        return isImage
                   ? $"<img src=\"{url}\" alt=\"{fileName}\" title=\"{fileName}\" data-url=\"{url}\" style=\"max-width:30px\" class=\"me-2\" />"
                   : "<i class=\"far fa-file-alt attachment-icon me-2\"></i>";
    }

    /// <summary>
    /// Delete all selected attachment(s)
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteAttachments_Click(object sender, EventArgs e)
    {
        var items = (from RepeaterItem item in this.List.Items
                     where item.ItemType is ListItemType.Item or ListItemType.AlternatingItem
                     where item.FindControlAs<CheckBox>("Selected").Checked
                     select item).ToList();

        if (items.Any())
        {
            items.ForEach(
                item => this.GetRepository<Attachment>().DeleteById(
                    item.FindControlAs<HiddenField>("FileID").Value.ToType<int>()));

            this.PageBoardContext.Notify(this.GetTextFormatted("DELETED", items.Count), MessageTypes.success);
        }

        this.BindData();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var dt = this.GetRepository<Attachment>().GetPaged(
            a => a.UserID == this.PageBoardContext.PageUserID,
            this.PagerTop.CurrentPageIndex,
            this.PagerTop.PageSize);

        this.List.DataSource = dt;
        this.PagerTop.Count = !dt.NullOrEmpty()
                                  ? this.GetRepository<Attachment>().Count(a => a.UserID == this.PageBoardContext.PageUserID).ToType<int>()
                                  : 0;

        this.DataBind();
    }
}