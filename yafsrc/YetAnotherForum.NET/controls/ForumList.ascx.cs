/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Forum List Control
    /// </summary>
    public partial class ForumList : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The Go to last post Image ToolTip.
        /// </summary>
        private string altLastPost;

        /// <summary>
        /// The Data Source
        /// </summary>
        private IEnumerable dataSource;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Alt.
        /// </summary>
        [NotNull]
        public string AltLastPost
        {
            get => string.IsNullOrEmpty(this.altLastPost) ? string.Empty : this.altLastPost;

            set => this.altLastPost = value;
        }

        /// <summary>
        ///   Gets or sets DataSource.
        /// </summary>
        public IEnumerable DataSource
        {
            get => this.dataSource;

            set
            {
                this.dataSource = value;
                DataRow[] arr;
                var t = this.dataSource.GetType();
                var arlist = new List<DataRow>();
                var subLIst = new List<DataRow>();
                var parents = new List<int>();
                if (t.Name == "DataRowCollection")
                {
                    arr = new DataRow[((DataRowCollection)this.dataSource).Count];
                    ((DataRowCollection)this.dataSource).CopyTo(arr, 0);

                    for (var i = 0; i < arr.Count(); i++)
                    {
                        // these are all sub forums related to start page forums
                        if (!arr[i]["ParentID"].IsNullOrEmptyDBField())
                        {
                            if (this.SubDataSource == null)
                            {
                                this.SubDataSource = arr[i].Table.Clone();
                            }

                            var drow = this.SubDataSource.NewRow();
                            drow.ItemArray = arr[i].ItemArray;

                            parents.Add(drow["ForumID"].ToType<int>());

                            if (parents.Contains(drow["ParentID"].ToType<int>()))
                            {
                                this.SubDataSource.Rows.Add(drow);
                                subLIst.Add(drow);
                            }
                            else
                            {
                                arlist.Add(arr[i]);
                            }
                        }
                        else
                        {
                            arlist.Add(arr[i]);
                        }
                    }
                }
                else
                {
                    // (t.Name == "DataRow[]")
                    arr = (DataRow[])this.dataSource;
                    for (var i = 0; i < arr.Count(); i++)
                    {
                        if (!arr[i]["ParentID"].IsNullOrEmptyDBField())
                        {
                            if (this.SubDataSource == null)
                            {
                                this.SubDataSource = arr[i].Table.Clone();
                            }

                            var drow = this.SubDataSource.NewRow();
                            drow.ItemArray = arr[i].ItemArray;

                            this.SubDataSource.Rows.Add(drow);
                        }
                        else
                        {
                            arlist.Add(arr[i]);
                        }
                    }
                }

                this.SubDataSource?.AcceptChanges();

                this.dataSource = arlist;

                this.ForumList1.DataSource = this.dataSource;
            }
        }

        /// <summary>
        /// Gets or sets the sub data source.
        /// </summary>
        /// <value>
        /// The sub data source.
        /// </value>
        private DataTable SubDataSource { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the "Forum Link Text" for the ForumList control.
        ///   Automatically disables the link if the current user doesn't
        ///   have proper permissions.
        /// </summary>
        /// <param name="row">
        /// Current data row
        /// </param>
        /// <returns>
        /// Forum link text
        /// </returns>
        public string GetForumLink([NotNull] DataRow row)
        {
            var forumID = row["ForumID"].ToType<int>();

            // get the Forum Description
            var output = row["Forum"].ToString();

            if (row["ReadAccess"].ToType<int>() > 0)
            {
                if (row["RemoteURL"] != DBNull.Value)
                {
                    output =
                        $"<a href=\"{row["RemoteURL"]}\" title=\"{this.GetText("COMMON", "VIEW_FORUM")}\" target=\"_blank\">{this.Page.HtmlEncode(output)}&nbsp;<i class=\"fa fa-external-link-alt fa-fw\"></i></a>";
                }
                else
                {
                    output =
                        $"<a href=\"{YafBuildLink.GetLink(ForumPages.topics, "f={0}&name={1}", forumID, output)}\" title=\"{this.GetText("COMMON", "VIEW_FORUM")}\">{this.Page.HtmlEncode(output)}</a>";
                }
            }
            else
            {
                // no access to this forum
                output = $"{output} {this.GetText("NO_FORUM_ACCESS")}";
            }

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ItemCreated event of the ForumList1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void ForumList1_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var row = (DataRow)e.Item.DataItem;
            var flags = new ForumFlags(row["Flags"]);

            var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                forumId: row["ForumID"].ToType<int>(),
                topicId: row["LastTopicID"].ToType<int>(),
                forumReadOverride: row["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime(),
                topicReadOverride: row["LastTopicAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

            var lastPosted = row["LastPosted"].ToType<DateTime?>() ?? lastRead;

            if (row["ImageUrl"].ToString().IsNotSet())
            {
                var forumIcon = e.Item.FindControlAs<PlaceHolder>("ForumIcon");

                var icon = new Literal { Text = "<i class=\"fa fa-comments fa-2x\" style=\"color: green\"></i>" };

                try
                {
                    if (flags.IsLocked)
                    {
                        icon.Text =
                            "<span class=\"fa-stack fa-1x\"><i class=\"fa fa-comments fa-stack-2x\"></i><i class=\"fa fa-lock fa-stack-1x fa-inverse\" style=\"color: orange;\"></i></span>";
                    }
                    else if (lastPosted > lastRead && row["ReadAccess"].ToType<int>() > 0)
                    {
                        icon.Text = "<i class=\"fa fa-comments fa-2x\" style=\"color: green\"></i>";
                    }
                    else
                    {
                        icon.Text = "<i class=\"fa fa-comments fa-2x\"></i>";
                    }
                }
                catch
                {
                }

                forumIcon.Controls.Add(icon);
            }
            else
            {
                var forumImage = e.Item.FindControlAs<HtmlImage>("ForumImage1");
                if (forumImage != null)
                {
                    forumImage.Src =
                        $"{YafForumInfo.ForumServerFileRoot}{YafBoardFolders.Current.Forums}/{row["ImageUrl"]}";

                    // TODO: vzrus: needs to be moved to css and converted to a more light control in the future.
                    // Highlight custom icon images and add tool tips to them. 
                    try
                    {
                        forumImage.Attributes.Clear();

                        if (flags.IsLocked)
                        {
                            forumImage.Attributes.Add("class", "forum_customimage_locked");
                            forumImage.Attributes.Add("alt", this.GetText("ICONLEGEND", "FORUM_LOCKED"));
                            forumImage.Attributes.Add("title", this.GetText("ICONLEGEND", "FORUM_LOCKED"));
                            forumImage.Attributes.Add(
                                "src",
                                $"{YafForumInfo.ForumServerFileRoot}{YafBoardFolders.Current.Forums}/{row["ImageUrl"]}");
                        }
                        else if (lastPosted > lastRead)
                        {
                            forumImage.Attributes.Add("class", "forum_customimage_newposts");
                            forumImage.Attributes.Add("alt", this.GetText("ICONLEGEND", "NEW_POSTS"));
                            forumImage.Attributes.Add("title", this.GetText("ICONLEGEND", "NEW_POSTS"));
                            forumImage.Attributes.Add(
                                "src",
                                $"{YafForumInfo.ForumServerFileRoot}{YafBoardFolders.Current.Forums}/{row["ImageUrl"]}");
                        }
                        else
                        {
                            forumImage.Attributes.Add("class", "forum_customimage_nonewposts");
                            forumImage.Attributes.Add(
                                "src",
                                $"{YafForumInfo.ForumServerFileRoot}{YafBoardFolders.Current.Forums}/{row["ImageUrl"]}");
                            forumImage.Attributes.Add("alt", this.GetText("ICONLEGEND", "NO_NEW_POSTS"));
                            forumImage.Attributes.Add("title", this.GetText("ICONLEGEND", "NO_NEW_POSTS"));
                        }

                        forumImage.Visible = true;
                    }
                    catch
                    {
                    }

                    forumImage.Visible = true;
                }
            }

            if (!this.Get<YafBoardSettings>().ShowModeratorList)
            {
                return;
            }

            var moderatorSpan = e.Item.FindControl("ModListMob_Span") as HtmlGenericControl;
            var modList1 = e.Item.FindControl("ForumModeratorListMob") as ForumModeratorList;

            if (modList1 != null)
            {
                var dra = row.GetChildRows("FK_Moderator_Forum");
                if (dra.GetLength(0) > 0)
                {
                    modList1.DataSource = dra;
                    modList1.Visible = true;
                    modList1.DataBind();

                    // set them as visible...
                    if (moderatorSpan != null)
                    {
                        moderatorSpan.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the moderated.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The get moderated.
        /// </returns>
        protected bool GetModerated([NotNull] object o)
        {
            return ((DataRow)o)["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
        }

        // Suppress rendering of footer if there is one or more 

        /// <summary>
        /// Gets the moderators footer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <returns>
        /// The get moderators footer.
        /// </returns>
        [NotNull]
        protected string GetModeratorsFooter([NotNull] Repeater sender)
        {
            if (sender.DataSource is DataRow[] && ((DataRow[])sender.DataSource).Length < 1)
            {
                return "-";
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the sub Forums.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>Returns the Sub Forums</returns>
        protected IEnumerable GetSubforums([NotNull] DataRow row)
        {
            if (!this.HasSubforums(row))
            {
                return null;
            }

            var arlist = new ArrayList();

            foreach (var subrow in this.SubDataSource.Rows.Cast<DataRow>()
                .Where(subrow => row["ForumID"].ToType<int>() == subrow["ParentID"].ToType<int>()).Where(
                    subrow => arlist.Count < this.Get<YafBoardSettings>().SubForumsInForumList))
            {
                arlist.Add(subrow);
            }

            this.SubDataSource.AcceptChanges();

            return arlist;
        }

        /// <summary>
        /// Gets the viewing.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The get viewing.
        /// </returns>
        protected string GetViewing([NotNull] object o)
        {
            var row = (DataRow)o;
            var viewing = row["Viewing"].ToType<int>();

            return viewing > 0
                       ? $"<i class=\"far fa-eye\" title=\"{this.GetTextFormatted("VIEWING", viewing)}\"></i> {viewing}"
                       : string.Empty;
        }

        /// <summary>
        /// Determines whether the specified row has sub forums.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        /// The has sub forums.
        /// </returns>
        protected bool HasSubforums([NotNull] DataRow row)
        {
            return this.SubDataSource != null && this.SubDataSource.Rows.Cast<DataRow>()
                       .Any(subrow => row["ForumID"].ToType<int>() == subrow["ParentID"].ToType<int>());
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.AltLastPost = this.GetText("DEFAULT", "GO_LAST_POST");
        }

        /// <summary>
        /// Gets the Posts string
        /// </summary>
        /// <param name="_o">The _o.</param>
        /// <returns>Returns the Posts string</returns>
        protected string Posts([NotNull] object _o)
        {
            var row = (DataRow)_o;

            return row["RemoteURL"] == DBNull.Value ? $"{row["Posts"]:N0}" : "-";
        }

        /// <summary>
        /// Gets the Topics string
        /// </summary>
        /// <param name="_o">The _o.</param>
        /// <returns>Returns the Topics string</returns>
        protected string Topics([NotNull] object _o)
        {
            var row = (DataRow)_o;
            return row["RemoteURL"] == DBNull.Value ? $"{row["Topics"]:N0}" : "-";
        }

        #endregion
    }
}