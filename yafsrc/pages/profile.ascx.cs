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

namespace yaf.pages
{
	/// <summary>
	/// Summary description for profile.
	/// </summary>
	public class profile : ForumPage
	{
		protected System.Web.UI.WebControls.Label Name;
		protected System.Web.UI.WebControls.Label Joined;
		protected System.Web.UI.WebControls.Label Email;
		protected System.Web.UI.HtmlControls.HtmlTableRow EmailRow;
		protected System.Web.UI.WebControls.Label LastVisit;
		protected System.Web.UI.WebControls.Label UserName;
		protected Repeater Groups, LastPosts;
		protected Label Rank, Location;
		protected PlaceHolder ModeratorInfo;
		protected HtmlTableRow SuspendedRow;
		protected DropDownList SuspendUnit;
		protected TextBox SuspendCount;
		protected Button RemoveSuspension, Suspend;
		protected HyperLink HomePage, Weblog;
		protected HtmlTableCell Stats, MSN, YIM, AIM, ICQ, RealName, Occupation, Interests, Gender;
		protected Image Avatar;
		protected controls.PageLinks PageLinks;
	
		public profile() : base("PROFILE")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["u"] == null)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				SuspendUnit.Items.Add(new ListItem(GetText("DAYS"),"1"));
				SuspendUnit.Items.Add(new ListItem(GetText("HOURS"),"2"));
				SuspendUnit.Items.Add(new ListItem(GetText("MINUTES"),"3"));
				SuspendUnit.Items.FindByValue("2").Selected = true;
				SuspendCount.Text = "2";

				BindData();
			}
		}

		private void BindData() 
		{
			using(DataTable dt = DB.user_list(Request.QueryString["u"],true)) 
			{
				DataRow user = dt.Rows[0];

				PageLinks.AddLink(Config.ForumSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(GetText("MEMBERS"),Forum.GetLink(Pages.members));
				PageLinks.AddLink(user["Name"].ToString(),Request.RawUrl);
				UserName.Text = (string)user["Name"];
				Name.Text = (string)user["Name"];
				Joined.Text = String.Format("{0}",FormatDateLong((DateTime)user["Joined"]));
				Email.Text = user["Email"].ToString();
				LastVisit.Text = FormatDateTime((DateTime)user["LastVisit"]);
				Rank.Text = user["RankName"].ToString();
				Location.Text = user["Location"].ToString();
				HomePage.Text = user["HomePage"].ToString();
				HomePage.NavigateUrl = user["HomePage"].ToString();
				
				double dAllPosts = 0.0;
				if((int)user["NumPostsForum"]>0) 
					dAllPosts = 100.0 * (int)user["NumPosts"] / (int)user["NumPostsForum"];

				Stats.InnerHtml = String.Format("{0:N0}<br/>[{1} / {2}]",
					user["NumPosts"],
					String.Format(GetText("NUMALL"),dAllPosts),
					String.Format(GetText("NUMDAY"),(double)(int)user["NumPosts"] / (int)user["NumDays"])
					);

				MSN.InnerText = user["MSN"].ToString();
				YIM.InnerText = user["YIM"].ToString();
				AIM.InnerText = user["AIM"].ToString();
				ICQ.InnerText = user["ICQ"].ToString();
				RealName.InnerText = user["RealName"].ToString();
				Weblog.Text = user["Weblog"].ToString();
				Weblog.NavigateUrl = user["Weblog"].ToString();
				Interests.InnerText = user["Interests"].ToString();
				Occupation.InnerText = user["Occupation"].ToString();
				Gender.InnerText = GetText("GENDER" + user["Gender"].ToString());

				if((bool)user["AvatarUpload"] && user["HasAvatarImage"]!=null && long.Parse(user["HasAvatarImage"].ToString())>0) 
				{
					Avatar.ImageUrl = ForumRoot + "image.aspx?u=" + (Request.QueryString["u"]);
				} 
				else if((bool)user["AvatarRemote"] && user["Avatar"].ToString().Length>0) 
				{
					Avatar.ImageUrl = String.Format("{3}image.aspx?url={0}&width={1}&height={2}",
						Server.UrlEncode(user["Avatar"].ToString()),
						user["AvatarWidth"],
						user["AvatarHeight"],
						ForumRoot);
				} 
				else 
				{
					Avatar.Visible = false;
				}

				Groups.DataSource = DB.usergroup_list(Request.QueryString["u"]);

				EmailRow.Visible = IsAdmin;
				ModeratorInfo.Visible = IsAdmin || IsForumModerator;
				SuspendedRow.Visible = !user.IsNull("Suspended");
				if(!user.IsNull("Suspended"))
					ViewState["SuspendedTo"] = FormatDateTime(user["Suspended"]);

				RemoveSuspension.Text = GetText("REMOVESUSPENSION");
				Suspend.Text = GetText("SUSPEND");
			}

			LastPosts.DataSource = DB.post_last10user(Request.QueryString["u"],PageUserID);
			
			DataBind();
		}

		private void Suspend_Click(object sender, System.EventArgs e) 
		{
			/// Admins can suspend anyone not admins
			/// Forum Moderators can suspend anyone not admin or forum moderator
			using(DataTable dt=DB.user_list(Request.QueryString["u"],null)) 
			{
				foreach(DataRow row in dt.Rows) 
				{
					if(int.Parse(row["IsAdmin"].ToString())>0) 
					{
						AddLoadMessage(GetText("ERROR_ADMINISTRATORS"));
						return;
					} 
					if(!IsAdmin && int.Parse(row["IsForumModerator"].ToString())>0) 
					{
						AddLoadMessage(GetText("ERROR_FORUMMODERATORS"));
						return;
					}
				}
			}

			DateTime suspend = DateTime.Now;
			int count = int.Parse(SuspendCount.Text);
			switch(SuspendUnit.SelectedValue) 
			{
				case "1":
					suspend += new TimeSpan(count,0,0,0);
					break;
				case "2":
					suspend += new TimeSpan(0,count,0,0);
					break;
				case "3":
					suspend += new TimeSpan(0,0,count,0);
					break;
			}

			DB.user_suspend(Request.QueryString["u"],suspend);
			BindData();
		}

		private void RemoveSuspension_Click(object sender, System.EventArgs e) 
		{
			DB.user_suspend(Request.QueryString["u"],null);
			BindData();
		}

		protected string GetSuspendedTo() 
		{
			if(ViewState["SuspendedTo"]!=null)
				return (string)ViewState["SuspendedTo"];
			else
				return "";
		}

		protected string FormatBody(object o) 
		{
			DataRowView row = (DataRowView)o;
			string html = row["Message"].ToString();

			if(html.IndexOf('<')<0) 
			{
				html = FormatMsg.ForumCodeToHtml(this,html);
			}

			if(row["Signature"].ToString().Length>0) 
			{
				string sig = row["Signature"].ToString();
				if(sig.IndexOf('<')<0) 
				{
					sig = FormatMsg.ForumCodeToHtml(this,sig);
				}
				
				html += "<br/><hr noshade/>" + sig;
			}

			return FormatMsg.FetchURL(this,html);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			RemoveSuspension.Click += new EventHandler(RemoveSuspension_Click);
			Suspend.Click += new EventHandler(Suspend_Click);
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
