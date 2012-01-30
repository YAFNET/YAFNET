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

    using YAF.Core;

    /// <summary>
    /// The role membership tests.
    /// </summary>
    [TestClass]
    public class RoleMembershipTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Role In Role Array Test
        /// </summary>
        [TestMethod]
        [Description("Role In Role Array Test")]
        public void Role_In_Role_Array_Test()
        {
            string[] roleArray = { "role1", "role2" };

            Assert.AreEqual(true, RoleMembershipHelper.RoleInRoleArray("role2", roleArray), "'role2' should be in Role Array");
            Assert.AreEqual(false, RoleMembershipHelper.RoleInRoleArray("norole", roleArray), "'norole' should not in the Role Array");
        }
    }
}