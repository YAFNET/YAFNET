/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/

namespace YAF.Tests.UserTests.Features
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;

    /// <summary>
    /// The user Reputation tests.
    /// </summary>
    [TestFixture]
    [NUnit.Framework.Description("The user Reputation tests.")]
    public class ReputationTests : TestBase
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Login User Setup
        /// </summary>
        [TestFixtureSetUp]
        public void SetUpTest()
        {
            this.browser = !TestConfig.UseExistingInstallation ? TestSetup.IEInstance : new IE();

            this.browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);

            Assert.IsTrue(this.LoginUser(), "Login failed");
        }

        /// <summary>
        /// Logout Test User
        /// </summary>
        [TestFixtureTearDown]
        public void TearDownTest()
        {
            this.LogoutUser();
        }

        /// <summary>
        /// Add +1 Reputation from TestUser2 to TestUser Test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Add +1 Reputation from TestUser2 to TestUser Test.")]
        public void Add_Reputation_To_Test_User_Test()
        {
            // First Creating a new test topic with the test user
            Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

            var newTestTopicUrl = this.browser.Url;

            // Now Login with Test User 2
            Assert.IsTrue(this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password), "Login with test user 2 failed");

            // Go To New Test Topic Url
            this.browser.GoTo(newTestTopicUrl);

            Assert.IsTrue(
               this.browser.Link(Find.ById(new Regex("_AddReputation"))).Exists,
               "Reputation is deactivated  in yaf or the user has already voted within the last 24 hours, or the user doesnt have enough points to be allowed to vote");

            this.browser.Link(Find.ById(new Regex("_AddReputation"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Thanks for your Voting"), "Voting failed");
        }

        /// <summary>
        /// Add -1 Reputation from TestUser2 to TestUser Test
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Add -1 Reputation from TestUser2 to TestUser Test")]
        public void Remove_Reputation_From_Test_User_Test()
        {
            // First Creating a new test topic with the test user
            Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

            var newTestTopicUrl = this.browser.Url;

            // Now Login with Test User 2
            Assert.IsTrue(this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password), "Login with test user 2 failed");

            // Go To New Test Topic Url
            this.browser.GoTo(newTestTopicUrl);

            Assert.IsTrue(
               this.browser.Link(Find.ById(new Regex("_RemoveReputation"))).Exists,
               "Reputation is deactivated (Or negative Reputation) in yaf or the user has already voted within the last 24 hours, or the user doesnt have enough points to be allowed to vote");

            this.browser.Link(Find.ById(new Regex("_RemoveReputation"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Thanks for your Voting"), "Voting failed");
        }
    }
}
