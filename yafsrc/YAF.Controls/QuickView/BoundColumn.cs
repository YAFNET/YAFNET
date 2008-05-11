using System;
using System.Collections.Generic;
using System.Text;

namespace YAF.Controls.QuickView
{
    public class BoundColumn : QuickViewColumn
    {
        private string _dataField;

        public string DataField
        {
            get { return _dataField; }
            set { _dataField = value; }
        }
    }
}
