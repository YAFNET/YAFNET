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
	public partial class info : ForumPage
	{

		public info()
			: base( "INFO" )
		{
			CheckSuspended = false;
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			// Put user code to initialize the page here
			if ( !IsPostBack )
			{
				Continue.NavigateUrl = Request.QueryString ["url"];
				Continue.Text = GetText( "continue" );
				if ( Request.QueryString ["url"] != null )
					RefreshURL = Request.QueryString ["url"];
				else
					Continue.Visible = false;

				try
				{
					switch ( int.Parse( Request.QueryString ["i"] ) )
					{
						case 1: /// Moderated
							Title.Text = GetText( "title_moderated" );
							Info.Text = GetText( "moderated" );
							break;
						case 2: /// Suspended
							Title.Text = GetText( "title_suspended" );
							Info.Text = String.Format( GetText( "suspended" ), FormatDateTime( SuspendedTo ) );
							break;
						case 3: /// Registration email
							Title.Text = GetText( "title_registration" );
							Info.Text = GetText( "registration" );
							RefreshTime = 10;
							RefreshURL = Forum.GetLink( Pages.login );
							break;
						case 4: /// Access Denied
							Title.Text = GetText( "title_accessdenied" );
							Info.Text = GetText( "accessdenied" );
							RefreshTime = 10;
							RefreshURL = Forum.GetLink( Pages.forum );
							break;
					}
				}
				catch ( Exception )
				{
					Title.Text = GetText( "title_exception" );
					Info.Text = string.Format( "{1} <b>{0}</b>.", PageUserName, GetText( "exception" ) );
					RefreshTime = 2;
					RefreshURL = Forum.GetLink( Pages.forum );
				}
				Continue.NavigateUrl = RefreshURL;
				Continue.Visible = RefreshURL != null;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit( e );
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
