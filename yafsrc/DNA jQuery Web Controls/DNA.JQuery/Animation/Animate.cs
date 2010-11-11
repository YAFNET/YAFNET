//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.Collections.Generic;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.UI;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// The animate.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ParseChildren(true)]
  public class Animate
  {
    #region Constants and Fields

    /// <summary>
    /// The animates.
    /// </summary>
    private List<Animate> animates;

    /// <summary>
    /// The attributes.
    /// </summary>
    private NameValueAttributeCollection<AnimationAttribute> attributes;

    /// <summary>
    /// The easing.
    /// </summary>
    private EasingMethods easing = EasingMethods.linear;

    /// <summary>
    /// The speed.
    /// </summary>
    private Speeds speed = Speeds.Normal;

    /// <summary>
    /// The speed value.
    /// </summary>
    private int speedValue = -1;

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
    public virtual List<Animate> Animates
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

    /// <summary>
    ///   Gets the Attribute Collection of the Animation
    /// </summary>
    [Category("Data")]
    [Description("Gets the Attribute Collection of the Animation")]
    [TypeConverter(typeof(CollectionConverter))]
    [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public virtual NameValueAttributeCollection<AnimationAttribute> Attributes
    {
      get
      {
        if (this.attributes == null)
        {
          this.InitAttributes();
        }

        return this.attributes;
      }
    }

    /// <summary>
    ///   Gets/Sets the easing plugin Method of the Animation
    /// </summary>
    [Category("Action")]
    [Description("Gets/Sets the easing plugin Method of the Animation")]
    [Bindable(true)]
    public virtual EasingMethods Easing
    {
      get
      {
        return this.easing;
      }

      set
      {
        this.easing = value;
      }
    }

    /// <summary>
    /// Gets or sets OnClientCallBack.
    /// </summary>
    [Category("ClientEvents")]
    [Description(" Gets/Sets the callback client event handler.")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    public string OnClientCallBack { get; set; }

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

    #endregion

    #region Public Methods

    /// <summary>
    /// The get animation scripts.
    /// </summary>
    /// <returns>
    /// The get animation scripts.
    /// </returns>
    public virtual string GetAnimationScripts()
    {
      var scripts = new StringBuilder();
      scripts.Append(".animate(" + this.Attributes.ToJSONString() + "," + this.GetSpeed());

      if (this.Easing != EasingMethods.linear)
      {
        scripts.Append(",'" + this.easing + "'");
      }

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

    /// <summary>
    /// The init attributes.
    /// </summary>
    private void InitAttributes()
    {
      this.attributes = new NameValueAttributeCollection<AnimationAttribute>();

      // if (EnableViewState)
      // (IStateManager)attributes).TrackViewState();
    }

    #endregion
  }
}