///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// The AnimationAttribute
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AnimationAttribute : NameValueAttribute
    {
        private AnimationAttributeNames animationType = AnimationAttributeNames.opacity;

        public AnimationAttribute() { }

        public AnimationAttribute(string name, string value) : base(name, value) { }

        /// <summary>
        /// Gets/Sets the AnimationType
        /// </summary>
        [Category("Behavior")]
        [Description(" Gets/Sets the AnimationType")]
        public AnimationAttributeNames AnimationType
        {
            get
            {
                return animationType;
            }
            set
            {
                Name = value.ToString();
                animationType = value;
            }
        }
    }
}
