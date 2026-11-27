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

using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ServiceStack.OrmLite;

using YAF.Core.Context;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

namespace YAF.UI.Chat.Areas.Chat.Pages;

/// <summary>
/// Class ChatModel.
/// Implements the <see cref="PageModel" />
/// Implements the <see cref="IHaveServiceLocator" />
/// </summary>
/// <seealso cref="PageModel" />
/// <seealso cref="IHaveServiceLocator" />
public class ChatModel : PageModel, IHaveServiceLocator
{
    /// <summary>
    /// Gets the service locator.
    /// </summary>
    /// <value>The service locator.</value>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Gets the latest chat messages with their users, the oldest first.
    /// </summary>
    public List<Tuple<ChatMessage, User>> Messages { get; private set; } = [];

    /// <summary>
    /// Gets a value indicating whether the current user can send messages (a suspended user can only read).
    /// </summary>
    public bool CanSend { get; private set; }

    /// <summary>
    /// Shows the chat (members only).
    /// </summary>
    /// <returns>The page, or the login page for guests.</returns>
    public IActionResult OnGet()
    {
        if (BoardContext.Current.IsGuest)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Account_Login, new { area = "Forums" });
        }

        this.CanSend = ChatRules.CanSendMessages(BoardContext.Current.IsGuest, BoardContext.Current.IsSuspendedNow);

        // Inject Table
        this.Get<IDbAccess>().Execute(db => db.Connection.CreateTableIfNotExists<ChatMessage>());

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<ChatMessage>();

        // the latest messages (all messages of all years were loaded, in the order of the database)
        expression.Join<ChatMessage, User>((msg, usr) => msg.UserId == usr.ID)
            .Where<ChatMessage>(msg => msg.BoardId == BoardContext.Current.BoardSettings.BoardId)
            .OrderByDescending<ChatMessage>(msg => msg.Created)
            .Limit(ChatRules.MessagesShown);

        var latest = this.GetRepository<ChatMessage>().DbAccess.Execute(
            db => db.Connection.SelectMulti<ChatMessage, User>(expression));

        latest.Reverse();

        this.Messages = latest;

        return this.Page();
    }
}
