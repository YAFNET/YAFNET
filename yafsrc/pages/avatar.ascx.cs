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
		protected System.Web.UI.WebControls.DropDownList listForum;
		protected System.Web.UI.WebControls.DropDownList listResInPage;
		protected System.Web.UI.WebControls.DropDownList listSearchWhere;
		protected System.Web.UI.WebControls.DropDownList listSearchWath;
		protected System.Web.UI.WebControls.TextBox txtSearchString;
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.Repeater SearchRes;
		protected System.Web.UI.WebControls.Literal AvatarResults;
		protected System.Web.UI.WebControls.Literal DirResults;
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
				Get_Avatar();
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
			Get_Avatar();
		}

		private void GoDir_Click(object sender,EventArgs e)
		{
			CurrentDir = Request.Form["__EVENTARGUMENT"];
			Get_Avatar();
		}

		private void Get_Avatar()
		{
			string avatarpath = Data.ForumRoot + "images/avatars/";

			//string curdir = CurrentDir;
			string pdir = "";
			int ct = 1;

			FileInfo file;
			FileInfo dir;

			string fname = "";

			string[] dirs;

			filepath = CurrentDir;
			string[] pardir = CurrentDir.Split('/');
			for(int i=0;i<pardir.Length-1;i++)
				pdir += pardir[i] + "/";
			if(pdir.Length>0) pdir = pdir.Substring(0,pdir.Length-1);
			
			// Count Images
			int imgct = 0;
			string[] files;

			files = Directory.GetFiles(Server.MapPath(avatarpath + filepath), "*.jpg");
			imgct = files.Length;
			files = Directory.GetFiles(Server.MapPath(avatarpath + filepath), "*.gif");
			imgct += files.Length;
			files = Directory.GetFiles(Server.MapPath(avatarpath + filepath), "*.jpeg");
			imgct += files.Length;
			files = Directory.GetFiles(Server.MapPath(avatarpath + filepath), "*.png");
			imgct += files.Length;

			string[] tmpfiles = new string[imgct];

			int count = 0;
			foreach(string x in Directory.GetFiles(Server.MapPath(avatarpath + filepath)))
			{
				if (x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".gif") || x.EndsWith(".png"))
				{
					if(x.ToLower()!="cvs")
						tmpfiles[count] = x;
				}

				count++;
			}

			files = tmpfiles;

			dirs = Directory.GetDirectories(Server.MapPath(avatarpath + filepath));

			if(filepath.Length>0 && !filepath.EndsWith("/")) filepath += "/";

			DirResults.Text = string.Format("<tr class='postheader'><td align='center'><a href=\"{0}\"><img src=\""+Data.ForumRoot+"images/folder.gif\" alt=\"Up\" class=\"borderless\" /><br />Up</a></td><td colspan=\"4\"><b>" + CurrentDir + "</b></td></tr>",Page.GetPostBackClientHyperlink(GoDir,pdir));

			ArrayList diral = new ArrayList();
	
			int ttldir = 0;

			for (int d = 0; d < dirs.Length; d++)
			{
				dir = new FileInfo(dirs[d]);
				diral.Add(dir.Name);
				ttldir++;
			}

			diral.Sort();

			for (int nodir = 0; nodir < ttldir; nodir++)
			{
				if (nodir == 0)
				{
					DirResults.Text += "<tr class='postheader'>" + Environment.NewLine;
				}

				DirResults.Text += string.Format("<td width='20%' align=\"center\"><a href=\"{0}\"><img src=\"{2}\" alt=\"{1}\" class=\"borderless\" /><br />{1}</a></td>",
					Page.GetPostBackClientHyperlink(GoDir,filepath + diral[nodir]),
					diral[nodir],
					Data.ForumRoot + "images/folder.gif"
					);
				if (ct >= 5 && nodir != (ttldir - 1))
				{
					DirResults.Text += "</tr><tr>";
					ct = 1;
				} 
				else 
				{
					ct++;
				}

				if (nodir == (ttldir - 1))
				{
					if(ct<=5) DirResults.Text += string.Format("<td colspan='{0}'>&nbsp;</td>",6-ct);
					DirResults.Text += "</tr>" + Environment.NewLine;
				}
			}

			Array.Sort(files);

			int pgnum = pager.CurrentPageIndex;
			pager.Count = count;

			int intpg = pgnum * pagesize;
			int y = 1;

			AvatarResults.Text = "";

			filepath = filepath.Replace("images/avatars/", "");

			for (int x = intpg; x < intpg + pagesize; x++)
			{
				if (x < files.Length)
				{
					file = new FileInfo(files[x]);
					fname = file.Name.ToLower();

					if (fname.EndsWith(".jpg") || fname.EndsWith(".gif") || fname.EndsWith(".jpeg") || fname.EndsWith(".png"))
					{
						if (x == intpg)
						{
							AvatarResults.Text += "<tr class='post'>" + Environment.NewLine;
						}

						AvatarResults.Text += "<td width='20%' align=\"center\">";

						AvatarResults.Text += "<a href=\"" + Forum.GetLink(Pages.cp_editprofile,"av=" + filepath + file.Name) + "\"><img src=\"" + avatarpath + filepath + file.Name + "\" alt=\"" + file.Name + "\" class=\"borderless\" /></a><br /><small>";
						AvatarResults.Text += file.Name;
						AvatarResults.Text += "</small></td>" + Environment.NewLine;

						if (y == 5 && x != ((intpg + pagesize) - 1))
						{
							AvatarResults.Text += "</tr><tr class='post'>";
							y = 1;
						} 
						else 
						{
							y++;
						}

						if (x == ((intpg + pagesize) - 1) || x == (files.Length - 1))
						{
							if(y<=5) AvatarResults.Text += string.Format("<td colspan='{0}'>&nbsp;</td>",6-y);
							AvatarResults.Text += "</tr>";
						}
					}
				}
			}
		}
	}
}
