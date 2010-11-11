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
  using System.Web.UI.WebControls;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// The progress bar is designed to simply display the current % complete 
  ///   for a process and can be animated by updating the current values over time. 
  ///   The bar is coded to be flexibly sized through CSS and will scale to fit inside it's parent container by default.
  /// </summary>
  /// <remarks>
  /// This is a determinate progress bar, meaning that it should only be used in situations 
  ///   where the system can accurately update the current status complete. A determinate progress bar should
  ///   never fill from left to right, then loop back to empty for a single process -- if the actual percent complete status
  ///   cannot be calculated, an indeterminate progress bar (coming soon) or spinner animation is 
  ///   a better way to provide user feedback.
  /// </remarks>
  [JQuery(Name = "progressbar", Assembly = "jQueryNet", DisposeMethod = "destroy", 
    ScriptResources = new[] { "ui.core.js", "ui.progressbar.js", "ui.resizable.js" }, 
    StartEvent = ClientRegisterEvents.ApplicationLoad)]
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:Progressbar runat=\"server\" ID=\"Progressbar1\"></{0}:Progressbar>")]
  [ToolboxBitmap(typeof(Progressbar), "Progressbar.Progressbar.ico")]
  public class Progressbar : WebControl, INamingContainer, IPostBackDataHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The new value.
    /// </summary>
    private int newValue;

    /// <summary>
    /// The percentage label style.
    /// </summary>
    private Style percentageLabelStyle;

    #endregion

    #region Events

    /// <summary>
    ///   Triggered when the progressbar 's value changed
    /// </summary>
    public event EventHandler ValueChanged;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the client change event handler
    /// </summary>
    [Category("ClientEvents")]
    [Description("Gets/Sets the client change event handler")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Bindable(true)]
    [JQueryOption("change", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "event", "ui" })]
    public string OnClientValueChanged { get; set; }

    ///// <summary>
    ///// Gets/Sets whether the progressbar can post the data to server
    ///// </summary>
    // [Category("Behavior")]
    // [Description("Gets/Sets whether the progressbar can post the data to server")]
    // public bool AutoPostBack
    // {
    // get
    // {
    // Object obj = ViewState["AutoPostBack"];
    // return (obj == null) ? false : (bool)obj;
    // }
    // set
    // {
    // ViewState["AutoPostBack"] = value;
    // }
    // }

    /// <summary>
    ///   Gets/Sets the percentage label style
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the percentage label style")]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public Style PercentageLabelStyle
    {
      get
      {
        if (this.percentageLabelStyle == null)
        {
          this.percentageLabelStyle = new Style();
        }

        return this.percentageLabelStyle;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the Progressbar can resizable
    /// </summary>
    [Category("Layout")]
    [Description("Gets/Sets whether the Progressbar can resizable")]
    public bool Resizable
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
    ///   Gets/Sets whether the Progressbar can show the percentage label
    /// </summary>
    [Category("Layout")]
    [Description("Gets/Sets whether the Progressbar can show the percentage label")]
    public bool ShowPercentage
    {
      get
      {
        object obj = this.ViewState["ShowPercentage"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ShowPercentage"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the value of the progressbar.
    /// </summary>
    [Category("Data")]
    [Description("Gets/Sets the value of the progressbar.")]
    [NotifyParentProperty(true)]
    [Bindable(true)]
    [JQueryOption("value")]
    public int Value
    {
      get
      {
        object obj = this.ViewState["Value"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["Value"] = value;
      }
    }

    /// <summary>
    /// Gets TagKey.
    /// </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        return HtmlTextWriterTag.Div;
      }
    }

    /// <summary>
    /// Gets HiddenKey.
    /// </summary>
    private string HiddenKey
    {
      get
      {
        return this.ClientID + "_value";
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The render begin tag.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public override void RenderBeginTag(HtmlTextWriter writer)
    {
      if (this.ShowPercentage)
      {
        var panel = new Panel();
        panel.ID = this.ClientID + "_PercentagePanel";
        panel.ApplyStyle(this.PercentageLabelStyle);
        var label = new Label();
        panel.Style.Add("text-align", "center");
        if (this.Width.IsEmpty)
        {
          panel.Style.Add("display", "block");
        }
        else
        {
          panel.Width = this.Width;
        }

        panel.Controls.Add(label);
        label.Text = this.Value + "%";
        panel.RenderControl(writer);
      }

      if (this.DesignMode)
      {
        this.Style.Add("display", "block");
        this.CssClass = "ui-progressbar ui-widget ui-widget-content ui-corner-all";
        if (this.Resizable)
        {
          this.Style.Add("position", "relative");
        }
      }

      base.RenderBeginTag(writer);
    }

    #endregion

    #region Implemented Interfaces

    #region IPostBackDataHandler

    /// <summary>
    /// The load post data.
    /// </summary>
    /// <param name="postDataKey">
    /// The post data key.
    /// </param>
    /// <param name="postCollection">
    /// The post collection.
    /// </param>
    /// <returns>
    /// The load post data.
    /// </returns>
    bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
    {
      if (!string.IsNullOrEmpty(postCollection[this.HiddenKey]))
      {
        int index = Convert.ToInt16(postCollection[this.HiddenKey]);
        if (index == this.Value)
        {
          return false;
        }
        else
        {
          this.Value = index;
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// The raise post data changed event.
    /// </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent()
    {
      this.EnsureChildControls();
      if (this.ValueChanged != null)
      {
        this.ValueChanged(this, EventArgs.Empty);
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.Page.RegisterRequiresPostBack(this);
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
      this.Page.ClientScript.RegisterHiddenField(this.HiddenKey, this.Value.ToString());
      var builder = new JQueryScriptBuilder(this);
      builder.Prepare();
      builder.Build();
      builder.AppendBindFunction(
        "progressbarchange", 
        new[] { "event", "ui" }, 
        "$get('" + this.ClientID + "_value').value=jQuery(this).progressbar('option', 'value');");

      // StringBuilder scripts = new StringBuilder();
      // scripts.Append("jQuery('#" + ClientID + "').bind('progressbarchange',function(event,ui){");
      // scripts.Append("$get('" + ClientID + "_value').value=jQuery(this).progressbar('option', 'value');");
      // scripts.Append("});");
      // ClientScriptManager.RegisterJQueryControl(this);

      if (this.Resizable)
      {
        if (this.ShowPercentage)
        {
          builder.AppendSelector();
          builder.Scripts.Append(".resizable({alsoResize:'#" + this.ClientID + "_PercentagePanel" + "'});");
        }
          
          // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_resizable_load", "jQuery('#" + ClientID + "').resizable({alsoResize:'#" + ClientID + "_PercentagePanel" + "'});");
        else
        {
          builder.AppendSelector();
          builder.Scripts.Append(".resizable();");
        }

        // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_resizable_load", "jQuery('#" + ClientID + "').resizable();");
      }

      ClientScriptManager.RegisterJQueryControl(this, builder);

      // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_reload", scripts.ToString());
    }

    /// <summary>
    /// The render contents.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void RenderContents(HtmlTextWriter writer)
    {
      if (this.DesignMode)
      {
        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "ui-progressbar-value ui-widget-header ui-corner-left");
        writer.WriteAttribute("style", "width:" + this.Value + "%;");
        writer.Write(HtmlTextWriter.TagRightChar);
        writer.WriteEndTag("div");

        if (this.Resizable)
        {
          this.CssClass = "ui-progressbar ui-widget ui-widget-content ui-corner-all ui-widget-default ui-resizable";
          writer.WriteBeginTag("div");
          writer.WriteAttribute("class", "ui-resizable-handle ui-resizable-se ui-icon ui-icon-gripsmall-diagonal-se");
          writer.WriteAttribute("style", "z-index: 1001;");
          writer.Write(HtmlTextWriter.TagRightChar);
          writer.WriteEndTag("div");
        }
      }

      base.RenderContents(writer);
    }

    #endregion
  }
}