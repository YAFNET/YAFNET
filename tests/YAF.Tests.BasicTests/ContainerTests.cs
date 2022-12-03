/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Tests.BasicTests;

using System.Web;

using HttpSimulator;

using Xunit;

using YAF.Core.Context;
using YAF.Core.Tasks;
using YAF.Tests.Utils;
using YAF.Types.Interfaces;

/// <summary>
/// The container tests.
/// </summary>
[TestFixture]
[Category("Basic Container Tests")]
public class ContainerTests
{
    /// <summary>
    /// The container is available from YAF Context not in request.
    /// </summary>
    [Test]
    [Description("The container is available from yaf context not in request.")]
    public void Container_Is_Available_From_BoardContext_Not_In_Request()
    {
        var serviceLocator = BoardContext.Current.ServiceLocator;

        var exception = Record.Exception(() => serviceLocator.Get<HttpRequestBase>());

        NUnit.Framework.Assert.AreNotEqual(null, exception, "Exception should not be Null");
    }

    /// <summary>
    /// The container is available to send digest in background.
    /// </summary>
    [Test]
    [Description("The container is available to send digest in background.")]
    public void Container_Is_Available_To_Send_Digest_In_Background()
    {
        using (new HttpSimulator("/", TestConfig.TestFilesDirectory).SimulateRequest())
        {
            var sendTask = new DigestSendTask();

            BoardContext.Current.ServiceLocator.Get<IInjectServices>().Inject(sendTask);

            sendTask.RunOnce();
        }
    }
}