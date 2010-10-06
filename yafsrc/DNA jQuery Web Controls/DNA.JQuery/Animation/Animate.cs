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
    [ParseChildren(true)]
    public class Animate
    {
        private NameValueAttributeCollection<AnimationAttribute> attributes = null;
        private EasingMethods easing = EasingMethods.linear;
        private List<Animate> animates;
        private Speeds speed = Speeds.Normal;
        private int speedValue = -1;

        /// <summary>
        /// Gets/Sets the easing plugin Method of the Animation
        /// </summary>
        [Category("Action")]
        [Description("Gets/Sets the easing plugin Method of the Animation")]
        [Bindable(true)]
        public virtual EasingMethods Easing
        {
            get { return easing; }
            set { easing = value; }
        }

        /// <summary>
        /// Gets the Attribute Collection of the Animation
        /// </summary>
        [Category("Data")]
        [Description("Gets the Attribute Collection of the Animation")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual NameValueAttributeCollection<AnimationAttribute> Attributes
        {
            get
            {
                if (attributes == null)
                    InitAttributes();
                return attributes;
            }
        }

        [Category("ClientEvents")]
        [Description(" Gets/Sets the callback client event handler.")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Bindable(true)]
        public string OnClientCallBack { get; set; }

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
        public virtual List<Animate> Animates
        {
            get
            {
                if (animates == null)
                    animates = new List<Animate>();
                return animates;
            }
        }

        private void InitAttributes()
        {
            attributes = new NameValueAttributeCollection<AnimationAttribute>();
            //if (EnableViewState)
            // (IStateManager)attributes).TrackViewState();
        }

        public virtual string GetAnimationScripts()
        {
            StringBuilder scripts = new StringBuilder();
            scripts.Append(".animate(" + Attributes.ToJSONString() + "," + GetSpeed());

            if (Easing != EasingMethods.linear)
                scripts.Append(",'" + easing.ToString() + "'");

            if (!string.IsNullOrEmpty(OnClientCallBack))
                scripts.Append("," + ClientScriptManager.FormatFunctionString(OnClientCallBack));

            scripts.Append(")");

            foreach (Animate ani in Animates)
                scripts.Append(ani.GetAnimationScripts());

            return scripts.ToString();
        }


        /// <summary>
        /// Gets/Sets the animation/effect speed value
        /// </summary>
        [Category("Data")]
        [Description("Gets/Sets the animation/effect speed value")]
        [Bindable(true)]
        public int SpeedValue
        {
            get { return speedValue; }
            set { speedValue = value; }
        }

        /// <summary>
        /// Gets/Sets the animation/effect speed value by string
        /// </summary>
        [Category("Data")]
        [Description("Gets/Sets the animation/effect speed value by string")]
        [Bindable(true)]
        public Speeds Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        protected string GetSpeed()
        {
            if (speedValue == -1)
                return "'" + Speed.ToString().ToLower() + "'";
            return speedValue.ToString();
        }

    }
}
