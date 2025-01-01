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

namespace YAF.Core.Controllers.Modals;

using System;

using Microsoft.Extensions.Logging;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Types.Attributes;
using YAF.Types.Objects;

/// <summary>
/// BBCode Controller
/// Implements the <see cref="ForumBaseController" />
/// </summary>
/// <seealso cref="ForumBaseController" />
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class BBCodeController : ForumBaseController
{
    /// <summary>
    /// Import
    /// </summary>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("Import")]
    public IActionResult Import([FromForm] IFormFile file)
    {
        if (!file.ContentType.StartsWith("text"))
        {
            return this.Ok(
                new MessageModalNotification(
               this.GetTextFormatted("IMPORT_FAILED", file.ContentType),
                MessageTypes.danger));
        }

        try
        {
            var importedCount = this.Get<IDataImporter>().BBCodeExtensionImport(
                this.PageBoardContext.PageBoardID,
                file.OpenReadStream());

            return this.Ok(
                new MessageModalNotification(
                importedCount > 0
                    ? string.Format(this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_SUCESS"), importedCount)
                    : this.GetText("ADMIN_BBCODE_IMPORT", "IMPORT_NOTHING"),
                MessageTypes.success));
        }
        catch (Exception x)
        {
            this.Get<ILogger<BBCodeController>>().Error(
                x,
                string.Format(this.GetText("ADMIN_BBCODE_IMPORT", "IMPORT_FAILED"), x.Message));

            return this.Ok(
                new MessageModalNotification(
                string.Format(this.GetText("ADMIN_BBCODE_IMPORT", "IMPORT_FAILED"), x.Message),
                MessageTypes.danger));
        }
    }
}