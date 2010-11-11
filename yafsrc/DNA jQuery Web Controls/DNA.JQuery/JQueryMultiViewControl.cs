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
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// The MultiView Base Class
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  // [ParseChildren(true)]
  public abstract class JQueryMultiViewControl : CompositeControl, IPostBackDataHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The new index.
    /// </summary>
    protected int NewIndex = -1;

    /// <summary>
    /// The selected index.
    /// </summary>
    private int selectedIndex;

    /// <summary>
    /// The views.
    /// </summary>
    private ViewCollection views;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the ViewControl whether can post back the event to server
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the ViewControl whether can post back the event to server")]
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
    ///   Gets/Sets the view content style class
    /// </summary>
    [Description(" Gets/Sets the view content style class")]
    [Category("Appearance")]
    [Bindable(true)]
    [Browsable(true)]
    [Themeable(true)]
    [CssClassProperty]
    public string ContentCssClass
    {
      get
      {
        object obj = this.ViewState["ContentCssClass"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["ContentCssClass"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the content style
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the content style")]
    [Bindable(true)]
    [Browsable(true)]
    [Themeable(true)]
    public string ContentStyle
    {
      get
      {
        object obj = this.ViewState["ContentStyle"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["ContentStyle"] = value;
      }
    }

    /// <summary>
    ///   Ges/Sets the header style class
    /// </summary>
    [Category("Appearance")]
    [Description("Ges/Sets the header style class")]
    [Bindable(true)]
    [Browsable(true)]
    [Themeable(true)]
    [CssClassProperty]
    public string HeaderCssClass
    {
      get
      {
        object obj = this.ViewState["HeaderCssClass"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["HeaderCssClass"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the header style for views
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the header style for views")]
    [Bindable(true)]
    [Browsable(true)]
    [Themeable(true)]
    public string HeaderStyle
    {
      get
      {
        object obj = this.ViewState["HeaderStyle"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["HeaderStyle"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the active view control's index
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the active view control's index")]
    [Browsable(true)]
    [Bindable(true)]
    [NotifyParentProperty(true)]
    public virtual int SelectedIndex
    {
      get
      {
        return this.selectedIndex;
      }

      set
      {
        this.EnsureChildControls();
        this.OnActiveViewChanged();

        if (value != this.selectedIndex)
        {
          View deactiveView = this.Views[this.SelectedIndex];
          if (deactiveView != null)
          {
            deactiveView.OnDeactive();
          }
        }

        View activeView = this.Views[value];
        if (activeView != null)
        {
          activeView.OnActive();
        }

        this.selectedIndex = value;
      }
    }

    /// <summary>
    ///   Gets the View Colleciton of the jQueryMultiViewControl
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Browsable(false)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(CollectionConverter))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ViewCollection Views
    {
      get
      {
        if (this.views == null)
        {
          this.views = new ViewCollection(this);
        }

        return this.views;
      }
    }

    /// <summary>
    /// Gets HiddenKey.
    /// </summary>
    protected string HiddenKey
    {
      get
      {
        return this.ClientID + "_selectedID";
      }
    }

    /// <summary>
    /// Gets TagKey.
    /// </summary>
    [Browsable(false)]
    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        return HtmlTextWriterTag.Div;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The apply view style.
    /// </summary>
    public void ApplyViewStyle()
    {
      foreach (View view in this.Views)
      {
        if (!string.IsNullOrEmpty(this.HeaderStyle))
        {
          if (string.IsNullOrEmpty(view.HeaderStyle))
          {
            view.HeaderStyle = this.HeaderStyle;
          }
        }

        if (!string.IsNullOrEmpty(this.ContentStyle))
        {
          view.Style.Value = this.ContentStyle;
        }

        if (string.IsNullOrEmpty(view.CssClass))
        {
          view.CssClass = this.ContentCssClass;
        }

        if (!string.IsNullOrEmpty(this.HeaderCssClass))
        {
          view.HeaderCssClass = this.HeaderCssClass;
        }
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
      return this.LoadPostData(postDataKey, postCollection);
    }

    /// <summary>
    /// The raise post data changed event.
    /// </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent()
    {
      this.RaisePostDataChangedEvent();
    }

    #endregion

    #endregion

    #region Methods

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
    protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
    {
      return true;
    }

    /// <summary>
    /// The on active view changed.
    /// </summary>
    protected virtual void OnActiveViewChanged()
    {
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      if (this.Views.Count > 0)
      {
        if (!this.Page.IsPostBack)
        {
          if (this.SelectedIndex == 0)
          {
            this.SelectedIndex = 0;
          }
        }
      }

      this.ApplyViewStyle();
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
      ClientScriptManager.RegisterJQueryControl(this);
      base.OnPreRender(e);
    }

    /// <summary>
    /// The raise post data changed event.
    /// </summary>
    protected virtual void RaisePostDataChangedEvent()
    {
      this.EnsureChildControls();
      this.SelectedIndex = this.NewIndex;
    }

    #endregion
  }
}