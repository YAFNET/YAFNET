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

using Microsoft.Extensions.Options;

using YAF.Core.Modules;
using YAF.Types.Objects;

namespace YAF.Core.Extensions;

/// <summary>
/// Extensions for the <see cref="ContainerBuilder"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Registers the yaf modules.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void RegisterYafModules(this ContainerBuilder builder)
    {
        builder.RegisterModule<BootstrapModule>();

        builder.Register(k => k.Resolve<IComponentContext>().Resolve<IOptions<MailConfiguration>>().Value)
            .As<MailConfiguration>();
        builder.Register(k => k.Resolve<IComponentContext>().Resolve<IOptions<BoardConfiguration>>().Value)
            .As<BoardConfiguration>();
    }
}