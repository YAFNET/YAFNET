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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace YAF.Core.Model;

using YAF.Types.Models;

/// <summary>
/// The DeviceSubscription repository extensions.
/// </summary>
public static class DeviceSubscriptionRepositoryExtensions
{
    /// <param name="repository">The repository.</param>
    extension(IRepository<DeviceSubscription> repository)
    {
        /// <summary>
        /// Adds new push subscription for that user and device (if not exist).
        /// </summary>
        /// <param name="deviceSubscription">The device subscription.</param>
        public async Task AddSubscriptionAsync(DeviceSubscription deviceSubscription )
        {
            if (await repository.ExistsAsync(x =>
                    x.UserID == BoardContext.Current.PageUserID && x.BoardID == BoardContext.Current.PageBoardID &&
                    x.Device == deviceSubscription.Device))
            {
                return;
            }

            deviceSubscription.BoardID = BoardContext.Current.PageBoardID;
            deviceSubscription.UserID = BoardContext.Current.PageUserID;

            await repository.InsertAsync(deviceSubscription);
        }

        /// <summary>
        /// Deletes the existing push subscription for that user and device (if exist).
        /// </summary>
        /// <param name="userAgent">The Useragent.</param>
        public async Task DeleteSubscriptionAsync(string userAgent)
        {
            if (await repository.ExistsAsync(x =>
                    x.UserID == BoardContext.Current.PageUserID && x.BoardID == BoardContext.Current.PageBoardID &&
                    x.Device == userAgent))
            {
                await repository.DeleteAsync(x =>
                    x.UserID == BoardContext.Current.PageUserID && x.BoardID == BoardContext.Current.PageBoardID &&
                    x.Device == userAgent);
            }
        }

        /// <summary>
        /// Gets the subscriptions by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<List<DeviceSubscription>> GetSubscriptionsByUserAsync(int userId)
        {
            return await repository.GetAsync(d => d.BoardID == repository.BoardID && d.UserID == userId);
        }
    }
}