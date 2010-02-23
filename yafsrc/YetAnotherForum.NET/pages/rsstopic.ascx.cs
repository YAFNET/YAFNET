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
      var feed = new YafSyndicationFeed();
      var writer = new XmlTextWriter(this.Response.OutputStream, Encoding.UTF8);
      var syndicationItems = new List<SyndicationItem>();

      YafRssFeeds feedType = YafRssFeeds.Forum;

      try
      {
        feedType = this.Request.QueryString["pg"].ToEnum<YafRssFeeds>(true);
      }
      catch
      {
        // default to Forum Feed.
      }

      switch (feedType)
      {
        case YafRssFeeds.LatestPosts:
          if (!this.PageContext.ForumReadAccess)
          {
            YafBuildLink.AccessDenied();
          }

          using (DataTable dtTopics = YafServices.DBBroker.GetLatestTopics(10))
          {
            foreach (DataRow row in dtTopics.Rows)
            {
              syndicationItems.AddSyndicationItem(
                row["Subject"].ToString(), 
                row["Message"].ToString(), 
                YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", this.Request.QueryString["t"]), 
                String.Format("TopicID{0}", this.Request.QueryString["t"]), 
                Convert.ToDateTime(row["Posted"]));
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
            foreach (DataRow row in dt.Rows)
            {
              syndicationItems.AddSyndicationItem(
                row["Subject"].ToString(), 
                row["Message"].ToString(), 
                YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", this.Request.QueryString["t"]), 
                String.Format("TopicID{0}", this.Request.QueryString["t"]), 
                Convert.ToDateTime(row["Posted"]));
            }
          }

          break;
        case YafRssFeeds.Posts:
          if (!this.PageContext.ForumReadAccess)
          {
            YafBuildLink.AccessDenied();
          }

          if (this.Request.QueryString["t"] != null)
          {
            using (
              DataTable dt = DB.post_list(
                this.PageContext.PageTopicID, 0, this.PageContext.BoardSettings.ShowDeletedMessages, false, false))
            {
              foreach (DataRow row in dt.Rows)
              {
                syndicationItems.AddSyndicationItem(
                  row["Subject"].ToString(), 
                  FormatMsg.FormatMessage(row["Message"].ToString(), new MessageFlags(row["Flags"])), 
                  YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", this.Request.QueryString["t"]), 
                  String.Format("TopicID{0}", this.Request.QueryString["t"]), 
                  Convert.ToDateTime(row["Posted"]));
              }
            }
          }

          break;
        case YafRssFeeds.Forum:
          using (DataTable dt = DB.forum_listread(this.PageContext.PageBoardID, this.PageContext.PageUserID, null, null)
            )
          {
            foreach (DataRow row in dt.Rows)
            {
              syndicationItems.AddSyndicationItem(
                row["Forum"].ToString(), 
                row["Description"].ToString(), 
                YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true, "f={0}", row["ForumID"]), 
                String.Format("ForumID{0}", row["ForumID"]), 
                DateTime.Now);
            }
          }

          break;
        case YafRssFeeds.Topics:
          if (!this.PageContext.ForumReadAccess)
          {
            YafBuildLink.AccessDenied();
          }

          int forumId;
          if (this.Request.QueryString["f"] != null && int.TryParse(this.Request.QueryString["f"], out forumId))
          {
            // vzrus changed to separate DLL specific code
            using (DataTable dt = DB.rsstopic_list(forumId))
            {
              foreach (DataRow row in dt.Rows)
              {
                syndicationItems.AddSyndicationItem(
                  row["Topic"].ToString(), 
                  row["Topic"].ToString(), 
                  YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["TopicID"]), 
                  String.Format("TopicID{0}", row["Topic"].ToString()), 
                  Convert.ToDateTime(row["Posted"]));
              }
            }
          }

          break;
        case YafRssFeeds.Active:
          using (
            DataTable dt = DB.topic_active(
              this.PageContext.PageBoardID, 
              this.PageContext.PageUserID, 
              DateTime.Now + TimeSpan.FromHours(-24), 
              (this.PageContext.Settings.CategoryID == 0) ? null : (object)this.PageContext.Settings.CategoryID, 
              false))
          {
            foreach (DataRow row in dt.Rows)
            {
              syndicationItems.AddSyndicationItem(
                row["Subject"].ToString(), 
                row["Subject"].ToString(), 
                YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]), 
                String.Format("TopicID{0}", row["LinkTopicID"].ToString()), 
                DateTime.Now);
            }
          }

          break;
        case YafRssFeeds.Favorite:
          using (
            DataTable dt = DB.topic_favorite_details(
              this.PageContext.PageBoardID, 
              this.PageContext.PageUserID, 
              DateTime.Now + TimeSpan.FromHours(-24), 
              (this.PageContext.Settings.CategoryID == 0) ? null : (object)this.PageContext.Settings.CategoryID, 
              false))
          {
            foreach (DataRow row in dt.Rows)
            {
              syndicationItems.AddSyndicationItem(
                row["Subject"].ToString(), 
                row["Subject"].ToString(), 
                YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]), 
                String.Format("TopicID{0}", row["LinkTopicID"].ToString()), 
                DateTime.Now);
            }
          }

          break;
        default:
          break;
      }

      // update the feed with the item list...
      feed.Items = syndicationItems;

      // write the feed to the response writer
      var rssFormatter = new Rss20FeedFormatter(feed);
      rssFormatter.WriteTo(writer);
      writer.Close();

      this.Response.ContentEncoding = Encoding.UTF8;
      this.Response.ContentType = "text/xml";
      this.Response.Cache.SetCacheability(HttpCacheability.Public);

      this.Response.End();
    }

    #endregion
  }
}