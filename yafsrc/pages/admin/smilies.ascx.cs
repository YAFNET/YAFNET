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
	/// Summary description for smilies.
	/// </summary>
	public class smilies : AdminPage
	{
		protected System.Web.UI.WebControls.Repeater List;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
				BindData();
		}

		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this smiley?')";
		}

		private void BindData() 
		{
			List.DataSource = DB.smiley_list(PageBoardID,null);
			DataBind();
		}

		private void List_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) 
		{
			switch(e.CommandName) 
			{
				case "add":
					Forum.Redirect(Pages.admin_smilies_edit);
					break;
				case "edit":
					Forum.Redirect(Pages.admin_smilies_edit,"s={0}",e.CommandArgument);
					break;
				case "delete":
					DB.smiley_delete(e.CommandArgument);
					BindData();
					break;
				case "import":
					Forum.Redirect(Pages.admin_smilies_import);
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
