///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.ComponentModel.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.Design;
using System.IO;

namespace DNA.UI.JQuery.Design
{
    /// <summary>
    /// Designer of Tabs
    /// </summary>
    public class TabsDesigner : ControlDesigner
    {
        private Tabs tabs;

        /// <summary>
        /// Initialize AccordionDesigner
        /// </summary>
        public TabsDesigner() { }

        public override bool AllowResize
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Initialize the TabsDesigner
        /// </summary>
        /// <param name="component"></param>
        public override void Initialize(IComponent component)
        {
            tabs = component as Tabs;
            if (tabs == null)
                throw new ArgumentException("Component must be an Tabs control", "component");
            base.Initialize(component);
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            if (tabs.Views.Count < 0)
                return GetEmptyDesignTimeHtml();

            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            if (string.IsNullOrEmpty(tabs.CssClass))
                tabs.CssClass = "ui-tabs ui-widget ui-widget-content ui-corner-all";
            tabs.RenderBeginTag(htmlWriter);
            htmlWriter.WriteBeginTag("ul");
            htmlWriter.WriteAttribute("class", "ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all");
            htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
            for (int i = 0; i < tabs.Views.Count; i++)
            {
                View view = tabs.Views[i];
                regions.Add(new DesignerRegion(this, "view" + i.ToString(), false));
                htmlWriter.WriteBeginTag("li");
                htmlWriter.WriteAttribute(DesignerRegion.DesignerRegionAttributeName, i.ToString());

                if (view.IsSelected)
                    htmlWriter.WriteAttribute("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active");
                else
                    htmlWriter.WriteAttribute("class", "ui-state-default ui-corner-top");
                htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
                htmlWriter.WriteBeginTag("a");
                htmlWriter.WriteAttribute("href", "#");
                htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
                htmlWriter.Write(view.Text);
                htmlWriter.WriteEndTag("a");
                htmlWriter.WriteEndTag("li");
            }
            htmlWriter.WriteEndTag("ul");
            regions.Add(new EditableDesignerRegion(this, "content", false));
            htmlWriter.WriteBeginTag("div");
            //htmlWriter.WriteAttribute("style","position:;");
            htmlWriter.WriteAttribute("class", "ui-tabs-panel ui-widget-content ui-corner-bottom");
            htmlWriter.WriteAttribute(EditableDesignerRegion.DesignerRegionAttributeName, tabs.Views.Count.ToString());
            htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
            htmlWriter.WriteEndTag("div");
            tabs.RenderEndTag(htmlWriter);
            return stringBuilder.ToString();
        }

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                View view = ControlParser.ParseControl(host, content) as View;
                int selectedIndex = int.Parse(region.Name.Substring(4));
                tabs.Views.RemoveAt(selectedIndex);
                tabs.Views.AddAt(selectedIndex, view);
                tabs.SelectedIndex = selectedIndex;
            }
        }

        protected override void OnClick(DesignerRegionMouseEventArgs e)
        {
            if (e.Region == null)
                return;

            if (e.Region.Name.StartsWith("content"))
                return;

            int index = int.Parse(e.Region.Name.Substring(4));

            if (index != tabs.SelectedIndex)
            {
                tabs.SelectedIndex = index;
                UpdateDesignTimeHtml();
            }
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                // int index = int.Parse(region.Name.Substring(4));
                View view = tabs.Views[tabs.SelectedIndex];
                view.CssClass = "ui-tabs-panel ui-widget-content ui-corner-bottom";
                view.ShowHeader = false;
                return ControlPersister.PersistControl(view);
            }
            return String.Empty;
        }

        public override string GetDesignTimeHtml()
        {
            if (tabs.Views.Count < 1)
                return GetEmptyDesignTimeHtml();
            return base.GetDesignTimeHtml();
        }

        protected override string GetEmptyDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("Please add View inside Tabs");
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                lists.Add(new TabDesignerActionList(Component));
                return lists;
            }
        }
    }
}
