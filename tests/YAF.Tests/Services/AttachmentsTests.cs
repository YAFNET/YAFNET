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

namespace YAF.Tests.Services;

using YAF.Core.Services;

/// <summary>
/// The attachments that are shown in the page (images) and the ones that are downloaded.
/// </summary>
[TestFixture]
public class AttachmentsTests
{
    [TestCase("image/png", true)]
    [TestCase("image/jpeg", true)]
    [TestCase("IMAGE/GIF", true)]
    [TestCase("image/svg+xml", false)]
    [TestCase("text/xml", false)]
    [TestCase("application/xml", false)]
    [TestCase("text/html", false)]
    [TestCase("text/plain", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    [Description("Only images without scripts (no svg) are shown in the page, all other files are downloaded")]
    public void IsInlineImage_Test(string contentType, bool expected)
    {
        Attachments.IsInlineImage(contentType).Should().Be(expected);
    }
}
