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

namespace YAF.Tests.CoreTests.Formatting
{
    using NUnit.Framework;

    using YAF.Core;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;

    /// <summary>
    /// BB Code Formatting Tests
    /// </summary>
    [TestFixture]
    public class BBCodeTests
    {
        #region Properties

        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        /// <summary>
        /// Formats the BB code strong to HTML test.
        /// </summary>
        [Test]
        [Description("Formats the BB code strong to HTML.")]
        public void Format_BBCode_Strong_To_HTML_Test()
        {
            var testMessage = YafContext.Current.Get<IFormatMessage>()
                                     .FormatMessage("[B]test[/B]", new MessageFlags(MessageFlags.Flags.IsBBCode));

            Assert.AreEqual(
                "<strong>test</strong>",
                testMessage,
                testMessage);
        }
    }
}
