///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Security.Permissions;
using System.ComponentModel;
using System.Web.UI.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Web.UI.Design.WebControls;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Effect runat=\"server\" ID=\"Effect1\"></{0}:Effect>")]
    [ParseChildren(true, DefaultProperty = "Attributes")]
    [System.Drawing.ToolboxBitmap(typeof(Effect), "Effect.Effect.ico")]
    public class Effect:EffectBase
    {
        private string onClientCallBack = "";
        private JQueryEffects effectType = JQueryEffects.Slide;
        private NameValueAttributeCollection<NameValueAttribute> attributes = null;
        private JQueryEffectMethods effectMethod = JQueryEffectMethods.none;

        /// <summary>
        /// Gets/Sets the effect method
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the effect method")]
        [Bindable(true)]
        public JQueryEffectMethods EffectMethod
        {
            get { return effectMethod; }
            set { effectMethod = value; }
        }

        /// <summary>
        /// Gets/Sets the effect type
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the effect type")]
        [Bindable(true)]
        public JQueryEffects EffectType
        {
            get { return effectType; }
            set { effectType = value; }
        }

        /// <summary>
        /// Gets/Sets the Effect client call back event handler
        /// </summary>
        [Category("ClientEvents")]
        [Description("Gets/Sets the Effect client call back event handler")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        public string OnClientCallBack
        {
            get { return onClientCallBack; }
            set { onClientCallBack = value; }
        }

        /// <summary>
        /// Gets the Attribute Collection of the Animation
        /// </summary>
        [Category("Data")]
        [Description("Gets the Attribute Collection of the Effect")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public NameValueAttributeCollection<NameValueAttribute> Attributes
        {
            get { return attributes; }
            private set { attributes = value; }
        }

        public Effect()
        {
            attributes = new NameValueAttributeCollection<NameValueAttribute>();
            if (EnableViewState)
                ((IStateManager)attributes).TrackViewState();
        }

        protected override void OnPreRender(EventArgs e)
        {
            //string clientID = ClientScriptManager.GetControlClientID(Page, TargetID);
            //string trigger = ClientScriptManager.GetControlClientID(Page, TriggerID);

            StringBuilder scripts = new StringBuilder();

            if (!Target.IsEmpty)
            {
                scripts.Append(Target.ToString(Page));
                //scripts.AppendLine("jQuery('#" + clientID + "')");
                
                if (effectMethod != JQueryEffectMethods.none)
                    scripts.Append("." + effectMethod.ToString() + "('" + Speed.ToString().ToLower() + "')");
                
                if (effectType != JQueryEffects.None)
                {
                    scripts.Append(".effect(" + GetEffect());

                    if (Attributes.Count > 0)
                        scripts.Append("," + Attributes.ToJSONString());
                    else
                        scripts.Append(",null");

                    scripts.Append("," + GetSpeed());

                    if (!string.IsNullOrEmpty(onClientCallBack))
                        scripts.Append("," + ClientScriptManager.FormatFunctionString(onClientCallBack));
                    scripts.Append(")");
                }
            }

            
            if (scripts.Length > 0)
            {
                if (!scripts.ToString().EndsWith(";"))
                    scripts.Append(";");

                ClientScriptManager.RegisterJQuery(this);
                ClientScriptManager.AddCompositeScript(this, "jQueryNet.effects.core.js", "jQueryNet");
                ClientScriptManager.AddCompositeScript(this, "jQueryNet.effects." + effectType.ToString().ToLower() + ".js", "jQueryNet");
                //ScriptManager.GetCurrent(Page).CompositeScript.Scripts.Add(new ScriptReference("jQuery.effects.core.js", "jQuery"));
                //ScriptManager.GetCurrent(Page).CompositeScript.Scripts.Add(new ScriptReference("jQuery.effects."+effectType.ToString().ToLower()+".js", "jQuery"));
                if (!Trigger.IsEmpty)
                {
                    scripts.Insert(0, Trigger.ToString(Page)+".bind('" + TriggerEvent.ToString().ToLower() + "',function(){");
                    scripts.Append("});");
                }
                ClientScriptManager.RegisterClientApplicationLoadScript(this, scripts.ToString());
            }

          //  base.OnPreRender(e);
        }
        
        private string GetEffect()
        {
            return "'" + effectType.ToString().ToLower() + "'";
        }

    }
}
