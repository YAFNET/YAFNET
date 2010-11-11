//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  #region Using

  using System.ComponentModel;

  #endregion

  /// <summary>
  /// DateName object reparents the name of the date
  /// </summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class DateName
  {
    #region Constants and Fields

    /// <summary>
    /// The name.
    /// </summary>
    private string name;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the name of the date
    /// </summary>
    [Category("Data")]
    [Description("Gets/Sets the name of the date")]
    public string Name
    {
      get
      {
        return this.name;
      }

      set
      {
        this.name = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The to string.
    /// </returns>
    public override string ToString()
    {
      return this.name;
    }

    #endregion
  }
}