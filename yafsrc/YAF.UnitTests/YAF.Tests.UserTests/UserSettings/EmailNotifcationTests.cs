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

namespace YAF.Tests.UserTests.UserSettings
{
    using System;
    using System.Text.RegularExpressions;

    using YAF.Types.Extensions;

    using netDumbster.smtp;
    using NUnit.Framework;
    using WatiN.Core;
    using WatiN.Core.Native.Windows;
    using YAF.Tests.Utils;

    /// <summary>
    /// The Email Notification tests.
    /// </summary>
    [TestFixture]
    public class EmailNotificationTests : TestBase
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
            this.browser = !TestConfig.UseExistingInstallation ? TestSetup._testBase.IEInstance : new IE();

            if (TestConfig.UseTestMailServer)
            {
                this.testMailServer = !TestConfig.UseExistingInstallation
                    ? TestSetup._testBase.SmtpServer
                    : SimpleSmtpServer.Start(TestConfig.TestMailPort.ToType<int>());
            }

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
        /// Add watch topic test.
        /// </summary>
        [Test]
        public void Add_Watch_Topic_Test()
        {
            // Go to Test Topic
            this.browser.GoTo(
                "{0}{2}postst{1}.aspx".FormatWith(
                    TestConfig.TestForumUrl,
                    TestConfig.TestTopicID,
                    TestConfig.ForumUrlRewritingPrefix));

            Assert.IsFalse(
                this.browser.ContainsText("You've passed an invalid value to the forum."),
                "Test Topic Doesn't Exists");

            // Open Topic Options Menu
            this.browser.Link(Find.ById(new Regex("_OptionsLink"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("Watch this topic"),
                "Watch Topic is disabled, or User already Watches that Topic");

            this.browser.Div(Find.ById(new Regex("_OptionsMenu")))
                .ElementsWithTag("li")
                .First(Find.ByText(new Regex("Watch this topic")))
                .Click();

            Assert.IsTrue(this.browser.ContainsText("Notification"), "Watch topic failed");

            this.browser.GoTo(
                "{0}{1}cp_subscriptions.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(
                this.browser.ContainsText("Email Notification Preferences"),
                "Email Notification Preferences is not available for that User");

            Assert.IsTrue(
                this.browser.Link(Find.ByUrl(new Regex(@"yaf_postst{0}_".FormatWith(TestConfig.TestTopicID)))).Exists);
        }

        /// <summary>
        /// Delete watch topic test.
        /// </summary>
        [Test]
        public void Delete_Watch_Topic_Test()
        {
            // Go to Test Topic
            this.browser.GoTo(
                "{0}{2}postst{1}.aspx".FormatWith(
                    TestConfig.TestForumUrl,
                    TestConfig.TestTopicID,
                    TestConfig.ForumUrlRewritingPrefix));

            Assert.IsFalse(
                this.browser.ContainsText("You've passed an invalid value to the forum."),
                "Test Topic Doesn't Exists");

            // Open Topic Options Menu
            this.browser.Link(Find.ById(new Regex("_OptionsLink"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("Unwatch this topic"),
                "Watch Topic is disabled, or User doesn't watch this topic");

            this.browser.Div(Find.ById(new Regex("_OptionsMenu")))
                .ElementsWithTag("li")
                .First(Find.ByText(new Regex("Unwatch this topic")))
                .Click();

            Assert.IsTrue(this.browser.ContainsText("Notification"), "Unwatch topic failed");
        }

        /// <summary>
        /// Add watch forum test.
        /// </summary>
        [Test]
        public void Add_Watch_Forum_Test()
        {
            this.browser.GoTo(
                "{0}{2}topics{1}.aspx".FormatWith(
                    TestConfig.TestForumUrl,
                    TestConfig.TestForumID,
                    TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("New Topic"), "Test Forum with that ID doesn't exists");

            Assert.IsTrue(
                this.browser.ContainsText("Watch Forum"),
                "Watch Forum is disabled, or User already Watches that Forum");

            // Watch the Test Forum
            this.browser.Link(Find.ById(new Regex("_WatchForum"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("You will now be notified when new posts are made in this forum."),
                "Watch form failed");

            this.browser.GoTo(
                "{0}{1}cp_subscriptions.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(
                this.browser.ContainsText("Email Notification Preferences"),
                "Email Notification Preferences is not available for that User");

            Assert.IsTrue(
                this.browser.Link(Find.ByUrl(new Regex(@"yaf_topics{0}_".FormatWith(TestConfig.TestForumID)))).Exists);
        }

        /// <summary>
        /// Delete watch forum test.
        /// </summary>
        [Test]
        public void Delete_Watch_Forum_Test()
        {
            this.browser.GoTo(
                "{0}{2}topics{1}.aspx".FormatWith(
                    TestConfig.TestForumUrl,
                    TestConfig.TestForumID,
                    TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("New Topic"), "Test Forum with that ID doesn't exists");

            Assert.IsTrue(
                this.browser.ContainsText("Unwatch Forum"),
                "Watch Forum is disabled, or User doesn't watch that Forum");

            this.browser.Link(Find.ById(new Regex("_WatchForum"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("You will no longer be notified when new posts are made in this forum."),
                "Watch forum failed");
        }

        /// <summary>
        /// Receive email on new post in Watched Topic test.
        /// </summary>
        [Test]
        public void Receive_Email_On_New_Post_Test()
        {
            Assert.IsTrue(TestConfig.UseTestMailServer, "This Test only works with the Test Mail Server Enabled");

            // Go to Test Topic
            this.browser.GoTo(
                "{0}{2}postst{1}.aspx".FormatWith(
                    TestConfig.TestForumUrl,
                    TestConfig.TestTopicID,
                    TestConfig.ForumUrlRewritingPrefix));

            Assert.IsFalse(
                this.browser.ContainsText("You've passed an invalid value to the forum."),
                "Test Topic Doesn't Exists");

            // Open Topic Options Menu
            this.browser.Link(Find.ById(new Regex("_OptionsLink"))).Click();

            if (this.browser.ContainsText("Watch this topic"))
            {
                this.browser.Div(Find.ById(new Regex("_OptionsMenu")))
                    .ElementsWithTag("li")
                    .First(Find.ByText(new Regex("Watch this topic")))
                    .Click();
            }

            this.LogoutUser(false);

            // Login as Admin and Post A Reply in the Test Topic
            Assert.IsTrue(this.LoginAdminUser(), "Login As Admin Failed");

            Assert.IsTrue(this.CreateNewReplyInTestTopic("Reply Message"), "Reply Message as Admin failed");

            // Check if an Email was received
            SmtpMessage mail = this.testMailServer.ReceivedEmail[0];

            Assert.AreEqual(
                "{0}@test.com".FormatWith(TestConfig.TestUserName.ToLower()),
                mail.ToAddresses[0].ToString(),
                "Receiver does not match");

            Assert.IsTrue(mail.FromAddress.ToString().Contains(TestConfig.TestForumMail), "Sender does not match");

            Assert.AreEqual(
                "Topic Subscription New Post Notification (From {0})".FormatWith(TestConfig.TestApplicationName),
                mail.Headers["Subject"],
                "Subject does not match");

            Assert.IsTrue(
                mail.MessageParts[0].BodyView.StartsWith("There's a new post in topic \""),
                "Body does not match");
        }

        /// <summary>
        /// Receive email on private message test.
        /// </summary>
        [Test]
        public void Receive_Email_On_Private_Message_Test()
        {
            Assert.IsTrue(TestConfig.UseTestMailServer, "This Test only works with the Test Mail Server Enabled");

            this.browser.GoTo(
                "{0}{1}cp_subscriptions.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(
                this.browser.ContainsText("Email Notification Preferences"),
                "Email Notification Preferences is not available for that User");

            // Make sure the option Receive an email notification when you get a new private message?
            var pmEmail = this.browser.CheckBox(Find.ById(new Regex("_PMNotificationEnabled")));

            if (!pmEmail.Checked)
            {
                pmEmail.Click();

                this.browser.Button(Find.ById(new Regex("_SaveUser"))).Click();
            }

            var testMessage = "This is an automated Test Message generated at {0}".FormatWith(DateTime.UtcNow);

            Assert.IsTrue(this.SendPrivateMessage(testMessage), "Test Message Send Failed");

            // Check if an Email was received
            SmtpMessage mail = this.testMailServer.ReceivedEmail[0];

            Assert.AreEqual(
                "{0}@test.com".FormatWith(TestConfig.TestUserName.ToLower()),
                mail.ToAddresses[0].ToString(),
                "Receiver does not match");

            Assert.IsTrue(mail.FromAddress.ToString().Contains(TestConfig.TestForumMail), "Sender does not match");

            Assert.AreEqual(
                "New Private Message From {0} at {1}".FormatWith(
                    TestConfig.TestUserName,
                    TestConfig.TestApplicationName),
                mail.Headers["Subject"],
                "Subject does not match");

            Assert.IsTrue(
                mail.MessageParts[0].BodyView.StartsWith(
                    "A new Private Message from {0} about {1} was send to you at {2}.".FormatWith(
                        TestConfig.TestUserName,
                        "Testmessage",
                        TestConfig.TestApplicationName)),
                "Body does not match");
        }

        /// <summary>
        /// TODO : Receive the email digest test.
        /// </summary>
        [Test]
        public void Receive_Email_Digest_Test()
        {
            Assert.IsTrue(TestConfig.UseTestMailServer, "This Test only works with the Test Mail Server Enabled");

            // Check if Digest is available and enabled
            this.browser.GoTo(
                "{0}{1}cp_subscriptions.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(
                this.browser.ContainsText("Email Notification Preferences"),
                "Email Notification Preferences is not available for that User");

            Assert.IsTrue(
                this.browser.ContainsText("Receive once daily digest/summary of activity?"),
                "Email Digest is not enabled in that Forum.");
        }
    }
}
