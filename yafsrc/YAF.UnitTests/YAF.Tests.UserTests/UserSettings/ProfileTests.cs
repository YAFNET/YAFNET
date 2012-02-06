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
    using YAF.Utils;

    /// <summary>
    /// The User Profile tests.
    /// </summary>
    [TestFixture]
    public class ProfileTests : TestBase
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
        /// Changes the forum language test.
        /// </summary>
        [Test]
        public void Change_Forum_Language_Test()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Profile"), "Edit Profile is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("What language do you want to use"), "Changing the Language is disabled for Users");

            // Switch Language to German
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Culture"))).SelectByValue("de-DE");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            this.browser.Refresh();

            Assert.IsTrue(this.browser.ContainsText("Angemeldet als"), "Changing Language failed");

            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            // Switch Language Back to English
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Culture"))).SelectByValue("en-US");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();
        }

        /// <summary>
        /// Changes the forum theme test.
        /// </summary>
        [Test]
        public void Change_Forum_Theme_Test()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Profile"), "Edit Profile is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("Select your preferred theme"), "Changing the Theme is disabled for Users");

            // Switch Theme to "Black Grey"
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Theme"))).SelectByValue("BlackGrey.xml");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            this.browser.Refresh();

            Assert.IsNotNull(this.browser.Link(Find.ByUrl(new Regex("Themes/BlackGrey/theme.css"))), "Changing Forum Theme failed");

            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            // Switch Theme Back to Clean Slate
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Theme"))).SelectByValue("cleanSlate.xml");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            Assert.IsNotNull(this.browser.Link(Find.ByUrl(new Regex("Themes/cleanSlate/theme.css"))), "Changing Forum Theme failed");
        }

        /// <summary>
        /// Change the forum user text editor test.
        /// </summary>
        [Test]
        public void Change_Forum_User_TextEditor_Test()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Profile"), "Edit Profile is not available for that User");

            Assert.IsTrue(
                this.browser.ContainsText("Select your text editor"), "Changing the TextEditor is disabled for Users");

            // Switch Editor to CKEditor (BBCode)
            this.browser.SelectList(Find.ById(new Regex("ProfileEditor_ForumEditor"))).SelectByValue("-1743422651");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            // Check if Editor Is Correct
            this.browser.GoTo("{0}yaf_postmessage.aspx?f={1}".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

            Assert.IsNotNull(this.browser.Table(Find.ByClass("cke_editor")), "Changing Text Editor Failed failed");

            // Switch Editor Back
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            this.browser.SelectList(Find.ById(new Regex("ProfileEditor_ForumEditor"))).SelectByValue("1");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            // Check if Editor Is Correct
            Assert.IsNotNull(this.browser.Image(Find.ByClass("ButtonOut")), "Changing Text Editor Failed failed");
        }

        /// <summary>
        /// Change the user email address test.
        /// </summary>
        [Test]
        public void Change_User_Email_Address_Test()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Profile"), "Edit Profile is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("Change Email Address"), "Changing the Email Address is disabled for Users");

            // Switch Theme to "Black Grey"
            var emailInput = this.browser.TextField(Find.ById(new Regex("_ProfileEditor_Email")));

            var oldEmailAddress = emailInput.Text;
            const string NewEmailAddress = "testmail123@localhost.com";

            emailInput.TypeText(NewEmailAddress);

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            this.browser.GoTo("{0}yaf_cp_profile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText(NewEmailAddress), "Email Address Changing failed");

            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            // Switch Email Address back
            this.browser.TextField(Find.ById(new Regex("_ProfileEditor_Email"))).TypeText(oldEmailAddress);

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            this.browser.GoTo("{0}yaf_cp_profile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText(oldEmailAddress), "Email Address Changing back failed");
        }

        /// <summary>
        /// Set the user birthday via jQuery DatePicker Plugin test.
        /// </summary>
        [Test]
        public void Set_User_Birthday_Test()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Profile"), "Edit Profile is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("Birthday"), "Birthday Selector is disabled for Users");

            // Switch Theme to "Black Grey"
            var birthdayInput = this.browser.TextField(Find.ById(new Regex("_ProfileEditor_Birthday")));
            birthdayInput.TypeText("1/1/2001");

            birthdayInput.Click();

            // Select Month
            this.browser.SelectList(Find.ByClass("ui-datepicker-month")).SelectByValue("9");

            // Select Year
            this.browser.SelectList(Find.ByClass("ui-datepicker-year")).SelectByValue("1991");
            this.browser.SelectList(Find.ByClass("ui-datepicker-year")).SelectByValue("1982");

            // Select Day
            this.browser.Link(Find.ByText("12")).Parent.Click();

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();

            this.browser.Refresh();

            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.TextField(Find.ById(new Regex("_ProfileEditor_Birthday"))).Text.Equals("10/12/1982"));

            // Change Back
            birthdayInput.TypeText("1/1/2001");
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();
        }

        /// <summary>
        /// Set the user country and region test.
        /// </summary>
        [Test]
        public void Set_User_Country_And_Region_Test()
        {
            this.browser.GoTo("{0}yaf_cp_editprofile.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Profile"), "Edit Profile is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("Country"), "Changing the Country is disabled for Users");

            // Switch Country to "Germany"
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Country"))).SelectByValue("DE");

            // Switch Region to "Berlin"
            this.browser.SelectList(Find.ById(new Regex("_ProfileEditor_Region"))).SelectByValue("BER");

            // Save the Profile Changes
            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateProfile"))).Click();
        }
    }
}
