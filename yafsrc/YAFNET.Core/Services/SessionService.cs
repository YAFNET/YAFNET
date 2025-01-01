/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Services;

using System;
using System.Collections;
using System.Collections.Generic;

using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// All references to session should go into this class
/// </summary>
public class SessionService : ISessionService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionService"/> class.
    /// </summary>
    /// <param name="contextAccessor">
    /// The context Accessor.
    /// </param>
    public SessionService(IHttpContextAccessor contextAccessor)
    {
        this.SessionState = contextAccessor.HttpContext.Session;
    }

    /// <summary>
    /// Gets or sets the index of the board forums.
    /// </summary>
    /// <value>The index of the board forums.</value>
    public int BoardForumsIndex
    {
        get => this.SessionState.GetData<int>("BoardForumsIndex");

        set => this.SessionState.SetData("BoardForumsIndex", value);
    }

    /// <summary>
    /// Gets or sets the mods.
    /// </summary>
    /// <value>The mods.</value>
    public List<SimpleModerator> Mods
    {
        get => this.SessionState.GetData<List<SimpleModerator>>("Mods");

        set => this.SessionState.SetData("Mods", value);
    }

    /// <summary>
    /// Gets or sets the forums.
    /// </summary>
    /// <value>The forums.</value>
    public List<ForumRead> Forums
    {
        get => this.SessionState.GetData<List<ForumRead>>("Forums");

        set => this.SessionState.SetData("Forums", value);
    }

    /// <summary>
    /// Gets or sets the multi quote ids.
    /// </summary>
    /// <value>
    /// The multi quote ids.
    /// </value>
    public List<MultiQuote> MultiQuoteIds
    {
        get => this.SessionState.GetData<List<MultiQuote>>("MultiQuoteIds");

        set => this.SessionState.SetData("MultiQuoteIds", value);
    }

    /// <summary>
    ///   Gets or sets ForumRead.
    /// </summary>
    public Hashtable ForumRead
    {
        get => this.SessionState.GetData<Hashtable>("forumread");

        set => this.SessionState.SetData("forumread", value);
    }

    /// <summary>
    ///   Gets or sets LastPm.
    /// </summary>
    public DateTime LastPendingBuddies
    {
        get => this.SessionState.GetString("lastpendingbuddies").IsSet() ? this.SessionState.GetData<DateTime>("lastpendingbuddies") : DateTimeHelper.SqlDbMinTime();

        set => this.SessionState.SetData("lastpendingbuddies", value);
    }

    /// <summary>
    ///   Gets or sets LastVisit.
    /// </summary>
    public DateTime? LastVisit
    {
        get => this.SessionState.GetData<DateTime?>("lastvisit");

        set
        {
            if (value == DateTimeHelper.SqlDbMinTime() && this.SessionState.GetData<DateTime?>("lastvisit") != null)
            {
                this.SessionState.Remove("lastvisit");
            }
            else
            {
                this.SessionState.SetData("lastvisit", value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the info message.
    /// </summary>
    public string InfoMessage
    {
        get => this.SessionState.GetString("InfoMessage");

        set => this.SessionState.SetData("InfoMessage", value);
    }

    /// <summary>
    ///   Gets PanelState.
    /// </summary>
    public IPanelSessionState PanelState => new PanelSessionState();

    /// <summary>
    /// Gets or sets SessionState.
    /// </summary>
    public ISession SessionState { get; set; }

    /// <summary>
    /// Gets the page data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public T GetPageData<T>()
    {
        return this.SessionState.GetData<T>($"PageData_{BoardContext.Current.PageUserID}");
    }

    /// <summary>
    /// Sets the page data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newData">The new data.</param>
    public void SetPageData<T>(T newData)
    {
        this.SessionState.SetData($"PageData_{BoardContext.Current.PageUserID}", newData);
    }

    /// <summary>
    /// Clears the page data.
    /// </summary>
    public void ClearPageData()
    {
        this.SessionState.Remove($"PageData_{BoardContext.Current.PageUserID}");
    }

    /// <summary>
    ///   Gets or sets TopicRead.
    /// </summary>
    public Hashtable TopicRead
    {
        get => this.SessionState.GetData<Hashtable>("topicread");

        set => this.SessionState.SetData("topicread", value);
    }

    /// <summary>
    /// Gets the last time the forum was read.
    /// </summary>
    /// <param name="forumId">
    /// This is the ID of the forum you wish to get the last read date from.
    /// </param>
    /// <returns>
    /// A DateTime object of when the forum was last read.
    /// </returns>
    public DateTime GetForumRead(int forumId)
    {
        var t = this.ForumRead;
        if (t == null || !t.ContainsKey(forumId))
        {
            return this.LastVisit ?? DateTimeHelper.SqlDbMinTime();
        }

        return (DateTime)t[forumId];
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
    public DateTime GetTopicRead(int topicId)
    {
        var t = this.TopicRead;

        if (t == null || !t.ContainsKey(topicId))
        {
            return this.LastVisit ?? DateTimeHelper.SqlDbMinTime();
        }

        return (DateTime)t[topicId];
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
    public void SetForumRead(int forumId, DateTime date)
    {
        var t = this.ForumRead ?? [];

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
    public void SetTopicRead(int topicId, DateTime date)
    {
        var t = this.TopicRead ?? [];

        t[topicId] = date;
        this.TopicRead = t;
    }
}