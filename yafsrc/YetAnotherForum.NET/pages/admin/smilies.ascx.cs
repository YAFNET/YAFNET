using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for smilies.
	/// </summary>
	public partial class smilies : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
				PageLinks.AddLink("Administration",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				PageLinks.AddLink("Smilies","");

				BindData();
			}
		}

		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this smiley?')";
		}

        private void Pager_PageChange(object sender, EventArgs e)
        {
            BindData();
        }

		private void BindData() 
		{
            Pager.PageSize = 15;
            DataView dv = YAF.Classes.Data.DB.smiley_list(PageContext.PageBoardID, null).DefaultView;
            Pager.Count = dv.Count;
            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = dv;
            pds.AllowPaging = true;
            pds.CurrentPageIndex = Pager.CurrentPageIndex;
            pds.PageSize = Pager.PageSize;
            List.DataSource = pds;
            DataBind();
		}

		private void List_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) 
		{
			switch(e.CommandName) 
			{
				case "add":
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_smilies_edit);
					break;
				case "edit":
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_smilies_edit,"s={0}",e.CommandArgument);
					break;
				case "moveup":
					YAF.Classes.Data.DB.smiley_resort(PageContext.PageBoardID, e.CommandArgument, -1);
					BindData();
					break;
				case "movedown":
					YAF.Classes.Data.DB.smiley_resort(PageContext.PageBoardID, e.CommandArgument, 1);
					BindData();
					break;
				case "delete":
					YAF.Classes.Data.DB.smiley_delete(e.CommandArgument);
					BindData();
					break;
				case "import":
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_smilies_import);
					break;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
            this.Pager.PageChange += new EventHandler(Pager_PageChange);
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
		}
		#endregion
	}
}
