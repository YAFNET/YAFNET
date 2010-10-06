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
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Drawing.Design;
using System.Collections.Generic;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:Animation runat=\"server\" ID=\"Animation1\"></{0}:Animation>")]
    [System.Drawing.ToolboxBitmap(typeof(Animation), "Animation.Animation.ico")]
    [ParseChildren(true)]
    public class Animation : EffectBase
    {
        private List<Animate> animates;

        /// <summary>
        /// Gets/Sets the animation sequence of this animation
        /// </summary>
        [Category("Action")]
        [Description("Gets the Attribute Collection of the Animation")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Animate> Animates
        {
            get
            {
                if (animates == null)
                    animates = new List<Animate>();
                return animates;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Animates.Count > 0)
            {
                StringBuilder scripts = new StringBuilder();
                if (!Target.IsEmpty)
                    scripts.Append(Target.ToString(Page));

                foreach (Animate a in Animates)
                    scripts.Append(a.GetAnimationScripts());

                //if (!string.IsNullOrEmpty(targetFunction))
                //{
                //    scripts.Append("." + targetFunction+"()" );//+ "(function(){");
                //    //scripts.Append("jQuery(this)");
                //}

                //scripts.Append(".animate(" + Attributes.ToJSONString() + "," + GetSpeed());

                //if (Easing != EasingMethods.linear)
                //    scripts.Append(",'"+easing.ToString()+"'");

                //if (!string.IsNullOrEmpty(OnClientCallBack))
                //    scripts.Append("," + ClientScriptManager.FormatFunctionString(OnClientCallBack));

                //scripts.Append(");");

                //if (scripts.Length > 0)
                //{
                if (!Trigger.IsEmpty)
                {
                    scripts.Insert(0, Trigger.ToString(Page) + ".bind('" + TriggerEvent.ToString().ToLower() + "',function(){");
                    scripts.Append("});");
                }

                ClientScriptManager.RegisterJQuery(this);

                //if (Easing != EasingMethods.linear)
                ClientScriptManager.AddCompositeScript(this, new ScriptReference("jQueryNet.plugins.easing.1.3.js", "jQueryNet"));
                ClientScriptManager.RegisterClientApplicationLoadScript(this, scripts.ToString());
                //}

                //base.OnPreRender(e);
            }
        }

    }
}
