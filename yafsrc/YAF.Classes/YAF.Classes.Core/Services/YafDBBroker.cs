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
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Diagnostics;
  using System.Linq;
  using System.Web;
  using YAF.Classes.Data;
  using YAF.Classes.Extensions;
  using YAF.Classes.Utils;

  /// <summary>
  /// Class used for multi-step DB operations so they can be cached, etc.
  /// </summary>
  public class YafDBBroker
  {
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
    public DataTable StyleTransformDataTable(DataTable dt, params string [] styleColumns)
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
    /// <param name="UserID"></param>
    /// <returns></returns>
    public DataTable UserBuddyList(int UserID)
    {
        string key = YafCache.GetBoardCacheKey(String.Format(Constants.Cache.UserBuddies, UserID));
        DataTable buddyList = YafContext.Current.Cache.GetItem(key, 10, () => DB.buddy_list(UserID));
        return buddyList;
    }

    /// <summary>
    /// The favorite topic list.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public List<int> FavoriteTopicList(int userID)
    {
        string key = YafCache.GetBoardCacheKey(String.Format(Constants.Cache.FavoriteTopicList, userID));

        // stored in the user session...
        var favoriteTopicList = HttpContext.Current.Session[key] as List<int>;

        // was it in the cache?
        if (favoriteTopicList == null)
        {
            // get fresh values
            DataTable favoriteTopicListDt = DB.topic_favorite_list(userID);

            // convert to list...
            favoriteTopicList = favoriteTopicListDt.GetColumnAsList<int>("TopicID");

            // store it in the user session...
            HttpContext.Current.Session.Add(key, favoriteTopicList);
        }

        return favoriteTopicList;
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
      string key = YafCache.GetBoardCacheKey(String.Format(Constants.Cache.UserIgnoreList, userId));

      // stored in the user session...
      var userList = HttpContext.Current.Session[key] as List<int>;

      // was it in the cache?
      if (userList == null)
      {
        // get fresh values
        DataTable userListDt = DB.user_ignoredlist(userId);

        // convert to list...
        userList = userListDt.GetColumnAsList<int>("IgnoredUserID");

        // store it in the user session...
        HttpContext.Current.Session.Add(key, userList);
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
      string key = YafCache.GetBoardCacheKey(String.Format(Constants.Cache.UserMedals, userId));

      // get the medals cached...
      DataTable dt = YafContext.Current.Cache.GetItem(key, 10, () => DB.user_listmedals(userId));

      return dt;
    }

    /// <summary>
    /// The user lazy data.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="styledNicks">
    /// The styled nicks.
    /// </param>
    /// <returns>
    /// </returns>
    public DataRow ActiveUserLazyData(object userID)
    {
        string key = YafCache.GetBoardCacheKey(String.Format(Constants.Cache.ActiveUserLazyData, userID));

        // get a row with user lazy data...
      return YafContext.Current.Cache.GetItem<DataRow>(
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
    /// The get moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    public DataTable GetModerators()
    {
      DataTable moderator = DB.forum_moderators();
      moderator.TableName = YafDBAccess.GetObjectName("Moderator");

      return moderator;
    }

    /// <summary>
    /// The get all moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    public List<Moderator> GetAllModerators()
    {
      // get the cached version of forum moderators if it's valid
      string key = YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators);

      var moderator = YafContext.Current.Cache.GetItem<DataTable>(key, YafContext.Current.BoardSettings.BoardModeratorsCacheTimeout, GetModerators);

      return
        moderator.ToListObject(
          (row) =>
          new Moderator(
            Convert.ToInt64(row["ForumID"]), 
            Convert.ToInt64(row["ModeratorID"]), 
            row["ModeratorName"].ToString(), 
            SqlDataLayerConverter.VerifyBool(row["IsGroup"])));
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
      return GetLatestTopics(numberOfPosts, YafContext.Current.PageUserID);
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
      return GetLatestTopics(numberOfPosts, userId , "Style");
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
      return this.StyleTransformDataTable(
          DB.topic_latest(YafContext.Current.PageBoardID, numberOfPosts, userId, YafContext.Current.BoardSettings.UseStyledNicks, YafContext.Current.BoardSettings.NoCountForumsInActiveDiscussions), styleColumnNames);
    }

    /// <summary>
    /// The get active list.
    /// </summary>
    /// <param name="guests">
    /// The guests.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetActiveList(bool guests)
    {
      return GetActiveList(YafContext.Current.BoardSettings.ActiveListTime, guests);
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
    public DataTable GetActiveList(int activeTime, bool guests)
    {
      return this.StyleTransformDataTable(DB.active_list(YafContext.Current.PageBoardID, guests, activeTime, YafContext.Current.BoardSettings.UseStyledNicks));
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
    public DataSet BoardLayout(object boardID, object userID, object categoryID, object parentID)
    {
      if (categoryID != null && long.Parse(categoryID.ToString()) == 0)
      {
        categoryID = null;
      }

      using (var ds = new DataSet())
      {
        // get the cached version of forum moderators if it's valid
        string key = YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators);

        var moderator = YafContext.Current.Cache.GetItem<DataTable>(key, YafContext.Current.BoardSettings.BoardModeratorsCacheTimeout, GetModerators);

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

        if (categoryID != null)
        {
          // make sure this only has the category desired in the dataset
          foreach (DataRow row in ds.Tables[YafDBAccess.GetObjectName("Category")].Rows)
          {
            if (Convert.ToInt32(row["CategoryID"]) != Convert.ToInt32(categoryID))
            {
              // delete it...
              row.Delete();
            }
          }

          ds.Tables[YafDBAccess.GetObjectName("Category")].AcceptChanges();
        }

        DataTable forum = DB.forum_listread(boardID, userID, categoryID, parentID);
        forum.TableName = YafDBAccess.GetObjectName("Forum");
        ds.Tables.Add(forum.Copy());

        ds.Relations.Add(
          "FK_Forum_Category", 
          ds.Tables[YafDBAccess.GetObjectName("Category")].Columns["CategoryID"], 
          ds.Tables[YafDBAccess.GetObjectName("Forum")].Columns["CategoryID"], 
          false);
        ds.Relations.Add(
          "FK_Moderator_Forum", 
          ds.Tables[YafDBAccess.GetObjectName("Forum")].Columns["ForumID"], 
          ds.Tables[YafDBAccess.GetObjectName("Moderator")].Columns["ForumID"], 
          false);

        bool deletedCategory = false;

        // remove empty categories...
        foreach (DataRow row in ds.Tables[YafDBAccess.GetObjectName("Category")].Rows)
        {
          DataRow[] childRows = row.GetChildRows("FK_Forum_Category");

          if (childRows.Length == 0)
          {
            // remove this category...
            row.Delete();
            deletedCategory = true;
          }
        }

        if (deletedCategory)
        {
          ds.Tables[YafDBAccess.GetObjectName("Category")].AcceptChanges();
        }

        return ds;
      }
    }

    /// <summary>
    /// Loads the message text into the paged data if "Message" and "MessageID" exists.
    /// </summary>
    /// <param name="dataRows"></param>
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
    /// Adds the Thanks info to a dataTable
    /// </summary>
    /// <param name="dataTable"></param>
    public void AddThanksInfo(DataTable dataTable)
    {
      var messageIds = dataTable.AsEnumerable().Select(x => x.Field<int>("MessageID"));

      // Add nescessary columns for later use in displaypost.ascx (Prevent repetitive 
      // calls to database.)  

      if (!dataTable.Columns.Contains("ThanksInfo"))
      {
          dataTable.Columns.Add("ThanksInfo", System.Type.GetType("System.String"));
      }
      dataTable.Columns.AddRange(
        new[]
          {
            // General Thanks Info
            //new DataColumn("ThanksInfo", Type.GetType("System.String")),
            // How many times has this message been thanked.
            new DataColumn("IsThankedByUser", Type.GetType("System.Boolean")),
            // How many times has the message poster thanked others?   
            new DataColumn("MessageThanksNumber", Type.GetType("System.Int32")),
            // How many times has the message poster been thanked?
            new DataColumn("ThanksFromUserNumber", Type.GetType("System.Int32")),
            // In how many posts has the message poster been thanked? 
            new DataColumn("ThanksToUserNumber", Type.GetType("System.Int32")),
            // In how many posts has the message poster been thanked? 
            new DataColumn("ThanksToUserPostsNumber", Type.GetType("System.Int32"))
          });


      // Initialize the "IsthankedByUser" column.
      dataTable.AsEnumerable().ForEach(x => x["IsThankedByUser"] = false);

      // Initialize the "Thank Info" column.
      dataTable.AsEnumerable().ForEach(x => x["ThanksInfo"] = String.Empty);

      // Iterate through all the thanks relating to this topic and make appropriate
      // changes in columns.
      using (DataTable allThanks = DB.message_GetAllThanks(messageIds.ToDelimitedString(",")))
      {
        EnumerableRowCollection<DataRow> thanksFiltered = from DataRow thankRow in allThanks.AsEnumerable()
                                                          where !thankRow["FromUserID"].IsNullOrEmptyDBField()
                                                          where
                                                            thankRow.Field<int>("FromUserID") ==
                                                            YafContext.Current.PageUserID
                                                          select thankRow;

        foreach (DataRow thanksRow in thanksFiltered)
        {
          DataRow row = thanksRow;

          foreach (var f in
            dataTable.AsEnumerable().Where(x => x.Field<int>("MessageID").Equals(row.Field<int>("MessageID"))))
          {
            f["IsThankedByUser"] = "true";
          }
        }

        var thanksFieldNames = new[] { "ThanksFromUserNumber", "ThanksToUserNumber", "ThanksToUserPostsNumber" };

        foreach (DataRow postRow in dataTable.AsEnumerable())
        {
          var messageId = postRow.Field<int>("MessageID");

          postRow["MessageThanksNumber"] =
            allThanks.AsEnumerable().Count(
              x => x.Field<int>("MessageID").Equals(messageId) && !x["FromUserID"].IsNullOrEmptyDBField());

          thanksFiltered = allThanks.AsEnumerable().Where(x => x.Field<int>("MessageID").Equals(messageId));
          bool hasThanks = thanksFiltered.Any();

          foreach (var f in thanksFieldNames)
          {
            postRow[f] = hasThanks ? thanksFiltered.First()[f] : 0;
          }
          
          // load all all thanks info into a special column...
          postRow["ThanksInfo"] =
            thanksFiltered.Where(x => !x["FromUserID"].IsNullOrEmptyDBField()).Select(
              x => "{0}|{1}".FormatWith(x.Field<int?>("FromUserID"), x.Field<DateTime?>("ThanksDate"))).
              ToDelimitedString(",");

          Debug.WriteLine(postRow["ThanksInfo"]);
        }
      }

      // save all changes
      dataTable.AcceptChanges();
    }
  }
}