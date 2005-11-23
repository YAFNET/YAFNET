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


namespace yaf.pages
{
	/// <summary>
	/// Summary description for avatar.
	/// </summary>
	public class avatar : ForumPage
	{
		protected System.Web.UI.WebControls.Label title;
		protected System.Web.UI.WebControls.DataList files;
		protected System.Web.UI.WebControls.DataList directories;
		protected System.Web.UI.WebControls.HyperLink goup;
		protected controls.PageLinks PageLinks;
		protected controls.Pager pager;
		protected LinkButton GoDir;
	
		public int pagenum = 0;
		public int pagesize = 20;

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

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageUserName,Forum.GetLink(Pages.cp_profile));
				PageLinks.AddLink(GetText("EDITPROFILE"),Forum.GetLink(Pages.cp_editprofile));
				PageLinks.AddLink(GetText("TITLE"),Forum.GetLink(Pages.avatar));

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
			this.Load += new System.EventHandler(this.Page_Load);
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
					fname.Text += "<p align=\"center\"><a href=\"" + Forum.GetLink(Pages.cp_editprofile, "av=" + CurrentDir + "/" + finfo.Name) + "\"><img src=\"" + strDirectory + "/" + finfo.Name + "\" alt=\"" + finfo.Name + "\" class=\"borderless\" /></a><br /><small>";
					fname.Text += finfo.Name;
					fname.Text += "</small></p>" + Environment.NewLine;
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

		public void directories_bind(object sender, DataListItemEventArgs e)
		{
			string strDirectory = Data.ForumRoot + "images/avatars/";
	
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				HyperLink dname = (HyperLink)e.Item.FindControl("dname");
				dname.NavigateUrl = Page.GetPostBackClientHyperlink(GoDir, filepath + Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name")));
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
