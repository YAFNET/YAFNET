/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Core.Data.Filters
{
    #region Using

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Classes;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    public class StyleFilter : IDbDataFilter
    {
        #region Fields

        private readonly YafBoardSettings _boardSettings;

        private readonly IStyleTransform _styleTransform;

        private readonly string[] _styledNickOperations = new[]
            {
                "active_list",
                "forum_moderators",
                "topic_latest",
                "shoutbox_getmessages",
                "topic_latest",
                "active_list_user",
                "admin_list"
            };

        #endregion

        #region Constructors and Destructors

        public StyleFilter(IStyleTransform styleTransform, YafBoardSettings boardSettings)
        {
            this._styleTransform = styleTransform;
            this._boardSettings = boardSettings;
        }

        #endregion

        #region Public Properties

        public IServiceLocator ServiceLocator { get; set; }

        public int SortOrder
        {
            get
            {
                return 100;
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool IsSupportedOperation(string operationName)
        {
            return this._styledNickOperations.Contains(operationName.ToLower());
        }

        public void Run(DbFunctionType dbfunctionType, string operationName, IEnumerable<KeyValuePair<string, object>> parameters, object data)
        {
            if (dbfunctionType == DbFunctionType.DataTable && this._styledNickOperations.Contains(operationName.ToLower()) && this._boardSettings.UseStyledNicks)
            {
                bool colorOnly = false;
                var dataTable = (DataTable)data;
                this._styleTransform.DecodeStyleByTable(dataTable, colorOnly);
            }
        }

        #endregion
    }
}