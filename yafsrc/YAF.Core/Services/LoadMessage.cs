/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
public class LoadMessage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "LoadMessage" /> class.
    /// </summary>
    public LoadMessage()
    {
        if (!this.SessionLoadString.NullOrEmpty())
        {
            this.LoadStringList.AddRange(this.SessionLoadString);

            // session load string no longer needed
            this.SessionLoadString.Clear();
        }

        BoardContext.Current.Unload += this.CurrentUnload;
    }

    /// <summary>
    ///   Gets LoadStringList.
    /// </summary>
    [NotNull]
    public List<MessageNotification> LoadStringList { get; } = new ();

    /// <summary>
    /// Gets the session load string.
    /// </summary>
    protected List<MessageNotification> SessionLoadString
    {
        get
        {
            if (BoardContext.Current.Get<IDataCache>().Get("LoadStringList") == null)
            {
                BoardContext.Current.Get<IDataCache>().Set(
                    "LoadStringList",
                    new List<MessageNotification>(),
                    TimeSpan.FromMinutes(30));
            }

            return BoardContext.Current.Get<IDataCache>().Get("LoadStringList") as List<MessageNotification>;
        }
    }

    /// <summary>
    /// Creates a message that will be returned on the next page load.
    /// </summary>
    /// <param name="message">The message you wish to display.</param>
    /// <param name="messageType">Type of the message.</param>
    public void Add([NotNull] string message, MessageTypes messageType)
    {
        this.LoadStringList.Add(new MessageNotification(message, messageType));
    }

    /// <summary>
    /// AddLoadMessageSession creates a message that will be returned on the next page.
    /// </summary>
    /// <param name="message">The message you wish to display.</param>
    /// <param name="messageType">Type of the message.</param>
    public void AddSession([NotNull] string message, MessageTypes messageType)
    {
        // add it too the session list...
        this.SessionLoadString.Add(new MessageNotification(message, messageType));
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

        return !this.LoadStringList.Any()
                   ? null
                   : this.LoadStringList.First();
    }

    /// <summary>
    /// Clear the load message...
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void CurrentUnload([NotNull] object sender, [NotNull] EventArgs e)
    {
        // clear the load message...
        this.Clear();
    }
}