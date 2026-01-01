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

namespace YAF.Core.Filters;

using System;
using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The suspend check module.
/// Implements the <see cref="ResultFilterAttribute" />
/// Implements the <see cref="IHaveServiceLocator" />
/// </summary>
/// <seealso cref="ResultFilterAttribute" />
/// <seealso cref="IHaveServiceLocator" />
[AttributeUsage(AttributeTargets.Class)]
public class UserSuspendCheckAttribute : ResultFilterAttribute, IHaveServiceLocator
{
    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// On result execution as an asynchronous operation.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <inheritdoc />
    public async override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // check for suspension if enabled...
        if (BoardContext.Current.Globals.IsSuspendCheckEnabled && BoardContext.Current.IsSuspended)
        {
            if (this.Get<IDateTimeService>().GetUserDateTime(BoardContext.Current.SuspendedUntil)
                <= this.Get<IDateTimeService>().GetUserDateTime(DateTime.UtcNow))
            {
                await this.GetRepository<User>().SuspendAsync(BoardContext.Current.PageUserID);

                await this.Get<ISendNotification>().SendUserSuspensionEndedNotificationAsync(
                    BoardContext.Current.PageUser.Email,
                    BoardContext.Current.PageUser.DisplayOrUserName());

                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(BoardContext.Current.PageUserID));
            }
            else
            {
                context.Result = this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Suspended);
            }
        }

        await next.Invoke();
    }
}