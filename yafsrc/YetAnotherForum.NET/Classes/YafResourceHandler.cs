/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
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

namespace YAF
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
    using System.Web.SessionState;
    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Core.Services.Auth;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Extensions;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// YAF Resource Handler for all kind of Stuff (Avatars, Attachments, Albums, etc.)
    /// </summary>
    public class YafResourceHandler : IHttpHandler, IReadOnlySessionState, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsReusable.
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => YafContext.Current.ServiceLocator;

        #endregion

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// The process request.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void ProcessRequest([NotNull] HttpContext context)
        {
            if (context.Session["lastvisit"] != null
                     ||
                     context.Request.UrlReferrer != null
                     && context.Request.UrlReferrer.AbsoluteUri.Contains(BaseUrlBuilder.BaseUrl))
            {
                // defaults
                var previewCropped = false;
                var localizationFile = "english.xml";

                if (context.Session["imagePreviewCropped"] is bool)
                {
                    previewCropped = context.Session["imagePreviewCropped"].ToType<bool>();
                }

                if (context.Session["localizationFile"] is string)
                {
                    localizationFile = context.Session["localizationFile"].ToString();
                }

                if (context.Session["localizationFile"] is string)
                {
                    localizationFile = context.Session["localizationFile"].ToString();
                }

                /////////////
                if (context.Request.QueryString.GetFirstOrDefault("twitterinfo") != null)
                {
                    this.GetTwitterUserInfo(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("userinfo") != null)
                {
                    this.GetUserInfo(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("bbcodelist") != null)
                {
                    this.GetCustomBBCodes(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("u") != null)
                {
                    this.GetResponseLocalAvatar(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("url") != null
                         && context.Request.QueryString.GetFirstOrDefault("width") != null
                         && context.Request.QueryString.GetFirstOrDefault("height") != null)
                {
                    this.GetResponseRemoteAvatar(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("a") != null)
                {
                    this.Get<IAttachment>().GetResponseAttachment(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("i") != null)
                {
                    var eTag = $@"""{context.Request.QueryString.GetFirstOrDefault("i")}""";

                    if (!CheckETag(context, eTag))
                    {
                        this.Get<IAttachment>().GetResponseImage(context);
                    }
                }
                else if (context.Request.QueryString.GetFirstOrDefault("p") != null)
                {
                    var eTag = $@"""{context.Request.QueryString.GetFirstOrDefault("p")}{localizationFile.GetHashCode()}""";

                    if (!CheckETag(context, eTag))
                    {
                        this.Get<IAlbum>().GetResponseImagePreview(context, localizationFile, previewCropped);
                    }
                }
                else if (context.Request.QueryString.GetFirstOrDefault("c") != null)
                {
                    // TommyB: End MOD: Preview Images   ##########
                    // captcha
                    this.GetResponseCaptcha(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("cover") != null
                         && context.Request.QueryString.GetFirstOrDefault("album") != null)
                {

                    var eTag = $@"""{context.Request.QueryString.GetFirstOrDefault("cover")}{localizationFile.GetHashCode()}""";

                    if (!CheckETag(context, eTag))
                    {
                        // album cover
                        this.Get<IAlbum>().GetAlbumCover(context, localizationFile, previewCropped);
                    }
                }
                else if (context.Request.QueryString.GetFirstOrDefault("imgprv") != null)
                {
                    // album image preview
                    var eTag =
                        $@"""{context.Request.QueryString.GetFirstOrDefault("imgprv")}{localizationFile.GetHashCode()}""";

                    if (!CheckETag(context, eTag))
                    {
                        this.Get<IAlbum>().GetAlbumImagePreview(context, localizationFile, previewCropped);
                    }

                }
                else if (context.Request.QueryString.GetFirstOrDefault("image") != null)
                {
                    var eTag = $@"""{context.Request.QueryString.GetFirstOrDefault("image")}""";

                    if (!CheckETag(context, eTag))
                    {
                        // album image
                        this.Get<IAlbum>().GetAlbumImage(context);
                    }
                }
            }
            else
            {
                // they don't have a session...
                context.Response.Write(
                    "Please do not link directly to this resource. You must have a session in the forum.");
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Check if the ETag that sent from the client is match to the current ETag.
        ///   If so, set the status code to 'Not Modified' and stop the response.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="eTagCode">
        /// The e Tag Code.
        /// </param>
        /// <returns>
        /// The check e tag.
        /// </returns>
        private static bool CheckETag([NotNull] HttpContext context, [NotNull] string eTagCode)
        {
            var ifNoneMatch = context.Request.Headers["If-None-Match"];

            if (!eTagCode.Equals(ifNoneMatch, StringComparison.Ordinal))
            {
                return false;
            }

            if (context.Request.QueryString.GetFirstOrDefault("v") != null)
            {
                return false;
            }

            context.Response.AppendHeader("Content-Length", "0");
            context.Response.StatusCode = HttpStatusCode.NotModified.ToType<int>();
            context.Response.StatusDescription = "Not modified";
            context.Response.SuppressContent = true;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetETag(eTagCode);
            context.Response.Flush();

            return true;
        }

        /// <summary>
        /// Gets the forum user info as JSON string for the hover cards
        /// </summary>
        /// <param name="context">The context.</param>
        private void GetUserInfo([NotNull] HttpContext context)
        {
            try
            {
                var userId = context.Request.QueryString.GetFirstOrDefault("userinfo").ToType<int>();

                var boardId = context.Request.QueryString.GetFirstOrDefault("boardId").ToType<int>();

                var user = UserMembershipHelper.GetMembershipUserById(userId, boardId);

                if (user == null || user.ProviderUserKey.ToString() == "0")
                {
                    context.Response.Write(
                   "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                    return;
                }

                // Check if user has access
                if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ProfileViewPermissions))
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
                    DateTime.UtcNow.AddMilliseconds(YafContext.Current.Get<YafBoardSettings>().OnlineStatusCacheTimeout));
                context.Response.Cache.SetLastModified(DateTime.UtcNow);

                var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userId);

                avatarUrl = avatarUrl.IsNotSet()
                           ? $"{YafForumInfo.ForumClientFileRoot}images/noavatar.gif"
                           : avatarUrl;

                var activeUsers = this.Get<IDataCache>().GetOrSet(
                    Constants.Cache.UsersOnlineStatus,
                    () =>
                    this.Get<YafDbBroker>().GetActiveList(
                        false, YafContext.Current.Get<YafBoardSettings>().ShowCrawlersInActiveList),
                    TimeSpan.FromMilliseconds(YafContext.Current.Get<YafBoardSettings>().OnlineStatusCacheTimeout));

                var userIsOnline =
                    activeUsers.AsEnumerable().Any(
                        x => x.Field<int>("UserId").Equals(userId) && !x.Field<bool>("IsHidden"));

                var userName = this.Get<YafBoardSettings>().EnableDisplayName ? userData.DisplayName : userData.UserName;

                userName = HttpUtility.HtmlEncode(userName);

                var location = userData.Profile.Country.IsSet()
                                   ? YafContext.Current.Get<IHaveLocalization>().GetText(
                                       "COUNTRY", userData.Profile.Country.Trim())
                                   : userData.Profile.Location;

                if (userData.Profile.Region.IsSet() && userData.Profile.Country.IsSet())
                {
                    var tag = $"RGN_{userData.Profile.Country.Trim()}_{userData.Profile.Region}";

                    location += $", {YafContext.Current.Get<IHaveLocalization>().GetText("REGION", tag)}";
                }

                var forumUrl = HttpUtility.UrlDecode(context.Request.QueryString.GetFirstOrDefault("forumUrl"));

                if (Config.IsMojoPortal)
                {
                    forumUrl = forumUrl + $"&g={ForumPages.pmessage}&u={userId}";
                }
                else
                {
                    forumUrl = forumUrl.Replace(".aspx", $".aspx?g={ForumPages.pmessage}&u={userId}");
                }

                var pmButton = new ThemeButton
                                   {
                                       ID = "PM",
                                       CssClass = "yafcssimagebutton",
                                       TextLocalizedPage = "POSTS",
                                       TextLocalizedTag = "PM",
                                       Type = ButtonAction.Secondary,
                                       Icon = "envelope-open-text",
                                       TitleLocalizedTag = "PM_TITLE",
                                       TitleLocalizedPage = "POSTS",
                                       NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, true, "u={0}", userId).Replace(
                                               "resource.ashx", "default.aspx"),
                                       ParamTitle0 = userName,
                                       Visible =
                                           !userData.IsGuest && this.Get<YafBoardSettings>().AllowPrivateMessages
                                           && !userId.Equals(YafContext.Current.PageUserID) && !YafContext.Current.IsGuest
                                   };

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
                                       Joined = this.Get<IDateTime>().FormatDateLong(userData.Joined),
                                       Online = userIsOnline,
                                       ActionButtons = pmButton.RenderToString()
                                   };

                if (YafContext.Current.Get<YafBoardSettings>().EnableUserReputation)
                {
                    userInfo.Points = (userData.Points.ToType<int>() > 0 ? "+" : string.Empty) + userData.Points;
                }

                context.Response.Write(userInfo.ToJson());

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(YafContext.Current.PageUserID, this, x, EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// Gets the list of all Custom BB Codes
        /// </summary>
        /// <param name="context">The context.</param>
        private void GetCustomBBCodes([NotNull] HttpContext context)
        {
            try
            {
                if (YafContext.Current == null)
                {
                    context.Response.Write(
                   "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                    return;
                }

                var customBbCode = this.Get<YafDbBroker>().GetCustomBBCode().ToList();

                context.Response.Clear();

                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(
                    DateTime.UtcNow.AddMilliseconds(YafContext.Current.Get<YafBoardSettings>().OnlineStatusCacheTimeout));
                context.Response.Cache.SetLastModified(DateTime.UtcNow);

                context.Response.Write(customBbCode.ToJson());

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(YafContext.Current.PageUserID, this, x, EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// Gets the twitter user info as JSON string for the hover cards
        /// </summary>
        /// <param name="context">The context.</param>
        private void GetTwitterUserInfo([NotNull] HttpContext context)
        {
            try
            {
                var twitterName = context.Request.QueryString.GetFirstOrDefault("twitterinfo").ToType<string>();

                if (!Config.IsTwitterEnabled)
                {
                    context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                    return;
                }

                var oAuth = new OAuthTwitter
                {
                    ConsumerKey = Config.TwitterConsumerKey,
                    ConsumerSecret = Config.TwitterConsumerSecret,
                    Token = Config.TwitterToken,
                    TokenSecret = Config.TwitterTokenSecret
                };

                var tweetAPI = new TweetAPI(oAuth);

                context.Response.Write(tweetAPI.UsersLookupJson(twitterName));

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(YafContext.Current.PageUserID, this, x, EventLogTypes.Information);

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
        private void GetResponseLocalAvatar([NotNull] HttpContext context)
        {
            try
            {
                var user = YafContext.Current.GetRepository<User>()
                    .GetById(context.Request.QueryString.GetFirstOrDefault("u").ToType<int>());

                if (user != null)
                {
                    var data = user.AvatarImage;
                    var contentType = user.AvatarImageType;

                    context.Response.Clear();
                    if (contentType.IsNotSet())
                    {
                        contentType = "image/jpeg";
                    }

                    context.Response.ContentType = contentType;
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
                    context.Response.Cache.SetLastModified(DateTime.UtcNow);

                    // context.Response.Cache.SetETag( eTag );
                    context.Response.OutputStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                   .Log(
                       YafContext.Current.PageUserID,
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
        private void GetResponseCaptcha([NotNull] HttpContext context)
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
                this.Get<ILogger>().Log(YafContext.Current.PageUserID, this, x);
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
        private void GetResponseRemoteAvatar([NotNull] HttpContext context)
        {
            var avatarUrl = context.Request.QueryString.GetFirstOrDefault("url");

            if (avatarUrl.StartsWith("/"))
            {
                var basePath = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}";

                avatarUrl = $"{basePath}{avatarUrl}";
            }

            var maxwidth = int.Parse(context.Request.QueryString.GetFirstOrDefault("width"));
            var maxheight = int.Parse(context.Request.QueryString.GetFirstOrDefault("height"));

            var eTag =
                $@"""{(context.Request.QueryString.GetFirstOrDefault("url") + maxheight + maxwidth).GetHashCode()}""";

            if (CheckETag(context, eTag))
            {
                // found eTag... no need to download this image...
                return;
            }

            var webClient = new WebClient { Credentials = CredentialCache.DefaultCredentials };

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => true;

                var originalData = webClient.DownloadData(avatarUrl);

                using (var avatarStream = new MemoryStream(originalData))
                {
                    using (var img = new Bitmap(avatarStream))
                    {
                        var width = img.Width;
                        var height = img.Height;

                        if (width <= maxwidth && height <= maxheight)
                        {
                            context.Response.Redirect(avatarUrl);
                        }

                        if (width > maxwidth)
                        {
                            height = (height / (double)width * maxwidth).ToType<int>();
                            width = maxwidth;
                        }

                        if (height > maxheight)
                        {
                            width = (width / (double)height * maxheight).ToType<int>();
                            height = maxheight;
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
                            context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
                            context.Response.Cache.SetLastModified(DateTime.UtcNow);
                            context.Response.Cache.SetETag(eTag);
                            bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                // issue getting access to the avatar...
                this.Get<ILogger>().Log(
                    YafContext.Current.PageUserID,
                    this,
                    $"URL: {avatarUrl}<br />Referer URL: {context.Request.UrlReferrer?.AbsoluteUri ?? string.Empty}<br />Exception: {exception}");

                // Output the data
                context.Response.Redirect(avatarUrl);

                /*context.Response.Redirect(
                                    "{0}/Images/{1}".FormatWith(YafForumInfo.ForumClientFileRoot, "noavatar.gif"));*/
            }
        }

        #endregion
    }
}