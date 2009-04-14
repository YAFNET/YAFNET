using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions;
using YAF.Classes.Utils;

namespace YAF.Classes.UnitTests
{
	[TestFixture]
	public class RoleMembershipTests
	{
		[Test]
		public void RoleInRoleArrayTest()
		{
			string[] roleArray = { "role1", "role2" };

			Assert.AreEqual( RoleMembershipHelper.RoleInRoleArray( "role2", roleArray ), true );
			Assert.AreEqual( RoleMembershipHelper.RoleInRoleArray( "norole", roleArray ), false );
		}
	}
}
