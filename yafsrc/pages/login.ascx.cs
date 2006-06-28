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
using System.Web.Security;

namespace yaf.pages
{
	/// <summary>
	/// Summary description for login.
	/// </summary>
	public partial class login : ForumPage
	{
	
		public login() : base("LOGIN")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!CanLogin)
				Forum.Redirect(Pages.forum);

			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
                Login1.CreateUserText = "Sign up for a new account.";
                Login1.CreateUserUrl = Forum.GetLink(Pages.register);
                Login1.PasswordRecoveryText = "Forgot your password?";
                Login1.PasswordRecoveryUrl = Forum.GetLink(Pages.recoverpassword);

				// set the focus using Damien McGivern client-side focus class
				//TODO McGiv.Web.UI.ClientSideFocus.setFocus(UserName);
			}
		}

		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
            Login1.LoggedIn += new EventHandler(Login1_LoggedIn);
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
		}

        void Login1_LoggedIn(object sender, EventArgs e)
        {
            if (Request.QueryString["ReturnUrl"] != null)
                Response.Redirect(Request.QueryString["ReturnUrl"]);
            else
                Forum.Redirect(Pages.forum);
        }
	}
}
