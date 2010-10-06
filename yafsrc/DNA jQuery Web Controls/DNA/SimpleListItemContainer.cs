///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Security.Permissions;
using System.Drawing.Design;
using System.Web.UI.Design.WebControls;

namespace DNA.UI
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxItem(false)]
    public class SimpleListItemContainer : WebControl, INamingContainer, IDataItemContainer
    {
        private object dataItem;
        private int dataItemIndex;
        private int displayIndex;

        public SimpleListItemContainer(SimpleListItem item, int dataIndex)
        {
            this.dataItem = item;
            this.dataItemIndex = dataIndex;
            this.displayIndex = dataIndex;
        }

        #region IDataItemContainer Members

        public object DataItem
        {
            get { return dataItem; }
        }

        public int DataItemIndex
        {
            get { return dataItemIndex; }
        }

        public int DisplayIndex
        {
            get { return displayIndex; }
        }

        #endregion

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Li;
            }
        }
    }
}
