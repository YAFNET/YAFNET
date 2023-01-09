/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Tests.CoreTests.Formatting;

using CategoryAttribute = System.ComponentModel.CategoryAttribute;


/// <summary>
/// Localization Tests
/// </summary>
[TestFixture]
[Category("Formatting")]
public class LocalizationTests : IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationTests"/> class.
    /// </summary>
    public LocalizationTests()
    {
        GlobalContainer.AutoFacContainer.Resolve<IInjectServices>().Inject(this);
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    [Inject]
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Simple test to check if the localizer works.
    /// </summary>
    [Test]
    [Description("Simple test to check if the localizer works.")]
    public void Simple_Localization_Test()
    {
        /*using (new HttpSimulator("/", TestConfig.TestFilesDirectory).SimulateRequest())
        {*/
            var testMessage = this.Get<ILocalization>().GetText("TOOLBAR", "WELCOME_GUEST");

            Assert.AreEqual(
                "Welcome Guest! To enable all features please try to register or login.",
                testMessage,
                testMessage);
       // }
    }

    /// <summary>
    /// Simple test (with parameter) to check if the localizer works.
    /// </summary>
    [Test]
    [Description("Simple test (with parameter) to check if the localizer works.")]
    public void Simple_Localization_With_Parameter_Test()
    {
    /*using (new HttpSimulator("/", TestConfig.TestFilesDirectory).SimulateRequest())
     {*/
    var testMessage = this.Get<ILocalization>()
                .GetTextFormatted("LOGGED_IN_AS", "TestUser");

            Assert.AreEqual("Logged in as: TestUser", testMessage, testMessage);
        //}
    }

    /// <summary>
    /// Simple test to check if the localizer works (With a specific language).
    /// </summary>
    [Test]
    [Description("Simple test to check if the localizer works (With a specific language).")]
    public void Simple_Localization_Language_Specific_Test()
    {
    /*using (new HttpSimulator("/", TestConfig.TestFilesDirectory).SimulateRequest())
    {*/
    var testMessage = this.Get<ILocalization>()
                .GetText("TOOLBAR", "WELCOME_GUEST", "german-du.json");

            Assert.AreEqual("Willkommen, Gast!", testMessage, testMessage);
       // }
    }
}