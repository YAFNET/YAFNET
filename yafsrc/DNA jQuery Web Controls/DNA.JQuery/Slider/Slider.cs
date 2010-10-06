///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// The Slider is a web control encapsulated the jQuery UI slider plugin.There are
    /// various properties such as multiple handles, and ranges. The handle can be moved with
    /// the mouse or the arrow keys.
    /// </summary>
    /// <remarks>
    /// When a value is chosen using the Slider, it is automatically persisted during
    /// full or partial postbacks. The developer can continue to reference the asp:TextBox to
    /// get and set the Slider's value.
    /// </remarks>
    /// <example>
    /// 	<code lang="ASP.NET" title="Slider Properties">
    /// 		<![CDATA[
    /// <DotNetAge:Slider ID="MySlider" 
    ///    AutoPostBack="false"
    ///    BoundControlID="Textbox1"
    ///    BoundControlID1="Textbox2"
    ///    Maximum="100"
    ///    Minimum="0"
    ///    OnClientSliding=""
    ///    OnClientStartSliding=""
    ///    OnClientStopSliding=""
    ///    OnClientValueChanged=""
    ///    Orientation="Horizontal"
    ///    Range="Both"
    ///    Step="1"
    ///    Value="0"
    ///    Value1="-1"
    ///    Width="300"
    /// />]]>
    /// 	</code>
    /// </example>
    [JQuery(Name = "slider", Assembly = "jQueryNet", DisposeMethod = "destroy", ScriptResources = new string[] { "ui.core.js", "ui.slider.js" }, StartEvent = ClientRegisterEvents.ApplicationLoad)]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Slider runat=\"server\" ID=\"Slider1\"></{0}:Slider>")]
    [System.Drawing.ToolboxBitmap(typeof(Slider), "Slider.Slider.ico")]
    public class Slider : WebControl, INamingContainer, IPostBackDataHandler
    {
        private int value = -1;
        private int value1 = -1;
        
        #region Properies

        /// <summary>
        /// Gets/Sets the Bounding Control ID for Slider that when slider's change the bounding controls value changed too.
        /// </summary>
        [Description("Gets/Sets the Bounding Control ID for Slider that when slider's change the bounding controls value changed too.")]
        [Category("Behavior")]
        [Bindable(true)]
        [TypeConverter(typeof(ControlIDConverter))]
        public string BoundControlID { get; set; }

        /// <summary>
        /// Gets/Sets the Bounding Control ID1 for Slider that when slider's change the bounding controls value1 changed too.
        /// </summary>
        [Description("Gets/Sets the Bounding Control ID1 for Slider that when slider's change the bounding controls value1 changed too.")]
        [Category("Behavior")]
        [Bindable(true)]
        [TypeConverter(typeof(ControlIDConverter))]
        public string BoundControlID1 { get; set; }

        /// <summary>
        ///  Gets/Sets whether to slide handle smoothly when user click outside handle on the bar.
        /// </summary>
        [Description("Gets/Sets whether to slide handle smoothly when user click outside handle on the bar.")]
        [Category("Behavior")]
        [Bindable(true)]
        [Themeable(true)]
        [JQueryOption("animate", IgnoreValue = false)]
        public bool EnabledAnimate
        {
            get
            {
                Object obj = ViewState["EnabledAnimate"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["EnabledAnimate"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the maximum value of the slider.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the maximum value of the slider.")]
        [Category("Data")]
        [Bindable(true)]
        [JQueryOption("max", IgnoreValue = 0)]
        public int Maximum
        {
            get
            {
                Object obj = ViewState["Maximum"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["Maximum"] = value;
            }
        }


        /// <summary>
        /// Gets/Sets the minimum value of the slider.
        /// </summary>
        [Category("Data")]
        [Description("Gets/Sets the minimum value of the slider.")]
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [JQueryOption("min", IgnoreValue = 0)]
        public int Minimum
        {
            get
            {
                Object obj = ViewState["Minimum"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["Minimum"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Silder's Orientaion
        /// </summary>
        [Category("Layout")]
        [Description("Gets/Sets the Silder's Orientaion")]
        [Bindable(true)]
        [Themeable(true)]
        [NotifyParentProperty(true)]
        [JQueryOption("orientation")]
        public Orientation Orientation
        {
            get
            {
                Object obj = ViewState["Orientation"];
                return (obj == null) ? Orientation.Vertical : (Orientation)obj;
            }
            set
            {
                ViewState["Orientation"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the size or amount of each interval or step the slider takes between min and max. 
        /// </summary>
        /// <remarks>
        /// The full specified value range of the slider (max - min) needs to be evenly divisible by the step.
        /// </remarks>
        [Bindable(true)]
        [Category("Data")]
        [Description("Gets/Sets the size or amount of each interval or step the slider takes between min and max. ")]
        [Themeable(true)]
        [JQueryOption("step", IgnoreValue = 0)]
        public int Step
        {
            get
            {
                Object obj = ViewState["Step"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["Step"] = value;
            }
        }

        /// <summary>
        ///  Gets/Sets the slider's range,if this property set the slider will detect if you have two handles and 
        ///  create a stylable  range element between these two. Two other possible values are 'Min' and 'Max'.
        ///  A Min range goes from the slider Min to one handle. A max range goes from one handle to the 
        ///  slider max.
        /// </summary>
        [Category("Data")]
        [Bindable(true)]
        [NotifyParentProperty(true)]
        [Description("Gets/Sets the slider's range")]
        [Themeable(true)]
        // [JQueryOption("range", IgnoreValue = SliderRanges.NotSet)]
        public SliderRanges Range
        {
            get
            {
                Object obj = ViewState["Range"];
                return (obj == null) ? SliderRanges.NotSet : (SliderRanges)obj;
            }
            set
            {
                ViewState["Range"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the value of the slider, if there's only one handle. 
        /// If there is more than one handle, determines the value of the first handle.
        /// </summary>
        [Description("Gets/Sets the value of the slider, if there's only one handle. ")]
        [Category("Data")]
        [NotifyParentProperty(true)]
        [Bindable(true)]
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        ///  This property can be used to specify multiple handles. If range is set to true, the length of 'values' should be 2.
        /// </summary>
        [Description(" This property can be used to specify multiple handles. If range is set to true, the length of 'values' should be 2.")]
        [Category("Data")]
        [NotifyParentProperty(true)]
        [Bindable(true)]
        public int Value1
        {
            get
            {
                return value1;
            }
            set
            {
                value1 = value;
            }
        }

        /// <summary>
        /// Gets/Sets the silder can be post the client changed to server
        /// </summary>
        [Category("Behavior")]
        [Bindable(true)]
        [Description("Gets/Sets the silder can be post the client changed to server")]
        public bool AutoPostBack
        {
            get
            {
                Object obj = ViewState["AutoPostBack"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AutoPostBack"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the start client event handler.
        /// </summary>
        /// <remarks>This event is triggered when the user starts sliding</remarks>
        [Category("ClientEvents")]
        [Description("Gets/Sets the start client event handler.")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("start", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientStartSliding { get; set; }

        /// <summary>
        /// Gets/Sets the slide client event handler.
        /// </summary>
        [Category("ClientEvents")]
        [Description("Gets/Sets the slide client event handler.")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("slide", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientSliding { get; set; }

        /// <summary>
        /// Gets/Sets the change client event handler.
        /// </summary>
        [Category("ClientEvents")]
        [Description(" Gets/Sets the change client event handler.")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("change", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientValueChanged { get; set; }

        /// <summary>
        /// Gets/Sets the stop client event handler.
        /// </summary>
        [Category("ClientEvents")]
        [Description("Gets/Sets the stop client event handler.")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("stop", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientStopSliding { get; set; }

        /// <summary>
        /// When slider's value changed this event will trigger.
        /// </summary>
        public event EventHandler ValueChanged;
        //UNDONE:Range and Values is not support now
        #endregion

        private string HiddenKey { get { return ClientID + "_value"; } }

        /// <summary>
        /// Gets/Sets the Slider width just only avalidable for Horizontal mode
        /// </summary>
        [NotifyParentProperty(true)]
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

        /// <summary>
        /// Gets/Sets the Slider height just only avalidable for Vertical mode
        /// </summary>
        [NotifyParentProperty(true)]
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

        /// <summary>
        ///  Slider has no tooltip
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string ToolTip
        {
            get
            {
                return base.ToolTip;
            }
            set
            {
                base.ToolTip = value;
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresPostBack(this);
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            JQueryScriptBuilder builder = new JQueryScriptBuilder(this);
            Page.ClientScript.RegisterHiddenField(HiddenKey, this.Value.ToString());

            builder.Prepare();

            if (Range != SliderRanges.NotSet)
            {
                if (Range == SliderRanges.Both)
                    builder.AddOption("range", true);
                else
                    builder.AddOption("range", Range.ToString().ToLower(), true);
            }

            if (Range == SliderRanges.Both)
                builder.AddOption("values", new int[] { value1, value });
            else
            {
                if (value > -1)
                    builder.AddOption("value", value);
            }

            builder.Build();

            StringBuilder scripts = new StringBuilder();

            if (Range == SliderRanges.Both)
            {
                Page.ClientScript.RegisterHiddenField(HiddenKey + "1", Value1.ToString());
                scripts.Append("$get('" + HiddenKey + "').value=ui.values[1];");
                scripts.Append("$get('" + HiddenKey + "1').value=ui.values[0];");

                if (!string.IsNullOrEmpty(BoundControlID))
                {
                    Control ctrl = ClientScriptManager.FindControl(Page.Form, BoundControlID);
                    if (ctrl == null)
                        throw new Exception("Counld not found the Control \"" + BoundControlID + "\"");
                    if (ctrl.GetType() == typeof(TextBox))
                        scripts.Append("$get('" + ctrl.ClientID + "').value=ui.values[1];");
                    else
                        scripts.Append("jQuery('#" + ctrl.ClientID + "').html(ui.values[1]);");
                }

                if (!string.IsNullOrEmpty(BoundControlID1))
                {
                    Control ctrl = ClientScriptManager.FindControl(Page.Form, BoundControlID1);
                    if (ctrl == null)
                        throw new Exception("Counld not found the Control \"" + BoundControlID1 + "\"");
                    if (ctrl.GetType() == typeof(TextBox))
                        scripts.Append("$get('" + ctrl.ClientID + "').value=ui.values[0];");
                    else
                        scripts.Append("jQuery('#" + ctrl.ClientID + "').html(ui.values[0]);");
                }

            }
            else
            {
                scripts.Append("$get('" + HiddenKey + "').value=ui.value;");
                if (!string.IsNullOrEmpty(BoundControlID))
                {
                    Control ctrl = ClientScriptManager.FindControl(Page.Form, BoundControlID);
                    if (ctrl == null)
                        throw new Exception("Counld not found the Control \"" + BoundControlID + "\"");
                    if (ctrl.GetType() == typeof(TextBox))
                        scripts.Append("$get('" + ctrl.ClientID + "').value=ui.value;");
                    else
                        scripts.Append("jQuery('#" + ctrl.ClientID + "').html(ui.value);");
                }
            }

            builder.AppendBindFunction("slide",new string[]{"event","ui"},scripts.ToString());

            if (AutoPostBack)
                builder.AppendBindFunction("slidestop", new string[] { "event", "ui" }, Page.ClientScript.GetPostBackEventReference(this, "") + ";");

            ClientScriptManager.RegisterJQueryControl(this, builder);

            #region 1.0.0.0
            //StringBuilder scripts = new StringBuilder();
            //Dictionary<string, string> options = new Dictionary<string, string>();
            //Page.ClientScript.RegisterHiddenField(HiddenKey, this.Value.ToString());

            //if (Range==SliderRanges.Both)
            //{
            //    Page.ClientScript.RegisterHiddenField(HiddenKey+"1", Value1.ToString());
            //    scripts.Append("$get('" + HiddenKey + "').value=ui.values[1];");
            //    scripts.Append("$get('" + HiddenKey + "1').value=ui.values[0];");

            //    if (!string.IsNullOrEmpty(BoundControlID))
            //    {
            //        Control ctrl = ClientScriptManager.FindControl(Page.Form, BoundControlID);
            //        if (ctrl == null)
            //            throw new Exception("Counld not found the Control \"" + BoundControlID + "\"");
            //        if (ctrl.GetType() == typeof(TextBox))
            //            scripts.Append("$get('" + ctrl.ClientID + "').value=ui.values[1];");
            //        else
            //            scripts.Append("jQuery('#" + ctrl.ClientID + "').html(ui.values[1]);");
            //    }

            //    if (!string.IsNullOrEmpty(BoundControlID1))
            //    {
            //        Control ctrl= ClientScriptManager.FindControl(Page.Form, BoundControlID1);
            //        if (ctrl == null)
            //            throw new Exception("Counld not found the Control \"" + BoundControlID1 + "\"");
            //        if (ctrl.GetType() == typeof(TextBox))
            //            scripts.Append("$get('" + ctrl.ClientID + "').value=ui.values[0];");
            //        else
            //            scripts.Append("jQuery('#" + ctrl.ClientID + "').html(ui.values[0]);");
            //    }

            //}
            //else
            //{
            //    scripts.Append("$get('" + HiddenKey + "').value=ui.value;");
            //    if (!string.IsNullOrEmpty(BoundControlID))
            //    {
            //        Control ctrl = ClientScriptManager.FindControl(Page.Form, BoundControlID);
            //        if (ctrl == null)
            //            throw new Exception("Counld not found the Control \"" + BoundControlID + "\"");
            //        if (ctrl.GetType() == typeof(TextBox))
            //            scripts.Append("$get('" + ctrl.ClientID + "').value=ui.value;");
            //        else
            //            scripts.Append("jQuery('#" + ctrl.ClientID + "').html(ui.value);");
            //    }
            //}

            //if (Range != SliderRanges.NotSet)
            //{
            //    if (Range == SliderRanges.Both)
            //        options.Add("range", "true");
            //    else
            //        options.Add("range", "'" + Range.ToString().ToLower() + "'");
            //}

            //if (Range==SliderRanges.Both)
            //    options.Add("values", "[" + value1.ToString() + "," + value.ToString() + "]");
            //else
            //{
            //    if (value > -1)
            //        options.Add("value", value.ToString());
            //}

            //if (options.Count > 0)
            //    ClientScriptManager.RegisterJQueryControl(this, options);
            //else
            //    ClientScriptManager.RegisterJQueryControl(this);

            //scripts.Insert(0, "jQuery('#" + ClientID + "').bind('slide',function(event,ui){");
            //scripts.Append("});");

            //if (AutoPostBack)
            //{
            //    //Debug Only
            //    //scripts.Append("Sys.Debug.trace('" + Value.ToString() + "');");
            //    scripts.Append("jQuery('#" + ClientID + "').bind('slidestop',function(event,ui){");
            //    scripts.Append(Page.ClientScript.GetPostBackEventReference(this, "") + ";");
            //    scripts.Append("});");
            //}

            //ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_reload", scripts.ToString());
            #endregion

        }

        #region IPostBackDataHandler Members

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if (!string.IsNullOrEmpty(postCollection[HiddenKey]))
            {
                string valueString = Context.Request.Form[HiddenKey];
                int v = 0;
                int v1 = -1;
                //string[] values = null;

                if (Range == SliderRanges.Both)
                {
                    v = int.Parse(Context.Request.Form[HiddenKey]);
                    v1 = int.Parse(Context.Request.Form[HiddenKey + "1"]);
                    if ((v != Value) || (v1 != Value1))
                    {
                        value = v;
                        value1 = v1;
                        return true;
                    }
                }
                else
                {
                    v = int.Parse(valueString);
                    if (v != Value)
                    {
                        Value = v;
                        return true;
                    }
                }
            }

            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        #endregion

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if ((this.Width.IsEmpty) || this.Width.Value <= 9)
                        Width = 300;
                    Height = 9;

                    if (String.IsNullOrEmpty(Page.StyleSheetTheme))
                    {
                        //None Styling thow exception
                    }

                    Attributes.Add("class", "ui-slider ui-slider-horizontal ui-widget ui-widget-content ui-corner-all");
                }
                else
                {
                    if ((this.Height.IsEmpty) || (this.Height.Value <= 9))
                        Height = 200;
                    Width = 9;
                    Attributes.Add("class", "ui-slider ui-slider-vertical ui-widget ui-widget-content ui-corner-all");
                }
            }
            base.RenderBeginTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Maximum > 0)
            {
                if (Value > Maximum)
                    throw new Exception("The Value is over the Maximum value");
            }

            if (Range == SliderRanges.Both)
            {
                if (Value < Value1)
                    throw new Exception("The Value1 must be >= Value when the Range sets");
            }


            if (DesignMode)
            {
                decimal max = Maximum;
                if (max == 0)
                    max = 100;
                decimal percent = (Convert.ToDecimal(Value) / max) * 100;

                writer.WriteBeginTag("div");

                switch (Range)
                {
                    case SliderRanges.NotSet:
                        break;
                    case SliderRanges.Max:
                        writer.WriteAttribute("class", "ui-slider-range ui-slider-range-min ui-widget-header");
                        if (Orientation == Orientation.Horizontal)
                            writer.WriteAttribute("style", "width: " + ((1 - (percent / 100)) * 100).ToString() + "%;left:" + percent.ToString() + "%");
                        else
                            writer.WriteAttribute("style", "height:" + ((1 - (percent / 100)) * 100).ToString() + "%;top:" + percent.ToString() + "%");
                        break;
                    case SliderRanges.Min:
                        writer.WriteAttribute("class", "ui-slider-range ui-slider-range-min ui-widget-header");
                        if (Orientation == Orientation.Horizontal)
                            writer.WriteAttribute("style", "width: " + percent.ToString() + "%;");
                        else
                            writer.WriteAttribute("style", "height:" + percent.ToString() + "%;");
                        break;
                    case SliderRanges.Both:
                        writer.WriteAttribute("class", "ui-slider-range ui-widget-header");
                        decimal percentValue = (Convert.ToDecimal(Value - Value1) / max) * 100;

                        if (Orientation == Orientation.Horizontal)
                            writer.WriteAttribute("style", "width: " + percentValue.ToString() + "%;left:" + ((Convert.ToDecimal(Value1) / max) * 100).ToString() + "%");
                        else
                            writer.WriteAttribute("style", "height:" + percentValue.ToString() + "%;top:" + ((1 - (percent / 100)) * 100).ToString() + "%");
                        break;
                }

                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                writer.WriteEndTag("div");


                //Value
                writer.WriteBeginTag("a");
                writer.WriteAttribute("href", "#");
                writer.WriteAttribute("class", "ui-slider-handle ui-state-default ui-corner-all");
                if (Orientation == Orientation.Horizontal)
                    writer.WriteAttribute("style", "left: " + percent.ToString() + "%;");
                else
                    writer.WriteAttribute("style", "top:" + ((1 - (percent / 100)) * 100).ToString() + "%;");
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                writer.WriteEndTag("a");

                //Value1 when range set 
                if (Range == SliderRanges.Both)
                {
                    writer.WriteBeginTag("a");
                    writer.WriteAttribute("href", "#");
                    writer.WriteAttribute("class", "ui-slider-handle ui-state-default ui-corner-all");
                    if (Orientation == Orientation.Horizontal)
                        writer.WriteAttribute("style", "left: " + ((Convert.ToDecimal(Value1) / max) * 100).ToString() + "%;");
                    else
                        writer.WriteAttribute("style", "top:" + ((1 - ((Convert.ToDecimal(Value1) / max))) * 100).ToString() + "%;");
                    writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                    writer.WriteEndTag("a");
                }
            }
            base.RenderContents(writer);
        }


    }
}
