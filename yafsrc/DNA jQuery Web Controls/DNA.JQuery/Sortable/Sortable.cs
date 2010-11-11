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

  using DNA.UI.JQuery.Design;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// Sortable is a ASP.NET WebControl that encapsulat the jQuery UI resizable it makes
  ///   selected controls sortable by dragging with the mouse.
  /// </summary>
  /// <remarks>
  /// <para>
  /// All ClientEvents receive two arguments: The original browser event and a
  ///     prepared ui object, view below for a documentation of this object:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// ui.helper - the current helper element (most often a clone of the
  ///       item)
  /// </item>
  /// <item>
  /// ui.position - current position of the helper
  /// </item>
  /// <item>
  /// ui.offset - current absolute position of the helper
  /// </item>
  /// <item>
  /// ui.item - the current dragged element * ui.placeholder - the placeholder
  ///       (if you defined one)
  /// </item>
  /// <item>
  /// ui.sender - the sortable where the item comes from (only exists if you
  ///       move from one connected list to another)
  /// </item>
  /// </list>
  /// </remarks>
  /// <example>
  /// <code lang="ASP.NET" title="Sortable control's properties">
  /// <![CDATA[
  /// <DotNetAge:Sortable ID="MySortable"
  ///     AllowDropOnEmpty="true"
  ///     AutoScroll="true"
  ///     Container="Parent"
  ///     Cursor="move"
  ///     DragHelper="Clone"
  ///     DragHelperOpacity="0.5"
  ///     DragOrientation="Both"
  ///     DragStartDelay="0"
  ///     DragStartDistance="0"
  ///     ForceHelperSize="true"
  ///     ForcePlaceholderSize="true"
  ///     OnClientSort="javascript puts there"
  ///     OnClientSortActivate=""
  ///     OnClientSortBeforeStop=""
  ///     OnClientSortChange=""
  ///     OnClientSortDeactivate=""
  ///     OnClientSortOut=""
  ///     OnClientSortOver=""
  ///     OnClientSortReceive=""
  ///     OnClientSortRemove=""
  ///     OnClientSortStart=""
  ///     OnClientSortStop=""
  ///     OnClientSortUpdate=""
  ///     PlaceHolderCssClass="placeholder"
  ///     Revert="true"
  ///     ScrollSensitivity="10"
  ///     ScrollSpeed="100"
  ///     SnapX="10"
  ///     SnapY="10"
  ///     Tolerance="Pointer"
  ///     ZIndex="1000"
  /// >
  ///       <Target TargetIDs="ControlID1, ... ,ControlIDn" 
  ///                  Selector=".ui-widget-header" TargetID="ControlID"/>
  ///       <ConnectWith/>
  ///       <ContainsIn/>
  ///       <CursorPosition Bottom="0" Left="0" Right="0" Top="0"/>
  ///       <DisableDraggingElements/>
  ///       <DragHandler/>
  ///       <DisableSortableItems/>
  /// </DotNetAge:Sortable>]]>
  ///   </code>
  /// </example>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Sortable runat=\"server\" ID=\"Sortable1\"></{0}:Sortable>")]
  [Designer(typeof(NoneUIControlDesigner))]
  [JQuery(Name = "sortable", Assembly = "jQueryNet", ScriptResources = new[] { "ui.core.js", "ui.sortable.js" }, 
    StartEvent = ClientRegisterEvents.ApplicationLoad)]
  [ToolboxBitmap(typeof(Sortable), "Sortable.Sortable.ico")]
  [ParseChildren(true)]
  public class Sortable : DraggableBase
  {
    #region Constants and Fields

    /// <summary>
    /// The connect width.
    /// </summary>
    private JQuerySelector connectWidth;

    /// <summary>
    /// The sortable items.
    /// </summary>
    private JQuerySelector sortableItems;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets whether empty allows for an item to be dropped from a linked selectable.
    /// </summary>
    [JQueryOption("dropOnEmpty", IgnoreValue = true)]
    [Category("Behavior")]
    [Bindable(true)]
    [Description("Gets/Sets whether empty allows for an item to be dropped from a linked selectable.")]
    public bool AllowDropOnEmpty
    {
      get
      {
        object obj = this.ViewState["DropOnEmpty"];
        return (obj == null) ? true : (bool)obj;
      }

      set
      {
        this.ViewState["DropOnEmpty"] = value;
      }
    }

    /// <summary>
    ///   Takes a jQuery selector with items that also have sortables applied. 
    ///   If used, the sortable is now connected to the other one-way, so you can drag from 
    ///   this sortable to the other.
    /// </summary>
    [JQueryOption("connectWith")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Takes a jQuery Server Selector with items that also have sortables applied. ")]
    public JQuerySelector ConnectWith
    {
      get
      {
        if (this.connectWidth == null)
        {
          this.connectWidth = new JQuerySelector();
          this.connectWidth.ExpressionOnly = true;
        }

        return this.connectWidth;
      }

      set
      {
        this.connectWidth = value;
        if (value != null)
        {
          this.connectWidth.ExpressionOnly = true;
        }
      }
    }

    /// <summary>
    ///   Specifies which items inside the element should be sortable.
    /// </summary>
    [JQueryOption("items")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Specifies which items inside the element should be sortable.")]
    public JQuerySelector DisableSortableItems
    {
      get
      {
        if (this.sortableItems == null)
        {
          this.sortableItems = new JQuerySelector();
          this.sortableItems.ExpressionOnly = true;
        }

        return this.sortableItems;
      }

      set
      {
        this.sortableItems = value;
        if (value != null)
        {
          this.sortableItems.ExpressionOnly = true;
        }
      }
    }

    /// <summary>
    ///   Gets/Sets whether forces the helper to have a size.
    /// </summary>
    [JQueryOption("forceHelperSize", IgnoreValue = false)]
    [Category("Appearance")]
    [Bindable(true)]
    [Description("Gets/Sets whether forces the helper to have a size.")]
    public bool ForceHelperSize
    {
      get
      {
        object obj = this.ViewState["ForceHelperSize"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ForceHelperSize"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets forces the placeholder to have a size.
    /// </summary>
    [JQueryOption("forcePlaceholderSize", IgnoreValue = false)]
    [Category("Appearance")]
    [Bindable(true)]
    [Description("Gets/Sets forces the placeholder to have a size.")]
    public bool ForcePlaceholderSize
    {
      get
      {
        object obj = this.ViewState["ForcePlaceholderSize"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ForcePlaceholderSize"] = value;
      }
    }

    /// <summary>
    ///   This event is triggered during sorting.
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered during sorting.")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("sort", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSort { get; set; }

    /// <summary>
    ///   This event is triggered when using connected lists, every connected list on drag start receives it
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when using connected lists, every connected list on drag start receives it")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("activate", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortActivate { get; set; }

    /// <summary>
    ///   This event is triggered when sorting stops, but when the placeholder/helper is still available.
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when sorting stops, but when the placeholder/helper is still available.")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("beforeStop", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortBeforeStop { get; set; }

    /// <summary>
    ///   This event is triggered during sorting, but only when the DOM position has changed.
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered during sorting, but only when the DOM position has changed.")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("change", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortChange { get; set; }

    /// <summary>
    ///   This event is triggered when sorting was stopped, is propagated to all possible connected lists
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when sorting was stopped, is propagated to all possible connected lists")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("deactivate", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortDeactivate { get; set; }

    /// <summary>
    ///   This event is triggered when a sortable item is moved away from a connected list
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when a sortable item is moved away from a connected list")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("out", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortOut { get; set; }

    /// <summary>
    ///   This event is triggered when a sortable item is moved into a connected list
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when a sortable item is moved into a connected list")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("over", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortOver { get; set; }

    /// <summary>
    ///   This event is triggered when a connected sortable list has received an item from another list
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when a connected sortable list has received an item from another list")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("receive", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortReceive { get; set; }

    /// <summary>
    ///   This event is triggered when a sortable item has been dragged out from the list and into another
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when a sortable item has been dragged out from the list and into another")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("remove", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortRemove { get; set; }

    /// <summary>
    ///   This event is triggered when sorting starts.
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when sorting starts.")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("start", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortStart { get; set; }

    /// <summary>
    ///   This event is triggered when sorting has stopped.
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when sorting has stopped.")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("stop", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortStop { get; set; }

    /// <summary>
    ///   This event is triggered when the user stopped sorting and the DOM position has changed.
    /// </summary>
    [Category("ClientEvents")]
    [Description("This event is triggered when the user stopped sorting and the DOM position has changed.")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("update", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientSortUpdate { get; set; }

    /// <summary>
    ///   Gets/Sets css class that gets applied to the otherwise white space.
    /// </summary>
    [JQueryOption("placeholder")]
    [CssClassProperty]
    [Category("Appearance")]
    [Description("Gets/Sets css class that gets applied to the otherwise white space.")]
    [Themeable(true)]
    [Bindable(true)]
    public string PlaceHolderCssClass
    {
      get
      {
        object obj = this.ViewState["PlaceHolder"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["PlaceHolder"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the item will be reverted to its new DOM position with a smooth animation.
    /// </summary>
    [JQueryOption("revert", IgnoreValue = false)]
    [Category("Behavior")]
    [Bindable(true)]
    [Description("Gets/Sets whether the item will be reverted to its new DOM position with a smooth animation.")]
    public bool Revert
    {
      get
      {
        object obj = this.ViewState["Revert"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["Revert"] = value;
      }
    }

    /// <summary>
    ///   This is the way the reordering behaves during drag. Possible values: 'Intersect', 'Pointer'. 
    ///   In some setups, 'Pointer' is more natural.
    /// </summary>
    [JQueryOption("tolerance", IgnoreValue = SortableTolerances.Intersect)]
    [Category("Appearance")]
    [Description("This is the way the reordering behaves during drag. ")]
    public SortableTolerances Tolerance
    {
      get
      {
        object obj = this.ViewState["Tolerance"];
        return (obj == null) ? SortableTolerances.Intersect : (SortableTolerances)obj;
      }

      set
      {
        this.ViewState["Tolerance"] = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Apply the jQuery sortable plugin to the target control
    /// </summary>
    /// <param name="control">
    /// the instance of the target control
    /// </param>
    /// <returns>
    /// the Sortable instance which have been created will return.
    /// </returns>
    public static Sortable ApplyTo(Control control)
    {
      var sortable = new Sortable();
      sortable.Target = new JQuerySelector(control);
      control.Page.Controls.Add(sortable);
      return sortable;
    }

    /// <summary>
    /// Apply the jQuery sortable plugin to the target controls
    /// </summary>
    /// <param name="control">
    /// the instances of the target controls
    /// </param>
    /// <returns>
    /// the Sortable instance which have been created will return.
    /// </returns>
    public static Sortable ApplyTo(params Control[] control)
    {
      var sortable = new Sortable();
      var ids = new string[control.Length];
      for (int i = 0; i < control.Length; i++)
      {
        ids[i] = "#" + control[i].ClientID;
      }

      sortable.Target = new JQuerySelector(ids);
      control[0].Page.Controls.Add(sortable);
      return sortable;
    }

    /// <summary>
    /// Apply the jQuery sortable plugin to the specify WebControls/HtmlElements by jQuery server side selector
    /// </summary>
    /// <param name="page">
    /// the Page instance that contains the WebControls/HtmlElements
    /// </param>
    /// <param name="selector">
    /// jQuery server side selctor instance
    /// </param>
    /// <returns>
    /// the Sortable instance which have been created will return.
    /// </returns>
    public static Sortable ApplyTo(Page page, JQuerySelector selector)
    {
      var sortable = new Sortable();
      sortable.Target = selector;
      page.Controls.Add(sortable);
      return sortable;
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

      if (this.DragHelper != DragHelpers.Original)
      {
        builder.AddOption("helper", this.DragHelper.ToString().ToLower(), true);
      }

      ClientScriptManager.RegisterJQueryControl(this, builder);
    }

    #endregion
  }
}