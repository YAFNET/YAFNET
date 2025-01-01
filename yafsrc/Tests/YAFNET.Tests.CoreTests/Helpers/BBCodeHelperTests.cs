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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.CoreTests.Helpers;

/// <summary>
/// YAF.Utils BBCodeHelper Tests
/// </summary>
public class BBCodeHelperTests
{
    /// <summary>
    /// Strips all BBCodes from a string.
    /// </summary>
    [Fact]
    [Description("Strips all BBCodes from a string.")]
    public void StripBBCodeTest()
    {
        const string testMessage =
            "This is a test text containing [b]bold[/b] and [i]italic[i] text and other bbcodes [img]http://test.com/testimage.jpg[/img]";

        Assert.Equal(
            "This is a test text containing bold and italic text and other bbcodes http://test.com/testimage.jpg",
            BBCodeHelper.StripBBCode(testMessage));
    }
}