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

namespace YAF.Tests.Utils
{
    using System.Text.RegularExpressions;

    using netDumbster.smtp;
    using WatiN.Core;
    using WatiN.Core.Exceptions;
    using WatiN.Core.Native.Windows;
    using YAF.Types.Extensions;

    /// <summary>
    /// Unit TestBase.
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// The IE Instance
        /// </summary>
        protected IE browser;

        /// <summary>
        /// The Test Mail Server.
        /// </summary>
        protected SimpleSmtpServer testMailServer;

        /// <summary>
        /// Logins the admin user.
        /// </summary>
        /// <returns>Returns if Successfully or not
        /// </returns>
        protected bool LoginAdminUser()
        {
            TestHelper.LoginUser(this.browser, TestConfig.AdminUserName, TestConfig.AdminPassword);

            return this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists;
        }

        /// <summary>
        /// Logins the user.
        /// </summary>
        /// <returns>
        /// Returns if Successfully or not
        /// </returns>
        protected bool LoginUser()
        {
            return this.LoginUser(TestConfig.TestUserName, TestConfig.TestUserPassword);
        }

        /// <summary>
        /// Logins the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// Returns if Successfully or not
        /// </returns>
        protected bool LoginUser(string userName, string password)
        {
            // Login as Test User
            var loginSucceed = TestHelper.LoginUser(this.browser, userName, password);
            if (!loginSucceed)
            {
                TestHelper.RegisterStandardTestUser(this.browser, userName, password);
            }

            return this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists;
        }

        /// <summary>
        /// lookouts the user.
        /// </summary>
        /// <param name="closeBrowser">
        /// The close Browser.
        /// </param>
        /// <returns>
        /// Returns if Successfully or not
        /// </returns>
        protected bool LogoutUser(bool closeBrowser = true)
        {
            this.browser.GoTo(TestConfig.TestForumUrl);

            this.browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);

            try
            {
                this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Click();

                this.browser.Button(Find.ById("forum_ctl02_OkButton")).Click();

                bool contains = this.browser.ContainsText("Welcome Guest");

                if (closeBrowser)
                {
                    this.browser.Close();
                }

                return contains;
            }
            catch (ElementNotFoundException)
            {
                if (closeBrowser)
                {
                    this.browser.Close();
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the new test topic.
        /// </summary>
        /// <returns>
        /// Returns if Creating of the 
        /// New Topic was successfully or not
        /// </returns>
        protected bool CreateNewTestTopic()
        {
            this.browser.GoTo("{0}yaf_postmessage.aspx?f={1}".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

            if (!this.browser.ContainsText("Post New Topic"))
            {
                return false;
            }

            // Create New Topic
            this.browser.TextField(Find.ById(new Regex("_TopicSubjectTextBox"))).TypeText("Auto Created Test Topic");

            this.browser.TextField(Find.ById(new Regex("_YafTextEditor"))).TypeText("This is a Test Message Created by an automated Unit Test");

            // Post New Topic
            this.browser.Link(Find.ById(new Regex("_PostReply"))).Click();

            return this.browser.ContainsText("Next Topic");
        }

        /// <summary>
        /// Creates a new reply in the test topic.
        /// </summary>
        /// <param name="message">The Reply message.</param>
        /// <returns>
        /// Returns if Reply was Created or not
        /// </returns>
        protected bool CreateNewReplyInTestTopic(string message)
        {
            // Go to Post New Topic
            this.browser.GoTo("{0}yaf_postst{1}.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.TestTopicID));

            if (this.browser.ContainsText("You've passed an invalid value to the forum."))
            {
                return false;
            }

            this.browser.Link(Find.ById(new Regex("_PostReplyLink1"))).Click();

            if (!this.browser.ContainsText("Post a reply"))
            {
                return false;
            }

            // Create New Reply
            this.browser.TextField(Find.ById(new Regex("_YafTextEditor"))).TypeText(message);

            // Post New Topic
            this.browser.Link(Find.ById(new Regex("_PostReply"))).Click();

            return this.browser.ContainsText(message);
        }

        /// <summary>
        /// Sends a private message.
        /// </summary>
        /// <param name="testMessage">The test message.</param>
        /// <returns>If the Message was sent or not</returns>
        protected bool SendPrivateMessage(string testMessage)
        {
            this.browser.GoTo("{0}yaf_pmessage.aspx".FormatWith(TestConfig.TestForumUrl));

            // Send a Message to Myself
            this.browser.TextField(Find.ById(new Regex("_To"))).TypeText(TestConfig.TestUserName);

            this.browser.TextField(Find.ById(new Regex("_PmSubjectTextBox"))).TypeText("Testmessage");
            this.browser.TextField(Find.ById(new Regex("_YafTextEditor"))).TypeText(testMessage);

            this.browser.Link(Find.ById(new Regex("_Save"))).Click();

            // Check if MessageBox is Shown
            return this.browser.ContainsText("unread message(s)");
        }
    }
}
