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

namespace yaf.admin
{
	/// <summary>
	/// Summary description for register.
	/// </summary>
	public class version : AdminPage
	{
		protected	HtmlGenericControl	Upgrade;
		private		long				m_lastVersion;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try 
			{
				using(RegisterForum.Register reg = new RegisterForum.Register()) 
				{
					m_lastVersion = reg.LatestVersion();
				}
			}
			catch(Exception) 
			{
				m_lastVersion = 0;
			}
			Upgrade.Visible = m_lastVersion > AppVersionCode;
		}

		protected string LastVersion 
		{
			get 
			{
				using(RegisterForum.Register reg = new RegisterForum.Register()) 
				{
					return AppVersionNameFromCode(m_lastVersion);
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
