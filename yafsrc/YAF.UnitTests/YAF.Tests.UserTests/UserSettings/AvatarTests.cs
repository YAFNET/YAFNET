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
    using System.IO;
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    /// The user Avatar tests.
    /// </summary>
    [TestFixture]
    public class AvatarTests : TestBase
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
        /// Select the avatar from collection test.
        /// </summary>
        [Test]
        public void Select_Avatar_From_Collection_Test()
        {
            // Go to Modify Avatar Page
            this.browser.GoTo("{0}yaf_cp_editavatar.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Modify Avatar"), "Modify Avatar is not available for that User");

            // Select an Avatar from the Avatar Collection
            Assert.IsTrue(this.browser.ContainsText("Select your Avatar from our Collection"), "Avatar Collection not available");

            this.browser.Link(Find.ById(new Regex("_ProfileEditor_OurAvatar"))).Click();

            // Select Common Category if exists
            this.browser.Link(Find.ById(new Regex("_directories_dirName_0"))).Click();

            // Select SampleAvatar.gif
            this.browser.Link(Find.ByUrl(new Regex(@"yaf_cp_editavatar\.aspx\?av"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Modify Avatar"), "Modify Avatar Failed");
        }

        /// <summary>
        /// Select the avatar from remote server test.
        /// </summary>
        [Test]
        public void Select_Avatar_From_Remote_Server_Test()
        {
            // Go to Modify Avatar Page
            this.browser.GoTo("{0}yaf_cp_editavatar.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Modify Avatar"), "Modify Avatar is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("Enter URL of Avatar on Remote Server to Use"), "Remote Avatar Url disabled");

            // Enter Test Avatar
            this.browser.TextField(Find.ById(new Regex("_ProfileEditor_Avatar"))).TypeText("http://www.gravatar.com/avatar/00000000000000000000000000000000");

            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateRemote"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Modify Avatar"), "Modify Avatar Failed");
        }

        /// <summary>
        /// Upload the avatar from computer test.
        /// </summary>
        [Test]
        public void Upload_Avatar_From_Computer_Test()
        {
            // Go to Modify Avatar Page
            this.browser.GoTo("{0}yaf_cp_editavatar.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Modify Avatar"), "Modify Avatar is not available for that User");

            Assert.IsTrue(this.browser.ContainsText("Upload Avatar from Your Computer"), "Upload Avatars disabled");

            string filePath = Path.GetFullPath(@"..\..\testfiles\avatar.png");

            // Enter Test Avatar
            this.browser.FileUpload(Find.ById(new Regex("_ProfileEditor_File"))).Set(filePath);

            this.browser.Button(Find.ById(new Regex("_ProfileEditor_UpdateUpload"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Modify Avatar"), "Modify Avatar Failed");
        }
    }
}
