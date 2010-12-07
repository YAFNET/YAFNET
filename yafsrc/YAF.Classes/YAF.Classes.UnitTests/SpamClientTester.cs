namespace YAF.Classes.UnitTests
{
  #region Using

  using System;

  using Xunit;
  using Xunit.Should;

  using YAF.Classes.Core.Services;

  #endregion

  /// <summary>
  /// The spam client tester.
  /// </summary>
  public class SpamClientTester
  {
    #region Public Methods

    /// <summary>
    /// The akismet_spam_client_verify_key_is_not_valid.
    /// </summary>
    [Fact]
    public void akismet_spam_client_verify_key_is_not_valid()
    {
      var service = new AkismetSpamClient("XXXX", new Uri("http://www.google.com"));

      service.VerifyApiKey().ShouldBeFalse();
    }

    #endregion
  }
}