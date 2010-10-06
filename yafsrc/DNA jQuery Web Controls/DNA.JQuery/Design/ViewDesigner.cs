///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.Design;

namespace DNA.UI.JQuery.Design
{
    public class ViewDesigner : ControlDesigner
    {
        private View _view;
 
        public override void Initialize(IComponent component)
        {
            //判断父类控件component.Site.Container

            _view = component as View;
            if (_view == null)
                throw new ArgumentException("Component must be an View Control", "component");
            base.Initialize(component);
        }

        public override bool AllowResize
        {
            get
            {
                return false;
            }
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            EditableDesignerRegion region = new EditableDesignerRegion(this, "viewContent", false);
            region.EnsureSize = true;
            region.Selectable = true;
            regions.Add(region);
            return base.GetDesignTimeHtml(regions);
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                StringBuilder html = new StringBuilder();
                for (int i = 0; i < _view.Controls.Count; i++)
                    html.Append(ControlPersister.PersistControl(_view.Controls[i], host));
                return html.ToString();
            }
            return String.Empty;
        }

        protected override void OnClick(DesignerRegionMouseEventArgs e)
        {
            if (e.Region == null)
                return;

            if (e.Region.Name.EndsWith("viewHeader"))
                return;
            
            e.Region.Highlight = true;
            e.Region.Selected = true;
            UpdateDesignTimeHtml();
        }

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
               Control[] controls=ControlParser.ParseControls(host, content);

               _view.Controls.Clear();
               foreach (Control ctrl in controls)
                   _view.Controls.Add(ctrl);
            }
        }
    }
}
