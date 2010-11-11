//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.ComponentModel;
  using System.Drawing;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.Design.WebControls;
  using System.Web.UI.WebControls;

  using DNA.UI.JQuery.Design;

  #endregion

  /// <summary>
  /// The nav view.
  /// </summary>
  [DefaultProperty("Items")]
  [ParseChildren(true, "Items")]
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:NavView runat=\"server\" ID=\"NavView1\" Text=\"NavView\"></{0}:NavView>")]
  [Designer(typeof(CompositeControlDesigner))]
  [ToolboxBitmap(typeof(NavView), "Accordion.NavView.ico")]
  public class NavView : View, IPostBackEventHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The items.
    /// </summary>
    private readonly NavItemCollection items;

    /// <summary>
    /// The show icon.
    /// </summary>
    private bool showIcon = true;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NavView"/> class.
    /// </summary>
    public NavView()
    {
      this.items = new NavItemCollection(this);
      ((IStateManager)this.Items).TrackViewState();
    }

    #endregion

    #region Events

    /// <summary>
    /// The nav item click.
    /// </summary>
    public event NavItemEventHandler NavItemClick;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the ViewControl whether can post back the event to server
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the NavView whether can post back the event to server")]
    [Bindable(true)]
    public bool AutoPostBack
    {
      get
      {
        object obj = this.ViewState["AutoPostBack"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["AutoPostBack"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the item style class
    /// </summary>
    [Bindable(true)]
    [CssClassProperty]
    [NotifyParentProperty(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the item style class")]
    public string ItemCssClass { get; set; }

    /// <summary>
    ///   Gets/Sets the item hover style class
    /// </summary>
    [Bindable(true)]
    [NotifyParentProperty(true)]
    [Category("Appearance")]
    [CssClassProperty]
    [Description("Gets/Sets the item hover style class")]
    public string ItemHoverCssClass { get; set; }

    /// <summary>
    ///   Gets/Sets the itemicon style class
    /// </summary>
    [Bindable(true)]
    [CssClassProperty]
    [NotifyParentProperty(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the ItemIcon style class")]
    public string ItemIconClass { get; set; }

    /// <summary>
    ///   Gets/Sets the item style
    /// </summary>
    [Bindable(true)]
    [NotifyParentProperty(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the item style")]
    public string ItemStyle { get; set; }

    /// <summary>
    ///   Gets/Sets the NavItemCollection of the NavView
    /// </summary>
    [Browsable(true)]
    [Category("Data")]
    [Description("Gets/Sets the NavItemCollection of the NavView")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Editor(typeof(NavItemCollectionEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(CollectionConverter))]
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    public NavItemCollection Items
    {
      get
      {
        return this.items;
      }
    }

    /// <summary>
    ///   Gets/Sets whether NavItem can show icon
    /// </summary>
    [Bindable(true)]
    [DefaultValue(true)]
    [NotifyParentProperty(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the item style")]
    public bool ShowItemIcon
    {
      get
      {
        return this.showIcon;
      }

      set
      {
        this.showIcon = value;
      }
    }

    /// <summary>
    ///   Gets/Sets when click the item which the window open to.
    /// </summary>
    [Category("Behavior")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Description("Gets/Sets when click the NavItem which the window open to")]
    [Bindable(true)]
    [TypeConverter(typeof(TargetConverter))]
    public string Target { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add item.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public void AddItem(string text)
    {
      this.Items.Add(new NavItem(text));
    }

    /// <summary>
    /// The add item.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="navigateUrl">
    /// The navigate url.
    /// </param>
    public void AddItem(string text, string navigateUrl)
    {
      this.Items.Add(new NavItem(text, navigateUrl));
    }

    /// <summary>
    /// The add item.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="naviageUrl">
    /// The naviage url.
    /// </param>
    /// <param name="imageUrl">
    /// The image url.
    /// </param>
    public void AddItem(string text, string naviageUrl, string imageUrl)
    {
      this.Items.Add(new NavItem(text, naviageUrl, imageUrl));
    }

    /// <summary>
    /// The render begin tag.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public override void RenderBeginTag(HtmlTextWriter writer)
    {
      if (string.IsNullOrEmpty(this.CssClass))
      {
        // CssClass = "dna-ui-navview";
        writer.AddAttribute("class", "dna-ui-navview");
      }

      base.RenderBeginTag(writer);
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
      if (!string.IsNullOrEmpty(eventArgument))
      {
        int i = int.Parse(eventArgument);
        this.OnNavItemClick(this.Items[i]);
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
      if (!string.IsNullOrEmpty(this.Target))
      {
        foreach (NavItem item in this.Items)
        {
          if (string.IsNullOrEmpty(item.Target))
          {
            item.Target = this.Target;
          }
        }
      }

      base.CreateChildControls();
    }

    /// <summary>
    /// The load view state.
    /// </summary>
    /// <param name="savedState">
    /// The saved state.
    /// </param>
    protected override void LoadViewState(object savedState)
    {
      var bag = savedState as object[];
      base.LoadViewState(bag[0]);
      if (this.Items != null)
      {
        this.Items.Clear();
        ((IStateManager)this.Items).LoadViewState(bag[1]);
      }
    }

    /// <summary>
    /// The on nav item click.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    protected void OnNavItemClick(NavItem item)
    {
      if (item != null)
      {
        if (this.NavItemClick != null)
        {
          this.NavItemClick(this, new NavItemEventArgs(item));
        }
      }
    }

    /// <summary>
    /// The render contents.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void RenderContents(HtmlTextWriter writer)
    {
      writer.WriteBeginTag("ul");
      writer.WriteAttribute("class", "ui-helper-reset");
      writer.Write(HtmlTextWriter.TagRightChar);

      foreach (NavItem item in this.Items)
      {
        writer.WriteBeginTag("li");

        if (!string.IsNullOrEmpty(this.ItemCssClass))
        {
          writer.WriteAttribute("class", this.ItemCssClass);
        }

        if (!string.IsNullOrEmpty(this.ItemStyle))
        {
          writer.WriteAttribute("style", this.ItemStyle);
        }

        if (!string.IsNullOrEmpty(this.ItemHoverCssClass))
        {
          writer.WriteAttribute("onmouseover", "jQuery(this).addClass('" + this.ItemHoverCssClass + "');");
          writer.WriteAttribute("onmouseout", "jQuery(this).removeClass('" + this.ItemHoverCssClass + "');");
        }

        writer.Write(HtmlTextWriter.TagRightChar);

        if (this.ShowItemIcon)
        {
          writer.WriteBeginTag("span");
          if (string.IsNullOrEmpty(this.ItemIconClass))
          {
            writer.WriteAttribute("class", "ui-icon ui-icon-triangle-1-e");
          }
          else
          {
            writer.WriteAttribute("class", this.ItemIconClass);
          }

          writer.Write(HtmlTextWriter.TagRightChar);
          writer.WriteEndTag("span");
        }

        writer.WriteBeginTag("a");
        writer.WriteAttribute("style", "position:relative;");

        // writer.WriteAttribute("style", "display:block;");
        if (!this.DesignMode)
        {
          if (this.AutoPostBack)
          {
            string postBackEvent = this.Page.ClientScript.GetPostBackEventReference(this, item.Index.ToString());
            writer.WriteAttribute("href", "javascript:" + postBackEvent + ";void(0);");
          }
          else
          {
            if (!string.IsNullOrEmpty(item.NavigateUrl))
            {
              writer.WriteAttribute("href", this.Page.ResolveUrl(item.NavigateUrl));
            }
            else
            {
              if (!string.IsNullOrEmpty(item.OnClientClick))
              {
                writer.WriteAttribute("href", "javascript:" + item.OnClientClick + "void(0);");
              }
              else
              {
                writer.WriteAttribute("href", "#");
              }
            }
          }
        }

        if (!string.IsNullOrEmpty(item.Target))
        {
          writer.WriteAttribute("target", item.Target);
        }

        writer.Write(HtmlTextWriter.TagRightChar);

        if (!string.IsNullOrEmpty(item.ImageUrl))
        {
          writer.Write("<img alt='' src='" + this.Page.ResolveUrl(item.ImageUrl) + "'/>");
          writer.Write("<span style='margin-left:3px;position:absolute;top:6px;'>" + item.Text + "</span>");

          // writer.Write("<table style='width:100%'><tr><td style='width:16px'>");
          // writer.Write("<img alt='' src='" + Page.ResolveUrl(item.ImageUrl) + "'/></td><td>");
          // writer.Write(item.Text + "</td></tr></table>");
        }
        else
        {
          writer.Write(item.Text);
        }

        writer.WriteEndTag("a");
        writer.WriteEndTag("li");
      }

      writer.WriteEndTag("ul");
    }

    /// <summary>
    /// The save view state.
    /// </summary>
    /// <returns>
    /// The save view state.
    /// </returns>
    protected override object SaveViewState()
    {
      if (this.IsViewStateEnabled)
      {
        var bag = new object[2];
        bag[0] = base.SaveViewState();
        bag[1] = ((IStateManager)this.Items).SaveViewState();
        return bag;
      }
      else
      {
        return null;
      }
    }

    #endregion
  }
}