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
	/// Summary description for smilies.
	/// </summary>
	public class smilies : BasePage
	{
		protected System.Web.UI.WebControls.Repeater List;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsAdmin) Response.Redirect(BaseDir);
			TopMenu = false;

			if(!IsPostBack) 
				BindData();
		}

		private void BindData() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				List.DataSource = DataManager.GetData(cmd);
			}
			DataBind();
		}

		private void List_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) 
		{
			switch(e.CommandName) 
			{
				case "add":
					Response.Redirect("smilies_edit.aspx");
					break;
				case "edit":
					Response.Redirect(String.Format("smilies_edit.aspx?s={0}",e.CommandArgument));
					break;
				case "delete":
					using(SqlCommand cmd = new SqlCommand("yaf_smiley_delete")) 
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@SmileyID",e.CommandArgument);
						DataManager.ExecuteNonQuery(cmd);
					}
					BindData();
					break;
				case "import":
					Response.Redirect("smilies_import.aspx");
					break;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.List.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.List_ItemCommand);
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
