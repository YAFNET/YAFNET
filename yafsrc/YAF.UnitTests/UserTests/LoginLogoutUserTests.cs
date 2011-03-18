/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
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

namespace YAF.UnitTests.UserTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WatiN.Core;
    
    /// <summary>
    /// The login/logoff user tester.
    /// </summary>
    [TestClass]
    public class LoginLogoutUserTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Login User Test
        /// </summary>
        [TestMethod]
        public void LoginUser()
        {
            using (var browser = new IE("http://localhost/yaf/yaf_login.aspx"))
            {
                const string UserName = "Testuser";
                const string Password = "testpass";

                browser.TextField(Find.ById("forum_ctl04_Login1_UserName")).TypeText(UserName);
                browser.TextField(Find.ById("forum_ctl04_Login1_Password")).TypeText(Password);
                browser.Button(Find.ById("forum_ctl04_Login1_LoginButton")).Click();

                browser.Refresh();
                Assert.IsTrue(browser.ContainsText("Logged in as:"), "Login failed");
            }
        }

        /// <summary>
        /// Logout User Test
        /// </summary>
        [TestMethod]
        public void LogoutUser()
        {
            using (var browser = new IE("http://localhost/yaf/"))
            {
                browser.Link(Find.ById("forum_ctl01_LogOutButton")).Click();

                browser.Button(Find.ById("forum_ctl02_OkButton")).Click();

                browser.Refresh();
                Assert.IsTrue(browser.ContainsText("Welcome Guest"), "Logout Failed");
            }
        }
    }
}
