
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace DNA.UI
{
    /// <summary>
    ///  Name Value pair attribute object
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NameValueAttribute:StateManagedObject
    {
        public NameValueAttribute() { }

        public NameValueAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets/Sets the name of the attribute
        /// </summary>
        [Description("Gets/Sets the name of the attribute")]
        [Category("Data")]
        public virtual string Name
        {
            get
            {
                Object obj = ViewState["Name"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["Name"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the value of the attribute
        /// </summary>
        [Description("Gets/Sets the value of the attribute")]
        [Category("Data")]
        public string Value
        {
            get
            {
                Object obj = ViewState["Value"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["Value"] = value;
            }
        }
    }
}
