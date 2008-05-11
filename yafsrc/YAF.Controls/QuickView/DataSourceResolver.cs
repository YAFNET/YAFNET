using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.ComponentModel;

namespace YAF.Controls.QuickView
{
    public class DataSourceResolver
    {
        private DataSourceResolver() { }

        public static IEnumerable ResolveDataSource(object o, string data_member)
        {
            IEnumerable ds;

            ds = o as IEnumerable;
            if (ds != null)
                return ds;

            IListSource ls = o as IListSource;

            if (ls == null)
                return null;

            IList member_list = ls.GetList();
            if (!ls.ContainsListCollection)
                return member_list;

            ITypedList tl = member_list as ITypedList;
            if (tl == null)
                return null;

            PropertyDescriptorCollection pd = tl.GetItemProperties(new PropertyDescriptor[0]);

            if (pd == null || pd.Count == 0)
                throw new HttpException("The selected data source did not contain any data members to bind to");

            PropertyDescriptor member_desc = data_member == "" ?
                pd[0] :
                pd.Find(data_member, true);

            if (member_desc != null)
                ds = member_desc.GetValue(member_list[0]) as IEnumerable;

            if (ds == null)
                throw new HttpException("A list corresponding to the selected DataMember was not found");

            return ds;
        }
    }
}
