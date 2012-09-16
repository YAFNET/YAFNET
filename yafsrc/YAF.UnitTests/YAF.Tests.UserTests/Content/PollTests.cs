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

namespace YAF.Tests.UserTests.Content
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.DialogHandlers;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    /// The topic poll tests.
    /// </summary>
    [TestFixture]
    public class PollTests : TestBase
    {
        /// <summary>
        /// Gets or sets Test Poll Topic Url.
        /// </summary>
        private string testPollTopicUrl = string.Empty;

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
        /// Creating a new topic with a Poll test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Creating a new topic with a Poll test.")]
        public void Create_Poll_Topic_Test()
        {
            this.CreatePollTopicTest(allowMultiVote: false, addPollImages: false);
        }

        /// <summary>
        /// Create a new Poll in an existing topic test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Create a new Poll in an existing topic test.")]
        public void Add_Poll_To_Topic_Test()
        {
            // First Creating a new test topic with the test user
            Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

            // Go to edit Page
            this.browser.Link(Find.ById(new Regex("_Edit_0"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Create Poll"), "Editing not allowed for that user");

            this.browser.Link(Find.ById(new Regex("_CreatePoll1"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Edit Poll"), "Creating Poll not possible");

            // Enter Poll Question
            this.browser.TextField(Find.ById(new Regex("_Question"))).TypeText("Is this a Test Question?");

            // Enter Poll Answer Choice 1
            this.browser.TextField(Find.ById(new Regex("_PollChoice_0"))).TypeText("Option 1");

            // Enter Poll Answer Choice 2
            this.browser.TextField(Find.ById(new Regex("_PollChoice_1"))).TypeText("Option 2");

            // Enter Poll Answer Choice 3
            this.browser.TextField(Find.ById(new Regex("_PollChoice_2"))).TypeText("Option 3");

            // Enter Poll Answer Choice 4
            this.browser.TextField(Find.ById(new Regex("_PollChoice_3"))).TypeText("Option 4");

            // Enter Poll Answer Choice 5
            this.browser.TextField(Find.ById(new Regex("_PollChoice_4"))).TypeText("Option 5");

            this.browser.CheckBox(Find.ById(new Regex("_ShowVotersCheckBox"))).Click();

            // Save Poll
            this.browser.Link(Find.ById(new Regex("_SavePoll"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Poll Question : Is this a Test Question?"), "Poll Creating Failed");

            this.testPollTopicUrl = this.browser.Url;
        }

        /// <summary>
        /// Vote in the test poll topic test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Vote in the test poll topic test.")]
        public void Vote_Poll_Test()
        {
            if (this.testPollTopicUrl.IsNotSet())
            {
                this.CreatePollTopicTest(allowMultiVote: false, addPollImages: false);
            }

            // Now Login with Test User 2
            Assert.IsTrue(this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password), "Login with test user 2 failed");

            // Go To New Test Topic Url
            this.browser.GoTo(this.testPollTopicUrl);

            Assert.IsTrue(!this.browser.ContainsText("You already voted"), "User has already voted");

            // Vote for Option 3
            this.browser.Link(Find.ById(new Regex("_MyLinkButton1_2"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Thank you for your vote!"), "Voting failed");
        }

        /// <summary>
        /// Multi Vote in the test poll topic test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Multi Vote in the test poll topic test.")]
        public void Multi_Vote_Poll_Test()
        {
            if (this.testPollTopicUrl.IsNotSet())
            {
                this.CreatePollTopicTest(allowMultiVote: true, addPollImages: false);
            }

            // Now Login with Test User 2
            Assert.IsTrue(this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password), "Login with test user 2 failed");

            // Go To New Test Topic Url
            this.browser.GoTo(this.testPollTopicUrl);

            Assert.IsTrue(!this.browser.ContainsText("You already voted"), "User has already voted");

            // Vote for Option 3
            this.browser.Link(Find.ById(new Regex("_MyLinkButton1_2"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Thank you for your vote!"), "Voting failed");

            // Vote for Option 5
            this.browser.Link(Find.ById(new Regex("_MyLinkButton1_4"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Thank you for your vote!"), "Voting failed");
        }

        /// <summary>
        /// Remove the test poll completly test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Remove the test poll completly test.")]
        public void Remove_Poll_Completly_Test()
        {
            if (this.testPollTopicUrl.IsNotSet())
            {
                this.CreatePollTopicTest(allowMultiVote: false, addPollImages: false);
            }

            // Go To New Test Topic Url
            this.browser.GoTo(this.testPollTopicUrl);

            // Go to edit Page
            this.browser.Link(Find.ById(new Regex("_Edit_0"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Remove Poll"), "Editing not allowed for that user");

            var delete = this.browser.Link(Find.ById(new Regex("_RemovePollAll_0")));

            var confirmDialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(browser.DialogWatcher, confirmDialog))
            {
                delete.ClickNoWait();
                confirmDialog.WaitUntilExists();
                confirmDialog.OKButton.Click();
                browser.WaitForComplete();
            }

            Assert.IsTrue(this.browser.ContainsText("Create Poll"), "Deleting Poll failed");
        }

        /// <summary>
        /// Create a new Topic with Poll where the Poll Question 
        /// and Answer Choices contains images test.
        /// </summary>
        [Test]
        [NUnit.Framework.Description("Create a new Topic with Poll where the Poll Question and Answer Choices contains images test.")]
        public void Create_Poll_With_Image_Options_Test()
        {
            // Now Login with Test User 2
            Assert.IsTrue(
                this.LoginUser(TestConfig.AdminUserName, TestConfig.AdminPassword),
                "Login with admin user failed failed");

            this.browser.GoTo("http://192.168.2.10/yaf/yaf_postst75_Auto-Created-Test-Topic-with-Poll.aspx");

            this.CreatePollTopicTest(allowMultiVote: false, addPollImages: true);

            Assert.IsTrue(this.browser.Image(Find.ById(new Regex("_ChoiceImage_2"))).Exists, "Image Not Found");
            Assert.IsTrue(
                this.browser.Image(Find.ById(new Regex("_ChoiceImage_2"))).Uri.Equals(
                    "http://ssl.bulix.org/projects/tig/raw-attachment/wiki/WikiStart/tig.png"),
                "Image Url does not match");
        }

        /// <summary>
        /// Creates the poll topic test.
        /// </summary>
        /// <param name="allowMultiVote">if set to <c>true</c> [allow multi vote].</param>
        /// <param name="addPollImages">if set to <c>true</c> [add poll images].</param>
        private void CreatePollTopicTest(bool allowMultiVote, bool addPollImages)
        {
            // Go to Post New Topic
            this.browser.GoTo("{0}yaf_postmessage.aspx?f={1}".FormatWith(TestConfig.TestForumUrl, TestConfig.TestForumID));

            Assert.IsTrue(this.browser.ContainsText("Post New Topic"), "Post New Topic not possible");

            Assert.IsTrue(this.browser.ContainsText("Add Poll?"), "User does doesnt have Poll Access");

            // Enable Add Poll Option
            this.browser.CheckBox(Find.ById(new Regex("_AddPollCheckBox"))).Click();

            // Create New Poll Topic
            this.browser.TextField(Find.ById(new Regex("_TopicSubjectTextBox"))).TypeText("Auto Created Test Topic with Poll");

            if (this.browser.ContainsText("Description"))
            {
                this.browser.TextField(Find.ById(new Regex("_TopicDescriptionTextBox"))).TypeText("Poll Testing");
            }

            if (this.browser.ContainsText("Status"))
            {
                this.browser.SelectList(Find.ById(new Regex("_TopicStatus"))).SelectByValue("QUESTION");
            }

            this.browser.TextField(Find.ById(new Regex("_YafTextEditor"))).TypeText("This is a test topic to test the Poll Creating");

            // Post New Topic
            this.browser.Link(Find.ById(new Regex("_PostReply"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Edit Poll"), "Topic Creating failed");

            // Enter Poll Question
            this.browser.TextField(Find.ById(new Regex("_Question"))).TypeText("Is this a Test Question?");

            // Enter Poll Question Image if exists
            if (this.browser.TextField(Find.ById(new Regex("_QuestionObjectPath"))).Exists && addPollImages)
            {
                this.browser.TextField(Find.ById(new Regex("_QuestionObjectPath"))).TypeText(
                    "http://download.codeplex.com/Download?ProjectName=yafnet&DownloadId=167900&Build=18559");
            }

            // Enter Poll Answer Choice 1
            this.browser.TextField(Find.ById(new Regex("_PollChoice_0"))).TypeText("Option 1");

            // Enter Poll Question Image if exists
            if (this.browser.TextField(Find.ById(new Regex("_ObjectPath_0"))).Exists)
            {
                this.browser.TextField(Find.ById(new Regex("_ObjectPath_0"))).TypeText(
                    "http://ssl.bulix.org/projects/tig/raw-attachment/wiki/WikiStart/tig.png");
            }

            // Enter Poll Answer Choice 2
            this.browser.TextField(Find.ById(new Regex("_PollChoice_1"))).TypeText("Option 2");

            // Enter Poll Question Image if exists
            if (this.browser.TextField(Find.ById(new Regex("_ObjectPath_1"))).Exists && addPollImages)
            {
                this.browser.TextField(Find.ById(new Regex("_ObjectPath_1"))).TypeText(
                    "http://ssl.bulix.org/projects/tig/raw-attachment/wiki/WikiStart/tig.png");
            }

            // Enter Poll Answer Choice 3
            this.browser.TextField(Find.ById(new Regex("_PollChoice_2"))).TypeText("Option 3");

            // Enter Poll Question Image if exists
            if (this.browser.TextField(Find.ById(new Regex("_ObjectPath_2"))).Exists && addPollImages)
            {
                this.browser.TextField(Find.ById(new Regex("_ObjectPath_2"))).TypeText(
                    "http://ssl.bulix.org/projects/tig/raw-attachment/wiki/WikiStart/tig.png");
            }

            // Enter Poll Answer Choice 4
            this.browser.TextField(Find.ById(new Regex("_PollChoice_3"))).TypeText("Option 4");

            // Enter Poll Question Image if exists
            if (this.browser.TextField(Find.ById(new Regex("_ObjectPath_3"))).Exists && addPollImages)
            {
                this.browser.TextField(Find.ById(new Regex("_ObjectPath_3"))).TypeText(
                    "http://ssl.bulix.org/projects/tig/raw-attachment/wiki/WikiStart/tig.png");
            }

            // Enter Poll Answer Choice 5
            this.browser.TextField(Find.ById(new Regex("_PollChoice_4"))).TypeText("Option 5");

            // Enter Poll Question Image if exists
            if (this.browser.TextField(Find.ById(new Regex("_ObjectPath_4"))).Exists && addPollImages)
            {
                this.browser.TextField(Find.ById(new Regex("_ObjectPath_5"))).TypeText(
                    "http://ssl.bulix.org/projects/tig/raw-attachment/wiki/WikiStart/tig.png");
            }

            if (allowMultiVote)
            {
                this.browser.CheckBox(Find.ById(new Regex("_AllowMultipleChoicesCheckBox"))).Click();
            }

            this.browser.CheckBox(Find.ById(new Regex("_ShowVotersCheckBox"))).Click();

            // Save Poll
            this.browser.Link(Find.ById(new Regex("_SavePoll"))).Click();

            Assert.IsTrue(this.browser.ContainsText("Poll Question : Is this a Test Question?"), "Poll Creating Failed");

            this.testPollTopicUrl = this.browser.Url;
        }
    }
}
