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

using System.Threading;

namespace YAF.Types.Objects;

using System.Text.RegularExpressions;

/// <summary>
/// 
/// </summary>
[Serializable]
public class SpamWordCheckItem
{
    /// <summary>
    ///   The _active lock.
    /// </summary>
    private readonly Lock _activeLock = new();

    /// <summary>
    ///   The _active.
    /// </summary>
    private bool _active = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpamWordCheckItem" /> class.
    /// </summary>
    /// <param name="spamWord">The spam word.</param>
    /// <param name="options">The options.</param>
    public SpamWordCheckItem(string spamWord, RegexOptions options)
    {
        this.Options = options;

        try
        {
            this.SpamWordRegEx = new Regex(spamWord, options, TimeSpan.FromMilliseconds(100));
        }
        catch (Exception)
        {
            this.SpamWordRegEx = null;
        }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether Active.
    /// </summary>
    public bool Active
    {
        get
        {
            bool value;

            lock (this._activeLock)
            {
                value = this._active;
            }

            return value;
        }

        set
        {
            lock (this._activeLock)
            {
                this._active = value;
            }
        }
    }

    /// <summary>
    /// Gets the spam word reg ex.
    /// </summary>
    /// <value>
    /// The spam word reg ex.
    /// </value>
    public Regex SpamWordRegEx { get; }

    /// <summary>
    /// Gets or sets Options.
    /// </summary>
    public RegexOptions Options { get; protected set; }
}