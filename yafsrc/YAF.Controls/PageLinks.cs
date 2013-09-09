/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Controls
{
    #region Using

    using System.Data;
    using System.Web.UI;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Page Links Control.
    /// </summary>
    public class PageLinks : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets LinkedPageLinkID.
        /// </summary>
        [CanBeNull]
        public string LinkedPageLinkID
        {
            get
            {
                return this.ViewState["LinkedPageLinkID"] != null ? this.ViewState["LinkedPageLinkID"].ToString() : null;
            }

            set
            {
                this.ViewState["LinkedPageLinkID"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets PageLinkDT.
        /// </summary>
        [CanBeNull]
        public DataTable PageLinkDT
        {
            get
            {
                if (this.ViewState["PageLinkDT"] != null)
                {
                    return this.ViewState["PageLinkDT"] as DataTable;
                }

                return null;
            }

            set
            {
                this.ViewState["PageLinkDT"] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the forum links.
        /// </summary>
        /// <param name="forumID">The forum id.</param>
        public void AddForumLinks(int forumID)
        {
            this.AddForumLinks(forumID, false);
        }

        /// <summary>
        /// Adds the forum links.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="noForumLink">
        /// The no forum link.
        /// </param>
        public void AddForumLinks(int forumID, bool noForumLink)
        {
            using (DataTable dtLinks = LegacyDb.forum_listpath(forumID))
            {
                foreach (DataRow row in dtLinks.Rows)
                {
                    if (noForumLink && row["ForumID"].ToType<int>() == forumID)
                    {
                        this.AddLink(row["Name"].ToString(), string.Empty);
                    }
                    else
                    {
                        this.AddLink(
                            row["Name"].ToString(),
                            YafBuildLink.GetLink(ForumPages.topics, "f={0}", row["ForumID"]));
                    }
                }
            }
        }

        /// <summary>
        /// Add a link.
        /// </summary>
        /// <param name="title">The title.</param>
        public void AddLink([NotNull] string title)
        {
            this.AddLink(title, string.Empty);
        }

        /// <summary>
        /// Add a link.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        public void AddLink([NotNull] string title, [NotNull] string url)
        {
            DataTable dt = this.PageLinkDT;

            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("Title", typeof(string));
                dt.Columns.Add("URL", typeof(string));
                this.PageLinkDT = dt;
            }

            DataRow dr = dt.NewRow();
            dr["Title"] = title;
            dr["URL"] = url;
            dt.Rows.Add(dr);
        }

        /// <summary>
        /// Clear all Links
        /// </summary>
        public void Clear()
        {
            this.PageLinkDT = null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            DataTable linkDataTable = null;

            if (this.LinkedPageLinkID.IsSet())
            {
                // attempt to get access to the other control...
                var plControl = this.Parent.FindControl(this.LinkedPageLinkID) as PageLinks;

                if (plControl != null)
                {
                    // use the other data stream...
                    linkDataTable = plControl.PageLinkDT;
                }
            }
            else
            {
                // use the data table from this control...
                linkDataTable = this.PageLinkDT;
            }

            if (linkDataTable == null || linkDataTable.Rows.Count == 0)
            {
                return;
            }

            writer.WriteLine(@"<div id=""{0}"" class=""yafPageLink breadcrumb"">".FormatWith(this.ClientID));

            var first = true;

            foreach (DataRow row in linkDataTable.Rows)
            {
                if (!first)
                {
                    writer.WriteLine(@"<span class=""linkSeperator divider"">&nbsp;&#187;&nbsp;</span>");
                }
                else
                {
                    first = false;
                }

                string title = this.HtmlEncode(row["Title"].ToString().Trim());
                string url = row["URL"].ToString().Trim();

                writer.WriteLine(
                    url.IsNotSet()
                        ? @"<span class=""currentPageLink active"">{0}</span>".FormatWith(title)
                        : @"<a href=""{0}"">{1}</a>".FormatWith(url, title));
            }

            writer.WriteLine("</div>");
        }

        #endregion
    }
}