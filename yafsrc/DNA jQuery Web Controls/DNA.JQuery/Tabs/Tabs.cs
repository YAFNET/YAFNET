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
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Drawing.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// Tabs is a ASP.NET WebControl that encapsulated the jQuery UI tabs plugin.Tabs are
    /// generally used to break content into multiple sections that can be swapped to save
    /// space, much like an Accordion.
    /// </summary>
    /// <remarks>
    /// 	<para>By default a Tabs will swap between tabbed sections onClick, but the events
    ///     can be changed to onHover through an property. Tab content can be loaded via Ajax
    ///     by setting an NavigateUrl on a View Control.</para>
    /// </remarks>
    /// <example>
    /// 	<code lang="ASP.NET" title="Tabs properties">
    /// 		<![CDATA[
    /// <DotNetAge:Tabs ID="MyTabs"
    ///      ActiveTabEvent="Click"
    ///      AsyncLoad="false"
    ///      AutoPostBack="false"
    ///      Collapsible="false"
    ///      ContentCssClass=""
    ///      ContentStyle=""
    ///      Deselectable="false"
    ///      EnabledContentCache="false"
    ///      HeaderCssClass=""
    ///      HeaderStyle=""
    ///      Height="300"
    ///      OnClientTabAdd=""
    ///      OnClientTabDisabled=""
    ///      OnClientTabEnabled=""
    ///      OnClientTabLoad=""
    ///      OnClientTabRemove=""
    ///      OnClientTabSelected=""
    ///      OnClientTabShow=""
    ///      SelectedIndex="0"
    ///      Sortable="false"
    ///      Spinner=""
    ///      Width="300"
    /// >
    ///    <Animations>
    ///        <DotNetAge:AnimationAttribute AnimationType="opacity" Value="toggle" />
    ///        <DotNetAge:AnimationAttribute AnimationType="height" Value="toggle" />
    ///    </Animations>
    ///    <Views>
    ///       <DotNetAge:View Text=""
    ///             ID="View1"
    ///             NavigateUrl=""
    ///             HeaderCssClass=""
    ///             HeaderStyle=""
    ///             Target="_blank">
    ///             <p>View is content is here</p>
    ///       </DotNetAge:View>
    ///       ...
    ///       <DotNetAge:View />
    ///    </Views>
    /// </DotNetAge:Tabs>]]>
    /// 	</code>
    /// </example>
    [JQuery(Name = "tabs", Assembly = "jQueryNet", DisposeMethod = "destroy", ScriptResources = new string[] { "ui.core.js", "ui.tabs.js", "ui.sortable.js" }, StartEvent = ClientRegisterEvents.ApplicationLoad)]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Tabs runat=\"server\" ID=\"Tabs1\"></{0}:Tabs>")]
    [System.Drawing.ToolboxBitmap(typeof(Tabs), "Tabs.Tabs.ico")]
    [Designer(typeof(Design.TabsDesigner))]
    [ParseChildren(true)]
    public class Tabs : JQueryMultiViewControl, INamingContainer
    {
        public event EventHandler TabChanged;
        private NameValueAttributeCollection<AnimationAttribute> animations = null;

        /// <summary>
        /// Gets/Sets Whether or not to cache remote tabs content.
        /// </summary>
        /// <remarks>
        /// e.g. load only once or with every click. 
        /// Cached content is being lazy loaded, e.g once and only once for the first click. 
        /// Note that to prevent the actual Ajax requests from being cached by the browser you need 
        /// to provide an extra cache: false flag to ajaxOptions.
        /// </remarks>
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/Sets Whether or not to cache remote tabs content")]
        [JQueryOption(Name = "cache", IgnoreValue = false)]
        public bool EnabledContentCache
        {
            get
            {
                Object obj = ViewState["EnabledContentCache"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["EnabledContentCache"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the AJAX async load mode.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/Sets the AJAX async load mode.")]
        public bool AsyncLoad
        {
            get
            {
                Object obj = ViewState["AsyncLoad"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AsyncLoad"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the active tab's event
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/Sets the active tab's event")]
        public DomEvents ActiveTabEvent
        {
            get
            {
                Object obj = ViewState["ActiveTabEvent"];
                return (obj == null) ? DomEvents.Click : (DomEvents)obj;
            }
            set
            {
                ViewState["ActiveTabEvent"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether tabs allow an already selected tab to become unselected again upon reselection.
        /// </summary>
        [JQueryOption(Name = "collapsible", IgnoreValue = false)]
        [Category("Behavior")]
        [Description("Gets/Sets whether tabs allow an already selected tab to become unselected again upon reselection.")]
        public bool Collapsible
        {
            get
            {
                Object obj = ViewState["Collapsible"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["Collapsible"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the tabs can be Sortable if this property is set the sortable plugin will be used
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the tabs can be Sortable if this property is set the sortable plugin will be used")]
        [Bindable(true)]
        public bool Sortable
        {
            get
            {
                Object obj = ViewState["Sortable"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["Sortable"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the tabs can be select
        /// </summary>
        /// <remarks>deprecated in jQuery UI 1.7, use collapsible.</remarks>
        [JQueryOption(Name = "deselectable", IgnoreValue = false)]
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/Sets whether the tabs can be select")]
        public bool Deselectable
        {
            get
            {
                Object obj = ViewState["Deselectable"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["Deselectable"] = value;
            }
        }

        /// <summary>
        ///  Gets/Sets the current selected tab's index
        /// </summary>
        /// <remarks>Zero-based index of the tab to be selected on initialization. To set all tabs to nselected pass -1 as value.</remarks>
        [JQueryOption(Name = "selected")]
        [Description("Gets/Sets the current selected tab's index")]
        public override int SelectedIndex
        {
            get
            {
                return base.SelectedIndex;
            }
            set
            {
                base.SelectedIndex = value;
            }
        }

        //public int SelectedIndex
        //{
        //    get
        //    {
        //        Object obj = ViewState["SelectedIndex"];
        //        return (obj == null) ? 0 : (int)obj;
        //    }
        //    set
        //    {
        //        ViewState["SelectedIndex"] = value;
        //    }
        //}

        /// <summary>
        /// Gets/Sets the HTML content of this string is shown in a tab title while remote content is loading. 
        /// </summary>
        /// <remarks>
        /// Pass in empty string to deactivate that behavior.
        /// </remarks>
        [JQueryOption("spinner")]
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/Sets the HTML content of this string is shown in a tab title while remote content is loading. ")]
        public string Spinner
        {
            get
            {
                Object obj = ViewState["Spinner"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["Spinner"] = value;
            }
        }

        /// <summary>
        ///Gets/Sets the tabs client load event handler.
        /// </summary>
        /// <remarks>This event is triggered after the content of a remote tab has been loaded.</remarks>
        [Browsable(true)]
        [Category("ClientEvents")]
        [Bindable(true)]
        [Description("Gets/Sets the tabs client load event handler")]
        [JQueryOption("load", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientTabLoad { get; set; }

        /// <summary>
        /// Gets/Sets the tabs client show event handler.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a tab is shown.
        /// </remarks>
        [Description("Gets/Sets the tabs client show event handler.")]
        [Browsable(true)]
        [Category("ClientEvents")]
        [Bindable(true)]
        [JQueryOption("show", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientTabShow { get; set; }

        /// <summary>
        /// Gets/Sets the tabs add event handler.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a tab is added
        /// </remarks>
        [Browsable(true)]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("add", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientTabAdd { get; set; }

        /// <summary>
        /// Gets/Sets the tab remove event handler.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a tab is removed.
        /// </remarks>
        [Browsable(true)]
        [Description("Gets/Sets the tab remove event handler.")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("remove", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientTabRemove { get; set; }

        /// <summary>
        /// Gets/Sets the tabs client tab selected event handler.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a tab is selected.
        /// </remarks>
        [Browsable(true)]
        [Description("Gets/Sets the tabs client tab selected event handler.")]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("select", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientTabSelected { get; set; }

        /// <summary>
        /// Gets/Sets the tabs client enabled event handler.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a tab is enabled.
        /// </remarks>
        [Browsable(true)]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("enable", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientTabEnabled { get; set; }

        /// <summary>
        /// Gets/Sets the tabs client disabled event handler.
        /// </summary>
        /// <remarks>
        /// This event is triggered when a tab is disable.
        /// </remarks>
        [Browsable(true)]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        [JQueryOption("disable", JQueryOptionTypes.Function, FunctionParams = new string[] { "event", "ui" })]
        public string OnClientTabDisabled { get; set; }

        /// <summary>
        /// Gets the animation attribute Collection of the tab hide and show
        /// </summary>
        [Category("Appearance")]
        [Description("Gets the animation Attribute Collection of the tab hide and show")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public NameValueAttributeCollection<AnimationAttribute> Animations
        {
            get
            {
                if (animations == null)
                    InitAnimations();
                return animations;
            }
        }

        private void InitAnimations()
        {
            animations = new NameValueAttributeCollection<AnimationAttribute>();
            if (EnableViewState)
                ((IStateManager)animations).TrackViewState();
        }

        protected override void OnActiveViewChanged()
        {
            if (TabChanged != null)
                TabChanged(this, EventArgs.Empty);
        }

        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterHiddenField(HiddenKey, SelectedIndex.ToString());
            JQueryScriptBuilder builder = new JQueryScriptBuilder(this);
            builder.Prepare();

            #region  disabled option
            string disabled = "";
            foreach (View view in Views)
            {
                if (!view.Enabled)
                {
                    if (!string.IsNullOrEmpty(disabled))
                        disabled = disabled + "," + view.Index.ToString();
                    else
                        disabled = view.Index.ToString();
                }
            }

            if (!string.IsNullOrEmpty(disabled))
                builder.AddOption("disabled", "[" + disabled + "]");
            #endregion

            if (ActiveTabEvent != DomEvents.Click)
                builder.AddOption("event", ActiveTabEvent.ToString().ToLower(), true);

            if (Animations.Count > 0)
                builder.AddOption("fx", animations.ToJSONString());

            if (this.AsyncLoad)
                builder.AddOption("ajaxOptions", "{async:true}");
            builder.Build();
            builder.AppendCssStyle("'display':'block'");

            if (Sortable)
            {
                builder.AppendSelector();
                builder.Scripts.Append(".tabs().find('.ui-tabs-nav').sortable({axis:'x'});");
            }

            StringBuilder onSelectedScript = new StringBuilder();
            onSelectedScript.AppendLine("$get('" + HiddenKey + "').value=$(this).tabs('option','selected');");

            if (AutoPostBack)
            {
                foreach (View view in Views)
                {
                    if (view.Index == SelectedIndex)
                        view.SetVisible(true);
                    else
                        view.SetVisible(false);
                }
                onSelectedScript.AppendLine(Page.ClientScript.GetPostBackEventReference(this, "selected") + ";");
            }

            Style.Add("display", "none");
            builder.AppendBindFunction("tabsshow", new string[] { "event", "ui" }, onSelectedScript.ToString());
            ClientScriptManager.RegisterJQueryControl(this, builder);

            #region v1.0.0.0
            //Dictionary<string, string> options = new Dictionary<string, string>();
            //Page.ClientScript.RegisterHiddenField(HiddenKey, SelectedIndex.ToString());
            //StringBuilder onSelectedScript = new StringBuilder();
            //onSelectedScript.AppendLine("$get('" + HiddenKey + "').value=$(this).tabs('option','selected');");
            //if (AutoPostBack)
            //{
            //    foreach (View view in Views)
            //    {
            //        if (view.Index == SelectedIndex)
            //            view.SetVisible(true);
            //        else
            //            view.SetVisible(false);
            //    }
            //    onSelectedScript.AppendLine(Page.ClientScript.GetPostBackEventReference(this, "selected") + ";");
            //}

            //Style.Add("display", "none");

            //#region other options
            //StringBuilder loadScripts = new StringBuilder();
            //loadScripts.Append("jQuery('#" + this.ClientID + "').css({'display':'block'});");

            //if (Sortable)
            //    loadScripts.AppendLine("jQuery('#" + this.ClientID + "').tabs().find('.ui-tabs-nav').sortable({axis:'x'});");

            //#region  disabled option

            //string disabled = "";
            //foreach (View view in Views)
            //{
            //    if (!view.Enabled)
            //    {
            //        if (!string.IsNullOrEmpty(disabled))
            //            disabled = disabled + "," + view.Index.ToString();
            //        else
            //            disabled = view.Index.ToString();
            //    }
            //}

            //if (!string.IsNullOrEmpty(disabled))
            //    options.Add("disabled", "[" + disabled + "]");
            //#endregion

            //if (ActiveTabEvent != DomEvents.Click)
            //    options.Add("event", "'" + ActiveTabEvent.ToString().ToLower() + "'");

            //if (Animations.Count > 0)
            //    options.Add("fx", animations.ToJSONString());

            //if (this.AsyncLoad)
            //    options.Add("ajaxOptions", "{async:true}");

            //if (options.Count > 0)
            //    ClientScriptManager.RegisterJQueryControl(this, options);
            //else
            //    base.OnPreRender(e);

            //onSelectedScript.Insert(0, "function(event,ui){");
            //onSelectedScript.Append("}");
            //loadScripts.AppendLine("jQuery('#" + ClientID + "').bind('tabsshow'," + onSelectedScript.ToString() + ");");
            // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_reload", loadScripts.ToString());

            //#endregion
            #endregion
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.WriteFullBeginTag("ul");

            foreach (DNA.UI.JQuery.View view in Views)
            {
                if (view.Visible)
                {
                    writer.WriteFullBeginTag("li");
                    writer.WriteBeginTag("a");
                    if (string.IsNullOrEmpty(view.NavigateUrl))
                        writer.WriteAttribute("href", "#" + view.ClientID);
                    else
                        writer.WriteAttribute("href", Page.ResolveUrl(view.NavigateUrl));
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Write(view.Text);
                    writer.WriteEndTag("a");
                    writer.WriteEndTag("li");
                }
            }
            writer.WriteEndTag("ul");

            foreach (DNA.UI.JQuery.View view in Views)
            {
                if (view.Visible == false)
                    continue;

                view.ShowHeader = false;
                if (string.IsNullOrEmpty(view.NavigateUrl))
                    view.RenderControl(writer);
            }
        }

        #region IPostBackDataHandler Members

        protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if (!string.IsNullOrEmpty(postCollection[HiddenKey]))
            {
                int index = Convert.ToInt16(postCollection[HiddenKey]);
                if (index == SelectedIndex)
                    return false;
                else
                {
                    newIndex = index;
                    return true;
                }
            }
            return false;
        }

        protected override void RaisePostDataChangedEvent()
        {
            EnsureChildControls();
            if (TabChanged != null)
                TabChanged(this, EventArgs.Empty);

            View deactiveView = Views[SelectedIndex];
            deactiveView.OnDeactive();

            View activeView = Views[newIndex];
            SelectedIndex = newIndex;
            activeView.OnActive();
        }

        #endregion
    }
}
