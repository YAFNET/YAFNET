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
