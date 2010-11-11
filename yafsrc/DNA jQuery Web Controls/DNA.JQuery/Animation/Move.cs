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
  /// The move.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class Move : Animate
  {
    #region Constants and Fields

    /// <summary>
    /// The left.
    /// </summary>
    private Unit left;

    /// <summary>
    /// The top.
    /// </summary>
    private Unit top;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Left.
    /// </summary>
    public Unit Left
    {
      get
      {
        return this.left;
      }

      set
      {
        this.left = value;
      }
    }

    /// <summary>
    /// Gets or sets Top.
    /// </summary>
    public Unit Top
    {
      get
      {
        return this.top;
      }

      set
      {
        this.top = value;
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
      if (!this.Left.IsEmpty)
      {
        if (this.Attributes["left"] == null)
        {
          this.Attributes.Add(new AnimationAttribute(AnimationAttributeNames.left.ToString(), this.left.ToString()));
        }
        else
        {
          this.Attributes["left"].Value = this.left.ToString();
        }
      }

      if (!this.Top.IsEmpty)
      {
        if (this.Attributes["top"] == null)
        {
          this.Attributes.Add(new AnimationAttribute(AnimationAttributeNames.top.ToString(), this.top.ToString()));
        }
        else
        {
          this.Attributes["top"].Value = this.top.ToString();
        }
      }

      return base.GetAnimationScripts();
    }

    #endregion
  }
}