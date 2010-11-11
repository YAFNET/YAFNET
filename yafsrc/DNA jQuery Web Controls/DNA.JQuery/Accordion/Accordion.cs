//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.Collections.Specialized;
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
  /// The Accordion is a web control encapsulated the jQuery UI drappable plugin that
  ///   allows you to provide multiple views and display them one at a time. It is like having
  ///   several CollapsiblePanels where only one can be expanded at a
  ///   time. The Accordion is implemented as a web control that contains View web controls.
  ///   Each View control has a template for its Content.
  /// </summary>
  /// <remarks>
  /// <para>
  /// </para>
  /// <para>
  /// You can place to kind of view inside the Accordion
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// </item>
  /// <item>
  /// View: contain the server controls or html controls
  /// </item>
  /// <item>
  /// NavView:this view can reparsent the nav view controls.
  /// </item>
  /// </list>
  /// </remarks>
  /// <seealso cref="NavView">NavView Class</seealso>
  /// <seealso cref="View">View Class</seealso>
  /// <example>
  /// <code lang="ASP.NET" title="Accordion Control's Properties">
  /// <![CDATA[
  /// <DotNetAge:Accordion ID="MyAccordion"
  ///           AllowCollapseAllSections="false"
  ///           AutoPostBack="false"
  ///           AutoSizeMode="None"
  ///           CollapseAnimation="BounceSlide"
  ///           ContentCssClass="ui-widget-content"
  ///           ContentStyle=""
  ///           HeaderCssClass=""
  ///           HeaderIconCssClass=""
  ///           HeaderSelectedIconCssClass=""
  ///           HeaderStyle=""
  ///           IsClearStyle="false"
  ///           OnClientViewChange=""
  ///           SelectedIndex="0"
  ///           OpenSectionEvent="Click"
  /// >
  ///    <Views>
  ///       <DotNetAge:View Text=""
  ///             ID="View1"
  ///             NavigateUrl=""
  ///             HeaderCssClass=""
  ///             HeaderStyle=""
  ///             Target="_blank">
  ///             <p>View is content is here</p>
  ///       </DotNetAge:View>
  ///       ...
  ///       <DotNetAge:View/>
  ///    </Views>
  /// </DotNetAge:Accordion>]]>
  ///   </code>
  /// </example>
  [JQuery(Name = "accordion", Assembly = "jQueryNet", DisposeMethod = "destroy", 
    ScriptResources = new[] { "ui.core.js", "effects.core.js", "ui.accordion.js", "plugins.easing.1.3.js" }, 
    StartEvent = ClientRegisterEvents.ApplicationLoad)]
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Accordion runat=\"server\" id=\"Accordion1\"></{0}:Accordion>")]
  [Designer(typeof(AccordionDesigner))]
  [ToolboxBitmap(typeof(Accordion), "Accordion.Accordion.ico")]
  [ParseChildren(true)]
  public class Accordion : JQueryMultiViewControl
  {
    #region Events

    /// <summary>
    ///   When Accordion is Changed this event will be trigged.
    /// </summary>
    /// <remarks>
    ///   The AutoPostBack Must be set to true for handling this server event
    /// </remarks>
    public event EventHandler ActiveViewChanged;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets whether all the sections can be closed at once. Allows collapsing the active section by the triggering event (click is the default).
    /// </summary>
    [JQueryOption(Name = "collapsible", DefaultValue = true, IgnoreValue = false)]
    [Themeable(true)]
    [Category("Behavior")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Bindable(true)]
    [Description(
      "Gets/Sets whether all the sections can be closed at once. Allows collapsing the active section by the triggering event (click is the default)."
      )]
    public bool AllowCollapseAllSections
    {
      get
      {
        object obj = this.ViewState["Collapsible"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["Collapsible"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the growth of the Accordion
    /// </summary>
    /// <remarks>
    ///   It also supports three AutoSize modes so it can fit in a variety of layouts.
    ///   <list type = "bullet">
    ///     <item></item>
    ///     <item><b>None</b> - The Accordion grows/shrinks without restriction. This can
    ///       cause other elements on your page to move up and down with it.</item>
    ///     <item><b>Limit</b> - The Accordion never grows larger than the value specified
    ///       by its Height property. This will cause the content to scroll if it is too
    ///       large to be displayed.</item>
    ///     <item><b>Fill</b> - The Accordion always stays the exact same size as its
    ///       Height property. This will cause the content to be expanded or shrunk if it
    ///       isn't the right size.</item>
    ///   </list>
    /// </remarks>
    [Category("Layout")]
    [Description("Gets/Sets the growth of the Accordion")]
    [Themeable(true)]
    [Bindable(true)]
    public AccordionSizeModes AutoSizeMode
    {
      get
      {
        object obj = this.ViewState["AutoSizeMode"];
        return (obj == null) ? AccordionSizeModes.None : (AccordionSizeModes)obj;
      }

      set
      {
        this.ViewState["AutoSizeMode"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets your favorite animation of accrodion In addition to the default,
    ///   'BounceSlide' and 'EaseSlide' are supported
    /// </summary>
    [Category("Behavior")]
    [Description(
      "Gets/Sets your favorite animation of accrodion.In addition to the default, 'BounceSlide' and 'EaseSlide' are supported."
      )]
    [Themeable(true)]
    [Bindable(true)]
    [JQueryOption(Name = "animated")]
    public CollapseAnimations CollapseAnimation
    {
      get
      {
        object obj = this.ViewState["CollapseAnimation"];
        return (obj == null) ? CollapseAnimations.BounceSlide : (CollapseAnimations)obj;
      }

      set
      {
        this.ViewState["CollapseAnimation"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the header icon style class.
    /// </summary>
    [Description("Gets/Sets the header icon style class.")]
    [Category("Appearance")]
    [Themeable(true)]
    [Bindable(true)]
    public string HeaderIconCssClass
    {
      get
      {
        object obj = this.ViewState["HeaderIconCssClass"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["HeaderIconCssClass"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the header selected icon style class.
    /// </summary>
    [Description("Gets/Sets the header selected icon style class.")]
    [Category("Appearance")]
    [Themeable(true)]
    [Bindable(true)]
    public string HeaderSelectedIconCssClass
    {
      get
      {
        object obj = this.ViewState["HeaderSelectedIconCssClass"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["HeaderSelectedIconCssClass"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the clears height and overflow styles after finishing animations. 
    ///   This enables accordions to work with dynamic content. Won't work together with AutoHeight.
    /// </summary>
    [JQueryOption(Name = "clearStyle", IgnoreValue = false)]
    [Description(
      "Gets/Sets the clears height and overflow styles after finishing animations. This enables accordions to work with dynamic content. Won't work together with AutoHeight."
      )]
    [Themeable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Browsable(true)]
    [Bindable(true)]
    public bool IsClearStyle
    {
      get
      {
        object obj = this.ViewState["ClearStyle"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ClearStyle"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets Accordion looks for the anchor that matches location.href and activates it. 
    ///   Great for href-based state-saving. Use navigationFilter to implement your own matcher.
    /// </summary>
    [JQueryOption(Name = "navigation", IgnoreValue = false)]
    [Category("Behavior")]
    [Description(
      "Gets/Sets Accordion looks for the anchor that matches location.href and activates it. Great for href-based state-saving. Use navigationFilter to implement your own matcher."
      )]
    [Themeable(true)]
    [Bindable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    public bool Navigation
    {
      get
      {
        object obj = this.ViewState["Navigation"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["Navigation"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the client function that overwrite the default location.href-matching with your own matcher.(from jquery).
    /// </summary>
    [Description(
      "Gets/Sets the client function that overwrite the default location.href-matching with your own matcher.")]
    [Category("Behavior")]
    [JQueryOption(Name = "navigationFilter", Type = JQueryOptionTypes.Function)]
    [Themeable(true)]
    [Bindable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    public string NavigationFilter
    {
      get
      {
        object obj = this.ViewState["NavigationFilter"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["NavigationFilter"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the client event handler for accordion's changes event
    /// </summary>
    /// <remarks>
    ///   This event's has two params for client function.
    ///   event: jquery object
    ///   ui:jquery object
    /// </remarks>
    [Description("Gets/Sets the client event handler for accordion's changes event")]
    [Category("ClientEvents")]
    [Bindable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [JQueryOption(Name = "change", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientViewChange { get; set; }

    /// <summary>
    ///   Gets/Sets the client event on which to trigger the accordion.
    /// </summary>
    [JQueryOption(Name = "event")]
    [Category("Behavior")]
    [Description("Gets/Sets the client event on which to trigger the accordion.")]
    [Themeable(true)]
    [Bindable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    public DomEvents OpenSectionEvent
    {
      get
      {
        object obj = this.ViewState["Event"];
        return (obj == null) ? DomEvents.Click : (DomEvents)obj;
      }

      set
      {
        this.ViewState["Event"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the active view control's index
    /// </summary>
    [Description("Gets/Sets the active view control's index")]
    [JQueryOption(Name = "active")]
    public override int SelectedIndex
    {
      get
      {
        return base.SelectedIndex;
      }

      set
      {
        base.SelectedIndex = value;
      }
    }

    /// <summary>
    /// Gets or sets ToolTip.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

    #region Methods

    /// <summary>
    /// Override the LoadPostData to handling the ActiveViewChanged event
    /// </summary>
    /// <param name="postDataKey">
    /// </param>
    /// <param name="postCollection">
    /// </param>
    /// <returns>
    /// The load post data.
    /// </returns>
    protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
    {
      this.EnsureChildControls();
      if (!string.IsNullOrEmpty(postCollection[this.HiddenKey]))
      {
        string viewID = postCollection[this.HiddenKey];
        int index = this.Views[viewID].Index;
        if (index == this.SelectedIndex)
        {
          return false;
        }
        else
        {
          this.NewIndex = index;
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// The on active view changed.
    /// </summary>
    protected override void OnActiveViewChanged()
    {
      if (this.ActiveViewChanged != null)
      {
        this.ActiveViewChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Initizialize the Accordion
    /// </summary>
    /// <param name="e">
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);

      if (!this.DesignMode)
      {
        this.Page.RegisterRequiresPostBack(this);

        // ScriptManager.GetCurrent(Page).RegisterPostBackControl(this);
      }
      else
      {
        if (string.IsNullOrEmpty(this.CssClass))
        {
          this.CssClass = "ui-accordion ui-widget ui-helper-reset";
        }
      }
    }

    /// <summary>
    /// Register thie additional options for Accordion
    /// </summary>
    /// <param name="e">
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      if (!this.DesignMode)
      {
        string hiddenFieldValue = string.Empty;
        if (this.Views[this.SelectedIndex] != null)
        {
          hiddenFieldValue = this.Views[this.SelectedIndex].ClientID;
        }

        this.Page.ClientScript.RegisterHiddenField(this.HiddenKey, hiddenFieldValue);

        var builder = new JQueryScriptBuilder(this);

        if ((!string.IsNullOrEmpty(this.HeaderIconCssClass)) && (!string.IsNullOrEmpty(this.HeaderSelectedIconCssClass)))
        {
          builder.AddOption(
            "icons", 
            "{'header':'" + this.HeaderIconCssClass + "','headerSelected':'" + this.HeaderSelectedIconCssClass + "'}");
        }

        switch (this.AutoSizeMode)
        {
          case AccordionSizeModes.Fill:
            builder.AddOption("fillSpace", true);
            break;
          case AccordionSizeModes.Limit:
            break;
          case AccordionSizeModes.None:
            builder.AddOption("autoHeight", false);
            break;
        }

        builder.Prepare();
        builder.Build();

        // v1.1.15
        string onchange = "if (ui.newHeader.context) {jQuery('#" + this.HiddenKey +
                          "').attr('value',ui.newHeader.context.nextSibling.id);}";
        if (this.AutoPostBack)
        {
          if (this.Views.Count > 0)
          {
            foreach (View view in this.Views)
            {
              if (view.Index == this.SelectedIndex)
              {
                view.SetVisible(true);
              }
              else
              {
                view.SetVisible(false);
              }
            }

            onchange += this.Page.ClientScript.GetPostBackEventReference(this, "viewChange") + ";";
          }
        }

        builder.AppendBindFunction("accordionchange", new[] { "event", "ui" }, onchange);
        if (this.AutoSizeMode == AccordionSizeModes.None)
        {
          this.Style.Add("display", "none");
          builder.AppendCssStyle("'display':'block'");
        }

        ClientScriptManager.RegisterJQueryControl(this, builder);
      }
    }

    #endregion
  }
}