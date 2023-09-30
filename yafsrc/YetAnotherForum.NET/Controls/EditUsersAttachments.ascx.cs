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

namespace YAF.Controls;

using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The edit users Attachments.
/// </summary>
public partial class EditUsersAttachments : BaseUserControl
{
    /// <summary>
    ///   Gets user ID of edited user.
    /// </summary>
    protected int CurrentUserID =>
        this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
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
    /// Handles the Click event of the Back control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Back_Click(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
    }

    /// <summary>
    /// Handles the ItemCommand event of the List control.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void List_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "delete":
                this.GetRepository<Attachment>().Delete(e.CommandArgument.ToType<int>());

                this.BindData();
                break;
        }
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
    protected void PagerTop_PageChange(object sender, EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// Gets the preview image.
    /// </summary>
    /// <param name="o">The Data Row object.</param>
    /// <returns>Returns the Preview Image</returns>
    protected string GetPreviewImage(object o)
    {
        var attach = o.ToType<Attachment>();

        var fileName = attach.FileName;
        var isImage = fileName.IsImageName();
        var url =
            $"{BoardInfo.ForumClientFileRoot}resource.ashx?i={attach.ID}&editor=true";

        return isImage
                   ? $"<img src=\"{url}\" alt=\"{fileName}\" title=\"{fileName}\" data-url=\"{url}\" style=\"max-width:30px\" class=\"img-thumbnail attachments-preview\" />"
                   : "<i class=\"far fa-file-alt attachment-icon\"></i>";
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
        (from RepeaterItem item in this.List.Items
         where item.ItemType is ListItemType.Item or ListItemType.AlternatingItem
         where item.FindControlAs<CheckBox>("Selected").Checked
         select item).ForEach(
            item => this.GetRepository<Attachment>().DeleteById(
                item.FindControlAs<ThemeButton>("ThemeButtonDelete").CommandArgument.ToType<int>()));

        this.BindData();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var dt = this.GetRepository<Attachment>().GetPaged(
            a => a.UserID == this.CurrentUserID,
            this.PagerTop.CurrentPageIndex,
            this.PagerTop.PageSize);

        this.List.DataSource = dt;
        this.PagerTop.Count = !dt.NullOrEmpty()
                                  ? this.GetRepository<Attachment>().Count(a => a.UserID == this.CurrentUserID).ToType<int>()
                                  : 0;

        this.DataBind();
    }
}