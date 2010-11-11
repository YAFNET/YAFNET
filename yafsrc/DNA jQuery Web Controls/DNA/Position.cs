//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.Collections;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI.WebControls;

  #endregion

  /// <summary>
  /// Position is the class define the control's bounds
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class Position
  {
    #region Constants and Fields

    /// <summary>
    /// The bottom.
    /// </summary>
    private Unit bottom;

    /// <summary>
    /// The left.
    /// </summary>
    private Unit left;

    /// <summary>
    /// The right.
    /// </summary>
    private Unit right;

    /// <summary>
    /// The top.
    /// </summary>
    private Unit top;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Bottom.
    /// </summary>
    public Unit Bottom
    {
      get
      {
        return this.bottom;
      }

      set
      {
        this.bottom = value;
      }
    }

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
    /// Gets or sets Right.
    /// </summary>
    public Unit Right
    {
      get
      {
        return this.right;
      }

      set
      {
        this.right = value;
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
    /// The to string.
    /// </summary>
    /// <returns>
    /// The to string.
    /// </returns>
    public override string ToString()
    {
      var ap = new ArrayList();
      if (!this.Top.IsEmpty)
      {
        ap.Add("top:" + this.top.Value);
      }

      if (!this.Left.IsEmpty)
      {
        ap.Add("left:" + this.left.Value);
      }

      if (!this.Right.IsEmpty)
      {
        ap.Add("right:" + this.right.Value);
      }

      if (!this.Bottom.IsEmpty)
      {
        ap.Add("bottom:" + this.Bottom.Value);
      }

      if (ap.Count > 0)
      {
        return "{" + string.Join(",", (string[])ap.ToArray(typeof(string))) + "}";
      }

      return String.Empty;
    }

    #endregion
  }
}