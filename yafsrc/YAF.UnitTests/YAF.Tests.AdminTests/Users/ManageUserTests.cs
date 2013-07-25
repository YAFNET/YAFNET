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

namespace YAF.Tests.AdminTests.Users
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.DialogHandlers;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

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
            this.browser = !TestConfig.UseExistingInstallation ? TestSetup._testBase.IEInstance : new IE();

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
            this.browser.GoTo(
                "{0}{1}admin_users.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

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

        [Test]
        [Ignore]
        // TODO
        public void Add_User_To_Test_Role_Test()
        {
            /*this.browser.GoTo("{0}{1}admin_users.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            // Search for TestUser
            this.browser.TextField(Find.ById(new Regex("_name"))).TypeText(TestConfig.TestUserName);
            this.browser.Button(Find.ById(new Regex("_search"))).Click();

            var userProfileLink = this.browser.Link(Find.ById(new Regex("_NameEdit_0")));

            Assert.IsNotNull(userProfileLink, "Test User doesn't Exist");

            userProfileLink.Click();

            this.browser.Link(Find.ByText("User Roles")).Click();


            */
        }
    }
}
