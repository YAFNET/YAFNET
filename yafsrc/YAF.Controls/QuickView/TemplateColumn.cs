using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;

namespace YAF.Controls.QuickView
{
    public class TemplateColumn : QuickViewColumn
    {
        private ITemplate footerTemplate, headerTemplate, itemTemplate;


        public override void InitializeControl(Control control)
        {
            // base.InitializeControl(control);

            ItemTemplate.InstantiateIn(control);

        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainerAttribute(typeof(QuickViewItem))]
        public virtual ITemplate FooterTemplate
        {
            get { return footerTemplate; }
            set { footerTemplate = value; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainerAttribute(typeof(QuickViewItem))]
        public virtual ITemplate HeaderTemplate
        {
            get { return headerTemplate; }
            set { headerTemplate = value; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainerAttribute(typeof(QuickViewItem))]
        public virtual ITemplate ItemTemplate
        {
            get { return itemTemplate; }
            set { itemTemplate = value; }
        }
    }
}
