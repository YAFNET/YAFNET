// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderTests.cs" company="">
//   
// </copyright>
// <summary>
//   The yaf membership provider tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Classes.UnitTests
{
  using NUnit.Framework;

  using YAF.Providers.Membership;

  /// <summary>
  /// The yaf membership provider tests.
  /// </summary>
  [TestFixture]
  public class YafMembershipProviderTests
  {
    #region Public Methods

    /// <summary>
    /// The test 193 default hash with incorrect salt.
    /// </summary>
    [Test]
    public void Test193DefaultHashWithIncorrectSalt()
    {
      string testPasswrd = ";Stupid12";
      string expectedResult = "uNBoPpKz+S46wPPVCeIFyHW0lVE=";
      string salt = "UwB5AHMAdABlAG0ALgBCAHkAdABlAFsAXQA=";
      string result = YafMembershipProvider.Hash(testPasswrd, "SHA1", salt, true, false, "none", string.Empty, false);
      Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    /// The test 193 default hash with salt.
    /// </summary>
    [Test]
    public void Test193DefaultHashWithSalt()
    {
      string testPasswrd = ";Stupid12";
      string expectedResult = "CoUcFgjFpOK1mt+vYrDtQO6IZ9I=";
      string salt = "9LeeEZnXUE81gAzeWFXCvw==";
      string result = YafMembershipProvider.Hash(testPasswrd, "SHA1", salt, true, false, "none", string.Empty, false);
      Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    /// The test 19 x password hash.
    /// </summary>
    [Test]
    public void Test19xPasswordHash()
    {
      string testPasswrd = ";Stupid12";
      string expectedResult = "32ADF4DF9AE5B5184EDB8D8BA9D65AD9";
      string result = YafMembershipProvider.Hash(testPasswrd, "MD5", string.Empty, false, true, "UPPER", string.Empty, false);
      Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    /// The test sh a 1 microsoft provider hash.
    /// </summary>
    [Test]
    public void TestSHA1MicrosoftProviderHash()
    {
      string testPasswrd = ";Stupid12";
      string expectedResult = "8dCiTK3x/mjTG6p4uO72CzNG1mk=";
      string salt = "8tX6TMKiZtmA/GwOgpf6uw==";
      string result = YafMembershipProvider.Hash(testPasswrd, "SHA1", salt, true, false, "none", string.Empty, true);
      Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    /// The test snitz password hash.
    /// </summary>
    [Test]
    public void TestSnitzPasswordHash()
    {
      string testPasswrd = ";Stupid12";
      string expectedResult = "fa679d16ac15713a5a305f45dce4295b241f6dd38e14f92daa330d599ce77d40";
      string result = YafMembershipProvider.Hash(testPasswrd, "SHA256", string.Empty, false, true, "lower", string.Empty, false);
      Assert.AreEqual(expectedResult, result);
    }

    #endregion
  }
}