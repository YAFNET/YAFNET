/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services
{
    #region Using

    using System.Collections;
    using System.Collections.Generic;
    using System.Web;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// All references to session should go into this class
    /// </summary>
    public class Session : ISession
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="sessionState">
        /// The session state.
        /// </param>
        public Session([NotNull] HttpSessionStateBase sessionState)
        {
            CodeContracts.VerifyNotNull(sessionState, "sessionState");

            this.SessionState = sessionState;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user activity page size.
        /// </summary>
        public int? UserActivityPageSize
        {
            get => (int?)this.SessionState["UserActivityPageSize"];

            set => this.SessionState["UserActivityPageSize"] = value;
        }

        /// <summary>
        ///   Gets or sets UserTopicSince.
        /// </summary>
        public int? UserTopicSince
        {
            get => (int?)this.SessionState["UserTopicSince"];

            set => this.SessionState["UserTopicSince"] = value;
        }

        /// <summary>
        /// Gets or sets Twitter Token.
        /// </summary>
        public string TwitterToken
        {
            get => (string)this.SessionState["TwitterToken"];

            set => this.SessionState["TwitterToken"] = value;
        }

        /// <summary>
        /// Gets or sets Twitter Token Secret.
        /// </summary>
        public string TwitterTokenSecret
        {
            get => (string)this.SessionState["TwitterTokenSecret"];

            set => this.SessionState["TwitterTokenSecret"] = value;
        }

        /// <summary>
        /// Gets or sets the multi quote ids.
        /// </summary>
        /// <value>
        /// The multi quote ids.
        /// </value>
        public List<MultiQuote> MultiQuoteIds
        {
            get => this.SessionState["MultiQuoteIds"] as List<MultiQuote>;

            set => this.SessionState["MultiQuoteIds"] = value;
        }

        /// <summary>
        ///   Gets or sets UnreadTopicSince.
        /// </summary>
        public int? UnreadTopicSince
        {
            get => (int?)this.SessionState["UnreadTopicSince"];

            set => this.SessionState["UnreadTopicSince"] = value;
        }

        /// <summary>
        ///   Gets or sets ActiveTopicSince.
        /// </summary>
        public int? ActiveTopicSince
        {
            get => (int?)this.SessionState["ActiveTopicSince"];

            set => this.SessionState["ActiveTopicSince"] = value;
        }

        /// <summary>
        ///   Gets or sets UnansweredTopicSince.
        /// </summary>
        public int? UnansweredTopicSince
        {
            get => (int?)this.SessionState["UnansweredTopicSince"];

            set => this.SessionState["UnansweredTopicSince"] = value;
        }

        /// <summary>
        ///   Gets or sets FavoriteTopicSince.
        /// </summary>
        public int? FavoriteTopicSince
        {
            get => (int?)this.SessionState["FavoriteTopicSince"];

            set => this.SessionState["FavoriteTopicSince"] = value;
        }

        /// <summary>
        ///   Gets or sets ForumRead.
        /// </summary>
        public Hashtable ForumRead
        {
            get => (Hashtable)this.SessionState["forumread"];

            set => this.SessionState["forumread"] = value;
        }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        public System.DateTime LastPendingBuddies
        {
            get
            {
                if (this.SessionState["lastpendingbuddies"] != null)
                {
                    return (System.DateTime)this.SessionState["lastpendingbuddies"];
                }

                return DateTimeHelper.SqlDbMinTime();
            }

            set => this.SessionState["lastpendingbuddies"] = value;
        }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        public System.DateTime LastPm
        {
            get
            {
                if (this.SessionState["lastpm"] != null)
                {
                    return (System.DateTime)this.SessionState["lastpm"];
                }

                return DateTimeHelper.SqlDbMinTime();
            }

            set => this.SessionState["lastpm"] = value;
        }

        /// <summary>
        ///   Gets or sets LastPost.
        /// </summary>
        public System.DateTime LastPost
        {
            get
            {
                if (this.SessionState["lastpost"] != null)
                {
                    return (System.DateTime)this.SessionState["lastpost"];
                }

                return DateTimeHelper.SqlDbMinTime();
            }

            set => this.SessionState["lastpost"] = value;
        }

        /// <summary>
        ///   Gets or sets LastVisit.
        /// </summary>
        public System.DateTime? LastVisit
        {
            get => (System.DateTime?)this.SessionState["lastvisit"];

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
        public IPanelSessionState PanelState => new PanelSessionState();

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

            set => this.SessionState["showlist"] = value;
        }

        /// <summary>
        ///   Gets or sets TopicRead.
        /// </summary>
        public Hashtable TopicRead
        {
            get => (Hashtable)this.SessionState["topicread"];

            set => this.SessionState["topicread"] = value;
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

            set => this.SessionState["unreadtopics"] = value;
        }

        #endregion

        #region Implemented Interfaces

        #region IYafSession

        /// <summary>
        /// Gets the last time the forum was read.
        /// </summary>
        /// <param name="forumId">
        /// This is the ID of the forum you wish to get the last read date from.
        /// </param>
        /// <returns>
        /// A DateTime object of when the forum was last read.
        /// </returns>
        public System.DateTime GetForumRead(int forumId)
        {
            var t = this.ForumRead;
            if (t == null || !t.ContainsKey(forumId))
            {
                return this.LastVisit ?? DateTimeHelper.SqlDbMinTime();
            }

            return (System.DateTime)t[forumId];
        }

        /// <summary>
        /// Returns the last time that the topicID was read.
        /// </summary>
        /// <param name="topicId">
        /// The topicID you wish to find the DateTime object for.
        /// </param>
        /// <returns>
        /// The DateTime object from the topicID.
        /// </returns>
        public System.DateTime GetTopicRead(int topicId)
        {
            var t = this.TopicRead;

            if (t == null || !t.ContainsKey(topicId))
            {
                return this.LastVisit ?? DateTimeHelper.SqlDbMinTime();
            }

            return (System.DateTime)t[topicId];
        }

        /// <summary>
        /// Sets the time that the forum was read.
        /// </summary>
        /// <param name="forumId">
        /// The forum ID that was read.
        /// </param>
        /// <param name="date">
        /// The DateTime you wish to set the read to.
        /// </param>
        public void SetForumRead(int forumId, System.DateTime date)
        {
            var t = this.ForumRead ?? new Hashtable();

            t[forumId] = date;
            this.ForumRead = t;
        }

        /// <summary>
        /// Sets the time that the <paramref name="topicId"/> was read.
        /// </summary>
        /// <param name="topicId">
        /// The topic ID that was read.
        /// </param>
        /// <param name="date">
        /// The DateTime you wish to set the read to.
        /// </param>
        public void SetTopicRead(int topicId, System.DateTime date)
        {
            var t = this.TopicRead ?? new Hashtable();

            t[topicId] = date;
            this.TopicRead = t;
        }

        #endregion

        #endregion
    }
}