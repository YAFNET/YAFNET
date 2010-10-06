///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class DraggableBase:Control
    {
        private JQuerySelector target = new JQuerySelector();
        private JQuerySelector disabledDraggingElements;
        private JQuerySelector dragHandler;
        private JQuerySelector containsIn;
        private Position currorPos;

        /// <summary>
        /// Gets/Sets which control to apply draggable
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [Description("Gets/Sets which control to apply draggable")]
        public JQuerySelector Target
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// Gets/Sets prevents dragging from starting on specified Controls/Elements.
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
                if (disabledDraggingElements == null)
                {
                    disabledDraggingElements = new JQuerySelector();
                    disabledDraggingElements.ExpressionOnly = true;
                }
                return disabledDraggingElements;
            }
            set
            {
                disabledDraggingElements = value;
                if (value != null)
                    disabledDraggingElements.ExpressionOnly = true;
            }
        }

        /// <summary>
        ///  Gets/Sets restricts drag start click to the specified Controls/Elements
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
                if (dragHandler == null)
                {
                    dragHandler = new JQuerySelector();
                    dragHandler.ExpressionOnly = true;
                }
                return dragHandler;
            }
            set
            {
                dragHandler = value;
                if (value != null)
                    dragHandler.ExpressionOnly = true;
            }
        }

        /// <summary>
        /// Gets/Sets a helper element to be used for dragging display.
        /// </summary>
        [Category("DragHelper")]
        public DragHelpers DragHelper
        {
            get
            {
                Object obj = ViewState["DragHelper"];
                return (obj == null) ? DragHelpers.Original : (DragHelpers)obj;
            }
            set
            {
                ViewState["DragHelper"] = value;
            }
        }

        /// <summary>
        /// Constrains dragging to within the bounds of the specified element.
        /// Possible string values: 'parent', 'document', 'window'
        /// </summary>
        [Category("Layout")]
        public Containers Container
        {
            get
            {
                Object obj = ViewState["Containment"];
                return (obj == null) ? Containers.NotSet : (Containers)obj;
            }
            set
            {
                ViewState["Containment"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Control/Element constrains dragging to within the bounds of the specified Control/Element.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [Description("Gets/Sets the Control/Element constrains dragging to within the bounds of the specified Control/Element.")]
        public JQuerySelector ContainsIn
        {
            get
            {
                if (containsIn == null)
                {
                    containsIn = new JQuerySelector();
                    containsIn.ExpressionOnly = true;
                }
                return containsIn;
            }
            set
            {
                containsIn = value;
                if (value != null)
                    containsIn.ExpressionOnly = true;
            }
        }

        /// <summary>
        /// Gets/Sets whether container auto-scrolls while dragging.
        /// </summary>
        [JQueryOption("scroll", IgnoreValue = true)]
        [Category("Behavior")]
        [Description("Gets/Sets whether container auto-scrolls while dragging.")]
        public bool AutoScroll
        {
            get
            {
                Object obj = ViewState["AutoScroll"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["AutoScroll"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets distance in pixels from the edge of the viewport after which the viewport should scroll. 
        /// Distance is relative to pointer, not the draggable.
        /// </summary>
        [JQueryOption("scrollSensitivity", IgnoreValue = 0)]
        [Description("Gets/Sets distance in pixels from the edge of the viewport after which the viewport should scroll.")]
        [Category("Behavior")]
        public int ScrollSensitivity
        {
            get
            {
                Object obj = ViewState["ScrollSensitivity"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["ScrollSensitivity"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets constrains dragging to either the horizontal or vertical axis or both of them.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets constrains dragging to either the horizontal or vertical axis or both of them.")]
        public DraggingOrientation DragOrientation
        {
            get
            {
                Object obj = ViewState["Orientation"];
                return (obj == null) ? DraggingOrientation.Both : (DraggingOrientation)obj;
            }
            set
            {
                ViewState["Orientation"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets time in milliseconds after mousedown until dragging should start.
        /// </summary>
        /// <remarks>
        /// This Property can be used to prevent unwanted drags when clicking on an element.
        /// </remarks>
        [JQueryOption("delay", IgnoreValue = 0)]
        [Category("Behavior")]
        [Description(" Gets/Sets time in milliseconds after mousedown until dragging should start.")]
        public int DragStartDelay
        {
            get
            {
                Object obj = ViewState["DragStartDelay"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["DragStartDelay"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets DragDistance in pixels after mousedown the mouse must move before dragging should start. 
        /// </summary>
        ///<remarks>
        /// This Property can be used to prevent unwanted drags when clicking on an element.
        /// </remarks>
        [Category("Behavior")]
        [Description(" Gets/Sets DragDistance in pixels after mousedown the mouse must move before dragging should start. ")]
        [JQueryOption("distance", IgnoreValue = 0)]
        public int DragStartDistance
        {
            get
            {
                Object obj = ViewState["DragStartDistance"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["DragStartDistance"] = value;
            }
        }

        /// <summary>
        /// The css cursor during the drag operation.
        /// </summary>
        [JQueryOption("cursor")]
        [Category("Appearance")]
        public string Cursor
        {
            get
            {
                Object obj = ViewState["Cursor"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["Cursor"] = value;
            }
        }

        /// <summary>
        /// Moves the dragging helper so the cursor always appears to drag from the same position. 
        /// Coordinates can be given as a hash using a combination of one or two keys: { Top, Left, Right, Bottom }.
        /// </summary>
        [JQueryOption("cursorAt")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Category("Appearance")]
        public Position CursorPosition
        {
            get
            {
                if (currorPos == null)
                    currorPos = new Position();
                return currorPos;
            }
            set
            {
                currorPos = value;
            }
        }

        /// <summary>
        /// Gets/Sets snaps the dragging helper to a grid  every x pixels
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets snaps the dragging helper to a grid every x pixels")]
        public int SnapX
        {
            get
            {
                Object obj = ViewState["SnapX"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["SnapX"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets snaps the dragging helper to a grid  every y pixels
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets snaps the dragging helper to a grid  every y pixels")]
        public int SnapY
        {
            get
            {
                Object obj = ViewState["SnapY"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["SnapY"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the speed at which the window should scroll once the mouse pointer gets within the 
        /// scrollSensitivity distance.
        /// </summary>
        [JQueryOption("scrollSpeed", IgnoreValue = 0)]
        [Category("Behavior")]
        [Description("Gets/Sets the speed at which the window should scroll once the mouse pointer gets within the ScrollSensitivity distance.")]
        public int ScrollSpeed
        {
            get
            {
                Object obj = ViewState["ScrollSpeed"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["ScrollSpeed"] = value;
            }
        }


        /// <summary>
        /// Gets/Sets opacity for the helper while being dragged.
        /// </summary>
        [JQueryOption("opacity", IgnoreValue = 0)]
        [Category("DragHelper")]
        [Description("Gets/Sets opacity for the helper while being dragged.")]
        public float DragHelperOpacity
        {
            get
            {
                Object obj = ViewState["DragHelperOpacity"];
                return (obj == null) ? 0 : (float)obj;
            }
            set
            {
                ViewState["DragHelperOpacity"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets z-index for the helper while being dragged.
        /// </summary>
        [JQueryOption("zIndex", IgnoreValue = 0)]
        [Category("Layout")]
        [Description("Gets/Sets z-index for the helper while being dragged.")]
        public int ZIndex
        {
            get
            {
                Object obj = ViewState["ZIndex"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["ZIndex"] = value;
            }
        }
    }
}
