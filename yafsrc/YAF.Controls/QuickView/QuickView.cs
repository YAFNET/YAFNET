/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.Text;
using System.IO;
using System.Data;
using System.Web.Util;



namespace YAF.Controls.QuickView
{
    /// <summary>
    /// QuickView Control Summary
    /// </summary>
    public class QuickView : BaseDataList, INamingContainer
    {
        #region Instance Variables

        private ArrayList _columnslist;
        private QuickViewColumn[] _renderColumns;
        private QuickViewColumnCollection _columns;

        private ArrayList _qvItems = new ArrayList();

        

        private static readonly object _itemCreatedEvent = new object();
        private static readonly object _itemDataBoundEvent = new object();

        #endregion

        [DefaultValue(null)]
        [MergableProperty(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual QuickViewColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columnslist = new ArrayList();
                    _columns = new QuickViewColumnCollection(this, _columnslist);
                    if (IsTrackingViewState)
                    {
                        IStateManager manager = (IStateManager)_columns;
                        manager.TrackViewState();
                    }
                }
                return _columns;
            }
        }


        /// <summary>
        /// Summary
        /// </summary>
        private void CreateRenderColumns(PagedDataSource paged, bool useDataSource)
        {

            _renderColumns = new QuickViewColumn[Columns.Count];
            Columns.CopyTo(_renderColumns, 0);
        }

        /// <summary>
        /// Summary
        /// </summary>
        protected override void CreateControlHierarchy(bool useDataSource)
        {
            IEnumerable dataSource;

            if (useDataSource)
            {
                dataSource = DataSourceResolver.ResolveDataSource(this.DataSource, this.DataMember);
                foreach (object dataItem in dataSource)
                {
                    this.CreateQuickViewItem(dataItem, useDataSource);
                }
            }
        }

        /// <summary>
        /// Summary
        /// </summary>
        protected void CreateQuickViewItem(object dataItem, bool databind)
        {
            QuickViewItem itm = new QuickViewItem();
            if (databind)
                itm.DataItem = dataItem;

            QuickViewItemEventArgs e = new QuickViewItemEventArgs(itm);

            foreach (QuickViewColumn dvc in this.Columns)
            {
                switch (dvc.GetType().Name)
                {
                    case "TemplateColumn":
                        ((TemplateColumn)dvc).ItemTemplate.InstantiateIn(itm);
                        break;
                }
            }

            //
            // It is very important that this be called *before* data
            // binding. Otherwise, we won't save our state in the viewstate.
            //
            // Controls.Add(itm);
            if (2 != -1)
                _qvItems.Add(itm);

            OnItemCreated(e);

            if (databind)
            {
                itm.DataBind();
                OnItemDataBound(e);
                itm.DataItem = null;
            }
        }

        /// <summary>
        /// Summary
        /// </summary>
        protected virtual void OnItemCreated(QuickViewItemEventArgs e)
        {
            QuickViewItemEventHandler itemCreated = (QuickViewItemEventHandler)Events[_itemCreatedEvent];
            if (itemCreated != null)
                itemCreated(this, e);
        }

        /// <summary>
        /// Summary
        /// </summary>
        protected virtual void OnItemDataBound(QuickViewItemEventArgs e)
        {
            QuickViewItemEventHandler itemDataBound = (QuickViewItemEventHandler)Events[_itemDataBoundEvent];
            if (itemDataBound != null)
                itemDataBound(this, e);
        }

        /// <summary>
        /// Summary
        /// </summary>
        protected override void PrepareControlHierarchy()
        {
            // I have no idea what to do with this method??
            for (int i = 1; i <= 10; i++)
            {
                this.Controls.Add((QuickViewItem)_qvItems[i]); // Add Control to Parent
            }
        }
    }
}
