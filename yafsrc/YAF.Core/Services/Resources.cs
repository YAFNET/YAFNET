/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;

    using Newtonsoft.Json;

    using ServiceStack;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Services.Auth;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers.ImageUtils;

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

                var boardId = context.Request.QueryString.GetFirstOrDefaultAs<int>("boardId");

                var user = UserMembershipHelper.GetMembershipUserById(userId, boardId);

                if (user == null || user.ProviderUserKey.ToString() == "0")
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

                var userData = new CombinedUserDataHelper(user, userId);

                context.Response.Clear();

                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(
                    System.DateTime.UtcNow.AddMilliseconds(BoardContext.Current.Get<BoardSettings>().OnlineStatusCacheTimeout));
                context.Response.Cache.SetLastModified(System.DateTime.UtcNow);

                var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userId);

                avatarUrl = avatarUrl.IsNotSet()
                           ? $"{BoardInfo.ForumClientFileRoot}images/noavatar.svg"
                           : avatarUrl;

                var activeUsers = this.Get<IDataCache>().GetOrSet(
                    Constants.Cache.UsersOnlineStatus,
                    () =>
                    this.Get<DataBroker>().GetActiveList(
                        false, BoardContext.Current.Get<BoardSettings>().ShowCrawlersInActiveList),
                    TimeSpan.FromMilliseconds(BoardContext.Current.Get<BoardSettings>().OnlineStatusCacheTimeout));

                var userIsOnline =
                    activeUsers.AsEnumerable().Any(
                        x => x.Field<int>("UserId").Equals(userId) && !x.Field<bool>("IsHidden"));

                var userName = this.Get<BoardSettings>().EnableDisplayName ? userData.DisplayName : userData.UserName;

                userName = HttpUtility.HtmlEncode(userName);

                var location = userData.Profile.Country.IsSet()
                                   ? BoardContext.Current.Get<IHaveLocalization>().GetText(
                                       "COUNTRY", userData.Profile.Country.Trim())
                                   : userData.Profile.Location;

                if (userData.Profile.Region.IsSet() && userData.Profile.Country.IsSet())
                {
                    var tag = $"RGN_{userData.Profile.Country.Trim()}_{userData.Profile.Region}";

                    location += $", {this.Get<IHaveLocalization>().GetText("REGION", tag)}";
                }

                var userInfo = new ForumUserInfo
                {
                    Name = userName,
                    RealName = HttpUtility.HtmlEncode(userData.Profile.RealName),
                    Avatar = avatarUrl,
                    Interests = HttpUtility.HtmlEncode(userData.Profile.Interests),
                    HomePage = userData.Profile.Homepage,
                    Posts = $"{userData.NumPosts:N0}",
                    Rank = userData.RankName,
                    Location = location,
                    Joined =
                        $"{this.Get<IHaveLocalization>().GetText("PROFILE", "JOINED")} {this.Get<IDateTime>().FormatDateLong(userData.Joined)}",
                    Online = userIsOnline/*,
                    ProfileLink = BuildLink.GetLink(ForumPages.Profile, true, "u={0}&name={1}", userId, userName)*/
                };

                if (BoardContext.Current.Get<BoardSettings>().EnableUserReputation)
                {
                    userInfo.Points = (userData.Points.ToType<int>() > 0 ? "+" : string.Empty) + userData.Points;
                }

                context.Response.Write(userInfo.ToJson());

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);

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

                var customBbCode = this.Get<DataBroker>().GetCustomBBCode()
                    .Where(e => e.Name != "ALBUMIMG" && e.Name != "ATTACH").Select(e => e.Name).ToList();

                context.Response.Clear();

                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(
                    System.DateTime.UtcNow.AddMilliseconds(BoardContext.Current.Get<BoardSettings>().OnlineStatusCacheTimeout));
                context.Response.Cache.SetLastModified(System.DateTime.UtcNow);

                context.Response.Write(customBbCode.ToJson());

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);

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
                    u => new { id = u.ID, name = this.Get<BoardSettings>().EnableDisplayName ? u.DisplayName : u.Name });

                context.Response.Clear();

                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = Encoding.UTF8;

                context.Response.Write(JsonConvert.SerializeObject(users));

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// Gets the twitter user info as JSON string for the hover cards
        /// </summary>
        /// <param name="context">The context.</param>
        public void GetTwitterUserInfo([NotNull] HttpContext context)
        {
            try
            {
                var twitterName = context.Request.QueryString.GetFirstOrDefault("twitterinfo");

                if (!Config.IsTwitterEnabled)
                {
                    context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                    return;
                }

                var authTwitter = new OAuthTwitter
                {
                    ConsumerKey = Config.TwitterConsumerKey,
                    ConsumerSecret = Config.TwitterConsumerSecret,
                    Token = Config.TwitterToken,
                    TokenSecret = Config.TwitterTokenSecret
                };

                var tweetApi = new TweetAPI(authTwitter);

                context.Response.Write(tweetApi.UsersLookupJson(twitterName));

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);

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
                context.Response.Cache.SetExpires(System.DateTime.UtcNow.AddHours(2));
                context.Response.Cache.SetLastModified(System.DateTime.UtcNow);

                context.Response.OutputStream.Write(data, 0, data.Length);
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
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
                        CaptchaHelper.GetCaptchaText(new HttpSessionStateWrapper(context.Session), context.Cache, true),
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
                this.Get<ILogger>().Log(BoardContext.Current.PageUserID, this, x);
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }

#endif
        }

        /// <summary>
        /// The get response remote avatar.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void GetResponseRemoteAvatar([NotNull] HttpContext context)
        {
            var avatarUrl = context.Request.QueryString.GetFirstOrDefault("url");

            if (avatarUrl.StartsWith("/"))
            {
                var basePath = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}";

                avatarUrl = $"{basePath}{avatarUrl}";
            }

            var maxWidth = int.Parse(context.Request.QueryString.GetFirstOrDefault("width"));
            var maxHeight = int.Parse(context.Request.QueryString.GetFirstOrDefault("height"));

            var etagCode =
                $@"""{(context.Request.QueryString.GetFirstOrDefault("url") + maxHeight + maxWidth).GetHashCode()}""";

            var webClient = new WebClient { Credentials = CredentialCache.DefaultCredentials };

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => true;

                var originalData = webClient.DownloadData(avatarUrl);

                if (originalData == null)
                {
                    // Output no-avatar
                    context.Response.Redirect($"{BoardInfo.ForumClientFileRoot}/Images/noavatar.svg");
                    return;
                }

                using (var avatarStream = new MemoryStream(originalData))
                {
                    using (var img = new Bitmap(avatarStream))
                    {
                        var width = img.Width;
                        var height = img.Height;

                        if (width <= maxWidth && height <= maxHeight)
                        {
                            context.Response.Redirect(avatarUrl);
                        }

                        if (width > maxWidth)
                        {
                            height = (height / (double)width * maxWidth).ToType<int>();
                            width = maxWidth;
                        }

                        if (height > maxHeight)
                        {
                            width = (width / (double)height * maxHeight).ToType<int>();
                            height = maxHeight;
                        }

                        // Create the target bitmap
                        using (var bmp = new Bitmap(width, height))
                        {
                            // Create the graphics object to do the high quality resizing
                            using (var gfx = Graphics.FromImage(bmp))
                            {
                                gfx.CompositingQuality = CompositingQuality.HighQuality;
                                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                gfx.SmoothingMode = SmoothingMode.HighQuality;

                                // Draw the source image
                                gfx.DrawImage(img, new Rectangle(new Point(0, 0), new Size(width, height)));
                            }

                            // Output the data
                            context.Response.ContentType = "image/jpeg";
                            context.Response.Cache.SetCacheability(HttpCacheability.Public);
                            context.Response.Cache.SetExpires(System.DateTime.UtcNow.AddHours(2));
                            context.Response.Cache.SetLastModified(System.DateTime.UtcNow);
                            context.Response.Cache.SetETag(etagCode);
                            bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                // issue getting access to the avatar...
                this.Get<ILogger>().Log(
                    BoardContext.Current.PageUserID,
                    this,
                    $"URL: {avatarUrl}<br />Referer URL: {context.Request.UrlReferrer?.AbsoluteUri ?? string.Empty}<br />Exception: {exception}");

                // Output the data
                context.Response.Redirect($"{BoardInfo.ForumClientFileRoot}/Images/noavatar.svg");
            }
        }
    }
}