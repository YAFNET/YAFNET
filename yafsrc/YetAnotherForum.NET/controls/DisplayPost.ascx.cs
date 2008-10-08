/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Data;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	///		Summary description for DisplayPost.
	/// </summary>
	public partial class DisplayPost : YAF.Classes.Base.BaseUserControl
	{
		#region Data Members

		// for parent page referencing
		private YAF.Classes.Base.ForumPage _parentPage;

		private DataRowView _row = null;
		private YafUserProfile _userProfile = null;
		private bool _isAlt = false;
		private bool _isThreaded = false;
		// flags
		private ForumFlags _forumFlags;
		private TopicFlags _topicFlags;
		private MessageFlags _messageFlags;

		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
			PopMenu1.Visible = PageContext.IsAdmin && !IsGuest;
			if (PopMenu1.Visible)
			{
				PopMenu1.ItemClick += new PopEventHandler(PopMenu1_ItemClick);
				PopMenu1.AddPostBackItem("userprofile", "User Profile");
				PopMenu1.AddPostBackItem("edituser", "Edit User (Admin)");
				PopMenu1.Attach(UserProfileLink);
			}

			ScriptManager.RegisterClientScriptInclude( this, typeof( DisplayPost ), "yafjs", YAF.Classes.Utils.YafForumInfo.GetURLToResource( "yaf.js" ) );
			NameCell.ColSpan = int.Parse(GetIndentSpan());
		}

		private void DisplayPost_PreRender( object sender, EventArgs e )
		{
			Attach.Visible = !PostDeleted && CanAttach && !IsLocked;

			Attach.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.attachments, "m={0}", DataRow ["MessageID"] );
			Edit.Visible = !PostDeleted && CanEditPost && !IsLocked;
			Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.postmessage, "m={0}", DataRow ["MessageID"] );
			MovePost.Visible = PageContext.ForumModeratorAccess && !IsLocked;
			MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.movemessage, "m={0}", DataRow ["MessageID"] );
			Delete.Visible = !PostDeleted && CanDeletePost && !IsLocked;
			Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.deletemessage, "m={0}&action=delete", DataRow ["MessageID"] );
			UnDelete.Visible = CanUnDeletePost && !IsLocked;
			UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.deletemessage, "m={0}&action=undelete", DataRow ["MessageID"] );
			Quote.Visible = !PostDeleted && CanReply && !IsLocked;
			Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.postmessage, "t={0}&f={1}&q={2}", PageContext.PageTopicID, PageContext.PageForumID, DataRow ["MessageID"] );

			// report posts
			ReportButton.Visible = PageContext.BoardSettings.AllowReportAbuse && !IsGuest; // Mek Addition 08/18/2007
			ReportButton.Text = PageContext.Localization.GetText( "REPORTPOST" ); // Mek Addition 08/18/2007
			ReportButton.Attributes.Add( "onclick", String.Format( "return confirm('{0}');", PageContext.Localization.GetText( "CONFIRM_REPORTPOST" ) ) );

			// report spam
			ReportSpamButton.Visible = PageContext.BoardSettings.AllowReportSpam && !IsGuest; // Mek Addition 08/18/2007
			ReportSpamButton.Text = PageContext.Localization.GetText( "REPORTSPAM" ); // Mek Addition 08/18/2007
			ReportSpamButton.Attributes.Add( "onclick", String.Format( "return confirm('{0}');", PageContext.Localization.GetText( "CONFIRM_REPORTSPAM" ) ) );

			// private messages
			Pm.Visible = !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowPrivateMessages && !IsSponserMessage;
			Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.pmessage, "u={0}", DataRow ["UserID"] );

			// emailing
			Email.Visible = !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowEmailSending && !IsSponserMessage;
			Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_email, "u={0}", DataRow ["UserID"] );

			// home page
			Home.Visible = !PostDeleted && !String.IsNullOrEmpty( UserProfile.Homepage );
			SetupThemeButtonWithLink( Home, UserProfile.Homepage );

			// blog page
			Blog.Visible = !PostDeleted && !String.IsNullOrEmpty( UserProfile.Blog );
			SetupThemeButtonWithLink( Blog, UserProfile.Blog );

			// MSN
			Msn.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.MSN );
			Msn.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_email, "u={0}", DataRow ["UserID"] );

			// Yahoo IM
			Yim.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.YIM );
			Yim.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_yim, "u={0}", DataRow ["UserID"] );

			// AOL IM
			Aim.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.AIM );
			Aim.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_aim, "u={0}", DataRow ["UserID"] );

			// ICQ
			Icq.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.ICQ );
			Icq.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_icq, "u={0}", DataRow ["UserID"] );

			// Skype
			Skype.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.Skype );
			Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_skype, "u={0}", DataRow ["UserID"] );

			if ( !PostDeleted )
			{
				AdminInformation.InnerHtml = @"<span class=""smallfont"">";
				if ( Convert.ToDateTime( DataRow ["Edited"] ) > Convert.ToDateTime( DataRow ["Posted"] ).AddSeconds( PageContext.BoardSettings.EditTimeOut ) )
				{
					// message has been edited
					// show, why the post was edited or deleted?
					string whoChanged = (Convert.ToBoolean(DataRow["IsModeratorChanged"])) ? PageContext.Localization.GetText("EDITED_BY_MOD") : PageContext.Localization.GetText("EDITED_BY_USER");
					AdminInformation.InnerHtml += String.Format(@"| <span class=""editedinfo"">{0} {1}:</span> {2}", PageContext.Localization.GetText("EDITED"), whoChanged, YafDateTime.FormatDateTimeShort(Convert.ToDateTime(DataRow["Edited"])));
					if ( Server.HtmlDecode( Convert.ToString( DataRow ["EditReason"] ) ) != "" )
					{
						// reason was specified
						AdminInformation.InnerHtml += String.Format(" |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), FormatMsg.RepairHtml((string)DataRow["EditReason"], true));
					}
					else
					{
						//reason was not specified
						AdminInformation.InnerHtml += String.Format(" |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), PageContext.Localization.GetText("EDIT_REASON_NA"));
					}
				}
			}
			else
			{
				AdminInformation.InnerHtml = @"<span class=""smallfont"">";
				if ( Server.HtmlDecode( Convert.ToString( DataRow ["DeleteReason"] ) ) != String.Empty )
				{
					// reason was specified
					AdminInformation.InnerHtml += String.Format(" |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), FormatMsg.RepairHtml((string)DataRow["DeleteReason"], true));
				}
				else
				{
					//reason was not specified
					AdminInformation.InnerHtml += String.Format(" |<b> {0}:</b> {1}", PageContext.Localization.GetText("EDIT_REASON"), PageContext.Localization.GetText("EDIT_REASON_NA"));
				}
			}

			// display admin only info
			if ( PageContext.IsAdmin )
			{
				AdminInformation.InnerHtml += String.Format(" |<b> {0}:</b> {1}", PageContext.Localization.GetText("IP"), DataRow["IP"].ToString());
			}
			AdminInformation.InnerHtml += "</span>";
		}


		override protected void OnInit( EventArgs e )
		{
			ReportButton.Command += new CommandEventHandler( Report_Command );
			ReportSpamButton.Command += new CommandEventHandler( Report_Command );
			this.PreRender += new EventHandler( DisplayPost_PreRender );
			this.Init += new EventHandler(DisplayPost_Init);
			base.OnInit( e );
		}

		void DisplayPost_Init(object sender, EventArgs e)
		{
			// retrieves parent page (ForumPage)
			RetrieveParentPage();
		}

		protected void SetupThemeButtonWithLink( ThemeButton thisButton, string linkUrl )
		{
			if ( !String.IsNullOrEmpty( linkUrl ) )
			{
				string link = linkUrl.Replace( "\"", "" );
				if ( !link.ToLower().StartsWith( "http" ) )
				{
					link = "http://" + link;
				}
				thisButton.NavigateUrl = link;
				thisButton.Attributes.Add( "target", "_blank" );
				if ( PageContext.BoardSettings.UseNoFollowLinks ) thisButton.Attributes.Add( "rel", "nofollow" );
			}
			else
			{
				thisButton.NavigateUrl = "";
			}
		}

		/// <summary>
		/// Retrieve parent page (ForumPage) if it exists.
		/// </summary>
		protected void RetrieveParentPage()
		{
			// get parent page (ForumPage type), if applicable
			System.Web.UI.Control parent = this.Parent;
			// cycle until no there is no parent 
			while (parent != null)
			{
				// is parent control of desired type?
				if (parent is YAF.Classes.Base.ForumPage)
				{
					_parentPage = (YAF.Classes.Base.ForumPage)parent;
					break;
				}
				else
				{
					// go one step up in hierarchy
					parent = parent.Parent;

					// are we topmost?
					if (parent == null)
					{
						_parentPage = null;
						break;
					}
				}
			}
		}


		/// <summary>
		/// Gets parent forum page (null if parent is not ForumPage).
		/// </summary>
		public YAF.Classes.Base.ForumPage ParentPage
		{
			get { return _parentPage; }
		}


		public DataRowView DataRow
		{
			get
			{
				return _row;
			}
			set
			{
				_row = value;

				// get all flags for forum, topic and message
				if (_row != null)
				{
					_forumFlags = new ForumFlags(_row["ForumFlags"]);
					_topicFlags = new TopicFlags(_row["TopicFlags"]);
					_messageFlags = new YAF.Classes.Data.MessageFlags(_row["Flags"]);
				}
				else
				{
					_forumFlags = new ForumFlags(0);
					_topicFlags = new TopicFlags(0);
					_messageFlags = new MessageFlags(0);
				}
			}
		}

		public bool IsGuest
		{
			get
			{
				if ( DataRow != null )
				{
					return UserMembershipHelper.IsGuestUser( DataRow ["UserID"] );
				}
				else return true;
			}
		}

		/// <summary>
		/// IsLocked flag should only be used for "ghost" posts such as the
		/// Sponser post that isn't really there.
		/// </summary>
		public bool IsLocked
		{
			get
			{
				if ( _messageFlags != null )
				{
					return _messageFlags.IsLocked;
				}
				
				return false;
			}
		}

		public YafUserProfile UserProfile
		{
			get
			{
				if ( _userProfile == null )
				{
					// setup instance of the user profile...
					_userProfile = PageContext.GetProfile( UserMembershipHelper.GetUserNameFromID( Convert.ToInt32( DataRow ["UserID"] ) ) );
				}

				return _userProfile;
			}
		}

		/*private MessageFlags PostMessageFlags
		{
			get
			{
				return new MessageFlags( Convert.ToInt32( DataRow ["Flags"] ) );
			}
		}*/

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
				// Ederon : 9/9/2007 - moderaotrs can edit locked posts
				// Ederon : 12/5/2007 - new flags implementation
				return ( ( !PostLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked &&
					( int ) DataRow ["UserID"] == PageContext.PageUserID ) || PageContext.ForumModeratorAccess ) &&
					PageContext.ForumEditAccess;
			}
		}

		private bool PostLocked
		{
			get
			{
				// post is explicitly locked
				if (_messageFlags.IsLocked) return true;

				// there is auto-lock period defined
				if ( !PageContext.IsAdmin && PageContext.BoardSettings.LockPosts > 0 )
				{
					DateTime edited = ( DateTime ) DataRow ["Edited"];
					// check if post is locked according to this rule
					if (edited.AddDays(PageContext.BoardSettings.LockPosts) < DateTime.Now)
						return true;
				}
				return false;
			}
		}

		private bool PostDeleted
		{
			get
			{
				return _messageFlags.IsDeleted;
			}
		}

		protected bool CanAttach
		{
			get
			{
				// Ederon : 9/9/2007 - moderaotrs can attack to locked posts
				return ( ( !PostLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked &&
					( int ) DataRow ["UserID"] == PageContext.PageUserID ) || PageContext.ForumModeratorAccess ) &&
					PageContext.ForumUploadAccess;
			}
		}

		protected bool CanDeletePost
		{
			get
			{
				// Ederon : 9/9/2007 - moderaotrs can delete in locked posts
				return ((!PostLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked &&
					( int ) DataRow ["UserID"] == PageContext.PageUserID ) || PageContext.ForumModeratorAccess ) &&
					PageContext.ForumDeleteAccess;
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
				// Ederon : 9/9/2007 - moderaotrs can reply in locked posts
				return ((!_messageFlags.IsLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked) ||
					PageContext.ForumModeratorAccess) && PageContext.ForumReplyAccess;
			}
		}

		public bool IsAlt
		{
			get { return this._isAlt; }
			set { this._isAlt = value; }
		}

		public bool IsThreaded
		{
			get
			{
				return _isThreaded;
			}
			set
			{
				_isThreaded = value;
			}
		}

		protected string GetIndentCell()
		{
			if ( !IsThreaded )
				return "";

			int iIndent = ( int ) DataRow ["Indent"];
			if ( iIndent > 0 )
				return string.Format( @"<td rowspan=""3"" width=""1%""><img src=""{1}images/spacer.gif"" width=""{0}"" height=""2"" alt=""""/></td>", iIndent * 32, YafForumInfo.ForumRoot );
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
		// TODO: Make into a Control and move all this code out of this file...
		protected string FormatUserBox()
		{
			#region Cache Retrieval

			// is this control contained in YAF.Classes.Base.ForumPage and has it any cache?
			if (ParentPage != null && ParentPage.PageCache != null)
			{
				// get cache for user boxes
				object cache = ParentPage.PageCache[Constants.Cache.UserBoxes];

				// is it hashtable?
				if (cache != null && cache is Hashtable)
				{
					// get only record for user who made message being
					cache = ((Hashtable)cache)[DataRow["UserID"]];

					// return from cache if there is something there
					if (cache != null && cache.ToString() != "") return cache.ToString();
				}
			}

			#endregion

			if ( IsSponserMessage ) return "";

			string userBox = PageContext.BoardSettings.UserBox;
			string filler = "";

			// for pattern matching
			Regex rx;


			// Avatar
			rx = new Regex(Constants.UserBox.Avatar);

			if (!PostDeleted &&
				(PageContext.BoardSettings.AvatarUpload && DataRow["HasAvatarImage"] != null && long.Parse(DataRow["HasAvatarImage"].ToString()) > 0))
			{
				filler = String.Format(
							PageContext.BoardSettings.UserBoxAvatar,
							String.Format(
								@"<img class=""avatarimage"" src=""{1}resource.ashx?u={0}"" alt="""" />",
								DataRow["UserID"],
								YafForumInfo.ForumRoot
								)
							);
			}
			else if (!PostDeleted &&
				DataRow["Avatar"].ToString().Length > 0) // Took out PageContext.BoardSettings.AvatarRemote
			{
				filler = String.Format(
							PageContext.BoardSettings.UserBoxAvatar,
							String.Format(
								@"<img class=""avatarimage"" src=""{3}resource.ashx?url={0}&amp;width={1}&amp;height={2}"" alt="""" /><br clear=""all"" />",
								Server.UrlEncode(DataRow["Avatar"].ToString()),
								PageContext.BoardSettings.AvatarWidth,
								PageContext.BoardSettings.AvatarHeight,
								YafForumInfo.ForumRoot
								)
							);
			}
			else
			{
				filler = "";
			}

			// replaces template placeholder with actual avatar
			userBox = rx.Replace(userBox, filler);


			// Ederon : 7/14/2007 - prepared for implementation of user medals
			// User Medals
			rx = new Regex(Constants.UserBox.Medals);

			if (PageContext.BoardSettings.ShowMedals)
			{
				using (DataTable dt = DB.user_listmedals(DataRow["UserID"]))
				{
					System.Text.StringBuilder ribbonBar = new System.Text.StringBuilder(500);
					System.Text.StringBuilder medals = new System.Text.StringBuilder(500);

					DataRow r;
					MedalFlags f;

					int i = 0;
					int inRow = 0;

					// do ribbon bar first
					while (dt.Rows.Count > i)
					{
						r = dt.Rows[i];
						f = new MedalFlags(r["Flags"]);
						
						// do only ribbon bar items first
						if (!(bool)r["OnlyRibbon"]) break;

						// skip hidden medals
						if (!f.AllowHiding || !(bool)r["Hide"])
						{
							if (inRow == 3)
							{
								// add break - only three ribbons in a row
								ribbonBar.Append("<br />");
								inRow = 0;
							}

							ribbonBar.AppendFormat(
								"<img src=\"{0}images/medals/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" />",
								YafForumInfo.ForumRoot,
								r["SmallRibbonURL"],
								r["SmallRibbonWidth"],
								r["SmallRibbonHeight"],
								r["Name"],
								f.ShowMessage ? String.Format(": {0}", r["Message"]) : ""
								);

							inRow++;
						}

						// move to next row
						i++;
					}

					// follow with the rest
					while (dt.Rows.Count > i)
					{
						r = dt.Rows[i];
						f = new MedalFlags(r["Flags"]);

						// skip hidden medals
						if (!f.AllowHiding || !(bool)r["Hide"])
						{
							medals.AppendFormat(
								"<img src=\"{0}images/medals/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" />",
								YafForumInfo.ForumRoot,
								r["SmallMedalURL"],
								r["SmallMedalWidth"],
								r["SmallMedalHeight"],
								r["Name"],
								f.ShowMessage ? String.Format(": {0}", r["Message"]) : ""
								);
						}

						// move to next row
						i++;
					}

					filler = String.Format(
								PageContext.BoardSettings.UserBoxMedals,
								ribbonBar.ToString(),
								medals.ToString()
								);
				}
			}
			else
			{
				filler = "";
			}

			// replaces template placeholder with actual medals
			userBox = rx.Replace(userBox, filler);


			// Rank Image
			rx = new Regex(Constants.UserBox.RankImage);

			if ( DataRow ["RankImage"].ToString().Length > 0 )
			{
				filler=	String.Format(
							PageContext.BoardSettings.UserBoxRankImage,
							String.Format(
								@"<img class=""rankimage"" src=""{0}images/ranks/{1}"" alt="""" />",
								YafForumInfo.ForumRoot,
								DataRow ["RankImage"]
								)
							);
			}
			else
			{
				filler = "";
			}

			// replaces template placeholder with actual rank image
			userBox = rx.Replace(userBox, filler);


			// Rank
			rx = new Regex(Constants.UserBox.Rank);

			filler = String.Format(
						PageContext.BoardSettings.UserBoxRank,
						PageContext.Localization.GetText("rank"),
						DataRow["RankName"]
						);

			// replaces template placeholder with actual rank
			userBox = rx.Replace(userBox, filler);


			// Groups
			rx = new Regex(Constants.UserBox.Groups);

			if ( PageContext.BoardSettings.ShowGroups )
			{
				System.Text.StringBuilder groupsText = new System.Text.StringBuilder( 500 );

				bool bFirst = true;

				foreach ( string role in System.Web.Security.Roles.GetRolesForUser( DataRow ["UserName"].ToString() ) )
				{
					if ( bFirst )
					{
						groupsText.AppendLine( role );
						bFirst = false;
					}
					else
					{
						groupsText.AppendFormat( ", {0}", role );
					}
				}

				filler = String.Format(
							PageContext.BoardSettings.UserBoxGroups,
							PageContext.Localization.GetText("groups"),
							groupsText.ToString()
							);
			}
			else
			{
				filler = "";
			}

			// replaces template placeholder with actual groups
			userBox = rx.Replace(userBox, filler);


			if ( !PostDeleted )
			{
				// Ederon : 02/24/2007
				// Joined Date
				rx = new Regex(Constants.UserBox.JoinDate);

				if ( PageContext.BoardSettings.DisplayJoinDate )
				{
					filler = String.Format(
								PageContext.BoardSettings.UserBoxJoinDate,
								PageContext.Localization.GetText("joined"),
								YafDateTime.FormatDateShort((DateTime)DataRow["Joined"])
								);
				}
				else
				{
					filler = "";
				}

				// replaces template placeholder with actual join date
				userBox = rx.Replace(userBox, filler);


				// Posts
				rx = new Regex(Constants.UserBox.Posts);

				filler = String.Format(
							PageContext.BoardSettings.UserBoxPosts,
							PageContext.Localization.GetText("posts"),
							DataRow["Posts"]
							);

				// replaces template placeholder with actual post count
				userBox = rx.Replace(userBox, filler);


				// Points
				rx = new Regex(Constants.UserBox.Points);

				if ( PageContext.BoardSettings.DisplayPoints )
				{
					filler = String.Format(
								PageContext.BoardSettings.UserBoxPoints,
								PageContext.Localization.GetText("points"),
								DataRow["Points"]
								);
				}
				else
				{
					filler = "";
				}

				// replaces template placeholder with actual points
				userBox = rx.Replace(userBox, filler);


				// Location
				rx = new Regex(Constants.UserBox.Location);

				if ( UserProfile.Location != string.Empty )
				{
					filler = String.Format(
								PageContext.BoardSettings.UserBoxLocation,
								PageContext.Localization.GetText("location"),
								FormatMsg.RepairHtml(UserProfile.Location, false)
								);
				}
				else
				{
					filler = "";
				}

				// replaces template placeholder with actual location
				userBox = rx.Replace(userBox, filler);
			}
			else
			{
				filler = "";
				rx = new Regex(Constants.UserBox.JoinDate);
				userBox = rx.Replace(userBox, filler);
				rx = new Regex(Constants.UserBox.Posts);
				userBox = rx.Replace(userBox, filler);
				rx = new Regex(Constants.UserBox.Points);
				userBox = rx.Replace(userBox, filler);
				rx = new Regex(Constants.UserBox.Location);
				userBox = rx.Replace(userBox, filler);
			}

			#region Cache Saving

			// is this control contained in YAF.Classes.Base.ForumPage and has it any cache?
			if (ParentPage != null && ParentPage.PageCache != null)
			{
				// get cache for user boxes
				object cache = ParentPage.PageCache[Constants.Cache.UserBoxes];

				// is it hashtable?
				if (cache != null && cache is Hashtable)
				{
					// save userbox for user of this id to cache
					((Hashtable)cache)[DataRow["UserID"]] = userBox;
				}
				else
				{
					// create new hashtable for userbox caching
					cache = new Hashtable();

					// save userbox of this user
					((Hashtable)cache)[DataRow["UserID"]] = userBox;

					// save cache
					ParentPage.PageCache[Constants.Cache.UserBoxes] = cache;
				}
			}

			#endregion

			return userBox;
		}

		private void PopMenu1_ItemClick( object sender, PopEventArgs e )
		{
			switch ( e.Item )
			{
				case "userprofile":
					YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.profile, "u={0}", DataRow ["UserID"] );
					break;
				case "edituser":
					YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_edituser, "u={0}", DataRow ["UserID"] );
					break;
			}
		}

		// <Summary> Command Button - Report post as Abusive/Spam </Summary>
		protected void Report_Command( object sender, CommandEventArgs e )
		{
			int ReportFlag = 0;
			switch ( e.CommandName )
			{
				case "ReportAbuse":
					ReportFlag = 7;
					break;
				case "ReportSpam":
					ReportFlag = 8;
					break;
			}
			YAF.Classes.Data.DB.message_report( ReportFlag, e.CommandArgument.ToString(), PageContext.PageUserID, DateTime.Today );
			PageContext.AddLoadMessage( PageContext.Localization.GetText( "REPORTEDFEEDBACK" ) );
		}
	}
}
