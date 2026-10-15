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

using Microsoft.AspNetCore.OutputCaching;

namespace YAF.Core.Controllers;

using System;
using System.IO;
using System.Security;

using Microsoft.Extensions.Logging;

using YAF.Core.BasePages;
using YAF.Types.Models;

/// <summary>
/// The Avatar controller.
/// </summary>
[Route("api/[controller]")]
public class AvatarController : ForumBaseController
{
    /// <summary>
    /// Gets the Text Avatar
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// The <see cref="ActionResult"/>.
    /// </returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // one hour, a changed display name shows the new letters soon (creating the svg is cheap)
    [OutputCache(Duration = 3600)]
    [HttpGet("GetTextAvatar")]
    public ActionResult GetTextAvatar(int userId)
    {
        try
        {
            var user = this.GetRepository<User>()
                .GetById(userId);

            if (user == null)
            {
                return this.NotFound();
            }

            // the abbreviation of the name, encoded for the svg (xml), e.g. "T&amp;" for "Tom & Jerry"
            var abbreviation = SecurityElement.Escape(user.DisplayOrUserName().GetAbbreviation());

            var width = this.Get<BoardSettings>().AvatarWidth;
            var height = this.Get<BoardSettings>().AvatarHeight;

            var fontSize = Math.Floor(width * 0.3);

            string backgroundColor;

            if (user.UserFlags.IsGuest)
            {
                backgroundColor = "#0c7333";
            }
            else if (ValidationHelper.IsNumeric(user.ProviderUserKey))
            {
                backgroundColor = $"#{user.ProviderUserKey.ToGuid().ToString()[..6]}";
            }
            else if (user.ProviderUserKey is { Length: >= 6 } key && key[..6].All(char.IsAsciiHexDigit))
            {
                backgroundColor = $"#{key[..6]}";
            }
            else
            {
                // no usable provider key, a color from the user id
                backgroundColor = $"#{(uint)user.ID * 2654435761u % 0xFFFFFF:x6}";
            }

            var svg = $@"<?xml version=""1.0"" encoding=""UTF-8""?><svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" 
                                  width=""{width}px"" height=""{height}px"" viewBox=""0 0 {width} {height}"" version=""1.1"">
                                  <rect fill=""{backgroundColor}"" width=""{width}"" height=""{height}"" cx=""32"" cy=""32"" r=""32""/>
                                     <text x=""50%"" y=""50%"" style=""color: #fff5f5f5;line-height: 1;font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif;"" 
                                           alignment-baseline=""middle"" text-anchor=""middle"" font-size=""{fontSize}"" font-weight=""500"" dy="".1em"" dominant-baseline=""middle"" fill=""#fff5f5f5"">
                                       {abbreviation}</text></svg>";

            var byteArray = Encoding.ASCII.GetBytes(svg);

            var stream = new MemoryStream(byteArray, 0, byteArray.Length);
            return new FileStreamResult(stream, "image/svg+xml");
        }
        catch (Exception x)
        {
            this.Get<ILogger<AvatarController>>()
                .Log(
                    this.PageBoardContext.PageUserID,
                    this,
                    $"Exception: {x}",
                    EventLogTypes.Information);

            return this.NotFound();
        }
    }

    /// <summary>
    /// Get response local avatar.
    /// </summary>
    /// <param name="userId">
    /// The User Id.
    /// </param>
    /// <returns>
    /// The <see cref="ActionResult"/>.
    /// </returns>
    [HttpGet("GetResponseLocalAvatar")]
    public ActionResult GetResponseLocalAvatar(int userId)
    {
        try
        {
            var user = this.GetRepository<User>()
                .GetById(userId);

            if (user == null)
            {
                return this.NotFound();
            }

            var data = user.AvatarImage;

            // a user without uploaded avatar (the link is only used for uploaded avatars)
            if (data is null)
            {
                return this.NotFound();
            }
            // the stored type comes from the upload, only ever serve image types
            var contentType = user.AvatarImageType?.Trim().ToLowerInvariant() switch
            {
                "image/png" or "image/jpeg" or "image/gif" or "image/webp" or "image/bmp" => user.AvatarImageType.Trim()
                    .ToLowerInvariant(),
                "image/jpg" or "image/pjpeg" => "image/jpeg",
                null or "" => "image/jpeg",
                _ => "image/png"
            };

            var stream = new MemoryStream(data, 0, data.Length);
            return new FileStreamResult(stream, contentType);
        }
        catch (Exception x)
        {
            this.Get<ILogger<AvatarController>>()
                .Log(
                    this.PageBoardContext.PageUserID,
                    this,
                    $"Exception: {x}",
                    EventLogTypes.Information);

            return this.NotFound();
        }
    }
}