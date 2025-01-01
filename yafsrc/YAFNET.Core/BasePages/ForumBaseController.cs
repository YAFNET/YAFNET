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

namespace YAF.Core.BasePages;

/// <summary>
/// The class that all YAF forum pages are derived from.
/// </summary>
public abstract class ForumBaseController : Controller, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    /// The Unicode Encoder
    /// </summary>
    private readonly UnicodeEncoder unicodeEncoder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumBaseController"/> class.
    /// </summary>
    protected ForumBaseController()
    {
        this.Get<IInjectServices>().Inject(this);

        // set the BoardContext ForumPage...
        BoardContext.Current.CurrentForumController = this;

        this.unicodeEncoder = new UnicodeEncoder();
    }

    /// <summary>
    ///   Gets the Localization.
    /// </summary>
    public ILocalization Localization => this.Get<ILocalization>();

    /// <summary>
    ///   Gets the current forum Context (helper reference)
    /// </summary>
    public BoardContext PageBoardContext => BoardContext.Current;

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Encodes the HTML
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>Returns the Encoded String</returns>
    public string HtmlEncode(object data)
    {
        return data is not string ? null : this.unicodeEncoder.XSSEncode(data.ToString());
    }
}