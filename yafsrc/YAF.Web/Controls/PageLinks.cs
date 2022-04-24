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
namespace YAF.Web.Controls;

#region Using

using YAF.Types.Objects;

#endregion

/// <summary>
/// Page Links Control.
/// </summary>
public class PageLinks : BaseControl, IAdd<PageLink>
{
    #region Properties

    /// <summary>
    ///   Gets or sets LinkedPageLinkID.
    /// </summary>
    [CanBeNull]
    public string LinkedPageLinkID
    {
        get => this.ViewState["LinkedPageLinkID"].ToType<string>();

        set => this.ViewState["LinkedPageLinkID"] = value;
    }

    /// <summary>
    ///   Gets or sets PageLink List
    /// </summary>
    [CanBeNull]
    public List<PageLink> PageLinkList
    {
        get => this.ViewState["PageLinkList"] as List<PageLink>;

        set => this.ViewState["PageLinkList"] = value;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Clear all Links
    /// </summary>
    public void Clear()
    {
        this.PageLinkList = null;
    }

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add([NotNull] PageLink item)
    {
        CodeContracts.VerifyNotNull(item);

        var list = this.PageLinkList ?? new List<PageLink>();

        list.Add(item);

        this.PageLinkList = list;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        List<PageLink> linkedPageList = null;

        if (this.LinkedPageLinkID.IsSet())
        {
            // attempt to get access to the other control...
            if (this.Parent.FindControl(this.LinkedPageLinkID) is PageLinks parentControl)
            {
                // use the other data stream...
                linkedPageList = parentControl.PageLinkList;
            }
        }
        else
        {
            // use the data table from this control...
            linkedPageList = this.PageLinkList;
        }

        if (linkedPageList == null || !linkedPageList.Any())
        {
            return;
        }

        writer.Write("<nav aria-label=\"breadcrump\"><ol class=\"breadcrumb\">");

        linkedPageList.ForEach(
            link =>
                {
                    var encodedTitle = this.HtmlEncode(link.Title);
                    var url = link.URL;

                    writer.WriteLine(
                        url.IsNotSet()
                            ? $@"<li class=""breadcrumb-item active"">{encodedTitle}</li>"
                            : $@"<li class=""breadcrumb-item""><a href=""{url}"">{encodedTitle}</a></li>");
                });

        writer.Write("</ol></nav>");

        // Inject Board Announcement
        var boardAnnounceControl =
            this.Page.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/BoardAnnouncement.ascx");

        writer.Write(boardAnnounceControl.RenderToString());
    }

    #endregion
}