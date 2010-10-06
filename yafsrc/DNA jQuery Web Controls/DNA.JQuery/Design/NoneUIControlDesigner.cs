///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;

namespace DNA.UI.JQuery.Design
{
    public class NoneUIControlDesigner : ControlDesigner
    {
        private IComponent noneUIControl;

        public override string GetDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml();
        }

        /// <summary>
        /// Initialize the NonUIControlDesigner
        /// </summary>
        /// <param name="component"></param>
        public override void Initialize(IComponent component)
        {
            noneUIControl = component;
            base.Initialize(component);
        }

        public override string GetPersistInnerHtml()
        {
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
                return ControlPersister.PersistInnerProperties(noneUIControl, host);
            return String.Empty;
        }
    }
}
