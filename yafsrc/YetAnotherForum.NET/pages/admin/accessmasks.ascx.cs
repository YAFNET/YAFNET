/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
    using System.Drawing;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Summary description for forums.
    /// </summary>
    public partial class accessmasks : AdminPage
    {
        /* Construction */
        #region Methods

        /// <summary>
        /// The bit set.
        /// </summary>
        /// <param name="_o">
        /// The _o.
        /// </param>
        /// <param name="bitmask">
        /// The bitmask.
        /// </param>
        /// <returns>
        /// The bit set.
        /// </returns>
        protected bool BitSet([NotNull] object _o, int bitmask)
        {
            var i = (int)_o;
            return (i & bitmask) != 0;
        }

        /// <summary>
        /// Creates navigation page links on top of forum (breadcrumbs).
        /// </summary>
        protected override void CreatePageLinks()
        {
            // board index
            this.PageLinks.AddRoot();

            // administration index
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_ACCESSMASKS", "TITLE"));

            this.Page.Header.Title = "{0} - {1}".FormatWith(
               this.GetText("ADMIN_ADMIN", "Administration"),
               this.GetText("ADMIN_ACCESSMASKS", "TITLE"));
        }

        /* Event Handlers */

        /// <summary>
        /// The delete_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // add on click confirm dialog
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("ADMIN_ACCESSMASKS", "CONFIRM_DELETE"));
        }

        /// <summary>
        /// Format access mask setting color formatting.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <returns>
        /// Set access mask flags are rendered green if true, and if not red
        /// </returns>
        protected string GetItemColor(bool enabled)
        {
            // show enabled flag red
            return enabled ? "tag tag-success" : "tag tag-danger";
        }

        /// <summary>
        /// Get a user friendly item name.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <returns>
        /// Item Name.
        /// </returns>
        protected string GetItemName(bool enabled)
        {
            return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
        }

        /// <summary>
        /// The list_ item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":

                    // redirect to editing page
                    YafBuildLink.Redirect(ForumPages.admin_editaccessmask, "i={0}", e.CommandArgument);
                    break;
                case "delete":

                    // attmempt to delete access masks
                    if (this.GetRepository<AccessMask>().Delete(e.CommandArgument.ToType<int>()))
                    {
                        // remove cache of forum moderators
                        this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);
                        this.BindData();
                    }
                    else
                    {
                        // used masks cannot be deleted
                        this.PageContext.AddLoadMessage(this.GetText("ADMIN_ACCESSMASKS", "MSG_NOT_DELETE"));
                    }

                    // quit switch
                    break;
            }
        }

        /// <summary>
        /// The new_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void New_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect to page for access mask creation
            YafBuildLink.Redirect(ForumPages.admin_editaccessmask);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.New.Text = "<i class=\"fa fa-plus-square fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("ADMIN_ACCESSMASKS", "NEW_MASK"));

            // create links
            this.CreatePageLinks();

            // bind data
            this.BindData();
        }

        /* Methods */

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            // list all access masks for this board
            this.List.DataSource = this.GetRepository<AccessMask>().List();
            this.DataBind();
        }

        #endregion
    }
}