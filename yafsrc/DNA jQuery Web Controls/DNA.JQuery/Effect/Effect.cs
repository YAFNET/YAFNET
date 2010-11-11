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
  using System.Drawing;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.UI;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// The effect.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Effect runat=\"server\" ID=\"Effect1\"></{0}:Effect>")]
  [ParseChildren(true, DefaultProperty = "Attributes")]
  [ToolboxBitmap(typeof(Effect), "Effect.Effect.ico")]
  public class Effect : EffectBase
  {
    #region Constants and Fields

    /// <summary>
    /// The attributes.
    /// </summary>
    private NameValueAttributeCollection<NameValueAttribute> attributes;

    /// <summary>
    /// The effect method.
    /// </summary>
    private JQueryEffectMethods effectMethod = JQueryEffectMethods.none;

    /// <summary>
    /// The effect type.
    /// </summary>
    private JQueryEffects effectType = JQueryEffects.Slide;

    /// <summary>
    /// The on client call back.
    /// </summary>
    private string onClientCallBack = string.Empty;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Effect"/> class.
    /// </summary>
    public Effect()
    {
      this.attributes = new NameValueAttributeCollection<NameValueAttribute>();
      if (this.EnableViewState)
      {
        ((IStateManager)this.attributes).TrackViewState();
      }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the Attribute Collection of the Animation
    /// </summary>
    [Category("Data")]
    [Description("Gets the Attribute Collection of the Effect")]
    [TypeConverter(typeof(CollectionConverter))]
    [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    public NameValueAttributeCollection<NameValueAttribute> Attributes
    {
      get
      {
        return this.attributes;
      }

      private set
      {
        this.attributes = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the effect method
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the effect method")]
    [Bindable(true)]
    public JQueryEffectMethods EffectMethod
    {
      get
      {
        return this.effectMethod;
      }

      set
      {
        this.effectMethod = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the effect type
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the effect type")]
    [Bindable(true)]
    public JQueryEffects EffectType
    {
      get
      {
        return this.effectType;
      }

      set
      {
        this.effectType = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Effect client call back event handler
    /// </summary>
    [Category("ClientEvents")]
    [Description("Gets/Sets the Effect client call back event handler")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    public string OnClientCallBack
    {
      get
      {
        return this.onClientCallBack;
      }

      set
      {
        this.onClientCallBack = value;
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
      // string clientID = ClientScriptManager.GetControlClientID(Page, TargetID);
      // string trigger = ClientScriptManager.GetControlClientID(Page, TriggerID);
      var scripts = new StringBuilder();

      if (!this.Target.IsEmpty)
      {
        scripts.Append(this.Target.ToString(this.Page));

        // scripts.AppendLine("jQuery('#" + clientID + "')");
        if (this.effectMethod != JQueryEffectMethods.none)
        {
          scripts.Append("." + this.effectMethod + "('" + this.Speed.ToString().ToLower() + "')");
        }

        if (this.effectType != JQueryEffects.None)
        {
          scripts.Append(".effect(" + this.GetEffect());

          if (this.Attributes.Count > 0)
          {
            scripts.Append("," + this.Attributes.ToJSONString());
          }
          else
          {
            scripts.Append(",null");
          }

          scripts.Append("," + this.GetSpeed());

          if (!string.IsNullOrEmpty(this.onClientCallBack))
          {
            scripts.Append("," + ClientScriptManager.FormatFunctionString(this.onClientCallBack));
          }

          scripts.Append(")");
        }
      }

      if (scripts.Length > 0)
      {
        if (!scripts.ToString().EndsWith(";"))
        {
          scripts.Append(";");
        }

        ClientScriptManager.RegisterJQuery(this);
        ClientScriptManager.AddCompositeScript(this, "jQueryNet.effects.core.js", "jQueryNet");
        ClientScriptManager.AddCompositeScript(
          this, "jQueryNet.effects." + this.effectType.ToString().ToLower() + ".js", "jQueryNet");

        // ScriptManager.GetCurrent(Page).CompositeScript.Scripts.Add(new ScriptReference("jQueryNet.effects.core.js", "jQueryNet"));
        // ScriptManager.GetCurrent(Page).CompositeScript.Scripts.Add(new ScriptReference("jQueryNet.effects."+effectType.ToString().ToLower()+".js", "jQueryNet"));
        if (!this.Trigger.IsEmpty)
        {
          scripts.Insert(
            0, this.Trigger.ToString(this.Page) + ".bind('" + this.TriggerEvent.ToString().ToLower() + "',function(){");
          scripts.Append("});");
        }

        ClientScriptManager.RegisterClientApplicationLoadScript(this, scripts.ToString());
      }

      // base.OnPreRender(e);
    }

    /// <summary>
    /// The get effect.
    /// </summary>
    /// <returns>
    /// The get effect.
    /// </returns>
    private string GetEffect()
    {
      return "'" + this.effectType.ToString().ToLower() + "'";
    }

    #endregion
  }
}