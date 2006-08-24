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

namespace yaf.pages.moderate
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class index : ForumPage
	{

		public index() : base("MODERATE_DEFAULT")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsModerator) 
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(GetText("MODERATE_DEFAULT","TITLE"),"");
				BindData();
			}
		}

		private void BindData() 
		{
			List.DataSource = DB.forum_moderatelist();
			DataBind();
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
		}
		#endregion
	}
}
