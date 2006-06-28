namespace yaf.controls
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

		private void Page_Load(object sender, System.EventArgs e)
		{
			PopMenu1.Visible = ForumPage.IsAdmin;
			if(PopMenu1.Visible) 
			{
				PopMenu1.ItemClick += new PopEventHandler(PopMenu1_ItemClick);
				PopMenu1.AddItem("userprofile","User Profile");
				PopMenu1.AddItem("edituser","Edit User (Admin)");
				PopMenu1.Attach(UserName);
			}

			Page.RegisterClientScriptBlock("yafjs",string.Format("<script language='javascript' src='{0}'></script>",ResolveUrl("../yaf.js")));
			NameCell.ColSpan = int.Parse(GetIndentSpan());
		}
		private void DisplayPost_PreRender(object sender,EventArgs e)
		{
			Attach.Visible		= CanAttach;
			Attach.Text			= ForumPage.GetThemeContents("BUTTONS","ATTACHMENTS");
			Attach.ToolTip		= "Attachments";
			Attach.NavigateUrl	= Forum.GetLink(Pages.attachments,"m={0}",DataRow["MessageID"]);
			Edit.Visible		= CanEditPost;
			Edit.Text			= ForumPage.GetThemeContents("BUTTONS","EDITPOST");
			Edit.ToolTip		= "Edit this post";
			Edit.NavigateUrl	= Forum.GetLink(Pages.postmessage,"m={0}",DataRow["MessageID"]);
			Delete.Visible		= CanDeletePost;
			Delete.Text			= ForumPage.GetThemeContents("BUTTONS","DELETEPOST");
			Delete.ToolTip		= "Delete this post";
			Delete.Attributes["onclick"] = string.Format("return confirm('{0}')",ForumPage.GetText("confirm_deletemessage"));
			Quote.Visible		= CanReply;
			Quote.Text			= ForumPage.GetThemeContents("BUTTONS","QUOTEPOST");
			Quote.ToolTip		= "Reply with quote";
			Quote.NavigateUrl	= Forum.GetLink(Pages.postmessage,"t={0}&f={1}&q={2}",ForumPage.PageTopicID,ForumPage.PageForumID,DataRow["MessageID"]);

			// private messages
			Pm.Visible			= ForumPage.User!=null && ForumPage.BoardSettings.AllowPrivateMessages;
			Pm.Text				= ForumPage.GetThemeContents("BUTTONS","PM");
			Pm.NavigateUrl		= Forum.GetLink(Pages.pmessage,"u={0}",DataRow["UserID"]);
			// emailing
			Email.Visible		= ForumPage.User!=null && ForumPage.BoardSettings.AllowEmailSending;
			Email.NavigateUrl	= Forum.GetLink(Pages.im_email,"u={0}",DataRow["UserID"]);
			Email.Text			= ForumPage.GetThemeContents("BUTTONS","EMAIL");
			Home.Visible		= DataRow["HomePage"]!=DBNull.Value;
			Home.NavigateUrl	= DataRow["HomePage"].ToString();
			Home.Text			= ForumPage.GetThemeContents("BUTTONS","WWW");
			Blog.Visible		= DataRow["Weblog"]!=DBNull.Value;
			Blog.NavigateUrl	= DataRow["Weblog"].ToString();
			Blog.Text			= ForumPage.GetThemeContents("BUTTONS","WEBLOG");
			Msn.Visible			= ForumPage.User!=null && DataRow["MSN"]!=DBNull.Value;
			Msn.Text			= ForumPage.GetThemeContents("BUTTONS","MSN");
			Msn.NavigateUrl		= Forum.GetLink(Pages.im_email,"u={0}",DataRow["UserID"]);
			Yim.Visible			= ForumPage.User!=null && DataRow["YIM"]!=DBNull.Value;
			Yim.NavigateUrl		= Forum.GetLink(Pages.im_yim,"u={0}",DataRow["UserID"]);
			Yim.Text			= ForumPage.GetThemeContents("BUTTONS","YAHOO");
			Aim.Visible			= ForumPage.User!=null && DataRow["AIM"]!=DBNull.Value;
			Aim.Text			= ForumPage.GetThemeContents("BUTTONS","AIM");
			Aim.NavigateUrl		= Forum.GetLink(Pages.im_aim,"u={0}",DataRow["UserID"]);
			Icq.Visible			= ForumPage.User!=null && DataRow["ICQ"]!=DBNull.Value;
			Icq.Text			= ForumPage.GetThemeContents("BUTTONS","ICQ");
			Icq.NavigateUrl		= Forum.GetLink(Pages.im_icq,"u={0}",DataRow["UserID"]);

			// display admin only info
			if (ForumPage.IsAdmin)
			{
				AdminInfo.InnerHtml = "<span class='smallfont'>"; 
				if (Convert.ToDateTime(DataRow["Edited"]) != Convert.ToDateTime(DataRow["Posted"]))
				{
					// message has been edited
					AdminInfo.InnerHtml += String.Format("<b>Edited:</b> {0}",ForumPage.FormatDateTimeShort(Convert.ToDateTime(DataRow["Edited"])));
				}
				AdminInfo.InnerHtml += String.Format(" <b>IP:</b> {0}",DataRow["IP"].ToString());
				AdminInfo.InnerHtml += "</span>";
			}				
		}


		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(DisplayPost_PreRender);
			Delete.Click += new EventHandler(Delete_Click);
			base.OnInit(e);
		}

		private DataRowView	m_row = null;
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

		protected bool CanEditPost
		{
			get
			{
				return !PostLocked && ((int)DataRow["ForumFlags"] & (int)ForumFlags.Locked)!=(int)ForumFlags.Locked && ((int)DataRow["TopicFlags"] & (int)TopicFlags.Locked)!=(int)TopicFlags.Locked && ((int)DataRow["UserID"]==ForumPage.PageUserID || ForumPage.ForumModeratorAccess) && ForumPage.ForumEditAccess;
			}
		}

		private bool PostLocked
		{
			get
			{
				if(!ForumPage.IsAdmin && ForumPage.BoardSettings.LockPosts>0) 
				{
					DateTime edited = (DateTime)DataRow["Edited"];
					if(edited.AddDays(ForumPage.BoardSettings.LockPosts)<DateTime.Now)
						return true;
				}
				return false;
			}
		}

		protected bool CanAttach
		{
			get
			{
				return !PostLocked && ((int)DataRow["ForumFlags"] & (int)ForumFlags.Locked)!=(int)ForumFlags.Locked && ((int)DataRow["TopicFlags"] & (int)TopicFlags.Locked)!=(int)TopicFlags.Locked && ((int)DataRow["UserID"]==ForumPage.PageUserID || ForumPage.ForumModeratorAccess) && ForumPage.ForumUploadAccess;
			}
		}

		protected bool CanDeletePost
		{
			get
			{
				return !PostLocked && ((int)DataRow["ForumFlags"] & (int)ForumFlags.Locked)!=(int)ForumFlags.Locked && ((int)DataRow["TopicFlags"] & (int)TopicFlags.Locked)!=(int)TopicFlags.Locked && ((int)DataRow["UserID"]==ForumPage.PageUserID || ForumPage.ForumModeratorAccess) && ForumPage.ForumDeleteAccess;
			}
		}
		protected bool CanReply
		{
			get
			{
				return ((int)DataRow["ForumFlags"] & (int)ForumFlags.Locked)!=(int)ForumFlags.Locked && ((int)DataRow["TopicFlags"] & (int)TopicFlags.Locked)!=(int)TopicFlags.Locked && ForumPage.ForumReplyAccess;
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
			if(!IsThreaded)
				return "";

			int iIndent = (int)DataRow["Indent"];
			if(iIndent>0)
				return string.Format("<td rowspan='3' width='1%'><img src='{1}images/spacer.gif' width='{0}' height='2'/></td>",iIndent*32,Data.ForumRoot);
			else
				return "";
		}
		protected string GetIndentSpan() 
		{
			if(!IsThreaded || (int)DataRow["Indent"]==0)
				return "2";
			else
				return "1";
		}
		protected string GetPostClass() 
		{
			if(this.IsAlt)
				return "post_alt";
			else
				return "post";
		}

		protected string FormatUserBox() 
		{
			System.Data.DataRowView row = DataRow;
			string html = "";

			// Avatar
			if(ForumPage.BoardSettings.AvatarUpload && row["HasAvatarImage"]!=null && long.Parse(row["HasAvatarImage"].ToString())>0) 
			{
				html += String.Format("<img src='{1}image.aspx?u={0}'><br clear=\"all\"/>",row["UserID"],Data.ForumRoot);
			} 
			else if(row["Avatar"].ToString().Length>0) // Took out ForumPage.BoardSettings.AvatarRemote
			{
				//html += String.Format("<img src='{0}'><br clear=\"all\"/>",row["Avatar"]);
				html += String.Format("<img src='{3}image.aspx?url={0}&width={1}&height={2}'><br clear=\"all\"/>",
					Server.UrlEncode(row["Avatar"].ToString()),
					ForumPage.BoardSettings.AvatarWidth,
					ForumPage.BoardSettings.AvatarHeight,
					Data.ForumRoot
					);
			}

			// Rank Image
			if(row["RankImage"].ToString().Length>0)
				html += String.Format("<img align=left src=\"{0}images/ranks/{1}\"/><br clear=\"all\"/>",Data.ForumRoot,row["RankImage"]);

			// Rank
			html += String.Format("{0}: {1}<br clear=\"all\"/>",ForumPage.GetText("rank"),row["RankName"]);

			// Groups
			if(ForumPage.BoardSettings.ShowGroups) 
			{
				using(DataTable dt = DB.usergroup_list(row["UserID"])) 
				{
					html += String.Format("{0}: ",ForumPage.GetText("groups"));
					bool bFirst = true;
					foreach(DataRow grp in dt.Rows) 
					{
						if(bFirst) 
						{
							html += grp["Name"].ToString();
							bFirst = false;
						} 
						else 
						{
							html += String.Format(", {0}",grp["Name"]);
						}
					}
					html += "<br/>";
				}
			}

			// Extra row
			html += "<br/>";

			// Joined
			html += String.Format("{0}: {1}<br/>",ForumPage.GetText("joined"),ForumPage.FormatDateShort((DateTime)row["Joined"]));

			// Posts
			html += String.Format("{0}: {1:N0}<br/>",ForumPage.GetText("posts"),row["Posts"]);

			// Location
			if(row["Location"].ToString().Length>0)
				html += String.Format("{0}: {1}<br/>",ForumPage.GetText("location"),FormatMsg.RepairHtml(ForumPage,row["Location"].ToString(),false));

			return html;
		}
		protected string FormatBody() 
		{
			DataRowView row = DataRow;

			string html2 = FormatMsg.FormatMessage(ForumPage,row["Message"].ToString(),new MessageFlags(Convert.ToInt32(row["Flags"])));
			
			// define valid image extensions
			string[] aImageExtensions = {"jpg","gif","png"};
		
			if (long.Parse(row["HasAttachments"].ToString()) > 0) 
			{				
				string stats = ForumPage.GetText("ATTACHMENTINFO");
				string strFileIcon = ForumPage.GetThemeContents("ICONS","ATTACHED_FILE");

				html2 += "<p>";

				using(DataTable dt = DB.attachment_list(row["MessageID"],null,null)) 
				{
					// show file then image attachments...
					int tmpDisplaySort = 0;
					
					while (tmpDisplaySort <= 1)
					{
						bool bFirstItem = true;

						foreach(DataRow dr in dt.Rows) 
						{
							string strFilename = Convert.ToString(dr["FileName"]).ToLower();
							bool bShowImage = false;

							// verify it's not too large to display (might want to make this a board setting)
							if (Convert.ToInt32(dr["Bytes"]) <= 262144)
							{
								// is it an image file?
								for (int i=0;i<aImageExtensions.Length;i++)
								{
									if (strFilename.EndsWith(aImageExtensions[i]))
									{
										bShowImage = true;
										break;
									}									
								}
							}

							if (bShowImage && tmpDisplaySort == 1)
							{
								if (bFirstItem)
								{
									html2 += "<i class=\"smallfont\">";
									html2 += String.Format(ForumPage.GetText("IMAGE_ATTACHMENT_TEXT"),Convert.ToString(row["UserName"]));
									html2 += "</i><br/>";
									bFirstItem = false;
								}
								html2 += String.Format("<img src=\"{0}image.aspx?a={1}\" alt=\"{2}\"><br/>",Data.ForumRoot,dr["AttachmentID"],Server.HtmlEncode(Convert.ToString(dr["FileName"])));
							}
							else if (!bShowImage && tmpDisplaySort == 0)
							{
								if (bFirstItem)
								{
									html2 += String.Format("<b class=\"smallfont\">{0}</b><br/>",ForumPage.GetText("ATTACHMENTS"));
									bFirstItem = false;
								}
								// regular file attachment
								int kb = (1023 + (int)dr["Bytes"]) / 1024;
								html2 += String.Format("<img border='0' src='{0}'> <b><a href=\"{1}image.aspx?a={2}\">{3}</a></b> <span class='smallfont'>{4}</span><br/>",strFileIcon,Data.ForumRoot,dr["AttachmentID"],dr["FileName"],String.Format(stats,kb,dr["Downloads"]));
							}
						}
						// now show images
						tmpDisplaySort++;
						html2 += "<br/>";
					}
				}
			}
			
			if(row["Signature"] != DBNull.Value && row["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" && ForumPage.BoardSettings.AllowSignatures)
			{
				// don't allow any HTML on signatures
				MessageFlags tFlags = new MessageFlags();
				tFlags.IsHTML = false;

				html2 += "<br/><hr noshade/>" + FormatMsg.FormatMessage(ForumPage,row["Signature"].ToString(),tFlags);
			}

			return html2;
		}

		private void Delete_Click(object sender,EventArgs e)
		{
			if(!CanDeletePost)
				return;

			// CHANGED BAI 30.01.2004
      
			//Create objects for easy access
			object tmpMessageID = DataRow["MessageID"];
			object tmpForumID   = DataRow["ForumID"];
			object tmpTopicID   = DataRow["TopicID"];
			
			// Delete message. If it is the last message of the topic, the topic is also deleted
			DB.message_delete(tmpMessageID);
			
			// retrieve topic information.
			DataRow topic = DB.topic_info(tmpTopicID);
			
			//If topic has been deleted, redirect to topic list for active forum, else show remaining posts for topic
			if (topic == null)
				Forum.Redirect(Pages.topics,"f={0}",tmpForumID);
			else
				Forum.Redirect(Pages.posts,"t={0}",tmpTopicID);
      
			// END CHANGED BAI 30.01.2004
		}

		private void PopMenu1_ItemClick(object sender, PopEventArgs e)
		{
			switch(e.Item) 
			{
				case "userprofile":
					Forum.Redirect(Pages.profile,"u={0}",DataRow["UserID"]);
					break;
				case "edituser":
					Forum.Redirect(Pages.admin_edituser,"u={0}",DataRow["UserID"]);
					break;
			}
		}
	}
}
