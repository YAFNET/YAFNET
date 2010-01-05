/* Yet Another Forum.NET
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
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render(HtmlTextWriter output)
        {
            string forumPageName = this.ForumPage;
            if (string.IsNullOrEmpty(forumPageName))
            {
                forumPageName = "MAINPAGE";
            }
            else
            {
                forumPageName = forumPageName.TrimStart("g=".ToCharArray());
                if (forumPageName.Contains("&"))
                {
                    forumPageName = forumPageName.Substring(0, forumPageName.IndexOf("&"));
                }
            }

            if (string.IsNullOrEmpty(forumPageName))
            {
                forumPageName = YafContext.Current.Localization.GetText("ACTIVELOCATION", "NODATA");
            }

                     output.BeginRender();              
                
                     if (this.TopicID > 0 && this.ForumID > 0)
                     {
                         output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "TOPICS"));
                         output.Write(@"<a href=""{0}"" id=""topicid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.topics, "t={0}", this.TopicID), this.UserID, this.TopicName);
                         if (!this.LastLinkOnly)
                         {
                             output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "TOPICINFORUM"));
                             output.Write(@"<a href=""{0}"" id=""forumidtopic_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.forum, "f={0}", this.ForumID), this.UserID, this.ForumName);
                         }
                     }
                     else if (this.ForumID > 0 && this.TopicID <= 0)
                     {
                        output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "FORUM"));
                        output.Write(@"<a href=""{0}"" id=""forumid_{1}"" runat=""server""> {2} </a>", YafBuildLink.GetLink(ForumPages.forum, "f={0}", this.ForumID), this.UserID, this.ForumName);
                     }                  
                     else
                      {
                          if (forumPageName == "forum" && this.TopicID <= 0 && this.ForumID <= 0)
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
                          else if (!YafContext.Current.IsHostAdmin && forumPageName.Contains("MODERATE_"))
                          {
                              output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "MODERATE"));
                          }
                          else if (!YafContext.Current.IsHostAdmin && forumPageName.Contains("ADMIN_"))
                          {
                              output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", "ADMINTASK"));
                          }
                          else
                          {
                              output.Write(YafContext.Current.Localization.GetText("ACTIVELOCATION", forumPageName));
                          }
                      } 

            output.EndRender();           
        }
    }
}