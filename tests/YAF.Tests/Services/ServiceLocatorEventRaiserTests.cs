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

namespace YAF.Tests.Services;

using System;
using System.Collections.Generic;

using NSubstitute;

using YAF.Core.Events;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Services;

/// <summary>
/// The <see cref="ServiceLocatorEventRaiser"/>.
/// </summary>
[TestFixture]
public class ServiceLocatorEventRaiserTests
{
    [Test]
    [Description("An isolated event is handled once by every handler, an exception of a handler is logged and the next handler still runs")]
    public void RaiseIssolated_Handles_Once_And_Isolates_Exceptions_Test()
    {
        var failing = new CountingHandler(1, fail: true);
        var working = new CountingHandler(2, fail: false);

        var serviceLocator = Substitute.For<IServiceLocator>();
        serviceLocator.Get(typeof(IEnumerable<IHandleEvent<TestEvent>>))
            .Returns(new IHandleEvent<TestEvent>[] { working, failing });
        serviceLocator.Get(typeof(IEnumerable<IFireEvent<TestEvent>>)).Returns(Array.Empty<IFireEvent<TestEvent>>());

        var raiser = new ServiceLocatorEventRaiser(serviceLocator, Substitute.For<ILoggerService>());

        var logged = new List<string>();

        raiser.Invoking(r => r.RaiseIssolated(new TestEvent(), (handler, _) => logged.Add(handler)))
            .Should().NotThrow();

        failing.Count.Should().Be(1);
        working.Count.Should().Be(1);
        logged.Should().ContainSingle().Which.Should().Be(nameof(CountingHandler));
    }

    public class TestEvent : IAmEvent;

    private class CountingHandler(int order, bool fail) : IHandleEvent<TestEvent>
    {
        public int Count { get; private set; }

        public int Order => order;

        public void Handle(TestEvent @event)
        {
            this.Count++;

            if (fail)
            {
                throw new InvalidOperationException("handler failed");
            }
        }
    }
}
