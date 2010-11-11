//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The dialog buttons.
  /// </summary>
  [Flags]
  public enum DialogButtons
  {
    /// <summary>
    /// The none.
    /// </summary>
    None = 0, 

    /// <summary>
    /// The ok.
    /// </summary>
    OK = 1, 

    /// <summary>
    /// The cancel.
    /// </summary>
    Cancel = 2, 

    /// <summary>
    /// The close.
    /// </summary>
    Close = 3
  }
}