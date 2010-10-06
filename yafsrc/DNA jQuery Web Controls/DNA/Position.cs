///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using System.Web.UI.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Web.UI.Design.WebControls;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// Position is the class define the control's bounds
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class Position
    {
        private Unit left;
        private Unit top;
        private Unit bottom;
        private Unit right;

        public Unit Bottom
        {
            get { return bottom; }
            set { bottom = value; }
        }

        public Unit Right
        {
            get { return right; }
            set { right = value; }
        }

        public Unit Left
        {
            get { return left; }
            set { left = value; }
        }

        public Unit Top
        {
            get { return top; }
            set { top = value; }
        }

        public override string ToString()
        {
            System.Collections.ArrayList ap = new System.Collections.ArrayList();
            if (!Top.IsEmpty)
                ap.Add("top:"+top.Value.ToString());
            if (!Left.IsEmpty)
                ap.Add("left:"+left.Value.ToString());
            if (!Right.IsEmpty)
                ap.Add("right:"+right.Value.ToString());
            if (!Bottom.IsEmpty)
                ap.Add("bottom:"+Bottom.Value.ToString());
            
            if (ap.Count > 0)
               return "{"+string.Join(",",(string[])ap.ToArray(typeof(string))) + "}";
            
            return String.Empty;
        }
    }
}
