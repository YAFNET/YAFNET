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
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
    /// Localization Tests
    /// </summary>
    [TestFixture]
    public class LocalizationTests
    {
        #region Properties

        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        /// <summary>
        /// Simple test to check if the localizer works.
        /// </summary>
        [Test]
        [Description("Simple test to check if the localizer works.")]
        public void Simple_Localization_Test()
        {
            var testMessage = YafContext.Current.Get<ILocalization>().GetText("TOOLBAR", "WELCOME_GUEST");

            Assert.AreEqual(
                "Welcome Guest! To enable all features please try to register or login.",
                testMessage,
                testMessage);
        }

        /// <summary>
        /// Simple test (with parameter) to check if the localizer works.
        /// </summary>
        [Test]
        [Description("Simple test (with parameter) to check if the localizer works.")]
        public void Simple_Localization_With_Parameter_Test()
        {
            var testMessage = YafContext.Current.Get<ILocalization>().GetTextFormatted("LOGGED_IN_AS", "TestUser");

            Assert.AreEqual(
                "Logged in as: {0}".FormatWith("TestUser"),
                testMessage,
                testMessage);
        }

        /// <summary>
        /// Simple test to check if the localizer works (With a specific language).
        /// </summary>
        [Test]
        [Description("Simple test to check if the localizer works (With a specific language).")]
        public void Simple_Localization_Language_Specific_Test()
        {
            var testMessage = YafContext.Current.Get<ILocalization>().GetText("TOOLBAR", "WELCOME_GUEST", "german-du.xml");

            Assert.AreEqual(
                "Willkommen, Gast!",
                testMessage,
                testMessage);
        }
    }
}