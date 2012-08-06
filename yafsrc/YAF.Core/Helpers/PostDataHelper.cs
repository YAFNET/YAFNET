/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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

using System.Web;
using YAF.Utils.Helpers;

namespace YAF.Core
{
  using System;
  using System.Data;

  using YAF.Types.Extensions;
  using YAF.Utils;
  using YAF.Types.Flags;

  /// <summary>
  /// The post data helper wrapper.
  /// </summary>
  public class PostDataHelperWrapper
  {
    /// <summary>
    /// The _forum flags.
    /// </summary>
    private ForumFlags _forumFlags;

    /// <summary>
    /// The _message flags.
    /// </summary>
    private MessageFlags _messageFlags;

    /// <summary>
    /// The current data row for this post.
    /// </summary>
    private DataRow _row;

    /// <summary>
    /// The _topic flags.
    /// </summary>
    private TopicFlags _topicFlags;

    /// <summary>
    /// The _user profile.
    /// </summary>
    private YafUserProfile _userProfile = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostDataHelperWrapper"/> class.
    /// </summary>
    public PostDataHelperWrapper()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostDataHelperWrapper"/> class.
    /// </summary>
    /// <param name="dataRow">
    /// The data row.
    /// </param>
    public PostDataHelperWrapper(DataRow dataRow)
      : this()
    {
      DataRow = dataRow;
    }

    /// <summary>
    /// Gets or sets DataRow.
    /// </summary>
    public DataRow DataRow
    {
      get
      {
        return this._row;
      }

      set
      {
        this._row = value;

        // get all flags for forum, topic and message
        if (this._row != null)
        {
          this._forumFlags = new ForumFlags(this._row["ForumFlags"]);
          this._topicFlags = new TopicFlags(this._row["TopicFlags"]);
          this._messageFlags = new MessageFlags(this._row["Flags"]);
        }
        else
        {
          this._forumFlags = new ForumFlags(0);
          this._topicFlags = new TopicFlags(0);
          this._messageFlags = new MessageFlags(0);
        }
      }
    }


    /// <summary>
    /// Gets UserProfile.
    /// </summary>
    public YafUserProfile UserProfile
    {
      get
      {
        if (this._userProfile == null)
        {
          // setup instance of the user profile...
          if (DataRow != null)
          {
            this._userProfile = YafUserProfile.GetProfile(UserMembershipHelper.GetUserNameFromID(UserId));
          }
        }

        return this._userProfile;
      }
    }

    /// <summary>
    /// Gets UserId.
    /// </summary>
    public int UserId
    {
      get
      {
        if (DataRow != null)
        {
          return Convert.ToInt32(DataRow["UserID"]);
        }

        return 0;
      }
    }

    /// <summary>
    /// Gets MessageId.
    /// </summary>
    public int MessageId
    {
      get
      {
        if (DataRow != null)
        {
          return Convert.ToInt32(DataRow["MessageID"]);
        }

        return 0;
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
        if (this._messageFlags != null)
        {
          return this._messageFlags.IsLocked;
        }

        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsSponserMessage.
    /// </summary>
    public bool IsSponserMessage
    {
      get
      {
        return DataRow["IP"].ToString() == "none";
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanThankPost.
    /// </summary>
    public bool CanThankPost
    {
      get
      {
        return (int) DataRow["UserID"] != YafContext.Current.PageUserID;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanEditPost.
    /// </summary>
    public bool CanEditPost
    {
      get
      {
        // Ederon : 9/9/2007 - moderaotrs can edit locked posts
        // Ederon : 12/5/2007 - new flags implementation
          return ((!PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked && (((UserId == YafContext.Current.PageUserID) && !DataRow["IsGuest"].ToType<bool>()) || (DataRow["IsGuest"].ToType<bool>() && (DataRow["IP"].ToString() == YafContext.Current.CurrentForumPage.Request.GetUserRealIPAddress())))) ||
                YafContext.Current.ForumModeratorAccess) && YafContext.Current.ForumEditAccess;
         
      }
    }

    /// <summary>
    /// Gets a value indicating whether PostLocked.
    /// </summary>
    public bool PostLocked
    {
      get
      {
        // post is explicitly locked
        if (this._messageFlags.IsLocked)
        {
          return true;
        }

        // there is auto-lock period defined
        if (!YafContext.Current.IsAdmin && YafContext.Current.BoardSettings.LockPosts > 0)
        {
          var edited = (DateTime) DataRow["Edited"];

          // check if post is locked according to this rule
          if (edited.AddDays(YafContext.Current.BoardSettings.LockPosts) < DateTime.UtcNow)
          {
            return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether PostDeleted.
    /// </summary>
    public bool PostDeleted
    {
      get
      {
        return this._messageFlags.IsDeleted;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanAttach.
    /// </summary>
    public bool CanAttach
    {
      get
      {
        // Ederon : 9/9/2007 - moderaotrs can attack to locked posts
        return ((!PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked && UserId == YafContext.Current.PageUserID) ||
                YafContext.Current.ForumModeratorAccess) && YafContext.Current.ForumUploadAccess;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanDeletePost.
    /// </summary>
    public bool CanDeletePost
    {
      get
      {
        // Ederon : 9/9/2007 - moderators can delete in locked posts
        // vzrus : only guests with the same IP can delete guest posts 
          return ((!PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked && (((UserId == YafContext.Current.PageUserID) && !DataRow["IsGuest"].ToType<bool>()) || (DataRow["IsGuest"].ToType<bool>() && (DataRow["IP"].ToString() == YafContext.Current.CurrentForumPage.Request.GetUserRealIPAddress())))) ||
              YafContext.Current.ForumModeratorAccess) && YafContext.Current.ForumDeleteAccess;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanUnDeletePost.
    /// </summary>
    public bool CanUnDeletePost
    {
      get
      {
        return PostDeleted && CanDeletePost;
      }
    }

    /// <summary>
    /// Gets a value indicating whether CanReply.
    /// </summary>
    public bool CanReply
    {
      get
      {
        // Ederon : 9/9/2007 - moderaotrs can reply in locked posts
        return ((!this._messageFlags.IsLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked) || YafContext.Current.ForumModeratorAccess) &&
               YafContext.Current.ForumReplyAccess;
      }
    }
  }
}