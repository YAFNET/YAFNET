//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;

  #endregion

  /// <summary>
  /// The fade out.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class FadeOut : Animate
  {
    #region Properties

    /// <summary>
    /// Gets or sets Easing.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override EasingMethods Easing
    {
      get
      {
        return base.Easing;
      }

      set
      {
        base.Easing = value;
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
      var scripts = new StringBuilder();
      scripts.Append(".fadeOut(");
      scripts.Append(this.GetSpeed());

      if (!string.IsNullOrEmpty(this.OnClientCallBack))
      {
        scripts.Append("," + ClientScriptManager.FormatFunctionString(this.OnClientCallBack));
      }

      scripts.Append(")");

      foreach (Animate ani in this.Animates)
      {
        scripts.Append(ani.GetAnimationScripts());
      }

      return scripts.ToString();
    }

    #endregion
  }
}