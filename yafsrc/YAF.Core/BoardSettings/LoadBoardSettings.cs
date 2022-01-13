/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Core.BoardSettings
{
    #region Using

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
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

            this.BoardName = this.CurrentBoard.Name;

            // get all the registry values for the forum
            this.LoadBoardSettingsFromDb();
        }

        #endregion

        #region Properties

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

            // Reset Board Settings
            BoardContext.Current.Get<CurrentBoardSettings>().Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the board settings from database.
        /// </summary>
        private void LoadBoardSettingsFromDb()
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