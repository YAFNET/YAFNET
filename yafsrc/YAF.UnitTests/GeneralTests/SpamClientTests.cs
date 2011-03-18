/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
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

namespace YAF.UnitTests.GeneralTests
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
        /// A test for SimpleURLParameterParserTest
        /// </summary>
        [TestMethod]
        public void akismet_spam_client_verify_key_is_not_valid()
        {
            var service = new AkismetSpamClient("XXXX", new Uri("http://www.google.com"));

            Assert.AreEqual(false, service.VerifyApiKey(), "The Verify of the API Key should be false");
        }

        /// <summary>
        /// A Test to Check for SPAM
        /// </summary>
        [TestMethod]
        public void CheckForSpam()
        {
            Assert.IsTrue(
                BlogSpamNet.CommentIsSpam(
                    new BlogSpamComment
                        {
                            comment =
                                "A friend of mine told me about this place. I'm just wondering if this thing works.   You check this posts everyday incase <a href=\"http://someone.finderinn.com\">find someone</a> needs some help?  I think this is a great job! Really nice place http://someone.finderinn.com here.   I found a lot of interesting stuff all around.  I enjoy beeing here and i'll come back soon. Many greetings.",
                            ip = "147.202.45.202",
                            agent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)",
                            email = "backthismailtojerry@fastmail.fm",
                            link = "http://someone.finderinn.com",
                            name = "someone",
                            version = String.Empty,
                            options = String.Empty,
                            subject = String.Empty
                        },
                    false),
                "This Comment should been True (SPAM)");
        }

        /// <summary>
        /// A Test to Check for Harmless SPAM
        /// </summary>
        [TestMethod]
        public void CheckForHarmless()
        {
            Assert.IsFalse(
                BlogSpamNet.CommentIsSpam(
                    new BlogSpamComment
                        {
                            comment = "Test comment",
                            ip = "127.0.0.1",
                            agent =
                                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon; .NET CLR 1.1.4322; .NET CLR 2.0.50727)",
                            email = "thewatcher@watchersnet.de",
                            link = "http://www.www.watchersnet.de",
                            name = "Ingo Herbote",
                            version = String.Empty,
                            options = "whitelist=127.0.0.1",
                            subject = String.Empty
                        },
                    false),
                "This Comment should been False (No SPAM)");
        }
    }
}