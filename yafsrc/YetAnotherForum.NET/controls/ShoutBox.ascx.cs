//*****************************************************************************************************
//  Original code by: DLESKTECH at http://www.dlesktech.com/support.aspx
//  Modifications by: KASL Technologies at www.kasltechnologies.com
//  Mod date:7/21/2009
//  Mods: working smileys, moved smilies to bottom, added clear button for admin, new stored procedure
//  Mods: fixed the time to show the viewers time not the server time
//  Mods: added small chat window popup that runs separately from forum
//  Note: flyout button opens smaller chat window
//  Note: clear button removes message more than 24hrs old from db
//*****************************************************************************************************
using System;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Controls;

namespace YAF.Controls
{
	public partial class ShoutBox : BaseUserControl
	{
		public string CacheKey
		{
			get
			{
				return YafCache.GetBoardCacheKey( Constants.Cache.Shoutbox );
			}
		}

		public ShoutBox()
			: base()
		{

		}

		protected void Page_Load( object sender, EventArgs e )
		{
			YafContext.Current.PageElements.RegisterJsBlock( shoutBoxUpdatePanel, "DisablePageManagerScrollJs", YAF.Utilities.JavaScriptBlocks.DisablePageManagerScrollJs );

			if ( PageContext.User != null )
			{
				//phShoutText.Visible = true;
				shoutBoxPanel.Visible = true;

				if ( PageContext.IsAdmin )
				{
					btnClear.Visible = true;
				}
			}

			if ( !IsPostBack )
			{
				btnFlyOut.Text = PageContext.Localization.GetText( "SHOUTBOX", "FLYOUT" );
				btnClear.Text = PageContext.Localization.GetText( "SHOUTBOX", "CLEAR" );
				btnButton.Text = PageContext.Localization.GetText( "SHOUTBOX", "SUBMIT" );

				FlyOutHolder.Visible = !YafControlSettings.Current.Popup;
				CollapsibleImageShoutBox.Visible = !YafControlSettings.Current.Popup;

				DataBind();
			}
		}

		protected void ShoutBoxRefreshTimer_Tick( object sender, EventArgs e )
		{
			DataBind();
		}
	
		protected void btnButton_Click( object sender, EventArgs e )
		{
			string username = PageContext.PageUserName;

			if ( username != null && messageTextBox.Text != String.Empty )
			{
				DB.shoutbox_savemessage( messageTextBox.Text, username, PageContext.PageUserID, Request.UserHostAddress );
				// clear cache...
				PageContext.Cache.Remove( CacheKey );
			}

			DataBind();
			messageTextBox.Text = String.Empty;
			ScriptManager scriptManager = ScriptManager.GetCurrent( Page );

			if ( scriptManager != null )
			{
				scriptManager.SetFocus( messageTextBox );
			}
		}

		protected void btnClear_Click( object sender, EventArgs e )
		{
			bool bl = DB.shoutbox_clearmessages();
			// cleared... re-load from cache...
			PageContext.Cache.Remove( CacheKey );
			DataBind();
		}

		public override void DataBind()
		{
			BindData();
			base.DataBind();
		}

		private void BindData()
		{
			DataTable shoutBoxMessages = (DataTable)PageContext.Cache[CacheKey];

			if ( shoutBoxMessages == null )
			{
				shoutBoxMessages = DB.shoutbox_getmessages( PageContext.BoardSettings.ShoutboxShowMessageCount );
				MessageFlags flags = new MessageFlags();
				flags.IsBBCode = true;
				flags.IsHtml = false;

				for ( int i = 0; i < shoutBoxMessages.Rows.Count; i++ )
				{
					string formattedMessage = FormatMsg.FormatMessage( shoutBoxMessages.Rows[i]["Message"].ToString(), flags );
					formattedMessage = FormatHyperLink( formattedMessage );
					shoutBoxMessages.Rows[i]["Message"] = formattedMessage;
				}

				// cache for 30 seconds -- could cause problems on web farm configurations.
				PageContext.Cache.Add( CacheKey, shoutBoxMessages, DateTime.Now.AddSeconds( 30 ) );
			}

			shoutBoxRepeater.DataSource = shoutBoxMessages;
			smiliesRepeater.DataSource = DB.smiley_listunique( PageContext.PageBoardID );
		}

		private static string FormatHyperLink( string message )
		{
			if ( message.Contains( "<a" ) )
			{
				for ( int i = 0; i < message.Length; i++ )
				{
					if ( i <= message.Length - 2 )
					{
						if ( message.Substring( i, 2 ) == "<a" )
						{
							message = message.Substring( i, 2 ) + " target=\"_blank\"" + message.Substring( i + 2, message.Length - ( i + 2 ) );
						}
					}
				}
			}
			return message;
		}

		protected static string FormatSmiliesOnClickString( string code, string path )
		{
			code = code.Replace( "'", "\'" );
			code = code.Replace( "\"", "\"\"" );
			code = code.Replace( "\\", "\\\\" );
			string onClickScript = String.Format( "insertsmiley('{0}','{1}');return false;", code, path );
			return onClickScript;
		}
	}
}