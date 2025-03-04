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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Flags;
using YAF.Types.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Helpers;
using YAF.Types.Extensions;
using YAF.Types.Modals;

/// <summary>
/// Administration Page to Add/Edit Medals
/// </summary>
public class EditMedalModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditMedalInputModel Input { get; set; }

    [BindProperty]
    public List<Tuple<Medal, UserMedal, User>> UserList { get; set; }

    [BindProperty]
    public List<Tuple<Medal, GroupMedal, Group>> GroupList { get; set; }

    [BindProperty]
    public List<SelectListItem> MedalImages { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "EditMedalModel" /> class.
    ///   Default constructor.
    /// </summary>
    public EditMedalModel()
        : base("ADMIN_EDITMEDAL", ForumPages.Admin_EditMedal)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_MEDALS", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Medals));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITMEDAL", "TITLE"), string.Empty);
    }

    public PartialViewResult OnGetAddUser(int medalId)
    {
        var medal = this.GetRepository<Medal>().GetById(medalId);

        return new PartialViewResult {
                                         ViewName = "Dialogs/UserMedalEdit",
                                         ViewData = new ViewDataDictionary<UserMedalEditModal>(
                                             this.ViewData,
                                             new UserMedalEditModal {
                                                                        MedalId = medal.ID,
                                                                        MedalName = medal.Name
                                             })
                                     };
    }

    public PartialViewResult OnGetAddGroup(int medalId)
    {
        var groupList = new SelectList(this.GetRepository<Group>().List(boardId: this.PageBoardContext.PageBoardID), nameof(Group.ID), nameof(Group.Name));

        return new PartialViewResult
               {
                   ViewName = "Dialogs/GroupMedalEdit",
                   ViewData = new ViewDataDictionary<GroupMedalEditModal>(
                       this.ViewData,
                       new GroupMedalEditModal {
                                                   MedalId = medalId,
                                                   GroupList = groupList
                                               })
               };
    }

    public PartialViewResult OnGetEditUser(int userId, int medalId)
    {
        // Edit
        var row = this.GetRepository<UserMedal>().List(userId, medalId).FirstOrDefault();

        return new PartialViewResult {
                                         ViewName = "Dialogs/UserMedalEdit",
                                         ViewData = new ViewDataDictionary<UserMedalEditModal>(
                                             this.ViewData,
                                             new UserMedalEditModal {
                                                                        MedalId = row!.Item1.ID,
                                                                        UserID = row.Item3.ID,
                                                                        UserName = row.Item3.Name,
                                                                        UserMessage =
                                                                            row.Item2.Message.IsSet()
                                                                                ? row.Item2.Message
                                                                                : row.Item1.Message,
                                                                        UserSortOrder = row.Item2.SortOrder,
                                                                        UserHide = row.Item2.Hide,
                                                                        MedalName = row.Item1.Name
                                                                    })
                                     };
    }

    public PartialViewResult OnGetEditGroup(int groupId, int medalId)
    {
        // Edit
        var row = this.GetRepository<GroupMedal>().List(groupId, medalId).FirstOrDefault();

        // remove all user medals...
        this.Get<IDataCache>().Remove(
            k => k.StartsWith(string.Format(Constants.Cache.UserMedals, string.Empty)));

        return new PartialViewResult
               {
                   ViewName = "Dialogs/GroupMedalEdit",
                   ViewData = new ViewDataDictionary<GroupMedalEditModal>(
                       this.ViewData,
                       new GroupMedalEditModal
                       {
                           MedalId = medalId,
                           GroupId = row!.Item2.GroupID,
                           GroupName = row.Item3.Name,
                           GroupMessage = row.Item2.Message.IsSet() ? row.Item2.Message : row.Item1.Message,
                           GroupSortOrder = row.Item2.SortOrder,
                           GroupHide = row.Item2.Hide
                       })
               };
    }

    /// <summary>
    /// Creates link to group editing admin interface.
    /// </summary>
    /// <param name="item">
    /// The data.
    /// </param>
    /// <returns>
    /// The format group link.
    /// </returns>
    public string FormatGroupLink(Tuple<Medal, GroupMedal, Group> item)
    {
        return string.Format(
            "<a href=\"{1}\">{0}</a>",
            item.Item3.Name,
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_EditGroup, new {i = item.Item3.ID }));
    }

    /// <summary>
    /// Creates link to user editing admin interface.
    /// </summary>
    /// <param name="item">
    /// The data.
    /// </param>
    /// <returns>
    /// The format user link.
    /// </returns>
    public string FormatUserLink(Tuple<Medal, UserMedal, User> item)
    {
        return string.Format(
            "<a href=\"{2}\">{0}&nbsp;({1})</a>",
            this.HtmlEncode(item.Item3.DisplayName),
            this.HtmlEncode(item.Item3.Name),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_EditUser, new {u = item.Item3.ID }));
    }

    /// <summary>
    /// On post remove group as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OnPostRemoveGroupAsync(int id)
    {
        await this.GetRepository<GroupMedal>().DeleteAsync(
            medal => medal.GroupID == id
                     && medal.MedalID == this.Input.Id);

        // remove all user medals...
        this.RemoveMedalsFromCache();

        this.BindData();
    }

    /// <summary>
    /// Handles page load event.
    /// </summary>
    public void OnGet(int? medalId = null)
    {
        this.Input = new EditMedalInputModel();

        this.BindData(medalId);
    }

    /// <summary>
    /// Removals all medals from the cache...
    /// </summary>
    protected void RemoveMedalsFromCache()
    {
        // remove all user medals...
        this.Get<IDataCache>().Remove(k => k.StartsWith(string.Format(Constants.Cache.UserMedals, string.Empty)));
    }

    /// <summary>
    /// Handles save button click.
    /// </summary>
    public IActionResult OnPostSave()
    {
        if (this.Input.MedalImage.IsNotSet() || this.Input.MedalImage == this.GetText("ADMIN_EDITMEDAL", "SELECT_IMAGE"))
        {
            return this.PageBoardContext.Notify(this.GetText("ADMIN_EDITMEDAL", "MSG_IMAGE"), MessageTypes.warning);
        }

        var flags = new MedalFlags(0)
                        {
                            ShowMessage = this.Input.ShowMessage,
                            AllowHiding = this.Input.AllowHiding
                        };

        var medalImage = this.Input.MedalImage;

        // save medal
        this.GetRepository<Medal>().Save(
            this.Input.Id,
            this.Input.Name,
            this.Input.Description,
            this.Input.Message,
            this.Input.Category,
            medalImage,
            flags.BitValue);

        // go back to medals administration
        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Medals);
    }

    public async Task OnPostRemoveUserAsync(int id)
    {
        // delete user-medal
        await this.GetRepository<UserMedal>().DeleteAsync(
            medal => medal.UserID == id
                     && medal.MedalID == this.Input.Id);

        // clear cache...
        this.Get<IDataCache>().Remove(
            string.Format(Constants.Cache.UserMedals, id));
        this.BindData();
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData(int? medalId = null)
    {
        this.CreateMedalImagesList();

        if (!medalId.HasValue)
        {
            return;
        }

        // load users and groups who has been assigned this medal
        this.UserList = this.GetRepository<UserMedal>().List(null, medalId.Value);

        this.GroupList = this.GetRepository<GroupMedal>().List(null, medalId.Value);

        var medal = this.GetRepository<Medal>().GetSingle(m => m.ID == medalId.Value);

        if (medal is null)
        {
            return;
        }

        // set controls
        this.Input.Id = medal.ID;
        this.Input.Name = medal.Name;
        this.Input.Description = medal.Description;
        this.Input.Message = medal.Message;
        this.Input.Category = medal.Category;
        this.Input.ShowMessage = medal.MedalFlags.ShowMessage;
        this.Input.AllowHiding = medal.MedalFlags.AllowHiding;
        this.Input.MedalImage = medal.MedalURL;
    }

    /// <summary>
    /// load available images from images/medals folder
    /// </summary>
    private void CreateMedalImagesList()
    {
        var list = new List<SelectListItem> { new(this.GetText("ADMIN_EDITMEDAL", "SELECT_IMAGE"), "") };

        var dir = new DirectoryInfo(
            Path.Combine(this.Get<BoardInfo>().WebRootPath, this.Get<BoardFolders>().Medals));

        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*").ToList();

            list.AddImageFiles(files, this.Get<BoardFolders>().Medals);
        }

        this.MedalImages = list;
    }
}