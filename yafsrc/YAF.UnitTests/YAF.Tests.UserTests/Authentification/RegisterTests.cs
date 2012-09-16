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

namespace YAF.Tests.UserTests.Authentification
{
    using System;
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Exceptions;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    /// The Register a test user Test.
    /// </summary>
    [TestFixture]
    public class RegisterUser
    {
        /// <summary>
        /// The Browser Instance
        /// </summary>
        private IE browser;

        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Logout New User
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.browser.GoTo(TestConfig.TestForumUrl);

            this.browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);

            try
            {
                this.browser.Link(Find.ById("forum_ctl01_LogOutButton")).Click();

                this.browser.Button(Find.ById("forum_ctl02_OkButton")).Click();

                Assert.IsTrue(this.browser.ContainsText("Welcome Guest"), "Logout Failed");

                this.browser.Close();
            }
            catch (ElementNotFoundException)
            {
                this.browser.Close();
            }
        }

        /// <summary>
        /// Register Random Test User Test
        /// </summary>
        [Test]
        public void Register_Random_New_User_Test()
        {
            this.browser = !TestConfig.UseExistingInstallation ? TestSetup.IEInstance : new IE();

            this.browser.GoTo("{0}yaf_register.aspx".FormatWith(TestConfig.TestForumUrl));

            // Create New Random Test User
            var random = new Random();

            var userName = "TestUser{0}".FormatWith(random.Next());
            var email = "{0}@test.com".FormatWith(userName.ToLower());

            this.browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);

            // Check if Registrations are Disabled
            Assert.IsFalse(this.browser.ContainsText("You tried to enter an area where you didn't have access"), "Registrations are disabled");

            // Accept the Rules
            if (this.browser.ContainsText("Forum Rules"))
            {
                this.browser.Button(Find.ById("forum_ctl04_Login1_LoginButton")).Click();
                this.browser.Refresh();
            }

            Assert.IsFalse(this.browser.ContainsText("Security Image"), "Captchas needs to be disabled in order to run the tests");

            // Fill the Register Page
            this.browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_UserName"))).TypeText(
                userName);

            if (this.browser.ContainsText("Display Name"))
            {
                this.browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_DisplayName"))).TypeText(userName);
            }

            this.browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Password"))).TypeText(TestConfig.TestUserPassword);
            this.browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_ConfirmPassword"))).TypeText(TestConfig.TestUserPassword);
            this.browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Email"))).TypeText(email);
            this.browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Question"))).TypeText(TestConfig.TestUserPassword);
            this.browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Answer"))).TypeText(TestConfig.TestUserPassword);

            // Create User
            this.browser.Button(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_StepNextButton"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Forum Preferences"), "Registration failed");

            this.browser.Button(Find.ById(new Regex("ProfileNextButton"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Logged in as:"), "Registration failed");
        }
    }
}
