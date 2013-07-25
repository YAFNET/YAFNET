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
    using System.IO;
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.DialogHandlers;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    /// The user album tests.
    /// </summary>
    [TestFixture]
    public class AlbumTests : TestBase
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
            this.browser = !TestConfig.UseExistingInstallation ? TestSetup._testBase.IEInstance : new IE();

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
        /// Add new user album test.
        /// </summary>
        [Test]
        public void Add_New_User_Album_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            // Add New Album
            var addAlbumButton = this.browser.Button(Find.ById(new Regex("_AddAlbum")));

            Assert.IsNotNull(addAlbumButton, "User has already reached max. Album Limit");

            addAlbumButton.Click();

            // Album Title
            this.browser.TextField(Find.ById(new Regex("_txtTitle"))).TypeText("TestAlbum");
            this.browser.Button(Find.ById(new Regex("UpdateTitle"))).Click();

            // Test Image
            string filePath = Path.GetFullPath(@"..\..\testfiles\avatar.png");
            this.browser.FileUpload(Find.ById(new Regex("_File"))).Set(filePath);
            this.browser.Button(Find.ById(new Regex("_Upload"))).Click();

            this.browser.Button(Find.ById(new Regex("_Back"))).Click();

            Assert.IsTrue(
                this.browser.ContainsText("{0} Album: TestAlbum".FormatWith(TestConfig.TestUserName)),
                "New Album Creating Failed");
        }

        /// <summary>
        /// Delete the user album test.
        /// </summary>
        [Test]
        public void Delete_User_Album_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            // Add New Album
            var edit = this.browser.Button(Find.ById(new Regex("_Albums_Edit_0")));

            Assert.IsNotNull(edit, "Albums doesn't exists.");

            edit.Click();

            Assert.IsTrue(this.browser.ContainsText("Add/Edit Album"));

            var delete = this.browser.Button(Find.ById(new Regex("_Delete")));

            var confirmDialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(this.browser.DialogWatcher, confirmDialog))
            {
                delete.ClickNoWait();
                confirmDialog.WaitUntilExists();
                confirmDialog.OKButton.Click();
                this.browser.WaitForComplete();
            }

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Album deleting failed");
        }

        /// <summary>
        /// Add an additional image test.
        /// </summary>
        [Test]
        public void Add_Additional_Image_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            Assert.IsNotNull(this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))), "Albums doesnt exists.");

            // Edit Album
            this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Add/Edit Album"));

            // Another Test Image
            string filePath = Path.GetFullPath(@"..\..\testfiles\testImage.jpg");
            this.browser.FileUpload(Find.ById(new Regex("_File"))).Set(filePath);
            this.browser.Button(Find.ById(new Regex("_Upload"))).Click();

            Assert.IsTrue(this.browser.ContainsText("testImage.jpg"), "Image Adding Failed");
        }

        /// <summary>
        /// Deletes an image from album test.
        /// </summary>
        [Test]
        public void Delete_Image_From_Album_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            Assert.IsNotNull(this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))), "Albums doesnt exists.");

            // Edit Album
            this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))).Click();

            // Get The Images Count
            var textOld = this.browser.Span(Find.ById(new Regex("_imagesInfo"))).InnerHtml;

            Assert.IsTrue(this.browser.ContainsText("Add/Edit Album"));

            var delete = this.browser.Link(Find.ById(new Regex("_List_ImageDelete_0")));

            var confirmDialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(this.browser.DialogWatcher, confirmDialog))
            {
                delete.ClickNoWait();
                confirmDialog.WaitUntilExists();
                confirmDialog.OKButton.Click();
                this.browser.WaitForComplete();
            }

            var textNew = this.browser.Span(Find.ById(new Regex("_imagesInfo"))).InnerHtml;

            Assert.AreNotEqual(textNew, textOld, "Image deleting failed");
        }

        /// <summary>
        /// Set the image as cover test.
        /// </summary>
        [Test]
        public void Set_Image_As_Cover_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            Assert.IsNotNull(this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))), "Albums doesnt exists.");

            // View Album
            this.browser.Link(Find.ByUrl(new Regex(@"yaf_album\.aspx\?u"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"));

            // Set First Album Image as Cover
            var setCoverButton = this.browser.Button(Find.ById(new Regex("_AlbumImages_SetCover_0")));

            Assert.AreEqual(
                "Set as Cover",
                this.browser.Button(Find.ById(new Regex("_AlbumImages_SetCover_0"))).Text,
                "Image is already Cover");

            setCoverButton.Click();

            Assert.AreEqual(
                "Remove Cover",
                this.browser.Button(Find.ById(new Regex("_AlbumImages_SetCover_0"))).Text,
                "Set as Cover Failed");
        }

        /// <summary>
        /// Remove the image as cover test.
        /// </summary>
        [Test]
        public void Remove_Image_As_Cover_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            Assert.IsNotNull(this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))), "Albums doesnt exists.");

            // View Album
            this.browser.Link(Find.ByUrl(new Regex(@"yaf_album\.aspx\?u"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"));

            // Set First Album Image as Cover
            var setCoverButton = this.browser.Button(Find.ById(new Regex("_AlbumImages_SetCover_0")));

            Assert.AreEqual(
                "Remove Cover",
                this.browser.Button(Find.ById(new Regex("_AlbumImages_SetCover_0"))).Text,
                "Image is not set as Cover");

            setCoverButton.Click();

            Assert.AreEqual(
                "Set as Cover",
                this.browser.Button(Find.ById(new Regex("_AlbumImages_SetCover_0"))).Text,
                "Remove as Cover Failed");
        }

        /// <summary>
        /// Edit the image caption test.
        /// </summary>
        [Test]
        public void Edit_Image_Caption_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            Assert.IsNotNull(this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))), "Albums doesnt exists.");

            // View Album
            this.browser.Link(Find.ByUrl(new Regex(@"yaf_album\.aspx\?u"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"));

            var imageTitleSpan = this.browser.Span(Find.ById(new Regex("spnTitle1")));
            imageTitleSpan.Click();

            var imageTitleInput = this.browser.TextField(Find.ById(imageTitleSpan.Id.Replace("spn", "txt")));
            imageTitleInput.TypeText("TestCaption");

            imageTitleInput.KeyPress((char)13);

            this.browser.Refresh();

            Assert.IsTrue(this.browser.ContainsText("TestCaption"), "Edit Caption Failed");
        }

        /// <summary>
        /// Edit the album name test.
        /// </summary>
        [Test]
        public void Edit_Album_Name_Test()
        {
            this.browser.GoTo(
                "{0}{1}cp_profile.aspx".FormatWith(TestConfig.TestForumUrl, TestConfig.ForumUrlRewritingPrefix));

            Assert.IsTrue(this.browser.ContainsText("Edit Albums"), "Albums Feature is not available for that User");

            this.browser.Link(Find.ByUrl(new Regex(@"yaf_albums\.aspx\?"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"), "Albums Feature is not available for that User");

            Assert.IsNotNull(this.browser.Button(Find.ById(new Regex("_Albums_Edit_0"))), "Albums doesnt exists.");

            // View Album
            this.browser.Link(Find.ByUrl(new Regex(@"yaf_album\.aspx\?u"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Album Images"));

            var imageTitleSpan = this.browser.Span(Find.ByClass("albumtitle"));
            imageTitleSpan.Click();

            var imageTitleInput = this.browser.TextField(Find.ById(imageTitleSpan.Id.Replace("spn", "txt")));
            imageTitleInput.TypeText("TestAlbumNameRandom");

            imageTitleInput.KeyPress((char)13);

            this.browser.Refresh();

            Assert.IsTrue(this.browser.ContainsText("TestCaption"), "Edit Caption Failed");
        }
    }
}
