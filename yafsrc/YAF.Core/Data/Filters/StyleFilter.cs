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

    /// <summary>
    /// The style filter.
    /// </summary>
    public class StyleFilter : IDbDataFilter
    {
        #region Fields

        /// <summary>
        /// The _board settings.
        /// </summary>
        private readonly YafBoardSettings _boardSettings;

        /// <summary>
        /// The _style transform.
        /// </summary>
        private readonly IStyleTransform _styleTransform;

        /// <summary>
        /// The _styled nick operations.
        /// </summary>
        private readonly string[] _styledNickOperations = new[]
            {
                "active_list", 
                "active_listtopic", 
                "active_listforum", 
                "forum_moderators", 
                "topic_latest", 
                "shoutbox_getmessages", 
                "topic_latest", 
                "active_list_user", 
                "admin_list"
            };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleFilter"/> class.
        /// </summary>
        /// <param name="styleTransform">
        /// The style transform.
        /// </param>
        /// <param name="boardSettings">
        /// The board settings.
        /// </param>
        public StyleFilter(IStyleTransform styleTransform, YafBoardSettings boardSettings)
        {
            this._styleTransform = styleTransform;
            this._boardSettings = boardSettings;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public int SortOrder
        {
            get
            {
                return 100;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The is supported operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSupportedOperation(string operationName)
        {
            return this._styledNickOperations.Contains(operationName.ToLower());
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="dbfunctionType">
        /// The dbfunction type.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public void Run(DbFunctionType dbfunctionType, string operationName, IEnumerable<KeyValuePair<string, object>> parameters, object data)
        {
            if (this._styledNickOperations.Contains(operationName.ToLower()) && this._boardSettings.UseStyledNicks)
            {
                bool colorOnly = false;

                if (dbfunctionType == DbFunctionType.DataTable)
                {
                    var dataTable = (DataTable)data;
                    this._styleTransform.DecodeStyleByTable(dataTable, colorOnly);
                }
            }
        }

        #endregion
    }
}