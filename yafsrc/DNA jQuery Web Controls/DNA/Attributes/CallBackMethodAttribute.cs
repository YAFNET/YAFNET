//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The call back method attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class CallBackMethodAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The friendly name.
    /// </summary>
    private string friendlyName = string.Empty;

    /// <summary>
    /// The has callback result.
    /// </summary>
    private bool hasCallbackResult;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets FriendlyName.
    /// </summary>
    public string FriendlyName
    {
      get
      {
        return this.friendlyName;
      }

      set
      {
        this.friendlyName = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether HasCallbackResult.
    /// </summary>
    public bool HasCallbackResult
    {
      get
      {
        return this.hasCallbackResult;
      }

      set
      {
        this.hasCallbackResult = value;
      }
    }

    #endregion
  }
}