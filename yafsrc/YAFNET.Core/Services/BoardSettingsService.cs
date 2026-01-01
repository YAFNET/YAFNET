/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Core.Services;

using Microsoft.Extensions.Logging;

using YAF.Configuration.Pattern;
using YAF.Core.Model;
using YAF.Types.Exceptions;
using YAF.Types.Models;

/// <summary>
/// Class BoardSettingsService.
/// Implements the <see cref="YAF.Types.Interfaces.IHaveServiceLocator" />
/// </summary>
/// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
public class BoardSettingsService : IHaveServiceLocator
{
    /// <summary>Initializes a new instance of the <see cref="T:YAF.Core.Services.BoardSettingsService" /> class.</summary>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public BoardSettingsService(ILogger<BoardSettingsService> logger, IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
        this.Logger = logger;
    }

    /// <summary>
    /// Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>Loads the board settings.</summary>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="board">The board.</param>
    /// <returns>BoardSettings.</returns>
    /// <exception cref="YAF.Types.Exceptions.EmptyBoardSettingException">No data for board ID: {boardId}</exception>
    public BoardSettings LoadBoardSettings(int boardId, Board board)
    {
        if (board == null)
        {
            board = this.GetRepository<Board>().GetById(boardId);

            if (board == null)
            {
                throw new EmptyBoardSettingException($"No data for board ID: {boardId}");
            }
        }

        var registry = new RegistryDictionaryOverride();
        var registryBoard = new RegistryDictionary();

        // get all the registry values for the forum
        var registryList = this.GetRepository<Registry>().List();

        // get all the registry settings into our hash table
        registryList.ForEach(
            row =>
                {
                    if (!registry.ContainsKey(row.Name.ToLower()))
                    {
                        registry.Add(row.Name.ToLower(), row.Value.IsNotSet() ? null : row.Value);
                    }
                });

        var registryBoardList = this.GetRepository<Registry>().List(boardId);

        // get all the registry settings into our hash table
        registryBoardList.ForEach(
            row =>
                {
                    if (!registryBoard.ContainsKey(row.Name.ToLower()))
                    {
                        registryBoard.Add(row.Name.ToLower(), row.Value.IsNotSet() ? null : row.Value);
                    }
                });

        return new BoardSettings(boardId, board.Name, board.Description, registry, registryBoard);
    }

    /// <summary>
    ///     Saves the whole setting registry to the database.
    /// </summary>
    public void SaveRegistry(BoardSettings boardSettings)
    {
        // loop through all values and commit them to the DB
        boardSettings.Registry.Keys.ForEach(
            key => this.GetRepository<Registry>().Save(key, boardSettings.Registry[key]));

        boardSettings.RegistryBoard.Keys.ForEach(
            key => this.GetRepository<Registry>().Save(key, boardSettings.RegistryBoard[key], boardSettings.BoardId));

        // Reset Board Settings
        this.Get<CurrentBoardSettings>().Reset();
    }
}