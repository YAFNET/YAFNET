/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core
{
    #region Using

    using System;
    using System.Data;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     The YAF load board settings.
    /// </summary>
    public class YafLoadBoardSettings : YafBoardSettings
    {
        #region Fields

        /// <summary>
        /// The _current board row.
        /// </summary>
        private DataRow _currentBoardRow;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafLoadBoardSettings"/> class.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        public YafLoadBoardSettings([NotNull] int boardID)
        {
            this._boardID = boardID;

            // get all the registry values for the forum
            this.LoadBoardSettingsFromDB();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current board row.
        /// </summary>
        protected DataRow CurrentBoardRow
        {
            get
            {
                if (this._currentBoardRow != null)
                {
                    return this._currentBoardRow;
                }

                var dataTable = YafContext.Current.GetRepository<Board>().List(this._boardID);

                if (dataTable.Rows.Count == 0)
                {
                    throw new EmptyBoardSettingException("No data for board ID: {0}".FormatWith(this._boardID));
                }

                this._currentBoardRow = dataTable.Rows[0];

                return this._currentBoardRow;
            }
        }

        /// <summary>
        /// Gets or sets the _legacy board settings.
        /// </summary>
        protected override YafLegacyBoardSettings _legacyBoardSettings
        {
            get
            {
                return base._legacyBoardSettings ?? (base._legacyBoardSettings = this.SetupLegacyBoardSettings(this.CurrentBoardRow));
            }

            set
            {
                base._legacyBoardSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the _membership app name.
        /// </summary>
        protected override string _membershipAppName
        {
            get
            {
                return base._membershipAppName ?? (base._membershipAppName = this._legacyBoardSettings.MembershipAppName);
            }

            set
            {
                base._membershipAppName = value;
            }
        }

        /// <summary>
        /// Gets or sets the _roles app name.
        /// </summary>
        protected override string _rolesAppName
        {
            get
            {
                return base._rolesAppName ?? (base._rolesAppName = this._legacyBoardSettings.RolesAppName);
            }

            set
            {
                base._rolesAppName = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Saves the whole setting registry to the database.
        /// </summary>
        public void SaveRegistry()
        {
            // loop through all values and commit them to the DB
            foreach (string key in this._reg.Keys)
            {
                LegacyDb.registry_save(key, this._reg[key]);
            }

            foreach (string key in this._regBoard.Keys)
            {
                LegacyDb.registry_save(key, this._regBoard[key], this._boardID);
            }
        }

        /// <summary>
        /// Saves just the guest user id backup setting for this board.
        /// </summary>
        public void SaveGuestUserIdBackup()
        {
            var key = "GuestUserIdBackup";

            if (this._regBoard.ContainsKey(key))
            {
                LegacyDb.registry_save(key, this._regBoard[key], this._boardID);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the board settings from database.
        /// </summary>
        protected void LoadBoardSettingsFromDB()
        {
            DataTable dataTable;

            using (dataTable = LegacyDb.registry_list())
            {
                // get all the registry settings into our hash table
                foreach (DataRow dr in dataTable.Rows)
                {
                    this._reg.Add(dr["Name"].ToString().ToLower(), dr["Value"] == DBNull.Value ? null : dr["Value"]);
                }
            }

            using (dataTable = LegacyDb.registry_list(null, this._boardID))
            {
                // get all the registry settings into our hash table
                foreach (DataRow dr in dataTable.Rows)
                {
                    this._regBoard.Add(dr["Name"].ToString().ToLower(), dr["Value"] == DBNull.Value ? null : dr["Value"]);
                }
            }
        }

        /// <summary>
        /// The setup legacy board settings.
        /// </summary>
        /// <param name="board">
        /// The board.
        /// </param>
        /// <returns>
        /// The <see cref="YafBoardSettings.YafLegacyBoardSettings"/>.
        /// </returns>
        private YafLegacyBoardSettings SetupLegacyBoardSettings([NotNull] DataRow board)
        {
            CodeContracts.VerifyNotNull(board, "board");

            var membershipAppName = board["MembershipAppName"].ToString().IsNotSet()
                                        ? YafContext.Current.Get<MembershipProvider>().ApplicationName
                                        : board["MembershipAppName"].ToString();

            var rolesAppName = board["RolesAppName"].ToString().IsNotSet()
                                   ? YafContext.Current.Get<RoleProvider>().ApplicationName
                                   : board["RolesAppName"].ToString();

            return new YafLegacyBoardSettings(
                board["Name"].ToString(), 
                Convert.ToString(board["SQLVersion"]), 
                board["AllowThreaded"].ToType<bool>(), 
                membershipAppName, 
                rolesAppName);
        }

        #endregion
    }
}