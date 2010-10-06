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
using System.Web.UI.HtmlControls;
using System.Text;

namespace DNA.UI
{
    /// <summary>
    ///  CollapsablePanel is a Panel Control can be collapse and expand.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ParseChildren(true, "ContentTempalte")]
    [ToolboxData("<{0}:CollapsablePanel runat=\"server\" id=\"CollapsablePanel1\"><ContentTempalte></ContentTempalte></{0}:CollapsablePanel>")]
    public class CollapsablePanel : CompositeControl, IPostBackDataHandler
    {
        public event EventHandler Collapse;
        public event EventHandler Expand;
        private string contentCssClass = "ui-widget-content ui-corner-bottom";
        private Panel bodyPanel;
        private HtmlGenericControl headerPanel;

        [CssClassProperty]
        public string ContentCssClass
        {
            get { return contentCssClass; }
            set { contentCssClass = value; }
        }

        #region [Private]
        private bool isCollapsed = false;
        private string expandIconCssClass = "";
        private string collapsedIconCssClass = "";
        private string title = "";
        private Style headerStyle;
        private bool usingDefaultStyle = true;
        private ITemplate contentTempalte;
        #endregion

        #region [Properties]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        [TemplateContainer(typeof(CollapsablePanel))]
        public ITemplate ContentTempalte
        {
            get { return contentTempalte; }
            set { contentTempalte = value; }
        }

        /// <summary>
        /// Gets/Sets whether using the default styles
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets whether using the default styles")]
        public bool UsingDefaultStyle
        {
            get { return usingDefaultStyle; }
            set { usingDefaultStyle = value; }
        }

        [Category("Appearance")]
        public Style HeaderStyle
        {
            get { return headerStyle; }
            set { headerStyle = value; }
        }

        /// <summary>
        /// Gets/Sets the Header Title text
        /// </summary>
        [Category("Appearance")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [Category("Appearance")]
        [CssClassProperty]
        public string ExpandIconCssClass
        {
            get { return expandIconCssClass; }
            set { expandIconCssClass = value; }
        }

        [Category("Appearance")]
        [CssClassProperty]
        public string CollapsedIconCssClass
        {
            get { return collapsedIconCssClass; }
            set { collapsedIconCssClass = value; }
        }

        public bool IsCollapsed
        {
            get { return isCollapsed; }
            set { isCollapsed = value; }
        }

        #endregion
        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresPostBack(this);
            if (!DesignMode)
            {
                if (Context.Request.Browser.Browser == "IE")
                {
                    if (Context.Request.Browser.MajorVersion < 8)
                    {
                        ClientScriptManager.AddCompositeScript(this, "jQueryNet.plugins.bgiframe.js", "jQueryNet");
                        ClientScriptManager.RegisterDocumentReadyScript(this, "$('#"+this.ClientID+"').bgiframe();");
                    }
                }
            }
            base.OnInit(e);
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        private string HiddenKey
        {
            get { return this.ClientID + "_Hidden"; }
        }

        protected override void CreateChildControls()
        {
            headerPanel = new HtmlGenericControl("h3");
            bodyPanel = new Panel();
            bodyPanel.ID = "Body";
            headerPanel.ID = "Header";
            Controls.Add(headerPanel);
            Controls.Add(bodyPanel);
            if (contentTempalte != null)
                contentTempalte.InstantiateIn(bodyPanel);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterHiddenField(HiddenKey, IsCollapsed.ToString());
            if (isCollapsed)

                headerPanel.Attributes.Add("class", "ui-widget-header ui-state-active ui-corner-all");
            else
                headerPanel.Attributes.Add("class", "ui-widget-header ui-state-active ui-corner-top");

            headerPanel.Style.Add(HtmlTextWriterStyle.Position, "relative");
            headerPanel.Style.Add(HtmlTextWriterStyle.Height, "25px");
            CollapsedIconCssClass = "ui-icon-triangle-1-e";
            ExpandIconCssClass = "ui-icon-triangle-1-s";
            bodyPanel.CssClass = contentCssClass;
            

            HtmlGenericControl span = new HtmlGenericControl("span");
            span.Style.Add(HtmlTextWriterStyle.Position, "absolute");
            span.Style.Add(HtmlTextWriterStyle.Top, "50%");
            span.Style.Add(HtmlTextWriterStyle.Left, "0.5em");
            span.Style.Add(HtmlTextWriterStyle.MarginTop, "-8px");
            if (isCollapsed)
            {
                span.Attributes.Add("class", "ui-icon " + CollapsedIconCssClass);
                bodyPanel.Style[HtmlTextWriterStyle.Display] = "none";
            }
            else
            {
                span.Attributes.Add("class", "ui-icon " + ExpandIconCssClass);
                bodyPanel.Style[HtmlTextWriterStyle.Display] = "block";
            }



            StringBuilder scripts = new StringBuilder();
            scripts.Append("jQuery(this).find('span').toggleClass('" + CollapsedIconCssClass + "');");
            scripts.Append("jQuery(this).find('span').toggleClass('" + ExpandIconCssClass + "');");
            scripts.Append("jQuery(this).toggleClass('ui-corner-all');");
            scripts.Append("jQuery(this).toggleClass('ui-corner-top');");
            scripts.Append("jQuery('#" + bodyPanel.ClientID + "').slideToggle('fast');");
            scripts.Append("if ($('#" + HiddenKey + "').val()=='True') ");
            scripts.Append("jQuery('#" + HiddenKey + "').val('False'); else ");
            scripts.Append("jQuery('#" + HiddenKey + "').val('True');");
            headerPanel.Attributes.Add("onclick", scripts.ToString());
            HyperLink label = new HyperLink();
            label.NavigateUrl = "#";
            label.Text = title;
            label.Style.Add(HtmlTextWriterStyle.Padding, "4px 0.5em 0.5em 30px");
            label.Font.Size = FontUnit.Parse("10pt");
            label.Style.Add(HtmlTextWriterStyle.Display, "block");

            headerPanel.Controls.Add(span);
            headerPanel.Controls.Add(label);
            base.RenderContents(writer);
        }


        #region IPostBackDataHandler Members

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            EnsureChildControls();
            bool ic = false;
            if (bool.TryParse(postCollection[HiddenKey], out ic))
            {
                IsCollapsed = ic;
                return true;
            }

            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            if (IsCollapsed)
            {
                if (Collapse != null)
                    Collapse(this, EventArgs.Empty);
            }
            else
            {
                if (Expand != null)
                    Expand(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
