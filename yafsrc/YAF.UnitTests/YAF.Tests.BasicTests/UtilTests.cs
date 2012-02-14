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

namespace YAF.Tests.BasicTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using YAF.Utils;

    /// <summary>
    /// The role membership tests.
    /// </summary>
    [TestClass]
    public class UtilTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Simple URL Parameter Parser Test
        /// </summary>
        [TestMethod]
        [Description("Simple URL Parameter Parser Test")]
        public void Simple_URL_Parameter_Parser_Test()
        {
            var parser = new SimpleURLParameterParser("g=forum&t=1&url=&pg=43#cool");

            Assert.AreEqual(true, parser.HasAnchor, "Url should have an Anchor");
            Assert.AreEqual("cool", parser.Anchor, "The Url should be called : cool");
            Assert.AreEqual("forum", parser.Parameters["g"], "The Parameter g should equals forum");
            Assert.AreEqual("url=&pg=43", parser.CreateQueryString(new[] { "g", "t" }), "The Parameter g should equals forum");
            Assert.AreEqual("43", parser.Parameters[3], "The Parameter Index 3 should be 43");
        }
    }
}
