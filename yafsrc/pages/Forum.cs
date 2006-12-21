using System;
using System.Web;
using System.Web.UI;

namespace YAF
{
	public enum ForumPages
	{
		forum,
		topics,
		posts,
		profile,
		activeusers,
		moderate,
		postmessage,
		mod_forumuser,
		attachments,
		pmessage,
		movetopic,
		emailtopic,
		printtopic,
		members,
		cp_inbox,
		cp_profile,
		cp_editprofile,
		cp_editavatar,
		cp_signature,
		cp_subscriptions,
		cp_message,
        cp_changepassword,
		login,
		approve,
		info,
		rules,
		register,
		search,
		active,
		logout,
		moderate_index,
		moderate_forum,
		error,
		avatar,
		admin_admin,
		admin_hostsettings,
		admin_boards,
		admin_boardsettings,
		admin_forums,
		admin_bannedip,
		admin_smilies,
		admin_accessmasks,
		admin_groups,
		admin_users,
		admin_ranks,
		admin_mail,
		admin_prune,
		admin_pm,
		admin_attachments,
		admin_eventlog,
		admin_nntpservers,
		admin_nntpforums,
		admin_nntpretrieve,
		admin_version,
		admin_bannedip_edit,
		admin_editaccessmask,
		admin_editboard,
		admin_editcategory,
		admin_editforum,
		admin_editgroup,
		admin_editnntpforum,
		admin_editnntpserver,
		admin_editrank,
		admin_edituser,
		// Added BAI 07.01.2004		 
		admin_reguser,
		// Added BAI 07.01.2004
		admin_smilies_edit,
		admin_smilies_import,
		// Added Rico83
		admin_replacewords,
		admin_replacewords_edit,
		im_yim,
		im_aim,
		im_icq,
		im_email,
		rsstopic,
		help_index,
		help_recover,
		lastposts,
        recoverpassword,
        deletemessage,
        movemessage
	}

	/// <summary>
	/// Summary description for Forum.
	/// </summary>
	[ToolboxData( "<{0}:Forum runat=\"server\"></{0}:Forum>" )]
	public class Forum : System.Web.UI.UserControl
	{
		public Forum()
		{
			this.Load += new EventHandler( Forum_Load );
			try
			{
				m_boardID = int.Parse( YAF.Classes.Utils.Config.BoardID );
			}
			catch
			{
				m_boardID = 1;
			}
		}

		private void Forum_Load( object sender, EventArgs e )
		{
			ForumPages Page;
			string m_baseDir = Data.ForumRoot;

			try
			{
				Page = ( ForumPages ) System.Enum.Parse( typeof( ForumPages ), Request.QueryString ["g"], true );
			}
			catch ( Exception )
			{
				Page = ForumPages.forum;
			}

			if ( !ValidPage( Page ) )
			{
				Forum.Redirect( ForumPages.topics, "f={0}", LockedForum );
			}

			string src = string.Format( "{0}pages/{1}.ascx", m_baseDir, Page );
			if ( src.IndexOf( "/moderate_" ) >= 0 )
				src = src.Replace( "/moderate_", "/moderate/" );
			if ( src.IndexOf( "/admin_" ) >= 0 )
				src = src.Replace( "/admin_", "/admin/" );
			if ( src.IndexOf( "/help_" ) >= 0 )
				src = src.Replace( "/help_", "/help/" );

			try
			{
				Pages.ForumPage ctl = ( Pages.ForumPage ) LoadControl( src );
				ctl.ForumControl = this;

				this.Controls.Add( ctl );
			}
			catch ( System.IO.FileNotFoundException )
			{
				throw new ApplicationException( "Failed to load " + src + "." );
			}
		}

		private YAF.Controls.Header m_header = null;
		private YAF.Controls.Footer m_footer = null;

		public YAF.Controls.Header Header
		{
			set
			{
				m_header = value;
			}
			get
			{
				return m_header;
			}
		}

		public YAF.Controls.Footer Footer
		{
			set
			{
				m_footer = value;
			}
			get
			{
				return m_footer;
			}
		}

		static public string GetLink( ForumPages Page )
		{
			return YAF.Classes.Utils.Config.UrlBuilder.BuildUrl( string.Format( "g={0}", Page ) );
		}

		static public string GetLink( ForumPages Page, string format, params object [] args )
		{
			return YAF.Classes.Utils.Config.UrlBuilder.BuildUrl( string.Format( "g={0}&{1}", Page, string.Format( format, args ) ) );
		}

		static public void Redirect( ForumPages Page )
		{
			System.Web.HttpContext.Current.Response.Redirect( GetLink( Page ) );
		}

		static public void Redirect( ForumPages Page, string format, params object [] args )
		{
			System.Web.HttpContext.Current.Response.Redirect( GetLink( Page, format, args ) );
		}

		private int m_boardID;

		public int BoardID
		{
			get
			{
				return m_boardID;
			}
			set
			{
				m_boardID = value;
			}
		}

		public int PageUserID
		{
			get
			{
				foreach ( Control c in Controls )
				{
					if ( c is YAF.Pages.ForumPage )
						return ( c as YAF.Pages.ForumPage ).PageUserID;
				}
				return 0;
			}
		}

		private int m_categoryID = Convert.ToInt32( YAF.Classes.Utils.Config.CategoryID );

		public int CategoryID
		{
			get
			{
				return m_categoryID;
			}
			set
			{
				m_categoryID = value;
			}
		}

		private int mLockedForum = 0;

		public int LockedForum
		{
			set
			{
				mLockedForum = value;
			}
			get
			{
				return mLockedForum;
			}
		}

		private bool ValidPage( ForumPages Page )
		{
			if ( LockedForum == 0 )
				return true;

			if ( Page == ForumPages.forum || Page == ForumPages.active || Page == ForumPages.activeusers )
				return false;

			if ( Page == ForumPages.cp_editprofile || Page == ForumPages.cp_inbox || Page == ForumPages.cp_message || Page == ForumPages.cp_profile || Page == ForumPages.cp_signature || Page == ForumPages.cp_subscriptions )
				return false;

			if ( Page == ForumPages.pmessage )
				return false;

			return true;
		}
	}
}
