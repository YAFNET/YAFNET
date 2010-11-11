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
  using System.Web.UI;

  using DNA.UI.JQuery.Design;

  #endregion

  /// <summary>
  /// The effect base.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [Designer(typeof(NoneUIControlDesigner))]
  public abstract class EffectBase : Control
  {
    // private string targetID = "";
    #region Constants and Fields

    /// <summary>
    /// The speed.
    /// </summary>
    private Speeds speed = Speeds.Normal;

    /// <summary>
    /// The speed value.
    /// </summary>
    private int speedValue = -1;

    // private string triggerID = "";
    /// <summary>
    /// The target.
    /// </summary>
    private JQuerySelector target;

    /// <summary>
    /// The trigger.
    /// </summary>
    private JQuerySelector trigger;

    /// <summary>
    /// The trigger event.
    /// </summary>
    private DomEvents triggerEvent = DomEvents.Click;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the animation/effect speed value by string
    /// </summary>
    [Category("Data")]
    [Description("Gets/Sets the animation/effect speed value by string")]
    [Bindable(true)]
    public Speeds Speed
    {
      get
      {
        return this.speed;
      }

      set
      {
        this.speed = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the animation/effect speed value
    /// </summary>
    [Category("Data")]
    [Description("Gets/Sets the animation/effect speed value")]
    [Bindable(true)]
    public int SpeedValue
    {
      get
      {
        return this.speedValue;
      }

      set
      {
        this.speedValue = value;
      }
    }

    /// <summary>
    ///   Gets/Sets which control that the effect apply to
    /// </summary>
    [Bindable(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Action")]
    public JQuerySelector Target
    {
      get
      {
        if (this.target == null)
        {
          this.target = new JQuerySelector();
        }

        return this.target;
      }

      set
      {
        this.target = value;
      }
    }

    /// <summary>
    ///   Gets/Sets which control to trigger the effect
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Action")]
    public JQuerySelector Trigger
    {
      get
      {
        if (this.trigger == null)
        {
          this.trigger = new JQuerySelector();
        }

        return this.trigger;
      }

      set
      {
        this.trigger = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the trigger event type
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the trigger event type")]
    public DomEvents TriggerEvent
    {
      get
      {
        return this.triggerEvent;
      }

      set
      {
        this.triggerEvent = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get speed.
    /// </summary>
    /// <returns>
    /// The get speed.
    /// </returns>
    protected string GetSpeed()
    {
      if (this.speedValue == -1)
      {
        return "'" + this.Speed.ToString().ToLower() + "'";
      }

      return this.speedValue.ToString();
    }

    #endregion
  }
}