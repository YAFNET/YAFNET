using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;
using Subtext.TestLibrary;
using TeamAgile.ApplicationBlocks.Interception.UnitTestExtensions;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Classes.UnitTests
{
	[TestFixture]
	public class UrlBuilderTests : UrlBuilder
	{
		[Test]
		public void TreatBaseUrlTest()
		{
			Assert.AreEqual( UrlBuilder.TreatBaseUrl( "http://donkey.com/" ), "http://donkey.com" );
		}

		[Test]
		public void TreatPathStrTest()
		{
			using ( HttpSimulator simulator = new HttpSimulator( "/", @"c:\inetpub\wwwroot" ).SimulateRequest() )
			{
				Assert.AreEqual( UrlBuilder.TreatPathStr( null ), @"/" );
				Assert.AreEqual( UrlBuilder.TreatPathStr( "~/forum" ), @"/forum/" );
			}
		}
	}
}
