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

namespace yaf.controls
{
	using System;
	using System.Data;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for smileys.
	/// </summary>
	public abstract class smileys : BaseUserControl
	{
		protected DataTable dtSmileys;
		private string _onclick;

		private void Page_Load(object sender, System.EventArgs e)
		{
			dtSmileys = DB.smiley_listunique();
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			System.Text.StringBuilder html = new System.Text.StringBuilder();
			html.AppendFormat("<table align=center cellpadding=9>");
			html.AppendFormat("<tr>");
			for(int i=0;i<dtSmileys.Rows.Count;i++) {
				DataRow row = dtSmileys.Rows[i];
				if(i%8==0 && i>0 && i+1<dtSmileys.Rows.Count) html.Append("</tr><tr>\n");
				string evt = "";
				if(_onclick.Length>0) {
					evt = String.Format("javascript:{0}('{1}','{3}images/emoticons/{2}')",_onclick,row["Code"],row["Icon"],ForumPage.ForumRoot);
				} else {
					evt = "javascript:void()";
				}
				html.AppendFormat("<td><a tabindex=\"999\" href=\"{2}\"><img src=\"{0}\" title=\"{1}\"/></a></td>\n",ForumPage.Smiley((string)row["Icon"]),row["Emoticon"],evt);
			}
			html.AppendFormat("</tr>");
			html.AppendFormat("</table>");
			writer.Write(html);
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		public string onclick {
			set {
				_onclick = value;
			}
		}
	}
}
