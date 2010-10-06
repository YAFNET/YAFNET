
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Security.Permissions;
using System.Drawing.Design;
using System.Web.UI.Design.WebControls;

namespace DNA.UI
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ParseChildren(true, "Items")]
    [ToolboxData("<{0}:SimpleListView runat=\"server\" id=\"SimpleListView1\"></{0}:SimpleListView>")]
    //[Designer(typeof(CompositeControlDesigner))]
    public class SimpleListView : DataBoundControl,INamingContainer
    {
        private StateManagedObjectCollection<SimpleListItem> items;
        private string target = "";
        private string itemCssClass = "";
        private ITemplate itemTemplate;
        private string textField = "";
        private string navigateUrlField = "";
        private string imageUrlField = "";
        private string cssClassField = "";
        private string targetField = "";

        /// <summary>
        /// Gets/Sets the Target field name on databinding
        /// </summary>
        [Category("Data")]
        [Description(" Gets/Sets the Target field name on databinding")]
        public string DataTargetField
        {
            get { return targetField; }
            set { targetField = value; }
        }

        /// <summary>
        /// Gets/Sets the CssClassField Name on DataBinding
        /// </summary>
        [Category("Data")]
        [Description("Gets/Sets the CssClass field Name on DataBinding")]
        public string DataCssClassField
        {
            get { return cssClassField; }
            set { cssClassField = value; }
        }

        /// <summary>
        /// Gets/Sets the ImageUrl field Name on DataBinding
        /// </summary>
        [Category("Data")]
        [Description("Gets/Sets the ImageUrl field Name on DataBinding")]
        public string DataImageUrlField
        {
            get { return imageUrlField; }
            set { imageUrlField = value; }
        }

        /// <summary>
        /// Get/Sets the NavigateUrl field name on DataBinding
        /// </summary>
        [Category("Data")]
        [Description("Get/Sets the NavigateUrl field name on DataBinding")]
        public string DataNavigateUrlField
        {
            get { return navigateUrlField; }
            set { navigateUrlField = value; }
        }

        /// <summary>
        /// Gets/Sets the Text field name on DataBinding
        /// </summary>
        [Category("Data")]
        [Description("Gets/Sets the Text field name on DataBinding")]
        public string DataTextField
        {
            get { return textField; }
            set { textField = value; }
        }

        [Category("Layout")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Multiple)]
        [TemplateContainer(typeof(SimpleListItemContainer))]
        public ITemplate ItemTemplate
        {
            get { return itemTemplate; }
            set { itemTemplate = value; }
        }

        /// <summary>
        /// Gets/Sets the item style class name
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the item style class name")]
        [CssClassProperty]
        public string ItemCssClass
        {
            get { return itemCssClass; }
            set { itemCssClass = value; }
        }

        /// <summary>
        /// Gets/Sets when click the item which the window open to
        /// </summary>
        [Category("Behavior")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Description("Gets/Sets when click the item which the window open to")]
        [Bindable(true)]
        [TypeConverter(typeof(TargetConverter))]
        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// Gets the Items collection instance of this view
        /// </summary>
        [Category("Data")]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public StateManagedObjectCollection<SimpleListItem> Items
        {
            get
            {
                if (items == null)
                {
                    items = new StateManagedObjectCollection<SimpleListItem>();
                    if (EnableViewState)
                        ((IStateManager)items).TrackViewState();
                }
                return items;
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                if (ItemTemplate == null)
                    return HtmlTextWriterTag.Div;
                else
                    return HtmlTextWriterTag.Ul;
            }
        }

        protected override void PerformDataBinding(System.Collections.IEnumerable data)
        {
            base.PerformDataBinding(data);
            if (data != null)
            {
                foreach (object dataItem in data)
                {
                    SimpleListItem item=new SimpleListItem();
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(dataItem);
                    item.Text = GetBindingStringValue(dataItem, props, DataTextField, "Text");
                    item.ImageUrl = GetBindingStringValue(dataItem, props, DataImageUrlField, "ImageUrl");
                    item.NavigateUrl = GetBindingStringValue(dataItem, props, DataNavigateUrlField, "NavigateUrl");
                    item.CssClass = GetBindingStringValue(dataItem, props, DataCssClassField, "CssClass");
                    item.Target = GetBindingStringValue(dataItem, props, DataTextField, "Target");
                    Items.Add(item);
                }
            }
        }

        private string GetBindingStringValue(object dataItem,PropertyDescriptorCollection props,string dateFieldName,string defaultFieldName)
        {
            if (!string.IsNullOrEmpty(dateFieldName))
                return DataBinder.GetPropertyValue(dataItem, dateFieldName, null);
            else
            {
                if (props.Count > 0)
                {
                    if (null != props[defaultFieldName])
                        if (null != props[defaultFieldName].GetValue(dataItem))
                            return  props[defaultFieldName].GetValue(dataItem).ToString();
                }
            }
            return String.Empty;
        }

        protected override void CreateChildControls()
        {
            if (!string.IsNullOrEmpty(Target))
            {
                foreach (SimpleListItem item in Items)
                {
                    if (string.IsNullOrEmpty(item.Target))
                        item.Target = Target;
                }
            }

            if (ItemTemplate != null)
            {
                //System.Web.UI.HtmlControls.HtmlContainerControl ul = new System.Web.UI.HtmlControls.HtmlGenericControl("ul");
                //Controls.Add(ul);
                for (int i = 0; i < Items.Count; i++)
                {
                    SimpleListItemContainer itemContainer = new SimpleListItemContainer(Items[i], i);
                    if (!string.IsNullOrEmpty(Items[i].CssClass))
                        itemContainer.CssClass = Items[i].CssClass;
                    else
                        if (!string.IsNullOrEmpty(ItemCssClass))
                            itemContainer.CssClass = ItemCssClass;
                    Controls.Add(itemContainer);
                    itemTemplate.InstantiateIn(itemContainer);
                    itemContainer.DataBind();
                }
            }
            base.CreateChildControls();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (ItemTemplate == null)
            {
                writer.WriteFullBeginTag("ul");

                foreach (SimpleListItem item in Items)
                {

                    writer.WriteBeginTag("li");
                    if (!string.IsNullOrEmpty(item.CssClass))
                        writer.WriteAttribute("class", item.CssClass);
                    else
                        if (!string.IsNullOrEmpty(itemCssClass))
                            writer.WriteAttribute("class", itemCssClass);

                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.WriteBeginTag("a");
                    if (!string.IsNullOrEmpty(item.NavigateUrl))
                        writer.WriteAttribute("href", Page.ResolveUrl(item.NavigateUrl));
                    else
                        writer.WriteAttribute("href", "#");

                    if (!string.IsNullOrEmpty(item.Target))
                        writer.WriteAttribute("target", item.Target);
                    writer.Write(HtmlTextWriter.TagRightChar);
                    if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        writer.Write("<table style='width:100%'><tr><td style='width:16px'>");
                        writer.Write("<img alt='' src='" + Page.ResolveUrl(item.ImageUrl) + "'/></td><td>");
                        writer.Write(item.Text + "</td></tr></table>");
                    }
                    else
                        writer.Write(item.Text);

                    writer.WriteEndTag("a");
                    writer.WriteEndTag("li");
                }
                writer.WriteEndTag("ul");
            }
            else
                base.RenderContents(writer);
        }
    }
}
