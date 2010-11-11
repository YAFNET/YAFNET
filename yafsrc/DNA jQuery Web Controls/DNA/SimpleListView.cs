//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;
  using System.Collections;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  #endregion

  /// <summary>
  /// The simple list view.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ParseChildren(true, "Items")]
  [ToolboxData("<{0}:SimpleListView runat=\"server\" id=\"SimpleListView1\"></{0}:SimpleListView>")]
  // [Designer(typeof(CompositeControlDesigner))]
  public class SimpleListView : DataBoundControl, INamingContainer
  {
    #region Constants and Fields

    /// <summary>
    /// The css class field.
    /// </summary>
    private string cssClassField = string.Empty;

    /// <summary>
    /// The image url field.
    /// </summary>
    private string imageUrlField = string.Empty;

    /// <summary>
    /// The item css class.
    /// </summary>
    private string itemCssClass = string.Empty;

    /// <summary>
    /// The item template.
    /// </summary>
    private ITemplate itemTemplate;

    /// <summary>
    /// The items.
    /// </summary>
    private StateManagedObjectCollection<SimpleListItem> items;

    /// <summary>
    /// The navigate url field.
    /// </summary>
    private string navigateUrlField = string.Empty;

    /// <summary>
    /// The target.
    /// </summary>
    private string target = string.Empty;

    /// <summary>
    /// The target field.
    /// </summary>
    private string targetField = string.Empty;

    /// <summary>
    /// The text field.
    /// </summary>
    private string textField = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the CssClassField Name on DataBinding
    /// </summary>
    [Category("Data")]
    [Description("Gets/Sets the CssClass field Name on DataBinding")]
    public string DataCssClassField
    {
      get
      {
        return this.cssClassField;
      }

      set
      {
        this.cssClassField = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the ImageUrl field Name on DataBinding
    /// </summary>
    [Category("Data")]
    [Description("Gets/Sets the ImageUrl field Name on DataBinding")]
    public string DataImageUrlField
    {
      get
      {
        return this.imageUrlField;
      }

      set
      {
        this.imageUrlField = value;
      }
    }

    /// <summary>
    ///   Get/Sets the NavigateUrl field name on DataBinding
    /// </summary>
    [Category("Data")]
    [Description("Get/Sets the NavigateUrl field name on DataBinding")]
    public string DataNavigateUrlField
    {
      get
      {
        return this.navigateUrlField;
      }

      set
      {
        this.navigateUrlField = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Target field name on databinding
    /// </summary>
    [Category("Data")]
    [Description(" Gets/Sets the Target field name on databinding")]
    public string DataTargetField
    {
      get
      {
        return this.targetField;
      }

      set
      {
        this.targetField = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Text field name on DataBinding
    /// </summary>
    [Category("Data")]
    [Description("Gets/Sets the Text field name on DataBinding")]
    public string DataTextField
    {
      get
      {
        return this.textField;
      }

      set
      {
        this.textField = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the item style class name
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the item style class name")]
    [CssClassProperty]
    public string ItemCssClass
    {
      get
      {
        return this.itemCssClass;
      }

      set
      {
        this.itemCssClass = value;
      }
    }

    /// <summary>
    /// Gets or sets ItemTemplate.
    /// </summary>
    [Category("Layout")]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [TemplateInstance(TemplateInstance.Multiple)]
    [TemplateContainer(typeof(SimpleListItemContainer))]
    public ITemplate ItemTemplate
    {
      get
      {
        return this.itemTemplate;
      }

      set
      {
        this.itemTemplate = value;
      }
    }

    /// <summary>
    ///   Gets the Items collection instance of this view
    /// </summary>
    [Category("Data")]
    [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    public StateManagedObjectCollection<SimpleListItem> Items
    {
      get
      {
        if (this.items == null)
        {
          this.items = new StateManagedObjectCollection<SimpleListItem>();
          if (this.EnableViewState)
          {
            ((IStateManager)this.items).TrackViewState();
          }
        }

        return this.items;
      }
    }

    /// <summary>
    ///   Gets/Sets when click the item which the window open to
    /// </summary>
    [Category("Behavior")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Description("Gets/Sets when click the item which the window open to")]
    [Bindable(true)]
    [TypeConverter(typeof(TargetConverter))]
    public string Target
    {
      get
      {
        return this.target;
      }

      set
      {
        this.target = value;
      }
    }

    /// <summary>
    /// Gets TagKey.
    /// </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        if (this.ItemTemplate == null)
        {
          return HtmlTextWriterTag.Div;
        }
        else
        {
          return HtmlTextWriterTag.Ul;
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
      if (!string.IsNullOrEmpty(this.Target))
      {
        foreach (SimpleListItem item in this.Items)
        {
          if (string.IsNullOrEmpty(item.Target))
          {
            item.Target = this.Target;
          }
        }
      }

      if (this.ItemTemplate != null)
      {
        // System.Web.UI.HtmlControls.HtmlContainerControl ul = new System.Web.UI.HtmlControls.HtmlGenericControl("ul");
        // Controls.Add(ul);
        for (int i = 0; i < this.Items.Count; i++)
        {
          var itemContainer = new SimpleListItemContainer(this.Items[i], i);
          if (!string.IsNullOrEmpty(this.Items[i].CssClass))
          {
            itemContainer.CssClass = this.Items[i].CssClass;
          }
          else if (!string.IsNullOrEmpty(this.ItemCssClass))
          {
            itemContainer.CssClass = this.ItemCssClass;
          }

          this.Controls.Add(itemContainer);
          this.itemTemplate.InstantiateIn(itemContainer);
          itemContainer.DataBind();
        }
      }

      base.CreateChildControls();
    }

    /// <summary>
    /// The perform data binding.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    protected override void PerformDataBinding(IEnumerable data)
    {
      base.PerformDataBinding(data);
      if (data != null)
      {
        foreach (object dataItem in data)
        {
          var item = new SimpleListItem();
          PropertyDescriptorCollection props = TypeDescriptor.GetProperties(dataItem);
          item.Text = this.GetBindingStringValue(dataItem, props, this.DataTextField, "Text");
          item.ImageUrl = this.GetBindingStringValue(dataItem, props, this.DataImageUrlField, "ImageUrl");
          item.NavigateUrl = this.GetBindingStringValue(dataItem, props, this.DataNavigateUrlField, "NavigateUrl");
          item.CssClass = this.GetBindingStringValue(dataItem, props, this.DataCssClassField, "CssClass");
          item.Target = this.GetBindingStringValue(dataItem, props, this.DataTextField, "Target");
          this.Items.Add(item);
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
      if (this.ItemTemplate == null)
      {
        writer.WriteFullBeginTag("ul");

        foreach (SimpleListItem item in this.Items)
        {
          writer.WriteBeginTag("li");
          if (!string.IsNullOrEmpty(item.CssClass))
          {
            writer.WriteAttribute("class", item.CssClass);
          }
          else if (!string.IsNullOrEmpty(this.itemCssClass))
          {
            writer.WriteAttribute("class", this.itemCssClass);
          }

          writer.Write(HtmlTextWriter.TagRightChar);
          writer.WriteBeginTag("a");
          if (!string.IsNullOrEmpty(item.NavigateUrl))
          {
            writer.WriteAttribute("href", this.Page.ResolveUrl(item.NavigateUrl));
          }
          else
          {
            writer.WriteAttribute("href", "#");
          }

          if (!string.IsNullOrEmpty(item.Target))
          {
            writer.WriteAttribute("target", item.Target);
          }

          writer.Write(HtmlTextWriter.TagRightChar);
          if (!string.IsNullOrEmpty(item.ImageUrl))
          {
            writer.Write("<table style='width:100%'><tr><td style='width:16px'>");
            writer.Write("<img alt='' src='" + this.Page.ResolveUrl(item.ImageUrl) + "'/></td><td>");
            writer.Write(item.Text + "</td></tr></table>");
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
      else
      {
        base.RenderContents(writer);
      }
    }

    /// <summary>
    /// The get binding string value.
    /// </summary>
    /// <param name="dataItem">
    /// The data item.
    /// </param>
    /// <param name="props">
    /// The props.
    /// </param>
    /// <param name="dateFieldName">
    /// The date field name.
    /// </param>
    /// <param name="defaultFieldName">
    /// The default field name.
    /// </param>
    /// <returns>
    /// The get binding string value.
    /// </returns>
    private string GetBindingStringValue(
      object dataItem, PropertyDescriptorCollection props, string dateFieldName, string defaultFieldName)
    {
      if (!string.IsNullOrEmpty(dateFieldName))
      {
        return DataBinder.GetPropertyValue(dataItem, dateFieldName, null);
      }
      else
      {
        if (props.Count > 0)
        {
          if (null != props[defaultFieldName])
          {
            if (null != props[defaultFieldName].GetValue(dataItem))
            {
              return props[defaultFieldName].GetValue(dataItem).ToString();
            }
          }
        }
      }

      return String.Empty;
    }

    #endregion
  }
}