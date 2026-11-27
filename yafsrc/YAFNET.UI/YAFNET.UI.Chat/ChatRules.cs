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


namespace YAF.UI.Chat;

/// <summary>
/// The rules of the chat. No dependencies, so the unit tests can compile this file.
/// </summary>
public static class ChatRules
{
    /// <summary>
    /// The maximum length of a message (the message field has the same limit).
    /// </summary>
    public const int MaxMessageLength = 1000;

    /// <summary>
    /// The number of messages the chat page shows (the latest ones).
    /// </summary>
    public const int MessagesShown = 100;

    /// <summary>
    /// Messages of the same user within this time are shown as one group (one name and avatar).
    /// </summary>
    public static readonly System.TimeSpan GroupTime = System.TimeSpan.FromMinutes(5);

    /// <summary>
    /// Checks if a user can send chat messages. The chat page sends guests to the login page,
    /// but the hub can be called directly, so the hub checks it again.
    /// </summary>
    /// <param name="isGuest">If the user is the guest user (not logged in).</param>
    /// <param name="isSuspendedNow">If the user is suspended at the moment.</param>
    /// <returns>True if the user can send messages.</returns>
    public static bool CanSendMessages(bool isGuest, bool isSuspendedNow)
    {
        return !isGuest && !isSuspendedNow;
    }

    /// <summary>
    /// Gets the text of a message as it is saved: without the spaces around it and
    /// at most <see cref="MaxMessageLength"/> characters (the hub can be called without the message field).
    /// </summary>
    /// <param name="message">The message (without html).</param>
    /// <returns>The text, or null if the message is empty (nothing is sent).</returns>
    public static string NormalizeMessage(string message)
    {
        var text = message?.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return text.Length > MaxMessageLength ? text[..MaxMessageLength].TrimEnd() : text;
    }

    /// <summary>
    /// Checks if a message continues the group of the message before it:
    /// same user and sent within <see cref="GroupTime"/>.
    /// </summary>
    /// <param name="previousUserId">The user of the message before (null for the first message).</param>
    /// <param name="previousCreated">The time of the message before.</param>
    /// <param name="userId">The user of the message.</param>
    /// <param name="created">The time of the message.</param>
    /// <returns>True if the message is shown without name and avatar.</returns>
    public static bool ContinuesGroup(int? previousUserId, System.DateTime previousCreated, int userId, System.DateTime created)
    {
        return previousUserId == userId && created >= previousCreated && created - previousCreated <= GroupTime;
    }
}
