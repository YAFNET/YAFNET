using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.admin
{
	/// <summary>
	/// Summary description for smilies_import.
	/// </summary>
	public class smilies_import : AdminPage
	{
		protected DropDownList File;
		protected CheckBox DeleteExisting;
		protected Button import, cancel;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
				BindData();
		}

		private void BindData() 
		{
			using(DataTable dt = new DataTable("Files")) 
			{
				dt.Columns.Add("FileID",typeof(long));
				dt.Columns.Add("FileName",typeof(string));
				DataRow dr = dt.NewRow();
				dr["FileID"] = 0;
				dr["FileName"] = "Select File (*.pak)";
				dt.Rows.Add(dr);
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Request.MapPath(String.Format("{0}images/emoticons",BaseDir)));
				System.IO.FileInfo[] files = dir.GetFiles("*.pak");
				long nFileID = 1;
				foreach(System.IO.FileInfo file in files) 
				{
					dr = dt.NewRow();
					dr["FileID"] = nFileID++;
					dr["FileName"] = file.Name;
					dt.Rows.Add(dr);
				}
				
				File.DataSource = dt;
				File.DataValueField = "FileID";
				File.DataTextField = "FileName";
			}
			DataBind();
		}

		private void import_Click(object sender, System.EventArgs e) 
		{
			if(long.Parse(File.SelectedValue)<1) 
			{
				AddLoadMessage("You must select a .pak file to import.");
				return;
			}

			string sFileName = Request.MapPath(String.Format("{0}images/emoticons/{1}",BaseDir,File.SelectedItem.Text));
			string sSplit = System.Text.RegularExpressions.Regex.Escape("=+:");

			using(System.IO.StreamReader file = new System.IO.StreamReader(sFileName)) 
			{
				// Delete existing smilies?
				if(DeleteExisting.Checked) 
					DB.smiley_delete(null);

				do 
				{
					string sLine = file.ReadLine();
					if(sLine==null)
						break;

					string[] split = System.Text.RegularExpressions.Regex.Split(sLine, sSplit, System.Text.RegularExpressions.RegexOptions.None);
					if(split.Length==3) 
						DB.smiley_save(null,split[2],split[0],split[1],0);
				} while(true);
				file.Close();
			}
			Response.Redirect("smilies.aspx");
		}

		private void cancel_Click(object sender, System.EventArgs e) 
		{
			Response.Redirect("smilies.aspx");
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			import.Click += new System.EventHandler(import_Click);
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
