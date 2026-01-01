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

namespace YAF.Core.Logger;

using System;

using Microsoft.Extensions.Logging;

using YAF.Types.Interfaces;

/// <summary>
/// The YAF DB logger provider.
/// </summary>
public class DbLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    /// <summary>
    /// The scope provider
    /// </summary>
    private IExternalScopeProvider scopeProvider;

    /// <summary>
    /// The settings change token
    /// </summary>
    protected IDisposable SettingsChangeToken;

    /// <summary>
    /// Sets the scope provider.
    /// </summary>
    /// <param name="scopeProvider">The provider.</param>
    void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopeProvider = scopeProvider;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        if (this.IsDisposed)
        {
            return;
        }

        try
        {
            this.Dispose(true);
        }
        finally
        {
            this.IsDisposed = true;
            GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor
        }
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.SettingsChangeToken == null)
        {
            return;
        }

        this.SettingsChangeToken.Dispose();
        this.SettingsChangeToken = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbLoggerProvider"/> class.
    /// </summary>
    /// <param name="injectServices">
    /// The inject services.
    /// </param>
    public DbLoggerProvider(IInjectServices injectServices)
    {
        this.InjectServices = injectServices;

        if (!this.IsDisposed)
        {
            this.Dispose(false);
        }
    }

    /// <summary>
    /// Gets or sets InjectServices.
    /// </summary>
    public IInjectServices InjectServices { get; set; }

    /// <summary>
    /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The instance of <see cref="T:Microsoft.Extensions.Logging.ILogger" /> that was created.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        var logger = new DbLogger(this, categoryName);
        this.InjectServices.Inject(logger);

        return logger;
    }

    /// <summary>
    /// Gets the scope provider.
    /// </summary>
    /// <value>The scope provider.</value>
    internal IExternalScopeProvider ScopeProvider => this.scopeProvider ??= new LoggerExternalScopeProvider();

    /// <summary>
    /// Gets or sets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed { get; protected set; }
}