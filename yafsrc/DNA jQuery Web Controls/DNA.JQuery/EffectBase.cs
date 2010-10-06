using System.ComponentModel;
using System.Security.Permissions;
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System.Web;
using System.Web.UI;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [Designer(typeof(Design.NoneUIControlDesigner))]
    public abstract class EffectBase:Control
    {
        //private string targetID = "";
        private Speeds speed = Speeds.Normal;
        private int speedValue = -1;
        private DomEvents triggerEvent = DomEvents.Click;
        //private string triggerID = "";
        private JQuerySelector target;
        private JQuerySelector trigger;

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

        /// <summary>
        /// Gets/Sets which control that the effect apply to
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
                if (target == null)
                    target = new JQuerySelector();
                return target;
            }
            set
            {
                target = value;
            }
        }

        /// <summary>
        /// Gets/Sets which control to trigger the effect
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Category("Action")]
        public JQuerySelector Trigger
        {
            get
            {
                if (trigger == null)
                    trigger = new JQuerySelector();
                return trigger;
            }
            set
            {
                trigger = value;
            }
        }

        /// <summary>
        /// Gets/Sets the trigger event type
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the trigger event type")]
        public DomEvents TriggerEvent
        {
            get { return triggerEvent; }
            set { triggerEvent = value; }
        }

        protected string GetSpeed()
        {
            if (speedValue == -1)
                return "'" + Speed.ToString().ToLower() + "'";
            return speedValue.ToString();
        }
    }
}
