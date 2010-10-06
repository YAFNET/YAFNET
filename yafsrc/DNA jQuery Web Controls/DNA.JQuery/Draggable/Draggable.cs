///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using System.Web.UI.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    ///  Draggable is a ASP.NET WebControl that encapsulated the jQuery UI drappable plugin.Draggable
    ///  Web Control implement all functions of jQuery UI droppable plugin,it makes selected
    ///  any WebControls or HTML elements draggable by mouse.
    ///  </summary>
    /// <remarks>
    ///   Draggable WebControls/HtmlElements gets a class of ui-draggable. During drag the WebControls/HtmlElements also gets a class of ui-draggable-dragging. 
    ///   If you want not just drag, but drag-and-drop, see the Droppable WebControl, which provides a drop target for 
    ///   draggables.
    ///   All ClientEvents (OnClientDragStart, OnClientDragStop,OnClientDrag) receive two arguments: 
    ///   The original browser event and a prepared ui object, view below for a documentation of this object (if you name your second argument 'ui'):
    ///  <list>
    /// 		<item>ui.helper - the jQuery object representing the helper that's being dragged</item>
    /// 		<item>ui.position - current position of the helper as { top, left } object, relative to the offset element</item>
    /// 		<item>ui.offset - current absolute position of the helper as { top, left } object, relative to page</item>
    /// 	</list>
    /// </remarks>
    /// <example>
    /// 	<code lang="ASP.NET" title="Draggable properties of ASP.NET">
    /// 		<![CDATA[
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
    ///   <Target TargetIDs="ControlID1, ... ,ControlIDn" Selector=".ui-widget-header" TargetID="ControlID" />
    ///   <ConnectToSortable />
    ///   <ContainsIn />
    ///   <DisableDraggingElements />
    ///   <DragHandler />
    ///   <SnapTo />
    /// </DotNetAge:Draggable>]]>
    /// 	</code>
    /// </example>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Draggable runat=\"server\" ID=\"Draggable1\"></{0}:Draggable>")]
    [Designer(typeof(Design.NoneUIControlDesigner))]
    [JQuery(Name = "draggable", Assembly = "jQueryNet", ScriptResources = new string[] { "ui.core.js", "ui.draggable.js" },
        StartEvent = ClientRegisterEvents.ApplicationLoad)]
    [System.Drawing.ToolboxBitmap(typeof(Draggable), "Draggable.Draggable.ico")]
    [ParseChildren(true)]
    public class Draggable : DraggableBase, INamingContainer
    {
        public event EventHandler Drag;
        private JQuerySelector snapTo;
        private JQuerySelector connectToSortable;
        #region Option Properties

        /// <summary>
        /// Apply the jQuery draggable plugin to the target control
        /// </summary>
        /// <param name="control">the instance of the target control</param>
        /// <returns>the Draggable instance which have been created will return.</returns>
        public static Draggable ApplyTo(Control control) 
        {
            Draggable draggable = new Draggable();
            draggable.Target = new JQuerySelector(control);
            control.Page.Controls.Add(draggable);
            return draggable;
        }

        /// <summary>
        /// Apply the jQuery draggable plugin to the target controls
        /// </summary>
        /// <param name="control">the instances of the target controls</param>
        /// <returns>the Draggable instance which have been created will return.</returns>
        public static Draggable ApplyTo(params Control[] control) 
        {
            Draggable draggable = new Draggable();
            string[] ids =new string[control.Length];
            for (int i = 0; i < control.Length; i++)
                ids[i] ="#"+ control[i].ClientID;
            draggable.Target = new JQuerySelector(ids);
            control[0].Page.Controls.Add(draggable);
            return draggable;
        }

        /// <summary>
        ///  Apply the jQuery draggable plugin to the specify WebControls/HtmlElements by jQuery server side selector
        /// </summary>
        /// <param name="page">the Page instance that contains the WebControls/HtmlElements</param>
        /// <param name="selector"> jQuery server side selctor instance</param>
        /// <returns>the Draggable instance which have been created will return.</returns>
        public static Draggable ApplyTo(Page page,JQuerySelector selector)
        {
            Draggable draggable = new Draggable();
            draggable.Target = selector;
            page.Controls.Add(draggable);
            return draggable;
        }

        /// <summary>
        /// Gets/Sets the Server/Client Selector that the draggable will snap to the edges of the selected Controls/Elements when near an edge of the element.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [Description(" Gets/Sets the Server/Client Selector that the draggable will snap to the edges of the selected Controls/Elements when near an edge of the element.")]
        public JQuerySelector SnapTo
        {
            get
            {
                if (snapTo == null)
                {
                    snapTo = new JQuerySelector();
                    snapTo.ExpressionOnly = true;
                }
                return snapTo;
            }
            set
            {
                snapTo = value;
                if (snapTo != null)
                {
                    snapTo.ExpressionOnly = true;
                    if (!value.IsEmpty)
                        AllowSnap = true;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the Server/Client Selector allows the draggable to be dropped onto the specified sortables. 
        /// If this propety is used (helper must be set to 'Clone' in order to work flawlessly), 
        /// a draggable can be dropped onto a sortable list and then becomes part of it. 
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [Description("Gets/Sets the Server/Client Selector allows the Draggable to be dropped onto the specified Sortables. ")]
        [JQueryOption("connectToSortable")]
        public JQuerySelector ConnectToSortable
        {
            get
            {
                if (connectToSortable == null)
                {
                    connectToSortable = new JQuerySelector();
                    connectToSortable.ExpressionOnly = true;
                }
                return connectToSortable;
            }
            set
            {
                connectToSortable = value;
                if (value != null)
                    connectToSortable.ExpressionOnly = true;
            }
        }

        /// <summary>
        ///  Gets/Sets whether prevent the ui-draggable class from being added. 
        ///  </summary>
        /// <remarks>
        /// This may be desired as a performance optimization when using Draggable on many
        /// hundreds of WebControls/HtmlElements.
        /// </remarks>
        [JQueryOption("addClasses", IgnoreValue = true)]
        [Category("Appearance")]
        [Description("Gets/Sets whether prevent the ui-draggable class from being added. ")]
        public bool AllowAddClasses
        {
            get
            {
                Object obj = ViewState["AddClasses"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["AddClasses"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the custom helper html
        /// </summary>
        [Browsable(false)]
        [Category("DragHelper")]
        [TemplateContainer(typeof(Draggable))]
        [TemplateInstance(TemplateInstance.Single)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CustomHelperTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets whether prevent iframes from capturing the mousemove events during a drag. 
        /// </summary>
        /// <remarks>
        /// Useful in combination with cursorAt, or in any case, if the mouse cursor is not over the helper.
        /// If set to true, transparent overlays will be placed over all iframes on the page. If a selector is supplied, 
        /// the matched iframes will have an overlay placed over them.
        /// </remarks>
        [JQueryOption("iframeFix", IgnoreValue = false)]
        [Category("Behavior")]
        [Description("Gets/Sets whether prevent iframes from capturing the mousemove events during a drag. ")]
        public bool PreventiFrameCapturingMouseEvents
        {
            get
            {
                Object obj = ViewState["PreventiFrameCapturingMouseEvents"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["PreventiFrameCapturingMouseEvents"] = value;
            }
        }


        /// <summary>
        /// Gets/Sets whether droppable positions are calculated on every mousemove. 
        /// </summary>
        /// <remarks>
        /// Caution: This solves issues on highly dynamic pages, but dramatically decreases performance.
        /// </remarks>
        [JQueryOption("refreshPositions", IgnoreValue = false)]
        [Category("Behavior")]
        [Description("Gets/Sets whether droppable positions are calculated on every mousemove. ")]
        public bool RefreshPositions
        {
            get
            {
                Object obj = ViewState["RefreshPositions"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["RefreshPositions"] = value;
            }
        }

        /// <summary>
        ///  Gets/Sets whether the element will return to its start position when dragging stops. 
        ///  </summary>
        ///  <remarks>
        /// Possible string values: 'Auto','Valid', 'Invalid'. If set to Invalid, revert will only occur if the draggable has
        /// not been dropped on a droppable. For Valid, it's the other way around.
        /// </remarks>
        [Category("Behavior")]
        [Description(" Gets/Sets whether the element will return to its start position when dragging stops. ")]
        public Reverts Revert
        {
            get
            {
                Object obj = ViewState["Revert"];
                return (obj == null) ? Reverts.NotSet : (Reverts)obj;
            }
            set
            {
                ViewState["Revert"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the duration of the revert animation, in milliseconds. Ignored if Revert is NotSet.
        /// </summary>
        [JQueryOption("revertDuration", IgnoreValue = 0)]
        [Description("Gets/Sets the duration of the revert animation, in milliseconds. Ignored if Revert is NotSet.")]
        [Category("Behavior")]
        public int RevertDuration
        {
            get
            {
                Object obj = ViewState["RevertDuration"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["RevertDuration"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the group sets of draggable and droppable items, in addition to droppable's Accept Property. 
        /// </summary>
        /// <remarks>
        /// A draggable with the same scope value as a droppable will be accepted by the droppable.
        /// </remarks>
        [JQueryOption("scope")]
        [Category("Behavior")]
        [Description("Gets/Sets the group sets of draggable and droppable items, in addition to droppable's Accept Property. ")]
        public string DragGroupName
        {
            get
            {
                Object obj = ViewState["DragGroupName"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["DragGroupName"] = value;
            }
        }

        /// <summary>
        ///  Get/Sets whether draggable can snap to the edges of the selected elements when near an edge of the element.
        /// </summary>
        [Category("Layout")]
        [Description("Get/Sets whether draggable can snap to the edges of the selected elements when near an edge of the element.")]
        public bool AllowSnap
        {
            get
            {
                Object obj = ViewState["AllowSnap"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AllowSnap"] = value;
            }
        }

        /// <summary>
        /// Get/Sets determines which edges of snap elements the draggable will snap to. Ignored if AllowSnap is false. 
        /// Possible values: 'Inner', 'Outer', 'Both','NotSet'
        /// </summary>
        [JQueryOption("snapMode", IgnoreValue = SnapModes.Both)]
        [Category("Layout")]
        [Description("Get/Sets determines which edges of snap elements the draggable will snap to. Ignored if AllowSnap is false. ")]
        public SnapModes SnapMode
        {
            get
            {
                Object obj = ViewState["SnapMode"];
                return (obj == null) ? SnapModes.Both : (SnapModes)obj;
            }
            set
            {
                ViewState["SnapMode"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the offset in pixels from the snap element edges at which snapping should occur. Ignored if AllowSnap is false.
        /// </summary>
        [JQueryOption("snapTolerance", IgnoreValue = 20)]
        [Category("Layout")]
        [Description("Gets/Sets the offset in pixels from the snap element edges at which snapping should occur. Ignored if AllowSnap is false.")]
        public int SnapOffset
        {
            get
            {
                Object obj = ViewState["SnapOffset"];
                return (obj == null) ? 20 : (int)obj;
            }
            set
            {
                ViewState["SnapOffset"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets  the z-Index of the defined group automatically, 
        /// always brings to front the dragged item. Very useful in things like window managers. 
        /// </summary>
        [Category("Layout")]
        [Description("Gets/Sets  the z-Index of the defined group automatically, ")]
        public int DragGroupMinZIndex
        {
            get
            {
                Object obj = ViewState["DragGroupZIndex"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["DragGroupZIndex"] = value;
            }
        }


        #endregion

        #region Event Properties

        /// <summary>
        /// This event is triggered when dragging starts
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered when dragging starts")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("start", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientDragStart { get; set; }

        /// <summary>
        /// This event is triggered when the mouse is moved during the dragging
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered when the mouse is moved during the dragging")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("drag", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientDrag { get; set; }

        /// <summary>
        /// This event is triggered when dragging stops
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered when dragging stops")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("stop", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientDragStop { get; set; }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            //Step 1 Register Script References
            JQueryScriptBuilder builder = new JQueryScriptBuilder(this, Target);

            //Do some customize
            //if (!DragHandler.IsEmpty)
            //  builder.AddOption("handle", DragHandler.ToString(Page),true);

            if (Container != Containers.NotSet)
            {
                if (Container != Containers.Customize)
                    builder.AddOption("containment", Container.ToString().ToLower(), true);
                else
                    builder.AddOption("containment", ContainsIn.ToString(Page));
            }

            if (DragOrientation != DraggingOrientation.Both)
            {
                if (DragOrientation == DraggingOrientation.Horizontal)
                    builder.AddOption("axis", "x", true);
                else
                    builder.AddOption("axis", "y", true);
            }

            if ((SnapX > 0) && (SnapY > 0))
                builder.AddOption("grid", new int[] { SnapX, SnapY });

            if (!string.IsNullOrEmpty(DragGroupName) && (DragGroupMinZIndex != 0))
                builder.AddOption("stack", "{group:'" + DragGroupName + "',min:" + DragGroupMinZIndex.ToString() + "}", false);

            if (AllowSnap)
            {
                if (!SnapTo.IsEmpty)
                    builder.AddOption("snap", SnapTo.ToString(Page));
                else
                    builder.AddOption("snap", true);
            }

            if (Revert != Reverts.NotSet)
            {
                if (Revert == Reverts.Auto)
                    builder.AddOption("revert", true);
                else
                    builder.AddOption("revert", Revert.ToString().ToLower(), true);
            }

            if (DragHelper != DragHelpers.Original)
                if (CustomHelperTemplate==null)
                    builder.AddOption("helper", DragHelper.ToString().ToLower(), true);

            if (CustomHelperTemplate != null)
            {
                PlaceHolder holder = new PlaceHolder();
                CustomHelperTemplate.InstantiateIn(holder);
                string helperHtml = ClientScriptManager.RenderControlToHTML(holder);
                helperHtml = helperHtml.Replace("\"", "'");
                helperHtml=helperHtml.Replace((char)13, (char)0);
                helperHtml = helperHtml.Replace((char)10,(char)0);
                builder.AddFunctionOption("helper", "return $(\"" + helperHtml + "\");", new string[] { "event" });
            }
            ClientScriptManager.RegisterJQueryControl(this, builder);
        }
    }
}
