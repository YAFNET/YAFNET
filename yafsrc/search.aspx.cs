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
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Summary description for topics.
	/// </summary>
	public class search : BasePage
	{
		protected System.Web.UI.WebControls.HyperLink HomeLink, ThisLink;
		protected System.Web.UI.WebControls.DropDownList ForumJump;
		protected System.Web.UI.WebControls.DropDownList DropDownList1;
		protected LinkButton moderate1, moderate2;
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.DropDownList listForum;
		protected System.Web.UI.WebControls.TextBox txtSearchString;
		protected System.Web.UI.WebControls.DropDownList listResInPage;
		protected System.Web.UI.WebControls.DropDownList listSearchWath;
		protected System.Web.UI.WebControls.DropDownList listSearchWhere;
		protected System.Web.UI.WebControls.Label lblPageCounts;
		protected System.Web.UI.WebControls.LinkButton btnPrevPage;
		protected System.Web.UI.WebControls.LinkButton btnNextPage;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks1;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks2;
		protected System.Web.UI.WebControls.Repeater SearchRes;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				HomeLink.NavigateUrl = BaseDir;
				HomeLink.Text = ForumName;
				ThisLink.NavigateUrl = "search.aspx";
				ThisLink.Text = GetText("title");
				btnSearch.Text = GetText("btnsearch");

				// Load result dropdown
				listResInPage.Items.Add(new ListItem(GetText("result5"),"5"));
				listResInPage.Items.Add(new ListItem(GetText("result10"),"10"));
				listResInPage.Items.Add(new ListItem(GetText("result25"),"25"));
				listResInPage.Items.Add(new ListItem(GetText("result50"),"50"));

				// Load searchwhere dropdown
				listSearchWhere.Items.Add(new ListItem(GetText("posts"),"0"));
				listSearchWhere.Items.Add(new ListItem(GetText("postedby"),"1"));

				// Load listSearchWath dropdown
				listSearchWath.Items.Add(new ListItem(GetText("match_all"),"0"));
				listSearchWath.Items.Add(new ListItem(GetText("match_any"),"1"));
				listSearchWath.Items.Add(new ListItem(GetText("match_exact"),"2"));

				// Load forum's combo
				listForum.Items.Add(new ListItem(GetText("allforums"),"-1"));
				DataTable dt = DB.forum_listread(PageUserID,null);

				int nOldCat = 0;
				for(int i=0;i<dt.Rows.Count;i++) 
				{
					DataRow row = dt.Rows[i];
					if((int)row["CategoryID"] != nOldCat) 
					{
						nOldCat = (int)row["CategoryID"];
						listForum.Items.Add(new ListItem((string)row["Category"],"-1"));
					}
					listForum.Items.Add(new ListItem(" - " + (string)row["Forum"],row["ForumID"].ToString()));
				}

				BindData( false );
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.SearchRes.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.SearchRes_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void BindData( bool newSearch )
		{
			if( Request.QueryString["p"] == null && !newSearch )
				return;

			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;
			
			if( newSearch )
			{
				SEARCH_FIELD sf = (SEARCH_FIELD)System.Enum.Parse( typeof( SEARCH_FIELD ), listSearchWhere.SelectedValue );
				SEARCH_WHAT sw = (SEARCH_WHAT)System.Enum.Parse( typeof( SEARCH_WHAT ), listSearchWath.SelectedValue );
				int forumID = int.Parse( listForum.SelectedValue );
				int pages = int.Parse( listResInPage.SelectedValue );

				pds.DataSource = DB.GetSearchResult( txtSearchString.Text, sf, sw, forumID, PageUserID ).DefaultView;
				pds.PageSize = pages;
				Session[ "ds" ] = pds.DataSource;
				pds.CurrentPageIndex = 0;
			}
			else
			{
				pds.PageSize = int.Parse( Request.QueryString["pp"] );
				pds.CurrentPageIndex = int.Parse( Request.QueryString["p"] );
				pds.DataSource = (System.Collections.IEnumerable)Session[ "ds" ];
			}

			if(pds.PageCount>1) 
			{
				PageLinks1.Visible = true;
				PageLinks2.Visible = true;

				PageLinks1.InnerHtml = String.Format("{0} Pages:",pds.PageCount);
				for(int i=0;i<pds.PageCount;i++) 
				{
					if(i==pds.CurrentPageIndex) 
					{
						PageLinks1.InnerHtml += String.Format(" [{0}]",i+1);
					} 
					else 
					{
						PageLinks1.InnerHtml += String.Format(" <a href=\"search.aspx?p={1}&pp={2}\">{0}</a>",i+1,i,pds.PageSize);
					}
				}
				PageLinks2.InnerHtml = PageLinks1.InnerHtml;
			} 
			else 
			{
				PageLinks1.Visible = false;
				PageLinks2.Visible = false;
			}

			SearchRes.DataSource = pds;

			DataBind();
		}

		public string FormatMessage( object o )
		{
			DataRowView row = (DataRowView)o;

			string body = row["Message"].ToString();
			bool isHtml = body.IndexOf('<')>=0;
			if(!isHtml) 
			{
				body = FormatMsg.ForumCodeToHtml(this,body);
			}
			return FormatMsg.FetchURL(this,body);
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			BindData( true );
		}

		private void SearchRes_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			object o = e.Item.FindControl( "CounterCol" );
			if( o != null )
			{
				int rowCount = e.Item.ItemIndex + 1 + (((PagedDataSource)SearchRes.DataSource).CurrentPageIndex * ((PagedDataSource)SearchRes.DataSource).PageSize);


				((System.Web.UI.HtmlControls.HtmlTableCell)o).InnerHtml = string.Format( "&nbsp;<B>{0}</B>.", rowCount );
			}
		}
	}
}
