using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	///		Summary description for DisplayPost.
	/// </summary>
	public partial class DisplayPost : YAF.Classes.Base.BaseUserControl
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			PopMenu1.Visible = PageContext.IsAdmin;
			if ( PopMenu1.Visible )
			{
				PopMenu1.ItemClick += new PopEventHandler( PopMenu1_ItemClick );
				PopMenu1.AddItem( "userprofile", "User Profile" );
				PopMenu1.AddItem( "edituser", "Edit User (Admin)" );
				PopMenu1.Attach( UserName );
			}

			Page.ClientScript.RegisterClientScriptBlock( this.GetType(), "yafjs", string.Format( "<script language='javascript' src='{0}'></script>", PageContext.Theme.GetURLToResource( "yaf.js" ) ) );
			NameCell.ColSpan = int.Parse( GetIndentSpan() );
		}

		private void DisplayPost_PreRender( object sender, EventArgs e )
		{
			// TODO localize tooltips
			Attach.Visible = !PostDeleted && CanAttach;
			Attach.Text = PageContext.Theme.GetItem( "BUTTONS", "ATTACHMENTS" );
			Attach.ToolTip = "Attachments";
			Attach.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.attachments, "m={0}", DataRow ["MessageID"] );
			Edit.Visible = !PostDeleted && CanEditPost;
			Edit.Text = PageContext.Theme.GetItem( "BUTTONS", "EDITPOST" );
			Edit.ToolTip = "Edit this post";
			Edit.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.postmessage, "m={0}", DataRow ["MessageID"] );
			MovePost.Visible = PageContext.ForumModeratorAccess;
			MovePost.Text = PageContext.Theme.GetItem( "BUTTONS", "MOVEPOST" );
			MovePost.ToolTip = "Move this post";
			MovePost.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.movemessage, "m={0}", DataRow ["MessageID"] );
			Delete.Visible = !PostDeleted && CanDeletePost;
			Delete.Text = PageContext.Theme.GetItem( "BUTTONS", "DELETEPOST" );
			Delete.ToolTip = "Delete this post";
			Delete.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.deletemessage, "m={0}&action=delete", DataRow ["MessageID"] );
			UnDelete.Visible = CanUnDeletePost;
			UnDelete.Text = PageContext.Theme.GetItem( "BUTTONS", "UNDELETEPOST" );
			UnDelete.ToolTip = "UnDelete this post";
			UnDelete.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.deletemessage, "m={0}&action=undelete", DataRow ["MessageID"] );
			Quote.Visible = !PostDeleted && CanReply;
			Quote.Text = PageContext.Theme.GetItem( "BUTTONS", "QUOTEPOST" );
			Quote.ToolTip = "Reply with quote";
			Quote.NavigateUrl = yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.postmessage, "t={0}&f={1}&q={2}", PageContext.PageTopicID, PageContext.PageForumID, DataRow ["MessageID"] );

			// private messages
			Pm.Visible = !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowPrivateMessages && !IsSponserMessage;
			Pm.Text = PageContext.Theme.GetItem( "BUTTONS", "PM" );
			Pm.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.pmessage, "u={0}", DataRow ["UserID"] );
			// emailing
			Email.Visible = !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowEmailSending && !IsSponserMessage;
			Email.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.im_email, "u={0}", DataRow ["UserID"] );
			Email.Text = PageContext.Theme.GetItem( "BUTTONS", "EMAIL" );
			// home page
			Home.Visible = !PostDeleted && DataRow ["HomePage"] != DBNull.Value;
			Home.NavigateUrl = DataRow ["HomePage"].ToString();
			Home.Text = PageContext.Theme.GetItem( "BUTTONS", "WWW" );
			// blog page
			Blog.Visible = !PostDeleted && DataRow ["Weblog"] != DBNull.Value;
			Blog.NavigateUrl = DataRow ["Weblog"].ToString();
			Blog.Text = PageContext.Theme.GetItem( "BUTTONS", "WEBLOG" );
			// MSN
			Msn.Visible = !PostDeleted && PageContext.User != null && DataRow ["MSN"] != DBNull.Value;
			Msn.Text = PageContext.Theme.GetItem( "BUTTONS", "MSN" );
			Msn.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.im_email, "u={0}", DataRow ["UserID"] );
			// Yahoo IM
			Yim.Visible = !PostDeleted && PageContext.User != null && DataRow ["YIM"] != DBNull.Value;
			Yim.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.im_yim, "u={0}", DataRow ["UserID"] );
			Yim.Text = PageContext.Theme.GetItem( "BUTTONS", "YAHOO" );
			// AOL IM
			Aim.Visible = !PostDeleted && PageContext.User != null && DataRow ["AIM"] != DBNull.Value;
			Aim.Text = PageContext.Theme.GetItem( "BUTTONS", "AIM" );
			Aim.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.im_aim, "u={0}", DataRow ["UserID"] );
			// ICQ
			Icq.Visible = !PostDeleted && PageContext.User != null && DataRow ["ICQ"] != DBNull.Value;
			Icq.Text = PageContext.Theme.GetItem( "BUTTONS", "ICQ" );
			Icq.NavigateUrl = yaf_BuildLink.GetLink( ForumPages.im_icq, "u={0}", DataRow ["UserID"] );

			if ( !PostDeleted )
			{
				AdminInfo.InnerHtml = "<span class='smallfont'>";
				if ( Convert.ToDateTime( DataRow ["Edited"] ) > Convert.ToDateTime( DataRow ["Posted"] ).AddSeconds( PageContext.BoardSettings.EditTimeOut ) )
				{
					// message has been edited
					// show, why the post was edited or deleted?
					string whoChanged = ( Convert.ToBoolean( DataRow ["IsModeratorChanged"] ) ) ? "by moderator" : "by user";
					AdminInfo.InnerHtml += String.Format( "|<b> <font color=red>{0} {1}:</font></b> {2}", "Edited", whoChanged, yaf_DateTime.FormatDateTimeShort( Convert.ToDateTime( DataRow ["Edited"] ) ) );
					if ( Server.HtmlDecode( Convert.ToString( DataRow ["EditReason"] ) ) != "" )
					{
						// reason was specified
						AdminInfo.InnerHtml += String.Format( " |<b> {0}:</b> {1}", "Reason", FormatMsg.RepairHtml( ( string ) DataRow ["EditReason"], true ) );
					}
					else
					{
						//reason was not specified
						AdminInfo.InnerHtml += String.Format( " |<b> {0}:</b> {1}", "Reason", "Not specified" );
					}
				}
			}
			else
			{
				AdminInfo.InnerHtml = "<span class='smallfont'>";
				if ( Server.HtmlDecode( Convert.ToString( DataRow ["DeleteReason"] ) ) != String.Empty )
				{
					// reason was specified
					AdminInfo.InnerHtml += String.Format( " |<b> {0}:</b> {1}", "Reason", FormatMsg.RepairHtml( ( string ) DataRow ["DeleteReason"], true ) );
				}
				else
				{
					//reason was not specified
					AdminInfo.InnerHtml += String.Format( " |<b> {0}:</b> {1}", "Reason", "Not specified" );
				}
			}

			// display admin only info
			if ( PageContext.IsAdmin )
			{
				AdminInfo.InnerHtml += String.Format( " |<b> IP:</b> {0}", DataRow ["IP"].ToString() );
			}
			AdminInfo.InnerHtml += "</span>";
		}


		override protected void OnInit( EventArgs e )
		{
			this.PreRender += new EventHandler( DisplayPost_PreRender );
			base.OnInit( e );
		}

		private DataRowView m_row = null;
		public DataRowView DataRow
		{
			get
			{
				return m_row;
			}
			set
			{
				m_row = value;
			}
		}

		private MessageFlags PostMessageFlags
		{
			get
			{
				return new MessageFlags( Convert.ToInt32( DataRow ["Flags"] ) );
			}
		}

		protected bool IsSponserMessage
		{
			get
			{
				return ( DataRow ["IP"].ToString() == "none" );
			}
		}

		protected bool CanEditPost
		{
			get
			{
				return !PostLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ( ( int ) DataRow ["UserID"] == PageContext.PageUserID || PageContext.ForumModeratorAccess ) && PageContext.ForumEditAccess;
			}
		}

		private bool PostLocked
		{
			get
			{
				if ( PostMessageFlags.IsLocked ) return true;

				if ( !PageContext.IsAdmin && PageContext.BoardSettings.LockPosts > 0 )
				{
					DateTime edited = ( DateTime ) DataRow ["Edited"];
					if ( edited.AddDays( PageContext.BoardSettings.LockPosts ) < DateTime.Now )
						return true;
				}
				return false;
			}
		}

		private bool PostDeleted
		{
			get
			{

				int deleted = ( int ) DataRow ["Flags"] & 8;
				if ( deleted == 8 )
					return true;
				return false;
			}
		}

		protected bool CanAttach
		{
			get
			{
				return !PostLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ( ( int ) DataRow ["UserID"] == PageContext.PageUserID || PageContext.ForumModeratorAccess ) && PageContext.ForumUploadAccess;
			}
		}

		protected bool CanDeletePost
		{
			get
			{
				return !PostLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ( ( int ) DataRow ["UserID"] == PageContext.PageUserID || PageContext.ForumModeratorAccess ) && PageContext.ForumDeleteAccess;
			}
		}

		public bool CanUnDeletePost
		{
			get
			{
				return PostDeleted && CanDeletePost;
			}
		}

		protected bool CanReply
		{
			get
			{
				return !PostMessageFlags.IsLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && PageContext.ForumReplyAccess;
			}
		}

		private bool m_isAlt = false;
		public bool IsAlt
		{
			get { return this.m_isAlt; }
			set { this.m_isAlt = value; }
		}

		private bool m_isThreaded = false;
		public bool IsThreaded
		{
			get
			{
				return m_isThreaded;
			}
			set
			{
				m_isThreaded = value;
			}
		}

		protected string GetIndentCell()
		{
			if ( !IsThreaded )
				return "";

			int iIndent = ( int ) DataRow ["Indent"];
			if ( iIndent > 0 )
				return string.Format( "<td rowspan='3' width='1%'><img src='{1}images/spacer.gif' width='{0}' height='2'/></td>", iIndent * 32, yaf_ForumInfo.ForumRoot );
			else
				return "";
		}

		protected string GetIndentSpan()
		{
			if ( !IsThreaded || ( int ) DataRow ["Indent"] == 0 )
				return "2";
			else
				return "1";
		}

		protected string GetPostClass()
		{
			if ( this.IsAlt )
				return "post_alt";
			else
				return "post";
		}

		// Prevents a high user box when displaying a deleted post.
		protected string GetUserBoxHeight()
		{
			if ( PostDeleted )
				return "0";
			return "100";
		}

		// Ederon : 7/14/2007 - implemented user box template for formatting
		protected string FormatUserBox()
		{
			if ( IsSponserMessage ) return "";

			System.Text.StringBuilder userboxOutput = new System.Text.StringBuilder( 1000 );

			// load output buffer with user box template
			userboxOutput.Append(PageContext.BoardSettings.UserBox);


			// Avatar
			if (!PostDeleted &&
				(PageContext.BoardSettings.AvatarUpload && DataRow["HasAvatarImage"] != null && long.Parse(DataRow["HasAvatarImage"].ToString()) > 0))
			{
				userboxOutput.Replace(
					Constants.UserBox.Avatar,
					String.Format(
						PageContext.BoardSettings.UserBoxAvatar,
						String.Format(
							"<img class=\"avatarimage\" src=\"{1}resource.ashx?u={0}\" />", 
							DataRow["UserID"], 
							yaf_ForumInfo.ForumRoot
							)
						)
					);
			}
			else if (!PostDeleted && 
				DataRow["Avatar"].ToString().Length > 0) // Took out PageContext.BoardSettings.AvatarRemote
			{
				userboxOutput.Replace(
					Constants.UserBox.Avatar,
					String.Format(
						PageContext.BoardSettings.UserBoxAvatar,
						String.Format(
							"<img class=\"avatarimage\" src='{3}resource.ashx?url={0}&width={1}&height={2}'><br clear=\"all\" />",
							Server.UrlEncode(DataRow["Avatar"].ToString()),
							PageContext.BoardSettings.AvatarWidth,
							PageContext.BoardSettings.AvatarHeight,
							yaf_ForumInfo.ForumRoot
							)
						)
					);
			}
			else
			{

				userboxOutput.Replace(Constants.UserBox.Avatar, "");
			}


			// Ederon : 7/14/2007 - prepared for implementation of user badges
			// User Badges
			/*
			if (PageContext.BoardSettings.ShowBadges)
			{
			}
			else
			{
			}
			*/


			// Rank Image
			if (DataRow["RankImage"].ToString().Length > 0)
			{
				userboxOutput.Replace(
					Constants.UserBox.RankImage,
					String.Format(
						PageContext.BoardSettings.UserBoxRankImage,
						String.Format(
							"<img class=\"rankimage\" align=left src=\"{0}images/ranks/{1}\" />",
							yaf_ForumInfo.ForumRoot, 
							DataRow["RankImage"]
							)
						)
					);
			}
			else
			{
				userboxOutput.Replace(Constants.UserBox.RankImage, "");
			}


			// Rank
			userboxOutput.Replace(
				Constants.UserBox.Rank,
				String.Format(
					PageContext.BoardSettings.UserBoxRank,
					PageContext.Localization.GetText("rank"),
					DataRow["RankName"]
					)
				);


			// Groups
			if (PageContext.BoardSettings.ShowGroups)
			{
				System.Text.StringBuilder groupsText = new System.Text.StringBuilder(150);

				using (DataTable dt = YAF.Classes.Data.DB.usergroup_list(DataRow["UserID"]))
				{
					userboxOutput.AppendFormat("{0}: ", PageContext.Localization.GetText("groups"));

					bool bFirst = true;

					foreach (DataRow grp in dt.Rows)
					{
						if (bFirst)
						{
							groupsText.AppendLine(grp["Name"].ToString());
							bFirst = false;
						}
						else
						{
							groupsText.AppendFormat(", {0}", grp["Name"]);
						}
					}
				}

				userboxOutput.Replace(
					Constants.UserBox.Groups,
					String.Format(
						PageContext.BoardSettings.UserBoxGroups,
						PageContext.Localization.GetText("groups"),
						groupsText.ToString()
						)
					);
			}
			else
			{
				userboxOutput.Replace(Constants.UserBox.Groups, "");
			}


			if (!PostDeleted)
			{
				// Ederon : 02/24/2007
				// Joined Date
				if (PageContext.BoardSettings.DisplayJoinDate)
				{
					userboxOutput.Replace(
						Constants.UserBox.JoinDate,
						String.Format(
							PageContext.BoardSettings.UserBoxJoinDate,
							PageContext.Localization.GetText("joined"),
							yaf_DateTime.FormatDateShort((DateTime)DataRow["Joined"])
							)
						);
				}
				else
				{
					userboxOutput.Replace(Constants.UserBox.JoinDate, "");
				}


				// Posts
				userboxOutput.Replace(
					Constants.UserBox.Posts,
					String.Format(
						PageContext.BoardSettings.UserBoxPosts,
						PageContext.Localization.GetText("posts"),
						DataRow["Posts"]
						)
					);


				// Points
				if (PageContext.BoardSettings.DisplayPoints)
				{
					userboxOutput.Replace(
						Constants.UserBox.Points,
						String.Format(
							PageContext.BoardSettings.UserBoxPoints,
							PageContext.Localization.GetText("points"),
							DataRow["Points"]
							)
						);
				}
				else
				{
					userboxOutput.Replace(Constants.UserBox.Points, "");
				}

				// Location
				if (DataRow["Location"].ToString().Length > 0)
				{
					userboxOutput.Replace(
						Constants.UserBox.Location,
						String.Format(
							PageContext.BoardSettings.UserBoxLocation,
							PageContext.Localization.GetText("location"),
							FormatMsg.RepairHtml(DataRow["Location"].ToString(), false)
							)
						);
				}
				else
				{
					userboxOutput.Replace(Constants.UserBox.Location, "");
				}
			}
			else
			{
				userboxOutput.Replace(Constants.UserBox.Groups, "");
				userboxOutput.Replace(Constants.UserBox.Posts, "");
				userboxOutput.Replace(Constants.UserBox.Points, "");
				userboxOutput.Replace(Constants.UserBox.Location, "");
			}

			return userboxOutput.ToString();
		}

		protected string FormatBody()
		{
			System.Text.StringBuilder messageOutput = new System.Text.StringBuilder( 2000 );

			if ( PostMessageFlags.NotFormatted )
			{
				messageOutput.Append( DataRow ["Message"].ToString() );
			}
			else
			{
				messageOutput.Append( FormatMsg.FormatMessage( DataRow ["Message"].ToString(), PostMessageFlags, Convert.ToBoolean( DataRow ["IsModeratorChanged"] ) ) );
			}

			if ( !PostDeleted )
			{

				AddAttachedFiles( ref messageOutput );

				if ( PageContext.BoardSettings.AllowSignatures && DataRow ["Signature"] != DBNull.Value && DataRow ["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" && DataRow ["Signature"].ToString().Trim().Length > 0 )
				{
					// don't allow any HTML on signatures
					MessageFlags tFlags = new MessageFlags();
					tFlags.IsHTML = false;

					messageOutput.Append( "<br/><hr noshade />" + FormatMsg.FormatMessage( DataRow ["Signature"].ToString(), tFlags ) );
				}
			}
			return messageOutput.ToString();
		}

		private void AddAttachedFiles( ref System.Text.StringBuilder messageOutput )
		{
			// define valid image extensions
			string [] aImageExtensions = { "jpg", "gif", "png", "bmp" };

			if ( long.Parse( DataRow ["HasAttachments"].ToString() ) > 0 )
			{
				string stats = PageContext.Localization.GetText( "ATTACHMENTINFO" );
				string strFileIcon = PageContext.Theme.GetItem( "ICONS", "ATTACHED_FILE" );

				messageOutput.Append( "<p>" );

				using ( DataTable dt = YAF.Classes.Data.DB.attachment_list( DataRow ["MessageID"], null, null ) )
				{
					// show file then image attachments...
					int tmpDisplaySort = 0;

					while ( tmpDisplaySort <= 1 )
					{
						bool bFirstItem = true;

						foreach ( DataRow dr in dt.Rows )
						{
							string strFilename = Convert.ToString( dr ["FileName"] ).ToLower();
							bool bShowImage = false;

							// verify it's not too large to display (might want to make this a board setting)
							if ( Convert.ToInt32( dr ["Bytes"] ) <= 262144 )
							{
								// is it an image file?
								for ( int i = 0; i < aImageExtensions.Length; i++ )
								{
									if ( strFilename.ToLower().EndsWith( aImageExtensions [i] ) )
									{
										bShowImage = true;
										break;
									}
								}
							}

							if ( bShowImage && tmpDisplaySort == 1 )
							{
								if ( bFirstItem )
								{
									messageOutput.AppendLine( @"<i class=""smallfont"">" );
									messageOutput.AppendFormat( PageContext.Localization.GetText( "IMAGE_ATTACHMENT_TEXT" ), Convert.ToString( DataRow ["UserName"] ) );
									messageOutput.AppendLine( @"</i><br />" );
									bFirstItem = false;
								}
								messageOutput.AppendFormat( @"<img src=""{0}resource.ashx?a={1}"" alt=""{2}"" /><br />", yaf_ForumInfo.ForumRoot, dr ["AttachmentID"], Server.HtmlEncode( Convert.ToString( dr ["FileName"] ) ) );
							}
							else if ( !bShowImage && tmpDisplaySort == 0 )
							{
								if ( bFirstItem )
								{
									messageOutput.AppendFormat( @"<b class=""smallfont"">{0}</b><br />", PageContext.Localization.GetText( "ATTACHMENTS" ) );
									bFirstItem = false;
								}
								// regular file attachment
								int kb = ( 1023 + ( int ) dr ["Bytes"] ) / 1024;
								messageOutput.AppendFormat( @"<img border=""0"" alt="""" src=""{0}"" /> <b><a href=""{1}resource.ashx?a={2}"">{3}</a></b> <span class=""smallfont"">{4}</span><br />", strFileIcon, yaf_ForumInfo.ForumRoot, dr ["AttachmentID"], dr ["FileName"], String.Format( stats, kb, dr ["Downloads"] ) );
							}
						}
						// now show images
						tmpDisplaySort++;
						messageOutput.AppendLine( "<br />" );
					}
				}
			}
		}

		private void PopMenu1_ItemClick( object sender, PopEventArgs e )
		{
			switch ( e.Item )
			{
				case "userprofile":
					yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.profile, "u={0}", DataRow ["UserID"] );
					break;
				case "edituser":
					yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_edituser, "u={0}", DataRow ["UserID"] );
					break;
			}
		}
	}
}
