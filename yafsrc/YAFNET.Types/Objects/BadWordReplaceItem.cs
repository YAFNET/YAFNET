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

using System.Threading;

namespace YAF.Types.Objects;

using System.Text.RegularExpressions;

/// <summary>
/// The bad word replace item.
/// </summary>
[Serializable]
public class BadWordReplaceItem
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
    /// Initializes a new instance of the <see cref="BadWordReplaceItem"/> class.
    /// </summary>
    /// <param name="goodWord">
    /// The good word.
    /// </param>
    /// <param name="badWord">
    /// The bad word.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    public BadWordReplaceItem(string goodWord, string badWord, RegexOptions options)
    {
        this.Options = options;
        this.GoodWord = goodWord;
        this.BadWord = badWord;

        try
        {
            this.BadWordRegEx = new Regex(badWord, options, TimeSpan.FromMilliseconds(100));
        }
        catch (Exception)
        {
            this.BadWordRegEx = null;
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
    ///   Gets BadWord.
    /// </summary>
    public string BadWord { get; }

    /// <summary>
    ///   Gets BadWordRegEx.
    /// </summary>
    public Regex BadWordRegEx { get; }

    /// <summary>
    ///   Gets GoodWord.
    /// </summary>
    public string GoodWord { get; }

    /// <summary>
    /// Gets or sets Options.
    /// </summary>
    public RegexOptions Options { get; protected set; }
}