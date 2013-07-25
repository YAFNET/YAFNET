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

namespace YAF.Tests.CoreTests
{
    using System;

    using NUnit.Framework;

    using YAF.Core.Services;
    using YAF.Core.Services.CheckForSpam;

    /// <summary>
    /// The spam client tester.
    /// </summary>
    [TestFixture]
    public class SpamClientTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// A test to check the Akismet API Key
        /// </summary>
        [Test]
        [Description("A test to check the Akismet API Key")]
        public void Akismet_Spam_Client_Verify_Key_Test()
        {
            var service = new AkismetSpamClient("XXXX", new Uri("http://www.google.com"));

            Assert.AreEqual(false, service.VerifyApiKey(), "The Verify of the API Key should be false");
        }

        /// <summary>
        /// A Test to Check for SPAM
        /// </summary>
        [Test]
        [Description("A Test to Check for SPAM")]
        public void Check_For_Spam_Test()
        {
            string result;

            Assert.IsTrue(
                BlogSpamNet.CommentIsSpam(
                    new BlogSpamComment
                    {
                        comment =
                            "beside the four [url=http://www.linkslondononline.com]links of london[/url] creatures and under [url=http://www.linkslondononline.com]links[/url] the feet of [url=http://www.linkslondononline.com]links of london jewellery[/url] the Seated [url=http://www.linkslondononline.com/sweetie-bracelets]sweetie bracelet[/url] One, as if seen through [url=http://www.linkslondononline.com/links-of-london-charms]links of london charm[/url] the transparent [url=http://www.linkslondononline.com/links-of-london-bracelets]links of london charm bracelet[/url] waters of the crystal sea",
                        ip = "147.202.45.202",
                        agent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)",
                        email = "backthismailtojerry@fastmail.fm",
                        link = "http://someone.finderinn.com",
                        name = "someone",
                        version = string.Empty,
                        options = string.Empty,
                        subject = string.Empty
                    },
                    false,
                    out result),
                "This Comment should been True (SPAM)" + result);
        }

        /// <summary>
        /// A Test to Check for Harmless SPAM
        /// </summary>
        [Test]
        [Description("A Test to Check for Harmless SPAM")]
        public void Check_For_Harmless_Test()
        {
            string result;

            Assert.IsFalse(
                BlogSpamNet.CommentIsSpam(
                    new BlogSpamComment
                        {
                            comment = "Test comment",
                            ip = "127.0.0.1",
                            agent =
                                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon; .NET CLR 1.1.4322; .NET CLR 2.0.50727)",
                            email = "johndoe@mydomain.com",
                            link = "http://www.mydomain.com",
                            name = "John Doe",
                            version = string.Empty,
                            options = "whitelist=127.0.0.1",
                            subject = string.Empty
                        },
                    false,
                    out result),
                "This Comment should been False (No SPAM)" + result);
        }

        /// <summary>
        /// A Test to Check for Bot via StopForumSpam.com API
        /// </summary>
        [Test]
        [Description("A Test to Check for Bot via StopForumSpam.com API")]
        public void Check_For_Bot_Test_via_StopForumSpam()
        {
            string responseText;
            Assert.IsTrue(
                new StopForumSpam().IsBot("84.16.230.111", "krasnhello@mail.ru", "someone", out responseText),
                "This should be a Bot" + responseText);
        }

        /// <summary>
        /// A Test to Check for Bot via BotScout.com API
        /// </summary>
        [Test]
        [Description("A Test to Check for Bot via BotScout.com API")]
        public void Check_For_Bot_Test_via_BotScout()
        {
            string responseText;

            Assert.IsTrue(
                new BotScout().IsBot("84.16.230.111", "krasnhello@mail.ru", "someone", out responseText),
                "This should be a Bot" + responseText);
        }

        /// <summary>
        /// A Test to Check for Bot via BotScout.com and StopForumSpam.com API
        /// </summary>
        [Test]
        [Description("A Test to Check for Bot via BotScout.com API and StopForumSpam.com API")]
        public void Check_For_Bot_Test()
        {
            string responseText, responseText2;
            var botScoutCheck = new BotScout().IsBot("84.16.230.111", "krasnhello@mail.ru", "someone", out responseText);
            var stopForumSpamCheck = new StopForumSpam().IsBot("84.16.230.111", "krasnhello@mail.ru", "someone", out responseText2);

            Assert.IsTrue(
                botScoutCheck && stopForumSpamCheck,
                "This should be a Bot");
        }
    }
}