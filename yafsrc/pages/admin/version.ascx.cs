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
	/// Summary description for register.
	/// </summary>
	public partial class version : AdminPage
	{
		private		long				m_lastVersion;
		private		DateTime			m_lastVersionDate;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Version Check","");
			}

			try 
			{
				using(RegisterForum.Register reg = new RegisterForum.Register()) 
				{
					m_lastVersion = reg.LatestVersion();
					m_lastVersionDate = reg.LatestVersionDate();
				}
			}
			catch(Exception) 
			{
				m_lastVersion = 0;
			}
			Upgrade.Visible = m_lastVersion > Data.AppVersionCode;
		}

		protected string LastVersion 
		{
			get 
			{
				return Data.AppVersionNameFromCode(m_lastVersion);
			}
		}
		protected string LastVersionDate
		{
			get
			{
				return m_lastVersionDate.ToShortDateString();
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
		}
		#endregion
	}
}
