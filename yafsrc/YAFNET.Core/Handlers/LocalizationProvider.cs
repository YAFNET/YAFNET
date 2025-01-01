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

namespace YAF.Core.Handlers;

using System;

using YAF.Core.Services.Localization;

/// <summary>
/// The localization handler.
/// </summary>
public class LocalizationProvider
{
    /// <summary>
    ///   The localization is Initialized flag.
    /// </summary>
    private bool initLocalization;

    /// <summary>
    ///   The after initializing Event.
    /// </summary>
    public event EventHandler<EventArgs> AfterInit;

    /// <summary>
    ///   The before initializing Event.
    /// </summary>
    public event EventHandler<EventArgs> BeforeInit;

    /// <summary>
    ///   Gets or sets Localization.
    /// </summary>
    public ILocalization Localization
    {
        get
        {
            if (!this.initLocalization)
            {
                this.InitLocalization();
            }

            return field;
        }

        set
        {
            field = value;
            this.initLocalization = value != null;
        }
    }

    /// <summary>
    ///   Gets or sets the Current TransPage for Localization
    /// </summary>
    public string TranslationPage {
        get;

        set {
            if (value == field)
            {
                return;
            }

            field = value;

            if (this.initLocalization)
            {
                // re-init localization
                this.Localization = null;
            }
        }
    } = string.Empty;

    /// <summary>
    /// Set up the localization
    /// </summary>
    protected void InitLocalization()
    {
        if (this.initLocalization)
        {
            return;
        }

        this.BeforeInit?.Invoke(this, EventArgs.Empty);

        if (this.TranslationPage.IsNotSet() && BoardContext.Current.CurrentForumPage != null)
        {
            this.Localization = BoardContext.Current.CurrentForumPage.Localization;
        }
        else
        {
            this.Localization = new Localization(this.TranslationPage);
        }

        this.AfterInit?.Invoke(this, EventArgs.Empty);
    }
}