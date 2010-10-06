///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;


namespace DNA.UI.JQuery.Design
{
    internal class TabDesignerActionList : DesignerActionList
    {
        protected DesignerActionItemCollection items;

        public TabDesignerActionList(IComponent component) : base(component) { }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            if (items == null)
            {
                items = new DesignerActionItemCollection();
                items.Add(new DesignerActionMethodItem(this,"AddView","Add View","Behavior"));
            }
            return items;
        }

        public void AddView()
        {
            TransactedChangeCallback addNewCallback = new TransactedChangeCallback(DoAddView);
            ControlDesigner.InvokeTransactedChange(Component,addNewCallback, "AddView", "Add new View");
        }

  
        public bool DoAddView(object args)
        {
            Tabs viewControl = Component as Tabs;
            View newView = new View();
            newView.Text = "View" + viewControl.Views.Count.ToString();
            newView.ID = "View" + viewControl.Views.Count.ToString();
            viewControl.Views.Add(newView);
            return true;
        }
    }
}
