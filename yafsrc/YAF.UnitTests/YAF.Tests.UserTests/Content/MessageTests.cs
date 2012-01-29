/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Tests.UserTests.Content
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Utils;

    /// <summary>
    /// The Message tests.
    /// </summary>
    [TestFixture]
    public class MessageTests : TestBase
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
        /// Create a new reply in a topic.
        /// </summary>
        [Test]
        public void Post_Reply_Test()
        {
            // Go to Post New Topic
            this.browser.GoTo("{0}yaf_postst{1}.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.TestTopicID));

            if (this.browser.ContainsText("You've passed an invalid value to the forum."))
            {
                // Topic doesnt exist create a topic first
                Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");
            }

            this.browser.Link(Find.ById(new Regex("_PostReplyLink1"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Post a reply"), "Post Reply not possible");

            // Create New Reply
            this.browser.TextField(Find.ById(new Regex("_YafTextEditor"))).TypeText("This is a Test Reply in an Test Topic Created by an automated Unit Test");

            // Post New Topic
            this.browser.Link(Find.ById(new Regex("_PostReply"))).Click();

            Assert.IsTrue(this.browser.ContainsText("This is a Test Reply in an Test Topic Created by an automated Unit Test"), "Reply Message failed");
        }

        /// <summary>
        /// Post a reply with quote_ test.
        /// </summary>
        [Test]
        public void Post_Reply_With_Quote_Test()
        {
            // Go to Post New Topic
            this.browser.GoTo("{0}yaf_postst{1}.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.TestTopicID));

            if (this.browser.ContainsText("You've passed an invalid value to the forum."))
            {
                // Topic doesnt exist create a topic first
                Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");
            }

            this.browser.Link(Find.ById(new Regex("DisplayPost1_Quote"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Post a reply"), "Post Reply not possible");

            // Create New Reply
            this.browser.TextField(Find.ById(new Regex("_YafTextEditor"))).AppendText("  Quoting Test");

            // Post New Topic
            this.browser.Link(Find.ById(new Regex("_PostReply"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Quoting Test"), "Quoting Message Failed");
        }
    }
}
