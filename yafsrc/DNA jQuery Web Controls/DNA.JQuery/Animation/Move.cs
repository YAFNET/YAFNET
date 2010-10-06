///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
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
    public class Move : Animate
    {
        private Unit left;
        private Unit top;

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

        public override string GetAnimationScripts()
        {
            if (!Left.IsEmpty)
            {
                if (Attributes["left"] == null)
                    Attributes.Add(new AnimationAttribute(AnimationAttributeNames.left.ToString(), left.ToString()));
                else
                    Attributes["left"].Value = left.ToString();
            }

            if (!Top.IsEmpty)
            {
                if (Attributes["top"] == null)
                    Attributes.Add(new AnimationAttribute(AnimationAttributeNames.top.ToString(), top.ToString()));
                else
                    Attributes["top"].Value = top.ToString();
            }
            return base.GetAnimationScripts();
        }
    }

}