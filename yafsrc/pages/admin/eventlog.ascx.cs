using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.pages.admin
{
	/// <summary>
	/// Summary description for attachments.
	/// </summary>
	public partial class eventlog : AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Event Log","");

				BindData();
			}
		}

		private void BindData() 
		{
			List.DataSource = DB.eventlog_list(PageBoardID);
			DataBind();
		}

		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this event log entry?')";
		}

		private void List_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "delete":
					DB.eventlog_delete(e.CommandArgument);
					BindData();
					break;
				case "show":
					Control ctl = e.Item.FindControl("details");
					LinkButton showbutton = e.Item.FindControl("showbutton") as LinkButton;
					if(ctl.Visible = !ctl.Visible)
						showbutton.Text = "Hide";
					else
						showbutton.Text = "Show";
					break;
			}
		}

		protected string EventImageCode(object o)
		{
			DataRowView row = (DataRowView)o;
			string imageName = "eventError.gif";
			string imageType = "Error";

			switch ((int)row["Type"])
			{
				case 1: 
					imageName = "eventWarning.gif";	
					imageType = "Warning";
					break;
				case 2:
					imageName = "eventInfo.gif";
					imageType = "Information";
					break;
			}		

			return "<img src=\"images/" + imageName + "\" width=\"16\" height=\"16\" alt=\"" + imageType + "\" title=\"" + imageType + "\">";
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			List.ItemCommand += new RepeaterCommandEventHandler(List_ItemCommand);
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
	}
}
