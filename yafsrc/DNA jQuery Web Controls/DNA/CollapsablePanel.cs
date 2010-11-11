//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  #endregion

  /// <summary>
  /// CollapsablePanel is a Panel Control can be collapse and expand.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ParseChildren(true, "ContentTempalte")]
  [ToolboxData(
    "<{0}:CollapsablePanel runat=\"server\" id=\"CollapsablePanel1\"><ContentTempalte></ContentTempalte></{0}:CollapsablePanel>"
    )]
  public class CollapsablePanel : CompositeControl, IPostBackDataHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The body panel.
    /// </summary>
    private Panel bodyPanel;

    /// <summary>
    /// The collapsed icon css class.
    /// </summary>
    private string collapsedIconCssClass = string.Empty;

    /// <summary>
    /// The content css class.
    /// </summary>
    private string contentCssClass = "ui-widget-content ui-corner-bottom";

    /// <summary>
    /// The content tempalte.
    /// </summary>
    private ITemplate contentTempalte;

    /// <summary>
    /// The expand icon css class.
    /// </summary>
    private string expandIconCssClass = string.Empty;

    /// <summary>
    /// The header panel.
    /// </summary>
    private HtmlGenericControl headerPanel;

    /// <summary>
    /// The header style.
    /// </summary>
    private Style headerStyle;

    /// <summary>
    /// The is collapsed.
    /// </summary>
    private bool isCollapsed;

    /// <summary>
    /// The title.
    /// </summary>
    private string title = string.Empty;

    /// <summary>
    /// The using default style.
    /// </summary>
    private bool usingDefaultStyle = true;

    #endregion

    #region Events

    /// <summary>
    /// The collapse.
    /// </summary>
    public event EventHandler Collapse;

    /// <summary>
    /// The expand.
    /// </summary>
    public event EventHandler Expand;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets CollapsedIconCssClass.
    /// </summary>
    [Category("Appearance")]
    [CssClassProperty]
    public string CollapsedIconCssClass
    {
      get
      {
        return this.collapsedIconCssClass;
      }

      set
      {
        this.collapsedIconCssClass = value;
      }
    }

    /// <summary>
    /// Gets or sets ContentCssClass.
    /// </summary>
    [CssClassProperty]
    public string ContentCssClass
    {
      get
      {
        return this.contentCssClass;
      }

      set
      {
        this.contentCssClass = value;
      }
    }

    /// <summary>
    /// Gets or sets ContentTempalte.
    /// </summary>
    [Browsable(false)]
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    [TemplateInstance(TemplateInstance.Single)]
    [TemplateContainer(typeof(CollapsablePanel))]
    public ITemplate ContentTempalte
    {
      get
      {
        return this.contentTempalte;
      }

      set
      {
        this.contentTempalte = value;
      }
    }

    /// <summary>
    /// Gets or sets ExpandIconCssClass.
    /// </summary>
    [Category("Appearance")]
    [CssClassProperty]
    public string ExpandIconCssClass
    {
      get
      {
        return this.expandIconCssClass;
      }

      set
      {
        this.expandIconCssClass = value;
      }
    }

    /// <summary>
    /// Gets or sets HeaderStyle.
    /// </summary>
    [Category("Appearance")]
    public Style HeaderStyle
    {
      get
      {
        return this.headerStyle;
      }

      set
      {
        this.headerStyle = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsCollapsed.
    /// </summary>
    public bool IsCollapsed
    {
      get
      {
        return this.isCollapsed;
      }

      set
      {
        this.isCollapsed = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Header Title text
    /// </summary>
    [Category("Appearance")]
    public string Title
    {
      get
      {
        return this.title;
      }

      set
      {
        this.title = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether using the default styles
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets whether using the default styles")]
    public bool UsingDefaultStyle
    {
      get
      {
        return this.usingDefaultStyle;
      }

      set
      {
        this.usingDefaultStyle = value;
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
        return this.ClientID + "_Hidden";
      }
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
      this.EnsureChildControls();
      bool ic = false;
      if (bool.TryParse(postCollection[this.HiddenKey], out ic))
      {
        this.IsCollapsed = ic;
        return true;
      }

      return false;
    }

    /// <summary>
    /// The raise post data changed event.
    /// </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent()
    {
      if (this.IsCollapsed)
      {
        if (this.Collapse != null)
        {
          this.Collapse(this, EventArgs.Empty);
        }
      }
      else
      {
        if (this.Expand != null)
        {
          this.Expand(this, EventArgs.Empty);
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
      this.headerPanel = new HtmlGenericControl("h3");
      this.bodyPanel = new Panel();
      this.bodyPanel.ID = "Body";
      this.headerPanel.ID = "Header";
      this.Controls.Add(this.headerPanel);
      this.Controls.Add(this.bodyPanel);
      if (this.contentTempalte != null)
      {
        this.contentTempalte.InstantiateIn(this.bodyPanel);
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.Page.RegisterRequiresPostBack(this);
      if (!this.DesignMode)
      {
        if (this.Context.Request.Browser.Browser == "IE")
        {
          if (this.Context.Request.Browser.MajorVersion < 8)
          {
            ClientScriptManager.AddCompositeScript(this, "jQueryNet.plugins.bgiframe.js", "jQueryNet");
            ClientScriptManager.RegisterDocumentReadyScript(this, "$('#" + this.ClientID + "').bgiframe();");
          }
        }
      }

      base.OnInit(e);
    }

    /// <summary>
    /// The render contents.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void RenderContents(HtmlTextWriter writer)
    {
      this.Page.ClientScript.RegisterHiddenField(this.HiddenKey, this.IsCollapsed.ToString());
      if (this.isCollapsed)
      {
        this.headerPanel.Attributes.Add("class", "ui-widget-header ui-state-active ui-corner-all");
      }
      else
      {
        this.headerPanel.Attributes.Add("class", "ui-widget-header ui-state-active ui-corner-top");
      }

      this.headerPanel.Style.Add(HtmlTextWriterStyle.Position, "relative");
      this.headerPanel.Style.Add(HtmlTextWriterStyle.Height, "25px");
      this.CollapsedIconCssClass = "ui-icon-triangle-1-e";
      this.ExpandIconCssClass = "ui-icon-triangle-1-s";
      this.bodyPanel.CssClass = this.contentCssClass;

      var span = new HtmlGenericControl("span");
      span.Style.Add(HtmlTextWriterStyle.Position, "absolute");
      span.Style.Add(HtmlTextWriterStyle.Top, "50%");
      span.Style.Add(HtmlTextWriterStyle.Left, "0.5em");
      span.Style.Add(HtmlTextWriterStyle.MarginTop, "-8px");
      if (this.isCollapsed)
      {
        span.Attributes.Add("class", "ui-icon " + this.CollapsedIconCssClass);
        this.bodyPanel.Style[HtmlTextWriterStyle.Display] = "none";
      }
      else
      {
        span.Attributes.Add("class", "ui-icon " + this.ExpandIconCssClass);
        this.bodyPanel.Style[HtmlTextWriterStyle.Display] = "block";
      }

      var scripts = new StringBuilder();
      scripts.Append("$(this).find('span').toggleClass('" + this.CollapsedIconCssClass + "');");
      scripts.Append("$(this).find('span').toggleClass('" + this.ExpandIconCssClass + "');");
      scripts.Append("$(this).toggleClass('ui-corner-all');");
      scripts.Append("$(this).toggleClass('ui-corner-top');");
      scripts.Append("$('#" + this.bodyPanel.ClientID + "').slideToggle('fast');");
      scripts.Append("if ($('#" + this.HiddenKey + "').val()=='True') ");
      scripts.Append("$('#" + this.HiddenKey + "').val('False'); else ");
      scripts.Append("$('#" + this.HiddenKey + "').val('True');");
      this.headerPanel.Attributes.Add("onclick", scripts.ToString());
      var label = new HyperLink();
      label.NavigateUrl = "#";
      label.Text = this.title;
      label.Style.Add(HtmlTextWriterStyle.Padding, "4px 0.5em 0.5em 30px");
      label.Font.Size = FontUnit.Parse("10pt");
      label.Style.Add(HtmlTextWriterStyle.Display, "block");

      this.headerPanel.Controls.Add(span);
      this.headerPanel.Controls.Add(label);
      base.RenderContents(writer);
    }

    #endregion
  }
}