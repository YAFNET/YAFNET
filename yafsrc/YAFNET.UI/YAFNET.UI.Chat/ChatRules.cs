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
    /// Gets the text of a message as it is saved: without the spaces around it.
    /// </summary>
    /// <param name="message">The message (without html).</param>
    /// <returns>The text, or null if the message is empty (nothing is sent).</returns>
    public static string NormalizeMessage(string message)
    {
        var text = message?.Trim();

        return string.IsNullOrEmpty(text) ? null : text;
    }
}
