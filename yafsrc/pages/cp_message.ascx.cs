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
using System.Data;
using System.Web.UI.WebControls;
using YAF.Classes.Base;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Classes.Utils;

namespace YAF.Pages // YAF.Pages
{
    /// <summary>
    /// Summary description for inbox.
    /// </summary>
    public partial class cp_message : ForumPage
    {
        public cp_message()
            : base("CP_MESSAGE")
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (User == null)
                yaf_BuildLink.Redirect(ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl());

            if (!IsPostBack)
            {
                if (String.IsNullOrEmpty(Request.QueryString["pm"]))
                    yaf_BuildLink.AccessDenied();
                else
                    BindData();
            }
        }

        protected bool IsOutbox
        {
            get
            {
                if (ViewState["IsOutbox"] == null)
                    return false;
                else
                    return (bool) ViewState["IsOutbox"];
            }
            set { ViewState["IsOutbox"] = value; }
        }

        /// <summary>
        /// Sets the IsOutbox property as appropriate for this private message.
        /// </summary>
        /// <remarks>User id parameters are downcast to object to allow for potential future use of non-integer user id's</remarks>
        /// <param name="fromUserId">User id of the message sender</param>
        /// <param name="toUserId">User id of the message receiver</param>
        /// <param name="messageIsInOutbox">Bool indicating whether the message is in the sender's outbox</param>
        private void SetMessageView(object fromUserId, object toUserId, bool messageIsInOutbox)
        {
            // Set whether the current view is the Inbox or Outbox (sent items)
            IsOutbox = fromUserId.Equals(PageContext.PageUserID);

            // If the message was sent from the user to himself, get the view based on the query string
            if (fromUserId.Equals(toUserId))
                IsOutbox = Request.QueryString["v"] == "out";

            if (IsOutbox && !messageIsInOutbox)
            {
                // If the view is of the user's Outbox but the PM is not in it
                if (!toUserId.Equals(PageContext.PageUserID))
                    // If the PM was not sent to the current viewing user, send the 
                    // user back to the outbox because they are trying to view a PM 
                    // that they have removed from their outbox (either manually or because
                    // they have *just* deleted it by pressing the Delete button while 
                    // viewing it)
                    yaf_BuildLink.Redirect(ForumPages.cp_inbox, "v=out");
                else
                    // If the PM was sent by the current user to himself but was deleted from the outbox,
                    // switch the view to be in Inbox mode
                    IsOutbox = false;
            }
        }

        private void BindData()
        {
            using (DataTable dt = DB.pmessage_list(Request.QueryString["pm"]))
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    if ((int) row["ToUserID"] != PageContext.PageUserID &&
                        (int) row["FromUserID"] != PageContext.PageUserID)
                        yaf_BuildLink.AccessDenied();

                    SetMessageView(row["FromUserID"], row["ToUserID"], (bool)row["IsInOutbox"]);

                    PageLinks.AddLink(PageContext.BoardSettings.Name, yaf_BuildLink.GetLink(ForumPages.forum));
                    PageLinks.AddLink(PageContext.PageUserName, yaf_BuildLink.GetLink(ForumPages.cp_profile));
                    if (IsOutbox)
                        PageLinks.AddLink(GetText("SENTITEMS"), yaf_BuildLink.GetLink(ForumPages.cp_inbox, "v=out"));
                    else
                        PageLinks.AddLink(GetText("INBOX"), yaf_BuildLink.GetLink(ForumPages.cp_inbox));
                    PageLinks.AddLink(HtmlEncode(row["Subject"]), "");

                    Inbox.DataSource = dt;
                }
                else
                    yaf_BuildLink.Redirect(ForumPages.cp_inbox);
            }

            DataBind();

            if (!IsOutbox)
                DB.pmessage_markread(Request.QueryString["pm"]);
        }

        protected void Inbox_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                if (IsOutbox)
                    DB.pmessage_delete(e.CommandArgument, true);
                else
                    DB.pmessage_delete(e.CommandArgument);

                BindData();
                PageContext.AddLoadMessage(GetText("msg_deleted"));
            }
            else if (e.CommandName == "reply")
            {
                yaf_BuildLink.Redirect(ForumPages.pmessage, "p={0}&q=0", e.CommandArgument);
            }
            else if (e.CommandName == "quote")
            {
                yaf_BuildLink.Redirect(ForumPages.pmessage, "p={0}&q=1", e.CommandArgument);
            }
        }

        protected void DeleteMessage_Load(object sender, EventArgs e)
        {
            ((LinkButton) sender).Attributes["onclick"] =
                String.Format("return confirm('{0}')", GetText("confirm_deletemessage"));
        }
    }
}