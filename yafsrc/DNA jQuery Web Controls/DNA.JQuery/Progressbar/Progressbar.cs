///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using System.IO;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// The progress bar is designed to simply display the current % complete 
    /// for a process and can be animated by updating the current values over time. 
    /// The bar is coded to be flexibly sized through CSS and will scale to fit inside it's parent container by default. 
    /// </summary>
    /// <remarks>
    /// This is a determinate progress bar, meaning that it should only be used in situations 
    /// where the system can accurately update the current status complete. A determinate progress bar should
    /// never fill from left to right, then loop back to empty for a single process -- if the actual percent complete status
    /// cannot be calculated, an indeterminate progress bar (coming soon) or spinner animation is 
    /// a better way to provide user feedback. 
    /// </remarks>
    [JQuery(Name = "progressbar", Assembly = "jQueryNet", DisposeMethod = "destroy", ScriptResources = new string[] { "ui.core.js", "ui.progressbar.js", "ui.resizable.js" },
        StartEvent = ClientRegisterEvents.ApplicationLoad)]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Progressbar runat=\"server\" ID=\"Progressbar1\"></{0}:Progressbar>")]
    [System.Drawing.ToolboxBitmap(typeof(Progressbar), "Progressbar.Progressbar.ico")]
    public class Progressbar : WebControl, INamingContainer, IPostBackDataHandler
    {
        //private int newValue = 0;
        private Style percentageLabelStyle;

        /// <summary>
        /// Triggered when the progressbar 's value changed
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets/Sets the value of the progressbar.
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
                Object obj = ViewState["Value"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["Value"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the client change event handler
        /// </summary>
        [Category("ClientEvents")]
        [Description("Gets/Sets the client change event handler")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("change", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientValueChanged { get; set; }

        ///// <summary>
        ///// Gets/Sets whether the progressbar can post the data to server
        ///// </summary>
        //[Category("Behavior")]
        //[Description("Gets/Sets whether the progressbar can post the data to server")]
        //public bool AutoPostBack
        //{
        //    get
        //    {
        //        Object obj = ViewState["AutoPostBack"];
        //        return (obj == null) ? false : (bool)obj;
        //    }
        //    set
        //    {
        //        ViewState["AutoPostBack"] = value;
        //    }
        //}

        /// <summary>
        /// Gets/Sets whether the Progressbar can show the percentage label
        /// </summary>
        [Category("Layout")]
        [Description("Gets/Sets whether the Progressbar can show the percentage label")]
        public bool ShowPercentage
        {
            get
            {
                Object obj = ViewState["ShowPercentage"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowPercentage"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the percentage label style
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the percentage label style")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style PercentageLabelStyle
        {
            get
            {
                if (percentageLabelStyle == null)
                    percentageLabelStyle = new Style();
                return percentageLabelStyle;
            }
        }

        /// <summary>
        /// Gets/Sets whether the Progressbar can resizable
        /// </summary>
        [Category("Layout")]
        [Description("Gets/Sets whether the Progressbar can resizable")]
        public bool Resizable
        {
            get
            {
                Object obj = ViewState["Resizable"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["Resizable"] = value;
            }
        }

        private string HiddenKey
        {
            get
            {
                return ClientID + "_value";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresPostBack(this);
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterHiddenField(HiddenKey, this.Value.ToString());
            JQueryScriptBuilder builder = new JQueryScriptBuilder(this);
            builder.Prepare();
            builder.Build();
            builder.AppendBindFunction("progressbarchange", new string[] { "event", "ui" }, "$get('" + ClientID + "_value').value=$(this).progressbar('option', 'value');");
            //StringBuilder scripts = new StringBuilder();
            //scripts.Append("jQuery('#" + ClientID + "').bind('progressbarchange',function(event,ui){");
            //scripts.Append("$get('" + ClientID + "_value').value=$(this).progressbar('option', 'value');");
            //scripts.Append("});");
            //ClientScriptManager.RegisterJQueryControl(this);

            if (Resizable)
            {
                if (ShowPercentage)
                {
                    builder.AppendSelector();
                    builder.Scripts.Append(".resizable({alsoResize:'#" + ClientID + "_PercentagePanel" + "'});");
                }
                //ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_resizable_load", "jQuery('#" + ClientID + "').resizable({alsoResize:'#" + ClientID + "_PercentagePanel" + "'});");
                else
                {
                    builder.AppendSelector();
                    builder.Scripts.Append(".resizable();");
                }
                    //ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_resizable_load", "jQuery('#" + ClientID + "').resizable();");
            }

            ClientScriptManager.RegisterJQueryControl(this, builder);
            //ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_reload", scripts.ToString());
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (ShowPercentage)
            {
                Panel panel = new Panel();
                panel.ID = ClientID + "_PercentagePanel";
                panel.ApplyStyle(PercentageLabelStyle);
                Label label = new Label();
                panel.Style.Add("text-align", "center");
                if (Width.IsEmpty)
                    panel.Style.Add("display", "block");
                else
                    panel.Width = Width;
                panel.Controls.Add(label);
                label.Text = Value.ToString() + "%";
                panel.RenderControl(writer);
            }

            if (DesignMode)
            {
                Style.Add("display", "block");
                CssClass = "ui-progressbar ui-widget ui-widget-content ui-corner-all";
                if (Resizable)
                    Style.Add("position", "relative");
            }

            base.RenderBeginTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                writer.WriteBeginTag("div");
                writer.WriteAttribute("class", "ui-progressbar-value ui-widget-header ui-corner-left");
                writer.WriteAttribute("style", "width:" + Value.ToString() + "%;");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.WriteEndTag("div");

                if (Resizable)
                {
                    CssClass = "ui-progressbar ui-widget ui-widget-content ui-corner-all ui-widget-default ui-resizable";
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", "ui-resizable-handle ui-resizable-se ui-icon ui-icon-gripsmall-diagonal-se");
                    writer.WriteAttribute("style", "z-index: 1001;");
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.WriteEndTag("div");
                }
            }
            base.RenderContents(writer);
        }

        #region IPostBackDataHandler Members

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if (!string.IsNullOrEmpty(postCollection[HiddenKey]))
            {
                int index = Convert.ToInt16(postCollection[HiddenKey]);
                if (index == Value)
                    return false;
                else
                {
                    Value = index;
                    return true;
                }
            }
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            EnsureChildControls();
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}
