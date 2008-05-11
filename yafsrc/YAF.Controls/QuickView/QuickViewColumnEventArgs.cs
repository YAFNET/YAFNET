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
    public sealed class QuickViewCommandEventArgs : CommandEventArgs
    {
        QuickViewItem _item;
        object _source;

        public QuickViewCommandEventArgs(QuickViewItem item, object source, CommandEventArgs args)
            : base(args)
        {
            this._item = item;
            this._source = source;
        }

        public QuickViewItem Item
        {
            get { return _item; }
        }

        public object CommandSource
        {
            get { return _source; }
        }
    }
}
