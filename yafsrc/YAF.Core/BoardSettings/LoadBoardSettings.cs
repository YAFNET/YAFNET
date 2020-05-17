/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     The Load board settings.
    /// </summary>
    public sealed class LoadBoardSettings : BoardSettings
    {
        #region Fields

        /// <summary>
        /// The current board.
        /// </summary>
        private Board currentBoard;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBoardSettings"/> class.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        public LoadBoardSettings([NotNull] int boardId)
        {
            this.BoardId = boardId;

            // get all the registry values for the forum
            this.LoadBoardSettingsFromDB();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the legacy board settings.
        /// </summary>
        protected override LegacyBoardSettings LegacySettings
        {
            get => base.LegacySettings ?? (base.LegacySettings = SetupLegacyBoardSettings(this.CurrentBoard));

            set => base.LegacySettings = value;
        }

        /// <summary>
        /// Gets or sets the _membership app name.
        /// </summary>
        /*protected override string membershipAppName
        {
            get => base.membershipAppName ?? (base.membershipAppName = this.LegacySettings.MembershipAppName);

            set => base.membershipAppName = value;
        }

        /// <summary>
        /// Gets or sets the _roles app name.
        /// </summary>
        protected override string rolesAppName
        {
            get => base.rolesAppName ?? (base.rolesAppName = this.LegacySettings.RolesAppName);

            set => base.rolesAppName = value;
        }*/

        /// <summary>
        /// Gets the current board.
        /// </summary>
        private Board CurrentBoard
        {
            get
            {
                if (this.currentBoard != null)
                {
                    return this.currentBoard;
                }

                var board = BoardContext.Current.GetRepository<Board>().GetById(this.BoardId);

                this.currentBoard = board ?? throw new EmptyBoardSettingException($"No data for board ID: {this.BoardId}");

                return this.currentBoard;
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
            this.Registry.Keys.ForEach(key => BoardContext.Current.GetRepository<Registry>().Save(key, this.Registry[key]));

            this.RegistryBoard.Keys.ForEach(
                key => BoardContext.Current.GetRepository<Registry>().Save(key, this.RegistryBoard[key], this.BoardId));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The setup legacy board settings.
        /// </summary>
        /// <param name="board">
        /// The board.
        /// </param>
        /// <returns>
        /// The <see cref="LegacyBoardSettings"/>.
        /// </returns>
        private static LegacyBoardSettings SetupLegacyBoardSettings([NotNull] Board board)
        {
            CodeContracts.VerifyNotNull(board, "board");

            /*var membershipAppName = board.MembershipAppName.IsNotSet()
                                        ? BoardContext.Current.Get<IAspNetUsersManager>().ApplicationName
                                        : board.MembershipAppName;

            var rolesAppName = board.RolesAppName.IsNotSet()
                                   ? BoardContext.Current.Get<IAspNetUsersManager>().ApplicationName
                                   : board.RolesAppName;*/

            return new LegacyBoardSettings(
                board.Name/*,
                membershipAppName,
                rolesAppName*/);
        }

        /// <summary>
        /// Loads the board settings from database.
        /// </summary>
        private void LoadBoardSettingsFromDB()
        {
            var registryList = BoardContext.Current.GetRepository<Registry>().List();

            // get all the registry settings into our hash table
            registryList.ForEach(
                row =>
                    {
                        if (!this.Registry.ContainsKey(row.Name.ToLower()))
                        {
                            this.Registry.Add(row.Name.ToLower(), row.Value.IsNotSet() ? null : row.Value);
                        }
                    });

            var registryBoardList = BoardContext.Current.GetRepository<Registry>().List(this.BoardId);

            // get all the registry settings into our hash table
            registryBoardList.ForEach(
                row =>
                    {
                        if (!this.RegistryBoard.ContainsKey(row.Name.ToLower()))
                        {
                            this.RegistryBoard.Add(row.Name.ToLower(), row.Value.IsNotSet() ? null : row.Value);
                        }
                    });
        }

        #endregion
    }
}