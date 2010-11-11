//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery.Design
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Web.UI.Design;

  #endregion

  /// <summary>
  /// The none ui control designer.
  /// </summary>
  public class NoneUIControlDesigner : ControlDesigner
  {
    #region Constants and Fields

    /// <summary>
    /// The none ui control.
    /// </summary>
    private IComponent noneUIControl;

    #endregion

    #region Public Methods

    /// <summary>
    /// The get design time html.
    /// </summary>
    /// <returns>
    /// The get design time html.
    /// </returns>
    public override string GetDesignTimeHtml()
    {
      return this.CreatePlaceHolderDesignTimeHtml();
    }

    /// <summary>
    /// The get persist inner html.
    /// </summary>
    /// <returns>
    /// The get persist inner html.
    /// </returns>
    public override string GetPersistInnerHtml()
    {
      var host = (IDesignerHost)this.Component.Site.GetService(typeof(IDesignerHost));
      if (host != null)
      {
        return ControlPersister.PersistInnerProperties(this.noneUIControl, host);
      }

      return String.Empty;
    }

    /// <summary>
    /// Initialize the NonUIControlDesigner
    /// </summary>
    /// <param name="component">
    /// </param>
    public override void Initialize(IComponent component)
    {
      this.noneUIControl = component;
      base.Initialize(component);
    }

    #endregion
  }
}