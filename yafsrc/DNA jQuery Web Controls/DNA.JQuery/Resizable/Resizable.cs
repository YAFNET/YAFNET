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
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using DNA.UI.JQuery.Design;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// <para>
  /// Resizable is a ASP.NET WebControl that encapsulat the jQuery UI resizable
  ///     plugin makes selected WebControls/Html Elements resizable (meaning they have
  ///     draggable resize handles). You can specify one or more handles as well as min and
  ///     max width and height.
  /// </para>
  /// </summary>
  /// <remarks>
  /// <para>
  /// All ClientEvents (OnClientResizeStart,OnClientResizeStop,OnClientResize)
  ///     receive two arguments: The original browser event and a prepared ui object. The ui
  ///     object has the following fields:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// </item>
  /// <item>
  /// ui.helper - a jQuery object containing the helper element
  /// </item>
  /// <item>
  /// ui.originalPosition - {top, left} before resizing started
  /// </item>
  /// <item>
  /// ui.originalSize - {width, height} before resizing started
  /// </item>
  /// <item>
  /// ui.position - {top, left} current position * ui.size - {width, height}
  ///       current size
  /// </item>
  /// </list>
  /// </remarks>
  /// <example>
  /// <code lang="ASP.NET" title="Resizable Control Properties">
  /// <![CDATA[
  /// <DotNetAge:Resizable ID="MyResizable"
  ///   AnimateDuration="1000"
  ///   AnimateEasing="swing"
  ///   AspectRatio="0.7"
  ///   AutoHide="true"
  ///   Container="Parent"
  ///   EnabledAnimation="true"
  ///   Handles="e, s, se"
  ///   HelperCssClass="ui-state-highlight"
  ///   MaxHeight="0"
  ///   MinHeight="0"
  ///   MaxWidth="0"
  ///   MinWidth="0"
  ///   OnClientResize="javascript here."
  ///   OnClientResizeStart=""
  ///   OnClientResizeStop=""
  ///   ResizeStartDelay="1000"
  ///   ResizeStartDistance="10"
  ///   ShowSemiTransparentHelper="true"
  ///   SnapX="20"
  ///   SnapY="20"
  /// >
  ///   <Target TargetIDs="ControlID1, ... ,ControlIDn" 
  ///           Selector=".ui-widget-header" TargetID="ControlID"/>
  ///   <AlsoResize/>
  ///   <ContainsIn/>
  ///   <DisableResizableElements/>
  /// </DotNetAge:Resizable>]]>
  ///   </code>
  /// </example>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Resizable runat=\"server\" ID=\"Resizable1\"></{0}:Resizable>")]
  [Designer(typeof(NoneUIControlDesigner))]
  [JQuery(Name = "resizable", Assembly = "jQueryNet", ScriptResources = new[] { "ui.core.js", "ui.resizable.js" }, 
    StartEvent = ClientRegisterEvents.ApplicationLoad)]
  [ToolboxBitmap(typeof(Resizable), "Resizable.Resizable.ico")]
  [ParseChildren(true)]
  public class Resizable : Control
  {
    #region Constants and Fields

    /// <summary>
    /// The also resize.
    /// </summary>
    private JQuerySelector alsoResize;

    /// <summary>
    /// The contains in.
    /// </summary>
    private JQuerySelector containsIn;

    /// <summary>
    /// The disabled dragging elements.
    /// </summary>
    private JQuerySelector disabledDraggingElements;

    /// <summary>
    /// The handlers.
    /// </summary>
    private string[] handlers;

    /// <summary>
    /// The target.
    /// </summary>
    private JQuerySelector target = new JQuerySelector();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the resize these elements synchronous when resizing.
    /// </summary>
    [JQueryOption("alsoResize")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Gets/Sets the resize these elements synchronous when resizing.")]
    public JQuerySelector AlsoResize
    {
      get
      {
        if (this.alsoResize == null)
        {
          this.alsoResize = new JQuerySelector();
          this.alsoResize.ExpressionOnly = true;
        }

        return this.alsoResize;
      }

      set
      {
        this.alsoResize = value;
        if (value != null)
        {
          this.alsoResize.ExpressionOnly = true;
        }
      }
    }

    /// <summary>
    ///   Gets/Sets duration time for animating, in milliseconds.
    /// </summary>
    [JQueryOption("animateDuration", IgnoreValue = 0)]
    [Category("Behavior")]
    [Bindable(true)]
    [Themeable(true)]
    [Description("Gets/Sets duration time for animating, in milliseconds.")]
    public int AnimateDuration
    {
      get
      {
        object obj = this.ViewState["AnimateDuration"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["AnimateDuration"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the easing effect for animating.
    /// </summary>
    [JQueryOption("animateEasing")]
    [Category("Behavior")]
    [Bindable(true)]
    [Themeable(true)]
    [Description("Gets/Sets the easing effect for animating.")]
    public EasingMethods AnimateEasing
    {
      get
      {
        object obj = this.ViewState["AnimateEasing"];
        return (obj == null) ? EasingMethods.swing : (EasingMethods)obj;
      }

      set
      {
        this.ViewState["AnimateEasing"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets resizing is constrained by the original aspect ratio. Otherwise a custom aspect ratio
    ///   can be specified, such as 9 / 16, or 0.5.
    /// </summary>
    [JQueryOption("aspectRatio", IgnoreValue = 0)]
    [Category("Appearance")]
    [Description(
      " Gets/Sets resizing is constrained by the original aspect ratio. Otherwise a custom aspect ratio can be specified, such as 9 / 16, or 0.5."
      )]
    public float AspectRatio
    {
      get
      {
        object obj = this.ViewState["AspectRatio"];
        return (obj == null) ? 0 : (float)obj;
      }

      set
      {
        this.ViewState["AspectRatio"] = value;
      }
    }

    /// <summary>
    ///   Get/Sets whether automatically hides the handles except when the mouse hovers over the element.
    /// </summary>
    [JQueryOption("autoHide", IgnoreValue = false)]
    [Category("Behavior")]
    [Description("Get/Sets whether automatically hides the handles except when the mouse hovers over the element.")]
    public bool AutoHide
    {
      get
      {
        object obj = this.ViewState["AutoScroll"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["AutoScroll"] = value;
      }
    }

    /// <summary>
    ///   Constrains dragging to within the bounds of the specified element.
    ///   Possible string values: 'parent', 'document', 'window'
    /// </summary>
    [Category("Layout")]
    public Containers Container
    {
      get
      {
        object obj = this.ViewState["Containment"];
        return (obj == null) ? Containers.NotSet : (Containers)obj;
      }

      set
      {
        this.ViewState["Containment"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Control/Element constrains dragging to within the bounds of the specified Control/Element.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description(
      "Gets/Sets the Control/Element constrains dragging to within the bounds of the specified Control/Element.")]
    public JQuerySelector ContainsIn
    {
      get
      {
        if (this.containsIn == null)
        {
          this.containsIn = new JQuerySelector();
          this.containsIn.ExpressionOnly = true;
        }

        return this.containsIn;
      }

      set
      {
        this.containsIn = value;
        if (value != null)
        {
          this.containsIn.ExpressionOnly = true;
        }
      }
    }

    /// <summary>
    ///   Gets/Sets prevents resizing if you start on elements matching the selector.
    /// </summary>
    [JQueryOption("cancel")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Gets/Sets prevents resizing if you start on elements matching the selector.")]
    public JQuerySelector DisableResizableElements
    {
      get
      {
        if (this.disabledDraggingElements == null)
        {
          this.disabledDraggingElements = new JQuerySelector();
          this.disabledDraggingElements.ExpressionOnly = true;
        }

        return this.disabledDraggingElements;
      }

      set
      {
        this.disabledDraggingElements = value;
        if (value != null)
        {
          this.disabledDraggingElements.ExpressionOnly = true;
        }
      }
    }

    /// <summary>
    ///   Gets/Sets whether enable animates to the final size after resizing.
    /// </summary>
    [JQueryOption("animate", IgnoreValue = false)]
    [Bindable(true)]
    [Category("Behavior")]
    [Themeable(true)]
    [Description("Gets/Sets whether enable animates to the final size after resizing.")]
    public bool EnabledAnimation
    {
      get
      {
        object obj = this.ViewState["EnabledAnimation"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["EnabledAnimation"] = value;
      }
    }

    /// <summary>
    ///   If specified as a string, should be a comma-split list of any of the following: 'n, e, s, w, ne, se, sw, nw, all'. 
    ///   The necessary handles will be auto-generated by the plugin.
    ///   If specified as an object, the following keys are supported: { n, e, s, w, ne, se, sw, nw }. 
    ///   The value of any specified should be a jQuery selector matching the child element of the resizable to use
    ///   as that handle. If the handle is not a child of the resizable, you can pass in the DOMElement or a valid jQuery 
    ///   object directly.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(StringArrayConverter))]
    [Category("Behavior")]
    public string[] Handles
    {
      get
      {
        return this.handlers;
      }

      set
      {
        this.handlers = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the css class that will be added to a proxy element to outline the resize 
    ///   during the drag of the resize handle. Once the resize is complete, the original element is sized.
    /// </summary>
    [JQueryOption("helper")]
    [CssClassProperty]
    [Category("Appearance")]
    [Description(
      "Gets/Sets the css class that will be added to a proxy element to outline the resize during the drag of the resize handle. Once the resize is complete, the original element is sized."
      )]
    [Themeable(true)]
    [Bindable(true)]
    public string HelperCssClass
    {
      get
      {
        object obj = this.ViewState["HelperCssClass"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["HelperCssClass"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the maximum height the resizable should be allowed to resize to.
    /// </summary>
    [JQueryOption("maxHeight", IgnoreValue = 0)]
    [Category("Layout")]
    [Description("Gets/Sets the maximum height the resizable should be allowed to resize to.")]
    [Bindable(true)]
    [Themeable(true)]
    public int MaxHeight
    {
      get
      {
        object obj = this.ViewState["MaxHeight"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MaxHeight"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the maximum width the resizable should be allowed to resize to.
    /// </summary>
    [JQueryOption("maxWidth", IgnoreValue = 0)]
    [Category("Layout")]
    [Description("Gets/Sets the maximum width the resizable should be allowed to resize to.")]
    [Bindable(true)]
    [Themeable(true)]
    public int MaxWidth
    {
      get
      {
        object obj = this.ViewState["MaxWidth"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MaxWidth"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets  the minimum height the resizable should be allowed to resize to.
    /// </summary>
    [JQueryOption("minHeight", IgnoreValue = 0)]
    [Category("Layout")]
    [Description("Gets/Sets  the minimum height the resizable should be allowed to resize to.")]
    [Bindable(true)]
    [Themeable(true)]
    public int MinHeight
    {
      get
      {
        object obj = this.ViewState["MinHeight"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MinHeight"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets  the minimum width the resizable should be allowed to resize to.
    /// </summary>
    [JQueryOption("minWidth", IgnoreValue = 0)]
    [Category("Layout")]
    [Description("Gets/Sets  the minimum width the resizable should be allowed to resize to.")]
    [Bindable(true)]
    [Themeable(true)]
    public int MinWidth
    {
      get
      {
        object obj = this.ViewState["MinWidth"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MinWidth"] = value;
      }
    }

    /// <summary>
    ///   This event is triggered during the resize, on the drag of the resize handler
    /// </summary>
    [Category("ClientEvents")]
    [Description("")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("resize", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientResize { get; set; }

    /// <summary>
    ///   This event is triggered at the start of a resize operation
    /// </summary>
    [Category("ClientEvents")]
    [Description("")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("start", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientResizeStart { get; set; }

    /// <summary>
    ///   This event is triggered at the end of a resize operation
    /// </summary>
    [Category("ClientEvents")]
    [Description("")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("stop", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientResizeStop { get; set; }

    /// <summary>
    ///   Tolerance, in milliseconds, for when resizing should start. If specified, resizing will not start until 
    ///   after mouse is moved beyond duration. This can help prevent unintended resizing when clicking on an element.
    /// </summary>
    [Category("Behavior")]
    [JQueryOption("delay", IgnoreValue = 0)]
    public int ResizeStartDelay
    {
      get
      {
        object obj = this.ViewState["ResizeStartDelay"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ResizeStartDelay"] = value;
      }
    }

    /// <summary>
    ///   Tolerance, in pixels, for when resizing should start. If specified, resizing will not start until after mouse is
    ///   moved beyond distance. This can help prevent unintended resizing when clicking on an element.
    /// </summary>
    [Category("Behavior")]
    [JQueryOption("distance", IgnoreValue = 0)]
    public int ResizeStartDistance
    {
      get
      {
        object obj = this.ViewState["ResizeStartDistance"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ResizeStartDistance"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets a semi-transparent helper element is shown for resizing.
    /// </summary>
    [JQueryOption("ghost", IgnoreValue = false)]
    [Category("Appearance")]
    [Description(" Gets/Sets a semi-transparent helper element is shown for resizing.")]
    public bool ShowSemiTransparentHelper
    {
      get
      {
        object obj = this.ViewState["Ghost"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["Ghost"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets snaps the resizing helper to a grid  every x pixels
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets snaps the resizing helper to a grid every x pixels")]
    public int SnapX
    {
      get
      {
        object obj = this.ViewState["SnapX"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["SnapX"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets snaps the resizing helper to a grid  every y pixels
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets snaps the dragging helper to a grid  every y pixels")]
    public int SnapY
    {
      get
      {
        object obj = this.ViewState["SnapY"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["SnapY"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets which control to apply resiable
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Gets/Sets which control to apply resiable")]
    public JQuerySelector Target
    {
      get
      {
        return this.target;
      }

      set
      {
        this.target = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Apply the jQuery resizable plugin to the target control
    /// </summary>
    /// <param name="control">
    /// the instance of the target control
    /// </param>
    /// <returns>
    /// the Resizable instance which have been created will return.
    /// </returns>
    public static Resizable ApplyTo(Control control)
    {
      var resizable = new Resizable();
      resizable.Target = new JQuerySelector(control);
      control.Page.Controls.Add(resizable);
      return resizable;
    }

    /// <summary>
    /// Apply the jQuery resizable plugin to the target controls
    /// </summary>
    /// <param name="control">
    /// the instances of the target controls
    /// </param>
    /// <returns>
    /// the Resizable instance which have been created will return.
    /// </returns>
    public static Resizable ApplyTo(params Control[] control)
    {
      var resizable = new Resizable();
      var ids = new string[control.Length];
      for (int i = 0; i < control.Length; i++)
      {
        ids[i] = "#" + control[i].ClientID;
      }

      resizable.Target = new JQuerySelector(ids);
      control[0].Page.Controls.Add(resizable);
      return resizable;
    }

    /// <summary>
    /// Apply the jQuery droppable plugin to the specify WebControls/HtmlElements by jQuery server side selctor
    /// </summary>
    /// <param name="page">
    /// the Page instance that contains the WebControls/HtmlElements
    /// </param>
    /// <param name="selector">
    /// jQuery server side selctor instance
    /// </param>
    /// <returns>
    /// the Resizable instance which have been created will return.
    /// </returns>
    public static Resizable ApplyTo(Page page, JQuerySelector selector)
    {
      var resizable = new Resizable();
      resizable.Target = selector;
      page.Controls.Add(resizable);
      return resizable;
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
      var builder = new JQueryScriptBuilder(this, this.Target);

      if ((this.Handles != null) && (this.Handles.Length > 0))
      {
        builder.AddOption("handlers", this.handlers);
      }

      if (this.Container != Containers.NotSet)
      {
        if (this.Container != Containers.Customize)
        {
          builder.AddOption("containment", this.Container.ToString().ToLower(), true);
        }
        else
        {
          builder.AddOption("containment", this.ContainsIn.ToString(this.Page));
        }
      }

      if ((this.SnapX > 0) && (this.SnapY > 0))
      {
        builder.AddOption("grid", new[] { this.SnapX, this.SnapY });
      }

      ClientScriptManager.RegisterJQueryControl(this, builder);
      base.OnPreRender(e);
    }

    #endregion
  }
}