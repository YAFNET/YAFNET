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

namespace YAF.Tests.UserTests.UserSettings
{
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Tests.Utils;
    using YAF.Utils;

    /// <summary>
    /// The Signature tests.
    /// </summary>
    [TestFixture]
    public class SignatureTests : TestBase
    {
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
        /// Change the user signature test.
        /// </summary>
        [Test]
        public void Change_User_SignatureTest()
        {
            this.browser.GoTo("{0}yaf_cp_signature.aspx".FormatWith(TestConfig.TestForumUrl));

            Assert.IsTrue(this.browser.ContainsText("Edit Signature"), "Edit Signature is not available for that User");

            // Enter Signature
            this.browser.TextField(Find.ById(new Regex("_SignatureEditor_YafTextEditor"))).TypeText(
                "This is a Test Signature created by an Unit Test");

            this.browser.Button(Find.ById(new Regex("_SignatureEditor_preview"))).Click();

            var previewCell = this.browser.TableCell(Find.ById(new Regex("_SignatureEditor_PreviewLine")));

            Assert.IsTrue(
                previewCell.InnerHtml.Contains("This is a Test Signature created by an Unit Test"),
                "Signature changing failed");
        }
    }
}
