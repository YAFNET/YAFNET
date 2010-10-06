///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Security.Permissions;
using System.Drawing.Design;

namespace DNA.UI.JQuery
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NavItem:SimpleListItem
    {
        private int index;

        /// <summary>
        /// Gets the index for NavItem
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Gets/Sets the NavItem client client event handler
        /// </summary>
        [Category("Behavior")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Description("Gets/Sets the NavItem client client event handler")]
        public string OnClientClick
        {
            get
            {
                Object obj = ViewState["OnClientClick"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["OnClientClick"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the user data 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Data
        {
            get
            {
                Object obj = ViewState["Data"];
                return (obj == null) ? null : obj;
            }
            set
            {
                ViewState["Data"] = value;
            }
        }

        public NavItem() { }

        public NavItem(string text) { Text = text; }

        public NavItem(string text, string navigateUrl) { Text = text; NavigateUrl = navigateUrl; }

        public NavItem(string text, string naviageUrl, string imageUrl) { Text = text; NavigateUrl = naviageUrl; ImageUrl = imageUrl; }
    }
}
