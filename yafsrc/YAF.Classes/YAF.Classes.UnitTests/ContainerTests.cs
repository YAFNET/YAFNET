namespace YAF.Classes.UnitTests
{
  #region Using

  using System;
  using System.Web;

  using Xunit;
  using Xunit.Should;

  using YAF.Core;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The container tests.
  /// </summary>
  public class ContainerTests
  {
    #region Public Methods

    /// <summary>
    /// The container_ is_ available_ from_ yaf context_ not_ in_ request.
    /// </summary>
    [Fact]
    public void Container_Is_Available_From_YafContext_Not_In_Request()
    {
      var serviceLocator = YafContext.Current.ServiceLocator;

      Exception exception = Record.Exception(() => serviceLocator.Get<HttpRequestBase>());

      exception.ShouldNotBeNull();
    }

    /// <summary>
    /// The container_ is_ available_ to_ send_ digest_ in_ background.
    /// </summary>
    [Fact]
    public void Container_Is_Available_To_Send_Digest_In_Background()
    {
      var sendTask = new DigestSendTask();

      YafContext.Current.ServiceLocator.Get<IInjectServices>().Inject(sendTask);

      sendTask.RunOnce();
    }

    #endregion
  }
}