/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.ServiceModel.Syndication;
  using System.Text;
  using System.Web;
  using System.Xml;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for rss.
  /// </summary>
  public partial class rsstopic : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="rsstopic"/> class.
    /// </summary>
    public rsstopic()
      : base("RSSTOPIC")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // Put user code to initialize the page here 
      
      // Atom feeds are enabled
      bool atomFeed = PageContext.BoardSettings.ShowAtomLink;
     
      // Atom feed as variable
      bool atomFeedByVar = this.Request.QueryString.GetFirstOrDefault("ft") == "atom";
      
      var feed = new YafSyndicationFeed();
      var syndicationItems = new List<SyndicationItem>();
      string lastPostIcon = PageContext.CurrentForumPage.GetThemeContents("ICONS", "ICON_NEWEST");
      string lastPostName = this.PageContext.Localization.GetText("DEFAULT", "GO_LAST_POST"); 
     
      YafRssFeeds feedType = YafRssFeeds.Forum;

      try
      {
        feedType = this.Request.QueryString.GetFirstOrDefault("pg").ToEnum<YafRssFeeds>(true);
      }
      catch
      {
        // default to Forum Feed.
      }
      if (atomFeedByVar)
      {
          atomFeed = true;
      }

      switch (feedType)
      {
        case YafRssFeeds.LatestPosts:
        if (!this.PageContext.BoardSettings.ShowActiveDiscussions)
          {
              YafBuildLink.AccessDenied();
          }

          using (DataTable dataTopics = YafServices.DBBroker.GetLatestTopics(this.PageContext.BoardSettings.ActiveDiscussionsCount <= 50 ? this.PageContext.BoardSettings.ActiveDiscussionsCount : 50, PageContext.PageUserID, "LastUserStyle"))
          {

              feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("ACTIVE_DISCUSSIONS"));
              
              foreach (DataRow row in dataTopics.Rows)
            {
                DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField()
                                          ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset
                                          : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;
                if (syndicationItems.Count <= 0)
                {
                    feed.LastUpdatedTime = lastPosted;
                    feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, (Convert.ToInt64(row["UserID"]))));
                    
                    // Alternate Link for feed
                    feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                }
                
                feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));
               
                string messageLink = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]);

                syndicationItems.AddSyndicationItem(
                    row["Topic"].ToString(),
                    SyndicationItemExtensions.GetContent(messageLink, lastPostIcon, lastPostName, lastPostName),
                    null,
                    YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", Convert.ToInt32(row["TopicID"])),
                    "{0}FeedType{1}TopicID{2}MessageID{3}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, Convert.ToInt32(row["TopicID"]), Convert.ToInt32(row["LastMessageID"])),
                    lastPosted, feed.Contributors[feed.Contributors.Count-1].Name);
            }
          }

          break;
        case YafRssFeeds.LatestAnnouncements:
          if (!this.PageContext.ForumReadAccess)
          {
            YafBuildLink.AccessDenied();
          }

          using (DataTable dt = DB.topic_announcements(this.PageContext.PageBoardID, 10, this.PageContext.PageUserID))
          {

            feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("POSTMESSAGE", "ANNOUNCEMENT"));
          
            foreach (DataRow row in dt.Rows)
            {
                DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField() ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                if (syndicationItems.Count <= 0)
                {
                    feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["UserID"])));
                    feed.LastUpdatedTime = lastPosted;

                    // Alternate Link
                    feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                }

                feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));

             
              syndicationItems.AddSyndicationItem(
                row["Subject"].ToString(), 
                row["Message"].ToString(),
                null,
                YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", this.Request.QueryString.GetFirstOrDefault("t")),
                "{0}FeedType{1}TopicID{2}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, this.Request.QueryString.GetFirstOrDefault("t")),
                lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
            }
          }

          break;
        // Posts Feed
        case YafRssFeeds.Posts:
          if (!this.PageContext.ForumReadAccess)
          {
            YafBuildLink.AccessDenied();
          }

          if (this.Request.QueryString.GetFirstOrDefault("t") != null)
          {
            using (
              DataTable dt = DB.post_list(
                this.PageContext.PageTopicID, 0, this.PageContext.BoardSettings.ShowDeletedMessages, false))
            {
              // get max 500 rows
              var dataRows = dt.AsEnumerable().Take(500);

              // load the missing message test
              YafServices.DBBroker.LoadMessageText(dataRows);
              feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("PROFILE", "TOPIC") + this.PageContext.PageTopicName);
              foreach (var row in dataRows)
              {
                  DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField() ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                  if (syndicationItems.Count <= 0)
                  {
                      feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["UserID"])));
                      feed.LastUpdatedTime = lastPosted;

                      // Alternate Link
                      feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                  }

                  feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));

                syndicationItems.AddSyndicationItem(
                  row["Subject"].ToString(), 
                  YafFormatMessage.FormatMessage(row["Message"].ToString(), new MessageFlags(row["Flags"])),
                   null,
                  YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", this.Request.QueryString.GetFirstOrDefault("t")),
                   "{0}FeedType{1}TopicID{2}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, this.Request.QueryString.GetFirstOrDefault("t")),
                   lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
              }
             
            }
          }

          break;

        // Forum Feed
        case YafRssFeeds.Forum:
          int icategoryId = 0;
          object categoryId = null;
          if (this.Request.QueryString.GetFirstOrDefault("c") != null && int.TryParse(this.Request.QueryString.GetFirstOrDefault("c"), out icategoryId))
          {
              categoryId = icategoryId;
          }
          using (
            DataTable dt = DB.forum_listread(
              this.PageContext.PageBoardID, this.PageContext.PageUserID, categoryId, null))
          {
 
            feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("DEFAULT", "FORUM"));

            foreach (DataRow row in dt.Rows)
            {
                  DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField() ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset : DateTime.MinValue + YafServices.DateTime.TimeOffset.Add(TimeSpan.FromDays(2));

                  if (syndicationItems.Count <= 0)
                  {
                      feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));
                      feed.LastUpdatedTime = lastPosted;

                      // Alternate Link
                      feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true))));
                  }
                  if (!row["LastUserID"].IsNullOrEmptyDBField())
                  {
                      feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                           Convert.ToInt64(
                                                                                               row["LastUserID"])));
                  }

                syndicationItems.AddSyndicationItem(
                row["Forum"].ToString(), 
                row["Description"].ToString(),
                null,
                YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true, "f={0}", row["ForumID"]),
               "{0}FeedType{1}ForumID{2}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, Convert.ToInt32(row["ForumID"])),
               lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
            }
            
          }
 
          break;

        // Topics Feed
        case YafRssFeeds.Topics:
          if (!this.PageContext.ForumReadAccess)
          {
            YafBuildLink.AccessDenied();
          }

          int forumId;
          if (this.Request.QueryString.GetFirstOrDefault("f") != null && int.TryParse(this.Request.QueryString.GetFirstOrDefault("f"), out forumId))
          {
           
            // vzrus changed to separate DLL specific code
            using (DataTable dt = DB.rsstopic_list(forumId))
            {

                feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("DEFAULT", "FORUM") + ":" + this.PageContext.PageForumName);

               
              foreach (DataRow row in dt.Rows)
              {
                  DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField() ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                  if (syndicationItems.Count <= 0)
                  {
                      feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));
                      feed.LastUpdatedTime = lastPosted;

                      // Alternate Link
                      feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                  }

                  feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));
                
                  syndicationItems.AddSyndicationItem(
                  row["Topic"].ToString(),
                  SyndicationItemExtensions.GetContent(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]), lastPostIcon, lastPostName, lastPostName),
                  null,
                  YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["TopicID"]),
                  "{0}FeedType{1}TopicID{2}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, Convert.ToInt32(row["TopicID"])),
                  lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
              }
             
            }
          }

        break;

        // Active Topics
        case YafRssFeeds.Active:
        DateTime toActDate = DateTime.UtcNow;
        string toActText = this.PageContext.Localization.GetText("MYTOPICS", "LAST_MONTH");

        if (this.Request.QueryString.GetFirstOrDefault("txt") != null)
        {
            toActText = Server.UrlDecode(Server.HtmlDecode(this.Request.QueryString.GetFirstOrDefault("txt").ToString()));
        }  
     
        if (this.Request.QueryString.GetFirstOrDefault("d") != null)
        {
            if (!DateTime.TryParse(Server.UrlDecode(Server.HtmlDecode(this.Request.QueryString.GetFirstOrDefault("d").ToString())), out toActDate))
            {
                toActDate = Convert.ToDateTime(YafServices.DateTime.FormatDateTimeShort(DateTime.UtcNow)) + TimeSpan.FromDays(-31);
                toActText = this.PageContext.Localization.GetText("MYTOPICS", "LAST_MONTH");
            }
            else
            {
                // To limit number of feeds items by timespan if we are getting an unreasonable time                
                if (toActDate < DateTime.UtcNow + TimeSpan.FromDays(-31))
                {
                    toActDate = DateTime.UtcNow + TimeSpan.FromDays(-31);
                    toActText = this.PageContext.Localization.GetText("MYTOPICS", "LAST_MONTH");
                }
            }
        } 
            
            feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("MYTOPICS", "ACTIVETOPICS") + " - " + toActText);
            
            using (
              DataTable dt = DB.topic_active(
                this.PageContext.PageBoardID, 
                this.PageContext.PageUserID, 
                toActDate, 
                (this.PageContext.Settings.CategoryID == 0) ? null : (object)this.PageContext.Settings.CategoryID, 
                false))
            {
                foreach (DataRow row in dt.Rows)
                {
                    DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField() ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["UserID"])));
                        feed.LastUpdatedTime = lastPosted;

                        // Alternate Link
                        feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                    }

                    feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));
                 
                    syndicationItems.AddSyndicationItem(
                      row["Subject"].ToString(),
                      SyndicationItemExtensions.GetContent(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]), lastPostIcon, lastPostName, lastPostName),
                      null,
                      YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]),
                      "{0}FeedType{1}TopicID{2}MessageID{3}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, Convert.ToInt32(row["LinkTopicID"]), Convert.ToInt32(row["LastMessageID"])),
                      lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
                }
               
            }

            break;
          case YafRssFeeds.Favorite:
          DateTime toFavDate = DateTime.UtcNow;
          string toFavText = this.PageContext.Localization.GetText("MYTOPICS", "LAST_MONTH");

          if (this.Request.QueryString.GetFirstOrDefault("txt") != null)
          {
              toFavText = Server.UrlDecode(Server.HtmlDecode(this.Request.QueryString.GetFirstOrDefault("txt").ToString()));
          }

          if (this.Request.QueryString.GetFirstOrDefault("d") != null)
          {
              if (!DateTime.TryParse(Server.UrlDecode(Server.HtmlDecode(this.Request.QueryString.GetFirstOrDefault("d").ToString())), out toFavDate))
              {
                  toFavDate = this.PageContext.CurrentUserData.Joined == null ? DateTime.MinValue + TimeSpan.FromDays(2) : (DateTime)this.PageContext.CurrentUserData.Joined;
                  toFavText = this.PageContext.Localization.GetText("MYTOPICS", "SHOW_ALL");
              }
          }
          else
          {
              toFavDate = this.PageContext.CurrentUserData.Joined == null ? DateTime.MinValue +TimeSpan.FromDays(2) : (DateTime)this.PageContext.CurrentUserData.Joined;
              toFavText = this.PageContext.Localization.GetText("MYTOPICS", "SHOW_ALL");
          }
          
          using (
            DataTable dt = DB.topic_favorite_details(
              this.PageContext.PageBoardID, 
              this.PageContext.PageUserID, 
              toFavDate, 
              (this.PageContext.Settings.CategoryID == 0) ? null : (object)this.PageContext.Settings.CategoryID, 
              false))
          {
              foreach (DataRow row in dt.Rows)
              {
                  feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("MYTOPICS", "FAVORITETOPICS") + " - " + toFavText);

                  DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField() ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;
                 
                  if (syndicationItems.Count <= 0)
                  {
                      feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["UserID"])));
                      feed.LastUpdatedTime = lastPosted;

                      // Alternate Link
                      feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                  }

                  feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["LastUserID"])));
                 
                  syndicationItems.AddSyndicationItem(
                    row["Subject"].ToString(),
                    SyndicationItemExtensions.GetContent(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]), lastPostIcon, lastPostName, lastPostName),
                     null,
                    YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]),
                    "{0}FeedType{1}TopicID{2}MessageID{3}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, Convert.ToInt32(row["LinkTopicID"]), Convert.ToInt32(row["LastMessageID"])),
                    lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
              }

             
          }
          
          break;
          default:
          YafBuildLink.AccessDenied();
          break;
      }  

      // update the feed with the item list...      
      feed.Items = syndicationItems;

      // Self Link
      feed.Links.Add(new SyndicationLink(new Uri(!atomFeedByVar ? Request.Url.AbsoluteUri : Request.Url.AbsoluteUri + "&pg=atom")));

      var writer = new XmlTextWriter(this.Response.OutputStream, Encoding.UTF8);
    
      // write the feed to the response writer);
        if (!atomFeed)
        {
            var rssFormatter = new Rss20FeedFormatter(feed);
            rssFormatter.WriteTo(writer);
           // this.Response.ContentType = "text/rss+xml";
        }
        else
        {
            var atomFormatter = new Atom10FeedFormatter(feed);
            atomFormatter.WriteTo(writer);
          //  this.Response.ContentType = "text/atom+xml";
        }
   
        writer.Close();

      this.Response.ContentEncoding = Encoding.UTF8;
      this.Response.ContentType = "text/xml";
      this.Response.Cache.SetCacheability(HttpCacheability.Public);

      this.Response.End();
    }

    #endregion
  }
}