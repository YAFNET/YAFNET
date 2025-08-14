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

namespace YAF.Core.BBCode;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using YAF.Core.Context;

/// <summary>
/// The YAF BBCode control.
/// </summary>
public class BBCodeControl : IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BBCodeControl"/> class.
    /// </summary>
    public BBCodeControl()
    {
        this.Get<IInjectServices>().Inject(this);
    }

    /// <summary>
    ///   Gets or sets MessageID.
    /// </summary>
    public int? MessageID { get; set; }

    /// <summary>
    ///   Gets or sets DisplayUserID.
    /// </summary>
    public int? DisplayUserID { get; set; }

    /// <summary>
    ///   Gets or sets Parameters.
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = [];

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => field ??= this.Get<ILocalization>();

    /// <summary>
    ///   Gets PageContext.
    /// </summary>
    public static BoardContext PageContext => BoardContext.Current;

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => PageContext.ServiceLocator;

    /// <summary>
    /// Creates a Unique ID
    /// </summary>
    /// <param name="prefix">
    /// The prefix.
    /// </param>
    /// <returns>
    /// The get unique id.
    /// </returns>
    public static string GetUniqueId(string prefix)
    {
        return prefix.IsSet()
                   ? $"{prefix}{Guid.NewGuid().ToString()[..5]}"
                   : Guid.NewGuid().ToString()[..10];
    }

    /// <summary>
    /// XSS Encode input String.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The encoded string.
    /// </returns>
    public static string HtmlEncode(object data)
    {
        if (data == null)
        {
            return null;
        }

        return BoardContext.Current.CurrentForumPage != null
                   ? BoardContext.Current.CurrentForumPage.HtmlEncode(data.ToString())
                   : new UnicodeEncoder().XSSEncode(data.ToString());
    }

    /// <summary>
    /// Gets the localized string.
    /// </summary>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="defaultString">
    /// The default string.
    /// </param>
    /// <returns>
    /// Returns the localized string.
    /// </returns>
    protected string LocalizedString(string tag, string defaultString)
    {
        return this.Get<ILocalization>().GetTextExists("BBCODEMODULE", tag)
                   ? this.GetText("BBCODEMODULE", tag)
                   : defaultString;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="stringBuilder">
    ///     The string builder.
    /// </param>
    public virtual Task RenderAsync(StringBuilder stringBuilder)
    {
        return Task.CompletedTask;
    }
}