/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Core.Model;

using YAF.Types.Models;

/// <summary>
///     The NntpServer repository extensions.
/// </summary>
public static class NntpServerRepositoryExtensions
{
    /// <summary>
    /// Save Server.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="nntpServerId">
    /// The NNTP server id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="address">
    /// The address.
    /// </param>
    /// <param name="port">
    /// The port.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="userPass">
    /// The user pass.
    /// </param>
    public static void Save(
        this IRepository<NntpServer> repository,
        int? nntpServerId,
        int boardId,
        string name,
        string address,
        int? port,
        string userName,
        string userPass)
    {
        if (nntpServerId.HasValue)
        {
            repository.UpdateOnly(
                () => new NntpServer
                          {
                              Name = name,
                              Address = address,
                              Port = port,
                              UserName = userName,
                              UserPass = userPass
                          },
                n => n.ID == nntpServerId);
        }
        else
        {
            var entity = new NntpServer
                             {
                                 Name = name,
                                 BoardID = boardId,
                                 Address = address,
                                 Port = port,
                                 UserName = userName,
                                 UserPass = userPass
                             };

            repository.Insert(entity);

            repository.FireNew(entity);
        }
    }

    /// <summary>
    /// Deletes the specified server.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="serverId">The server identifier.</param>
    public static void Delete(this IRepository<NntpServer> repository, int serverId)
    {
        var forums = BoardContext.Current.GetRepository<NntpForum>().Get(n => n.NntpServerID == serverId)
            .Select(forum => forum.ForumID).ToList();

        BoardContext.Current.GetRepository<NntpTopic>().DeleteByIds(forums);
        BoardContext.Current.GetRepository<NntpForum>().Delete(n => n.NntpServerID == serverId);

        repository.Delete(n => n.ID == serverId);
    }
}