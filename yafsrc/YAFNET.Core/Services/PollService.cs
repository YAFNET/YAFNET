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

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// Class PollService.
/// Implements the <see cref="YAF.Types.Interfaces.IHaveServiceLocator" />
/// </summary>
/// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
public class PollService : IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PollService"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    public PollService(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Days to run.
    /// </summary>
    /// <param name="poll">
    /// The Poll
    /// </param>
    /// <param name="closesSoon">
    /// The closes Soon.
    /// </param>
    /// <returns>
    /// The days to run.
    /// </returns>
    public int? DaysToRun(Poll poll, out bool closesSoon)
    {
        closesSoon = false;

        if (!poll.Closes.HasValue)
        {
            return null;
        }

        var ts = poll.Closes.Value - DateTime.UtcNow;

        if (ts.TotalDays >= 1)
        {
            return ts.TotalDays.ToType<int>();
        }

        if (ts.TotalSeconds <= 0)
        {
            return 0;
        }

        closesSoon = true;
        return 1;
    }

    /// <summary>
    /// Determines whether [is poll closed] [the specified poll id].
    /// </summary>
    /// <returns>
    /// The is poll closed.
    /// </returns>
    public bool IsPollClosed(Poll poll)
    {
        var dtr = this.Get<PollService>().DaysToRun(poll, out _);
        return dtr == 0;
    }

    /// <summary>
    /// Gets the Vote Width
    /// </summary>
    /// <param name="dataSource">The data source.</param>
    /// <param name="itemTuple">The item tuple.</param>
    /// <returns>System.Int32.</returns>
    public int VoteWidth(List<Tuple<Poll, Choice>> dataSource, Tuple<Poll, Choice> itemTuple)
    {
        try
        {
            var votes = dataSource.Sum(x => x.Item2.Votes);

            return 100 * itemTuple.Item2.Votes / votes;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}