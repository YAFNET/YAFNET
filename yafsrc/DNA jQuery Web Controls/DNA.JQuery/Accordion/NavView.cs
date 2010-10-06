///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System.Web;
using System.Web.UI;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Web.UI.Design.WebControls;

namespace DNA.UI.JQuery
{
    [DefaultProperty("Items")]
    [ParseChildren(true, "Items")]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:NavView runat=\"server\" ID=\"NavView1\" Text=\"NavView\"></{0}:NavView>")]
    [Designer(typeof(CompositeControlDesigner))]
    [System.Drawing.ToolboxBitmap(typeof(NavView), "Accordion.NavView.ico")]
    public class NavView : View, IPostBackEventHandler
    {
        private NavItemCollection items;
        private bool showIcon=true;
        public event NavItemEventHandler NavItemClick;

        /// <summary>
        /// Gets/Sets the ViewControl whether can post back the event to server
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the NavView whether can post back the event to server")]
        [Bindable(true)]
        public bool AutoPostBack
        {
            get
            {
                object obj = ViewState["AutoPostBack"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AutoPostBack"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the NavItemCollection of the NavView
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("Gets/Sets the NavItemCollection of the NavView")]
        [NotifyParentProperty(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(Design.NavItemCollectionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionConverter))]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public NavItemCollection Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets/Sets when click the item which the window open to.
        /// </summary>
        [Category("Behavior")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Description("Gets/Sets when click the NavItem which the window open to")]
        [Bindable(true)]
        [TypeConverter(typeof(TargetConverter))]
        public new string Target { get; set; }

        /// <summary>
        /// Gets/Sets the item style class
        /// </summary>
        [Bindable(true)]
        [CssClassProperty]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the item style class")]
        public string ItemCssClass { get; set; }

        /// <summary>
        /// Gets/Sets the item style
        /// </summary>
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the item style")] 
        public string ItemStyle { get; set; }

        /// <summary>
        /// Gets/Sets whether NavItem can show icon
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the item style")]
        public bool ShowItemIcon { get { return showIcon; } set { showIcon = value; } }

        /// <summary>
        /// Gets/Sets the itemicon style class
        /// </summary>
        [Bindable(true)]
        [CssClassProperty]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the ItemIcon style class")]
        public string ItemIconClass { get; set; }

        /// <summary>
        /// Gets/Sets the item hover style class
        /// </summary>
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [Category("Appearance")]
        [CssClassProperty]
        [Description("Gets/Sets the item hover style class")]
        public string ItemHoverCssClass { get; set; }

        public NavView()
        {
            items = new NavItemCollection(this);
            ((IStateManager)Items).TrackViewState();
        }

        public void AddItem(string text)
        {
            Items.Add(new NavItem(text));
        }

        public void AddItem(string text, string navigateUrl)
        {
            Items.Add(new NavItem(text, navigateUrl));
        }

        public void AddItem(string text, string naviageUrl, string imageUrl)
        {
            Items.Add(new NavItem(text, naviageUrl, imageUrl));
        }

        protected void OnNavItemClick(NavItem item)
        {
            if (item != null)
            {
                if (NavItemClick != null)
                    NavItemClick(this, new NavItemEventArgs(item));
            }
        }

        protected override void CreateChildControls()
        {
            if (!string.IsNullOrEmpty(Target))
            {
                foreach (NavItem item in Items)
                {
                    if (string.IsNullOrEmpty(item.Target))
                        item.Target = Target;
                }
            }
            base.CreateChildControls();
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(CssClass))
                //CssClass = "dna-ui-navview";
                writer.AddAttribute("class","dna-ui-navview");
            base.RenderBeginTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("ul");
            writer.WriteAttribute("class", "ui-helper-reset");
            writer.Write(HtmlTextWriter.TagRightChar);

            foreach (NavItem item in Items)
            {
                writer.WriteBeginTag("li");

                if (!string.IsNullOrEmpty(ItemCssClass))
                    writer.WriteAttribute("class", ItemCssClass);

                if (!string.IsNullOrEmpty(ItemStyle))
                    writer.WriteAttribute("style", ItemStyle);

                if (!string.IsNullOrEmpty(ItemHoverCssClass))
                {
                    writer.WriteAttribute("onmouseover", "jQuery(this).addClass('" + ItemHoverCssClass + "');");
                    writer.WriteAttribute("onmouseout", "jQuery(this).removeClass('" + ItemHoverCssClass + "');");
                }

                writer.Write(HtmlTextWriter.TagRightChar);

                if (ShowItemIcon)
                {
                    writer.WriteBeginTag("span");
                    if (string.IsNullOrEmpty(ItemIconClass))
                        writer.WriteAttribute("class", "ui-icon ui-icon-triangle-1-e");
                    else
                        writer.WriteAttribute("class", ItemIconClass);
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.WriteEndTag("span");
                }

                writer.WriteBeginTag("a");
                writer.WriteAttribute("style", "position:relative;");
                //writer.WriteAttribute("style", "display:block;");
                if (!DesignMode)
                {
                    if (AutoPostBack)
                    {
                        string postBackEvent = Page.ClientScript.GetPostBackEventReference(this, item.Index.ToString());
                        writer.WriteAttribute("href", "javascript:" + postBackEvent + ";void(0);");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.NavigateUrl))
                            writer.WriteAttribute("href", Page.ResolveUrl(item.NavigateUrl));
                        else
                        {
                            if (!string.IsNullOrEmpty(item.OnClientClick))
                                writer.WriteAttribute("href", "javascript:" + item.OnClientClick + "void(0);");
                            else
                                writer.WriteAttribute("href", "#");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(item.Target))
                    writer.WriteAttribute("target", item.Target);
                writer.Write(HtmlTextWriter.TagRightChar);

                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    writer.Write("<img alt='' src='" + Page.ResolveUrl(item.ImageUrl) + "'/>");
                    writer.Write("<span style='margin-left:3px;position:absolute;top:6px;'>" + item.Text + "</span>");
                    //writer.Write("<table style='width:100%'><tr><td style='width:16px'>");
                    //writer.Write("<img alt='' src='" + Page.ResolveUrl(item.ImageUrl) + "'/></td><td>");
                   // writer.Write(item.Text + "</td></tr></table>");
                }
                else
                    writer.Write(item.Text);
                writer.WriteEndTag("a");
                writer.WriteEndTag("li");
            }
            writer.WriteEndTag("ul");
        }

        #region IPostBackEventHandler Members

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (!string.IsNullOrEmpty(eventArgument))
            {
                int i = int.Parse(eventArgument);
                OnNavItemClick(Items[i]);
            }
        }

        #endregion

        protected override void LoadViewState(object savedState)
        {
            object[] bag = savedState as object[];
            base.LoadViewState(bag[0]);
            if (Items != null)
            {
                Items.Clear();
                ((IStateManager)Items).LoadViewState(bag[1]);
            }
        }

        protected override object SaveViewState()
        {
            if (this.IsViewStateEnabled)
            {
                object[] bag = new object[2];
                bag[0] = base.SaveViewState();
                bag[1] = ((IStateManager)Items).SaveViewState();
                return bag;
            }
            else return null;
        }
    }
}
