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

namespace YAF.Tests.AdminTests.Settings
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;

    using YAF.Tests.Utils;
    using YAF.Utils;

    /// <summary>
    /// The  Host Settings Tests
    /// </summary>
    [TestFixture]
    public class HostSettingsTests : TestBase
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
        /// Basic test to check if the Host Settings are correctly saved
        /// </summary>
        [Test]
        public void HostSettings_Saved_Correctly_Test()
        {
            this.browser.GoTo("{0}yaf_admin_hostsettings.aspx".FormatWith(TestConfig.TestForumUrl));

            // Modify a random setting
            var width = this.browser.TextField(Find.ById(new Regex("_ImageAttachmentResizeWidth"))).Text;

            this.browser.TextField(Find.ById(new Regex("_ImageAttachmentResizeWidth"))).TypeText("350");

            this.browser.Button(Find.ById(new Regex("_Save"))).Click();

            this.browser.GoTo("{0}yaf_admin_hostsettings.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.TextField(Find.ById(new Regex("_ImageAttachmentResizeWidth"))).Text.Equals("350"));

            // Restore old Setting
            this.browser.TextField(Find.ById(new Regex("_ImageAttachmentResizeWidth"))).TypeText(width);

            this.browser.Button(Find.ById(new Regex("_Save"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Who is Online"));
        }
    }
}
