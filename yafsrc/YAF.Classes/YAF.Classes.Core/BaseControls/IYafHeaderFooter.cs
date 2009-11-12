using System.Web.UI;

namespace YAF.Classes.Core
{
  /// <summary>
  /// Yaf Header Interface
  /// </summary>
  public interface IYafHeader
  {
    /// <summary>
    /// Gets or sets a value indicating whether SimpleRender.
    /// </summary>
    bool SimpleRender
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets RefreshURL.
    /// </summary>
    string RefreshURL
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets RefreshTime.
    /// </summary>
    int RefreshTime
    {
      get;
      set;
    }

    /// <summary>
    /// Gets ThisControl.
    /// </summary>
    Control ThisControl
    {
      get;
    }
  }

  /// <summary>
  /// Yaf Footer Interface
  /// </summary>
  public interface IYafFooter
  {
    /// <summary>
    /// Gets or sets a value indicating whether SimpleRender.
    /// </summary>
    bool SimpleRender
    {
      get;
      set;
    }

    /// <summary>
    /// Gets ThisControl.
    /// </summary>
    Control ThisControl
    {
      get;
    }
  }
}