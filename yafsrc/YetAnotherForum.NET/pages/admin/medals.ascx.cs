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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Administration inferface for managing medals.
    /// </summary>
    public partial class medals : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "medals" /> class. 
        ///   Default constructor.
        /// </summary>
        public medals()
            : base("ADMIN_MEDALS")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            // administration index
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_MEDALS", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_MEDALS", "TITLE"));
        }

        /// <summary>
        /// Handles item command of medal list repeater.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void MedalListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":

                    // edit medal
                    YafBuildLink.Redirect(ForumPages.admin_editmedal, "medalid={0}", e.CommandArgument);
                    break;
                case "delete":
                    // delete medal
                    this.GetRepository<Medal>().Delete(e.CommandArgument.ToType<int>());

                    // re-bind data
                    this.BindData();
                    break;
                case "moveup":
                    this.GetRepository<Medal>().Resort(e.CommandArgument.ToType<int>(), -1);
                    this.BindData();
                    break;
                case "movedown":
                    this.GetRepository<Medal>().Resort(e.CommandArgument.ToType<int>(), 1);
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Handles click on new medal button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void NewMedalClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect to medal edit page
            YafBuildLink.Redirect(ForumPages.admin_editmedal);
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // this needs to be done just once, not during postbacks
            if (this.IsPostBack)
            {
                return;
            }

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Formats HTML output to display image representation of a medal.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// HTML markup with image representation of a medal.
        /// </returns>
        [NotNull]
        protected string RenderImages([NotNull] object data)
        {
            var output = new StringBuilder(250);

            var dr = (DataRowView)data;

            // image of medal
            output.AppendFormat(
                "<img src=\"{0}{5}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" align=\"top\" />",
                YafForumInfo.ForumClientFileRoot,
                dr["SmallMedalURL"],
                dr["SmallMedalWidth"],
                dr["SmallMedalHeight"],
                this.GetText("ADMIN_MEDALS", "DISPLAY_BOX"),
                YafBoardFolders.Current.Medals);

            // if available, create also ribbon bar image of medal
            if (!dr["SmallRibbonURL"].IsNullOrEmptyDBField())
            {
                output.AppendFormat(
                    " &nbsp; <img src=\"{0}{5}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" align=\"top\" />",
                    YafForumInfo.ForumClientFileRoot,
                    dr["SmallRibbonURL"],
                    dr["SmallRibbonWidth"],
                    dr["SmallRibbonHeight"],
                    this.GetText("ADMIN_MEDALS", "DISPLAY_RIBBON"),
                    YafBoardFolders.Current.Medals);
            }

            return output.ToString();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // list medals for this board
            this.MedalList.DataSource = this.GetRepository<Medal>().List();

            // bind data to controls
            this.DataBind();
        }

        #endregion
    }
}