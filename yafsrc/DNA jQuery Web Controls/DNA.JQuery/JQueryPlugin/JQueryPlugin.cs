///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Security.Permissions;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.Script;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// 	<para>JQueryPlugin is a WebContorl that manages the jQuery script libraries and
    ///     script files.It gernates the script for jQuery plugins just like this :
    ///     $(selector).name(options);</para>
    /// 	<list type="bullet">
    /// 		<item>Target property is <a href="http://wiki.dotnetage.com/JQueryServerSideSelector.ashx">JQuery Sever Side
    ///         Selector</a> to apply the jQuery plugin to one or more WebControls.</item>
    /// 		<item>Name property use to specify the jQuery plugin name.</item>
    /// 		<item>The Options collection property hold all the option parameters of the
    ///         jQuery plugin options of needs.</item>
    /// 		<item>StartEvent property tell JQueryPlugin when jQuery scripts run
    ///         at.(ApplicationLoad,ApplicationInit,DocumentDeady)</item>
    /// 	</list>
    /// </summary>
    /// <remarks>
    /// 	<para>There are some suggestions of using jQuery plugin(s) in ASP.NET though
    ///     JQueryPlugin WebControl</para>
    /// 	<list type="bullet">
    /// 		<item></item>
    /// 		<item>If the jQuery plugin has a few options i suggest to use JQueryPlugin
    ///         WebControl this is the best choice for you.</item>
    /// 		<item>When the jQuery plugins option has over more then 10,and the plugin has
    ///         it own HtmlElements or need some server side function supports i suggest to
    ///         using the DNA.UI framework to write a jQuery UI WebControl for this jQuery
    ///         plugin.</item>
    /// 		<item>When the jQuery plugin options has over more than 10 and it has none ui
    ///         but it using frequently,i suggest to write the jQuery None UI WebControl by
    ///         using DNA.UI framework.</item>
    /// 	</list>
    /// 	<para></para>
    /// </remarks>
    /// <example>
    /// 	<code lang="ASP.NET" title="Apply jQuery.ui.resizable to a Panel Control">
    /// 		<![CDATA[
    /// <asp:Panel id="Panel1" runat="server"></asp:Panel>
    /// <DotNetAge:JQueryPlugin runat="Server"
    ///   ID="ResizablePlugin"
    ///   Name="resizable"
    /// >
    ///   <Target TargetID="Panel1" />
    ///   <PlugInScripts>
    ///     <Script Name="jQuery.ui.resizable" Assembly="jQuery" />
    ///   </PlugInScripts>
    /// </DotNetAge:JQueryPlugin>]]>
    /// 	</code>
    /// </example>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:JQueryPlugin runat=\"server\" ID=\"JQueryPlugin1\"></{0}:JQueryPlugin>")]
    [System.Drawing.ToolboxBitmap(typeof(JQueryPlugin), "JQueryPlugin.JQueryPlugin.ico")]
    [ParseChildren(true)]
    [Designer(typeof(Design.NoneUIControlDesigner))]
    public class JQueryPlugin : Control
    {
        private StateManagedObjectCollection<JQueryOption> options;
        private ScriptReferenceCollection scripts;
        private string name = "";
        private JQuerySelector target = new JQuerySelector();

        /// <summary>
        /// Gets/Sets the event which the plugin start in.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the event which the plugin start in.")]
        public ClientRegisterEvents StartEvent { get; set; }

        /// <summary>
        /// Gets/Sets the plugin name that jQuery script using
        /// </summary>
        [Description("Gets/Sets the plugin name that jQuery script using")]
        [Category("Action")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets/Sets the which control plugin apply to. 
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Action")]
        public JQuerySelector Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        /// <summary>
        /// Gets/Sets the options of the plugin
        /// </summary>
        [Description("Gets/Sets the options of the plugin")]
        [Category("Data")]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public StateManagedObjectCollection<JQueryOption> Options
        {
            get
            {
                if (options == null)
                {
                    options = new StateManagedObjectCollection<JQueryOption>();
                    if (EnableViewState)
                        ((IStateManager)options).TrackViewState();
                }
                return options;
            }
        }

        /// <summary>
        /// Gets/Sets the scriptreferences of the plugin use
        /// </summary>
        [Description("Gets/Sets the scriptreferences of the plugin use")]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ScriptReferenceCollection PlugInScripts
        {
            get
            {
                if (scripts == null)
                    scripts = new ScriptReferenceCollection();
                return scripts;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (!DesignMode)
            {
                ClientScriptManager.RegisterJQuery(this);
                RegisterScriptReferences();
            }
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!DesignMode)
            {
                //string _target = ClientScriptManager.GetControlClientID(Page, TargetID);
                if (target.IsEmpty)
                    throw new Exception("The Target property can not be Empty");

                if (string.IsNullOrEmpty(Name))
                    throw new Exception("Plugin Name can not be empty.Please set the plugin Name first!");

                StringBuilder scripts = new StringBuilder();
                scripts.Append(target.ToString(Page) + "." + name + "(");
                if (Options.Count > 0)
                    scripts.Append("{" + OptionsToString() + "}");
                scripts.Append(");");
                switch (StartEvent)
                {
                    case ClientRegisterEvents.ApplicaitonInit:
                        ClientScriptManager.RegisterClientApplicationInitScript(this, scripts.ToString());
                        break;
                    case ClientRegisterEvents.ApplicationLoad:
                        ClientScriptManager.RegisterClientApplicationLoadScript(this, scripts.ToString());
                        break;
                    case ClientRegisterEvents.DocumentReady:
                        ClientScriptManager.RegisterDocumentReadyScript(this, scripts.ToString());
                        break;
                }
            }

        }

        protected virtual string OptionsToString()
        {
            StringBuilder ops = new StringBuilder();
            foreach (JQueryOption option in options)
            {
                if (ops.Length > 0)
                    ops.Append(",");

                switch (option.Type)
                {
                    case JavaScriptTypes.String:
                        ops.Append(option.Name + ":'" + option.Value + "'");
                        break;
                    case JavaScriptTypes.Array:
                        if (!option.Value.StartsWith("["))
                            ops.Append(option.Name + ":[" + option.Value + "]");
                        else
                            ops.Append(option.Name + ":" + option.Value);
                        break;
                    case JavaScriptTypes.Boolean:
                        ops.Append(option.Name + ":" + option.Value.ToLower());
                        break;
                    case JavaScriptTypes.Function:
                        ops.Append(option.Name + ":" + ClientScriptManager.FormatFunctionString(option.Value));
                        break;
                    default:
                        ops.Append(option.Name + ":" + option.Value);
                        break;
                }
            }
            return ops.ToString();
        }

        protected virtual void RegisterScriptReferences()
        {
            foreach (ScriptReference scriptRef in PlugInScripts)
                ClientScriptManager.AddCompositeScript(this, scriptRef);
        }
    }
}
