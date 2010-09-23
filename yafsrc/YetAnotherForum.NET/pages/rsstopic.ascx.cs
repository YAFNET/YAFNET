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
  /// Summary description for rss topic.
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
      if (!(PageContext.BoardSettings.ShowRSSLink || PageContext.BoardSettings.ShowAtomLink))
      {
          YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
      }
     
      // Atom feed as variable
       bool atomFeedByVar = Request.QueryString.GetFirstOrDefault("ft") ==
                             YafSyndicationFormats.Atom.ToInt().ToString();

      YafSyndicationFeed feed = null;
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

      switch (feedType)
      {
       // Latest posts feed
        case YafRssFeeds.LatestPosts:
            if (!this.PageContext.BoardSettings.ShowActiveDiscussions)
            {
                YafBuildLink.AccessDenied();
            }

              GetPostLatestFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName);
        break;

        // Latest Announcements feed
        case YafRssFeeds.LatestAnnouncements:
            if (!this.PageContext.ForumReadAccess)
            {
                YafBuildLink.AccessDenied();
            }

              GetLatestAnnouncementsFeed(ref feed, feedType, atomFeedByVar);
            break;

        // Posts Feed
        case YafRssFeeds.Posts:
            if (!(this.PageContext.ForumReadAccess && PageContext.BoardSettings.ShowPostsFeeds))
            {
                YafBuildLink.AccessDenied();
            }
            
            if (this.Request.QueryString.GetFirstOrDefault("t") != null)
            {
                int topicId;   
                if (int.TryParse(this.Request.QueryString.GetFirstOrDefault("t"), out topicId))
                {
                    GetPostsFeed(ref feed, feedType, atomFeedByVar, topicId);
                }
            }

          break;

        // Forum Feed
        case YafRssFeeds.Forum:
            
            object categoryId = null;
            
            if (this.Request.QueryString.GetFirstOrDefault("c") != null)
            {
               int icategoryId = 0;
               if (int.TryParse(this.Request.QueryString.GetFirstOrDefault("c"), out icategoryId))
                {
                    categoryId = icategoryId;
                }
            }
            
            GetForumFeed(ref feed, feedType,atomFeedByVar, categoryId);
            break;

        // Topics Feed
        case YafRssFeeds.Topics:
            if (!this.PageContext.ForumReadAccess)
            {
                YafBuildLink.AccessDenied();
            }
            
            int forumId;
            if (this.Request.QueryString.GetFirstOrDefault("f") != null)
            {
                if (int.TryParse(this.Request.QueryString.GetFirstOrDefault("f"), out forumId))
                {
                    GetTopicsFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName, forumId);
                }
            }
            break;

        // Active Topics
        case YafRssFeeds.Active:
            int categoryActiveIntId;
            object categoryActiveId = null;
            if (this.Request.QueryString.GetFirstOrDefault("f") != null && int.TryParse(this.Request.QueryString.GetFirstOrDefault("f"), out categoryActiveIntId))
            {
                categoryActiveId = categoryActiveIntId;
            }

            GetActiveFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName, categoryActiveId);
            
            break;
        case YafRssFeeds.Favorite:
            int categoryFavIntId;
            object categoryFavId = null;
            if (this.Request.QueryString.GetFirstOrDefault("f") != null && int.TryParse(this.Request.QueryString.GetFirstOrDefault("f"), out categoryFavIntId))
            {
                categoryFavId = categoryFavIntId;
            }

            GetFavoriteFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName, categoryFavId);
            break;
          default:
              YafBuildLink.AccessDenied();
              break;
      }  

      // update the feed with the item list... 
      // the list should be added after all other feed properties are set
      if (feed != null)
      {
          var writer = new XmlTextWriter(this.Response.OutputStream, Encoding.UTF8);
          writer.WriteStartDocument();

          // write the feed to the response writer);
          if (!atomFeedByVar)
          {
              var rssFormatter = new Rss20FeedFormatter(feed);
              rssFormatter.WriteTo(writer);
             // this.Response.ContentType = "text/rss+xml";
          }
          else
          {
              var atomFormatter = new Atom10FeedFormatter(feed);
              atomFormatter.WriteTo(writer);

             // this.Response.ContentType = "text/atom+xml";
          }
        
          writer.WriteEndDocument();
          writer.Close();

          this.Response.ContentEncoding = Encoding.UTF8;
          this.Response.ContentType = "text/xml";
          this.Response.Cache.SetCacheability(HttpCacheability.Public);

          this.Response.End();
      }
      else
      {
          YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
      }
    }

    /// <summary>
    /// The method to return latest topic content to display in a feed.
    /// </summary>
    /// <param name="link">A linkt to an active topic.</param>
    /// <param name="imgUrl">A latest topic icon Url.</param>
    /// <param name="imgAlt">A latest topic icon Alt text.</param>
    /// <param name="linkName">A latest topic displayed link name</param>
    /// <param name="text">An active topic first message content/partial content.</param>
    /// <returns>An Html formatted first message content string.</returns>
    private static string GetPostLatestContent(string link, string imgUrl, string imgAlt, string linkName, string text, int flags)
    {
        // this stub should be replaced by something more usable
        return @"<a href=""{0}"" ><img src=""{1}"" alt =""{2}"" />{3}</a>".FormatWith(link, imgUrl, imgAlt, linkName);
        /* return YafFormatMessage.FormatMessage(text, new MessageFlags{IsBBCode = true}) + @"<br /><a href=""" + link + @""" >" + @"<img src=""{0}"" alt =""{1}"" />".FormatWith(imgUrl, imgAlt) + linkName +
               "</a>"; */
    }

    /// <summary>
    /// The method creates YafSyndicationFeed for topics in a forum.
    /// </summary>
    /// <param name="feed">The YafSyndicationFeed.</param>
    /// <param name="feedType">The FeedType.</param>
    /// <param name="atomFeedByVar">The Atom feed checker.</param>
    /// <param name="lastPostIcon">The icon for last post link.</param>
    /// <param name="lastPostName">The last post name.</param>
    private void GetPostLatestFeed(ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, string lastPostIcon, string lastPostName)
    {
        var syndicationItems = new List<SyndicationItem>();

        using (DataTable dataTopics = YafServices.DBBroker.GetLatestTopics(this.PageContext.BoardSettings.ActiveDiscussionsCount <= 50 ? this.PageContext.BoardSettings.ActiveDiscussionsCount : 50, PageContext.PageUserID, "LastUserStyle"))
        {
            feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("ACTIVE_DISCUSSIONS"), feedType, atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt());

            foreach (DataRow row in dataTopics.Rows)
            {
                // don't render moved topics
                if (row["TopicMovedID"].IsNullOrEmptyDBField())
                {
                    DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField()
                                              ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset
                                              : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;
                    if (syndicationItems.Count <= 0)
                    {
                        feed.LastUpdatedTime = lastPosted;
                        feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                        Convert.ToInt64(row["UserID"])));

                        // Alternate Link for feed
                        // feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                    }

                    feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                         Convert.ToInt64(
                                                                                             row["LastUserID"])));

                    string messageLink = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}",
                                                                        row["LastMessageID"]);

                    syndicationItems.AddSyndicationItem(
                        row["Topic"].ToString(),
                        GetPostLatestContent(messageLink, lastPostIcon, lastPostName, lastPostName, String.Empty,
                                             !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                                 ? Convert.ToInt32(row["LastMessageFlags"])
                                                 : 22),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", Convert.ToInt32(row["TopicID"])),
                        "{0}FeedType{1}TopicID{2}MessageID{3}".FormatWith(YafContext.Current.BoardSettings.Name,
                                                                          feedType, Convert.ToInt32(row["TopicID"]),
                                                                          Convert.ToInt32(row["LastMessageID"])),
                        lastPosted,
                        feed.Contributors[feed.Contributors.Count - 1].Name);
                }
            }

            feed.Items = syndicationItems;
        }
    }

    /// <summary>
    /// The method creates YafSyndicationFeed for topic announcements.
    /// </summary>
    /// <param name="feed">The YafSyndicationFeed.</param>
    /// <param name="feedType">The FeedType.</param>
    /// <param name="atomFeedByVar">The Atom feed checker.</param>
    private void GetLatestAnnouncementsFeed(ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar)
    {
        var syndicationItems = new List<SyndicationItem>();
        using (DataTable dt = DB.topic_announcements(this.PageContext.PageBoardID, 10, this.PageContext.PageUserID))
        {
            feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("POSTMESSAGE", "ANNOUNCEMENT"), feedType, atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt());

            foreach (DataRow row in dt.Rows)
            {
                   // don't render moved topics
                if (row["TopicMovedID"].IsNullOrEmptyDBField())
                {
                    DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField()
                                              ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset
                                              : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                        Convert.ToInt64(row["UserID"])));
                        feed.LastUpdatedTime = lastPosted;

                        // Alternate Link
                        // feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                    }

                    feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                         Convert.ToInt64(
                                                                                             row["LastUserID"])));

                    syndicationItems.AddSyndicationItem(
                        row["Subject"].ToString(),
                        row["Message"].ToString(),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}",
                                                       this.Request.QueryString.GetFirstOrDefault("t")),
                        "{0}FeedType{1}TopicID{2}".FormatWith(YafContext.Current.BoardSettings.Name, feedType,
                                                              this.Request.QueryString.GetFirstOrDefault("t")),
                        lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
                }
            }

            feed.Items = syndicationItems;
        }
    }

    /// <summary>
    /// The method creates YafSyndicationFeed for posts.
    /// </summary>
    /// <param name="feed">The YafSyndicationFeed.</param>
    /// <param name="feedType">The FeedType.</param>
    /// <param name="atomFeedByVar">The Atom feed checker.</param>
    private void GetPostsFeed(ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, int topicId)
    {
        var syndicationItems = new List<SyndicationItem>();
        using (
             DataTable dt = DB.post_list(
               topicId, 0, this.PageContext.BoardSettings.ShowDeletedMessages, false))
        {
            // last page posts
            var dataRows = dt.AsEnumerable().Take(PageContext.BoardSettings.PostsPerPage);
            var altItem = false;
 
            // load the missing message test
            YafServices.DBBroker.LoadMessageText(dataRows);
            feed = new YafSyndicationFeed("{0}{1} - {2}".FormatWith(this.PageContext.Localization.GetText("PROFILE", "TOPIC"), this.PageContext.PageTopicName, PageContext.BoardSettings.PostsPerPage), feedType, atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt());
       
            foreach (var row in dataRows)
            {
                DateTime posted = Convert.ToDateTime(row["Edited"]) + YafServices.DateTime.TimeOffset;

                if (syndicationItems.Count <= 0)
                {
                    feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["UserID"])));
                    feed.LastUpdatedTime = posted;

                    // Alternate Link
                    // feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                }

                feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty, Convert.ToInt64(row["UserID"])));

                syndicationItems.AddSyndicationItem(
                  row["Subject"].ToString(),
                  YafFormatMessage.FormatSyndicationMessage(row["Message"].ToString(), new MessageFlags(row["Flags"]), altItem, 32000),
                  null,
                  YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", this.Request.QueryString.GetFirstOrDefault("t")),
                  "{0}FeedType{1}TopicID{2}".FormatWith(YafContext.Current.BoardSettings.Name, feedType, this.Request.QueryString.GetFirstOrDefault("t")),
                  posted, 
                  feed.Contributors[feed.Contributors.Count - 1].Name);
                  altItem = !altItem;
            }

            feed.Items = syndicationItems;
        }
    }

    /// <summary>
    /// The method creates YafSyndicationFeed for forums in a category.
    /// </summary>
    /// <param name="feed">The YafSyndicationFeed.</param>
    /// <param name="feedType">The FeedType.</param>
    /// <param name="atomFeedByVar">The Atom feed checker.</param>
    /// <param name="categoryId">The category id.</param>
    private void GetForumFeed(ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, object categoryId)
    {
        var syndicationItems = new List<SyndicationItem>();
        using (
           DataTable dt = DB.forum_listread(
             this.PageContext.PageBoardID, this.PageContext.PageUserID, categoryId, null))
        {
            feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("DEFAULT", "FORUM"), feedType, atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt());

            foreach (DataRow row in dt.Rows)
            {
                if (row["TopicMovedID"].IsNullOrEmptyDBField())
                {
                    DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField()
                                              ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset
                                              : DateTime.MinValue +
                                                YafServices.DateTime.TimeOffset.Add(TimeSpan.FromDays(2));

                    if (syndicationItems.Count <= 0)
                    {
                        if (row["LastUserID"].IsNullOrEmptyDBField() || row["LastUserID"].IsNullOrEmptyDBField())
                        {
                            break;
                        }

                        feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(
                            String.Empty,
                            Convert.ToInt64(row["LastUserID"])));

                        feed.LastUpdatedTime = lastPosted;

                        // Alternate Link
                        // feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true))));
                    }

                    if (!row["LastUserID"].IsNullOrEmptyDBField())
                    {
                        feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(
                            String.Empty,
                            Convert.ToInt64(row["LastUserID"])));
                    }

                    syndicationItems.AddSyndicationItem(
                        row["Forum"].ToString(),
                        row["Description"].ToString(),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true, "f={0}", row["ForumID"]),
                        "{0}FeedType{1}ForumID{2}".FormatWith(YafContext.Current.BoardSettings.Name,
                                                              feedType,
                                                              Convert.ToInt32(row["ForumID"])),
                        lastPosted,
                        feed.Contributors[feed.Contributors.Count - 1].Name);
                }
            }

            feed.Items = syndicationItems;
        }
    }

    /// <summary>
    /// The method creates YafSyndicationFeed for topics in a forum.
    /// </summary>
    /// <param name="feed">The YafSyndicationFeed.</param>
    /// <param name="feedType">The FeedType.</param>
    /// <param name="atomFeedByVar">The Atom feed checker.</param>
    /// <param name="lastPostIcon">The icon for last post link.</param>
    /// <param name="lastPostName">The last post name.</param>
    /// <param name="forumId">The forum id.</param>
    private void GetTopicsFeed(ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, string lastPostIcon, string lastPostName, int forumId)
    {
        var syndicationItems = new List<SyndicationItem>();

            // vzrus changed to separate DLL specific code
            using (DataTable dt = DB.rsstopic_list(forumId))
            {
                feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("DEFAULT", "FORUM") + ":" + this.PageContext.PageForumName, feedType, atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt());

                foreach (DataRow row in dt.Rows)
                {
                 
                        DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField()
                                                  ? Convert.ToDateTime(row["LastPosted"]) +
                                                    YafServices.DateTime.TimeOffset
                                                  : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                        if (syndicationItems.Count <= 0)
                        {
                            feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                            Convert.ToInt64(
                                                                                                row["LastUserID"])));
                            feed.LastUpdatedTime = lastPosted;

                            // Alternate Link
                            //  feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                        }
                        if (!row["LastUserID"].IsNullOrEmptyDBField())
                        {
                            feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                                 Convert.ToInt64(
                                                                                                     row["LastUserID"])));
                        }

                        syndicationItems.AddSyndicationItem(
                            row["Topic"].ToString(),
                            GetPostLatestContent(
                                YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}",
                                                               row["LastMessageID"]), lastPostIcon, lastPostName,
                                lastPostName, String.Empty,
                                !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                    ? Convert.ToInt32(row["LastMessageFlags"])
                                    : 22),
                            null,
                            YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["TopicID"]),
                            "{0}FeedType{1}TopicID{2}".FormatWith(YafContext.Current.BoardSettings.Name, feedType,
                                                                  Convert.ToInt32(row["TopicID"])),
                            lastPosted,
                            feed.Contributors[feed.Contributors.Count - 1].Name);
                   
                }

                feed.Items = syndicationItems;
            }
    }

    /// <summary>
    /// The method creates YafSyndicationFeed for Active topics.
    /// </summary>
    /// <param name="feed">The YafSyndicationFeed.</param>
    /// <param name="feedType">The FeedType.</param>
    /// <param name="atomFeedByVar">The Atom feed checker.</param>
    /// <param name="lastPostIcon">The icon for last post link.</param>
    /// <param name="lastPostName">The last post name.</param>
    private void GetActiveFeed(ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, string lastPostIcon, string lastPostName, object categoryActiveId)
    {
        var syndicationItems = new List<SyndicationItem>();
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

        feed = new YafSyndicationFeed(this.PageContext.Localization.GetText("MYTOPICS", "ACTIVETOPICS") + " - " + toActText, feedType, atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt());

        using (
          DataTable dt = DB.topic_active(
            this.PageContext.PageBoardID,
            this.PageContext.PageUserID,
            toActDate,
            categoryActiveId,
            false))
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row["TopicMovedID"].IsNullOrEmptyDBField())
                {
                    DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField()
                                              ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset
                                              : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                        Convert.ToInt64(row["UserID"])));
                        feed.LastUpdatedTime = lastPosted;

                        // Alternate Link
                        // feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                    }

                    feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                         Convert.ToInt64(
                                                                                             row["LastUserID"])));

                    string messageLink = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}",
                                                                        row["LastMessageID"]);
                    syndicationItems.AddSyndicationItem(
                        row["Subject"].ToString(),
                        GetPostLatestContent(messageLink, lastPostIcon, lastPostName, lastPostName, String.Empty,
                                             !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                                 ? Convert.ToInt32(row["LastMessageFlags"])
                                                 : 22),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]),
                        messageLink,
                        lastPosted, feed.Contributors[feed.Contributors.Count - 1].Name);
                }
            }

            feed.Items = syndicationItems;
        }
    }

    /// <summary>
    /// The method creates YafSyndicationFeed for Favorite topics.
    /// </summary>
    /// <param name="feed">The YafSyndicationFeed.</param>
    /// <param name="feedType">The FeedType.</param>
    /// <param name="atomFeedByVar">The Atom feed checker.</param>
    /// <param name="lastPostIcon">The icon for last post link.</param>
    /// <param name="lastPostName">The last post name.</param>
    private void GetFavoriteFeed(ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, string lastPostIcon, string lastPostName, object categoryActiveId)
    {
        var syndicationItems = new List<SyndicationItem>();
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
            toFavDate = this.PageContext.CurrentUserData.Joined == null ? DateTime.MinValue + TimeSpan.FromDays(2) : (DateTime)this.PageContext.CurrentUserData.Joined;
            toFavText = this.PageContext.Localization.GetText("MYTOPICS", "SHOW_ALL");
        }

        using (
          DataTable dt = DB.topic_favorite_details(
            this.PageContext.PageBoardID,
            this.PageContext.PageUserID,
            toFavDate,
            categoryActiveId,
            false))
        {
            feed =
                     new YafSyndicationFeed(
                         "{0} - {1}".FormatWith(this.PageContext.Localization.GetText("MYTOPICS", "FAVORITETOPICS"),
                                                toFavText), feedType,
                         atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt());

            foreach (DataRow row in dt.Rows)
            {
                if (row["TopicMovedID"].IsNullOrEmptyDBField())
                {

                    DateTime lastPosted = !row["LastPosted"].IsNullOrEmptyDBField()
                                              ? Convert.ToDateTime(row["LastPosted"]) + YafServices.DateTime.TimeOffset
                                              : Convert.ToDateTime(row["Posted"]) + YafServices.DateTime.TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                        Convert.ToInt64(row["UserID"])));
                        feed.LastUpdatedTime = lastPosted;

                        // Alternate Link
                        // feed.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(YafContext.Current.CurrentForumPage.ForumURL)));
                    }

                    feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(String.Empty,
                                                                                         Convert.ToInt64(
                                                                                             row["LastUserID"])));

                    syndicationItems.AddSyndicationItem(
                        row["Subject"].ToString(),
                        GetPostLatestContent(
                            YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]),
                            lastPostIcon, lastPostName, lastPostName, String.Empty,
                            !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                ? Convert.ToInt32(row["LastMessageFlags"])
                                : 22),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]),
                        "{0}FeedType{1}TopicID{2}MessageID{3}".FormatWith(YafContext.Current.BoardSettings.Name,
                                                                          feedType, Convert.ToInt32(row["LinkTopicID"]),
                                                                          Convert.ToInt32(row["LastMessageID"])),
                        lastPosted,
                        feed.Contributors[feed.Contributors.Count - 1].Name);
                }
            }

            feed.Items = syndicationItems;
        }
    }

      #endregion
  }
}