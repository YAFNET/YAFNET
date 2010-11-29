namespace YAF.Controls
{
  
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes.Core;
    using YAF.Classes.Utils;

  /// <summary>
  /// Rendered DIV container
  /// </summary>
  public class Container : Control, INamingContainer
  {
    /// <summary>
    /// The _content template.
    /// </summary>
    private ITemplate _contentTemplate;

    /// <summary>
    /// The _css class.
    /// </summary>
    private string _cssClass;

    /// <summary>
    /// The _hide text.
    /// </summary>
    private string _hideText;

    /// <summary>
    /// The _rounded corners.
    /// </summary>
    private bool _roundedCorners;

    /// <summary>
    /// The _show text.
    /// </summary>
    private string _showText;

    /// <summary>
    /// The _title.
    /// </summary>
    private string _title;

    #region Private strings

    /// <summary>
    /// Client ID for Contents DIV - Required for Javascript
    /// </summary>
    private string ContentsClientID
    {
      get
      {
        return ClientID + "_content";
      }
    }

    /// <summary>
    /// Client ID for HTML Link - Required for Javascript
    /// </summary>
    private string LinkClientID
    {
      get
      {
        return ClientID + "_expandLink";
      }
    }

    /// <summary>
    /// Summary description for ForumJump.
    /// </summary>
    /// <returns>
    /// The expand link.
    /// </returns>
    private string ExpandLink()
    {
      string jsHref = "javascript:toggleContainer('{0}', '{1}', '{2}', '{3}');".FormatWith(this.ContentsClientID, this.LinkClientID, this.ShowText, this.HideText);
      string link = "<a id=\"{0}\" href=\"{1}\">{2}</a>".FormatWith(this.LinkClientID, jsHref, this.ShowText);
      return link;
    }

    #endregion

    #region Render Methods

    /// <summary>
    /// Renders surrounding div
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      if (!Visible)
      {
        return;
      }

      writer.BeginRender();

      writer.WriteBeginTag("div");
      writer.WriteAttribute("id", ClientID);
      if (this.CSSClass.IsSet())
      {
        // Make sure CSS Class is not empty before rendering attribute
        writer.WriteAttribute("class", CSSClass);
      }

      writer.Write(">");
      writer.WriteLine();

      if (RoundedCorners)
      {
        RenderRoundedCorners(writer); // Render additional DIVs for Sliding doors technique
      }
      else
      {
        RenderContents(writer); // Render contents div and Contents
      }

      writer.WriteLine();
      writer.WriteEndTag("div");
      writer.WriteLine();

      writer.EndRender();
    }

    /// <summary>
    /// Renders surrounding DIV for Sliding Doors Rounded Corners technique
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderRoundedCorners(HtmlTextWriter writer)
    {
      // Write Begining div
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "roundedHeader");
      writer.Write(">");
      writer.Write("<div class=\"rightCorner\"></div>");
      writer.WriteEndTag("div");

      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "roundedContents");
      writer.Write(">");
      RenderContents(writer); // Render all Content Controls
      writer.WriteEndTag("div");

      // Write End div
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "roundedFooter");
      writer.Write(">");
      writer.Write("<div class=\"rightCorner\"></div>");
      writer.WriteEndTag("div");
    }

    /// <summary>
    /// Renders contents div and childcontrols
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderContents(HtmlTextWriter writer)
    {
      // Expandable DIV
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "expandablePanel");
      writer.Write(">");
      writer.WriteLine();

      // Container DIV
      writer.WriteBeginTag("div");
      writer.WriteAttribute("id", ContentsClientID);
      writer.WriteAttribute("class", "contents");
      writer.Write(">");
      writer.WriteLine();

      if (this.Title.IsSet())
      {
        writer.WriteFullBeginTag("h2");
        writer.Write(Title);
        writer.WriteEndTag("h2");
        writer.WriteLine();
      }

      base.Render(writer);

      writer.WriteEndTag("div");
      writer.WriteLine();

      // Footer DIV
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "footer");
      writer.Write(">");
      writer.WriteLine();
      writer.WriteLine(ExpandLink()); // Render Show/Hide
      writer.WriteLine();
      writer.WriteEndTag("div");
      writer.WriteLine();

      // end expandable div
      writer.WriteEndTag("div");
      writer.WriteLine();
    }

    #endregion

    #region Overrides

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
    }

    protected override void OnPreRender(EventArgs e)
    {
        YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
        base.OnPreRender(e);
    }

    /// <summary>
    /// The data bind.
    /// </summary>
    public override void DataBind()
    {
      CreateChildControls();
      ChildControlsCreated = true;
      base.DataBind();
    }

    /// <summary>
    /// The create child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
      // Render all Contents in the Contents Template
      var templateControl = new PlaceHolder();
      if (Contents != null)
      {
        Contents.InstantiateIn(templateControl);
      }

      Controls.Add(templateControl);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Contents to render within the container
    /// </summary>
    [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof (PlaceHolder)), DefaultValue(typeof (ITemplate), ""), 
     Description("Contents")]
    public ITemplate Contents
    {
      get
      {
        return this._contentTemplate;
      }

      set
      {
        this._contentTemplate = value;
      }
    }

    /// <summary>
    /// Css Clas for surrounding DIV
    /// </summary>
    /// 
    [Browsable(true), PersistenceMode(PersistenceMode.Attribute), Description("CssClass")]
    public string CSSClass
    {
      get
      {
        return this._cssClass;
      }

      set
      {
        this._cssClass = value;
      }
    }

    /// <summary>
    /// Hide Text required for expanding/collapsing container
    /// </summary>
    [Browsable(true), PersistenceMode(PersistenceMode.Attribute), Description("ShowText")]
    public string ShowText
    {
      get
      {
        return this._showText;
      }

      set
      {
        this._showText = value;
      }
    }

    /// <summary>
    /// Hide Text required for expanding/collapsing container
    /// </summary>
    [Browsable(true), PersistenceMode(PersistenceMode.Attribute), Description("HideText")]
    public string HideText
    {
      get
      {
        return this._hideText;
      }

      set
      {
        this._hideText = value;
      }
    }

    /// <summary>
    /// If present renders a h2 html tag
    /// </summary>
    [Browsable(true), PersistenceMode(PersistenceMode.Attribute), Description("Title")]
    public string Title
    {
      get
      {
        return this._title;
      }

      set
      {
        this._title = value;
      }
    }

    /// <summary>
    /// If true, renders SLIDING doors technique additional DIV tags
    /// </summary>
    [Browsable(true), PersistenceMode(PersistenceMode.Attribute), Description("RoundedCorners")]
    public bool RoundedCorners
    {
      get
      {
        return this._roundedCorners;
      }

      set
      {
        this._roundedCorners = value;
      }
    }

    #endregion
  }
}