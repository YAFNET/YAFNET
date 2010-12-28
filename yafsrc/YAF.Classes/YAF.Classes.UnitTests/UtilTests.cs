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
  using Xunit;
  using Xunit.Should;

  using YAF.Core;
  using YAF.Utils;

  /// <summary>
  /// The role membership tests.
  /// </summary>
  public class RoleMembershipTests
  {
    #region Public Methods

    /// <summary>
    /// The role in role array test.
    /// </summary>
    [Fact]
    public void RoleInRoleArrayTest()
    {
      string[] roleArray = { "role1", "role2" };

      RoleMembershipHelper.RoleInRoleArray("role2", roleArray).ShouldBeTrue();
      RoleMembershipHelper.RoleInRoleArray("norole", roleArray).ShouldBeFalse();
    }

    /// <summary>
    /// The simple url parameter parser test.
    /// </summary>
    [Fact]
    public void SimpleURLParameterParserTest()
    {
      var parser = new SimpleURLParameterParser("g=forum&t=1&url=&pg=43#cool");

      parser.HasAnchor.ShouldBeTrue();
      parser.Anchor.ShouldBe("cool");
      parser.Parameters["g"].ShouldBe("forum");
      parser.CreateQueryString(new[] { "g", "t" }).ShouldBe("url=&pg=43");
      parser.Parameters[3].ShouldBe("43");
    }

    #endregion
  }
}