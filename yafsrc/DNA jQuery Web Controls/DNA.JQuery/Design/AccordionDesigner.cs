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
    /// The Control Designer for Accordion
    /// </summary>
    public class AccordionDesigner : CompositeControlDesigner
    {
        private Accordion _accordion;

        /// <summary>
        /// Initialize AccordionDesigner
        /// </summary>
        public AccordionDesigner() { }

        public override bool AllowResize
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Initialize the AccordionDesigner
        /// </summary>
        /// <param name="component"></param>
        public override void Initialize(IComponent component)
        {
            _accordion = component as Accordion;
            if (_accordion == null)
                throw new ArgumentException("Component must be an Accordion control", "component");
            base.Initialize(component);
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            if (_accordion.Views.Count == 0)
                return GetEmptyDesignTimeHtml();

            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            _accordion.RenderBeginTag(htmlWriter);
            for (int i = 0; i < _accordion.Views.Count; i++)
            {
                htmlWriter.WriteBeginTag("div");
                View view = _accordion.Views[i];
                //if (_accordion.SelectedIndex == view.Index)
                if (view.IsSelected)
                {
                    regions.Add(new EditableDesignerRegion(this, "view" + i.ToString(), false));
                    //if (string.IsNullOrEmpty(view.HeaderCssClass))
                    //    view.HeaderCssClass = "ui-accordion-header ui-helper-reset ui-state-active ui-corner-top";
                    htmlWriter.WriteAttribute(EditableDesignerRegion.DesignerRegionAttributeName, i.ToString());
                    htmlWriter.Write(HtmlTextWriter.TagRightChar);
                    htmlWriter.WriteEndTag("div");
                }
                else
                {
                    regions.Add(new DesignerRegion(this, "view" + i.ToString(), true));
                   // if (string.IsNullOrEmpty(view.HeaderCssClass))
                    //    view.HeaderCssClass = "ui-accordion-header ui-helper-reset ui-state-default ui-corner-all";
                    htmlWriter.WriteAttribute(DesignerRegion.DesignerRegionAttributeName, i.ToString());
                    htmlWriter.Write(HtmlTextWriter.TagRightChar);
                    view.RenderHeader(htmlWriter);
                    htmlWriter.WriteEndTag("div");
                }
            }
            _accordion.RenderEndTag(htmlWriter);
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
                _accordion.Views.RemoveAt(selectedIndex);
                _accordion.Views.AddAt(selectedIndex, view);
                _accordion.SelectedIndex = selectedIndex;
            }
        }

        protected override void OnClick(DesignerRegionMouseEventArgs e)
        {
            if (e.Region == null)
                return;

            int index = int.Parse(e.Region.Name.Substring(4));

            if (index != _accordion.SelectedIndex)
            {
                _accordion.SelectedIndex = index;
                UpdateDesignTimeHtml();
            }
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                int index = int.Parse(region.Name.Substring(4));
                View view = _accordion.Views[index];
                view.ShowHeader = true;
                //TODO: don't do this.in design time do not set the css class property
                if (string.IsNullOrEmpty(view.CssClass))
                    view.CssClass = "ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content-active";
                return ControlPersister.PersistControl(view);
            }
            return String.Empty;
        }

        public override string GetDesignTimeHtml()
        {
            if (_accordion.Views.Count==0)
                return GetEmptyDesignTimeHtml();
            return base.GetDesignTimeHtml();
        }

        protected override string GetEmptyDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("Please add View or NavView inside this Accordion");
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                lists.Add(new AccordionDesignerActionList(Component));
                return lists;
            }
        }

    }
}
