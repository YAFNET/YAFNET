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
    public class NavItemEventArgs:EventArgs
    {
        private NavItem _item;
        
        public NavItemEventArgs(NavItem item) { _item = item; }

        public NavItem Item
        {
            get { return _item; }
        }
    }

    public delegate void NavItemEventHandler(object sender,NavItemEventArgs e);
}
