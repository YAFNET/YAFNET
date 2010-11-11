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
  /// Draggable is a ASP.NET WebControl that encapsulated the jQuery UI drappable plugin.Draggable
  ///   Web Control implement all functions of jQuery UI droppable plugin,it makes selected
  ///   any WebControls or HTML elements draggable by mouse.
  /// </summary>
  /// <remarks>
  /// Draggable WebControls/HtmlElements gets a class of ui-draggable. During drag the WebControls/HtmlElements also gets a class of ui-draggable-dragging. 
  ///   If you want not just drag, but drag-and-drop, see the Droppable WebControl, which provides a drop target for 
  ///   draggables.
  ///   All ClientEvents (OnClientDragStart, OnClientDragStop,OnClientDrag) receive two arguments: 
  ///   The original browser event and a prepared ui object, view below for a documentation of this object (if you name your second argument 'ui'):
  ///   <list>
  /// <item>
  /// ui.helper - the jQuery object representing the helper that's being dragged
  /// </item>
  /// <item>
  /// ui.position - current position of the helper as { top, left } object, relative to the offset element
  /// </item>
  /// <item>
  /// ui.offset - current absolute position of the helper as { top, left } object, relative to page
  /// </item>
  /// </list>
  /// </remarks>
  /// <example>
  /// <code lang="ASP.NET" title="Draggable properties of ASP.NET">
  /// <![CDATA[
  /// <DotNetAge:Draggable ID="MyDraggable"
  ///    runat="server"
  ///    AllowAddClasses="true"
  ///    AllowSnap="true"
  ///    AutoScroll="true"
  ///    Container="Parent"
  ///    Cursor="pointer"
  ///    DragGroupMinZIndex="1000"
  ///    DragGroupName="myDragGroup"
  ///    DragHelper="Clone"
  ///    DragHelperOpacity="0.7"
  ///    DragStartDelay="1000"
  ///    DragStartDistance="20"
  ///    OnClientDrag="client script here"
  ///    OnClientDragStart="..."
  ///    OnClientDragStop="..."
  ///    DragOrientation="Both"
  ///    PreventiFrameCapturingMouseEvents="true"
  ///    RefreshPositions="true"
  ///    Revert="Auto"
  ///    RevertDuration="1000"
  ///    ScrollSensitivity="100"
  ///    ScrollSpeed="100"
  ///    SnapMode="Both"
  ///    SnapOffset="0"
  ///    SnapX="20"
  ///    SnapY="20"
  ///    ZIndex="1000"
  /// >
  ///   <CustomHelperTemplate>
  ///      <div>custom helper</div>
  ///   </CutomHelperTemplate>
  ///   <CursorPosition Left="0px" Top="0px" Right="0px" Bottom="0px"/>
  ///   <Target TargetIDs="ControlID1, ... ,ControlIDn" Selector=".ui-widget-header" TargetID="ControlID"/>
  ///   <ConnectToSortable/>
  ///   <ContainsIn/>
  ///   <DisableDraggingElements/>
  ///   <DragHandler/>
  ///   <SnapTo/>
  /// </DotNetAge:Draggable>]]>
  ///   </code>
  /// </example>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Draggable runat=\"server\" ID=\"Draggable1\"></{0}:Draggable>")]
  [Designer(typeof(NoneUIControlDesigner))]
  [JQuery(Name = "draggable", Assembly = "jQueryNet", ScriptResources = new[] { "ui.core.js", "ui.draggable.js" }, 
    StartEvent = ClientRegisterEvents.ApplicationLoad)]
  [ToolboxBitmap(typeof(Draggable), "Draggable.Draggable.ico")]
  [ParseChildren(true)]
  public class Draggable : DraggableBase, INamingContainer
  {
    #region Constants and Fields

    /// <summary>
    /// The connect to sortable.
    /// </summary>
    private JQuerySelector connectToSortable;

    /// <summary>
    /// The snap to.
    /// </summary>
    private JQuerySelector snapTo;

    #endregion

    #region Events

    /// <summary>
    /// The drag.
    /// </summary>
    public event EventHandler Drag;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets whether prevent the ui-draggable class from being added.
    /// </summary>
    /// <remarks>
    ///   This may be desired as a performance optimization when using Draggable on many
    ///   hundreds of WebControls/HtmlElements.
    /// </remarks>
    [JQueryOption("addClasses", IgnoreValue = true)]
    [Category("Appearance")]
    [Description("Gets/Sets whether prevent the ui-draggable class from being added. ")]
    public bool AllowAddClasses
    {
      get
      {
        object obj = this.ViewState["AddClasses"];
        return (obj == null) ? true : (bool)obj;
      }

      set
      {
        this.ViewState["AddClasses"] = value;
      }
    }

    /// <summary>
    ///   Get/Sets whether draggable can snap to the edges of the selected elements when near an edge of the element.
    /// </summary>
    [Category("Layout")]
    [Description(
      "Get/Sets whether draggable can snap to the edges of the selected elements when near an edge of the element.")]
    public bool AllowSnap
    {
      get
      {
        object obj = this.ViewState["AllowSnap"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["AllowSnap"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Server/Client Selector allows the draggable to be dropped onto the specified sortables. 
    ///   If this propety is used (helper must be set to 'Clone' in order to work flawlessly), 
    ///   a draggable can be dropped onto a sortable list and then becomes part of it.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description(
      "Gets/Sets the Server/Client Selector allows the Draggable to be dropped onto the specified Sortables. ")]
    [JQueryOption("connectToSortable")]
    public JQuerySelector ConnectToSortable
    {
      get
      {
        if (this.connectToSortable == null)
        {
          this.connectToSortable = new JQuerySelector();
          this.connectToSortable.ExpressionOnly = true;
        }

        return this.connectToSortable;
      }

      set
      {
        this.connectToSortable = value;
        if (value != null)
        {
          this.connectToSortable.ExpressionOnly = true;
        }
      }
    }

    /// <summary>
    ///   Gets/Sets the custom helper html
    /// </summary>
    [Browsable(false)]
    [Category("DragHelper")]
    [TemplateContainer(typeof(Draggable))]
    [TemplateInstance(TemplateInstance.Single)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ITemplate CustomHelperTemplate { get; set; }

    /// <summary>
    ///   Gets/Sets  the z-Index of the defined group automatically, 
    ///   always brings to front the dragged item. Very useful in things like window managers.
    /// </summary>
    [Category("Layout")]
    [Description("Gets/Sets  the z-Index of the defined group automatically, ")]
    public int DragGroupMinZIndex
    {
      get
      {
        object obj = this.ViewState["DragGroupZIndex"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["DragGroupZIndex"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the group sets of draggable and droppable items, in addition to droppable's Accept Property.
    /// </summary>
    /// <remarks>
    ///   A draggable with the same scope value as a droppable will be accepted by the droppable.
    /// </remarks>
    [JQueryOption("scope")]
    [Category("Behavior")]
    [Description(
      "Gets/Sets the group sets of draggable and droppable items, in addition to droppable's Accept Property. ")]
    public string DragGroupName
    {
      get
      {
        object obj = this.ViewState["DragGroupName"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["DragGroupName"] = value;
      }
    }

    /// <summary>
    ///   This event is triggered when the mouse is moved during the dragging
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when the mouse is moved during the dragging")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("drag", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientDrag { get; set; }

    /// <summary>
    ///   This event is triggered when dragging starts
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when dragging starts")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("start", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientDragStart { get; set; }

    /// <summary>
    ///   This event is triggered when dragging stops
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when dragging stops")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("stop", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientDragStop { get; set; }

    /// <summary>
    ///   Gets/Sets whether prevent iframes from capturing the mousemove events during a drag.
    /// </summary>
    /// <remarks>
    ///   Useful in combination with cursorAt, or in any case, if the mouse cursor is not over the helper.
    ///   If set to true, transparent overlays will be placed over all iframes on the page. If a selector is supplied, 
    ///   the matched iframes will have an overlay placed over them.
    /// </remarks>
    [JQueryOption("iframeFix", IgnoreValue = false)]
    [Category("Behavior")]
    [Description("Gets/Sets whether prevent iframes from capturing the mousemove events during a drag. ")]
    public bool PreventiFrameCapturingMouseEvents
    {
      get
      {
        object obj = this.ViewState["PreventiFrameCapturingMouseEvents"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["PreventiFrameCapturingMouseEvents"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether droppable positions are calculated on every mousemove.
    /// </summary>
    /// <remarks>
    ///   Caution: This solves issues on highly dynamic pages, but dramatically decreases performance.
    /// </remarks>
    [JQueryOption("refreshPositions", IgnoreValue = false)]
    [Category("Behavior")]
    [Description("Gets/Sets whether droppable positions are calculated on every mousemove. ")]
    public bool RefreshPositions
    {
      get
      {
        object obj = this.ViewState["RefreshPositions"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["RefreshPositions"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the element will return to its start position when dragging stops.
    /// </summary>
    /// <remarks>
    ///   Possible string values: 'Auto','Valid', 'Invalid'. If set to Invalid, revert will only occur if the draggable has
    ///   not been dropped on a droppable. For Valid, it's the other way around.
    /// </remarks>
    [Category("Behavior")]
    [Description(" Gets/Sets whether the element will return to its start position when dragging stops. ")]
    public Reverts Revert
    {
      get
      {
        object obj = this.ViewState["Revert"];
        return (obj == null) ? Reverts.NotSet : (Reverts)obj;
      }

      set
      {
        this.ViewState["Revert"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the duration of the revert animation, in milliseconds. Ignored if Revert is NotSet.
    /// </summary>
    [JQueryOption("revertDuration", IgnoreValue = 0)]
    [Description("Gets/Sets the duration of the revert animation, in milliseconds. Ignored if Revert is NotSet.")]
    [Category("Behavior")]
    public int RevertDuration
    {
      get
      {
        object obj = this.ViewState["RevertDuration"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["RevertDuration"] = value;
      }
    }

    /// <summary>
    ///   Get/Sets determines which edges of snap elements the draggable will snap to. Ignored if AllowSnap is false. 
    ///   Possible values: 'Inner', 'Outer', 'Both','NotSet'
    /// </summary>
    [JQueryOption("snapMode", IgnoreValue = SnapModes.Both)]
    [Category("Layout")]
    [Description(
      "Get/Sets determines which edges of snap elements the draggable will snap to. Ignored if AllowSnap is false. ")]
    public SnapModes SnapMode
    {
      get
      {
        object obj = this.ViewState["SnapMode"];
        return (obj == null) ? SnapModes.Both : (SnapModes)obj;
      }

      set
      {
        this.ViewState["SnapMode"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the offset in pixels from the snap element edges at which snapping should occur. Ignored if AllowSnap is false.
    /// </summary>
    [JQueryOption("snapTolerance", IgnoreValue = 20)]
    [Category("Layout")]
    [Description(
      "Gets/Sets the offset in pixels from the snap element edges at which snapping should occur. Ignored if AllowSnap is false."
      )]
    public int SnapOffset
    {
      get
      {
        object obj = this.ViewState["SnapOffset"];
        return (obj == null) ? 20 : (int)obj;
      }

      set
      {
        this.ViewState["SnapOffset"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Server/Client Selector that the draggable will snap to the edges of the selected Controls/Elements when near an edge of the element.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description(
      " Gets/Sets the Server/Client Selector that the draggable will snap to the edges of the selected Controls/Elements when near an edge of the element."
      )]
    public JQuerySelector SnapTo
    {
      get
      {
        if (this.snapTo == null)
        {
          this.snapTo = new JQuerySelector();
          this.snapTo.ExpressionOnly = true;
        }

        return this.snapTo;
      }

      set
      {
        this.snapTo = value;
        if (this.snapTo != null)
        {
          this.snapTo.ExpressionOnly = true;
          if (!value.IsEmpty)
          {
            this.AllowSnap = true;
          }
        }
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Apply the jQuery draggable plugin to the target control
    /// </summary>
    /// <param name="control">
    /// the instance of the target control
    /// </param>
    /// <returns>
    /// the Draggable instance which have been created will return.
    /// </returns>
    public static Draggable ApplyTo(Control control)
    {
      var draggable = new Draggable();
      draggable.Target = new JQuerySelector(control);
      control.Page.Controls.Add(draggable);
      return draggable;
    }

    /// <summary>
    /// Apply the jQuery draggable plugin to the target controls
    /// </summary>
    /// <param name="control">
    /// the instances of the target controls
    /// </param>
    /// <returns>
    /// the Draggable instance which have been created will return.
    /// </returns>
    public static Draggable ApplyTo(params Control[] control)
    {
      var draggable = new Draggable();
      var ids = new string[control.Length];
      for (int i = 0; i < control.Length; i++)
      {
        ids[i] = "#" + control[i].ClientID;
      }

      draggable.Target = new JQuerySelector(ids);
      control[0].Page.Controls.Add(draggable);
      return draggable;
    }

    /// <summary>
    /// Apply the jQuery draggable plugin to the specify WebControls/HtmlElements by jQuery server side selector
    /// </summary>
    /// <param name="page">
    /// the Page instance that contains the WebControls/HtmlElements
    /// </param>
    /// <param name="selector">
    /// jQuery server side selctor instance
    /// </param>
    /// <returns>
    /// the Draggable instance which have been created will return.
    /// </returns>
    public static Draggable ApplyTo(Page page, JQuerySelector selector)
    {
      var draggable = new Draggable();
      draggable.Target = selector;
      page.Controls.Add(draggable);
      return draggable;
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
      // Step 1 Register Script References
      var builder = new JQueryScriptBuilder(this, this.Target);

      // Do some customize
      // if (!DragHandler.IsEmpty)
      // builder.AddOption("handle", DragHandler.ToString(Page),true);
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

      if (this.DragOrientation != DraggingOrientation.Both)
      {
        if (this.DragOrientation == DraggingOrientation.Horizontal)
        {
          builder.AddOption("axis", "x", true);
        }
        else
        {
          builder.AddOption("axis", "y", true);
        }
      }

      if ((this.SnapX > 0) && (this.SnapY > 0))
      {
        builder.AddOption("grid", new[] { this.SnapX, this.SnapY });
      }

      if (!string.IsNullOrEmpty(this.DragGroupName) && (this.DragGroupMinZIndex != 0))
      {
        builder.AddOption("stack", "{group:'" + this.DragGroupName + "',min:" + this.DragGroupMinZIndex + "}", false);
      }

      if (this.AllowSnap)
      {
        if (!this.SnapTo.IsEmpty)
        {
          builder.AddOption("snap", this.SnapTo.ToString(this.Page));
        }
        else
        {
          builder.AddOption("snap", true);
        }
      }

      if (this.Revert != Reverts.NotSet)
      {
        if (this.Revert == Reverts.Auto)
        {
          builder.AddOption("revert", true);
        }
        else
        {
          builder.AddOption("revert", this.Revert.ToString().ToLower(), true);
        }
      }

      if (this.DragHelper != DragHelpers.Original)
      {
        if (this.CustomHelperTemplate == null)
        {
          builder.AddOption("helper", this.DragHelper.ToString().ToLower(), true);
        }
      }

      if (this.CustomHelperTemplate != null)
      {
        var holder = new PlaceHolder();
        this.CustomHelperTemplate.InstantiateIn(holder);
        string helperHtml = ClientScriptManager.RenderControlToHTML(holder);
        helperHtml = helperHtml.Replace("\"", "'");
        helperHtml = helperHtml.Replace((char)13, (char)0);
        helperHtml = helperHtml.Replace((char)10, (char)0);
        builder.AddFunctionOption("helper", "return $(\"" + helperHtml + "\");", new[] { "event" });
      }

      ClientScriptManager.RegisterJQueryControl(this, builder);
    }

    #endregion
  }
}