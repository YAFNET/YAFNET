/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Web;
  using System.Web.Caching;

  using YAF.Classes.Data;
  using YAF.Classes.Extensions;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  public interface IDBBroker
  {
    /// <summary>
    /// The user lazy data.
    /// </summary>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <returns>
    /// </returns>
    DataRow ActiveUserLazyData(object userID);

    /// <summary>
    /// Adds the Thanks info to a dataTable
    /// </summary>
    /// <param name="dataRows">
    /// </param>
    void AddThanksInfo(IEnumerable<DataRow> dataRows);

    /// <summary>
    /// The get smilies.
    /// </summary>
    /// <returns>
    /// Table with list of smiles
    /// </returns>
    IEnumerable<TypedSmileyList> GetSmilies();

    /// <summary>
    /// Returns the layout of the board
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="userID">
    /// UserID
    /// </param>
    /// <param name="categoryID">
    /// CategoryID
    /// </param>
    /// <param name="parentID">
    /// ParentID
    /// </param>
    /// <returns>
    /// Returns board layout
    /// </returns>
    DataSet BoardLayout(int boardID, int userID, int? categoryID, int? parentID);

    /// <summary>
    /// The favorite topic list.
    /// </summary>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <returns>
    /// </returns>
    List<int> FavoriteTopicList(int userID);

    /// <summary>
    /// The get active list.
    /// </summary>
    /// <param name="guests">
    /// The guests.
    /// </param>
    /// <param name="bots">
    /// The bots.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetActiveList(bool guests, bool crawlers);

    /// <summary>
    /// The get active list.
    /// </summary>
    /// <param name="activeTime">
    /// The active time.
    /// </param>
    /// <param name="guests">
    /// The guests.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetActiveList(int activeTime, bool guests, bool crawlers);

    /// <summary>
    /// The get all moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    List<SimpleModerator> GetAllModerators();

    /// <summary>
    /// Get a simple forum/topic listing.
    /// </summary>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// </returns>
    List<SimpleForum> GetSimpleForumTopic(int boardId, int userId, DateTime timeFrame, int maxCount);

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetLatestTopics(int numberOfPosts);

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetLatestTopics(int numberOfPosts, int userId);

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="styleColumnNames">
    /// The style Column Names.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetLatestTopics(int numberOfPosts, int userId, params string[] styleColumnNames);

    /// <summary>
    /// The get moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    DataTable GetModerators();

    /// <summary>
    /// Loads the message text into the paged data if "Message" and "MessageID" exists.
    /// </summary>
    /// <param name="dataRows">
    /// </param>
    void LoadMessageText(IEnumerable<DataRow> dataRows);

    /// <summary>
    /// The style transform func wrap.
    /// </summary>
    /// <param name="dt">
    /// The DateTable
    /// </param>
    /// <returns>
    /// The style transform wrap.
    /// </returns>
    DataTable StyleTransformDataTable(DataTable dt);

    /// <summary>
    /// The style transform func wrap.
    /// </summary>
    /// <param name="dt">
    /// The DateTable
    /// </param>
    /// <param name="styleColumns">
    /// Style columns names
    /// </param>
    /// <returns>
    /// The style transform wrap.
    /// </returns>
    DataTable StyleTransformDataTable(DataTable dt, params string[] styleColumns);

    /// <summary>
    /// The Buddy list for the user with the specified UserID.
    /// </summary>
    /// <param name="UserID">
    /// </param>
    /// <returns>
    /// </returns>
    DataTable UserBuddyList(int UserID);

    /// <summary>
    /// The user ignored list.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    List<int> UserIgnoredList(int userId);

    /// <summary>
    /// The user medals.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable UserMedals(int userId);
  }

  /// <summary>
  /// Class used for multi-step DB operations so they can be cached, etc.
  /// </summary>
  public class YafDBBroker : IDBBroker
  {
    #region Public Methods

    /// <summary>
    /// The user lazy data.
    /// </summary>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <returns>
    /// </returns>
    public DataRow ActiveUserLazyData(object userID)
    {
      string key = YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData.FormatWith(userID));

      // get a row with user lazy data...
      return YafContext.Current.Cache.GetItem(
        key, 
        YafContext.Current.BoardSettings.ActiveUserLazyDataCacheTimeout, 
        () =>
        DB.user_lazydata(
          userID, 
          YafContext.Current.PageBoardID, 
          YafContext.Current.BoardSettings.AllowEmailSending, 
          YafContext.Current.BoardSettings.EnableBuddyList, 
          YafContext.Current.BoardSettings.AllowPrivateMessages, 
          YafContext.Current.BoardSettings.EnableAlbum, 
          YafContext.Current.BoardSettings.UseStyledNicks));
    }

    /// <summary>
    /// Adds the Thanks info to a dataTable
    /// </summary>
    /// <param name="dataRows">
    /// </param>
    public void AddThanksInfo(IEnumerable<DataRow> dataRows)
    {
      var messageIds = dataRows.Select(x => x.Field<int>("MessageID"));

      // Initialize the "IsthankedByUser" column.
      dataRows.ForEach(x => x["IsThankedByUser"] = false);

      // Initialize the "Thank Info" column.
      dataRows.ForEach(x => x["ThanksInfo"] = String.Empty);

      // Iterate through all the thanks relating to this topic and make appropriate
      // changes in columns.
      var allThanks = DB.MessageGetAllThanks(messageIds.ToDelimitedString(","));

      foreach (var f in
          allThanks.Where(t => t.FromUserID != null && t.FromUserID == YafContext.Current.PageUserID).SelectMany(thanks => dataRows.Where(x => x.Field<int>("MessageID") == thanks.MessageID)))
      {
          f["IsThankedByUser"] = "true";
          f.AcceptChanges();
      }

      var thanksFieldNames = new[] { "ThanksFromUserNumber", "ThanksToUserNumber", "ThanksToUserPostsNumber" };

      foreach (DataRow postRow in dataRows)
      {
        var messageId = postRow.Field<int>("MessageID");

        postRow["MessageThanksNumber"] = allThanks.Where(t => t.FromUserID != null && t.MessageID == messageId).Count();

        var thanksFiltered = allThanks.Where(t => t.MessageID == messageId);

        if (thanksFiltered.Any())
        {
          var thanksItem = thanksFiltered.First();

          postRow["ThanksFromUserNumber"] = thanksItem.ThanksFromUserNumber ?? 0;
          postRow["ThanksToUserNumber"] = thanksItem.ThanksToUserNumber ?? 0;
          postRow["ThanksToUserPostsNumber"] = thanksItem.ThanksToUserPostsNumber ?? 0;
        }
        else
        {
            DataRow row = postRow;
            thanksFieldNames.ForEach(f => row[f] = 0);
        }

          // load all all thanks info into a special column...
        postRow["ThanksInfo"] =
          thanksFiltered.Where(t => t.FromUserID != null).Select(
            x => "{0}|{1}".FormatWith(x.FromUserID.Value, x.ThanksDate)).ToDelimitedString(",");

        postRow.AcceptChanges();
      }
    }

    /// <summary>
    /// The get smilies.
    /// </summary>
    /// <returns>
    /// Table with list of smiles
    /// </returns>
    public IEnumerable<TypedSmileyList> GetSmilies()
    {
      string cacheKey = YafCache.GetBoardCacheKey(Constants.Cache.Smilies);

      return YafContext.Current.Cache.GetItem(
        cacheKey, 60, () => DB.SmileyList(YafContext.Current.PageBoardID, null));
    }

    /// <summary>
    /// Returns the layout of the board
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="userID">
    /// UserID
    /// </param>
    /// <param name="categoryID">
    /// CategoryID
    /// </param>
    /// <param name="parentID">
    /// ParentID
    /// </param>
    /// <returns>
    /// Returns board layout
    /// </returns>
    public DataSet BoardLayout(int boardID, int userID, int? categoryID, int? parentID)
    {
      if (categoryID.HasValue && categoryID == 0)
      {
        categoryID = null;
      }

      using (var ds = new DataSet())
      {
        // get the cached version of forum moderators if it's valid
        string key = YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators);

        var moderator = YafContext.Current.Cache.GetItem<DataTable>(
          key, YafContext.Current.BoardSettings.BoardModeratorsCacheTimeout, this.GetModerators);

        // insert it into this DataSet
        ds.Tables.Add(moderator.Copy());

        // get the Category Table
        key = YafCache.GetBoardCacheKey(Constants.Cache.ForumCategory);
        DataTable category = YafContext.Current.Cache.GetItem(
          key, 
          YafContext.Current.BoardSettings.BoardCategoriesCacheTimeout, 
          () =>
            {
              var catDt = DB.category_list(boardID, null);
              catDt.TableName = YafDBAccess.GetObjectName("Category");
              return catDt;
            });

        // add it to this dataset				
        ds.Tables.Add(category.Copy());

        DataTable categoryTable = ds.Tables[YafDBAccess.GetObjectName("Category")];

        if (categoryID.HasValue)
        {
          // make sure this only has the category desired in the dataset
          foreach (DataRow row in
            categoryTable.AsEnumerable().Where(row => row.Field<int>("CategoryID") != categoryID))
          {
            // delete it...
            row.Delete();
          }

          categoryTable.AcceptChanges();
        }

        DataTable forum = DB.forum_listread(boardID, userID, categoryID, parentID, YafContext.Current.BoardSettings.UseStyledNicks);
        forum.TableName = YafDBAccess.GetObjectName("Forum");
        ds.Tables.Add(forum.Copy());

        ds.Relations.Add(
          "FK_Forum_Category", 
          categoryTable.Columns["CategoryID"], 
          ds.Tables[YafDBAccess.GetObjectName("Forum")].Columns["CategoryID"], 
          false);
        ds.Relations.Add(
          "FK_Moderator_Forum", 
          ds.Tables[YafDBAccess.GetObjectName("Forum")].Columns["ForumID"], 
          ds.Tables[YafDBAccess.GetObjectName("Moderator")].Columns["ForumID"], 
          false);

        bool deletedCategory = false;

        // remove empty categories...
        foreach (DataRow row in
          categoryTable.SelectTypedList(
            row => new { row, childRows = row.GetChildRows("FK_Forum_Category") }).Where(@t => !@t.childRows.Any())
            .Select(@t => @t.row))
        {
          // remove this category...
          row.Delete();
          deletedCategory = true;
        }

        if (deletedCategory)
        {
          categoryTable.AcceptChanges();
        }

        return ds;
      }
    }

    /// <summary>
    /// The favorite topic list.
    /// </summary>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <returns>
    /// </returns>
    public List<int> FavoriteTopicList(int userID)
    {
      string key = YafCache.GetBoardCacheKey(Constants.Cache.FavoriteTopicList.FormatWith(userID));

      // stored in the user session...
      var favoriteTopicList = YafContext.Current.Get<HttpSessionStateBase>()[key] as List<int>;

      // was it in the cache?
      if (favoriteTopicList == null)
      {
        // get fresh values
        DataTable favoriteTopicListDt = DB.topic_favorite_list(userID);

        // convert to list...
        favoriteTopicList = favoriteTopicListDt.GetColumnAsList<int>("TopicID");

        // store it in the user session...
        YafContext.Current.Get<HttpSessionStateBase>().Add(key, favoriteTopicList);
      }

      return favoriteTopicList;
    }


    /// <summary>
    /// The get active list.
    /// </summary>
    /// <param name="guests">
    /// The guests.
    /// </param>
    /// <param name="bots">
    /// The bots.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetActiveList(bool guests, bool crawlers)
    {
        return this.GetActiveList(YafContext.Current.BoardSettings.ActiveListTime, guests, crawlers);
    }

    /// <summary>
    /// The get active list.
    /// </summary>
    /// <param name="activeTime">
    /// The active time.
    /// </param>
    /// <param name="guests">
    /// The guests.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetActiveList(int activeTime, bool guests, bool crawlers)
    {
      return
        this.StyleTransformDataTable(
          DB.active_list(
            YafContext.Current.PageBoardID, guests, crawlers, activeTime, YafContext.Current.BoardSettings.UseStyledNicks));
    }

    /// <summary>
    /// The get all moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    public List<SimpleModerator> GetAllModerators()
    {
      // get the cached version of forum moderators if it's valid
      string key = YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators);

      var moderator = YafContext.Current.Cache.GetItem<DataTable>(
        key, YafContext.Current.BoardSettings.BoardModeratorsCacheTimeout, this.GetModerators);
      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
          new StyleTransform(YafContext.Current.Theme).DecodeStyleByTable(ref moderator, false);
      }
      return
        moderator.SelectTypedList(
          row =>
          new SimpleModerator(
            row.Field<int>("ForumID"),
            row.Field<int>("ModeratorID"),
            row.Field<string>("ModeratorName"),
            row.Field<string>("Style"),
            SqlDataLayerConverter.VerifyBool(row["IsGroup"]))).ToList();
    }

    /// <summary>
    /// Get a simple forum/topic listing.
    /// </summary>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// </returns>
    public List<SimpleForum> GetSimpleForumTopic(int boardId, int userId, DateTime timeFrame, int maxCount)
    {
      var forumData =
        DB.forum_listall(boardId, userId).SelectTypedList(x => new SimpleForum() { ForumID = x.Field<int>("ForumID"), Name = x.Field<string>("Forum") }).ToList();

      // get topics for all forums...
      foreach (var forum in forumData)
      {
        // add topics
        var topics =
          DB.topic_list(forum.ForumID, userId, -1, timeFrame, 0, maxCount, false, false).SelectTypedList(
            x => this.LoadSimpleTopic(x, forum)).Where(x => x.LastPostDate >= timeFrame).ToList();

        forum.Topics = topics;
      }

      return forumData;
    }

    /// <summary>
    /// </summary>
    /// Creates a Simple Topic item.
    /// <param name="row">
    /// The row.
    /// </param>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private SimpleTopic LoadSimpleTopic([NotNull] DataRow row, [NotNull] SimpleForum forum)
    {
      CodeContracts.ArgumentNotNull(row, "row");
      CodeContracts.ArgumentNotNull(forum, "forum");

      return new SimpleTopic()
        {
          TopicID = row.Field<int>("TopicID"),
          CreatedDate = row.Field<DateTime>("Posted"),
          Subject = row.Field<string>("Subject"),
          StartedUserID = row.Field<int>("UserID"),
          StartedUserName = UserMembershipHelper.GetDisplayNameFromID(row.Field<int>("UserID")),
          Replies = row.Field<int>("Replies"),
          LastPostDate = row.Field<DateTime>("LastPosted"),
          LastUserID = row.Field<int>("LastUserID"),
          LastUserName = UserMembershipHelper.GetDisplayNameFromID(row.Field<int>("LastUserID")),
          LastMessageID = row.Field<int>("LastMessageID"),
          FirstMessage = row.Field<string>("FirstMessage"),
          LastMessage = DB.MessageList(row.Field<int>("LastMessageID")).First().Message,
          Forum = forum
        };
    }

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetLatestTopics(int numberOfPosts)
    {
      return this.GetLatestTopics(numberOfPosts, YafContext.Current.PageUserID);
    }

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetLatestTopics(int numberOfPosts, int userId)
    {
      return this.GetLatestTopics(numberOfPosts, userId, "Style");
    }

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="styleColumnNames">
    /// The style Column Names.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetLatestTopics(int numberOfPosts, int userId, params string[] styleColumnNames)
    {
      return
        this.StyleTransformDataTable(
          DB.topic_latest(
            YafContext.Current.PageBoardID, 
            numberOfPosts, 
            userId, 
            YafContext.Current.BoardSettings.UseStyledNicks, 
            YafContext.Current.BoardSettings.NoCountForumsInActiveDiscussions), 
          styleColumnNames);
    }

    /// <summary>
    /// The get moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    public DataTable GetModerators()
    {
        DataTable moderator = DB.forum_moderators(YafContext.Current.BoardSettings.UseStyledNicks);
      moderator.TableName = YafDBAccess.GetObjectName("Moderator");

      return moderator;
    }

    /// <summary>
    /// Loads the message text into the paged data if "Message" and "MessageID" exists.
    /// </summary>
    /// <param name="dataRows">
    /// </param>
    public void LoadMessageText(IEnumerable<DataRow> dataRows)
    {
      var messageIds =
        dataRows.AsEnumerable().Where(x => x.Field<string>("Message").IsNotSet()).Select(x => x.Field<int>("MessageID"));

      var messageTextTable = DB.message_GetTextByIds(messageIds.ToDelimitedString(","));

      if (messageTextTable == null)
      {
        return;
      }

      // load them into the page data...
      foreach (var dataRow in dataRows)
      {
        // find the message id in the results...
        var message =
          messageTextTable.AsEnumerable().Where(x => x.Field<int>("MessageID") == dataRow.Field<int>("MessageID")).
            FirstOrDefault();

        if (message != null)
        {
          dataRow.BeginEdit();
          dataRow["Message"] = message.Field<string>("Message");
          dataRow.EndEdit();
        }
      }
    }

    /// <summary>
    /// The style transform func wrap.
    /// </summary>
    /// <param name="dt">
    /// The DateTable
    /// </param>
    /// <returns>
    /// The style transform wrap.
    /// </returns>
    public DataTable StyleTransformDataTable(DataTable dt)
    {
      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        var styleTransform = new StyleTransform(YafContext.Current.Theme);
        styleTransform.DecodeStyleByTable(ref dt, true);
      }

      return dt;
    }

    /// <summary>
    /// The style transform func wrap.
    /// </summary>
    /// <param name="dt">
    /// The DateTable
    /// </param>
    /// <param name="styleColumns">
    /// Style columns names
    /// </param>
    /// <returns>
    /// The style transform wrap.
    /// </returns>
    public DataTable StyleTransformDataTable(DataTable dt, params string[] styleColumns)
    {
      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        var styleTransform = new StyleTransform(YafContext.Current.Theme);
        styleTransform.DecodeStyleByTable(ref dt, true, styleColumns);
      }

      return dt;
    }

    /// <summary>
    /// The Buddy list for the user with the specified UserID.
    /// </summary>
    /// <param name="UserID">
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable UserBuddyList(int UserID)
    {
      string key = YafCache.GetBoardCacheKey(Constants.Cache.UserBuddies.FormatWith(UserID));
      DataTable buddyList = YafContext.Current.Cache.GetItem(key, 10, () => DB.buddy_list(UserID));
      return buddyList;
    }

    /// <summary>
    /// The user ignored list.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public List<int> UserIgnoredList(int userId)
    {
      string key = YafCache.GetBoardCacheKey(Constants.Cache.UserIgnoreList.FormatWith(userId));

      // stored in the user session...
      var userList = YafContext.Current.Get<HttpSessionStateBase>()[key] as List<int>;

      // was it in the cache?
      if (userList == null)
      {
        // get fresh values
        DataTable userListDt = DB.user_ignoredlist(userId);

        // convert to list...
        userList = userListDt.GetColumnAsList<int>("IgnoredUserID");

        // store it in the user session...
        YafContext.Current.Get<HttpSessionStateBase>().Add(key, userList);
      }

      return userList;
    }

    /// <summary>
    /// The user medals.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable UserMedals(int userId)
    {
      string key = YafCache.GetBoardCacheKey(Constants.Cache.UserMedals.FormatWith(userId));

      // get the medals cached...
      DataTable dt = YafContext.Current.Cache.GetItem(key, 10, () => DB.user_listmedals(userId));

      return dt;
    }

    #endregion
  }
}