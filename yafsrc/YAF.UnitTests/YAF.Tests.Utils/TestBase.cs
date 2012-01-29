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

namespace YAF.Tests.Utils
{
    using System;
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using netDumbster.smtp;
    using WatiN.Core;
    using WatiN.Core.Exceptions;
    using WatiN.Core.Native.Windows;
    using YAF.Utils;

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
            // Login as Test User
            var loginSucceed = TestHelper.LoginUser(this.browser, TestConfig.TestUserName, TestConfig.TestUserPassword);
            if (!loginSucceed)
            {
                TestHelper.RegisterStandardTestUser(this.browser);
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
            this.browser.GoTo("{0}yaf_postmessage.aspx?f={0}".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

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
            return this.browser.ContainsText("New Private Message(s)");
        }
    }
}
