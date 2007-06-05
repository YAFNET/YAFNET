/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
    /// <summary>
    /// Summary description for inbox.
    /// </summary>
    public partial class cp_inbox : YAF.Classes.Base.ForumPage
    {

        public cp_inbox()
            : base("CP_INBOX")
        {
        }

        private void SetSort(string field, bool asc)
        {
            if (ViewState["SortField"] != null && (string)ViewState["SortField"] == field)
            {
                ViewState["SortAsc"] = !(bool)ViewState["SortAsc"];
            }
            else
            {
                ViewState["SortField"] = field;
                ViewState["SortAsc"] = asc;
            }
        }

        protected void SubjectLink_Click(object sender, EventArgs e)
        {
            SetSort("Subject", true);
            BindData();
        }

        protected void FromLink_Click(object sender, EventArgs e)
        {
            if (IsOutbox)
                SetSort("ToUser", true);
            else
                SetSort("FromUser", true);
            BindData();
        }

        protected void DateLink_Click(object sender, EventArgs e)
        {
            SetSort("Created", false);
            BindData();
        }

        protected void DeleteSelected_Load(object sender, EventArgs e)
        {
            ((Button)sender).Attributes["onclick"] = String.Format("return confirm('{0}')", GetText("confirm_delete"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (User == null)
                yaf_BuildLink.Redirect(ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl());

            if (!IsPostBack)
            {
                SetSort("Created", false);
                IsOutbox = Request.QueryString["sent"] != null;
                BindData();

                PageLinks.AddLink(PageContext.BoardSettings.Name, yaf_BuildLink.GetLink(ForumPages.forum));
                PageLinks.AddLink(PageContext.PageUserName, yaf_BuildLink.GetLink(ForumPages.cp_profile));
                PageLinks.AddLink(GetText(IsOutbox ? "sentitems" : "title"), "");

                SubjectLink.Text = Server.HtmlEncode(GetText("subject"));
                FromLink.Text = GetText(IsOutbox ? "to" : "from");
                DateLink.Text = GetText("date");
            }
        }

        protected bool IsOutbox
        {
            get
            {
                if (ViewState["IsOutbox"] == null)
                    return false;
                else
                    return (bool)ViewState["IsOutbox"];
            }
            set { ViewState["IsOutbox"] = value; }
        }

        private void BindData()
        {
            object toUserID = null;
            object fromUserID = null;
            if (IsOutbox)
                fromUserID = PageContext.PageUserID;
            else
                toUserID = PageContext.PageUserID;
            using (DataView dv = DB.pmessage_list(toUserID, fromUserID, null).DefaultView)
            {
                if (IsOutbox)
                    dv.RowFilter = "IsInOutbox = True";

                dv.Sort = String.Format("{0} {1}", ViewState["SortField"], (bool)ViewState["SortAsc"] ? "asc" : "desc");
                Inbox.DataSource = dv;
                DataBind();
            }
            if (IsOutbox)
                SortFrom.Visible = (string)ViewState["SortField"] == "ToUser";
            else
                SortFrom.Visible = (string)ViewState["SortField"] == "FromUser";
            SortFrom.Src = GetThemeContents("SORT", (bool)ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
            SortSubject.Visible = (string)ViewState["SortField"] == "Subject";
            SortSubject.Src = GetThemeContents("SORT", (bool)ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
            SortDate.Visible = (string)ViewState["SortField"] == "Created";
            SortDate.Src = GetThemeContents("SORT", (bool)ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
        }

        protected string FormatBody(object o)
        {
            DataRowView row = (DataRowView)o;
            return (string)row["Body"];
        }

        protected void Inbox_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                long nItemCount = 0;
                foreach (RepeaterItem item in Inbox.Items)
                {
                    if (((CheckBox)item.FindControl("ItemCheck")).Checked)
                    {
                        if (IsOutbox)
                            DB.pmessage_delete(((Label)item.FindControl("UserPMessageID")).Text, true);
                        else
                            DB.pmessage_delete(((Label)item.FindControl("UserPMessageID")).Text);
                        nItemCount++;
                    }
                }

                BindData();
                if (nItemCount == 1)
                    PageContext.AddLoadMessage(GetText("msgdeleted1"));
                else
                    PageContext.AddLoadMessage(String.Format(GetText("msgdeleted2"), nItemCount));
            }
        }

        protected string GetImage(object o)
        {
            if ((bool)((DataRowView)o)["IsRead"])
                return GetThemeContents("ICONS", "TOPIC");
            else
                return GetThemeContents("ICONS", "TOPIC_NEW");
        }
    }
}
