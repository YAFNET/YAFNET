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

namespace yaf.pages
{
	/// <summary>
	/// Summary description for info.
	/// </summary>
	public class info : ForumPage
	{
		protected Label Info;
		protected HyperLink Continue;

		public info() : base("INFO")
		{
			CheckSuspended = false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if(!IsPostBack) 
			{
				Continue.NavigateUrl = Request.QueryString["url"];
				Continue.Text = GetText("continue");
				if(Request.QueryString["url"]!=null)
					RefreshURL = Request.QueryString["url"];
				else
					Continue.Visible = false;

				switch(int.Parse(Request.QueryString["i"])) 
				{
					case 1:
						Info.Text = GetText("moderated");
						break;
					case 2:
						Info.Text = String.Format(GetText("suspended"),FormatDateTime(SuspendedTo));
						break;
				}
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
		}
		#endregion
	}
}
