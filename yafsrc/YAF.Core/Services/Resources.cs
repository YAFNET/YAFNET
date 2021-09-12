/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text;
    using System.Web;

    using Newtonsoft.Json;

    using ServiceStack;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Utilities.ImageUtils;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects;

    #endregion

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

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

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
                this.Get<ILoggerService>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);

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

                // TODo : Cache list
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
                this.Get<ILoggerService>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);

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
                this.Get<ILoggerService>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);

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
                    return;
                }

                var abbreviation = user.DisplayOrUserName().GetAbbreviation();

                var backgroundColor = ValidationHelper.IsNumeric(user.ProviderUserKey)
                    ? $"#{user.ProviderUserKey.ToGuid().ToString().Substring(0, 6)}"
                    : $"#{user.ProviderUserKey.Substring(0, 6)}";

                using (var bmp = new Bitmap(this.Get<BoardSettings>().AvatarWidth, this.Get<BoardSettings>().AvatarHeight))
                {
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        using (Brush brush = new SolidBrush((Color)new ColorConverter().ConvertFromString(backgroundColor)))
                        {
                            graphics.FillRectangle(
                                brush,
                                0,
                                0,
                                this.Get<BoardSettings>().AvatarWidth,
                                this.Get<BoardSettings>().AvatarHeight);
                        }

                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center
                        };

                        var font = new Font("Arial", 48, FontStyle.Bold, GraphicsUnit.Pixel);

                        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                        graphics.DrawString(
                            abbreviation,
                            font,
                            new SolidBrush(Color.WhiteSmoke),
                            new RectangleF(
                                0,
                                0,
                                this.Get<BoardSettings>().AvatarWidth,
                                this.Get<BoardSettings>().AvatarHeight),
                            sf);
                        graphics.Flush();

                        var converter = new ImageConverter();

                        var image = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                        context.Response.Clear();

                        context.Response.ContentType = "image/png";
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
                        context.Response.Cache.SetLastModified(DateTime.UtcNow);

                        context.Response.OutputStream.Write(image, 0, image.Length);
                    }
                }
            }
            catch (Exception x)
            {
                this.Get<ILoggerService>()
                    .Log(
                        BoardContext.Current.PageUserID,
                        this,
                        $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                        EventLogTypes.Information);

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
                       $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                       EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// The get response captcha.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void GetResponseCaptcha([NotNull] HttpContext context)
        {
#if (!DEBUG)
            try
            {
#endif

                var captchaImage =
                    new CaptchaImage(
                        CaptchaHelper.GetCaptchaText(new HttpSessionStateWrapper(context.Session), MemoryCache.Default, true),
                        250,
                        50,
                        "Century Schoolbook");
                context.Response.Clear();
                context.Response.ContentType = "image/jpeg";
                captchaImage.Image.Save(context.Response.OutputStream, ImageFormat.Jpeg);
#if (!DEBUG)
            }
            catch (Exception x)
            {
                this.Get<ILoggerService>().Log(BoardContext.Current.PageUserID, this, x);
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }

#endif
        }
    }
}