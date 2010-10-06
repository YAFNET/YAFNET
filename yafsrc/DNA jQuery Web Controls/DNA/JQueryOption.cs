
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
 
using System.Web;
using System.Security.Permissions;

namespace DNA.UI
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class JQueryOption
    {
        private string name;
        private string _value = null;
        private JQueryOptionTypes type = JQueryOptionTypes.Value;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public JQueryOptionTypes Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
