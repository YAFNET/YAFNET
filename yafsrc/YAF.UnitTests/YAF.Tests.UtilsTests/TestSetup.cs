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

namespace YAF.Tests.CoreTests
{
    using System.IO;
    using System.Web.Security;

    using HttpSimulator;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Testing a YAF Installation
    /// </summary>
    [SetUpFixture]
    public class TestSetup
    {
        /// <summary>
        /// Sets up the mocked YAF Forum instance
        /// </summary>
        [SetUp]
        public void Preparations()
        {
            var _factory = new MockRepository(MockBehavior.Strict);
            var _membership = _factory.Create<MembershipProvider>();
            var _roleProvider = _factory.Create<RoleProvider>();

            Membership.ApplicationName = "YetAnotherForum";

            Membership.Providers.AddMembershipProvider("MyMembershipProvider", _membership.Object);

            Roles.Providers.AddRoleProvider("MyRoleProvider", _roleProvider.Object);

            var currentPath = Path.GetFullPath(@"..\..\testfiles\forum\");

            HttpSimulator simulator = new HttpSimulator("yaf/", currentPath);

            simulator.SimulateRequest();
        }

        /// <summary>
        /// Removes the fake providers.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Membership.Providers.RemoveMembershipProvider("MyMembershipProvider");

            Roles.Providers.RemoveRoleProvider("MyRoleProvider");
        }
    }
}
