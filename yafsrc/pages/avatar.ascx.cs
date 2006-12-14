/* A mod created by jwendl
 * For loading Avatars...
*/

using System;
using System.IO;
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
//using jwendl.Pager;


namespace YAF.Pages
{
	/// <summary>
	/// Summary description for avatar.
	/// </summary>
	public partial class avatar : ForumPage
	{
		protected System.Web.UI.WebControls.Label title;
	
		public int pagenum = 0;
		public int pagesize = 20;

		private int returnUserID = 0;

		string filepath = "";

		public avatar() : base("AVATAR")
		{
		}

		private string CurrentDir
		{
			get
			{
				if(ViewState["CurrentDir"]!=null)
					return (string)ViewState["CurrentDir"];
				else
					return "";
			}
			set
			{
				ViewState["CurrentDir"] = value;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if ( Request.QueryString ["u"] != null )
		{
				returnUserID = Convert.ToInt32( Request.QueryString ["u"] );
			}

			if(!IsPostBack)
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink( ForumPages.forum));

				if ( returnUserID > 0 )
				{
					PageLinks.AddLink( "Administration", Forum.GetLink( ForumPages.admin_admin ) );
					PageLinks.AddLink( "Users", Forum.GetLink( ForumPages.admin_users ) );
				}
				else
				{				
				PageLinks.AddLink(PageUserName,Forum.GetLink( ForumPages.cp_profile));
					PageLinks.AddLink( GetText( "CP_EDITAVATAR", "TITLE" ), Forum.GetLink( ForumPages.cp_editavatar ) );
				}				
				PageLinks.AddLink(GetText("TITLE"),"");

				pager.PageSize = 20;
				bind_data();
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
			pager.PageChange += new EventHandler(pager_PageChange);
			GoDir.Click += new EventHandler(GoDir_Click);
		}
		#endregion

		private void pager_PageChange(object sender,EventArgs e)
		{
			bind_data();
		}

		private void GoDir_Click(object sender, EventArgs e)
		{
			CurrentDir = Request.Form["__EVENTARGUMENT"];
			bind_data();
		}

		public void files_bind(object sender, DataListItemEventArgs e)
		{
			string strDirectory = Data.ForumRoot + "images/avatars/" + CurrentDir;

			/*
			string pdir = "";
			string[] pardir = CurrentDir.Split('/');
			for (int i=0; i<pardir.Length-1; i++)
				pdir += pardir[i] + "/";
			if(pdir.Length>0) pdir = pdir.Substring(0,pdir.Length-1);
			*/
			
			Literal fname = (Literal)e.Item.FindControl("fname");
	
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				FileInfo finfo = new FileInfo(Server.MapPath(Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name"))));
				string tmpExt = finfo.Extension.ToLower();

				if (tmpExt == ".gif" || tmpExt == ".jpg" || tmpExt == ".jpeg" || tmpExt == ".png" || tmpExt == ".bmp")
				{
					string link;

					if ( returnUserID > 0 )
					{
						link = Forum.GetLink( ForumPages.admin_edituser, "u={0}&av={1}", returnUserID, (CurrentDir + "/" + finfo.Name) );
					}
					else
				{
						link = Forum.GetLink( ForumPages.cp_editavatar, "av=" + CurrentDir + "/" + finfo.Name );
					}

					fname.Text = string.Format( @"<p align=""center""><a href=""{0}""><img src=""{1}"" alt=""{2}"" class=""borderless"" /></a><br /><small>{2}</small></p>{3}", link, ( strDirectory + "/" + finfo.Name ), finfo.Name, Environment.NewLine );
				} 
			}
			
			/*
			if (e.Item.ItemType == ListItemType.Header) 
			{
				HyperLink dhead = (HyperLink)e.Item.FindControl("dhead");
				dhead.NavigateUrl = Page.GetPostBackClientHyperlink(GoDir, pdir);
				dhead.Text = String.Format("<p align=\"center\"><img src=\"{0}\" alt=\"{1}\" /><br />UP</a></p>", Data.ForumRoot + "images/folder.gif", Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name")));
			}
			*/
		}

		protected override void Render( HtmlTextWriter writer )
		{
			/*
			foreach ( DataListItem item in directories.Items )
			{
				HyperLink dname = ( HyperLink ) item.FindControl( "dname" );
				Page.ClientScript.RegisterForEventValidation( dname.UniqueID );				
				Trace.Write( dname.ID );
			}

			LinkButton dimage = ( LinkButton ) this.FindControl( "GoDir" );
			Page.ClientScript.RegisterForEventValidation( dimage.UniqueID );
			
			/*
			foreach ( GridViewRow r in GridView1.Rows )
			{
				if ( r.RowType == DataControlRowType.DataRow )
				{
					Page.ClientScript.RegisterForEventValidation( r.UniqueID + "$ctl00" );
					Page.ClientScript.RegisterForEventValidation( r.UniqueID + "$ctl01" );
				}
			}
			 */

			base.Render( writer );
		}

		public void directories_bind(object sender, DataListItemEventArgs e)
		{
			string strDirectory = Data.ForumRoot + "images/avatars/";
	
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				HyperLink dname = (HyperLink)e.Item.FindControl("dname");				

				Trace.Write(dname.UniqueID);

				dname.NavigateUrl = Page.ClientScript.GetPostBackClientHyperlink(GoDir, filepath + Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name")),true);
				dname.Text = String.Format("<p align=\"center\"><img src=\"{0}\" alt=\"{1}\" /><br />{1}</p>", Data.ForumRoot + "images/folder.gif", Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name")));
			}
		}
		
		private void bind_data()
		{
			string strDirectory = Data.ForumRoot + "images/avatars/" + CurrentDir;

			DirectoryInfo dirinfo = new DirectoryInfo(Server.MapPath(strDirectory));

			if (CurrentDir == "") 
			{
				files.Visible = false;
				directories.Visible = true;
				directories.DataSource = dirinfo.GetDirectories();
				directories.DataBind();
			}
			else
			{
				files.Visible = true;
				directories.Visible = false;
				files.DataSource = dirinfo.GetFiles("*.*");
				files.DataBind();
			}
		}
	}
}
