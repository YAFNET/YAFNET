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

namespace YAF.Configuration;

/// <summary>
/// Class provides glue/settings transfer between YAF forum control and base classes
/// </summary>
public class ControlSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControlSettings"/> class.
    /// </summary>
    public ControlSettings()
    {
        this.CategoryID = Config.CategoryID;

        this.BoardID = Config.BoardID == 0 ? 1 : Config.BoardID;
    }

    /// <summary>
    /// Gets or sets BoardID.
    /// </summary>
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets CategoryID.
    /// </summary>
    public int CategoryID { get; set; }
}