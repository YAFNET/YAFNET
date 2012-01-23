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