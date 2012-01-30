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
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.DialogHandlers;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Utils;

    /// <summary>
    /// The user buddy tests.
    /// </summary>
    [TestFixture]
    public class BuddyTests : TestBase
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
        /// Request Buddy Test
        /// Login as Admin and add the TestUser as Buddy
        /// </summary>
        [Test]
        public void Request_Buddy_Test()
        {
            this.LoginAdminUser();

            this.browser.GoTo("{0}yaf_cp_editbuddies.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Pending Requests"), "My Buddies function is not available for that User, or is disabled for that Forum");

            // Go to Members Page and Find the Test User 
            this.browser.GoTo("{0}yaf_members.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Search Members"), "Members List View Permissions needs to be disabled.");

            this.browser.TextField(Find.ById(new Regex("_UserSearchName"))).TypeText(TestConfig.TestUserName);

            var userProfileLink = this.browser.Link(Find.ByText(TestConfig.TestUserName));

            Assert.IsNotNull(userProfileLink, "User Profile Not Found");

            userProfileLink.Click();

            Assert.IsFalse(this.browser.ContainsText("Remove Buddy"), "User is already a Buddy");

            Assert.IsTrue(this.browser.ContainsText("Add as buddy"), "My Buddies function is not available for that User, or is disabled for that Forum");

            this.browser.Link(Find.ById(new Regex("_lnkBuddy"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Buddy request sent."));

            this.browser.Link(Find.ByText("OK")).Click();
        }

        /// <summary>
        /// Approve the buddy request test.
        /// </summary>
        [Test]
        public void Approve_Buddy_Request_Test()
        {
            Assert.IsTrue(this.LoginUser(), "Login failed");

            this.browser.GoTo("{0}yaf_cp_editbuddies.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Pending Requests"), "My Buddies function is not available for that User, or is disabled for that Forum");

            this.browser.Link(Find.ByText("Pending Requests")).Click();

            // Select the First Request
            this.browser.Link(Find.ByText("Approve")).Click();

            Assert.IsTrue(this.browser.ContainsText("You have been added to {0}'s buddy list.".FormatWith(TestConfig.AdminUserName)), "Approve Buddy Failed");

            this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
        }

        /// <summary>
        /// Deny the Newest buddy Request test.
        /// </summary>
        [Test]
        public void Deny_Buddy_Request_Test()
        {
            Assert.IsTrue(this.LoginUser(), "Login failed");

            this.browser.GoTo("{0}yaf_cp_editbuddies.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Pending Requests"), "My Buddies function is not available for that User, or is disabled for that Forum");

            this.browser.Link(Find.ByText("Pending Requests")).Click();

            // Select the First Request
            var deny = this.browser.Link(Find.ByText("Deny"));

            var confirmDialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(browser.DialogWatcher, confirmDialog))
            {
                deny.ClickNoWait();
                confirmDialog.WaitUntilExists();
                confirmDialog.OKButton.Click();
                browser.WaitForComplete();
            }

            Assert.IsTrue(this.browser.ContainsText("Buddy request denied."), "Deny Request Failed");

            this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
        }

        /// <summary>
        /// Approve and add buddy request test.
        /// </summary>
        [Test]
        public void Approve_and_Add_Buddy_Request_Test()
        {
            Assert.IsTrue(this.LoginUser(), "Login failed");

            this.browser.GoTo("{0}yaf_cp_editbuddies.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Pending Requests"), "My Buddies function is not available for that User, or is disabled for that Forum");

            this.browser.Link(Find.ByText("Pending Requests")).Click();

            // Select the First Request
            this.browser.Link(Find.ByText("Approve and Add")).Click();

            Assert.IsTrue(this.browser.ContainsText("You and {0} are now buddies.".FormatWith(TestConfig.AdminUserName)), "Approve and Add Buddy Failed");

            this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
        }

        /// <summary>
        /// Remove a buddy test.
        /// </summary>
        [Test]
        public void Remove_Buddy_Test()
        {
            Assert.IsTrue(this.LoginUser(), "Login failed");

            this.browser.GoTo("{0}yaf_cp_editbuddies.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Pending Requests"), "My Buddies function is not available for that User, or is disabled for that Forum");

            // Select the Newest Buddy
            var delete = this.browser.Link(Find.ById(new Regex("_BuddyList1_rptBuddy_lnkRemove_0")));

            Assert.IsNotNull(delete, "Currently the Test User doesnt have any Buddies");

            var confirmDialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(browser.DialogWatcher, confirmDialog))
            {
                delete.ClickNoWait();
                confirmDialog.WaitUntilExists();
                confirmDialog.OKButton.Click();
                browser.WaitForComplete();
            }

            Assert.IsTrue(this.browser.ContainsText("has been removed from your Buddy list."), "Remove Buddy Failed");

            this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
        }
    }
}
