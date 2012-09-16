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

namespace YAF.Tests.UserTests.Features
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    /// Mobile Theme Tests
    /// </summary>
    [TestFixture]
    public class MobileThemeTests : TestBase
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
        /// Logout Test User and Switch Back to Default Theme
        /// </summary>
        [TestFixtureTearDown]
        public void TearDownTest()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            // Switch Theme Back to Clean Slate
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Theme"))).SelectByValue("cleanSlate.xml");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            this.LogoutUser();
        }

        /// <summary>
        /// Check if all Mobile Pages work without throwing an Error Test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Check if all Mobile Pages work without throwing an Error Test.")]
        public void Check_Mobile_Pages_Test()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Profile"), "Edit Profile is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("Select your preferred theme"), "Changing the Theme is disabled for Users");

            // Switch Theme to "Black Grey"
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Theme"))).SelectByValue("YafMobile.xml");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            this.browser.Refresh();

            Assert.IsNotNull(this.browser.Link(Find.ByUrl(new Regex("Themes/Yafmobile/theme.css"))), "Changing Forum Theme failed");

            // Now Check if each mobile page is displayed correctly
            // Check the main Page
            this.browser.GoTo(TestConfig.TestForumUrl);

            Assert.IsTrue(
                !this.browser.ContainsText("Server Error") && !this.browser.ContainsText("Forum Error"),
                "There is something wrong with the mobile forum page");

            // Check the forum Category Page
            this.browser.GoTo("{0}yaf_mytopics.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(
                !this.browser.ContainsText("Server Error") && !this.browser.ContainsText("Forum Error"),
                "There is something wrong with the Forums Category page");

            // Check the forum topic view Page
            this.browser.GoTo("{0}yaf_postst{1}.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.TestTopicID));

            Assert.IsTrue(
                !this.browser.ContainsText("Server Error") && !this.browser.ContainsText("Forum Error"),
                "There is something wrong with the posts page");

            // Check My Topics Page
            this.browser.GoTo("{0}yaf_topics{1}.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

            Assert.IsTrue(
                !this.browser.ContainsText("Server Error") && !this.browser.ContainsText("Forum Error"),
                "There is something wrong with the mobile forum category page");

            // Check the Post Message Page
            this.browser.GoTo(
                "{0}yaf_postmessage.aspx?f={1}".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

            Assert.IsTrue(
                !this.browser.ContainsText("Server Error") && !this.browser.ContainsText("Forum Error"),
                "There is something wrong with the mobile post message page");

            // Check the Send PM Page
            this.browser.GoTo("{0}yaf_pmessage.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(
                !this.browser.ContainsText("Server Error") && !this.browser.ContainsText("Forum Error"),
                "There is something wrong with the mobile send private message page");

            // Check the Profile Page
            this.browser.GoTo("{0}yaf_profile2.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(
                !this.browser.ContainsText("Server Error") && !this.browser.ContainsText("Forum Error"),
                "There is something wrong with the mobile profile page");
        }
    }
}