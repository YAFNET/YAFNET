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

namespace YAF.Types.Interfaces.Services;

using System.Collections.Generic;

using YAF.Types.Objects;

/// <summary>
/// SpamWordCheck Interface
/// </summary>
public interface ISpamWordCheck
{
    /// <summary>
    /// Gets the spam word items.
    /// </summary>
    /// <value>
    /// The spam word items.
    /// </value>
    IEnumerable<SpamWordCheckItem> SpamWordItems { get; }

    /// <summary>
    /// Checks for spam word.
    /// </summary>
    /// <param name="searchText">The search text.</param>
    /// <param name="spamWord">The spam word.</param>
    /// <returns>
    /// Returns if the search Text contains a spam word
    /// </returns>
    bool CheckForSpamWord(string searchText, out string spamWord);
}