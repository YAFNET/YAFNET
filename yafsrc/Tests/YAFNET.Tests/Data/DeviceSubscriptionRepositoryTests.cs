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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Data;

using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models;

/// <summary>
/// The push subscriptions of the devices (<see cref="DeviceSubscriptionRepositoryExtensions"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class DeviceSubscriptionRepositoryTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A renewed subscription of the same device replaces the endpoint and the keys")]
    public async Task RenewedSubscriptionIsUpdated()
    {
        var repository = this.board.Current.GetRepository<DeviceSubscription>();

        await repository.AddSubscriptionAsync(Subscription("https://fcm.googleapis.com/fcm/send/old", "old"));
        await repository.AddSubscriptionAsync(Subscription("https://fcm.googleapis.com/fcm/send/new", "new"));

        var subscriptions = await repository.GetSubscriptionsByUserAsync(this.board.PageUser.ID);

        subscriptions.Should().ContainSingle();
        subscriptions[0].Should().BeEquivalentTo(
            new { EndPoint = "https://fcm.googleapis.com/fcm/send/new", P256dh = "new", Auth = "new", BoardID = TestDatabase.BoardId });
    }

    [Test]
    [Description("The user and board come from the context, not from the posted subscription")]
    public async Task UserAndBoardAreTheCurrentOnes()
    {
        var repository = this.board.Current.GetRepository<DeviceSubscription>();

        var subscription = Subscription("https://fcm.googleapis.com/fcm/send/x", "x");
        subscription.UserID = 999;
        subscription.BoardID = 999;

        await repository.AddSubscriptionAsync(subscription);

        (await repository.GetSubscriptionsByUserAsync(this.board.PageUser.ID)).Should().ContainSingle();
        (await repository.GetSubscriptionsByUserAsync(999)).Should().BeEmpty();
    }

    private static DeviceSubscription Subscription(string endpoint, string key)
    {
        return new DeviceSubscription
               {
                   Device = "Mozilla/*",
                   UserAgent = "Mozilla/5.0",
                   EndPoint = endpoint,
                   P256dh = key,
                   Auth = key
               };
    }
}
