/* Yet Another Forum.net
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// All references to session should go into this class
    /// </summary>
    public class YafSession : IYafSession
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSession"/> class.
        /// </summary>
        /// <param name="sessionState">
        /// The session state.
        /// </param>
        public YafSession([NotNull] HttpSessionStateBase sessionState)
        {
            CodeContracts.VerifyNotNull(sessionState, "sessionState");

            this.SessionState = sessionState;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets UserTopicSince.
        /// </summary>
        public int? UserTopicSince
        {
            get
            {
                if (this.SessionState["UserTopicSince"] != null)
                {
                    return (int)this.SessionState["UserTopicSince"];
                }

                return null;
            }

            set
            {
                this.SessionState["UserTopicSince"] = value;
            }
        }

        /// <summary>
        /// Gets or sets Twitter Token.
        /// </summary>
        public string TwitterToken
        {
            get
            {
                if (this.SessionState["TwitterToken"] != null)
                {
                    return (string)this.SessionState["TwitterToken"];
                }

                return null;
            }

            set
            {
                this.SessionState["TwitterToken"] = value;
            }
        }

        /// <summary>
        /// Gets or sets Twitter Token Secret.
        /// </summary>
        public string TwitterTokenSecret
        {
            get
            {
                if (this.SessionState["TwitterTokenSecret"] != null)
                {
                    return (string)this.SessionState["TwitterTokenSecret"];
                }

                return null;
            }

            set
            {
                this.SessionState["TwitterTokenSecret"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the multi quote ids.
        /// </summary>
        /// <value>
        /// The multi quote ids.
        /// </value>
        public List<int> MultiQuoteIds
        {
            get
            {
                if (this.SessionState["MultiQuoteIds"] != null)
                {
                    return this.SessionState["MultiQuoteIds"] as List<int>;
                }

                return null;
            }

            set
            {
                this.SessionState["MultiQuoteIds"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets UnreadTopicSince.
        /// </summary>
        public int? UnreadTopicSince
        {
            get
            {
                if (this.SessionState["UnreadTopicSince"] != null)
                {
                    return (int)this.SessionState["UnreadTopicSince"];
                }

                return null;
            }

            set
            {
                this.SessionState["UnreadTopicSince"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets ActiveTopicSince.
        /// </summary>
        public int? ActiveTopicSince
        {
            get
            {
                if (this.SessionState["ActiveTopicSince"] != null)
                {
                    return (int)this.SessionState["ActiveTopicSince"];
                }

                return null;
            }

            set
            {
                this.SessionState["ActiveTopicSince"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets UnansweredTopicSince.
        /// </summary>
        public int? UnansweredTopicSince
        {
            get
            {
                if (this.SessionState["UnansweredTopicSince"] != null)
                {
                    return (int)this.SessionState["UnansweredTopicSince"];
                }

                return null;
            }

            set
            {
                this.SessionState["UnansweredTopicSince"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets FavoriteTopicSince.
        /// </summary>
        public int? FavoriteTopicSince
        {
            get
            {
                if (this.SessionState["FavoriteTopicSince"] != null)
                {
                    return (int)this.SessionState["FavoriteTopicSince"];
                }

                return null;
            }

            set
            {
                this.SessionState["FavoriteTopicSince"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets ForumRead.
        /// </summary>
        public Hashtable ForumRead
        {
            get
            {
                return this.SessionState["forumread"] != null ? (Hashtable)this.SessionState["forumread"] : null;
            }

            set
            {
                this.SessionState["forumread"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        public DateTime LastPendingBuddies
        {
            get
            {
                if (this.SessionState["lastpendingbuddies"] != null)
                {
                    return (DateTime)this.SessionState["lastpendingbuddies"];
                }

                return DateTimeHelper.SqlDbMinTime();
            }

            set
            {
                this.SessionState["lastpendingbuddies"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        public DateTime LastPm
        {
            get
            {
                if (this.SessionState["lastpm"] != null)
                {
                    return (DateTime)this.SessionState["lastpm"];
                }

                return DateTimeHelper.SqlDbMinTime();
            }

            set
            {
                this.SessionState["lastpm"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets LastPost.
        /// </summary>
        public DateTime LastPost
        {
            get
            {
                if (this.SessionState["lastpost"] != null)
                {
                    return (DateTime)this.SessionState["lastpost"];
                }

                return DateTimeHelper.SqlDbMinTime();
            }

            set
            {
                this.SessionState["lastpost"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets LastVisit.
        /// </summary>
        public DateTime? LastVisit
        {
            get
            {
                if (this.SessionState["lastvisit"] != null)
                {
                    return (DateTime?)this.SessionState["lastvisit"];
                }

                return null;
            }

            set
            {
                if (value == DateTimeHelper.SqlDbMinTime() && this.SessionState["lastvisit"] != null)
                {
                    this.SessionState.Remove("lastvisit");
                }
                else
                {
                    this.SessionState["lastvisit"] = value;
                }
            }
        }

        /// <summary>
        ///   Gets PanelState.
        /// </summary>
        [NotNull]
        public IPanelSessionState PanelState
        {
            get
            {
                // TODO: Get rid of this and replace with DI.
                return new PanelSessionState();
            }
        }

        /// <summary>
        ///   Gets or sets SearchData.
        /// </summary>
        [CanBeNull]
        public SearchResult[] SearchData
        {
            get
            {
                if (this.SessionState["SearchResults"] != null)
                {
                    return this.SessionState["SearchResults"] as SearchResult[];
                }

                return null;
            }

            set
            {
                this.SessionState["SearchResults"] = value;
            }
        }

        /// <summary>
        /// Gets or sets SessionState.
        /// </summary>
        public HttpSessionStateBase SessionState { get; set; }

        /// <summary>
        ///   Gets or sets ShowList.
        /// </summary>
        public int ShowList
        {
            get
            {
                if (this.SessionState["showlist"] != null)
                {
                    return (int)this.SessionState["showlist"];
                }

                // nothing in session
                return -1;
            }

            set
            {
                this.SessionState["showlist"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets TopicRead.
        /// </summary>
        public Hashtable TopicRead
        {
            get
            {
                if (this.SessionState["topicread"] != null)
                {
                    return (Hashtable)this.SessionState["topicread"];
                }

                return null;
            }

            set
            {
                this.SessionState["topicread"] = value;
            }
        }

        /// <summary>
        /// Gets or sets UnreadTopics.
        /// </summary>
        public int UnreadTopics
        {
            get
            {
                if (this.SessionState["unreadtopics"] != null)
                {
                    return (int)this.SessionState["unreadtopics"];
                }

                return 0;
            }

            set
            {
                this.SessionState["unreadtopics"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets if the user wants to use the mobile theme.
        /// </summary>
        public bool? UseMobileTheme
        {
            get
            {
                if (this.SessionState["UseMobileTheme"] == null)
                {
                    return null;
                }

                return (bool)this.SessionState["UseMobileTheme"];
            }

            set
            {
                this.SessionState["UseMobileTheme"] = value;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IYafSession

        /// <summary>
        /// Gets the last time the forum was read.
        /// </summary>
        /// <param name="forumID">
        /// This is the ID of the forum you wish to get the last read date from.
        /// </param>
        /// <returns>
        /// A DateTime object of when the forum was last read.
        /// </returns>
        public DateTime GetForumRead(int forumID)
        {
            Hashtable t = this.ForumRead;
            if (t == null || !t.ContainsKey(forumID))
            {
                return this.LastVisit ?? DateTimeHelper.SqlDbMinTime();
            }

            return (DateTime)t[forumID];
        }

        /// <summary>
        /// Returns the last time that the topicID was read.
        /// </summary>
        /// <param name="topicID">
        /// The topicID you wish to find the DateTime object for.
        /// </param>
        /// <returns>
        /// The DateTime object from the topicID.
        /// </returns>
        public DateTime GetTopicRead(int topicID)
        {
            Hashtable t = this.TopicRead;

            if (t == null || !t.ContainsKey(topicID))
            {
                return this.LastVisit ?? DateTimeHelper.SqlDbMinTime();
            }

            return (DateTime)t[topicID];
        }

        /// <summary>
        /// Sets the time that the forum was read.
        /// </summary>
        /// <param name="forumID">
        /// The forum ID that was read.
        /// </param>
        /// <param name="date">
        /// The DateTime you wish to set the read to.
        /// </param>
        public void SetForumRead(int forumID, DateTime date)
        {
            Hashtable t = this.ForumRead ?? new Hashtable();

            t[forumID] = date;
            this.ForumRead = t;
        }

        /// <summary>
        /// Sets the time that the <paramref name="topicID"/> was read.
        /// </summary>
        /// <param name="topicID">
        /// The topic ID that was read.
        /// </param>
        /// <param name="date">
        /// The DateTime you wish to set the read to.
        /// </param>
        public void SetTopicRead(int topicID, DateTime date)
        {
            Hashtable t = this.TopicRead ?? new Hashtable();

            t[topicID] = date;
            this.TopicRead = t;
        }

        #endregion

        #endregion
    }
}