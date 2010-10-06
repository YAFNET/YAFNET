///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.UI.JQuery.Design
{
    public class NavItemCollectionEditor:DNA.UI.Design.ItemCollectionEditor
    {
        public NavItemCollectionEditor(Type type) : base(type) { }

        protected override Type CreateCollectionItemType()
        {
            return typeof(NavItem);
        }

        protected override string GetDisplayText(object value)
        {
            NavItem item = value as NavItem;
            if (item != null)
                if (!string.IsNullOrEmpty(item.Text))
                    return item.Text;
            return base.GetDisplayText(value);
        }
    }
}
