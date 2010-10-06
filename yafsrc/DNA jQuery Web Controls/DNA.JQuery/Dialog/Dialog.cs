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

[assembly: WebResource("DNA.UI.JQuery.Dialog.Error.gif", "image/gif")]
[assembly: WebResource("DNA.UI.JQuery.Dialog.Info.gif", "image/gif")]
[assembly: WebResource("DNA.UI.JQuery.Dialog.Question.gif", "image/gif")]
[assembly: WebResource("DNA.UI.JQuery.Dialog.Stop.gif", "image/gif")]
namespace DNA.UI.JQuery {

    /// <summary>
    ///  The Dialog is a web control encapsulated the jQuery UI dialog plugin. A Dialog is a floating window that contains a title bar and a content area. 
    ///  The dialog window can be moved, resized and closed with the 'x' icon by default.
    ///  If the content length exceeds the maximum height, a scrollbar will automatically appear.
    ///  A bottom button bar and semi-transparent modal overlay layer are common options that can be added. 
    /// </summary>
    /// <example>
    /// 	<code lang="ASP.NET" title="Dialog 's Properties">
    /// 		<![CDATA[
    /// <DotNetAge:Dialog ID="MyDialog"
    ///    AutoOpen="false"
    ///    BgiFrame="true"
    ///    CloseOnEscape="true"
    ///    DialogButtons="None"
    ///    HideEffect="None"
    ///    IconUrl=""
    ///    IsDraggable="false"
    ///    IsResizable="false"
    ///    IsStack="true"
    ///    MaxHeight="0"
    ///    MaxWidth="0"
    ///    MessageText=""
    ///    MinHeight="0"
    ///    MinWidth="0"
    ///    OnClientBeforeClose=""
    ///    OnClientClose=""
    ///    OnClientDrag=""
    ///    OnClientDragStart=""
    ///    OnClientDragStop=""
    ///    OnClientFocus=""
    ///    OnClientOpen=""
    ///    OnClientResize=""
    ///    OnClientResizeStart=""
    ///    OnClientResizeStop=""
    ///    Position="Center"
    ///    ShowEffect="None"
    ///    ShowModal="false"
    ///    Title=""
    ///    ZIndex="1000"
    /// >
    ///   <BodyTemplate>
    ///      Dialog body here
    ///   </BodyTemplate>
    ///   <Buttons>
    ///      <DotNetAge:DialogButton 
    ///          CommandArgument=""
    ///          CommandName=""
    ///          OnClientClick=""
    ///          Text="" />
    ///          ...
    ///   </Buttons>
    ///    <Target TargetIDs="ControlID1, ... ,ControlIDn" 
    ///                Selector=".ui-widget-header" 
    ///                TargetID="ControlID" />
    /// </DotNetAge:Dialog>]]>
    /// 	</code>
    /// </example>
    [JQuery(Name = "dialog", Assembly = "jQueryNet", DisposeMethod = "destroy", ScriptResources = new string[] { "ui.core.js", "ui.draggable.js", "ui.resizable.js", "ui.dialog.js" }, StartEvent = ClientRegisterEvents.ApplicationLoad)]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Dialog runat=\"server\" ID=\"Dialog1\" Title=\"[DialogTitle]\"></{0}:Dialog>")]
    [Designer(typeof(Design.DialogDesigner))]
    [ParseChildren(true, "Buttons")]
    [System.Drawing.ToolboxBitmap(typeof(Dialog), "Dialog.Dialog.ico")]
    public class Dialog : CompositeControl, IPostBackEventHandler
    {
        private List<DialogButton> buttons;
        private JQuerySelector trigger;
        private ITemplate bodyTemplate;
        #region Properties

        /// <summary>
        /// Gets/Sets whether  the dialog will open automatically.
        /// </summary>
        [Bindable(true)]
        [Description("Gets/Sets whether  the dialog will open automatically.")]
        [Browsable(true)]
        [Category("Action")]
        [JQueryOption("autoOpen", IgnoreValue = true)]
        public bool AutoOpen
        {
            get
            {
                Object obj = ViewState["AutoOpen"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AutoOpen"] = value;
            }
        }

        /// <summary>
        /// When true, the bgiframe plugin will be used, 
        /// to fix the issue in IE6 where select boxes show on top of other elements, regardless of zIndex. 
        /// </summary>
        /// <remarks>
        /// When dialog detect the current browser is IE6 this property will be set to true.
        /// </remarks>
        [Description("When true, the bgiframe plugin will be used, to fix the issue in IE6 where select boxes show on top of other elements, regardless of zIndex.")]
        [Browsable(true)]
        [Category("Layout")]
        [JQueryOption("bgiframe", IgnoreValue = false)]
        public bool BgiFrame
        {
            get
            {
                Object obj = ViewState["BgiFrame"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["BgiFrame"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the dialog should close when it has focus and the user presses the esacpe (ESC) key.
        /// </summary>
        [Themeable(true)]
        [Description("Gets/Sets whether the dialog should close when it has focus and the user presses the esacpe (ESC) key.")]
        [Bindable(true)]
        [Browsable(true)]
        [Category("Action")]
        [JQueryOption("closeOnEscape", IgnoreValue = true)]
        public bool CloseOnEscape
        {
            get
            {
                Object obj = ViewState["CloseOnEscape"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["CloseOnEscape"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the dialog style sheet class.
        /// </summary>
        [Description("Gets/Sets the dialog style sheet class.")]
        [Themeable(true)]
        [Bindable(true)]
        [CssClassProperty]
        [Browsable(true)]
        [Category("Appearance")]
        [NotifyParentProperty(true)]
        [JQueryOption("dialogClass", IgnoreValue = "")]
        public override string CssClass
        {
            get
            {
                return base.CssClass;
            }
            set
            {
                base.CssClass = value;
            }
        }

        /// <summary>
        ///  Gets/Sets whether the dialog can be draggable and set to true it will be draggable by the titlebar.
        /// </summary>
        [JQueryOption("draggable", DefaultValue = true, IgnoreValue = true)]
        [Description("Gets/Sets whether the dialog can be draggable and set to true it will be draggable by the titlebar.")]
        [Browsable(true)]
        [Category("DragDop")]
        [NotifyParentProperty(true)]
        [Bindable(true)]
        public bool IsDraggable
        {
            get
            {
                Object obj = ViewState["Draggable"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["Draggable"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Dialog's Height
        /// </summary>
        [Themeable(true)]
        [Browsable(true)]
        [Description("Gets/Sets the Dialog's Height")]
        [Category("Appearance")]
        [NotifyParentProperty(true)]
        [JQueryOption("height")]
        public override Unit Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }

        /// <summary>
        /// Gets/Sets the effect to be used when the dialog is closed.
        /// </summary>
        [Themeable(true)]
        [Description("Gets/Sets the effect to be used when the dialog is closed.")]
        //[JQueryOption("hide")]
        [Browsable(true)]
        [Category("Appearance")]
        [Bindable(true)]
        public JQueryEffects HideEffect
        {
            get
            {
                Object obj = ViewState["HideEffect"];
                return (obj == null) ? JQueryEffects.None : (JQueryEffects)obj;
            }
            set
            {
                ViewState["HideEffect"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the dialog's maximum height to which the dialog can be resized, in pixels.
        /// </summary>
        [Themeable(true)]
        [Description("Gets/Sets the dialog's maximum height to which the dialog can be resized, in pixels.")]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [JQueryOption("maxHeight", IgnoreValue = 0)]
        [Bindable(true)]
        public int MaxHeight
        {
            get
            {
                Object obj = ViewState["MaxHeight"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["MaxHeight"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the maximum width to which the dialog can be resized, in pixels.
        /// </summary>
        [Description("Gets/Sets the maximum width to which the dialog can be resized, in pixels.")]
        [Themeable(true)]
        [Browsable(true)]
        [Category("Appearance")]
        [NotifyParentProperty(true)]
        [JQueryOption("maxWidth", IgnoreValue = 0)]
        [Bindable(true)]
        public int MaxWidth
        {
            get
            {
                Object obj = ViewState["MaxWidth"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["MaxWidth"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the minimum height to which the dialog can be resized, in pixels.
        /// </summary>
        [Description("Gets/Sets the minimum height to which the dialog can be resized, in pixels.")]
        [Themeable(true)]
        [Browsable(true)]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [JQueryOption("minHeight", IgnoreValue = 0)]
        public int MinHeight
        {
            get
            {
                Object obj = ViewState["MinHeight"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["MinHeight"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the minimum width to which the dialog can be resized, in pixels.
        /// </summary>
        [Description("Gets/Sets the minimum width to which the dialog can be resized, in pixels.")]
        [Bindable(true)]
        [Themeable(true)]
        [Browsable(true)]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [JQueryOption("minWidth", IgnoreValue = 0)]
        public int MinWidth
        {
            get
            {
                Object obj = ViewState["MinWidth"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["MinWidth"] = value;
            }
        }

        /// <summary>
        ///  Gets/Sets the title of dialog
        /// </summary>
        [Description("Gets/Sets the title of dialog")]
        [Bindable(true)]
        [JQueryOption("title")]
        [Browsable(true)]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [Localizable(true)]
        public string Title
        {
            get
            {
                Object obj = ViewState["Title"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the dialog modal behavior; other items on the page will be disabled (i.e. cannot be interacted with). 
        /// Modal dialogs create an overlay below the dialog but above other page elements.
        /// </summary>
        [JQueryOption("modal", IgnoreValue = false)]
        [Browsable(true)]
        [Category("Action")]
        public bool ShowModal
        {
            get
            {
                Object obj = ViewState["ShowModal"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowModal"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the dialog's default position when open
        /// </summary>
        [Category("Layout")]
        [NotifyParentProperty(true)]
        [Bindable(true)]
        [Description("Gets/Sets the dialog's default position when open")]
        public DialogPositions Position
        {
            get
            {
                Object obj = ViewState["Position"];
                return (obj == null) ? DialogPositions.Center : (DialogPositions)obj;
            }
            set
            {
                ViewState["Position"] = value;
            }
        }

        #region This Code provides by nibblersrevenge for more detail http://dj.codeplex.com/Thread/View.aspx?ThreadId=60151

        /// <summary>
        /// Gets/Sets the dialog's custom position when open
        /// </summary>
        [Category("Layout")]
        [NotifyParentProperty(true)]
        [Bindable(true)]
        [Description("Gets/Sets the dialog's custom position when open")]
        public int PositionLeft
        {
            get
            {
                object obj = ViewState["PositionLeft"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["PositionLeft"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the dialog's custom position when open
        /// </summary>
        [Category("Layout")]
        [NotifyParentProperty(true)]
        [Bindable(true)]
        [Description("Gets/Sets the dialog's custom position when open")]
        public int PositionTop
        {
            get
            {
                object obj = ViewState["PositionTop"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["PositionTop"] = value;
            }

        }

        /// <summary>
        /// Gets/Sets whether the dialog can be resizable
        /// </summary>
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [Themeable(true)]
        [Browsable(true)]
        [Category("Action")]
        [Description("Gets/Sets whether the dialog can be resizable")]
        [JQueryOption("resizable", IgnoreValue = true)]
        public bool IsResizable
        {
            get
            {
                Object obj = ViewState["Resizable"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["Resizable"] = value;
            }
        }
        #endregion

        /// <summary>
        /// Gets/Sets the effect to be used when the dialog is opened.
        /// </summary>
        [Description("Gets/Sets the effect to be used when the dialog is opened.")]
        [Themeable(true)]
        [Bindable(true)]
        [Category("Appearance")]
        //[JQueryOption("show")]
        public JQueryEffects ShowEffect
        {
            get
            {
                Object obj = ViewState["ShowEffect"];
                return (obj == null) ? JQueryEffects.None : (JQueryEffects)obj;
            }
            set
            {
                ViewState["ShowEffect"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the dialog will stack on top of other dialogs. 
        /// This will cause the dialog to move to the front of other dialogs when it gains focus.
        /// </summary>
        [Description("Gets/Sets whether the dialog will stack on top of other dialogs. ")]
        [Themeable(true)]
        [Bindable(true)]
        [Category("Layout")]
        [JQueryOption("stack", DefaultValue = true)]
        public bool IsStack
        {
            get
            {
                Object obj = ViewState["IsStack"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["IsStack"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the dialog width
        /// </summary>
        [Description("Gets/Sets the dialog width")]
        [Category("Appearance")]
        [Themeable(true)]
        [Bindable(true)]
        [JQueryOption("width")]
        public override Unit Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        /// <summary>
        /// Gets/sets the starting z-index for the dialog.
        /// </summary>
        [Description("Gets/sets the starting z-index for the dialog.")]
        [Category("Layout")]
        [Themeable(true)]
        [Bindable(true)]
        [JQueryOption("zIndex", IgnoreValue = 0)]
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

        /// <summary>
        /// Gets/Sets the dialog body template
        /// </summary>
        [Category("Layout")]
        [TemplateContainer(typeof(Dialog))]
        [TemplateInstanceAttribute(TemplateInstance.Single)]
        [DefaultValue(null)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [BrowsableAttribute(false)]
        public ITemplate BodyTemplate
        {
            get { return bodyTemplate; }
            set { bodyTemplate = value; }
        }

        /// <summary>
        /// Gets/Sets which control to trigger the dialog open;
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Action")]
        public JQuerySelector Trigger
        {
            get
            {
                if (trigger == null)
                    trigger = new JQuerySelector();
                return trigger;
            }
            set
            {
                trigger = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override string ToolTip
        {
            get
            {
                return base.ToolTip;
            }
            set
            {
                base.ToolTip = value;
            }
        }

        #endregion

        #region ClientEvents
        /// <summary>
        /// Gets/Sets the dialog beforeclose event client handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientBeforeClose=" $(this).dialog('option','autoOpen',true);"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog beforeclose event client handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("beforeclose", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientBeforeClose { get; set; }

        /// <summary>
        /// Gets/Sets the dialog open event client handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientOpen=" $(this).dialog('option','title','Simaple');"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog open event client handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("open", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientOpen { get; set; }

        /// <summary>
        /// Gets/Sets the dialog focus event client handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientFocus=" $(this).dialog('option','title','Editing');"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog focus event client handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("focus", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientFocus { get; set; }

        /// <summary>
        /// Gets/Sets the dialog dragStart event client handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientDragStart="var x=event.screenX;"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog dragStart event client handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("dragStart", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientDragStart { get; set; }

        /// <summary>
        /// Gets/Sets the dialog dragStop event client event handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientDragStop="var x=event.screenX;"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog dragStop event client event handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("dragStop", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientDragStop { get; set; }

        /// <summary>
        /// Gets/Sets the dialog drag event client handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientDrag="var x=event.screenX;"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog drag event client handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("drag", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientDrag { get; set; }

        /// <summary>
        /// Gets/Sets the dialog resizeStart event client event handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientResizeStart="var x=event.screenX;"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog resizeStart event client event handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("resizeStart", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientResizeStart { get; set; }

        /// <summary>
        /// Gets/Sets the dialog resize event client event handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientResize="var x=event.screenX;"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog resize event client event handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("resize", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientResize { get; set; }

        /// <summary>
        /// Gets/Sets the dialog resizeStop event client event handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        /// <example>
        ///  OnClientResizeStop="var x=event.screenX;"
        /// </example>
        [Browsable(true)]
        [Description("Gets/Sets the dialog resizeStop event client event handler")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("resizeStop", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientResizeStop { get; set; }

        /// <summary>
        /// Gets/Sets the dialog close event client handler
        /// </summary>
        /// <remarks>
        /// you can write the javascript in jQuery in this property directly if "function" declare not found this property will
        /// generate the function keyword automatic.
        /// this event have two params "event","ui"
        /// </remarks>
        [Browsable(true)]
        [Category("ClientEvents")]
        [Description("Gets/Sets the dialog close event client handler")]
        [Bindable(true)]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [JQueryOption("close", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientClose { get; set; }

        /// <summary>
        /// Specifies the collection which buttons should be displayed on the dialog. 
        /// </summary>
        [Category("Layout")]
        [Description("Specifies the collection which buttons should be displayed on the dialog. ")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<DialogButton> Buttons
        {
            get
            {
                if (buttons == null)
                    buttons = new List<DialogButton>();
                return buttons;
            }
        }

        /// <summary>
        /// Gets/Sets the message in Dialog
        /// </summary>
        /// <remarks>
        /// if the BodyTemplate is set this property will be ignore
        /// </remarks>
        [Category("Data")]
        [Description("Gets/Sets the message in Dialog")]
        [Localizable(true)]
        public string MessageText
        {
            get
            {
                Object obj = ViewState["MessageText"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["MessageText"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the message icon in dialog
        /// </summary>
        /// <remarks>
        /// If the BodyTemplate is set this property will be ignore
        /// </remarks>
        [Category("Appearance")]
        [Description("Gets/Sets the message icon in dialog")]
        [Editor(typeof(ImageUrlEditor), typeof(UITypeEditor))]
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets/Sets the default dialog buttons
        /// </summary>
        [Category("Layout")]
        [Description("Gets/Sets the default dialog buttons")]
        public DialogButtons DialogButtons { get; set; }

        /// <summary>
        /// Gets/Sets the default dialog icon
        /// </summary>
        [Category("Layout")]
        [Description("Gets/Sets the default dialog icon")]
        public DialogIcons DialogIcon { get; set; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Show the default modal dialog to display information.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="icon"></param>
        public static void ShowModalDialog(Control container, string title, string msg, DialogIcons icon)
        {
            //Dialog dialog = new Dialog();
            string id = "dj_sys_dialog";
            Dialog dialog = ClientScriptManager.FindControl(container.Page.Form, "dj_sys_dialog") as Dialog;
            if (dialog == null)
            {
                dialog = new Dialog();
                dialog.ID = id;
                container.Controls.Add(dialog);
            }

            dialog.IsResizable = false;
            dialog.IsDraggable = true;
            dialog.ShowModal = true;
            dialog.DialogButtons = DialogButtons.OK;
            if (dialog.Buttons != null)
                dialog.Buttons.Clear();
            dialog.Title = title;
            dialog.MessageText = msg;
            dialog.DialogIcon = icon;
            dialog.AutoOpen = true;
            //dialog.OnClienClose = "jQuery(this).dialog('destory');$(this).remove();";
        }

        #endregion

        /// <summary>
        /// When the dialog button click this event will be trigged
        /// </summary>
        public event CommandEventHandler ButtonCommand;

        protected override void OnInit(EventArgs e)
        {
            if (!DesignMode)
            {
                if (Context.Request.Browser.Browser == "IE")
                {
                    if (Context.Request.Browser.MajorVersion < 8)
                    {
                        this.BgiFrame = true;
                        ClientScriptManager.RegisterDocumentReadyScript(this, "$.ui.dialog.defaults.bgiframe = true;");
                    }
                }

                if (BgiFrame)
                    ClientScriptManager.AddCompositeScript(this, "jQueryNet.plugins.bgiframe.js", "jQueryNet");
            }
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!DesignMode)
            {
                JQueryScriptBuilder builder = new JQueryScriptBuilder(this);
                string fs = null;

                if (this.Buttons != null)
                {
                    List<string> functions = new List<string>();
                    ///check Button seq：1.check OnClientClick 2.check CommandName 3.check WebServiceUrl
                    foreach (DialogButton btn in Buttons)
                    {
                        if (!string.IsNullOrEmpty(btn.OnClientClick))
                        {
                            //if (!string.IsNullOrEmpty(btn.OnClientClick))
                            functions.Add(FormatFunctionString(btn.Text, btn.OnClientClick));
                        }
                        else
                        //if (string.IsNullOrEmpty(btn.CommandName))
                        {
                            string postScript = Page.ClientScript.GetPostBackEventReference(this, btn.CommandName + ":" + btn.CommandArgument);
                            functions.Add(FormatFunctionString(btn.Text, postScript));
                        }
                    }

                    if (functions.Count > 0)
                        fs = String.Join(",", functions.ToArray());
                }
                //Dictionary<string, string> options = new Dictionary<string, string>();
                builder.Prepare();

                if (Position != DialogPositions.Center)
                {
                    string pos = "";
                    switch (Position)
                    {
                        case DialogPositions.Top:
                            pos = "'top'";
                            break;
                        case DialogPositions.Bottom:
                            pos = "'bottom'";
                            break;
                        case DialogPositions.Left:
                            pos = "'left'";
                            break;
                        case DialogPositions.Right:
                            pos = "'right'";
                            break;
                        case DialogPositions.LeftTop:
                            pos = "['left','top']";
                            break;
                        case DialogPositions.RightTop:
                            pos = "['right','top']";
                            break;
                        case DialogPositions.LeftBottom:
                            pos = "['left','bottom']";
                            break;
                        case DialogPositions.RightBottom:
                            pos = "['right','bottom']";
                            break;
                        #region this code provides by nibblersrevenge for more detail :http://dj.codeplex.com/Thread/View.aspx?ThreadId=60151
                        case DialogPositions.PositionAbsolute:
                            pos = "[" + PositionLeft + "," + PositionTop + "]";
                            break;
                        #endregion
                    }
                    //options.Add("position", pos);
                    builder.AddOption("position", pos);
                }

                if (ShowEffect != JQueryEffects.None)
                    builder.AddOption("show", ShowEffect.ToString().ToLower(), true);

                if (HideEffect != JQueryEffects.None)
                    builder.AddOption("hide", HideEffect.ToString().ToLower(), true);

                if (!string.IsNullOrEmpty(fs))
                    builder.AddOption("buttons", "{" + fs + "}");
                //options.Add("buttons", "{" + fs + "}");

                builder.Build();

                if (!Trigger.IsEmpty)
                {
                    this.AutoOpen = false;
                    //builder.AppendSelector(Trigger);
                    builder.AppendBindFunction(Trigger, "click", "jQuery('#" + ClientID + "').dialog('open');void(0);");
                }

                #region v1.0.0.0
                //if (!string.IsNullOrEmpty(Trigger))
                //{
                //    this.AutoOpen = false;
                //    Control targetControl = ClientScriptManager.FindControl(Page.Form, Trigger);
                //    string targetID = Trigger;
                //    if (targetControl != null)
                //        targetID = targetControl.ClientID;

                //   // string script = "if ($get('" + targetID + "')!=null) $addHandler($get('" + targetID + "'),'click',function(){$('#" + this.ClientID + "').dialog('open');void(0);});";
                //   // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_reload", script);
                //}
                #endregion

                ClientScriptManager.RegisterJQueryControl(this, builder);

                //if (options.Count > 0)
                //    ClientScriptManager.RegisterJQueryControl(this, options);
                //else
                //    ClientScriptManager.RegisterJQueryControl(this);
            }
            //base.OnPreRender(e);
        }

        private string FormatFunctionString(string functionStr)
        {
            string formatted = functionStr;
            if (!formatted.EndsWith(";"))
                formatted += ";";

            if (!formatted.StartsWith("function()"))
                formatted = "function(){" + formatted + "}";
            return formatted;
        }

        private string FormatFunctionString(string text, string functionStr)
        {
            return "'" + text + "':" + FormatFunctionString(functionStr);
        }

        protected override void CreateChildControls()
        {
            if (!string.IsNullOrEmpty(Title))
                this.Attributes.Add("title", Title);
            else
                this.Attributes.Add("title", "Dialog");

            if (BodyTemplate == null)
                BodyTemplate = new CompiledTemplateBuilder(new BuildTemplateMethod(CreateDefaultDialog));

            if (!DesignMode)
            {
                BodyTemplate.InstantiateIn(this);
                this.Style.Add("display", "none");
            }
        }

        protected virtual void CreateDefaultDialog(Control template)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<div style='margin:10px'>");

            builder.Append("<div style='vertical-align:middle;height:64px;padding-left:40px;");

            string img = GetImageUrl();

            if (!string.IsNullOrEmpty(img))
            {
                builder.Append("background:transparent url(");
                builder.Append(img);
                builder.Append(") no-repeat;");
            }

            builder.Append("'>");
            builder.Append(MessageText + "</div></div>");
            if (!string.IsNullOrEmpty(img) && (!string.IsNullOrEmpty(MessageText)))
                template.Controls.Add(new LiteralControl(builder.ToString()));

            string closeScript = "jQuery(this).dialog('close');";

            if ((this.DialogButtons & DialogButtons.OK) == DialogButtons.OK)
            {
                DialogButton OKbtn = new DialogButton();
                OKbtn.Text = "OK";
                OKbtn.OnClientClick = closeScript;
                this.Buttons.Add(OKbtn);
            }

            if ((this.DialogButtons & DialogButtons.Cancel) == DialogButtons.Cancel)
            {
                DialogButton Cancelbtn = new DialogButton();
                Cancelbtn.Text = "Cancel";
                Cancelbtn.OnClientClick = closeScript;
                this.Buttons.Add(Cancelbtn);
            }

            if ((this.DialogButtons & DialogButtons.Cancel) == DialogButtons.Close)
            {
                DialogButton Cancelbtn = new DialogButton();
                Cancelbtn.Text = "Close";
                Cancelbtn.OnClientClick = closeScript;
                this.Buttons.Add(Cancelbtn);
            }

        }

        protected virtual string GetImageUrl()
        {
            if (!string.IsNullOrEmpty(IconUrl))
                return Page.ResolveUrl(IconUrl);

            switch (DialogIcon)
            {
                case DialogIcons.Info:
                    return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Info.gif");
                case DialogIcons.Stop:
                    return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Stop.gif");
                case DialogIcons.Question:
                    return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Question.gif");
                case DialogIcons.Error:
                    return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Error.gif");
            }

            return "";
        }

        #region IPostBackEventHandler Members

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (ButtonCommand != null)
            {
                if (!string.IsNullOrEmpty(eventArgument))
                {
                    EnsureChildControls();
                    string[] args = eventArgument.Split(new char[] { ':' });
                    CommandEventArgs e = new CommandEventArgs(args[0], args[1]);
                    ButtonCommand(this, e);
                }
            }
        }

        #endregion
    }


}
