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
    using NUnit.Framework;

    using YAF.Providers.Membership;

    /// <summary>
    /// The yaf membership provider tests.
    /// </summary>
    [TestFixture]
    public class ProviderTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// The test 193 default hash with incorrect salt.
        /// </summary>
        [Test]
        public void Test193DefaultHashWithIncorrectSalt()
        {
            const string TestPasswrd = ";Stupid12";
            const string ExpectedResult = "uNBoPpKz+S46wPPVCeIFyHW0lVE=";
            const string Salt = "UwB5AHMAdABlAG0ALgBCAHkAdABlAFsAXQA=";

            string result = YafMembershipProvider.Hash(TestPasswrd, "SHA1", Salt, true, false, "none", string.Empty, false);

            Assert.AreEqual(ExpectedResult, result, "ExpectedResult does not match the result");
        }

        /// <summary>
        /// The test 193 default hash with salt.
        /// </summary>
        [Test]
        public void Test193DefaultHashWithSalt()
        {
            const string TestPasswrd = ";Stupid12";
            const string ExpectedResult = "CoUcFgjFpOK1mt+vYrDtQO6IZ9I=";
            const string Salt = "9LeeEZnXUE81gAzeWFXCvw==";

            string result = YafMembershipProvider.Hash(TestPasswrd, "SHA1", Salt, true, false, "none", string.Empty, false);

            Assert.AreEqual(ExpectedResult, result, "ExpectedResult does not match the result");
        }

        /// <summary>
        /// The test 19 x password hash.
        /// </summary>
        [Test]
        public void Test19xPasswordHash()
        {
            const string TestPasswrd = ";Stupid12";
            const string ExpectedResult = "32ADF4DF9AE5B5184EDB8D8BA9D65AD9";

            string result = YafMembershipProvider.Hash(TestPasswrd, "MD5", string.Empty, false, true, "UPPER", string.Empty, false);

            Assert.AreEqual(ExpectedResult, result, "ExpectedResult does not match the result");
        }

        /// <summary>
        /// The test sh a 1 Microsoft provider hash.
        /// </summary>
        [Test]
        public void TestSHA1MicrosoftProviderHash()
        {
            const string TestPasswrd = ";Stupid12";
            const string ExpectedResult = "8dCiTK3x/mjTG6p4uO72CzNG1mk=";
            const string Salt = "8tX6TMKiZtmA/GwOgpf6uw==";

            string result = YafMembershipProvider.Hash(TestPasswrd, "SHA1", Salt, true, false, "none", string.Empty, true);

            Assert.AreEqual(ExpectedResult, result, "ExpectedResult does not match the result");
        }

        /// <summary>
        /// The test snitz password hash.
        /// </summary>
        [Test]
        public void TestSnitzPasswordHash()
        {
            const string TestPasswrd = ";Stupid12";
            const string ExpectedResult = "fa679d16ac15713a5a305f45dce4295b241f6dd38e14f92daa330d599ce77d40";

            string result = YafMembershipProvider.Hash(TestPasswrd, "SHA256", string.Empty, false, true, "lower", string.Empty, false);

            Assert.AreEqual(ExpectedResult, result, "ExpectedResult does not match the result");
        }
    }
}