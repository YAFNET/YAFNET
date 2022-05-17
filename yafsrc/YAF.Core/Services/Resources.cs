/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Core.Services;

using System;
using System.Text;

using Newtonsoft.Json;

using ServiceStack.Text;

using YAF.Core.Model;
using YAF.Core.Utilities.StringUtils;
using YAF.Types.Constants;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Resources
/// </summary>
public class Resources : IResources, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Resources"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public Resources([NotNull] IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets the forum user info as JSON string for the hover cards
    /// </summary>
    /// <param name="context">The context.</param>
    public void GetUserInfo([NotNull] HttpContext context)
    {
        try
        {
            var userId = context.Request.QueryString.GetFirstOrDefaultAs<int>("userinfo");

            var user = this.Get<IAspNetUsersHelper>().GetBoardUser(userId);

            if (user == null || user.Item1.ID == 0)
            {
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                return;
            }

            // Check if user has access
            if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ProfileViewPermissions))
            {
                context.Response.Write(string.Empty);

                return;
            }

            context.Response.Clear();

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(TimeSpan.FromSeconds(this.Get<BoardSettings>().OnlineStatusCacheTimeout));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);

            var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(user.Item1);

            avatarUrl = avatarUrl.IsNotSet()
                            ? $"{BoardInfo.ForumClientFileRoot}resource.ashx?avatar={user.Item1.ID}"
                            : avatarUrl;

            var activeUsers = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.UsersOnlineStatus,
                () => this.GetRepository<Active>().List(
                    false,
                    this.Get<BoardSettings>().ShowCrawlersInActiveList,
                    this.Get<BoardSettings>().ActiveListTime),
                TimeSpan.FromMilliseconds(this.Get<BoardSettings>().OnlineStatusCacheTimeout));

            var userIsOnline =
                activeUsers.Any(
                    x => x.UserID == userId && x.IsActiveExcluded == false);

            var userName = user.Item1.DisplayOrUserName();

            userName = HttpUtility.HtmlEncode(userName);

            var location = user.Item2.Profile_Country.IsSet()
                               ? BoardContext.Current.Get<IHaveLocalization>().GetText(
                                   "COUNTRY", user.Item2.Profile_Country.Trim())
                               : HttpUtility.HtmlEncode(user.Item2.Profile_Location);

            if (user.Item2.Profile_Region.IsSet() && user.Item2.Profile_Country.IsSet())
            {
                var tag = $"RGN_{user.Item2.Profile_Country.Trim()}_{user.Item2.Profile_Region}";

                location += $", {this.Get<IHaveLocalization>().GetText("REGION", tag)}";
            }

            var userInfo = new ForumUserInfo
                               {
                                   Name = userName,
                                   RealName = HttpUtility.HtmlEncode(user.Item2.Profile_RealName),
                                   Avatar = avatarUrl,
                                   Interests = HttpUtility.HtmlEncode(user.Item2.Profile_Interests),
                                   HomePage = user.Item2.Profile_Homepage,
                                   Posts = $"{user.Item1.NumPosts:N0}",
                                   Rank = user.Item3.Name,
                                   Location = location,
                                   Joined =
                                       $"{this.Get<IHaveLocalization>().GetText("PROFILE", "JOINED")} {this.Get<IDateTimeService>().FormatDateLong(user.Item1.Joined)}",
                                   Online = userIsOnline
                               };

            if (this.Get<BoardSettings>().EnableUserReputation)
            {
                userInfo.Points = (user.Item1.Points > 0 ? "+" : string.Empty) + user.Item1.Points;
            }

            context.Response.Write(userInfo.ToJson());

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(BoardContext.Current.PageUserID, this, x);

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    /// <summary>
    /// Gets the list of all Custom BB Codes
    /// </summary>
    /// <param name="context">The context.</param>
    public void GetCustomBBCodes([NotNull] HttpContext context)
    {
        try
        {
            if (BoardContext.Current == null)
            {
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                return;
            }

            // TODO : Cache list
            var customBbCode = this.GetRepository<BBCode>().GetByBoardId()
                .Where(e => e.Name != "ALBUMIMG" && e.Name != "ATTACH").Select(e => e.Name).ToList();

            context.Response.Clear();

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(
                TimeSpan.FromSeconds(this.Get<BoardSettings>().OnlineStatusCacheTimeout));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);

            context.Response.Write(customBbCode.ToJson());

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(BoardContext.Current.PageUserID, this, x);

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    /// <summary>
    /// Get all Mentioned Users
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    public void GetMentionUsers([NotNull] HttpContext context)
    {
        try
        {
            if (BoardContext.Current == null)
            {
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                return;
            }

            var searchQuery = context.Request.QueryString.GetFirstOrDefault("users");

            var usersList = BoardContext.Current.GetRepository<User>().Get(
                user => this.Get<BoardSettings>().EnableDisplayName
                            ? user.DisplayName.StartsWith(searchQuery)
                            : user.Name.StartsWith(searchQuery));

            var users = usersList.AsEnumerable().Where(u => !this.Get<IUserIgnored>().IsIgnored(u.ID)).Select(
                u => new { id = u.ID, name = u.DisplayOrUserName() });

            context.Response.Clear();

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            context.Response.Write(JsonConvert.SerializeObject(users));

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(BoardContext.Current.PageUserID, this, x);

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    /// <summary>
    /// Gets the Default Text Avatar
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    public void GetTextAvatar([NotNull] HttpContext context)
    {
        try
        {
            var user = BoardContext.Current.GetRepository<User>()
                .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("avatar"));

            if (user == null)
            {
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                return;
            }
                
            var name = new UnicodeEncoder().XSSEncode(user.DisplayOrUserName());

            if (name.StartsWith("&"))
            {
                name = name.Replace("&", string.Empty);
            }

            var abbreviation = name.GetAbbreviation();

            var width = this.Get<BoardSettings>().AvatarWidth;
            var height = this.Get<BoardSettings>().AvatarHeight;

            var fontSize = Math.Floor(width * 0.3);

            string backgroundColor;

            if (user.UserFlags.IsGuest)
            {
                backgroundColor = "#0c7333";
            }
            else
            {
                backgroundColor = ValidationHelper.IsNumeric(user.ProviderUserKey)
                                      ? $"#{user.ProviderUserKey.ToGuid().ToString().Substring(0, 6)}"
                                      : $"#{user.ProviderUserKey.Substring(0, 6)}";
            }

            var svg = $@"<?xml version=""1.0"" encoding=""UTF-8""?><svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" 
                                  width=""{width}px"" height=""{height}px"" viewBox=""0 0 {width} {height}"" version=""1.1"">
                                  <rect fill=""{backgroundColor}"" width=""{width}"" height=""{height}"" cx=""32"" cy=""32"" r=""32""/>
                                     <text x=""50%"" y=""50%"" style=""color: #fff5f5f5;line-height: 1;font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif;"" 
                                           alignment-baseline=""middle"" text-anchor=""middle"" font-size=""{fontSize}"" font-weight=""500"" dy="".1em"" dominant-baseline=""middle"" fill=""#fff5f5f5"">
                                       {abbreviation}</text></svg>";

            var byteArray = Encoding.ASCII.GetBytes(svg);

            context.Response.Clear();

            context.Response.ContentType = "image/svg+xml";
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);

            context.Response.OutputStream.Write(byteArray, 0, byteArray.Length);
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(
                BoardContext.Current.PageUserID,
                this,
                $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}");

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    /// <summary>
    /// The get response local avatar.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    public void GetResponseLocalAvatar([NotNull] HttpContext context)
    {
        try
        {
            var user = BoardContext.Current.GetRepository<User>()
                .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("u"));

            if (user == null)
            {
                return;
            }

            var data = user.AvatarImage;
            var contentType = user.AvatarImageType;

            context.Response.Clear();
            if (contentType.IsNotSet())
            {
                contentType = "image/jpeg";
            }

            context.Response.ContentType = contentType;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(
                TimeSpan.FromHours(2));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);

            context.Response.OutputStream.Write(data, 0, data.Length);
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>()
                .Log(
                    BoardContext.Current.PageUserID,
                    this,
                    $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}");

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }
}