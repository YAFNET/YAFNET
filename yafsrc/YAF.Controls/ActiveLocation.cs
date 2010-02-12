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

namespace YAF.Controls
{
    using System;
    using System.Web.UI;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Utils;

    /// <summary>
    /// Provides Active Users location info
    /// </summary>
    public class ActiveLocation : BaseControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveLocation"/> class.
        /// </summary>
        public ActiveLocation()
        {
        }

        /// <summary>
        /// Gets or sets the localization tag for the current location.
        /// It should be  equal to page name
        /// </summary>
        public string ForumPage
        {
            get
            {
                if (ViewState["ForumPage"] != null || ViewState["ForumPage"] != DBNull.Value)
                {
                  // string localizedPage = ViewState["ForumPage"].ToString().Substring(ViewState["ForumPage"].ToString().IndexOf("default.aspx?") - 14, ViewState["ForumPage"].ToString().IndexOf("&"));
                    return ViewState["ForumPage"].ToString();
                }

                return "MAINPAGE";
            }

            set
            {
                ViewState["ForumPage"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the forumname of the current location
        /// </summary>
        public string ForumName
        {
            get
            {
                if (ViewState["ForumName"] != null)
                {
                    return ViewState["ForumName"].ToString();
                }

                return string.Empty;
            }

            set
            {
                ViewState["ForumName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the topicname of the current location
        /// </summary>
        public string TopicName
        {
            get
            {
                if (ViewState["TopicName"] != null)
                {
                    return ViewState["TopicName"].ToString();
                }

                return string.Empty;
            }

            set
            {
                ViewState["TopicName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the forumid of the current location
        /// </summary>
        public int ForumID
        {
            get
            {
                if (ViewState["ForumID"] != null)
                {
                    return Convert.ToInt32(ViewState["ForumID"]);
                }

                return -1;
            }

            set
            {
                ViewState["ForumID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the topicid of the current location 
        /// </summary>
        public int TopicID
        {
            get
            {
                if (ViewState["TopicID"] != null)
                {
                    return Convert.ToInt32(ViewState["TopicID"]);
                }

                return -1;
            }

            set
            {
                ViewState["TopicID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the userid of the current user
        /// </summary>
        public int UserID
        {
            get
            {
                if (ViewState["UserID"] != null)
                {
                    return Convert.ToInt32(ViewState["UserID"]);
                }

                return -1;
            }

            set
            {
                ViewState["UserID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the UserName of the current user
        /// </summary>
        public string UserName
        {
            get
            {
                if (ViewState["UserName"] != null)
                {
                    return ViewState["UserName"].ToString();
                }

                return string.Empty;
            }

            set
            {
                ViewState["UserName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only topic link.
        /// </summary>
        public bool LastLinkOnly
        {
            get
            {
                if (ViewState["LastLinkOnly"] != null)
                {
                    return Convert.ToBoolean(ViewState["LastLinkOnly"]);
                }

                return false;
            }

            set
            {
                ViewState["LastLinkOnly"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only topic link.
        /// </summary>
        public bool HasForumAccess
        {
            get
            {
                if (ViewState["HasForumAccess"] != null)
                {
                    return Convert.ToBoolean(ViewState["HasForumAccess"]);
                }

                return true;
            }

            set
            {
                ViewState["HasForumAccess"] = value;
            }
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render(HtmlTextWriter output)
        {
            string forumPageName = this.ForumPage;
            string forumPageAttributes = null;
            
            // Find a user page name. If it's missing we are very probably on the start page 
            if (string.IsNullOrEmpty(forumPageName))
            {               
                    forumPageName = "MAINPAGE";               
            }
            else
            {
                // We find here a page name start position
                forumPageName = forumPageName.Substring(forumPageName.IndexOf("g=") + 2);

                // We find here a page name end position
                if (forumPageName.Contains("&"))
                {
                    forumPageAttributes = forumPageName.Substring(forumPageName.IndexOf("&") + 1);
                    forumPageName = forumPageName.Substring(0, forumPageName.IndexOf("&"));                    
                }
            } 
                
                     output.BeginRender();              
                     
                     // All pages should be processed in call frequency order 
                     // We are in messages
                     
                     if (this.TopicID > 0 && this.ForumID > 0)
                     {
                         if (forumPageName == "topics")
                         {
                             output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "TOPICS"));                            
                         }
                         else if (forumPageName == "posts")
                         {
                             output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "POSTS"));
                         }
                         else if (forumPageName == "postmessage")
                         {
                             output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "POSTMESSAGE_FULL"));
                         }
                         else
                         {
                             output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "POSTS"));
                         }
                         if (HasForumAccess)
                         {                   
                             output.Write(@"<a href=""{0}"" id=""topicid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.TopicID), this.UserID, this.TopicName);
                         if (!this.LastLinkOnly)
                         {
                             output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "TOPICINFORUM"));
                             output.Write(@"<a href=""{0}"" id=""forumidtopic_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.ForumID), this.UserID, this.ForumName);
                         }
                         }
                     }
                     else if (this.ForumID > 0 && this.TopicID <= 0)
                     {
                         // User views a forum
                        if (forumPageName == "topics")
                        {                
                            output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "FORUM"));
                            if (HasForumAccess)
                            {
                                output.Write(@"<a href=""{0}"" id=""forumid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.ForumID), this.UserID, this.ForumName);
                            }
                        }
                     }                  
                     else
                      { 
                          // First specially treated pages where we can render
                          // an info about user name, etc. 
                          if (forumPageName == "profile")
                          {
                              output.Write(Profile(forumPageAttributes));
                             
                          }
                          else if (forumPageName == "albums")
                          {
                              // On albums first page
                              output.Write(Albums(forumPageAttributes));                        
           
                          }
                          else if (forumPageName == "album")
                          {
                              // Views an album
                              output.Write(Album(forumPageAttributes));
                              
                          }
                          else if (forumPageName == "forum" && this.TopicID <= 0 && this.ForumID <= 0)
                          {
                              if (this.ForumPage.Contains("c="))
                              {
                                  output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "FORUMFROMCATEGORY"));
                              }
                              else
                              {
                                  output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "MAINPAGE"));
                              }
                          }                    
                          else if (!YafContext.Current.IsAdmin && forumPageName.ToUpper().Contains("MODERATE_"))
                          {

                              // We shouldn't show moderators activity to all users but admins
                              output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "MODERATE"));
                          }                          
                          else if (!YafContext.Current.IsHostAdmin && forumPageName.ToUpper().Contains("ADMIN_"))
                          {

                              // We shouldn't show admin activity to all users 
                              output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ADMINTASK"));
                          }                          
                          else
                          {
                              // Generic action name based on page name
                              output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", forumPageName));
                          }
                      } 

            output.EndRender();           
        }

        /// <summary>
        /// A method to get album path string.
        /// </summary>
        /// <param name="forumPageAttributes">A page query string cleared from page name.</param>
        /// <returns>The string</returns> 
        private string Album(string forumPageAttributes)
        {

            string outstring = string.Empty;
            string userID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=") + 2).Trim();

            if (userID.Contains("&"))
            {
                userID = userID.Substring(0, userID.IndexOf("&")).Trim();
            }

            string albumID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("a=") + 2);

            if (albumID.Contains("&"))
            {
                albumID = albumID.Substring(0, albumID.IndexOf("&")).Trim();
            }
            else
            {
                albumID = albumID.Substring(0).Trim();
            }

            if (ValidationHelper.IsValidInt(userID) && ValidationHelper.IsValidInt(albumID))
            {
                string albumName;

                // The DataRow should not be missing in the case
                System.Data.DataRow dr = YAF.Classes.Data.DB.album_list(null, Convert.ToInt32(albumID.Trim())).Rows[0];

                // If album doesn't have a Title, use his ID.
                if (!string.IsNullOrEmpty(dr["Title"].ToString()))
                {
                    albumName = dr["Title"].ToString();
                }
                else
                {
                    albumName = dr["AlbumID"].ToString();
                }

                // Render
                if (Convert.ToInt32(userID) != UserID)
                {
                    outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ALBUM"));                    
                    outstring += string.Format(@"<a href=""{0}"" id=""uiseralbumid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.album, "a={0}", albumID), userID + PageContext.PageUserID.ToString(), albumName);
                    outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ALBUM_OFUSER"));
                    outstring += string.Format(@"<a href=""{0}"" id=""albumuserid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.profile, "u={0}", userID), userID, YAF.Classes.Core.UserMembershipHelper.GetUserNameFromID(Convert.ToInt64(userID)));
                }
                else
                {
                    outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ALBUM_OWN"));
                    outstring += string.Format(@"<a href=""{0}"" id=""uiseralbumid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.album, "a={0}", albumID), userID + PageContext.PageUserID.ToString(), albumName);
                }

            }
            else
            {
                outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ALBUM"));
            }

            return outstring;
        }

        /// <summary>
        /// A method to get albums path string.
        /// </summary>
        /// <param name="forumPageAttributes">A page query string cleared from page name.</param>
        /// <returns>The string</returns> 
        private string Albums(string forumPageAttributes)
        {
           
            string outstring = string.Empty;

            string userID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=") + 2).Substring(0).Trim();

            if (ValidationHelper.IsValidInt(userID))
            {
                if (Convert.ToInt32(userID) == UserID)
                {
                    outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ALBUMS_OWN"));
                }
                else
                {
                    outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ALBUMS_OFUSER"));
                    outstring += string.Format(@"<a href=""{0}"" id=""albumsuserid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.profile, "u={0}", userID), userID + PageContext.PageUserID.ToString(), YAF.Classes.Core.UserMembershipHelper.GetUserNameFromID(Convert.ToInt64(userID)));
                }
            }
            else
            {
                outstring += string.Format(YafContext.Current.Localization.GetTextFormatted("ACTIVELOCATION", "ALBUMS"));
            }
            return outstring;
        }

        private string Profile(string forumPageAttributes)
        {
            string outstring = string.Empty;
            string userID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=") + 2);

            if (userID.Contains("&"))
            {
                userID = userID.Substring(0, userID.IndexOf("&")).Trim();
            }
            else
            {
                userID = userID.Substring(0).Trim();
            }
            if (ValidationHelper.IsValidInt(userID.Trim()))
            {
                if (Convert.ToInt32(userID) != UserID)
                {
                    outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "PROFILE_OFUSER"));
                    outstring += string.Format(@"<a href=""{0}"" id=""profileuserid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.profile, "u={0}", userID), userID + PageContext.PageUserID.ToString(), YAF.Classes.Core.UserMembershipHelper.GetUserNameFromID(Convert.ToInt64(userID)));
                }
                else
                {
                    outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "PROFILE_OWN"));
                }


            }
            else
            {
                outstring += string.Format(YafContext.Current.Localization.GetText("ACTIVELOCATION", "PROFILE"));
            }

            return outstring;
        }
    }
}