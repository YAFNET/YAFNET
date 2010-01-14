// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilTests.cs" company="">
//   
// </copyright>
// <summary>
//   The role membership tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Classes.UnitTests
{
  using NUnit.Framework;

  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// The role membership tests.
  /// </summary>
  [TestFixture]
  public class RoleMembershipTests
  {
    #region Public Methods

    /// <summary>
    /// The role in role array test.
    /// </summary>
    [Test]
    public void RoleInRoleArrayTest()
    {
      string[] roleArray = { "role1", "role2" };

      Assert.AreEqual(RoleMembershipHelper.RoleInRoleArray("role2", roleArray), true);
      Assert.AreEqual(RoleMembershipHelper.RoleInRoleArray("norole", roleArray), false);
    }

    /// <summary>
    /// The simple url parameter parser test.
    /// </summary>
    [Test]
    public void SimpleURLParameterParserTest()
    {
      var parser = new SimpleURLParameterParser("g=forum&t=1&url=&pg=43#cool");

      Assert.IsTrue(parser.HasAnchor);
      Assert.AreEqual(parser.Anchor, "cool");
      Assert.AreEqual(parser.Parameters["g"], "forum");
      Assert.AreEqual(parser.CreateQueryString(new[] { "g", "t" }), "url=&pg=43");
      Assert.IsNotNull(parser.Parameters[3]);
      Assert.AreEqual(parser.Parameters[3], "43");
    }

    #endregion
  }
}