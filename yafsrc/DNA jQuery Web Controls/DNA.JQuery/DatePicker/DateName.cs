///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// DateName object reparents the name of the date
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DateName
    {
        private string name;

        /// <summary>
        /// Gets/Sets the name of the date
        /// </summary>
        [Category("Data")]
        [Description("Gets/Sets the name of the date")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
