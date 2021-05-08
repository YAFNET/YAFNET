/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Administration Page for managing medals.
    /// </summary>
    public partial class Medals : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Medals" /> class. 
        ///   Default constructor.
        /// </summary>
        public Medals()
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
            this.PageLinks.AddAdminIndex();

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_MEDALS", "TITLE"), string.Empty);
        }

        /// <summary>
        /// Handles item command of medal list repeater.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void MedalListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            var medalId = e.CommandArgument.ToType<int>();

            switch (e.CommandName)
            {
                case "edit":

                    // edit medal
                    this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditMedal, "medalid={0}", medalId);
                    break;
                case "delete":
                    // delete medal
                    this.GetRepository<UserMedal>().Delete(m => m.MedalID == medalId);
                    this.GetRepository<GroupMedal>().Delete(m => m.MedalID == medalId);

                    this.GetRepository<Medal>().Delete(m => m.ID == medalId);

                    // re-bind data
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
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditMedal);
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // this needs to be done just once, not during post-backs
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
            var output = new StringBuilder();

            var medal = (Medal)data;

            // image of medal
            output.AppendFormat(
                "<img src=\"{0}{3}/{1}\" width=\"{2}\" />",
                BoardInfo.ForumClientFileRoot,
                medal.SmallMedalURL,
                this.GetText("ADMIN_MEDALS", "DISPLAY_BOX"),
                this.Get<BoardFolders>().Medals);

            // if available, create also ribbon bar image of medal
            if (medal.SmallRibbonURL.IsSet())
            {
                output.AppendFormat(
                    " &nbsp; <img src=\"{0}{3}/{1}\" alt=\"{2}\" />",
                    BoardInfo.ForumClientFileRoot,
                    medal.SmallRibbonURL,
                    this.GetText("ADMIN_MEDALS", "DISPLAY_RIBBON"),
                    this.Get<BoardFolders>().Medals);
            }

            return output.ToString();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // list medals for this board
            this.MedalList.DataSource = this.GetRepository<Medal>().Get(m => m.BoardID == this.PageContext.PageBoardID)
                .OrderBy(m => m.Category).ThenBy(m => m.SortOrder);

            // bind data to controls
            this.DataBind();
        }

        #endregion
    }
}