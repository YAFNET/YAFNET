/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Collections.Generic;

using YAF.Types.Constants;
using YAF.Types.Objects;

/// <summary>
/// The load message.
/// </summary>
public class SessionMessageService
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "SessionMessageService" /> class.
    /// </summary>
    public SessionMessageService()
    {
        var pending = ReadPending();

        if (pending.Count != 0)
        {
            this.LoadStringList.AddRange(pending);

            // the messages are shown on this page, they are no longer needed
            Session?.Remove(SessionKey);
        }

        BoardContext.Current.Unload += this.CurrentUnload;
    }

    /// <summary>
    ///   Gets LoadStringList.
    /// </summary>
    public List<MessageNotification> LoadStringList { get; } = [];

    /// <summary>
    /// The session key of the messages for the next page of this user.
    /// </summary>
    private const string SessionKey = "SessionMessages";

    /// <summary>
    /// Gets the session of the current user (the messages are per user, never per IP address,
    /// several users can share one address), or <c>null</c> without a session.
    /// </summary>
    private static ISession Session
    {
        get
        {
            try
            {
                return BoardContext.Current.Get<IHttpContextAccessor>().HttpContext?.Session;
            }
            catch (InvalidOperationException)
            {
                // no session configured
                return null;
            }
        }
    }

    /// <summary>
    /// Reads the messages stored for the next page.
    /// </summary>
    /// <returns>Returns the messages</returns>
    private static List<MessageNotification> ReadPending()
    {
        return Session?.GetData<List<MessageNotification>>(SessionKey) ?? [];
    }

    /// <summary>
    /// AddLoadMessageSession creates a message that will be returned on the next page.
    /// </summary>
    /// <param name="message">The message you wish to display.</param>
    /// <param name="messageType">Type of the message.</param>
    public void AddSession(string message, MessageTypes messageType)
    {
        var session = Session;

        if (session is null)
        {
            return;
        }

        // add it to the session list...
        var pending = ReadPending();
        pending.Add(new MessageNotification(message, messageType));

        session.SetData(SessionKey, pending);
    }

    /// <summary>
    /// Clear the Load String (error) List
    /// </summary>
    public void Clear()
    {
        this.LoadStringList.Clear();
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <returns>Returns the Current Message</returns>
    public MessageNotification GetMessage()
    {
        if (this.LoadStringList == null)
        {
            return null;
        }

        return !this.LoadStringList.HasItems()
                   ? null
                   : this.LoadStringList[0];
    }

    /// <summary>
    /// Clear the load message...
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void CurrentUnload(object sender, EventArgs e)
    {
        // clear the load message...
        this.Clear();
    }
}