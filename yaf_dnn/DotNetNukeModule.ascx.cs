namespace yaf_dnn
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DotNetNuke;
	using yaf;


	/// <summary>
	///		Summary description for DotNetNukeModule.
	/// </summary>
	public class DotNetNukeModule : PortalModuleControl
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
			}
			catch(Exception)
			{
				Forum1.BoardID = 1;
			}

			// Put user code to initialize the page here
			if(HttpContext.Current.User.Identity.IsAuthenticated)
			{
				DotNetNuke.UserController userController = new DotNetNuke.UserController(); 
				DotNetNuke.UserInfo userInfo; 

				DotNetNuke.PortalSettings _portalSettings = (DotNetNuke.PortalSettings)HttpContext.Current.Items["PortalSettings"]; 

				userInfo = userController.GetUser(_portalSettings.PortalId, int.Parse(HttpContext.Current.User.Identity.Name)); 

				m_userID = userInfo.UserID; 
				m_userName = userInfo.Username; 
				m_email = userInfo.Email; 

				//See if the host is the user
				if (m_userID == _portalSettings.SuperUserId)
				{
				}
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
	}
}
