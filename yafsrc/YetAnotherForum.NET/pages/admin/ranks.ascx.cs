/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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

using System.Drawing;

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Controls;
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
    /// Summary description for ranks.
    /// </summary>
    public partial class ranks : AdminPage
    {
        #region Methods

        /// <summary>
        /// Format string color.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Set values  are rendered green if true, and if not red
        /// </returns>
        protected Color GetItemColorString(string item)
        {
            // show enabled flag red
            return item.IsSet() ? Color.Green : Color.Red;
        }

        /// <summary>
        /// Format access mask setting color formatting.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <returns>
        /// Set access mask flags  are rendered green if true, and if not red
        /// </returns>
        protected Color GetItemColor(bool enabled)
        {
            // show enabled flag red
            return enabled ? Color.Green : Color.Red;
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
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("ADMIN_RANKS", "CONFIRM_DELETE"));
        }

        /// <summary>
        /// The ladder info.
        /// </summary>
        /// <param name="_o">
        /// The _o.
        /// </param>
        /// <returns>
        /// The ladder info.
        /// </returns>
        protected string LadderInfo([NotNull] object _o)
        {
            var dr = (DataRowView)_o;

            // object IsLadder,object MinPosts
            // Eval( "IsLadder"),Eval( "MinPosts")
            bool isLadder = dr["Flags"].BinaryAnd(RankFlags.Flags.IsLadder);

            return isLadder
                       ? "{0} ({1} {2})".FormatWith(GetItemName(true), dr["MinPosts"], this.GetText("ADMIN_RANKS", "POSTS"))
                       : GetItemName(false);
        }

        /// <summary>
        /// The new rank_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void NewRank_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_editrank);
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

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_RANKS", "TITLE"));

            this.Page.Header.Title = "{0} - {1}".FormatWith(
               this.GetText("ADMIN_ADMIN", "Administration"),
               this.GetText("ADMIN_RANKS", "TITLE"));

            this.NewRank.Text = this.GetText("ADMIN_RANKS", "NEW_RANK");

            this.BindData();
        }

        /// <summary>
        /// The rank list_ item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RankList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    YafBuildLink.Redirect(ForumPages.admin_editrank, "r={0}", e.CommandArgument);
                    break;
                case "delete":
                    LegacyDb.rank_delete(e.CommandArgument);
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.RankList.DataSource = LegacyDb.rank_list(this.PageContext.PageBoardID, null);
            this.DataBind();
        }

        #endregion
    }
}