using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YAF.Classes.Utils;

namespace YAF.Classes.Utils.Test
{
	[TestClass]
	public class YafUtilsTest
	{
		public YafUtilsTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void TestMethodForSimpleURLParameterParser()
		{
			SimpleURLParameterParser parser = new SimpleURLParameterParser( "g=forum&t=1&url=&pg=43#cool" );

			Assert.IsTrue( parser.HasAnchor );
			Assert.AreEqual( parser.Anchor, "cool" );
			Assert.AreEqual( parser.Parameters ["g"], "forum" );
			Assert.AreEqual( parser.CreateQueryString( new string [] { "g", "t" } ) , "url=&pg=43" );
			Assert.IsNotNull( parser.Parameters [3] );
			Assert.AreEqual( parser.Parameters [3], "43" );
		}
	}
}
