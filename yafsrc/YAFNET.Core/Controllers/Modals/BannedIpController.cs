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

using Microsoft.Extensions.Logging;

using System;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// BannedIp Controller
/// Implements the <see cref="ForumBaseController" />
/// </summary>
/// <seealso cref="ForumBaseController" />
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class BannedIpController : ForumBaseController
{
    /// <summary>
    /// Import
    /// </summary>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("Import")]
    public IActionResult Import([FromForm]IFormFile file)
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
            var importedCount = this.Get<IDataImporter>().BannedIpAddressesImport(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID,
                file.OpenReadStream());

            return this.Ok(
                new MessageModalNotification(
                importedCount > 0
                    ? string.Format(this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_SUCESS"), importedCount)
                    : this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_NOTHING"),
                MessageTypes.success));
        }
        catch (Exception x)
        {
            this.Get<ILogger<BannedEmailController>>().Error(
                x,
                string.Format(this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_FAILED"), x.Message));

            return this.Ok(
                new MessageModalNotification(
               string.Format(this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_FAILED"), x.Message),
                MessageTypes.danger));
        }
    }

    /// <summary>
    /// Add or Edit
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("Edit")]
    public IActionResult Edit([FromBody] BannedEmailEditModal model)
    {
        if (model.Id is 0)
        {
            model.Id = null;
        }

        var ipParts = model.Mask.Trim().Split('.');

        // do some validation...
        var ipError = new StringBuilder();

        if (ipParts.Length != 4)
        {
            ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_ADRESS"));
        }

        foreach (var ip in ipParts)
        {
            // see if they are numbers...
            if (!ulong.TryParse(ip, out var number))
            {
                if (ip.Trim() == "*")
                {
                    continue;
                }

                if (ip.Trim().Length != 0)
                {
                    ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_SECTION"), ip);
                }
                else
                {
                    ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_VALUE"));
                }

                break;
            }

            // try parse succeeded... verify number amount...
            if (number > 255)
            {
                ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_LESS"), ip);
            }
        }

        // show error(s) if not valid...
        if (ipError.Length > 0)
        {
            this.PageBoardContext.SessionNotify(ipError.ToString(), MessageTypes.warning);
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_BannedIps);
        }

        if (!this.GetRepository<BannedIP>().Save(
                model.Id,
                model.Mask.Trim(),
                model.BanReason.IsSet() ? model.BanReason.Trim() : model.BanReason,
                this.PageBoardContext.PageUserID))
        {
            return this.Ok(
                new MessageModalNotification(
               this.GetText("ADMIN_BANNEDIP", "MSG_EXIST"),
                MessageTypes.warning));
        }

        if (this.PageBoardContext.BoardSettings.LogBannedIP)
        {
            this.Get<ILogger<BannedIpController>>().Log(
                $"IP or mask {model.Mask.Trim()} was saved by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
                EventLogTypes.IpBanSet);
        }

        return this.Ok();
    }
}