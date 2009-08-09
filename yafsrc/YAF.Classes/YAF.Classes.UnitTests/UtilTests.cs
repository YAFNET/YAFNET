using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions;
using YAF.Classes.Core;
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

		[Test]
		public void SimpleURLParameterParserTest()
		{
			SimpleURLParameterParser parser = new SimpleURLParameterParser( "g=forum&t=1&url=&pg=43#cool" );

			Assert.IsTrue( parser.HasAnchor );
			Assert.AreEqual( parser.Anchor, "cool" );
			Assert.AreEqual( parser.Parameters["g"], "forum" );
			Assert.AreEqual( parser.CreateQueryString( new string[] { "g", "t" } ), "url=&pg=43" );
			Assert.IsNotNull( parser.Parameters[3] );
			Assert.AreEqual( parser.Parameters[3], "43" );
		}
	}
}
