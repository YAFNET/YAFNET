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
    /// The topic tests.
    /// </summary>
    [TestFixture]
    public class TopicTests : TestBase
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
        /// Create the new topic in forum 1 test.
        /// </summary>
        [Test]
        public void Create_New_Topic_Test()
        {
            // Go to Post New Topic
            this.browser.GoTo("{0}yaf_postmessage.aspx?f={0}".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

            Assert.IsTrue(this.browser.ContainsText("Post New Topic"), "Post New Topic not possible");

            // Create New Topic
            this.browser.TextField(Find.ById(new Regex("_TopicSubjectTextBox"))).TypeText("Auto Created Test Topic");

            if (this.browser.ContainsText("Description"))
            {
                this.browser.TextField(Find.ById(new Regex("_TopicDescriptionTextBox"))).TypeText("Test Description");
            }

            if (this.browser.ContainsText("Status"))
            {
                this.browser.SelectList(Find.ById(new Regex("_TopicStatus"))).SelectByValue("INFORMATIC");
            }

            this.browser.TextField(Find.ById(new Regex("_YafTextEditor"))).TypeText("This is a Test Message Created by an automated Unit Test");

            // Post New Topic
            this.browser.Link(Find.ById(new Regex("_PostReply"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Next Topic"), "Topic Creating failed");
        }
    }
}
