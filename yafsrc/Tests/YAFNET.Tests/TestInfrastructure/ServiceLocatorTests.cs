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

namespace YAF.Tests.TestInfrastructure;

using Autofac;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using YAF.Core;
using YAF.Types.Attributes;
using YAF.Types.Interfaces;

using NamedParameter = YAF.Types.Objects.NamedParameter;
using TypedParameter = YAF.Types.Objects.TypedParameter;

/// <summary>
/// The Autofac based <see cref="AutoFacServiceLocatorProvider"/> and the generic <see cref="IServiceLocatorExtensions"/>.
/// </summary>
[TestFixture]
public class ServiceLocatorTests
{
    private IContainer container;

    private AutoFacServiceLocatorProvider locator;

    [SetUp]
    public void SetUp()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<Greeter>().AsSelf();
        builder.Register(_ => new Greeter("named")).Named<Greeter>("special");
        builder.RegisterType<Clock>().AsSelf().SingleInstance();
        builder.RegisterInstance(NullLoggerProvider.Instance).As<ILoggerProvider>();
        builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().InstancePerLifetimeScope();

        this.container = builder.Build();
        this.locator = this.container.Resolve<AutoFacServiceLocatorProvider>();
    }

    [TearDown]
    public void TearDown()
    {
        this.container.Dispose();
    }

    [Test]
    [Description("Services are resolved by type, by name and with named or typed constructor parameters")]
    public void Get_Test()
    {
        IServiceLocator services = this.locator;

        services.Get<Clock>().Should().BeSameAs(services.Get<Clock>());
        services.Get<Greeter>("special").Text.Should().Be("named");
        services.Get<Greeter>([new NamedParameter("text", "by name")]).Text.Should().Be("by name");
        services.Get<Greeter>([new TypedParameter(typeof(string), "by type")]).Text.Should().Be("by type");
        services.Get<Greeter>("special", []).Text.Should().Be("named");

        services.Invoking(s => s.Get<Greeter>([new OtherParameter()])).Should().Throw<NotSupportedException>();
        services.Invoking(s => s.Get(null)).Should().Throw<ArgumentNullException>();
        services.Invoking(s => s.Get(typeof(Greeter), (string)null)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("TryGet and GetService return nothing for services that are not registered")]
    public void Try_Get_Test()
    {
        IServiceLocator services = this.locator;

        services.TryGet<Clock>(out var clock).Should().BeTrue();
        clock.Should().NotBeNull();

        services.TryGet<IDisposable>(out var missing).Should().BeFalse();
        missing.Should().BeNull();

        services.TryGet<Greeter>("special", out var named).Should().BeTrue();
        named.Text.Should().Be("named");
        services.TryGet<Greeter>("unknown", out _).Should().BeFalse();

        this.locator.GetService(typeof(Clock)).Should().NotBeNull();
        this.locator.GetService(typeof(IDisposable)).Should().BeNull();
    }

    [Test]
    [Description("The IHaveServiceLocator extensions resolve through the object's locator")]
    public void Have_Service_Locator_Test()
    {
        var owner = new Owner { ServiceLocator = this.locator };

        owner.Get<Clock>().Should().NotBeNull();
        owner.Get<Greeter>("special").Text.Should().Be("named");
        owner.Get<Greeter>([new NamedParameter("text", "x")]).Text.Should().Be("x");
        owner.Get<Greeter>("special", []).Text.Should().Be("named");
        owner.TryGet<Clock>(out _).Should().BeTrue();
        owner.TryGet<Greeter>("unknown", out _).Should().BeFalse();
    }

    [Test]
    [Description("Only properties marked with the attribute are injected, loggers are named after the declaring type")]
    public void Inject_Marked_Test()
    {
        var target = new InjectTarget();

        this.locator.InjectMarked<InjectAttribute>(target);
        this.locator.InjectMarked<InjectAttribute>(new InjectTarget());

        target.Clock.Should().NotBeNull();
        target.Logger.Should().NotBeNull();
        target.NotMarked.Should().BeNull();

        this.locator.Invoking(l => l.InjectMarked<InjectAttribute>(null)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("A scope resolves its own instances, its tag is kept and disposing it ends the scope")]
    public void Create_Scope_Test()
    {
        var scope = this.locator.CreateScope("request");

        scope.Tag.Should().Be("request");
        scope.Should().NotBeSameAs(this.locator);
        ((IServiceLocator)scope).Get<Clock>().Should().BeSameAs(((IServiceLocator)this.locator).Get<Clock>());

        scope.Dispose();

        scope.Invoking(s => ((IServiceLocator)s).Get<Clock>()).Should().Throw<ObjectDisposedException>();
    }

    public class Greeter(string text = "default")
    {
        public string Text { get; } = text;
    }

    public class Clock;

    public class Owner : IHaveServiceLocator
    {
        public IServiceLocator ServiceLocator { get; set; }
    }

    public class InjectTarget
    {
        [Inject]
        public Clock Clock { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        public Clock NotMarked { get; set; }
    }

    private class OtherParameter : IServiceLocationParameter
    {
        public object Value { get; set; }
    }
}
