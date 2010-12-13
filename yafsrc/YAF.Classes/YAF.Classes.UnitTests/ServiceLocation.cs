namespace YAF.Classes.UnitTests
{
  using Microsoft.Practices.ServiceLocation;

  using Xunit;
  using Xunit.Should;

  using YAF.Classes.Core;

  public class ServiceLocation
  {
    [Fact]
    public void Test_Service_Location()
    {
      ServiceLocator.Current.ShouldNotBeNull();

      var sendMail = ServiceLocator.Current.GetInstance<ISendMail>();

      sendMail.ShouldBeInstanceOf<YafSendMail>();
    }
  }
}