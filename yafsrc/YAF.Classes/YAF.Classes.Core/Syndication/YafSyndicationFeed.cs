/* Yet Another Forum.NET
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
  using System.ServiceModel.Syndication;

  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The syndication item extensions.
  /// </summary>
  public static class SyndicationItemExtensions
  {
    #region Public Methods

    /// <summary>
    /// The add syndication item.
    /// </summary>
    /// <param name="currentList">
    /// The current list.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="link">
    /// The alternate link.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    /// <param name="author">
    /// The author.
    /// </param> 
    public static void AddSyndicationItem(
      this List<SyndicationItem> currentList, string title, string content, string summary,string link, string id, DateTime posted, string  author)
    {
        var si = new SyndicationItem(
            YafServices.BadWordReplace.Replace(title),
            new TextSyndicationContent(YafServices.BadWordReplace.Replace(content),TextSyndicationContentKind.Html),
            // Alternate Link
            new Uri(link),
            id,
            new DateTimeOffset(posted));
       
        // si.Links.Add(SyndicationLink.CreateSelfLink(new Uri(link), "text/html"));
        si.PublishDate = new DateTimeOffset(posted);
        si.Authors.Add(new SyndicationPerson(String.Empty, author, String.Empty));
       
        if (summary.IsNotSet())
        {
            si.Summary = new TextSyndicationContent(YafServices.BadWordReplace.Replace(content),
                                                    TextSyndicationContentKind.Html);
        }

        currentList.Add(si);
    }

    public static SyndicationPerson NewSyndicationPerson(string userEmail, long userId)
    {
        return new SyndicationPerson(userEmail, YafContext.Current.BoardSettings.EnableDisplayName
                             ? UserMembershipHelper.GetDisplayNameFromID(userId)
                            : UserMembershipHelper.GetUserNameFromID(userId), YafBuildLink.GetLinkNotEscaped(ForumPages.profile,true,"u={0}", userId));
    }


   public static string GetContent(string link, string imgUrl, string imgAlt, string linkName)
   {

       return @"<a href=""" + link + @""" >" + @"<img src=""{0}"" alt =""{1}"" />".FormatWith(imgUrl, imgAlt) + linkName +
              "</a>"; 
   }


    /// <summary>
    /// The add syndication item.
    /// </summary>
    /// <param name="currentList">
    /// The current list.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="link">
    /// The link.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    public static void AddSyndicationItem(
      this List<SyndicationItem> currentList, string title, string content, string link, string id, DateTime posted)
    {
        var si = new SyndicationItem(
            YafServices.BadWordReplace.Replace(title),
            new TextSyndicationContent(YafServices.BadWordReplace.Replace(content)),
            new Uri(link),
            id,
            new DateTimeOffset(posted));
        currentList.Add(si);
    }
    #endregion
  }

  /// <summary>
  /// Summary description for YafSyndicationFeed.
  /// </summary>
  public class YafSyndicationFeed : SyndicationFeed
  {
      #region Constants

      private const string FeedCategories = "YAF,YetAnotherForum";

      #endregion

   #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafSyndicationFeed"/> class. 
    /// </summary>
    /// <param name="subTitle"></param>
    public YafSyndicationFeed(string subTitle)
    {
        this.Copyright = new TextSyndicationContent("Copyright 2006 - 2010 Jaben Cargman");
        this.Description = new TextSyndicationContent("YetAnotherForum.NET - {0}".FormatWith(YafContext.Current.Localization.GetText("RSSFEED")));
        this.Title = new TextSyndicationContent("{0} - {1} - {2}".FormatWith(YafContext.Current.Localization.GetText("RSSFEED"), YafContext.Current.BoardSettings.Name, subTitle));
        this.Id = "{0}&pg={1}".FormatWith(YafContext.HttpContext.Request.Url.AbsoluteUri, YafSyndicationFormats.Atom.ToInt());
        this.LastUpdatedTime = DateTime.UtcNow;
        this.Language = YafContext.Current.Localization.LanguageCode;
        this.BaseUri = new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.forum, true));
        this.Generator = "YetAnotherForum.NET - {0}".FormatWith(YafContext.Current.Localization.GetText("RSSFEED"));
        this.Categories.Add(new SyndicationCategory(FeedCategories));
        this.ImageUrl = new Uri("{0}{1}/YAFLogo.jpg".FormatWith(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Images), UriKind.Relative);
     
        // writer.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"" + YafForumInfo.ForumClientFileRoot + "rss.xsl\" media=\"screen\"?>");
    }
    #endregion

    ///// <summary>
    ///// The write rss prologue.
    ///// </summary>
    ///// <param name="writer">
    ///// The writer.
    ///// </param>
    ///// <returns>
    ///// </returns>
    // public XmlTextWriter WriteRSSPrologue(XmlTextWriter writer)
    // {
    // /*
    // writer.WriteStartDocument();
    // writer.WriteStartElement("rss");
    // writer.WriteAttributeString("version", "2.0");
    // writer.WriteStartElement("channel");
    // writer.WriteElementString("title", "RSS File for " + page.ForumURL);
    // writer.WriteElementString("link", page.ForumURL);
    // writer.WriteElementString("description", "Yet Another Forum Web Application");
    // writer.WriteElementString("copyright", "Copyright 2002-2004 Bjørnar Henden");
    // */
    // writer.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + this.en);

    // writer.WriteRaw("<rss version=\"2.0\">" + this.en);
    // writer.WriteRaw("\t<channel>" + this.en);
    // writer.WriteRaw("\t\t<title>RSS Feed for " + YafContext.Current.BoardSettings.Name + "</title>" + this.en);
    // writer.WriteRaw("\t\t<link>" + Encode(YafForumInfo.ForumURL) + "</link>" + this.en);
    // writer.WriteRaw("\t\t<description>Yet Another Forum.NET Forum RSS Feed</description>" + this.en);
    // writer.WriteRaw("\t\t<copyright>Copyright 2006 - 2010 Jaben Cargman</copyright>" + this.en);

    // return writer;
    // }

    ///// <summary>
    ///// The add rss item.
    ///// </summary>
    ///// <param name="writer">
    ///// The writer.
    ///// </param>
    ///// <param name="sItemTitle">
    ///// The s item title.
    ///// </param>
    ///// <param name="sItemLink">
    ///// The s item link.
    ///// </param>
    ///// <param name="sItemDescription">
    ///// The s item description.
    ///// </param>
    ///// <returns>
    ///// </returns>
    // public XmlTextWriter AddRSSItem(XmlTextWriter writer, string sItemTitle, string sItemLink, string sItemDescription)
    // {
    // return AddRSSItem(writer, sItemTitle, sItemLink, sItemDescription, DateTime.UtcNow.ToString("r"));
    // }

    ///// <summary>
    ///// The add rss item.
    ///// </summary>
    ///// <param name="writer">
    ///// The writer.
    ///// </param>
    ///// <param name="sItemTitle">
    ///// The s item title.
    ///// </param>
    ///// <param name="sItemLink">
    ///// The s item link.
    ///// </param>
    ///// <param name="sItemDescription">
    ///// The s item description.
    ///// </param>
    ///// <param name="sPubDate">
    ///// The s pub date.
    ///// </param>
    ///// <returns>
    ///// </returns>
    // public XmlTextWriter AddRSSItem(XmlTextWriter writer, string sItemTitle, string sItemLink, string sItemDescription, string sPubDate)
    // {
    // /*
    // writer.WriteStartElement("item");
    // writer.WriteElementString("title", sItemTitle);
    // writer.WriteElementString("link", sItemLink);
    // writer.WriteElementString("description", sItemDescription);
    // writer.WriteElementString("pubDate", DateTime.UtcNow.ToString("r"));
    // writer.WriteEndElement();
    // */
    // writer.WriteRaw("\t\t<item>" + this.en);
    // writer.WriteRaw("\t\t\t<title>" + Encode(sItemTitle) + "</title>" + this.en);
    // writer.WriteRaw("\t\t\t<link>" + Encode(sItemLink) + "</link>" + this.en);
    // writer.WriteRaw("\t\t\t<description><![CDATA[" + sItemDescription + "]]></description>" + this.en);
    // writer.WriteRaw("\t\t\t<pubDate>" + sPubDate + "</pubDate>" + this.en);
    // writer.WriteRaw("\t\t</item>" + this.en);

    // return writer;
    // }

    ///// <summary>
    ///// The write rss closing.
    ///// </summary>
    ///// <param name="writer">
    ///// The writer.
    ///// </param>
    ///// <returns>
    ///// </returns>
    // public XmlTextWriter WriteRSSClosing(XmlTextWriter writer)
    // {
    // /*
    // writer.WriteEndElement();
    // writer.WriteEndElement();
    // writer.WriteEndDocument();
    // */
    // writer.WriteRaw("\t</channel>" + this.en);
    // writer.WriteRaw("</rss>");

    // return writer;
    // }

    ///// <summary>
    ///// The encode.
    ///// </summary>
    ///// <param name="input">
    ///// The input.
    ///// </param>
    ///// <returns>
    ///// The encode.
    ///// </returns>
    // private string Encode(string input)
    // {
    // string output = input;
    // output = output.Replace("&", "&amp;");
    // return output;
    // }
  }
}