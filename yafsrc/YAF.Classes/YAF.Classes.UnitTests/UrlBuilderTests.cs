// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlBuilderTests.cs" company="">
//   
// </copyright>
// <summary>
//   The url builder tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Classes.UnitTests
{
  using NUnit.Framework;

  using Subtext.TestLibrary;

  /// <summary>
  /// The url builder tests.
  /// </summary>
  [TestFixture]
  public class UrlBuilderTests : UrlBuilder
  {
    #region Public Methods

    /// <summary>
    /// The treat base url test.
    /// </summary>
    [Test]
    public void TreatBaseUrlTest()
    {
      Assert.AreEqual(TreatBaseUrl("http://donkey.com/"), "http://donkey.com");
    }

    /// <summary>
    /// The treat path str test.
    /// </summary>
    [Test]
    public void TreatPathStrTest()
    {
      using (HttpSimulator simulator = new HttpSimulator("/", @"c:\inetpub\wwwroot").SimulateRequest())
      {
        Assert.AreEqual(TreatPathStr(null), @"/");
        Assert.AreEqual(TreatPathStr("~/forum"), @"/forum/");
      }
    }

    #endregion
  }
}