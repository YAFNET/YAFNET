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
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for cp_subscriptions.
	/// </summary>
    public partial class cp_changepassword : ForumPage
	{

		public cp_changepassword() : base("CP_CHANGEPASSWORD")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(User==null)
			{
				if(CanLogin)
					Forum.Redirect( ForumPages.login,"ReturnUrl={0}",Utils.GetSafeRawUrl());
				else
					Forum.Redirect( ForumPages.forum);
			}

            if (!IsPostBack)
            {
                PageLinks.AddLink(BoardSettings.Name, Forum.GetLink( ForumPages.forum));
                PageLinks.AddLink(PageUserName, Forum.GetLink( ForumPages.cp_profile));
                //TODO PageLinks.AddLink(GetText("TITLE"), "dd");
                PageLinks.AddLink("Change Password", Forum.GetLink( ForumPages.cp_changepassword));
            }
        }

        void ChangePassword1_CancelButtonClick(object sender, EventArgs e)
        {
            Forum.Redirect( ForumPages.cp_profile);
        }

		override protected void OnInit(EventArgs e)
		{
            this.Load += new System.EventHandler(this.Page_Load);
            ChangePassword1.CancelButtonClick += new EventHandler(ChangePassword1_CancelButtonClick);
            ChangePassword1.ChangedPassword += new EventHandler(ChangePassword1_ChangedPassword);
            base.OnInit(e);
		}

        void ChangePassword1_ChangedPassword(object sender, EventArgs e)
        {
            Forum.Redirect( ForumPages.cp_profile);
        }
	}
}
