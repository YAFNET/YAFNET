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
using System.Security.Permissions;

namespace YAF.Controls.QuickView
{

    // CAS
    [AspNetHostingPermissionAttribute(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermissionAttribute(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class QuickViewItem : WebControl, INamingContainer, IDataItemContainer
    {
        #region Fields
        private object _dataItem;
        private int dataset_index;
        private int item_index;
        #endregion	// Fields


        #region Public Instance Properties

        public virtual object DataItem
        {
            get
            {
                return _dataItem;
            }

            set
            {
                _dataItem = value;
            }
        }

        public virtual int DataSetIndex
        {
            get
            {
                return dataset_index;
            }
        }

        public virtual int ItemIndex
        {
            get
            {
                return item_index;
            }
        }

        #endregion	// Public Instance Properties

        #region IDataItemContainer Properties
        object IDataItemContainer.DataItem
        {
            get { return _dataItem; }
        }

        int IDataItemContainer.DataItemIndex
        {
            get { return item_index; }
        }

        int IDataItemContainer.DisplayIndex
        {
            get { return item_index; }
        }
        #endregion	// IDataItemContainer Properties

    }
}
