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

namespace yaf.pages.admin
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class editaccessmask : AdminPage {
	
		protected void Page_Load(object sender, System.EventArgs e) 
		{
			if(!IsPostBack) {
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Access Masks","");

				BindData();
				if(Request.QueryString["i"] != null) {
					using(DataTable dt = DB.accessmask_list(PageBoardID,Request.QueryString["i"])) 
					{
						DataRow row = dt.Rows[0];
						Name.Text				= (string)row["Name"];
						ReadAccess.Checked		= BitSet(row["Flags"],(int)AccessFlags.ReadAccess);
						PostAccess.Checked		= BitSet(row["Flags"],(int)AccessFlags.PostAccess);
						ReplyAccess.Checked		= BitSet(row["Flags"],(int)AccessFlags.ReplyAccess);
						PriorityAccess.Checked	= BitSet(row["Flags"],(int)AccessFlags.PriorityAccess);
						PollAccess.Checked		= BitSet(row["Flags"],(int)AccessFlags.PollAccess);
						VoteAccess.Checked		= BitSet(row["Flags"],(int)AccessFlags.VoteAccess);
						ModeratorAccess.Checked	= BitSet(row["Flags"],(int)AccessFlags.ModeratorAccess);
						EditAccess.Checked		= BitSet(row["Flags"],(int)AccessFlags.EditAccess);
						DeleteAccess.Checked	= BitSet(row["Flags"],(int)AccessFlags.DeleteAccess);
						UploadAccess.Checked	= BitSet(row["Flags"],(int)AccessFlags.UploadAccess);
					}
				}
			}
		}

		private void BindData() {
			DataBind();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		protected void Save_Click(object sender, System.EventArgs e)
		{
			// Forum
			object accessMaskID = null;
			if(Request.QueryString["i"]!=null)
				accessMaskID = Request.QueryString["i"];

			DB.accessmask_save(accessMaskID,
				PageBoardID,
				Name.Text,
				ReadAccess.Checked,
				PostAccess.Checked,
				ReplyAccess.Checked,
				PriorityAccess.Checked,
				PollAccess.Checked,
				VoteAccess.Checked,
				ModeratorAccess.Checked,
				EditAccess.Checked,
				DeleteAccess.Checked,
				UploadAccess.Checked);
			Forum.Redirect(Pages.admin_accessmasks);
		}

		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			Forum.Redirect(Pages.admin_accessmasks);
		}

		protected bool BitSet(object _o,int bitmask) 
		{
			int i = (int)_o;
			return (i & bitmask)!=0;
		}
	}
}
