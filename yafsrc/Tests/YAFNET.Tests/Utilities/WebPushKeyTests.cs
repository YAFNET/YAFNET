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

namespace YAF.Tests.Utilities;

using WebEssentials.AspNetCore.Pwa.WebPush;

/// <summary>
/// The VAPID keys of the web push: url safe base64, usually without padding.
/// </summary>
[TestFixture]
public class WebPushKeyTests
{
    [Test]
    [Description("Generated keys (url safe base64 without padding) are valid")]
    public void Generated_Keys_Are_Valid_Test()
    {
        var keys = VapidHelper.GenerateVapidKeys();

        keys.PublicKey.Should().HaveLength(87).And.NotContain("=");
        keys.PrivateKey.Should().HaveLength(43).And.NotContain("=");

        FluentActions.Invoking(() => VapidHelper.ValidatePublicKey(keys.PublicKey)).Should().NotThrow();
        FluentActions.Invoking(() => VapidHelper.ValidatePrivateKey(keys.PrivateKey)).Should().NotThrow();
    }

    [Test]
    [Description("Keys with padding are valid too")]
    public void Padded_Keys_Are_Valid_Test()
    {
        var keys = VapidHelper.GenerateVapidKeys();

        FluentActions.Invoking(() => VapidHelper.ValidatePublicKey(keys.PublicKey + "=")).Should().NotThrow();
        FluentActions.Invoking(() => VapidHelper.ValidatePrivateKey(keys.PrivateKey + "=")).Should().NotThrow();
    }

    [Test]
    [Description("A key of the wrong length is refused")]
    public void Wrong_Key_Length_Test()
    {
        var keys = VapidHelper.GenerateVapidKeys();

        FluentActions.Invoking(() => VapidHelper.ValidatePublicKey(keys.PrivateKey)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => VapidHelper.ValidatePrivateKey(keys.PublicKey)).Should().Throw<ArgumentException>();
    }
}
