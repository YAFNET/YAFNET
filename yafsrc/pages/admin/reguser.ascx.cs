/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørn Atle Isaksen
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
namespace yaf.pages.admin
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for reguser.
	/// </summary>
	public partial class reguser : AdminPage
	{
    

		private void Page_Load(object sender, System.EventArgs e)
		{
      if(!IsPostBack) 
      {
        PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
        PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
        PageLinks.AddLink("Users","");

        TimeZones.DataSource = Data.TimeZones();
        DataBind();
        TimeZones.Items.FindByValue("0").Selected = true;
      }
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ForumRegister.Click += new System.EventHandler(this.ForumRegister_Click);
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
		
    private void cancel_Click(object sender,EventArgs e) 
    {
      Forum.Redirect(Pages.admin_users);
    }
		
    private void ForumRegister_Click(object sender, System.EventArgs e)
    {
      if(Page.IsValid) 
      {
        if(!Utils.IsValidEmail(Email.Text))
        {
          AddLoadMessage("You have entered an illegal e-mail address.");
          return;
        }

        if(DB.user_find(PageBoardID,false,UserName.Text,Email.Text).Rows.Count>0) 
        {
          AddLoadMessage("Username or email are already registered.");
          return;
        }

				if (DB.user_register(this,PageBoardID,UserName.Text,Password.Text,Email.Text,Location.Text,HomePage.Text,TimeZones.SelectedItem.Value,BoardSettings.EmailVerification,DBNull.Value))
				{
					// success
					Forum.Redirect(Pages.admin_reguser);
				}
      }
    }
		
	}
}
