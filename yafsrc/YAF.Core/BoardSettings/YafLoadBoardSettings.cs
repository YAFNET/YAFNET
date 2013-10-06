/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     The yaf load board settings.
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
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="EmptyBoardSettingException">
        /// <c>EmptyBoardSettingException</c>.
        /// </exception>
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
        /// <exception cref="EmptyBoardSettingException">
        /// </exception>
        protected DataRow CurrentBoardRow
        {
            get
            {
                if (this._currentBoardRow == null)
                {
                    var dataTable = YafContext.Current.GetRepository<Board>().List(this._boardID);

                    if (dataTable.Rows.Count == 0)
                    {
                        throw new EmptyBoardSettingException("No data for board ID: {0}".FormatWith(this._boardID));
                    }

                    this._currentBoardRow = dataTable.Rows[0];
                }

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
        ///     The load board settings from db.
        /// </summary>
        /// <exception cref="Exception">
        /// </exception>
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

    /// <summary>
    ///     The empty board setting exception.
    /// </summary>
    public class EmptyBoardSettingException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyBoardSettingException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public EmptyBoardSettingException(string message)
            : base(message)
        {
        }

        #endregion
    }
}