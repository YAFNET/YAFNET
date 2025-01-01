/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Modules;

using Autofac;

/// <summary>
///     The utilities module.
/// </summary>
public class UtilitiesModule : BaseModule
{
    /// <summary>
    /// The load.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    override protected void Load(ContainerBuilder builder)
    {
        RegisterWebAbstractions(builder);
    }

    /// <summary>
    /// The register web abstractions.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    private static void RegisterWebAbstractions(ContainerBuilder builder)
    {
        builder.Register(_ => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>()
            .InstancePerLifetimeScope();
        builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerLifetimeScope();
        builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>()
            .InstancePerLifetimeScope();
        builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>()
            .InstancePerLifetimeScope();
        builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>()
            .InstancePerLifetimeScope();
    }
}