/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

/// <summary>
/// The YAF board folders.
/// </summary>
public class BoardFolders
{
    /// <summary>
    /// Gets BoardFolder.
    /// </summary>
    public string BoardFolder =>
        Config.MultiBoardFolders
            ? $"{Config.BoardRoot}{BoardContext.Current.Get<ControlSettings>().BoardID}/"
            : Config.BoardRoot;

    /// <summary>
    /// Gets Uploads.
    /// </summary>
    public string Uploads => $"{this.BoardFolder}Uploads";

    /// <summary>
    /// Gets Images.
    /// </summary>
    public string Images => $"{this.BoardFolder}Images";

    /// <summary>
    /// Gets Avatars.
    /// </summary>
    public string Avatars => $"{this.BoardFolder}Images/Avatars";

    /// <summary>
    /// Gets Categories.
    /// </summary>
    public string Categories => $"{this.BoardFolder}Images/Categories";

    /// <summary>
    /// Gets Categories.
    /// </summary>
    public string Forums => $"{this.BoardFolder}Images/Forums";

    /// <summary>
    /// Gets Medals.
    /// </summary>
    public string Medals => $"{this.BoardFolder}Images/Medals";

    /// <summary>
    /// Gets Logos.
    /// </summary>
    public string Logos => $"{this.BoardFolder}Images/Logos";
}