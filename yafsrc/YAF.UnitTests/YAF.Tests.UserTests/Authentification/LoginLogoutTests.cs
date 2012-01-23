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

namespace YAF.Tests.UserTests.Authentification
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Utils;

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
