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

namespace YAF.Tests.UserTests.UserSettings
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Utils;

    /// <summary>
    /// The user Change Password tests.
    /// </summary>
    [TestFixture]
    public class ChangePasswordTests : TestBase
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
        /// Change the user password test
        /// </summary>
        [Test]
        public void Change_User_Password_Test()
        {
            this.browser.GoTo("{0}yaf_cp_changepassword.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Change Password"), "Change Password is not available for that User");

            // Enter Old Password
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_CurrentPassword"))).TypeText(
                TestConfig.TestUserPassword);

            // Enter New Password
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_NewPassword"))).TypeText(
                string.Format("{0}ABCDEF", TestConfig.TestUserPassword));
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_ConfirmNewPassword"))).TypeText(
                string.Format("{0}ABCDEF", TestConfig.TestUserPassword));

            // Submit
            this.browser.Button(Find.ById(new Regex("_ChangePasswordContainerID_ChangePasswordPushButton"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("Password has been successfully changed."), "Changing Password Failed");

            this.browser.Button(Find.ById(new Regex("_SuccessContainerID_ContinuePushButton"))).Click();

            // Now Change Password Back to Default Password
            this.browser.GoTo("{0}yaf_cp_changepassword.aspx".FormatWith(TestConfig.TestForumUrl));

            // Enter Old Password
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_CurrentPassword"))).TypeText(
                string.Format("{0}ABCDEF", TestConfig.TestUserPassword));

            // Enter New Password
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_NewPassword"))).TypeText(
                TestConfig.TestUserPassword);
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_ConfirmNewPassword"))).TypeText(
                TestConfig.TestUserPassword);

            // Submit
            this.browser.Button(Find.ById(new Regex("_ChangePasswordContainerID_ChangePasswordPushButton"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("Password has been successfully changed."), "Changing Password Failed");
        }
    }
}