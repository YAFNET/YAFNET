/* YetAnotherForum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using YAF.Classes.Base;
using YAF.Classes.Utils;
using YAF.Classes;
using YAF.Modules;

namespace YAF
{
	/// <summary>
	/// Summary description for Forum.
	/// </summary>
	[ToolboxData( "<{0}:Forum runat=\"server\"></{0}:Forum>" )]
	public class Forum : System.Web.UI.UserControl
	{
		private YAF.Controls.Header _header;
		private YAF.Controls.Footer _footer;
		private string _origHeaderClientID;
		private string _origFooterClientID;
		private YAF.Classes.Utils.ForumPages _page;
		private ForumPage currentForumPage;
		public event EventHandler<YAF.Classes.Base.ForumPageArgs> PageTitleSet;
		private YAF.Modules.ModuleManager _moduleManager = null;

		public Forum()
		{
			this.Load += new EventHandler( Forum_Load );
			// setup header/footer
			_header = new YAF.Controls.Header();
			_footer = new YAF.Controls.Footer();
			_origHeaderClientID = _header.ClientID;
			_origFooterClientID = _footer.ClientID;
		}

		private void Forum_Load( object sender, EventArgs e )
		{
			string m_baseDir = YafForumInfo.ForumFileRoot;

			if ( Request.QueryString ["g"] != null )
			{
				try
				{
					_page = (ForumPages)Enum.Parse(typeof(ForumPages), Request.QueryString ["g"], true);
				}
				catch (Exception)
				{
					_page = ForumPages.forum;
				}
			}
			else
			{
				_page = ForumPages.forum;
			}

			if ( !ValidPage( _page ) )
			{
				YafBuildLink.Redirect( ForumPages.topics, "f={0}", LockedForum );
			}

			string src = string.Format( "{0}pages/{1}.ascx", m_baseDir, _page );
			if ( src.IndexOf( "/moderate_" ) >= 0 )
				src = src.Replace( "/moderate_", "/moderate/" );
			if ( src.IndexOf( "/admin_" ) >= 0 )
				src = src.Replace( "/admin_", "/admin/" );
			if ( src.IndexOf( "/help_" ) >= 0 )
				src = src.Replace( "/help_", "/help/" );

			try
			{
				// handle script manager first...
				if ( ScriptManager.GetCurrent( Page ) == null )
				{
					// add a script manager since one doesn't exist...
					ScriptManager yafScriptManager = new ScriptManager();
					yafScriptManager.ID = "YafScriptManager";
					yafScriptManager.EnablePartialRendering = true;
					this.Controls.Add( yafScriptManager );
				}

				if (_moduleManager == null)
				{
					_moduleManager = new ModuleManager();
					_moduleManager.CreateModules();
					_moduleManager.InitModulesBeforeForumPage( YafContext.Current, this, _page );
				}

				currentForumPage = (YAF.Classes.Base.ForumPage)LoadControl(src);
				currentForumPage.PageTitleSet += new EventHandler<YAF.Classes.Base.ForumPageArgs>(forumControl_PageTitleSet);

				currentForumPage.ForumFooter = _footer;
				currentForumPage.ForumHeader = _header;
			
				// add the header control before the page rendering...
				if ( YafContext.Current.Settings.LockedForum == 0 && _origHeaderClientID == _header.ClientID )
					this.Controls.AddAt( 0, _header );

				this.Controls.Add(currentForumPage);

				// add the footer control after the page...
				if ( YafContext.Current.Settings.LockedForum == 0 && _origFooterClientID == _footer.ClientID )
					this.Controls.Add( _footer );

				// load plugins/functionality modules
				if (_moduleManager != null)
				{
					_moduleManager.InitModulesAfterForumPage( YafContext.Current, this, currentForumPage, _page );
				}
			}
			catch ( System.IO.FileNotFoundException )
			{
				throw new ApplicationException( "Failed to load " + src + "." );
			}
		}

		protected override void Render( HtmlTextWriter writer )
		{
			// wrap the forum in one main div and then a page div for better CSS selection
			writer.WriteLine();
			writer.Write( String.Format(@"<div class=""yafnet"" id=""{0}"">", this.ClientID) );
			writer.Write( String.Format( @"<div id=""yafpage_{0}"">", _page.ToString() ) );

			// render the forum
			base.Render( writer );

			writer.WriteLine( "</div></div>" );
		}
		
		/// <summary>
		/// Called when the forum control sets it's Page Title
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
    void forumControl_PageTitleSet( object sender, YAF.Classes.Base.ForumPageArgs e )
    {
      if ( PageTitleSet != null ) PageTitleSet( this, e );
    }		

		/// <summary>
		/// The forum header control
		/// </summary>
		public YAF.Controls.Header Header
		{
			set
			{
				_header = value;
			}
			get
			{
				return _header;
			}
		}

		/// <summary>
		/// The forum footer control
		/// </summary>
		public YAF.Controls.Footer Footer
		{
			set
			{
				_footer = value;
			}
			get
			{
				return _footer;
			}
		}

		private bool ValidPage( ForumPages Page )
		{
			if ( LockedForum == 0 )
				return true;

			if ( Page == YAF.Classes.Utils.ForumPages.forum || Page == YAF.Classes.Utils.ForumPages.active || Page == YAF.Classes.Utils.ForumPages.activeusers )
				return false;

			if ( Page == YAF.Classes.Utils.ForumPages.cp_editprofile || Page == YAF.Classes.Utils.ForumPages.cp_pm || Page == YAF.Classes.Utils.ForumPages.cp_message || Page == YAF.Classes.Utils.ForumPages.cp_profile || Page == YAF.Classes.Utils.ForumPages.cp_signature || Page == YAF.Classes.Utils.ForumPages.cp_subscriptions )
				return false;

			if ( Page == YAF.Classes.Utils.ForumPages.pmessage )
				return false;

			return true;
		}

		/// <summary>
		/// UserID for the current User (Read Only)
		/// </summary>
		public int PageUserID
		{
			get
			{
				return YafContext.Current.PageUserID;
			}
		}

		/// <summary>
		/// UserName for the current User (Read Only)
		/// </summary>
		public string PageUserName
		{
			get
			{
				if ( YafContext.Current.User == null ) return "Guest";				
				return YafContext.Current.User.UserName;
			}
		}

		/// <summary>
		/// Get or sets the Board ID for this instance of the forum control, overriding the value defined in app.config.
		/// </summary>
		public int BoardID
		{
			get
			{
				return YafControlSettings.Current.BoardID;
			}
			set
			{
				YafControlSettings.Current.BoardID = value;
			}
		}

		/// <summary>
		/// Gets or sets the CategoryID for this instance of the forum control
		/// </summary>
		public int CategoryID
		{
			get
			{
				return YafControlSettings.Current.CategoryID;
			}
			set
			{
				YafControlSettings.Current.CategoryID = value;
			}
		}

		public int LockedForum
		{
			get
			{
				return YafControlSettings.Current.LockedForum;
			}
			set
			{
				YafControlSettings.Current.LockedForum = value;
			}
		}
	}
}
