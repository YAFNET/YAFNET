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

namespace YAF.Tests.BasicTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using YAF.Core.Services;
    using YAF.Core.Services.CheckForSpam;

    /// <summary>
    /// The spam client tester.
    /// </summary>
    [TestClass]
    public class SpamClientTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// A test to check the Akismet API Key
        /// </summary>
        [TestMethod]
        [Description("A test to check the Akismet API Key")]
        public void Akismet_Spam_Client_Verify_Key_Test()
        {
            var service = new AkismetSpamClient("XXXX", new Uri("http://www.google.com"));

            Assert.AreEqual(false, service.VerifyApiKey(), "The Verify of the API Key should be false");
        }

        /// <summary>
        /// A Test to Check for SPAM
        /// </summary>
        [TestMethod]
        [Description("A Test to Check for SPAM")]
        public void Check_For_Spam_Test()
        {
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
                    false),
                "This Comment should been True (SPAM)");
        }

        /// <summary>
        /// A Test to Check for Harmless SPAM
        /// </summary>
        [TestMethod]
        [Description("A Test to Check for Harmless SPAM")]
        public void Check_For_Harmless_Test()
        {
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
                    false),
                "This Comment should been False (No SPAM)");
        }
    }
}