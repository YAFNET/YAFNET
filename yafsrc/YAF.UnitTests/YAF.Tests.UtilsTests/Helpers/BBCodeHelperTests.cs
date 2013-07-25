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

namespace YAF.Tests.UtilsTests.Helpers
{
    using NUnit.Framework;

    using YAF.Utils.Helpers;

    /// <summary>
    /// YAF.Utils BBCodeHelper Tests
    /// </summary>
    [TestFixture]
    public class BBCodeHelperTests
    {
        /// <summary>
        /// Strips all BBCodes from a string.
        /// </summary>
        [Test]
        [Description("Strips all BBCodes from a string.")]
        public void StripBBCode_Test()
        {
            const string testMessage =
                "This is a test text containing [b]bold[/b] and [i]italic[i] text and other bbcodes [img]http://test.com/testimage.jpg[/img]";

            Assert.AreEqual(
                "This is a test text containing bold and italic text and other bbcodes http://test.com/testimage.jpg",
                BBCodeHelper.StripBBCode(testMessage));
        }

        /// <summary>
        /// Strips all BBCode Quotes (including the Quote Text) from a string.
        /// </summary>
        [Test]
        [Description("Strips all BBCode Quotes (including the Quote Text) from a string.")]
        public void StripBBCodeQuotes_Test()
        {
            const string testMessage = "This is a test text containing a[quote] Quoted Message[/quote].";

            Assert.AreEqual("This is a test text containing a.", BBCodeHelper.StripBBCodeQuotes(testMessage));
        }

        /// <summary>
        /// Strips all BBCode Urls from a string.
        /// </summary>
        [Test]
        [Description("Strips all BBCode Urls from a string.")]
        public void StripBBCodeUrls_Test()
        {
            const string testMessage =
                "This is a test text containing an URL: [url]http://test.com/[/url] and [url=http://test.com/]test url[/url].";

            Assert.AreEqual(
                "This is a test text containing an URL:  and .",
                BBCodeHelper.StripBBCodeUrls(testMessage));
        }
    }
}
