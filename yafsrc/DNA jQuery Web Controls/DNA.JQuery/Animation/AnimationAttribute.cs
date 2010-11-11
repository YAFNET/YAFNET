//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Web;

  #endregion

  /// <summary>
  /// The AnimationAttribute
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class AnimationAttribute : NameValueAttribute
  {
    #region Constants and Fields

    /// <summary>
    /// The animation type.
    /// </summary>
    private AnimationAttributeNames animationType = AnimationAttributeNames.opacity;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimationAttribute"/> class.
    /// </summary>
    public AnimationAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimationAttribute"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public AnimationAttribute(string name, string value)
      : base(name, value)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the AnimationType
    /// </summary>
    [Category("Behavior")]
    [Description(" Gets/Sets the AnimationType")]
    public AnimationAttributeNames AnimationType
    {
      get
      {
        return this.animationType;
      }

      set
      {
        this.Name = value.ToString();
        this.animationType = value;
      }
    }

    #endregion
  }
}