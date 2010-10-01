namespace YAF.Classes.UnitTests
{
  using System;

  using NUnit.Framework;

  using YAF.Classes.Core.Services;

  [TestFixture]
  public class SpamClientTester
  {
    [Test] 
    public void akismet_spam_client_verify_key_is_not_valid()
    {
      var service = new AkismetSpamClient("XXXX", new Uri("http://www.google.com"));

      Assert.AreEqual(service.VerifyApiKey(), false);
    }
  }
}