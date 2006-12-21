namespace YAF.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for DisplayPost.
	/// </summary>
	public partial class DisplayPost : BaseUserControl
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			PopMenu1.Visible = ForumPage.IsAdmin;
			if ( PopMenu1.Visible )
			{
				PopMenu1.ItemClick += new PopEventHandler( PopMenu1_ItemClick );
				PopMenu1.AddItem( "userprofile", "User Profile" );
				PopMenu1.AddItem( "edituser", "Edit User (Admin)" );
				PopMenu1.Attach( UserName );
			}

			Page.ClientScript.RegisterClientScriptBlock( this.GetType(), "yafjs", string.Format( "<script language='javascript' src='{0}'></script>", ForumPage.GetURLToResource("yaf.js" ) ) );
			NameCell.ColSpan = int.Parse( GetIndentSpan() );
		}

		private void DisplayPost_PreRender( object sender, EventArgs e )
		{
			// TODO localize tooltips
			Attach.Visible = !PostDeleted && CanAttach;
			Attach.Text = ForumPage.GetThemeContents( "BUTTONS", "ATTACHMENTS" );
			Attach.ToolTip = "Attachments";
			Attach.NavigateUrl = Forum.GetLink( ForumPages.attachments, "m={0}", DataRow ["MessageID"] );
            Edit.Visible = !PostDeleted && CanEditPost;
			Edit.Text = ForumPage.GetThemeContents( "BUTTONS", "EDITPOST" );
			Edit.ToolTip = "Edit this post";
			Edit.NavigateUrl = Forum.GetLink( ForumPages.postmessage, "m={0}", DataRow ["MessageID"] );
            MovePost.Visible = ForumPage.ForumModeratorAccess;
            MovePost.Text = ForumPage.GetThemeContents("BUTTONS", "MOVEPOST");
            MovePost.ToolTip = "Move this post";
            MovePost.NavigateUrl = Forum.GetLink(ForumPages.movemessage, "m={0}", DataRow["MessageID"]);
            Delete.Visible = !PostDeleted && CanDeletePost;
			Delete.Text = ForumPage.GetThemeContents( "BUTTONS", "DELETEPOST" );
            Delete.ToolTip = "Delete this post";
            Delete.NavigateUrl = Forum.GetLink(ForumPages.deletemessage, "m={0}&action=delete", DataRow["MessageID"]);
            UnDelete.Visible = CanUnDeletePost;
            UnDelete.Text = ForumPage.GetThemeContents("BUTTONS", "UNDELETEPOST");
            UnDelete.ToolTip = "UnDelete this post";
            UnDelete.NavigateUrl = Forum.GetLink(ForumPages.deletemessage, "m={0}&action=undelete", DataRow["MessageID"]);
            Quote.Visible = !PostDeleted && CanReply;
			Quote.Text = ForumPage.GetThemeContents( "BUTTONS", "QUOTEPOST" );
			Quote.ToolTip = "Reply with quote";
			Quote.NavigateUrl = Forum.GetLink( ForumPages.postmessage, "t={0}&f={1}&q={2}", ForumPage.PageTopicID, ForumPage.PageForumID, DataRow ["MessageID"] );

			// private messages
            Pm.Visible = !PostDeleted && ForumPage.User != null && ForumPage.BoardSettings.AllowPrivateMessages && !IsSponserMessage;
			Pm.Text = ForumPage.GetThemeContents( "BUTTONS", "PM" );
			Pm.NavigateUrl = Forum.GetLink( ForumPages.pmessage, "u={0}", DataRow ["UserID"] );
			// emailing
            Email.Visible = !PostDeleted && ForumPage.User != null && ForumPage.BoardSettings.AllowEmailSending && !IsSponserMessage;
			Email.NavigateUrl = Forum.GetLink( ForumPages.im_email, "u={0}", DataRow ["UserID"] );
			Email.Text = ForumPage.GetThemeContents( "BUTTONS", "EMAIL" );
            // home page
            Home.Visible = !PostDeleted && DataRow["HomePage"] != DBNull.Value;
			Home.NavigateUrl = DataRow ["HomePage"].ToString();
			Home.Text = ForumPage.GetThemeContents( "BUTTONS", "WWW" );
            // blog page
            Blog.Visible = !PostDeleted && DataRow["Weblog"] != DBNull.Value;
			Blog.NavigateUrl = DataRow ["Weblog"].ToString();
			Blog.Text = ForumPage.GetThemeContents( "BUTTONS", "WEBLOG" );
            // MSN
            Msn.Visible = !PostDeleted && ForumPage.User != null && DataRow["MSN"] != DBNull.Value;
			Msn.Text = ForumPage.GetThemeContents( "BUTTONS", "MSN" );
			Msn.NavigateUrl = Forum.GetLink( ForumPages.im_email, "u={0}", DataRow ["UserID"] );
            // Yahoo IM
            Yim.Visible = !PostDeleted && ForumPage.User != null && DataRow["YIM"] != DBNull.Value;
			Yim.NavigateUrl = Forum.GetLink( ForumPages.im_yim, "u={0}", DataRow ["UserID"] );
			Yim.Text = ForumPage.GetThemeContents( "BUTTONS", "YAHOO" );
            // AOL IM
            Aim.Visible = !PostDeleted && ForumPage.User != null && DataRow["AIM"] != DBNull.Value;
			Aim.Text = ForumPage.GetThemeContents( "BUTTONS", "AIM" );
			Aim.NavigateUrl = Forum.GetLink( ForumPages.im_aim, "u={0}", DataRow ["UserID"] );
            // ICQ
            Icq.Visible = !PostDeleted && ForumPage.User != null && DataRow["ICQ"] != DBNull.Value;
			Icq.Text = ForumPage.GetThemeContents( "BUTTONS", "ICQ" );
			Icq.NavigateUrl = Forum.GetLink( ForumPages.im_icq, "u={0}", DataRow ["UserID"] );

            if (!PostDeleted)
            {
                AdminInfo.InnerHtml = "<span class='smallfont'>";
                if (Convert.ToDateTime(DataRow["Edited"]) > Convert.ToDateTime(DataRow["Posted"]).AddSeconds(ForumPage.BoardSettings.EditTimeOut))
                {
                    // message has been edited
                    // show, why the post was edited or deleted?
                    string whoChanged = (Convert.ToBoolean(DataRow["IsModeratorChanged"])) ? "by moderator" : "by user";
                    AdminInfo.InnerHtml += String.Format("|<b> <font color=red>{0} {1}:</font></b> {2}", "Edited", whoChanged, ForumPage.FormatDateTimeShort(Convert.ToDateTime(DataRow["Edited"])));
                    if (Server.HtmlDecode(Convert.ToString(DataRow["EditReason"])) != "")
                    {
                        // reason was specified
                        AdminInfo.InnerHtml += String.Format(" |<b> {0}:</b> {1}", "Reason", YAF.FormatMsg.RepairHtml(ForumPage, (string)DataRow["EditReason"], true));
                    }
                    else
                    {
                        //reason was not specified
                        AdminInfo.InnerHtml += String.Format(" |<b> {0}:</b> {1}", "Reason", "Not specified");
                    }
                }
            }
            else
            {
                AdminInfo.InnerHtml = "<span class='smallfont'>";
                if (Server.HtmlDecode(Convert.ToString(DataRow["DeleteReason"])) != String.Empty)
                {
                    // reason was specified
                    AdminInfo.InnerHtml += String.Format(" |<b> {0}:</b> {1}", "Reason", YAF.FormatMsg.RepairHtml(ForumPage, (string)DataRow["DeleteReason"], true));
                }
                else
                {
                    //reason was not specified
                    AdminInfo.InnerHtml += String.Format(" |<b> {0}:</b> {1}", "Reason", "Not specified");
                }
            }

            // display admin only info
            if (ForumPage.IsAdmin)
            {
                AdminInfo.InnerHtml += String.Format(" |<b> IP:</b> {0}", DataRow["IP"].ToString());
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

		private YAF.MessageFlags PostMessageFlags
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
				return (DataRow["IP"].ToString() == "none");
			}
		}

		protected bool CanEditPost
		{
			get
			{
				return !PostLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ( ( int ) DataRow ["UserID"] == ForumPage.PageUserID || ForumPage.ForumModeratorAccess ) && ForumPage.ForumEditAccess;
			}
		}

		private bool PostLocked
		{
			get
			{
				if ( PostMessageFlags.IsLocked ) return true;

				if ( !ForumPage.IsAdmin && ForumPage.BoardSettings.LockPosts > 0 )
				{
					DateTime edited = ( DateTime ) DataRow ["Edited"];
					if ( edited.AddDays( ForumPage.BoardSettings.LockPosts ) < DateTime.Now )
						return true;
				}
				return false;
			}
		}

        private bool PostDeleted
        {
            get
            {

                int deleted = (int)DataRow["Flags"] & 8;
                if (deleted == 8)
                    return true;
                return false;
            }
        }

		protected bool CanAttach
		{
			get
			{
				return !PostLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ( ( int ) DataRow ["UserID"] == ForumPage.PageUserID || ForumPage.ForumModeratorAccess ) && ForumPage.ForumUploadAccess;
			}
		}

		protected bool CanDeletePost
		{
			get
			{
				return !PostLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ( ( int ) DataRow ["UserID"] == ForumPage.PageUserID || ForumPage.ForumModeratorAccess ) && ForumPage.ForumDeleteAccess;
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
				return !PostMessageFlags.IsLocked && ( ( int ) DataRow ["ForumFlags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) DataRow ["TopicFlags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ForumPage.ForumReplyAccess;
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
				return string.Format( "<td rowspan='3' width='1%'><img src='{1}images/spacer.gif' width='{0}' height='2'/></td>", iIndent * 32, Data.ForumRoot );
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
            if (PostDeleted)
                return "0";
            return "100";
        }

		protected string FormatUserBox()
		{
			if ( IsSponserMessage ) return "";

			System.Text.StringBuilder userboxOutput = new System.Text.StringBuilder( 1000 );
            if (!PostDeleted)
            {
                // Avatar
                if (ForumPage.BoardSettings.AvatarUpload && DataRow["HasAvatarImage"] != null && long.Parse(DataRow["HasAvatarImage"].ToString()) > 0)
                {
                    userboxOutput.AppendFormat("<img src='{1}resource.ashx?u={0}' /><br clear=\"all\" />", DataRow["UserID"], Data.ForumRoot);
                }
                else if (DataRow["Avatar"].ToString().Length > 0) // Took out ForumPage.BoardSettings.AvatarRemote
                {
                    userboxOutput.AppendFormat("<img src='{3}resource.ashx?url={0}&width={1}&height={2}'><br clear=\"all\" />",
                        Server.UrlEncode(DataRow["Avatar"].ToString()),
                        ForumPage.BoardSettings.AvatarWidth,
                        ForumPage.BoardSettings.AvatarHeight,
                        Data.ForumRoot
                        );
                }
            }

			// Rank Image
			if ( DataRow ["RankImage"].ToString().Length > 0 )
				userboxOutput.AppendFormat( "<img align=left src=\"{0}images/ranks/{1}\" /><br clear=\"all\" />", Data.ForumRoot, DataRow ["RankImage"] );

			// Rank
			userboxOutput.AppendFormat( "{0}: {1}<br clear=\"all\" />", ForumPage.GetText( "rank" ), DataRow ["RankName"] );

			// Groups
			if ( ForumPage.BoardSettings.ShowGroups )
			{
				using ( DataTable dt = YAF.Classes.Data.DB.usergroup_list( DataRow ["UserID"] ) )
				{
					userboxOutput.AppendFormat( "{0}: ", ForumPage.GetText( "groups" ) );
					
					bool bFirst = true;

					foreach ( DataRow grp in dt.Rows )
					{
						if ( bFirst )
						{
							userboxOutput.AppendLine( grp ["Name"].ToString() );
							bFirst = false;
						}
						else
						{
							userboxOutput.AppendFormat( ", {0}", grp ["Name"] );
						}
					}
					userboxOutput.AppendLine( "<br/>" );
				}
			}
            if (!PostDeleted)
            {
                // Extra row
                userboxOutput.AppendLine("<br/>");

                // Joined
                userboxOutput.AppendFormat("{0}: {1}<br />", ForumPage.GetText("joined"), ForumPage.FormatDateShort((DateTime)DataRow["Joined"]));

                // Posts
                userboxOutput.AppendFormat("{0}: {1:N0}<br />", ForumPage.GetText("posts"), DataRow["Posts"]);


                // Points
                if (ForumPage.BoardSettings.DisplayPoints)
                {
                    userboxOutput.AppendFormat("{0}: {1:N0}<br />", ForumPage.GetText("points"), DataRow["Points"]);
                }

                // Location
                if (DataRow["Location"].ToString().Length > 0)
                    userboxOutput.AppendFormat("{0}: {1}<br />", ForumPage.GetText("location"), FormatMsg.RepairHtml(ForumPage, DataRow["Location"].ToString(), false));
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
                messageOutput.Append(FormatMsg.FormatMessage(ForumPage, DataRow["Message"].ToString(), PostMessageFlags, Convert.ToBoolean(DataRow["IsModeratorChanged"])));
			}

            if (!PostDeleted)
            {

                AddAttachedFiles(ref messageOutput);

                if (ForumPage.BoardSettings.AllowSignatures && DataRow["Signature"] != DBNull.Value && DataRow["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" && DataRow["Signature"].ToString().Trim().Length > 0)
                {
                    // don't allow any HTML on signatures
                    MessageFlags tFlags = new MessageFlags();
                    tFlags.IsHTML = false;

                    messageOutput.Append("<br/><hr noshade />" + FormatMsg.FormatMessage(ForumPage, DataRow["Signature"].ToString(), tFlags));
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
				string stats = ForumPage.GetText( "ATTACHMENTINFO" );
				string strFileIcon = ForumPage.GetThemeContents( "ICONS", "ATTACHED_FILE" );

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
									messageOutput.AppendFormat( ForumPage.GetText( "IMAGE_ATTACHMENT_TEXT" ), Convert.ToString( DataRow ["UserName"] ) );
									messageOutput.AppendLine( @"</i><br />" );
									bFirstItem = false;
								}
								messageOutput.AppendFormat( @"<img src=""{0}resource.ashx?a={1}"" alt=""{2}"" /><br />", Data.ForumRoot, dr ["AttachmentID"], Server.HtmlEncode( Convert.ToString( dr ["FileName"] ) ) );
							}
							else if ( !bShowImage && tmpDisplaySort == 0 )
							{
								if ( bFirstItem )
								{
									messageOutput.AppendFormat( @"<b class=""smallfont"">{0}</b><br />", ForumPage.GetText( "ATTACHMENTS" ) );
									bFirstItem = false;
								}
								// regular file attachment
								int kb = ( 1023 + ( int ) dr ["Bytes"] ) / 1024;
								messageOutput.AppendFormat( @"<img border=""0"" alt="""" src=""{0}"" /> <b><a href=""{1}resource.ashx?a={2}"">{3}</a></b> <span class=""smallfont"">{4}</span><br />", strFileIcon, Data.ForumRoot, dr ["AttachmentID"], dr ["FileName"], String.Format( stats, kb, dr ["Downloads"] ) );
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
					Forum.Redirect( ForumPages.profile, "u={0}", DataRow ["UserID"] );
					break;
				case "edituser":
					Forum.Redirect( ForumPages.admin_edituser, "u={0}", DataRow ["UserID"] );
					break;
			}
		}
	}
}
