//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// The draggable base.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public abstract class DraggableBase : Control
  {
    #region Constants and Fields

    /// <summary>
    /// The contains in.
    /// </summary>
    private JQuerySelector containsIn;

    /// <summary>
    /// The curror pos.
    /// </summary>
    private Position currorPos;

    /// <summary>
    /// The disabled dragging elements.
    /// </summary>
    private JQuerySelector disabledDraggingElements;

    /// <summary>
    /// The drag handler.
    /// </summary>
    private JQuerySelector dragHandler;

    /// <summary>
    /// The target.
    /// </summary>
    private JQuerySelector target = new JQuerySelector();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets whether container auto-scrolls while dragging.
    /// </summary>
    [JQueryOption("scroll", IgnoreValue = true)]
    [Category("Behavior")]
    [Description("Gets/Sets whether container auto-scrolls while dragging.")]
    public bool AutoScroll
    {
      get
      {
        object obj = this.ViewState["AutoScroll"];
        return (obj == null) ? true : (bool)obj;
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
    ///   The css cursor during the drag operation.
    /// </summary>
    [JQueryOption("cursor")]
    [Category("Appearance")]
    public string Cursor
    {
      get
      {
        object obj = this.ViewState["Cursor"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["Cursor"] = value;
      }
    }

    /// <summary>
    ///   Moves the dragging helper so the cursor always appears to drag from the same position. 
    ///   Coordinates can be given as a hash using a combination of one or two keys: { Top, Left, Right, Bottom }.
    /// </summary>
    [JQueryOption("cursorAt")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Category("Appearance")]
    public Position CursorPosition
    {
      get
      {
        if (this.currorPos == null)
        {
          this.currorPos = new Position();
        }

        return this.currorPos;
      }

      set
      {
        this.currorPos = value;
      }
    }

    /// <summary>
    ///   Gets/Sets prevents dragging from starting on specified Controls/Elements.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Gets/Sets prevents dragging from starting on specified Controls/Elements.")]
    [JQueryOption("cancel")]
    public JQuerySelector DisableDraggingElements
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
    ///   Gets/Sets restricts drag start click to the specified Controls/Elements
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Gets/Sets restricts drag start click to the specified Controls/Elements")]
    [JQueryOption("handle")]
    public JQuerySelector DragHandler
    {
      get
      {
        if (this.dragHandler == null)
        {
          this.dragHandler = new JQuerySelector();
          this.dragHandler.ExpressionOnly = true;
        }

        return this.dragHandler;
      }

      set
      {
        this.dragHandler = value;
        if (value != null)
        {
          this.dragHandler.ExpressionOnly = true;
        }
      }
    }

    /// <summary>
    ///   Gets/Sets a helper element to be used for dragging display.
    /// </summary>
    [Category("DragHelper")]
    public DragHelpers DragHelper
    {
      get
      {
        object obj = this.ViewState["DragHelper"];
        return (obj == null) ? DragHelpers.Original : (DragHelpers)obj;
      }

      set
      {
        this.ViewState["DragHelper"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets opacity for the helper while being dragged.
    /// </summary>
    [JQueryOption("opacity", IgnoreValue = 0)]
    [Category("DragHelper")]
    [Description("Gets/Sets opacity for the helper while being dragged.")]
    public float DragHelperOpacity
    {
      get
      {
        object obj = this.ViewState["DragHelperOpacity"];
        return (obj == null) ? 0 : (float)obj;
      }

      set
      {
        this.ViewState["DragHelperOpacity"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets constrains dragging to either the horizontal or vertical axis or both of them.
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets constrains dragging to either the horizontal or vertical axis or both of them.")]
    public DraggingOrientation DragOrientation
    {
      get
      {
        object obj = this.ViewState["Orientation"];
        return (obj == null) ? DraggingOrientation.Both : (DraggingOrientation)obj;
      }

      set
      {
        this.ViewState["Orientation"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets time in milliseconds after mousedown until dragging should start.
    /// </summary>
    /// <remarks>
    ///   This Property can be used to prevent unwanted drags when clicking on an element.
    /// </remarks>
    [JQueryOption("delay", IgnoreValue = 0)]
    [Category("Behavior")]
    [Description(" Gets/Sets time in milliseconds after mousedown until dragging should start.")]
    public int DragStartDelay
    {
      get
      {
        object obj = this.ViewState["DragStartDelay"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["DragStartDelay"] = value;
      }
    }

    ///<summary>
    ///  Gets/Sets DragDistance in pixels after mousedown the mouse must move before dragging should start.
    ///</summary>
    ///<remarks>
    ///  This Property can be used to prevent unwanted drags when clicking on an element.
    ///</remarks>
    [Category("Behavior")]
    [Description(" Gets/Sets DragDistance in pixels after mousedown the mouse must move before dragging should start. ")
    ]
    [JQueryOption("distance", IgnoreValue = 0)]
    public int DragStartDistance
    {
      get
      {
        object obj = this.ViewState["DragStartDistance"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["DragStartDistance"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets distance in pixels from the edge of the viewport after which the viewport should scroll. 
    ///   Distance is relative to pointer, not the draggable.
    /// </summary>
    [JQueryOption("scrollSensitivity", IgnoreValue = 0)]
    [Description("Gets/Sets distance in pixels from the edge of the viewport after which the viewport should scroll.")]
    [Category("Behavior")]
    public int ScrollSensitivity
    {
      get
      {
        object obj = this.ViewState["ScrollSensitivity"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ScrollSensitivity"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the speed at which the window should scroll once the mouse pointer gets within the 
    ///   scrollSensitivity distance.
    /// </summary>
    [JQueryOption("scrollSpeed", IgnoreValue = 0)]
    [Category("Behavior")]
    [Description(
      "Gets/Sets the speed at which the window should scroll once the mouse pointer gets within the ScrollSensitivity distance."
      )]
    public int ScrollSpeed
    {
      get
      {
        object obj = this.ViewState["ScrollSpeed"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ScrollSpeed"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets snaps the dragging helper to a grid  every x pixels
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets snaps the dragging helper to a grid every x pixels")]
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
    ///   Gets/Sets snaps the dragging helper to a grid  every y pixels
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
    ///   Gets/Sets which control to apply draggable
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [Category("Behavior")]
    [Description("Gets/Sets which control to apply draggable")]
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

    /// <summary>
    ///   Gets/Sets z-index for the helper while being dragged.
    /// </summary>
    [JQueryOption("zIndex", IgnoreValue = 0)]
    [Category("Layout")]
    [Description("Gets/Sets z-index for the helper while being dragged.")]
    public int ZIndex
    {
      get
      {
        object obj = this.ViewState["ZIndex"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ZIndex"] = value;
      }
    }

    #endregion
  }
}