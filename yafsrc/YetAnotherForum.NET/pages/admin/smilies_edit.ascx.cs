/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for smilies_edit.
	/// </summary>
	public partial class smilies_edit : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
				PageLinks.AddLink("Administration",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				PageLinks.AddLink("Smilies","");

				BindData();
			}			
		}

		private void BindData() 
		{
			
			using(DataTable dt = new DataTable("Files")) 
			{
				dt.Columns.Add("FileID",typeof(long));
				dt.Columns.Add("FileName",typeof(string));
				dt.Columns.Add("Description",typeof(string));
				DataRow dr = dt.NewRow();
				dr["FileID"] = 0;
				dr["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
				dr["Description"] = "Select Smiley Image";
				dt.Rows.Add(dr);
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Request.MapPath(String.Format("{0}images/emoticons",YafForumInfo.ForumRoot)));
				System.IO.FileInfo[] files = dir.GetFiles("*.*");
				long nFileID = 1;
				foreach(System.IO.FileInfo file in files) 
				{
					string sExt = file.Extension.ToLower();
					if(sExt!=".png" && sExt!=".gif" && sExt!=".jpg")
						continue;
					
					dr = dt.NewRow();
					dr["FileID"] = nFileID++;
					dr["FileName"] = file.Name;
					dr["Description"] = file.Name;
					dt.Rows.Add(dr);
				}
				
				Icon.DataSource = dt;
				Icon.DataValueField = "FileName";
				Icon.DataTextField = "Description";
			}
			DataBind();

			if(Request["s"]!=null) 
			{
				using(DataTable dt = YAF.Classes.Data.DB.smiley_list(PageContext.PageBoardID,Request.QueryString["s"])) 
				{
					if(dt.Rows.Count>0) 
					{
						Code.Text = dt.Rows[0]["Code"].ToString();
						Emotion.Text = dt.Rows[0]["Emoticon"].ToString();
						if (Icon.Items.FindByText(dt.Rows[0]["Icon"].ToString()) != null) Icon.Items.FindByText(dt.Rows[0]["Icon"].ToString()).Selected = true;
						Preview.Src = String.Format("{0}images/emoticons/{1}",YafForumInfo.ForumRoot,dt.Rows[0]["Icon"]);
						SortOrder.Text = dt.Rows[0]["SortOrder"].ToString();		// Ederon : 9/4/2007
					}
				}
			}
			else
			{
				Preview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
			}
			Icon.Attributes["onchange"] = String.Format(
				"getElementById('{1}').src='{0}images/emoticons/' + this.value",
				YafForumInfo.ForumRoot,
				Preview.ClientID
				);
		}

		private void save_Click(object sender, System.EventArgs e) 
		{
			string code		= Code.Text.Trim();
			string emotion	= Emotion.Text.Trim();
			string icon		= Icon.SelectedItem.Text.Trim();
			int sortOrder;

			if(code.Length==0) 
			{
				PageContext.AddLoadMessage("Please enter the code to use for this emotion.");
				return;
			}
			if(emotion.Length==0) 
			{
				PageContext.AddLoadMessage("Please enter the emotion for this icon.");
				return;
			}
			if(Icon.SelectedIndex<1) 
			{
				PageContext.AddLoadMessage("Please select an icon to use for this emotion.");
				return;
			}
			// Ederon 9/4/2007
			if (!int.TryParse(SortOrder.Text, out sortOrder) || sortOrder < 0 || sortOrder > 255)
			{
				PageContext.AddLoadMessage("Sort order must be number between 0 and 255.");
				return;
			}

			YAF.Classes.Data.DB.smiley_save(Request.QueryString["s"], PageContext.PageBoardID, code, icon, emotion, sortOrder, 0);

			// invalidate the cache...
			YafCache.Current.Remove( YafCache.GetBoardCacheKey( Constants.Cache.Smilies ) );
			YAF.Classes.UI.ReplaceRulesCreator.ClearCache();

			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_smilies);
		}

		private void cancel_Click(object sender, System.EventArgs e) 
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_smilies);
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			save.Click += new System.EventHandler(save_Click);
			cancel.Click += new System.EventHandler(cancel_Click);
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
		}
		#endregion
	}
}
