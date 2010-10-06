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
    public class Resize : Animate
    {
        private Unit width;
        private Unit height;

        public Unit Width
        {
            get { return width; }
            set { width = value; }
        }

        public Unit Height
        {
            get { return height; }
            set { height = value; }
        }

        public override string GetAnimationScripts()
        {
            if (!Height.IsEmpty)
            {
                if (Attributes["height"] == null)
                    Attributes.Add(new AnimationAttribute(AnimationAttributeNames.height.ToString(), Height.ToString()));
                else
                    Attributes["height"].Value = Height.ToString();
            }

            if (!Width.IsEmpty)
            {
                if (Attributes["width"] == null)
                    Attributes.Add(new AnimationAttribute(AnimationAttributeNames.width.ToString(), Width.ToString()));
                else
                    Attributes["width"].Value = Width.ToString();
            }
            return base.GetAnimationScripts();
        }
    }

}
