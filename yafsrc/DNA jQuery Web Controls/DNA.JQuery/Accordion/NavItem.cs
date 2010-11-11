//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Web;

  #endregion

  /// <summary>
  /// The nav item.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class NavItem : SimpleListItem
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NavItem"/> class.
    /// </summary>
    public NavItem()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavItem"/> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public NavItem(string text)
    {
      this.Text = text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavItem"/> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="navigateUrl">
    /// The navigate url.
    /// </param>
    public NavItem(string text, string navigateUrl)
    {
      this.Text = text;
      this.NavigateUrl = navigateUrl;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavItem"/> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="naviageUrl">
    /// The naviage url.
    /// </param>
    /// <param name="imageUrl">
    /// The image url.
    /// </param>
    public NavItem(string text, string naviageUrl, string imageUrl)
    {
      this.Text = text;
      this.NavigateUrl = naviageUrl;
      this.ImageUrl = imageUrl;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the user data
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object Data
    {
      get
      {
        object obj = this.ViewState["Data"];
        return (obj == null) ? null : obj;
      }

      set
      {
        this.ViewState["Data"] = value;
      }
    }

    /// <summary>
    ///   Gets the index for NavItem
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Index { get; set; }

    /// <summary>
    ///   Gets/Sets the NavItem client client event handler
    /// </summary>
    [Category("Behavior")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Description("Gets/Sets the NavItem client client event handler")]
    public string OnClientClick
    {
      get
      {
        object obj = this.ViewState["OnClientClick"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["OnClientClick"] = value;
      }
    }

    #endregion
  }
}