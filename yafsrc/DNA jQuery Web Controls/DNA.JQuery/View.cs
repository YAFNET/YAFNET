///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;

///<notes> Updates Notes:
/// Date:2009/05/17
///  1.Inhert from the CompositeControl not from Control to support the designer
///  2.Remove "ContentStyle" and "ContentCss" use "Style" and "CssClass" from CompositeControl
///  3.Remove Render Override , RenderContent
///  3.Override RenderBeginTag
///</notes>

namespace DNA.UI.JQuery
{
    /// <summary>
    ///  View Control is a Container Control can be use inside Accordion or Tab Control.
    /// </summary>
    [PersistChildren(false)]
    [ParseChildren(false)]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:View runat=\"server\" ID=\"View1\" Text=\"View\"></{0}:View>")]
    [System.Drawing.ToolboxBitmap(typeof(View), "View.ico")]
    [Designer(typeof(Design.ViewDesigner))]
    public class View : CompositeControl
    {
        public event EventHandler Active;
        public event EventHandler Deactive;
        //private bool enabled = true;
        private bool showHeader = true;

        /// <summary>
        /// Gets the View 's Container Control
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public JQueryMultiViewControl ParentContainer { get; internal set; }

        /// <summary>
        /// Gets whether the view is selected
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [NotifyParentProperty(true)]
        public bool IsSelected
        {
            get
            {
                return Index == ParentContainer.SelectedIndex;
            }
        }

        /// <summary>
        /// Gets/Sets the view header css style string.The ShowHeader Property must be set to true.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the the view header css style string")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [Themeable(true)]
        public string HeaderStyle { get; set; }

        /// <summary>
        /// Gets/Sets the View Header's css class
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the View Header's css class")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Bindable(true)]
        [Themeable(true)]
        [CssClassProperty]
        [NotifyParentProperty(true)]
        public string HeaderCssClass { get; set; }

        internal int Index { get; set; }

        /// <summary>
        /// Gets/Sets if the view show's it's header
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets if the view show's it's header")]
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [Themeable(true)]
        public bool ShowHeader
        {
            get { return showHeader; }
            set { showHeader = value; }
        }

        /// <summary>
        /// Gets/Sets the view's header title text
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the view's header title text")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Localizable(true)]
        [Bindable(true)]
        [NotifyParentProperty(true)]
        public string Text { get; set; }

        /// <summary>
        /// Gets/Sets when set the view header naviage url
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets when set the view header naviage url")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [UrlProperty]
        [Editor(typeof(UrlEditor), typeof(UITypeEditor))]
        public string NavigateUrl { get; set; }

        /// <summary>
        /// Gets/Sets when click the view header which the window open to
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets when click the view header which the window open to")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [Bindable(true)]
        [TypeConverter(typeof(TargetConverter))]
        public string Target { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
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

        [Browsable(false)]
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        internal void OnActive()
        {
            if (Active != null)
                Active(this, EventArgs.Empty);
        }

        internal void OnDeactive()
        {
            if (Deactive != null)
                Deactive(this, EventArgs.Empty);
        }

        internal void SetVisible(bool value)
        {
            foreach (Control ctrl in Controls)
                ctrl.Visible = value;
        }

        public virtual void RenderHeader(HtmlTextWriter writer)
        {
            string url = "#";
            if (!string.IsNullOrEmpty(NavigateUrl))
                url = ResolveUrl(url);

            writer.WriteBeginTag("h3");

            if (!string.IsNullOrEmpty(ToolTip))
                writer.WriteAttribute("title", ToolTip);

            if (!string.IsNullOrEmpty(HeaderStyle))
                writer.WriteAttribute("style", HeaderStyle);

            if (!string.IsNullOrEmpty(HeaderCssClass))
                writer.WriteAttribute("class", HeaderCssClass);
            else
            {
                //UNDONE: when drawing EditableDesignerRegion all property will be set to null ???
                if (DesignMode) //This default header style just use in accrodion
                {
                    if (ParentContainer != null)
                        writer.WriteAttribute("class", "ui-accordion-header ui-helper-reset ui-state-default");
                    else
                        writer.WriteAttribute("class", "ui-accordion-header ui-helper-reset ui-state-active");
                }
            }

            writer.Write(HtmlTextWriter.TagRightChar);

            if (DesignMode)
            {
                writer.WriteBeginTag("span");
                writer.WriteAttribute("class", "ui-icon ui-icon-triangle-1-e");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.WriteEndTag("span");
            }

            writer.WriteBeginTag("a");

            writer.WriteAttribute("href", url);
            if (!string.IsNullOrEmpty(Target))
                writer.WriteAttribute("target", Target);
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Write(Text);
            writer.WriteEndTag("a");
            writer.WriteEndTag("h3");
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (ShowHeader)
                RenderHeader(writer);

            if (DesignMode)
            {
                //    if (string.IsNullOrEmpty(CssClass))
                //        Attributes.Add("class", "ui-accordion-content ui-helper-reset ui-widget-content ui-accordion-content-active");
                Attributes.Add(DesignerRegion.DesignerRegionAttributeName, "viewContent");
            }

            base.RenderBeginTag(writer);
        }
    }
}
