//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI.WebControls;

  #endregion

  /// <summary>
  /// The resize.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class Resize : Animate
  {
    #region Constants and Fields

    /// <summary>
    /// The height.
    /// </summary>
    private Unit height;

    /// <summary>
    /// The width.
    /// </summary>
    private Unit width;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Height.
    /// </summary>
    public Unit Height
    {
      get
      {
        return this.height;
      }

      set
      {
        this.height = value;
      }
    }

    /// <summary>
    /// Gets or sets Width.
    /// </summary>
    public Unit Width
    {
      get
      {
        return this.width;
      }

      set
      {
        this.width = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The get animation scripts.
    /// </summary>
    /// <returns>
    /// The get animation scripts.
    /// </returns>
    public override string GetAnimationScripts()
    {
      if (!this.Height.IsEmpty)
      {
        if (this.Attributes["height"] == null)
        {
          this.Attributes.Add(new AnimationAttribute(AnimationAttributeNames.height.ToString(), this.Height.ToString()));
        }
        else
        {
          this.Attributes["height"].Value = this.Height.ToString();
        }
      }

      if (!this.Width.IsEmpty)
      {
        if (this.Attributes["width"] == null)
        {
          this.Attributes.Add(new AnimationAttribute(AnimationAttributeNames.width.ToString(), this.Width.ToString()));
        }
        else
        {
          this.Attributes["width"].Value = this.Width.ToString();
        }
      }

      return base.GetAnimationScripts();
    }

    #endregion
  }
}