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
        /// A test for SimpleURLParameterParserTest
        /// </summary>
        [TestMethod]
        public void RoleInRoleArrayTest()
        {
            string[] roleArray = { "role1", "role2" };

            Assert.AreEqual(true, RoleMembershipHelper.RoleInRoleArray("role2", roleArray), "'role2' should be in Role Array");
            Assert.AreEqual(false, RoleMembershipHelper.RoleInRoleArray("norole", roleArray), "'norole' should not in the Role Array");
        }
    }
}