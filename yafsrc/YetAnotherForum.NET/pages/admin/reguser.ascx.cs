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
namespace YAF.Pages.Admin
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.Security;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using YAF.Classes.Utils;

	/// <summary>
	///		Summary description for reguser.
	/// </summary>
	public partial class reguser : YAF.Classes.Base.AdminPage
	{
    

		protected void Page_Load(object sender, System.EventArgs e)
		{
      if(!IsPostBack) 
      {
        PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
        PageLinks.AddLink("Administration",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
        PageLinks.AddLink("Users","");

        TimeZones.DataSource = YafStaticData.TimeZones();
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

		}
		#endregion
		
    protected void cancel_Click(object sender,EventArgs e) 
    {
      YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users);
    }
		
    protected void ForumRegister_Click(object sender, System.EventArgs e)
    {
      if(Page.IsValid) 
      {
        if(!General.IsValidEmail(Email.Text))
        {
          PageContext.AddLoadMessage("You have entered an illegal e-mail address.");
          return;
        }

        if(YAF.Classes.Data.DB.user_find(PageContext.PageBoardID,false,UserName.Text,Email.Text).Rows.Count>0) 
        {
          PageContext.AddLoadMessage("Username or email are already registered.");
          return;
        }

				string hashinput = DateTime.Now.ToString() + Email.Text + Security.CreatePassword( 20 );
				string hash = FormsAuthentication.HashPasswordForStoringInConfigFile( hashinput, "md5" );

        if (YAF.Classes.Data.DB.user_register(PageContext.PageBoardID,UserName.Text,Password.Text,hash,Email.Text,Location.Text,HomePage.Text,TimeZones.SelectedItem.Value,!PageContext.BoardSettings.EmailVerification))
				{

					if ( PageContext.BoardSettings.EmailVerification )
					{
						//  Build a MailMessage
						string body = General.ReadTemplate( "verifyemail.txt" );
						body = body.Replace( "{link}", String.Format( "{1}{0}", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.approve, "k={0}", hash ), YAF.Classes.Utils.YafForumInfo.ServerURL ) );
						body = body.Replace( "{key}", hash );
						body = body.Replace( "{forumname}", PageContext.BoardSettings.Name );
						body = body.Replace( "{forumlink}", String.Format( "{0}", ForumURL ) );

						General.SendMail( PageContext.BoardSettings.ForumEmail, Email.Text, String.Format( "{0} email verification", PageContext.BoardSettings.Name ), body );
					}

					// success
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_reguser);
				}
      }
    }
		
	}
}
