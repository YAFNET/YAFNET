/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Linq;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Controls;
using YAF.Editors;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for postmessage.
	/// </summary>
	public partial class postmessage : YAF.Classes.Core.ForumPage
	{
		protected BaseForumEditor _forumEditor;
		protected System.Web.UI.WebControls.Label _uxNoEditSubject;
		protected int _ownerUserId;

		public postmessage()
			: base("POSTMESSAGE")
		{

		}

		override protected void OnInit(System.EventArgs e)
		{
			// get the forum editor based on the settings
			_forumEditor = PageContext.EditorModuleManager.GetEditorInstance( PageContext.BoardSettings.ForumEditor );
			EditorLine.Controls.Add(_forumEditor);

			base.OnInit(e);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			PageContext.QueryIDs = new QueryStringIDHelper( new string [] {"m", "t", "q"}, false );

			DataRow currentRow = null;

			if (QuotedTopicID != null)
			{
				DataTable dt = DB.message_list( QuotedTopicID );

				if ( dt.Rows.Count == 0 )
				{
					// we're done here...
					YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
					return;
				}

				currentRow = dt.Rows[0];

				if (Convert.ToInt32(currentRow["TopicID"]) != PageContext.PageTopicID)
					YafBuildLink.AccessDenied();

				if (!CanQuotePostCheck(currentRow))
					YafBuildLink.AccessDenied();
			}
			else if (EditTopicID != null)
			{
				DataTable dt = DB.message_list( EditTopicID );

				if ( dt.Rows.Count == 0 )
				{
					// we're done here...
					YafBuildLink.RedirectInfoPage( InfoMessage.Invalid );
					return;
				}
				
				currentRow = dt.Rows[0];

				_ownerUserId = Convert.ToInt32(currentRow["UserId"]);

				if (!CanEditPostCheck(currentRow))
					YafBuildLink.AccessDenied();
			}

			if (PageContext.PageForumID == 0)
				YafBuildLink.AccessDenied();
			if (Request["t"] == null && Request["m"] == null && !PageContext.ForumPostAccess)
				YafBuildLink.AccessDenied();
			if (Request["t"] != null && !PageContext.ForumReplyAccess)
				YafBuildLink.AccessDenied();

			//Message.EnableRTE = PageContext.BoardSettings.AllowRichEdit;
			_forumEditor.StyleSheet = PageContext.Theme.BuildThemePath("theme.css");
			_forumEditor.BaseDir = YafForumInfo.ForumRoot + "editors";

			Title.Text = GetText("NEWTOPIC");
			PollExpire.Attributes.Add("style", "width:50px");

			if (!IsPostBack)
			{
				// helper bool -- true if this is a completely new topic...
				bool isNewTopic = ( TopicID == null ) && ( QuotedTopicID == null ) && ( EditTopicID == null );

				Priority.Items.Add(new ListItem(GetText("normal"), "0"));
				Priority.Items.Add(new ListItem(GetText("sticky"), "1"));
				Priority.Items.Add(new ListItem(GetText("announcement"), "2"));
				Priority.SelectedIndex = 0;

				EditReasonRow.Visible = false;

				PersistencyRow.Visible = PageContext.ForumPriorityAccess;
				PriorityRow.Visible = PageContext.ForumPriorityAccess;
				CreatePollRow.Visible = !HasPoll(currentRow) && CanHavePoll(currentRow) && PageContext.ForumPollAccess;
				RemovePollRow.Visible = HasPoll(currentRow) && CanHavePoll(currentRow) && PageContext.ForumPollAccess && PageContext.ForumModeratorAccess;

				if (RemovePollRow.Visible)
				{
					InitPollUI( currentRow );
				}

				// Show post to blog option only to a new post
				BlogRow.Visible = (PageContext.BoardSettings.AllowPostToBlog && isNewTopic && !PageContext.IsGuest);

				// handle new topic options...
				NewTopicOptionsRow.Visible = isNewTopic && !PageContext.IsGuest;
				if ( isNewTopic && PageContext.ForumUploadAccess )
				{
					TopicAttach.Visible = true;
					TopicAttachLabel.Visible = true;
				}

				if ( ( PageContext.IsGuest && PageContext.BoardSettings.EnableCaptchaForGuests ) || 
					(PageContext.BoardSettings.EnableCaptchaForPost && !PageContext.IsCaptchaExcluded) )
				{
					Session ["CaptchaImageText"] = CaptchaHelper.GetCaptchaString();
					imgCaptcha.ImageUrl = String.Format( "{0}resource.ashx?c=1", YafForumInfo.ForumRoot );
					tr_captcha1.Visible = true;
					tr_captcha2.Visible = true;
				}

				if (PageContext.Settings.LockedForum == 0)
				{
					PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
					PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));					
				}
				PageLinks.AddForumLinks(PageContext.PageForumID);

				// check if it's a reply to a topic...
				if ( TopicID != null )
				{
					InitReplyToTopic();
				}

				// If currentRow != null, we are quoting a post in a new reply, or editing an existing post
				if (currentRow != null)
				{
					MessageFlags messageFlags = new MessageFlags(currentRow["Flags"]);
					string message = currentRow["Message"].ToString();

					if (QuotedTopicID != null)
					{
						// quoting a reply to a topic...
						InitQuotedReply( currentRow, message, messageFlags );
					}
					else if (EditTopicID != null)
					{
						// editing a message...
						InitEditedPost( currentRow, message, messageFlags );
					}
				}

				// add the "New Topic" page link last...
				if ( isNewTopic )
				{
					PageLinks.AddLink( GetText( "NEWTOPIC" ) );
				}

				// form user is only for "Guest"
				From.Text = PageContext.PageUserName;
				if (User != null)
					FromRow.Visible = false;
			}
		}

		private void InitPollUI( DataRow currentRow )
		{
			RemovePoll.CommandArgument = currentRow["PollID"].ToString();

			if (currentRow["PollID"] != DBNull.Value)
			{
				DataTable choices = YAF.Classes.Data.DB.poll_stats(currentRow["PollID"]);

				Question.Text = choices.Rows[0]["Question"].ToString();
				if (choices.Rows[0]["Closes"] != DBNull.Value)
				{
					TimeSpan closing = (DateTime)choices.Rows[0]["Closes"] - DateTime.Now;

					PollExpire.Text = (SqlDataLayerConverter.VerifyInt32((closing.TotalDays + 1)).ToString());
				}
				else
				{
					PollExpire.Text = null;
				}

				for (int i = 0; i < choices.Rows.Count; i++)
				{
					HiddenField idField = (HiddenField)this.FindControl(String.Format("PollChoice{0}ID", i + 1));
					TextBox choiceField = (TextBox)this.FindControl(String.Format("PollChoice{0}", i + 1));

					idField.Value = choices.Rows[i]["ChoiceID"].ToString();
					choiceField.Text = choices.Rows[i]["Choice"].ToString();
				}

				ChangePollShowStatus( true );
			}
		}

		private void InitQuotedReply( DataRow currentRow, string message, MessageFlags messageFlags )
		{
			if (PageContext.BoardSettings.RemoveNestedQuotes)
				message = FormatMsg.RemoveNestedQuotes(message);

			// If the message being quoted in YafBBCode but the editor uses HTML, convert the message text to HTML
			if (messageFlags.IsBBCode && _forumEditor.UsesHTML)
				message = YafBBCode.ConvertBBCodeToHtmlForEdit(message);

			// Ensure quoted replies have bad words removed from them
			message = YafServices.BadWordReplace.Replace(message);

			// Quote the original message
			_forumEditor.Text = String.Format("[quote={0}]{1}[/quote]\n", currentRow["username"], message).TrimStart();
		}

		private void InitEditedPost( DataRow currentRow, string message, MessageFlags messageFlags )
		{
			// If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
			if (messageFlags.IsBBCode && _forumEditor.UsesHTML)
				message = YafBBCode.ConvertBBCodeToHtmlForEdit(message);
						
			_forumEditor.Text = message;

			Title.Text = GetText("EDIT");

			// add topic link...
			PageLinks.AddLink(Server.HtmlDecode(currentRow["Topic"].ToString()),
			                  YafBuildLink.GetLink(ForumPages.posts, "m={0}", EditTopicID));
			// editing..
			PageLinks.AddLink(GetText("EDIT"));

			string blogPostID = currentRow["BlogPostID"].ToString();
			if (blogPostID != string.Empty) // The user used this post to blog
			{
				BlogPostID.Value = blogPostID;
				PostToBlog.Checked = true;
				BlogRow.Visible = true;
			}

			Subject.Text = Server.HtmlDecode(Convert.ToString(currentRow["Topic"]));

			if ((Convert.ToInt32(currentRow["TopicOwnerID"]) == Convert.ToInt32(currentRow["UserID"])) ||
			    PageContext.ForumModeratorAccess)
			{
				// allow editing of the topic subject
				Subject.Enabled = true;
			}
			else
			{
				// disable the subject
				Subject.Enabled = false;
			}

			Priority.SelectedItem.Selected = false;
			Priority.Items.FindByValue(currentRow["Priority"].ToString()).Selected = true;
			EditReasonRow.Visible = true;
			ReasonEditor.Text = Server.HtmlDecode(Convert.ToString(currentRow["EditReason"]));
			Persistency.Checked = messageFlags.IsPersistent;
		}

		private void InitReplyToTopic()
		{
			DataRow topic = DB.topic_info( TopicID );
			TopicFlags topicFlags = new TopicFlags(SqlDataLayerConverter.VerifyInt32(topic["Flags"]));

			// Ederon : 9/9/2007 - moderators can reply in locked topics
			if (topicFlags.IsLocked && !PageContext.ForumModeratorAccess)
				Response.Redirect( Request.UrlReferrer.ToString() );
			SubjectRow.Visible = false;
			Title.Text = GetText( "reply" );

			// add topic link...
			PageLinks.AddLink( Server.HtmlDecode( topic ["Topic"].ToString() ), YafBuildLink.GetLink( ForumPages.posts, "t={0}", TopicID ) );
			// add "reply" text...
			PageLinks.AddLink( GetText( "reply" ) );

			// show attach file option if its a reply...
			if ( PageContext.ForumUploadAccess )
			{
				NewTopicOptionsRow.Visible = true;
				TopicAttach.Visible = true;
				TopicAttachLabel.Visible = true;
				TopicWatch.Visible = false;
				TopicWatchLabel.Visible = false;
				TopicAttachBr.Visible = false;
			}

			// show the last posts AJAX frame...
			LastPosts1.Visible = true;
			LastPosts1.TopicID = TopicID.Value;
		}

		private bool CanEditPostCheck(DataRow message)
		{
			bool postLocked = false;

			if (!PageContext.IsAdmin && PageContext.BoardSettings.LockPosts > 0)
			{
				DateTime edited = (DateTime)message["Edited"];

				if (edited.AddDays(PageContext.BoardSettings.LockPosts) < DateTime.Now)
					postLocked = true;
			}

			DataRow forumInfo, topicInfo;

			// get topic and forum information
			topicInfo = DB.topic_info(PageContext.PageTopicID);
			using (DataTable dt = DB.forum_list(PageContext.PageBoardID, PageContext.PageForumID))
				forumInfo = dt.Rows[0];

			// Ederon : 9/9/2007 - moderator can edit in locked topics
			return ((!postLocked &&
				!General.BinaryAnd(forumInfo["Flags"], ForumFlags.Flags.IsLocked) &&
				!General.BinaryAnd(topicInfo["Flags"], TopicFlags.Flags.IsLocked) &&
				(SqlDataLayerConverter.VerifyInt32(message["UserID"])== PageContext.PageUserID)) || PageContext.ForumModeratorAccess) &&
				PageContext.ForumEditAccess;
		}

		private bool CanQuotePostCheck(DataRow message)
		{
			DataRow forumInfo, topicInfo;

			// get topic and forum information
			topicInfo = DB.topic_info(PageContext.PageTopicID);
			using (DataTable dt = DB.forum_list(PageContext.PageBoardID, PageContext.PageForumID))
				forumInfo = dt.Rows[0];

			if ( topicInfo == null || forumInfo == null ) return false;

			// Ederon : 9/9/2007 - moderator can reply to locked topics
			return (!General.BinaryAnd(forumInfo["Flags"], ForumFlags.Flags.IsLocked) &&
				!General.BinaryAnd(topicInfo["Flags"], TopicFlags.Flags.IsLocked) || PageContext.ForumModeratorAccess) &&
				PageContext.ForumReplyAccess;
		}


		private bool HasPoll(DataRow message)
		{
			return message != null && message["PollID"] != DBNull.Value && message["PollID"] != null;
		}


		private bool CanHavePoll(DataRow message)
		{
			return (TopicID == null && QuotedTopicID == null && EditTopicID == null) ||
				(message != null && (SqlDataLayerConverter.VerifyInt32(message["Position"])) == 0);
		}


		/// <summary>
		/// Handles verification of the PostReply. Adds javascript message if there is a problem.
		/// </summary>
		/// <returns>true if everything is verified</returns>
		protected bool IsPostReplyVerified()
		{
			if (SubjectRow.Visible && Subject.Text.Length <= 0)
			{
				PageContext.AddLoadMessage(GetText("NEED_SUBJECT"));
				return false;
			}

			if (PollRow1.Visible)
			{
				if (Question.Text.Trim().Length == 0)
				{
					PageContext.AddLoadMessage(GetText("NEED_QUESTION"));
					return false;
				}

				string p1 = PollChoice1.Text.Trim();
				string p2 = PollChoice2.Text.Trim();
				if (p1.Length == 0 || p2.Length == 0)
				{
					PageContext.AddLoadMessage(GetText("NEED_CHOICES"));
					return false;
				}
			}

			if ( (
					( PageContext.IsGuest && PageContext.BoardSettings.EnableCaptchaForGuests ) || 
					(PageContext.BoardSettings.EnableCaptchaForPost && !PageContext.IsCaptchaExcluded)
				  ) && 	Session["CaptchaImageText"].ToString() != tbCaptcha.Text.Trim())
			{
				PageContext.AddLoadMessage( GetText( "BAD_CAPTCHA" ) );
				return false;
			}

			return true;
		}

		/// <summary>
		/// Verifies the user isn't posting too quickly, if so, tells them to wait.
		/// </summary>
		/// <returns>True if there is a delay in effect.</returns>
		protected bool IsPostReplyDelay()
		{
			// see if there is a post delay
			if (!(PageContext.IsAdmin || PageContext.IsModerator) && PageContext.BoardSettings.PostFloodDelay > 0)
			{
				// see if they've past that delay point
				if (Mession.LastPost > DateTime.Now.AddSeconds(-PageContext.BoardSettings.PostFloodDelay) && EditTopicID == null)
				{
					PageContext.AddLoadMessage(GetTextFormatted( "wait", (Mession.LastPost - DateTime.Now.AddSeconds(-PageContext.BoardSettings.PostFloodDelay)).Seconds));
					return true;
				}
			}

			return false;
		}

		protected long PostReplyHandleReplyToTopic()
		{
			long nMessageID = 0;

			if (!PageContext.ForumReplyAccess)
				YafBuildLink.AccessDenied();

			object replyTo = (QuotedTopicID != null) ? QuotedTopicID.Value : -1;

			// make message flags
			MessageFlags tFlags = new MessageFlags();

			tFlags.IsHtml = _forumEditor.UsesHTML;
			tFlags.IsBBCode = _forumEditor.UsesBBCode;
			tFlags.IsPersistent = Persistency.Checked;

			// Bypass Approval if Admin or Moderator.
			tFlags.IsApproved = (PageContext.IsAdmin || PageContext.IsModerator);

			DB.message_save(TopicID.Value, PageContext.PageUserID, _forumEditor.Text, User != null ? null : From.Text, Request.UserHostAddress, null, replyTo, tFlags.BitValue, ref nMessageID);

			return nMessageID;
		}

		protected long PostReplyHandleEditPost()
		{
			long nMessageID = 0;

			if (!PageContext.ForumEditAccess)
				YafBuildLink.AccessDenied();

			string subjectSave = "";

			if (Subject.Enabled) subjectSave = HtmlEncode(Subject.Text);

			// Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
			// Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
			// retrieve message flags
			MessageFlags messageFlags = new MessageFlags(DB.message_list(EditTopicID).Rows[0]["Flags"]);
			messageFlags.IsHtml = _forumEditor.UsesHTML;
			messageFlags.IsBBCode = _forumEditor.UsesBBCode;
			messageFlags.IsPersistent = Persistency.Checked;

			bool isModeratorChanged = (PageContext.PageUserID != _ownerUserId);
			DB.message_update(Request.QueryString["m"], Priority.SelectedValue, _forumEditor.Text, subjectSave, messageFlags.BitValue, HtmlEncode(ReasonEditor.Text), isModeratorChanged, PageContext.IsAdmin || PageContext.IsModerator);

			// update poll
			if ( !string.IsNullOrEmpty( RemovePoll.CommandArgument ) || PollRow1.Visible )
			{
				DB.topic_poll_update( null, Request.QueryString ["m"], GetPollID() );
			}

			nMessageID = EditTopicID.Value;

			HandlePostToBlog(_forumEditor.Text, Subject.Text);

			// remove cache if it exists...
			PageContext.Cache.Remove( string.Format( Constants.Cache.FirstPostCleaned, PageContext.PageBoardID, TopicID ) );

			return nMessageID;
		}

		protected long PostReplyHandleNewPost()
		{
			long nMessageID = 0;

			if (!PageContext.ForumPostAccess)
				YafBuildLink.AccessDenied();

			// make message flags
			MessageFlags tFlags = new MessageFlags();

			tFlags.IsHtml = _forumEditor.UsesHTML;
			tFlags.IsBBCode = _forumEditor.UsesBBCode;
			tFlags.IsPersistent = Persistency.Checked;

			// Bypass Approval if Admin or Moderator.
			tFlags.IsApproved = (PageContext.IsAdmin || PageContext.IsModerator);

			string blogPostID = HandlePostToBlog(_forumEditor.Text, Subject.Text);

			// Save to Db
			long topicID = DB.topic_save(PageContext.PageForumID, HtmlEncode(Subject.Text), _forumEditor.Text, PageContext.PageUserID, Priority.SelectedValue, this.GetPollID(), User != null ? null : From.Text, Request.UserHostAddress, null, blogPostID, tFlags.BitValue, ref nMessageID);

			if ( TopicWatch.Checked )
			{
				// subscribe to this topic...
				DB.watchtopic_add( PageContext.PageUserID, topicID );
			}

			return nMessageID;
		}

		protected string HandlePostToBlog(string message, string subject)
		{
			string blogPostID = string.Empty;

			// Does user wish to post this to their blog?
			if (PageContext.BoardSettings.AllowPostToBlog && PostToBlog.Checked)
			{
				try
				{
					// Post to blog
					YAF.Utilities.MetaWeblog blog = new YAF.Utilities.MetaWeblog( PageContext.Profile.BlogServiceUrl );
					blogPostID = blog.newPost(PageContext.Profile.BlogServicePassword, PageContext.Profile.BlogServiceUsername, BlogPassword.Text, subject, message);
				}
				catch
				{
					PageContext.AddLoadMessage(GetText("POSTTOBLOG_FAILED"));
				}
			}

			return blogPostID;
		}

		/// <summary>
		/// Handles the PostReply click including: Replying, Editing and New post.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void PostReply_Click(object sender, System.EventArgs e)
		{
			if (!IsPostReplyVerified()) return;
			if (IsPostReplyDelay()) return;

			// update the last post time...
			Mession.LastPost = DateTime.Now.AddSeconds(30);

			long nMessageID = 0;

			if (TopicID != null) // Reply to topic
			{
				nMessageID = PostReplyHandleReplyToTopic();
			}
			else if (EditTopicID != null) // Edit existing post
			{
				nMessageID = PostReplyHandleEditPost();
			}
			else // New post
			{
				nMessageID = PostReplyHandleNewPost();
			}

			// Check if message is approved
			bool bApproved = false;
			using (DataTable dt = DB.message_list(nMessageID))
				foreach (DataRow row in dt.Rows)
					bApproved = General.BinaryAnd(row["Flags"], MessageFlags.Flags.IsApproved);

			// Create notification emails
			if (bApproved)
			{
				CreateMail.WatchEmail(nMessageID);

				if ( PageContext.ForumUploadAccess && TopicAttach.Checked )
				{
					// redirect to the attachment page...
					YafBuildLink.Redirect( ForumPages.attachments, "m={0}", nMessageID );
				}
				else
				{
					// regular redirect...
					YafBuildLink.Redirect( ForumPages.posts, "m={0}&#post{0}", nMessageID );
				}
			}
			else
			{
				// Tell user that his message will have to be approved by a moderator
				//PageContext.AddLoadMessage("Since you posted to a moderated forum, a forum moderator must approve your post before it will become visible.");
				string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", PageContext.PageForumID);

				if (YAF.Classes.Config.IsRainbow)
					YafBuildLink.Redirect(ForumPages.info, "i=1");
				else
					YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", Server.UrlEncode(url));
			}
		}

		protected void ChangePollShowStatus( bool newStatus )
		{
			CreatePollRow.Visible = !newStatus;
			RemovePollRow.Visible = newStatus;
			PollRowExpire.Visible = newStatus;

			for ( int i = 1; i < 10; i++ )
			{
				HtmlTableRow pollRow = (HtmlTableRow)this.FindControl( String.Format( "PollRow{0}", i ) );

				if ( pollRow != null )
					pollRow.Visible = newStatus;
			}
		}

		protected void CreatePoll_Click(object sender, System.EventArgs e)
		{
			ChangePollShowStatus( true );

			// clear the fields...
			PollExpire.Text = "";
			Question.Text = "";

			for ( int i = 1; i < 10; i++ )
			{
				TextBox choiceField = (TextBox) this.FindControl( String.Format( "PollChoice{0}", i ) );

				if ( choiceField != null )
					choiceField.Text = "";
			}
		}

		protected void RemovePoll_Command(object sender, CommandEventArgs e)
		{
			ChangePollShowStatus( false );

			if (e.CommandArgument != null && e.CommandArgument.ToString() != "")
			{
				DB.poll_remove(e.CommandArgument);
				((LinkButton)sender).CommandArgument = null;
			}
		}

		protected void RemovePoll_Load(object sender, System.EventArgs e)
		{
			((ThemeButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}');", GetText("ASK_POLL_DELETE"));
		}

		private object GetPollID()
		{
			int daysPollExpire = 0;
			object datePollExpire = null;

			if (int.TryParse(PollExpire.Text.Trim(), out daysPollExpire))
				datePollExpire = DateTime.Now.AddDays(daysPollExpire);

			// we are just using existing poll
			if (!string.IsNullOrEmpty( RemovePoll.CommandArgument ))
			{
				int pollID = Convert.ToInt32(RemovePoll.CommandArgument);
				DB.poll_update(pollID, Question.Text, datePollExpire);

				for (int i = 1; i < 10; i++)
				{
					HiddenField idField = (HiddenField)this.FindControl(String.Format("PollChoice{0}ID", i));
					TextBox choiceField = (TextBox)this.FindControl(String.Format("PollChoice{0}", i));

					if (string.IsNullOrEmpty(idField.Value) && !string.IsNullOrEmpty(choiceField.Text))
					{
						// add choice
						DB.choice_add(pollID, choiceField.Text);
					}
					else if (!string.IsNullOrEmpty(idField.Value) && !string.IsNullOrEmpty(choiceField.Text))
					{
						// update choice
						DB.choice_update(idField.Value, choiceField.Text);
					}
					else if (!string.IsNullOrEmpty(idField.Value) && string.IsNullOrEmpty(choiceField.Text))
					{
						// remove choice
						DB.choice_delete(idField.Value);
					}
				}

				return Convert.ToInt32(RemovePoll.CommandArgument);
			}
			else if (PollRow1.Visible) // User wishes to create a poll
			{
				return DB.poll_save(Question.Text,
					PollChoice1.Text,
					PollChoice2.Text,
					PollChoice3.Text,
					PollChoice4.Text,
					PollChoice5.Text,
					PollChoice6.Text,
					PollChoice7.Text,
					PollChoice8.Text,
					PollChoice9.Text,
					datePollExpire);
			}
			return null; // A poll was not created on this post
		}

		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			if (TopicID != null || EditTopicID != null)
			{
				// reply to existing topic or editing of existing topic
				YafBuildLink.Redirect(ForumPages.posts, "t={0}", PageContext.PageTopicID);
			}
			else
			{
				// new topic -- cancel back to forum
				YafBuildLink.Redirect(ForumPages.topics, "f={0}", PageContext.PageForumID);
			}
		}

		protected void Preview_Click(object sender, System.EventArgs e)
		{
			PreviewRow.Visible = true;

			PreviewMessagePost.MessageFlags.IsHtml = _forumEditor.UsesHTML;
			PreviewMessagePost.MessageFlags.IsBBCode = _forumEditor.UsesBBCode;
			PreviewMessagePost.Message = _forumEditor.Text;

			if ( PageContext.BoardSettings.AllowSignatures )
			{
				using ( DataTable userDt = DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
				{
					if ( !userDt.Rows [0].IsNull( "Signature" ) )
					{
						PreviewMessagePost.Signature = userDt.Rows [0] ["Signature"].ToString();
					}
				}
			}
		}

		#region Querystring Values

		protected long? TopicID
		{
			get { return PageContext.QueryIDs["t"]; }
		}

		protected long? EditTopicID
		{
			get { return PageContext.QueryIDs["m"]; }
		}

		protected long? QuotedTopicID
		{
			get { return PageContext.QueryIDs["q"]; }
		}
		
		#endregion
	}
}
