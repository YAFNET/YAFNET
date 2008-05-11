using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace YAF.Controls.QuickView
{
    public class QuickViewItemEventArgs : EventArgs
    {
        QuickViewItem _item;

        public QuickViewItemEventArgs(QuickViewItem item)
        {
            this._item = item;
        }

        public QuickViewItem Item
        {
            get { return _item; }
        }
    }
}
