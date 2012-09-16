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
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    /// The login/log off user tester.
    /// </summary>
    [TestFixture]
    public class LoginLogoutUserTests
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
        /// Set Up
        /// </summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
            this.browser = !TestConfig.UseExistingInstallation ? TestSetup.IEInstance : new IE();

            this.browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            this.browser.Close();
        }

        /// <summary>
        /// Login via Login Page User Test
        /// </summary>
        [Test]
        public void Login_Page_User_Test()
        {
            this.browser.GoTo("{0}yaf_login.aspx".FormatWith(TestConfig.TestForumUrl));

            if (this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists)
            {
                // Logout First
                this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Click();

                this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
            }

            this.browser.GoTo("{0}yaf_login.aspx".FormatWith(TestConfig.TestForumUrl));

            this.browser.TextField(Find.ById(new Regex("Login1_UserName"))).TypeText(TestConfig.TestUserName);
            this.browser.TextField(Find.ById(new Regex("Login1_Password"))).TypeText(TestConfig.TestUserPassword);
            this.browser.Button(Find.ById(new Regex("Login1_LoginButton"))).Click();

            Assert.IsTrue(this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists, "Login failed");
        }

        /// <summary>
        /// Login via Login Box User Test
        /// </summary>
        [Test]
        public void Login_LoginBox_User_Test()
        {
            this.browser.GoTo(TestConfig.TestForumUrl);

            if (this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists)
            {
                // Logout First
                this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Click();

                this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
            }

            this.browser.GoTo(TestConfig.TestForumUrl);

            this.browser.Link(Find.ByClass("LoginLink")).Click();

            Assert.IsTrue(this.browser.TextField(Find.ById(new Regex("Login1_UserName"))).Exists, "Login Box is disabled in Host Settings");

            this.browser.TextField(Find.ById(new Regex("Login1_UserName"))).TypeText(TestConfig.TestUserName);
            this.browser.TextField(Find.ById(new Regex("Login1_Password"))).TypeText(TestConfig.TestUserPassword);
            this.browser.Button(Find.ById(new Regex("Login1_LoginButton"))).Click();

            this.browser.Refresh();

            Assert.IsTrue(this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists, "Login failed");
        }

        /// <summary>
        /// Logout User Test
        /// </summary>
        [Test]
        public void Logout_User_Test()
        {
            this.browser.GoTo(TestConfig.TestForumUrl);

            this.browser.Link(Find.ById(new Regex("_LogOutButton"))).Click();

            this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Welcome Guest"), "Logout Failed");
        }
    }
}
