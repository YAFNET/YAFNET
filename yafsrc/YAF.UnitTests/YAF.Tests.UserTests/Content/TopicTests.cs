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

namespace YAF.Tests.UserTests.Content
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

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
            this.browser.GoTo("{0}yaf_postmessage.aspx?f={1}".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

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
