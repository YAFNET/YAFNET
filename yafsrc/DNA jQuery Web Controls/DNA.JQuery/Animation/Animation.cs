//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Drawing;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.UI;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// The animation.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Animation runat=\"server\" ID=\"Animation1\"></{0}:Animation>")]
  [ToolboxBitmap(typeof(Animation), "Animation.Animation.ico")]
  [ParseChildren(true)]
  public class Animation : EffectBase
  {
    #region Constants and Fields

    /// <summary>
    /// The animates.
    /// </summary>
    private List<Animate> animates;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the animation sequence of this animation
    /// </summary>
    [Category("Action")]
    [Description("Gets the Attribute Collection of the Animation")]
    [TypeConverter(typeof(CollectionConverter))]
    [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [MergableProperty(false)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public List<Animate> Animates
    {
      get
      {
        if (this.animates == null)
        {
          this.animates = new List<Animate>();
        }

        return this.animates;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      if (this.Animates.Count > 0)
      {
        var scripts = new StringBuilder();
        if (!this.Target.IsEmpty)
        {
          scripts.Append(this.Target.ToString(this.Page));
        }

        foreach (Animate a in this.Animates)
        {
          scripts.Append(a.GetAnimationScripts());
        }

        // if (!string.IsNullOrEmpty(targetFunction))
        // {
        // scripts.Append("." + targetFunction+"()" );//+ "(function(){");
        // //scripts.Append("jQuery(this)");
        // }

        // scripts.Append(".animate(" + Attributes.ToJSONString() + "," + GetSpeed());

        // if (Easing != EasingMethods.linear)
        // scripts.Append(",'"+easing.ToString()+"'");

        // if (!string.IsNullOrEmpty(OnClientCallBack))
        // scripts.Append("," + ClientScriptManager.FormatFunctionString(OnClientCallBack));

        // scripts.Append(");");

        // if (scripts.Length > 0)
        // {
        if (!this.Trigger.IsEmpty)
        {
          scripts.Insert(
            0, this.Trigger.ToString(this.Page) + ".bind('" + this.TriggerEvent.ToString().ToLower() + "',function(){");
          scripts.Append("});");
        }

        ClientScriptManager.RegisterJQuery(this);

        // if (Easing != EasingMethods.linear)
        ClientScriptManager.AddCompositeScript(
          this, new ScriptReference("jQueryNet.plugins.easing.1.3.js", "jQueryNet"));
        ClientScriptManager.RegisterClientApplicationLoadScript(this, scripts.ToString());

        // }

        // base.OnPreRender(e);
      }
    }

    #endregion
  }
}