using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.admin
{
	/// <summary>
	/// Summary description for smilies_edit.
	/// </summary>
	public class smilies_edit : BasePage
	{
		protected DropDownList Icon;
		protected TextBox Code, Emotion;
		protected Button save, cancel;
		protected HtmlImage Preview;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsAdmin) Response.Redirect(BaseDir);
			TopMenu = false;

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
				dr["FileName"] = "Select Icon";
				dt.Rows.Add(dr);
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Request.MapPath(String.Format("{0}images/emoticons",BaseDir)));
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
					dt.Rows.Add(dr);
				}
				
				Icon.DataSource = dt;
				Icon.DataValueField = "FileName";
				Icon.DataTextField = "FileName";
			}
			DataBind();

			if(Request["s"]!=null) 
			{
				using(SqlCommand cmd = new SqlCommand("yaf_smiley_list")) 
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@SmileyID",Request["s"]);
					using(DataTable dt = DataManager.GetData(cmd)) 
					{
						if(dt.Rows.Count>0) 
						{
							Code.Text = dt.Rows[0]["Code"].ToString();
							Emotion.Text = dt.Rows[0]["Emoticon"].ToString();
							Icon.Items.FindByText(dt.Rows[0]["Icon"].ToString()).Selected = true;
							Preview.Src = String.Format("../images/emoticons/{0}",dt.Rows[0]["Icon"]);
						}
					}
				}
			}
			Icon.Attributes["onchange"] = "getElementById('Preview').src='../images/emoticons/' + this.value";
		}

		private void save_Click(object sender, System.EventArgs e) 
		{
			string	sCode		= Code.Text.Trim();
			string	sEmotion	= Emotion.Text.Trim();
			string	sIcon		= Icon.SelectedItem.Text.Trim();

			if(sCode.Length==0) 
			{
				AddLoadMessage("Please enter the code to use for this emotion.");
				return;
			}
			if(sEmotion.Length==0) 
			{
				AddLoadMessage("Please enter the emotion for this icon.");
				return;
			}
			if(Icon.SelectedIndex<1) 
			{
				AddLoadMessage("Please select an icon to use for this emotion.");
				return;
			}
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				if(Request["s"]!=null)
					cmd.Parameters.Add("@SmileyID",Request["s"]);
				cmd.Parameters.Add("@Code",sCode);
				cmd.Parameters.Add("@Icon",sIcon);
				cmd.Parameters.Add("@Emoticon",sEmotion);
				cmd.Parameters.Add("@Replace",0);
				DataManager.ExecuteNonQuery(cmd);
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
