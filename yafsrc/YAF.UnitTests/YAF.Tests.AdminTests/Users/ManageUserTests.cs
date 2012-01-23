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

namespace YAF.Tests.AdminTests.Users
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.DialogHandlers;

    using YAF.Tests.Utils;
    using YAF.Utils;

    /// <summary>
    /// The Manage User Tests
    /// </summary>
    [TestFixture]
    public class ManageUserTests : TestBase
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

            Assert.IsTrue(this.LoginAdminUser(), "Login failed");
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
        /// Delete the random test user test.
        /// </summary>
        [Test]
        public void Delete_Random_Test_User_Test()
        {
            this.browser.GoTo("{0}yaf_admin_users.aspx".FormatWith(TestConfig.TestForumUrl));

            // Search for TestUser
            this.browser.TextField(Find.ById(new Regex("_name"))).TypeText(TestConfig.TestUserName);
            this.browser.Button(Find.ById(new Regex("_search"))).Click();

            var userProfileLink = this.browser.Link(Find.ById(new Regex("1_NameEdit")));

            Assert.IsNotNull(userProfileLink, "Random Test User doesn't Exist, Please Run the Register_Random_New_User_Test before");

            var userDelete = this.browser.Link(Find.ById(new Regex("UserList_ctl01_ThemeButtonDelete")));

            var confirmDialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(browser.DialogWatcher, confirmDialog))
            {
                userDelete.ClickNoWait();
                confirmDialog.WaitUntilExists();
                confirmDialog.OKButton.Click();
                browser.WaitForComplete();
            }
        }
    }
}
