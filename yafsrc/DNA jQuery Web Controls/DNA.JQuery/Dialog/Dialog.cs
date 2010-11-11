//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

#region Using

using System.Web.UI;

#endregion

[assembly: WebResource("DNA.UI.JQuery.Dialog.Error.gif", "image/gif")]
[assembly: WebResource("DNA.UI.JQuery.Dialog.Info.gif", "image/gif")]
[assembly: WebResource("DNA.UI.JQuery.Dialog.Question.gif", "image/gif")]
[assembly: WebResource("DNA.UI.JQuery.Dialog.Stop.gif", "image/gif")]

namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Drawing;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.Design;
  using System.Web.UI.WebControls;

  using DNA.UI.JQuery.Design;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// The Dialog is a web control encapsulated the jQuery UI dialog plugin. A Dialog is a floating window that contains a title bar and a content area. 
  ///   The dialog window can be moved, resized and closed with the 'x' icon by default.
  ///   If the content length exceeds the maximum height, a scrollbar will automatically appear.
  ///   A bottom button bar and semi-transparent modal overlay layer are common options that can be added.
  /// </summary>
  /// <example>
  /// <code lang="ASP.NET" title="Dialog 's Properties">
  /// <![CDATA[
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
  ///          Text=""/>
  ///          ...
  ///   </Buttons>
  ///    <Target TargetIDs="ControlID1, ... ,ControlIDn" 
  ///                Selector=".ui-widget-header" 
  ///                TargetID="ControlID"/>
  /// </DotNetAge:Dialog>]]>
  ///   </code>
  /// </example>
  [JQuery(Name = "dialog", Assembly = "jQueryNet", DisposeMethod = "destroy", 
    ScriptResources = new[] { "ui.core.js", "ui.draggable.js", "ui.resizable.js", "ui.dialog.js" }, 
    StartEvent = ClientRegisterEvents.ApplicationLoad)]
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Dialog runat=\"server\" ID=\"Dialog1\" Title=\"[DialogTitle]\"></{0}:Dialog>")]
  [Designer(typeof(DialogDesigner))]
  [ParseChildren(true, "Buttons")]
  [ToolboxBitmap(typeof(Dialog), "Dialog.Dialog.ico")]
  public class Dialog : CompositeControl, IPostBackEventHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The buttons.
    /// </summary>
    private List<DialogButton> buttons;

    /// <summary>
    /// The trigger.
    /// </summary>
    private JQuerySelector trigger;

    #endregion

    #region Events

    /// <summary>
    ///   When the dialog button click this event will be trigged
    /// </summary>
    public event CommandEventHandler ButtonCommand;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets whether  the dialog will open automatically.
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
        object obj = this.ViewState["AutoOpen"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["AutoOpen"] = value;
      }
    }

    /// <summary>
    ///   When true, the bgiframe plugin will be used, 
    ///   to fix the issue in IE6 where select boxes show on top of other elements, regardless of zIndex.
    /// </summary>
    /// <remarks>
    ///   When dialog detect the current browser is IE6 this property will be set to true.
    /// </remarks>
    [Description(
      "When true, the bgiframe plugin will be used, to fix the issue in IE6 where select boxes show on top of other elements, regardless of zIndex."
      )]
    [Browsable(true)]
    [Category("Layout")]
    [JQueryOption("bgiframe", IgnoreValue = false)]
    public bool BgiFrame
    {
      get
      {
        object obj = this.ViewState["BgiFrame"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["BgiFrame"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dialog body template
    /// </summary>
    [Category("Layout")]
    [TemplateContainer(typeof(Dialog))]
    [TemplateInstanceAttribute(TemplateInstance.Single)]
    [DefaultValue(null)]
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    [BrowsableAttribute(false)]
    public ITemplate BodyTemplate { get; set; }

    /// <summary>
    ///   Specifies the collection which buttons should be displayed on the dialog.
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
        if (this.buttons == null)
        {
          this.buttons = new List<DialogButton>();
        }

        return this.buttons;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the dialog should close when it has focus and the user presses the esacpe (ESC) key.
    /// </summary>
    [Themeable(true)]
    [Description(
      "Gets/Sets whether the dialog should close when it has focus and the user presses the esacpe (ESC) key.")]
    [Bindable(true)]
    [Browsable(true)]
    [Category("Action")]
    [JQueryOption("closeOnEscape", IgnoreValue = true)]
    public bool CloseOnEscape
    {
      get
      {
        object obj = this.ViewState["CloseOnEscape"];
        return (obj == null) ? true : (bool)obj;
      }

      set
      {
        this.ViewState["CloseOnEscape"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dialog style sheet class.
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
    ///   Gets/Sets the default dialog buttons
    /// </summary>
    [Category("Layout")]
    [Description("Gets/Sets the default dialog buttons")]
    public DialogButtons DialogButtons { get; set; }

    /// <summary>
    ///   Gets/Sets the default dialog icon
    /// </summary>
    [Category("Layout")]
    [Description("Gets/Sets the default dialog icon")]
    public DialogIcons DialogIcon { get; set; }

    /// <summary>
    ///   Gets/Sets the Dialog's Height
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
    ///   Gets/Sets the effect to be used when the dialog is closed.
    /// </summary>
    [Themeable(true)]
    [Description("Gets/Sets the effect to be used when the dialog is closed.")]
    // [JQueryOption("hide")]
    [Browsable(true)]
    [Category("Appearance")]
    [Bindable(true)]
    public JQueryEffects HideEffect
    {
      get
      {
        object obj = this.ViewState["HideEffect"];
        return (obj == null) ? JQueryEffects.None : (JQueryEffects)obj;
      }

      set
      {
        this.ViewState["HideEffect"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the message icon in dialog
    /// </summary>
    /// <remarks>
    ///   If the BodyTemplate is set this property will be ignore
    /// </remarks>
    [Category("Appearance")]
    [Description("Gets/Sets the message icon in dialog")]
    [Editor(typeof(ImageUrlEditor), typeof(UITypeEditor))]
    public string IconUrl { get; set; }

    /// <summary>
    ///   Gets/Sets whether the dialog can be draggable and set to true it will be draggable by the titlebar.
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
        object obj = this.ViewState["Draggable"];
        return (obj == null) ? true : (bool)obj;
      }

      set
      {
        this.ViewState["Draggable"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the dialog can be resizable
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
        object obj = this.ViewState["Resizable"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["Resizable"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the dialog will stack on top of other dialogs. 
    ///   This will cause the dialog to move to the front of other dialogs when it gains focus.
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
        object obj = this.ViewState["IsStack"];
        return (obj == null) ? true : (bool)obj;
      }

      set
      {
        this.ViewState["IsStack"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dialog's maximum height to which the dialog can be resized, in pixels.
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
        object obj = this.ViewState["MaxHeight"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MaxHeight"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the maximum width to which the dialog can be resized, in pixels.
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
        object obj = this.ViewState["MaxWidth"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MaxWidth"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the message in Dialog
    /// </summary>
    /// <remarks>
    ///   if the BodyTemplate is set this property will be ignore
    /// </remarks>
    [Category("Data")]
    [Description("Gets/Sets the message in Dialog")]
    [Localizable(true)]
    public string MessageText
    {
      get
      {
        object obj = this.ViewState["MessageText"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["MessageText"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the minimum height to which the dialog can be resized, in pixels.
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
        object obj = this.ViewState["MinHeight"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MinHeight"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the minimum width to which the dialog can be resized, in pixels.
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
        object obj = this.ViewState["MinWidth"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["MinWidth"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dialog beforeclose event client handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientBeforeClose=" jQuery(this).dialog('option','autoOpen',true);"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog beforeclose event client handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("beforeclose", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientBeforeClose { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog close event client handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    [Browsable(true)]
    [Category("ClientEvents")]
    [Description("Gets/Sets the dialog close event client handler")]
    [Bindable(true)]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [JQueryOption("close", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientClose { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog drag event client handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientDrag="var x=event.screenX;"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog drag event client handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("drag", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientDrag { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog dragStart event client handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientDragStart="var x=event.screenX;"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog dragStart event client handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("dragStart", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientDragStart { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog dragStop event client event handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientDragStop="var x=event.screenX;"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog dragStop event client event handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("dragStop", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientDragStop { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog focus event client handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientFocus=" jQuery(this).dialog('option','title','Editing');"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog focus event client handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("focus", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientFocus { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog open event client handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientOpen=" jQuery(this).dialog('option','title','Simaple');"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog open event client handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("open", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientOpen { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog resize event client event handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientResize="var x=event.screenX;"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog resize event client event handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("resize", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientResize { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog resizeStart event client event handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientResizeStart="var x=event.screenX;"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog resizeStart event client event handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("resizeStart", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientResizeStart { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog resizeStop event client event handler
    /// </summary>
    /// <remarks>
    ///   you can write the javascript in jQuery in this property directly if "function" declare not found this property will
    ///   generate the function keyword automatic.
    ///   this event have two params "event","ui"
    /// </remarks>
    /// <example>
    ///   OnClientResizeStop="var x=event.screenX;"
    /// </example>
    [Browsable(true)]
    [Description("Gets/Sets the dialog resizeStop event client event handler")]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("resizeStop", JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientResizeStop { get; set; }

    /// <summary>
    ///   Gets/Sets the dialog's default position when open
    /// </summary>
    [Category("Layout")]
    [NotifyParentProperty(true)]
    [Bindable(true)]
    [Description("Gets/Sets the dialog's default position when open")]
    public DialogPositions Position
    {
      get
      {
        object obj = this.ViewState["Position"];
        return (obj == null) ? DialogPositions.Center : (DialogPositions)obj;
      }

      set
      {
        this.ViewState["Position"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dialog's custom position when open
    /// </summary>
    [Category("Layout")]
    [NotifyParentProperty(true)]
    [Bindable(true)]
    [Description("Gets/Sets the dialog's custom position when open")]
    public int PositionLeft
    {
      get
      {
        object obj = this.ViewState["PositionLeft"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["PositionLeft"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dialog's custom position when open
    /// </summary>
    [Category("Layout")]
    [NotifyParentProperty(true)]
    [Bindable(true)]
    [Description("Gets/Sets the dialog's custom position when open")]
    public int PositionTop
    {
      get
      {
        object obj = this.ViewState["PositionTop"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["PositionTop"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the effect to be used when the dialog is opened.
    /// </summary>
    [Description("Gets/Sets the effect to be used when the dialog is opened.")]
    [Themeable(true)]
    [Bindable(true)]
    [Category("Appearance")]
    // [JQueryOption("show")]
      public JQueryEffects ShowEffect
    {
      get
      {
        object obj = this.ViewState["ShowEffect"];
        return (obj == null) ? JQueryEffects.None : (JQueryEffects)obj;
      }

      set
      {
        this.ViewState["ShowEffect"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dialog modal behavior; other items on the page will be disabled (i.e. cannot be interacted with). 
    ///   Modal dialogs create an overlay below the dialog but above other page elements.
    /// </summary>
    [JQueryOption("modal", IgnoreValue = false)]
    [Browsable(true)]
    [Category("Action")]
    public bool ShowModal
    {
      get
      {
        object obj = this.ViewState["ShowModal"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ShowModal"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the title of dialog
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
        object obj = this.ViewState["Title"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["Title"] = value;
      }
    }

    /// <summary>
    /// Gets or sets ToolTip.
    /// </summary>
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

    /// <summary>
    ///   Gets/Sets which control to trigger the dialog open;
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
    /// Gets or sets a value indicating whether Visible.
    /// </summary>
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

    /// <summary>
    ///   Gets/Sets the dialog width
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
    ///   Gets/sets the starting z-index for the dialog.
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
        object obj = this.ViewState["ZIndex"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ZIndex"] = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show the default modal dialog to display information.
    /// </summary>
    /// <param name="container">
    /// </param>
    /// <param name="title">
    /// </param>
    /// <param name="msg">
    /// </param>
    /// <param name="icon">
    /// </param>
    public static void ShowModalDialog(Control container, string title, string msg, DialogIcons icon)
    {
      // Dialog dialog = new Dialog();
      string id = "dj_sys_dialog";
      var dialog = ClientScriptManager.FindControl(container.Page.Form, "dj_sys_dialog") as Dialog;
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
      {
        dialog.Buttons.Clear();
      }

      dialog.Title = title;
      dialog.MessageText = msg;
      dialog.DialogIcon = icon;
      dialog.AutoOpen = true;

      // dialog.OnClienClose = "jQuery(this).dialog('destory');jQuery(this).remove();";
    }

    #endregion

    #region Implemented Interfaces

    #region IPostBackEventHandler

    /// <summary>
    /// The raise post back event.
    /// </summary>
    /// <param name="eventArgument">
    /// The event argument.
    /// </param>
    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
      if (this.ButtonCommand != null)
      {
        if (!string.IsNullOrEmpty(eventArgument))
        {
          this.EnsureChildControls();
          string[] args = eventArgument.Split(new[] { ':' });
          var e = new CommandEventArgs(args[0], args[1]);
          this.ButtonCommand(this, e);
        }
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The create child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
      if (!string.IsNullOrEmpty(this.Title))
      {
        this.Attributes.Add("title", this.Title);
      }
      else
      {
        this.Attributes.Add("title", "Dialog");
      }

      if (this.BodyTemplate == null)
      {
        this.BodyTemplate = new CompiledTemplateBuilder(this.CreateDefaultDialog);
      }

      if (!this.DesignMode)
      {
        this.BodyTemplate.InstantiateIn(this);
        this.Style.Add("display", "none");
      }
    }

    /// <summary>
    /// The create default dialog.
    /// </summary>
    /// <param name="template">
    /// The template.
    /// </param>
    protected virtual void CreateDefaultDialog(Control template)
    {
      var builder = new StringBuilder();
      builder.Append("<div style='margin:10px'>");

      builder.Append("<div style='vertical-align:middle;height:64px;padding-left:40px;");

      string img = this.GetImageUrl();

      if (!string.IsNullOrEmpty(img))
      {
        builder.Append("background:transparent url(");
        builder.Append(img);
        builder.Append(") no-repeat;");
      }

      builder.Append("'>");
      builder.Append(this.MessageText + "</div></div>");
      if (!string.IsNullOrEmpty(img) && (!string.IsNullOrEmpty(this.MessageText)))
      {
        template.Controls.Add(new LiteralControl(builder.ToString()));
      }

      string closeScript = "jQuery(this).dialog('close');";

      if ((this.DialogButtons & DialogButtons.OK) == DialogButtons.OK)
      {
        var OKbtn = new DialogButton();
        OKbtn.Text = "OK";
        OKbtn.OnClientClick = closeScript;
        this.Buttons.Add(OKbtn);
      }

      if ((this.DialogButtons & DialogButtons.Cancel) == DialogButtons.Cancel)
      {
        var Cancelbtn = new DialogButton();
        Cancelbtn.Text = "Cancel";
        Cancelbtn.OnClientClick = closeScript;
        this.Buttons.Add(Cancelbtn);
      }

      if ((this.DialogButtons & DialogButtons.Cancel) == DialogButtons.Close)
      {
        var Cancelbtn = new DialogButton();
        Cancelbtn.Text = "Close";
        Cancelbtn.OnClientClick = closeScript;
        this.Buttons.Add(Cancelbtn);
      }
    }

    /// <summary>
    /// The get image url.
    /// </summary>
    /// <returns>
    /// The get image url.
    /// </returns>
    protected virtual string GetImageUrl()
    {
      if (!string.IsNullOrEmpty(this.IconUrl))
      {
        return this.Page.ResolveUrl(this.IconUrl);
      }

      switch (this.DialogIcon)
      {
        case DialogIcons.Info:
          return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Info.gif");
          break;
        case DialogIcons.Stop:
          return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Stop.gif");
          break;
        case DialogIcons.Question:
          return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Question.gif");
          break;
        case DialogIcons.Error:
          return ClientScriptManager.GetControlResourceWebUrl(this, "DNA.UI.JQuery.Dialog.Error.gif");
          break;
      }

      return string.Empty;
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      if (!this.DesignMode)
      {
        if (this.Context.Request.Browser.Browser == "IE")
        {
          if (this.Context.Request.Browser.MajorVersion < 8)
          {
            this.BgiFrame = true;
            ClientScriptManager.RegisterDocumentReadyScript(this, "$.ui.dialog.defaults.bgiframe = true;");
          }
        }

        if (this.BgiFrame)
        {
          ClientScriptManager.AddCompositeScript(this, "jQueryNet.plugins.bgiframe.js", "jQueryNet");
        }
      }

      base.OnInit(e);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      if (!this.DesignMode)
      {
        var builder = new JQueryScriptBuilder(this);
        string fs = null;

        if (this.Buttons != null)
        {
          var functions = new List<string>();

          ///check Button seq：1.check OnClientClick 2.check CommandName 3.check WebServiceUrl
          foreach (DialogButton btn in this.Buttons)
          {
            if (!string.IsNullOrEmpty(btn.OnClientClick))
            {
              // if (!string.IsNullOrEmpty(btn.OnClientClick))
              functions.Add(this.FormatFunctionString(btn.Text, btn.OnClientClick));
            }
            else
            {
              // if (string.IsNullOrEmpty(btn.CommandName))
              string postScript = this.Page.ClientScript.GetPostBackEventReference(
                this, btn.CommandName + ":" + btn.CommandArgument);
              functions.Add(this.FormatFunctionString(btn.Text, postScript));
            }
          }

          if (functions.Count > 0)
          {
            fs = String.Join(",", functions.ToArray());
          }
        }

        // Dictionary<string, string> options = new Dictionary<string, string>();
        builder.Prepare();

        if (this.Position != DialogPositions.Center)
        {
          string pos = string.Empty;
          switch (this.Position)
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

            case DialogPositions.PositionAbsolute:
              pos = "[" + this.PositionLeft + "," + this.PositionTop + "]";
              break;
          }

          // options.Add("position", pos);
          builder.AddOption("position", pos);
        }

        if (this.ShowEffect != JQueryEffects.None)
        {
          builder.AddOption("show", this.ShowEffect.ToString().ToLower(), true);
        }

        if (this.HideEffect != JQueryEffects.None)
        {
          builder.AddOption("hide", this.HideEffect.ToString().ToLower(), true);
        }

        if (!string.IsNullOrEmpty(fs))
        {
          builder.AddOption("buttons", "{" + fs + "}");
        }

        // options.Add("buttons", "{" + fs + "}");
        builder.Build();

        if (!this.Trigger.IsEmpty)
        {
          this.AutoOpen = false;

          // builder.AppendSelector(Trigger);
          builder.AppendBindFunction(this.Trigger, "click", "jQuery('#" + this.ClientID + "').dialog('open');void(0);");
        }

        // if (!string.IsNullOrEmpty(Trigger))
        // {
        // this.AutoOpen = false;
        // Control targetControl = ClientScriptManager.FindControl(Page.Form, Trigger);
        // string targetID = Trigger;
        // if (targetControl != null)
        // targetID = targetControl.ClientID;

        // // string script = "if ($get('" + targetID + "')!=null) $addHandler($get('" + targetID + "'),'click',function(){$('#" + this.ClientID + "').dialog('open');void(0);});";
        // // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_reload", script);
        // }
        ClientScriptManager.RegisterJQueryControl(this, builder);

        // if (options.Count > 0)
        // ClientScriptManager.RegisterJQueryControl(this, options);
        // else
        // ClientScriptManager.RegisterJQueryControl(this);
      }

      // base.OnPreRender(e);
    }

    /// <summary>
    /// The format function string.
    /// </summary>
    /// <param name="functionStr">
    /// The function str.
    /// </param>
    /// <returns>
    /// The format function string.
    /// </returns>
    private string FormatFunctionString(string functionStr)
    {
      string formatted = functionStr;
      if (!formatted.EndsWith(";"))
      {
        formatted += ";";
      }

      if (!formatted.StartsWith("function()"))
      {
        formatted = "function(){" + formatted + "}";
      }

      return formatted;
    }

    /// <summary>
    /// The format function string.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="functionStr">
    /// The function str.
    /// </param>
    /// <returns>
    /// The format function string.
    /// </returns>
    private string FormatFunctionString(string text, string functionStr)
    {
      return "'" + text + "':" + this.FormatFunctionString(functionStr);
    }

    #endregion
  }
}