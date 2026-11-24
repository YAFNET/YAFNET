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

namespace YAF.Tests.Context;

using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

using YAF.Core;
using YAF.Core.Context;
using YAF.Types.Interfaces;

/// <summary>
/// The lifetime of the <see cref="BoardContext"/> instances that <see cref="BoardContextPageProvider"/> creates.
/// </summary>
[TestFixture]
public class BoardContextPageProviderTests
{
    private IContainer container;

    private DefaultHttpContext httpContext;

    private CompletingResponseFeature response;

    [SetUp]
    public void SetUp()
    {
        this.response = new CompletingResponseFeature();
        this.httpContext = new DefaultHttpContext();
        this.httpContext.Features.Set<IHttpResponseFeature>(this.response);

        var builder = new ContainerBuilder();

        // same registrations as GeneralModule uses in production
        builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>()
            .InstancePerLifetimeScope();
        builder.RegisterType<BoardContextPageProvider>().AsSelf().SingleInstance();

        builder.RegisterInstance(new HttpContextAccessor { HttpContext = this.httpContext }).As<IHttpContextAccessor>();
        builder.RegisterType<ScopedResource>().AsSelf().InstancePerLifetimeScope();

        this.container = builder.Build();
    }

    [TearDown]
    public void TearDown()
    {
        this.container.Dispose();
    }

    [Test]
    [Description("The board context of a request is reused for the request and disposed with its lifetime scope when the response completes")]
    public async Task Request_Instance_Is_Disposed_When_Response_Completes_Test()
    {
        var provider = this.container.Resolve<BoardContextPageProvider>();

        var boardContext = provider.Instance;
        provider.Instance.Should().BeSameAs(boardContext);

        var resource = (ScopedResource)boardContext.ServiceLocator.GetService(typeof(ScopedResource));
        var unloaded = false;
        boardContext.Unload += (_, _) => unloaded = true;

        resource.IsDisposed.Should().BeFalse();

        await this.response.CompleteAsync();

        unloaded.Should().BeTrue();
        resource.IsDisposed.Should().BeTrue();
        boardContext.Invoking(c => c.ServiceLocator).Should().Throw<ObjectDisposedException>();
    }

    [Test]
    [Description("Without a request the provider keeps one board context, which is not disposed")]
    public void Instance_Without_Request_Is_Not_Disposed_Test()
    {
        this.container.Resolve<IHttpContextAccessor>().HttpContext = null;
        var provider = this.container.Resolve<BoardContextPageProvider>();

        var boardContext = provider.Instance;

        provider.Instance.Should().BeSameAs(boardContext);
        boardContext.ServiceLocator.Should().NotBeNull();
        this.response.CallbackCount.Should().Be(0);
    }

    /// <summary>
    /// A disposable service with the lifetime of a scope.
    /// </summary>
    private sealed class ScopedResource : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }

    /// <summary>
    /// A response feature that runs the registered <see cref="HttpResponse.OnCompleted(Func{object, Task}, object)"/> callbacks on demand.
    /// </summary>
    private sealed class CompletingResponseFeature : HttpResponseFeature
    {
        private readonly List<(Func<object, Task> Callback, object State)> callbacks = [];

        public int CallbackCount => this.callbacks.Count;

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            this.callbacks.Add((callback, state));
        }

        public async Task CompleteAsync()
        {
            foreach (var (callback, state) in this.callbacks)
            {
                await callback(state);
            }
        }
    }
}
