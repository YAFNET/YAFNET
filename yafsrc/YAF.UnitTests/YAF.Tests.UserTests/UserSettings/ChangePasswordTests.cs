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
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

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
                "{0}ABCDEF".FormatWith(TestConfig.TestUserPassword));
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_ConfirmNewPassword"))).TypeText(
                "{0}ABCDEF".FormatWith(TestConfig.TestUserPassword));

            // Submit
            this.browser.Button(Find.ById(new Regex("_ChangePasswordContainerID_ChangePasswordPushButton"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("Password has been successfully changed."), "Changing Password Failed");

            this.browser.Button(Find.ById(new Regex("_SuccessContainerID_ContinuePushButton"))).Click();

            // Now Change Password Back to Default Password
            this.browser.GoTo("{0}yaf_cp_changepassword.aspx".FormatWith(TestConfig.TestForumUrl));

            // Enter Old Password
            this.browser.TextField(Find.ById(new Regex("_ChangePasswordContainerID_CurrentPassword"))).TypeText(
                "{0}ABCDEF".FormatWith(TestConfig.TestUserPassword));

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