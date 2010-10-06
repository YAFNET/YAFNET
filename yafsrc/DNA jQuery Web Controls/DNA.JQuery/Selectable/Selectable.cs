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
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Drawing.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// 	<div id="overview-main">
    /// 		<para>Selectable is a ASP.NET WebControl that encapsulat the jQuery UI selectable
    ///     plugin.Selectable Web Control implement all functions of The jQuery UI selectable plugin.Selectable
    ///     allows for Controls to be selected by
    ///         dragging a box (sometimes called a lasso) with the mouse over the elements.
    ///         Also, elements can be selected by click or drag while holding the Ctrl/Meta
    ///         key, allowing for multiple (non-contiguous) selections.</para>
    /// 	</div>
    /// </summary>
    /// <example>
    /// 	<code lang="ASP.NET" title="Selectable properties">
    /// 		<![CDATA[
    /// <DotNetAge:Selectable ID="MySelectable"
    ///     AutoRefresh="false"
    ///     OnClientSelected="javascript puts here"
    ///     OnClientSelecting=""
    ///     OnClientSelectStart=""
    ///     OnClientSelectStop=""
    ///     OnClientUnselect=""
    ///     OnClientUnselecting=""
    ///     SelectingStartDelay="1000"
    ///     SelectingStartDistance="10"
    ///     Tolerance="Touch">
    ///     <Target TargetIDs="ControlID1, ... ,ControlIDn" 
    ///                  Selector=".ui-widget-header" TargetID="ControlID" />
    ///     <DisableResizableElements />
    ///     <Filter />
    /// </DotNetAge:Selectable>]]>
    /// 	</code>
    /// </example>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Selectable runat=\"server\" ID=\"Selectable1\"></{0}:Selectable>")]
    [Designer(typeof(Design.NoneUIControlDesigner))]
    [JQuery(Name = "selectable", Assembly = "jQueryNet", ScriptResources = new string[] { "ui.core.js", "ui.selectable.js" },
        StartEvent = ClientRegisterEvents.ApplicationLoad)]
    [System.Drawing.ToolboxBitmap(typeof(Selectable), "Selectable.Selectable.ico")]
    [ParseChildren(true)]
    public class Selectable : Control
    {
        private JQuerySelector target = new JQuerySelector();
        private JQuerySelector disabledDraggingElements;
        private JQuerySelector filter;


        /// <summary>
        /// Apply the jQuery selectable plugin to the target control
        /// </summary>
        /// <param name="control">the instance of the target control</param>
        /// <returns>the Selectable instance which have been created will return.</returns>
        public static Selectable ApplyTo(Control control)
        {
            Selectable selectable = new Selectable();
            selectable.Target = new JQuerySelector(control);
            control.Page.Controls.Add(selectable);
            return selectable;
        }

        /// <summary>
        /// Apply the jQuery selectable plugin to the target controls
        /// </summary>
        /// <param name="control">the instances of the target controls</param>
        /// <returns>the Selectable instance which have been created will return.</returns>
        public static Selectable ApplyTo(params Control[] control)
        {
            Selectable selectable = new Selectable();
            string[] ids = new string[control.Length];
            for (int i = 0; i < control.Length; i++)
                ids[i] = "#" + control[i].ClientID;
            selectable.Target = new JQuerySelector(ids);
            control[0].Page.Controls.Add(selectable);
            return selectable;
        }

        /// <summary>
        ///  Apply the jQuery selectable plugin to the specify WebControls/HtmlElements by jQuery server side selector
        /// </summary>
        /// <param name="page">the Page instance that contains the WebControls/HtmlElements</param>
        /// <param name="selector"> jQuery server side selctor instance</param>
        /// <returns>the Selectable instance which have been created will return.</returns>
        public static Selectable ApplyTo(Page page, JQuerySelector selector)
        {
            Selectable selectable = new Selectable();
            selectable.Target = selector;
            page.Controls.Add(selectable);
            return selectable;
        }


        /// <summary>
        /// Gets/Sets which control to apply resiable
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [Description("Gets/Sets which control to apply resiable")]
        public JQuerySelector Target
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// Gets/Sets prevents selecting  if you start on elements matching the selector.
        /// </summary>
        [JQueryOption("cancel")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [Description("Gets/Sets prevents selecting if you start on elements matching the selector.")]
        public JQuerySelector DisableResizableElements
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
        /// Gets/Sets the matching child elements will be made selectees (able to be selected).
        /// </summary>
        [JQueryOption("filter")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [Description("Gets/Sets the matching child elements will be made selectees (able to be selected).")]
        public JQuerySelector Filter
        {
            get
            {
                if (filter == null)
                {
                    filter = new JQuerySelector();
                    filter.ExpressionOnly = true;
                }
                return filter;
            }
            set
            {
                filter = value;
                if (value != null)
                    filter.ExpressionOnly = true;
            }
        }

        /// <summary>
        /// Gets/Sets whether to refresh (recalculate) the position and size of each selectee at the beginning of each select operation. 
        /// </summary>
        /// <remarks>
        /// If you have many many items, you may want to set this to false and call the refresh method manually.
        /// </remarks>
        [JQueryOption("autoRefersh", IgnoreValue = true)]
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/Sets whether to refresh (recalculate) the position and size of each selectee at the beginning of each select operation. ")]
        public bool AutoRefresh
        {
            get
            {
                Object obj = ViewState["AutoRefresh"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["AutoRefresh"] = value;
            }
        }

        /// <summary>
        /// Tolerance, in milliseconds, for when selecting should start. If specified, resizing will not start until 
        /// after mouse is moved beyond duration. This can help prevent unintended resizing when clicking on an element.
        /// </summary>
        [Category("Behavior")]
        [Bindable(true)]
        [JQueryOption("delay", IgnoreValue = 0)]
        public int SelectingStartDelay
        {
            get
            {
                Object obj = ViewState["SelectingStartDelay"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["SelectingStartDelay"] = value;
            }
        }

        /// <summary>
        /// Tolerance, in pixels, for when resizing should start. If specified, resizing will not start until after mouse is
        /// moved beyond distance. This can help prevent unintended resizing when clicking on an element.
        /// </summary>
        [Category("Behavior")]
        [JQueryOption("distance", IgnoreValue = 0)]
        [Bindable(true)]
        public int SelectingStartDistance
        {
            get
            {
                Object obj = ViewState["SelectingStartDistance"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["SelectingStartDistance"] = value;
            }
        }

        [JQueryOption("tolerance", IgnoreValue = SelectableTolerances.Touch)]
        [Category("Behavior")]
        public SelectableTolerances Tolerance
        {
            get
            {
                Object obj = ViewState["Tolerance"];
                return (obj == null) ? SelectableTolerances.Touch : (SelectableTolerances)obj;
            }
            set
            {
                ViewState["Tolerance"] = value;
            }
        }

        /// <summary>
        /// This event is triggered at the end of the select operation, on each element added to the selection.
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered at the end of the select operation, on each element added to the selection.")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("selected", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientSelected { get; set; }

        /// <summary>
        /// This event is triggered during the select operation, on each element added to the selection
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered during the select operation, on each element added to the selection")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("selecting", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientSelecting { get; set; }

        /// <summary>
        /// This event is triggered at the beginning of the select operation
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered at the beginning of the select operation")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("start", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientSelectStart { get; set; }

        /// <summary>
        /// This event is triggered at the beginning of the select operation
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered at the beginning of the select operation")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("stop", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientSelectStop { get; set; }

        /// <summary>
        /// This event is triggered at the end of the select operation, on each element removed from the selection
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered at the end of the select operation, on each element removed from the selection")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("unselected", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientUnselect { get; set; }

        /// <summary>
        /// This event is triggered during the select operation, on each element removed from the selection
        /// </summary>
        [Category("ClientEvents")]
        [Description("This event is triggered during the select operation, on each element removed from the selection")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("unselecting", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientUnselecting { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            JQueryScriptBuilder builder = new JQueryScriptBuilder(this, Target);
            ClientScriptManager.RegisterJQueryControl(this, builder);
            base.OnPreRender(e);
        }

    }
}
