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

namespace YAF.Pages.moderate
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class unapprovedposts : YAF.Classes.Base.ForumPage
	{

		public unapprovedposts()
			: base( "MODERATE_FORUM" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !PageContext.IsModerator || !PageContext.ForumModeratorAccess )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( GetText( "MODERATE_DEFAULT", "TITLE" ), YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.moderate_index ) );
				PageLinks.AddLink( PageContext.PageForumName );
				BindData();
			}
		}

		protected void Delete_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton )sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", GetText( "MODERATE_FORUM", "ASK_DELETE" ) );
		}

		private void BindData()
		{
			List.DataSource = YAF.Classes.Data.DB.message_unapproved( PageContext.PageForumID );
			DataBind();
		}

		protected string FormatMessage( DataRowView row )
		{
			MessageFlags messageFlags = new MessageFlags( row ["Flags"] );
			string msg;

			if ( messageFlags.NotFormatted )
			{
				msg = HtmlEncode(row ["Message"].ToString());
			}
			else
			{
				msg = YAF.Classes.UI.FormatMsg.FormatMessage( row ["Message"].ToString(), messageFlags, Convert.ToBoolean( row ["IsModeratorChanged"] ) );
			}

			return msg;
		}

		private void List_ItemCommand( object sender, RepeaterCommandEventArgs e )
		{
			switch ( e.CommandName.ToLower() )
			{
				case "approve":
					YAF.Classes.Data.DB.message_approve( e.CommandArgument );
					BindData();
					PageContext.AddLoadMessage( GetText( "MODERATE_FORUM", "APPROVED" ) );
					General.CreateWatchEmail( e.CommandArgument );
					break;
				case "delete":
					YAF.Classes.Data.DB.message_delete( e.CommandArgument, true, "", 1, true );
					BindData();
					PageContext.AddLoadMessage( GetText( "MODERATE_FORUM", "DELETED" ) );
					break;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			List.ItemCommand += new RepeaterCommandEventHandler( List_ItemCommand );
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
