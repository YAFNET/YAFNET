namespace DotNetNuke.Modules.YAF
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DotNetNuke;
	using DotNetNuke.Common;
	using DotNetNuke.Services.Search;
	using DotNetNuke.Entities.Modules;
	using DotNetNuke.Entities.Modules.Actions;
	using DotNetNuke.Services.Localization;
	using DotNetNuke.Services.Exceptions;
	using yaf;

	/// <summary>
	///		Summary description for DotNetNukeModule.
	/// </summary>
	public class DotNetNukeModule : DotNetNuke.Entities.Modules.PortalModuleBase, IActionable
	{
		private int m_userID; 
		private string m_userName; 
		private string m_email; 
		//private string m_firstName; 
		//private string m_lastName; 
		//private string m_location; 
		//private bool m_isAuthenticated;

		protected yaf.Forum Forum1;

		protected System.Web.UI.WebControls.Panel pnlModuleContent;

		private void DotNetNukeModule_Load(object sender, System.EventArgs e)
		{
			try 
			{
				Forum1.BoardID = int.Parse(Settings["forumboardid"].ToString());

				string cID = Settings["forumcategoryid"].ToString();
				if(cID!=string.Empty)
					Forum1.CategoryID = int.Parse(cID);
			}
			catch(Exception)
			{
				Forum1.BoardID = 1;
			}

			// Put user code to initialize the page here
			if(HttpContext.Current.User.Identity.IsAuthenticated)
			{
				DotNetNuke.Entities.Users.UserInfo userInfo; 
				DotNetNuke.Entities.Portals.PortalSettings _portalSettings = (DotNetNuke.Entities.Portals.PortalSettings)HttpContext.Current.Items["PortalSettings"]; 
				userInfo = DotNetNuke.Entities.Users.UserController.GetUser(_portalSettings.PortalId, this.UserId, false);

				m_userID = userInfo.UserID; 
				m_userName = userInfo.Username; 
				m_email = userInfo.Membership.Email;

				if(m_userID == _portalSettings.AdministratorId)
				{
					/*
					try
					{
						DataRow row = DB.dnn_board_match(m_portalid); 
						int brdid = (int)row["boardID"];
						//There is a match so do nothing and continue
					}
					catch(Exception)
					{
						//Create a new board
						//Find out last boardID created
						DataTable tbl = DB.board_list(null);
						int numboards = tbl.Rows.Count;
						numboards +=1;
						string newBoardName = "New Board " + numboards.ToString();
						DB.board_create(newBoardName,false,m_userName,m_email,"password",m_portalid);
					}
					*/
				}
			}			
		}

		private void InitializeComponent()
		{
		
		}
	
		override protected void OnInit(EventArgs e)
		{
			Load += new EventHandler(DotNetNukeModule_Load);
			base.OnInit(e);
		}
		#region IActionable Members

		public ModuleActionCollection ModuleActions
		{
			get
			{
				ModuleActionCollection actions = new ModuleActionCollection();
				actions.Add(GetNextActionID(), "Edit YAF Settings", ModuleActionType.AddContent, String.Empty, String.Empty, EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
				return actions;
			}
		}

		#endregion
	}
}
