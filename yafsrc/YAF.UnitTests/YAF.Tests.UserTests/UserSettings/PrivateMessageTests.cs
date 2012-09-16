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
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.DialogHandlers;
    using WatiN.Core.Exceptions;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    /// The Private Message tests.
    /// </summary>
    [TestFixture]
    public class PrivateMessageTests : TestBase
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
        /// Send a private message test.
        /// </summary>
        [Test]
        public void Send_Private_Message_Test()
        {
            this.browser.GoTo("{0}yaf_cp_pm.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Archive"), "Private Message Function is not available for that User, or is disabled");

            var testMessage = "This is an automated Test Message generated at {0}".FormatWith(DateTime.UtcNow);

            Assert.IsTrue(this.SendPrivateMessage(testMessage), "Test Message Send Failed");

            // Read Message
            this.browser.GoTo("{0}yaf_cp_pm.aspx".FormatWith(TestConfig.TestForumUrl));

            // Get First Message
            this.browser.Link(Find.ByText("Testmessage")).Click();

            Assert.IsTrue(this.browser.ContainsText(testMessage), "Test Message Send Failed");
        }

        /// <summary>
        /// Archive a private message test.
        /// </summary>
        [Test]
        public void Archive_Private_Message_Test()
        {
            this.browser.GoTo("{0}yaf_cp_pm.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Archive"), "Private Message Function is not available for that User, or is disabled");

            // Select the First Message
            this.browser.CheckBox(Find.ById(new Regex("_ItemCheck_0"))).Click();

            this.browser.Link(Find.ById(new Regex("_MessagesView_ArchiveSelected"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Message was archived."), "Message Archiving Failed");

            this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
        }

        /// <summary>
        /// Export a private message test.
        /// </summary>
        [Test]
        public void Export_Private_Message_Test()
        {
            this.browser.GoTo("{0}yaf_cp_pm.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Archive"), "Private Message Function is not available for that User, or is disabled");

            this.browser.Link(Find.ByText("Sent Items")).Click();

            // Select the First Message
            this.browser.CheckBox(Find.ById(new Regex("_ItemCheck_0"))).Click();

            this.browser.Link(Find.ById(new Regex("_MessagesView_ExportSelected"))).Click();

            // Switch to XML Export
            this.browser.RadioButton(Find.ByValue("txt")).Click();

            var filePath = Path.GetFullPath(@"..\..\testfiles\");

            try
            {
                var download = new FileDownloadHandler(Path.Combine(filePath, "textExport.txt"));
                this.browser.AddDialogHandler(download);
                this.browser.Button(Find.ById(new Regex("_OkButton"))).ClickNoWait();
                download.WaitUntilFileDownloadDialogIsHandled(15);
                download.WaitUntilDownloadCompleted(150);
                this.browser.RemoveDialogHandler(download);
            }
            catch (WatiNException)
            {
                Assert.Pass("Test Currently doesn't work in IE9");
            }
            
            var file = new FileStream(Path.Combine(filePath, "textExport.txt"), FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(file);
            var fileContent = sr.ReadToEnd();

            sr.Close();
            file.Close();

            Assert.IsTrue(fileContent.Contains("This is an automtated Test Message generated at ."), "Message Export Failed");
        }

        /// <summary>
        /// Delete a private message test.
        /// </summary>
        [Test]
        public void Delete_Private_Message_Test()
        {
            this.browser.GoTo("{0}yaf_cp_pm.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Archive"), "Private Message Function is not available for that User, or is disabled");

            // Select the First Message
            this.browser.CheckBox(Find.ById(new Regex("_ItemCheck_0"))).Click();

            var delete = this.browser.Link(Find.ById(new Regex("_MessagesView_DeleteSelected")));

            var confirmDialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(browser.DialogWatcher, confirmDialog))
            {
                delete.ClickNoWait();
                confirmDialog.WaitUntilExists();
                confirmDialog.OKButton.Click();
                browser.WaitForComplete();
            }

            Assert.IsTrue(this.browser.ContainsText("1 message was deleted."), "Message deleting Failed");

            this.browser.Button(Find.ById(new Regex("_OkButton"))).Click();
        }
    }
}
