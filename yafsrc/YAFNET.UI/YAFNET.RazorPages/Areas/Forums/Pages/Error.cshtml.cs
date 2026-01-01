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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YAF.Pages;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;

using YAF.Types.Extensions;

/// <summary>
/// The error model.
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : ForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorModel"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public ErrorModel(ILogger<ErrorModel> logger)
        : base("Error", ForumPages.Error)
    {
    }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>The error message.</value>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// The show request id.
    /// </summary>
    public string ErrorDescription { get; set; }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        return this.ShowError();
    }

    /// <summary>
    /// Called when [post].
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPost()
    {
        return this.ShowError();
    }

    /// <summary>
    /// Shows the error.
    /// </summary>
    /// <returns>IActionResult.</returns>
    private PageResult ShowError()
    {
        var exceptionHandlerPathFeature =
            this.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature is null)
        {
            return this.Page();
        }

        var error = exceptionHandlerPathFeature.Error;

        this.ErrorMessage = error.Message;
        this.ErrorDescription = error.StackTrace;

        this.Get<ILogger<ErrorModel>>().Error(error, error.Message);

        return this.Page();
    }
}