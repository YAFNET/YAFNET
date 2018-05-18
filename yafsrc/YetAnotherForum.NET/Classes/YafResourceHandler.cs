/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.SessionState;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Services.Auth;
    using YAF.Core.Services.Localization;
    using YAF.Core.Services.Startup;
    using YAF.Modules.BBCode;
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
            if (context.Request.QueryString.GetFirstOrDefault("r") != null)
            {
                // resource request
                GetResource(context);
            }
            else if (context.Session["lastvisit"] != null
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
                    this.GetResponseAttachment(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("i") != null)
                {
                    // TommyB: Start MOD: Preview Images   ##########
                    this.GetResponseImage(context);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("p") != null)
                {
                    this.GetResponseImagePreview(context, localizationFile, previewCropped);
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
                    // album cover
                    this.GetAlbumCover(context, localizationFile, previewCropped);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("imgprv") != null)
                {
                    // album image preview
                    this.GetAlbumImagePreview(context, localizationFile, previewCropped);
                }
                else if (context.Request.QueryString.GetFirstOrDefault("image") != null)
                {
                    // album image
                    this.GetAlbumImage(context);
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
        /// Get the Album Or Image Attachment Preview
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="previewWidth">The preview width.</param>
        /// <param name="previewHeight">The preview height.</param>
        /// <param name="previewCropped">The preview Cropped</param>
        /// <param name="downloads">The downloads.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="localizationPage">The localization page.</param>
        /// <returns>
        /// Resized Image Stream
        /// </returns>
        [NotNull]
        private static MemoryStream GetAlbumOrAttachmentImageResized(
            [NotNull] Stream data,
            int previewWidth,
            int previewHeight,
            bool previewCropped,
            int downloads,
            [NotNull] string localizationFile,
            string localizationPage)
        {
            const int PixelPadding = 6;
            const int BottomSize = 26;

            var localization = new YafLocalization(localizationPage);
            localization.LoadTranslation(localizationFile);

            using (var src = new Bitmap(data))
            {
                var ms = new MemoryStream();

                var newImgSize = new Size(previewWidth, previewHeight);

                if (previewCropped)
                {
                    var width = (float)newImgSize.Width;
                    var height = (float)newImgSize.Height;

                    var xRatio = width / src.Width;
                    var yRatio = height / src.Height;

                    var ratio = Math.Min(xRatio, yRatio);

                    newImgSize =
                        new Size(
                            Math.Min(
                                newImgSize.Width,
                                Math.Round(src.Width * ratio, MidpointRounding.AwayFromZero).ToType<int>()),
                            Math.Min(
                                newImgSize.Height,
                                Math.Round(src.Height * ratio, MidpointRounding.AwayFromZero).ToType<int>()));

                    newImgSize.Width = newImgSize.Width - PixelPadding;
                    newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;
                }
                else
                {
                    var finalHeight = Math.Abs(src.Height * newImgSize.Width / src.Width);

                    // Height resize if necessary
                    if (finalHeight > newImgSize.Height)
                    {
                        newImgSize.Width = src.Width * newImgSize.Height / src.Height;
                        finalHeight = newImgSize.Height;
                    }

                    newImgSize.Height = finalHeight;
                    newImgSize.Width = newImgSize.Width - PixelPadding;
                    newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;

                    if (newImgSize.Height <= BottomSize + PixelPadding)
                    {
                        newImgSize.Height = finalHeight;
                    }
                }

                var heightToSmallFix = newImgSize.Height <= BottomSize + PixelPadding;

                using (
                    var dst = new Bitmap(
                        newImgSize.Width + PixelPadding,
                        newImgSize.Height + BottomSize + PixelPadding,
                        PixelFormat.Format24bppRgb))
                {
                    var rSrcImg = new Rectangle(
                        0, 0, src.Width, src.Height + (heightToSmallFix ? BottomSize + PixelPadding : 0));

                    if (previewCropped)
                    {
                        rSrcImg = new Rectangle(0, 0, newImgSize.Width, newImgSize.Height);
                    }

                    var rDstImg = new Rectangle(3, 3, dst.Width - PixelPadding, dst.Height - PixelPadding - BottomSize);
                    var rDstTxt1 = new Rectangle(3, rDstImg.Height + 3, newImgSize.Width, BottomSize - 13);
                    var rDstTxt2 = new Rectangle(3, rDstImg.Height + 16, newImgSize.Width, BottomSize - 13);

                    using (var g = Graphics.FromImage(dst))
                    {
                        g.Clear(Color.FromArgb(64, 64, 64));
                        g.FillRectangle(Brushes.White, rDstImg);

                        g.CompositingMode = CompositingMode.SourceOver;
                        g.CompositingQuality = CompositingQuality.GammaCorrected;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                        g.DrawImage(src, rDstImg, rSrcImg, GraphicsUnit.Pixel);

                        using (var f = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            using (var brush = new SolidBrush(Color.FromArgb(191, 191, 191)))
                            {
                                var sf = new StringFormat
                                    {
                                       Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center 
                                    };

                                g.DrawString(localization.GetText("IMAGE_RESIZE_ENLARGE"), f, brush, rDstTxt1, sf);

                                sf.Alignment = StringAlignment.Far;
                                g.DrawString(
                                    localization.GetText("IMAGE_RESIZE_VIEWS").FormatWith(downloads),
                                    f,
                                    brush,
                                    rDstTxt2,
                                    sf);
                            }
                        }
                    }

                    // save the bitmap to the stream...
                    dst.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    return ms;
                }
            }
        }

        /// <summary>
        /// The get resource.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void GetResource([NotNull] HttpContext context)
        {
            // redirect to the resource?
            context.Response.Redirect("resources/{0}".FormatWith(context.Request.QueryString.GetFirstOrDefault("r")));

            /*string resourceName = "YAF.App_GlobalResources." + context.Request.QueryString["r"];
            int lastIndex = resourceName.LastIndexOf('.');
            string extension = resourceName.Substring(lastIndex, resourceName.Length - lastIndex).ToLower();

            string resourceType = "text/plain";

            switch (extension)
            {
                case ".js":
                    resourceType = "application/x-javascript";
                    break;
                case ".css":
                    resourceType = "text/css";
                    break;
            }

            if (resourceType != string.Empty)
            {
                context.Response.Clear();
                context.Response.ContentType = resourceType;

                try
                {
                    // attempt to load the resource...
                    byte[] data = null;

                    Stream input = GetType().Assembly.GetManifestResourceStream(resourceName);

                    data = new byte[input.Length];
                    input.Read(data, 0, data.Length);
                    input.Close();

                    context.Response.OutputStream.Write(data, 0, data.Length);
                }
                catch
                {
                    YAF.Classes.Data.DB.eventlog_create(
                        null, GetType().ToString(), "Attempting to access invalid resource: " + resourceName, 1);
                    context.Response.Write("Error: Invalid forum resource. Please contact the forum admin.");
                }
            }*/
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
                           ? "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot)
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
                    var tag = "RGN_{0}_{1}".FormatWith(userData.Profile.Country.Trim(), userData.Profile.Region);

                    location += ", {0}".FormatWith(YafContext.Current.Get<IHaveLocalization>().GetText("REGION", tag));
                }

                var forumUrl = HttpUtility.UrlDecode(context.Request.QueryString.GetFirstOrDefault("forumUrl"));

                if (Config.IsMojoPortal)
                {
                    forumUrl = forumUrl + "&g={0}&u={1}".FormatWith(ForumPages.pmessage, userId);
                }
                else
                {
                    forumUrl = forumUrl.Replace(".aspx", ".aspx?g={0}&u={1}".FormatWith(ForumPages.pmessage, userId));
                }

                var pmButton = new ThemeButton
                                   {
                                       ID = "PM",
                                       CssClass = "yafcssimagebutton",
                                       TextLocalizedPage = "POSTS",
                                       TextLocalizedTag = "PM",
                                       ImageThemeTag = "PM",
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

                                       /*profilelink =
                                                                                  Config.IsAnyPortal ? userlinkUrl : YafBuildLink.GetLink(ForumPages.profile, true, "u={0}&name={1}", userId).Replace(
                                                                                      "resource.ashx", "default.aspx"),*/
                                       Interests = HttpUtility.HtmlEncode(userData.Profile.Interests),
                                       HomePage = userData.Profile.Homepage,
                                       Posts = "{0:N0}".FormatWith(userData.NumPosts),
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
                       "URL: {0}<br />Referer URL: {1}<br />Exception: {2}".FormatWith(
                           context.Request.Url,
                           context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty,
                           x),
                       EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// Checks the access rights.
        /// </summary>
        /// <param name="boardID">The board id.</param>
        /// <param name="messageID">The message id.</param>
        /// <returns>
        /// The check access rights.
        /// </returns>
        private bool CheckAccessRights([NotNull] int boardID, [NotNull] int messageID)
        {
            if (messageID.Equals(0))
            {
                return true;
            }

            // Find user name
            var user = UserMembershipHelper.GetUser();

            var browser = "{0} {1}".FormatWith(
                HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version);
            var platform = HttpContext.Current.Request.Browser.Platform;
            var isMobileDevice = HttpContext.Current.Request.Browser.IsMobileDevice;
            bool isSearchEngine;
            bool dontTrack;
            var userAgent = HttpContext.Current.Request.UserAgent;

            // try and get more verbose platform name by ref and other parameters             
            UserAgentHelper.Platform(
                userAgent,
                HttpContext.Current.Request.Browser.Crawler,
                ref platform,
                ref browser,
                out isSearchEngine,
                out dontTrack);

            this.Get<StartupInitializeDb>().Run();

            object userKey = DBNull.Value;

            if (user != null)
            {
                userKey = user.ProviderUserKey;
            }

            var pageRow = LegacyDb.pageload(
                HttpContext.Current.Session.SessionID,
                boardID,
                userKey,
                HttpContext.Current.Request.GetUserRealIPAddress(),
                HttpContext.Current.Request.FilePath,
                HttpContext.Current.Request.QueryString.ToString(),
                browser,
                platform,
                null,
                null,
                null,
                messageID,
                isSearchEngine, // don't track if this is a search engine
                isMobileDevice,
                dontTrack);

            return pageRow["DownloadAccess"].ToType<bool>() || pageRow["ModeratorAccess"].ToType<bool>();
        }

        /// <summary>
        /// The get album cover.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
        private void GetAlbumCover(
            [NotNull] HttpContext context,
            string localizationFile,
            bool previewCropped)
        {
            var eTag = @"""{0}{1}""".FormatWith(
                context.Request.QueryString.GetFirstOrDefault("cover"), localizationFile.GetHashCode());

            if (CheckETag(context, eTag))
            {
                // found eTag... no need to resend/create this image...
                return;
            }

            try
            {
                // CoverID
                var fileName = string.Empty;
                var data = new MemoryStream();
                if (context.Request.QueryString.GetFirstOrDefault("cover") == "0")
                {
                    fileName =
                        context.Server.MapPath(
                            "{0}/images/{1}".FormatWith(YafForumInfo.ForumClientFileRoot, "noCover.png"));
                }
                else
                {
                    using (
                        var dt = LegacyDb.album_image_list(
                            null, context.Request.QueryString.GetFirstOrDefault("cover")))
                    {
                        if (dt.HasRows())
                        {
                            var row = dt.Rows[0];
                            var sUpDir = YafBoardFolders.Current.Uploads;

                            var oldFileName =
                                context.Server.MapPath(
                                    "{0}/{1}.{2}.{3}".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                            var newFileName =
                                context.Server.MapPath(
                                    "{0}/{1}.{2}.{3}.yafalbum".FormatWith(
                                        sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

                            // use the new fileName (with extension) if it exists...
                            fileName = File.Exists(newFileName) ? newFileName : oldFileName;
                        }
                    }
                }

                using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[input.Length];
                    input.Read(buffer, 0, buffer.Length);
                    data.Write(buffer, 0, buffer.Length);
                    input.Close();
                }

                // reset position...
                data.Position = 0;
                var imagesNumber =
                    LegacyDb.album_getstats(null, context.Request.QueryString.GetFirstOrDefault("album"))[1];
                var ms = GetAlbumOrAttachmentImageResized(
                    data,
                    this.Get<YafBoardSettings>().ImageAttachmentResizeWidth,
                    this.Get<YafBoardSettings>().ImageAttachmentResizeHeight,
                    previewCropped,
                    imagesNumber,
                    localizationFile,
                    "ALBUM");

                context.Response.ContentType = "image/png";

                // output stream...
                context.Response.OutputStream.Write(ms.ToArray(), 0, ms.Length.ToType<int>());
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
                context.Response.Cache.SetLastModified(DateTime.UtcNow);
                context.Response.Cache.SetETag(eTag);

                data.Dispose();
                ms.Dispose();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(YafContext.Current.PageUserID, this, x, EventLogTypes.Information);
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// The get album image.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private void GetAlbumImage([NotNull] HttpContext context)
        {
            try
            {
                var eTag = @"""{0}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("image"));

                if (CheckETag(context, eTag))
                {
                    // found eTag... no need to resend/create this image -- just mark another view?
                    // YAF.Classes.Data.DB.album_image_download(context.Request.QueryString.GetFirstOrDefault("image"));
                    return;
                }

                /*var image = this.GetRepository<UserAlbumImage>()
                    .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("image"));*/
                
                // ImageID
                using (
                    var dt = LegacyDb.album_image_list(
                        null, context.Request.QueryString.GetFirstOrDefault("image")))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        byte[] data;

                        var sUpDir = YafBoardFolders.Current.Uploads;

                        var oldFileName =
                            context.Server.MapPath(
                                "{0}/{1}.{2}.{3}".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                        var newFileName =
                            context.Server.MapPath(
                                "{0}/{1}.{2}.{3}.yafalbum".FormatWith(
                                    sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

                        // use the new fileName (with extension) if it exists...
                        var fileName = File.Exists(newFileName) ? newFileName : oldFileName;

                        using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            data = new byte[input.Length];
                            input.Read(data, 0, data.Length);
                            input.Close();
                        }

                        context.Response.ContentType = row["ContentType"].ToString();

                        if (context.Response.ContentType.Contains("text"))
                        {
                            context.Response.Write(
                                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
                        }
                        else
                        {
                            // context.Response.Cache.SetCacheability(HttpCacheability.Public);
                            // context.Response.Cache.SetETag(eTag);
                            context.Response.OutputStream.Write(data, 0, data.Length);

                            // add a download count...
                            this.GetRepository<UserAlbumImage>().IncrementDownload(
                                context.Request.QueryString.GetFirstOrDefaultAs<int>("image"));
                        }
                        
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                   .Log(
                       YafContext.Current.PageUserID,
                       this,
                       "URL: {0}<br />Referer URL: {1}<br />Exception: {2}".FormatWith(
                           context.Request.Url,
                           context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty,
                           x),
                       EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// The get album image preview.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
        private void GetAlbumImagePreview(
            [NotNull] HttpContext context,
            string localizationFile,
            bool previewCropped)
        {
            var eTag = @"""{0}{1}""".FormatWith(
                context.Request.QueryString.GetFirstOrDefault("imgprv"), localizationFile.GetHashCode());

            if (CheckETag(context, eTag))
            {
                // found eTag... no need to resend/create this image...
                return;
            }

            try
            {
                // ImageID
                using (
                    var dt = LegacyDb.album_image_list(
                        null, context.Request.QueryString.GetFirstOrDefault("imgprv")))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var data = new MemoryStream();

                        var sUpDir = YafBoardFolders.Current.Uploads;

                        var oldFileName =
                            context.Server.MapPath(
                                "{0}/{1}.{2}.{3}".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                        var newFileName =
                            context.Server.MapPath(
                                "{0}/{1}.{2}.{3}.yafalbum".FormatWith(
                                    sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

                        // use the new fileName (with extension) if it exists...
                        var fileName = File.Exists(newFileName) ? newFileName : oldFileName;

                        using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var buffer = new byte[input.Length];
                            input.Read(buffer, 0, buffer.Length);
                            data.Write(buffer, 0, buffer.Length);
                            input.Close();
                        }

                        // reset position...
                        data.Position = 0;

                        var ms = GetAlbumOrAttachmentImageResized(
                            data,
                            this.Get<YafBoardSettings>().ImageAttachmentResizeWidth,
                            this.Get<YafBoardSettings>().ImageAttachmentResizeHeight,
                            previewCropped,
                            row["Downloads"].ToType<int>(),
                            localizationFile,
                            "POSTS");

                        context.Response.ContentType = "image/png";

                        // output stream...
                        context.Response.OutputStream.Write(ms.ToArray(), 0, ms.Length.ToType<int>());
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
                        context.Response.Cache.SetLastModified(DateTime.UtcNow);
                        context.Response.Cache.SetETag(eTag);

                        data.Dispose();
                        ms.Dispose();

                        break;
                    }
                }
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        this,
                        "URL: {0}<br />Referer URL: {1}<br />Exception: {2}".FormatWith(
                            context.Request.Url,
                            context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty,
                            x),
                        EventLogTypes.Information);
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// The get response attachment.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private void GetResponseAttachment([NotNull] HttpContext context)
        {
            try
            {
                // AttachmentID
                var attachment =
                    this.GetRepository<Attachment>()
                        .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("a"));

                var boardID = context.Request.QueryString.GetFirstOrDefault("b") != null
                                  ? context.Request.QueryString.GetFirstOrDefaultAs<int>("b")
                                  : YafContext.Current.BoardSettings.BoardID;

                if (!this.CheckAccessRights(boardID, attachment.MessageID))
                {
                    // tear it down
                    // no permission to download
                    context.Response.Write(
                        "You have insufficient rights to download this resource. Contact forum administrator for further details.");
                    return;
                }

                byte[] data;


                this.GetRepository<Attachment>()
                    .IncrementDownloadCounter(attachment.ID);

                if (attachment.FileData == null)
                {
                    var uploadFolder = YafBoardFolders.Current.Uploads;

                    var oldFileName =
                        context.Server.MapPath(
                            "{0}/{1}.{2}".FormatWith(
                                uploadFolder,
                                attachment.MessageID > 0
                                    ? attachment.MessageID.ToString()
                                    : "u{0}".FormatWith(attachment.UserID),
                                attachment.FileName));

                    var newFileName =
                        context.Server.MapPath(
                            "{0}/{1}.{2}.yafupload".FormatWith(
                                uploadFolder,
                                attachment.MessageID > 0
                                    ? attachment.MessageID.ToString()
                                    : "u{0}-{1}".FormatWith(attachment.UserID, attachment.ID),
                                attachment.FileName));

                    var fileName = oldFileName;

                    if (File.Exists(oldFileName))
                    {
                        fileName = oldFileName;
                    }
                    else
                    {
                        oldFileName =
                        context.Server.MapPath(
                            "{0}/{1}.{2}.yafupload".FormatWith(
                                uploadFolder,
                                attachment.MessageID > 0
                                    ? attachment.MessageID.ToString()
                                    : "u{0}".FormatWith(attachment.UserID),
                                attachment.FileName));

                        // use the new fileName (with extension) if it exists...
                        fileName = File.Exists(newFileName) ? newFileName : oldFileName;

                        // its an old extension
                        if (!File.Exists(fileName))
                        {
                            fileName = context.Server.MapPath(
                                "{0}/{1}.{2}.yafupload".FormatWith(
                                    uploadFolder,
                                    attachment.MessageID.ToString(),
                                    attachment.FileName));
                        }
                    }

                    using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        data = input.ToArray();
                        input.Close();
                    }
                }
                else
                {
                    data = attachment.FileData;
                }

                context.Response.ContentType = attachment.ContentType;
                context.Response.AppendHeader(
                    "Content-Disposition",
                    "attachment; filename={0}".FormatWith(
                        HttpUtility.UrlPathEncode(attachment.FileName).Replace("+", "_")));
                context.Response.OutputStream.Write(data, 0, data.Length);
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        this,
                        "URL: {0}<br />Referer URL: {1}<br />Exception: {2}".FormatWith(
                            context.Request.Url,
                            context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty,
                            x),
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

        // TommyB: Start MOD: PreviewImages   ##########

        /// <summary>
        /// Gets the response image.
        /// </summary>
        /// <param name="context">The context.</param>
        private void GetResponseImage([NotNull] HttpContext context)
        {
            try
            {
                var eTag = @"""{0}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("i"));

                // AttachmentID
                var attachment =
                    this.GetRepository<Attachment>()
                        .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("i"));


                if (context.Request.QueryString.GetFirstOrDefault("editor") == null)
                {
                    // add a download count...
                    this.GetRepository<Attachment>()
                        .IncrementDownloadCounter(attachment.ID);
                }

                if (CheckETag(context, eTag))
                {
                    // found eTag... no need to resend/create this image 
                   return;
                }

                var boardID = context.Request.QueryString.GetFirstOrDefault("b") != null
                                  ? context.Request.QueryString.GetFirstOrDefaultAs<int>("b")
                                  : YafContext.Current.BoardSettings.BoardID;

                // check download permissions here
                if (!this.CheckAccessRights(boardID, attachment.MessageID))
                {
                    // tear it down
                    // no permission to download
                    context.Response.Write(
                        "You have insufficient rights to download this resource. Contact forum administrator for further details.");
                    return;
                }

                byte[] data;

                if (attachment.FileData == null)
                {
                    var uploadFolder = YafBoardFolders.Current.Uploads;

                    var oldFileName =
                         context.Server.MapPath(
                             "{0}/{1}.{2}".FormatWith(
                                 uploadFolder,
                                 attachment.MessageID > 0
                                     ? attachment.MessageID.ToString()
                                     : "u{0}".FormatWith(attachment.UserID),
                                 attachment.FileName));

                    var newFileName =
                        context.Server.MapPath(
                            "{0}/{1}.{2}.yafupload".FormatWith(
                                uploadFolder,
                                attachment.MessageID > 0
                                    ? attachment.MessageID.ToString()
                                    : "u{0}-{1}".FormatWith(attachment.UserID, attachment.ID),
                                attachment.FileName));

                    var fileName = oldFileName;

                    if (File.Exists(oldFileName))
                    {
                        fileName = oldFileName;

                    }
                    else
                    {
                        oldFileName =
                        context.Server.MapPath(
                            "{0}/{1}.{2}.yafupload".FormatWith(
                                uploadFolder,
                                attachment.MessageID > 0
                                    ? attachment.MessageID.ToString()
                                    : "u{0}".FormatWith(attachment.UserID),
                                attachment.FileName));

                        // use the new fileName (with extension) if it exists...
                        fileName = File.Exists(newFileName) ? newFileName : oldFileName;
                    }

                    using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        data = input.ToArray();
                        input.Close();
                    }
                }
                else
                {
                    data = attachment.FileData;
                }

                context.Response.ContentType = attachment.ContentType;
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetETag(eTag);
                context.Response.OutputStream.Write(data, 0, data.Length);
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        this,
                        "URL: {0}<br />Referer URL: {1}<br />Exception: {2}".FormatWith(
                            context.Request.Url,
                            context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty,
                            x),
                        EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// Gest the Preview Image as Response
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
        private void GetResponseImagePreview(
            [NotNull] HttpContext context,
            string localizationFile,
            bool previewCropped)
        {
            var eTag = @"""{0}{1}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("p"), localizationFile.GetHashCode());

            if (CheckETag(context, eTag))
            {
                // found eTag... no need to resend/create this image...
                return;
            }

            // defaults
            var previewMaxWidth = 200;
            var previewMaxHeight = 200;

            if (context.Session["imagePreviewWidth"] is int)
            {
                previewMaxWidth = context.Session["imagePreviewWidth"].ToType<int>();
            }

            if (context.Session["imagePreviewHeight"] is int)
            {
                previewMaxHeight = context.Session["imagePreviewHeight"].ToType<int>();
            }

            try
            {
                // AttachmentID
                var attachment =
                    this.GetRepository<Attachment>()
                        .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("p"));

                var boardID = context.Request.QueryString.GetFirstOrDefault("b") != null
                                  ? context.Request.QueryString.GetFirstOrDefaultAs<int>("b")
                                  : YafContext.Current.BoardSettings.BoardID;

                if (!this.CheckAccessRights(boardID, attachment.MessageID))
                {
                    // tear it down
                    // no permission to download
                    context.Response.Write(
                        "You have insufficient rights to download this resource. Contact forum administrator for further details.");
                    return;
                }

                var data = new MemoryStream();

                if (attachment.FileData == null)
                {
                    var uploadFolder = YafBoardFolders.Current.Uploads;

                    var oldFileName =
                         context.Server.MapPath(
                             "{0}/{1}.{2}".FormatWith(
                                 uploadFolder,
                                 attachment.MessageID > 0
                                     ? attachment.MessageID.ToString()
                                     : "u{0}".FormatWith(attachment.UserID),
                                 attachment.FileName));

                    var newFileName =
                        context.Server.MapPath(
                            "{0}/{1}.{2}.yafupload".FormatWith(
                                uploadFolder,
                                attachment.MessageID > 0
                                    ? attachment.MessageID.ToString()
                                    : "u{0}-{1}".FormatWith(attachment.UserID, attachment.ID),
                                attachment.FileName));

                    var fileName = oldFileName;

                    if (File.Exists(oldFileName))
                    {
                        fileName = oldFileName;
                    }
                    else
                    {
                        oldFileName =
                        context.Server.MapPath(
                            "{0}/{1}.{2}.yafupload".FormatWith(
                                uploadFolder,
                                attachment.MessageID > 0
                                    ? attachment.MessageID.ToString()
                                    : "u{0}".FormatWith(attachment.UserID),
                                attachment.FileName));

                        // use the new fileName (with extension) if it exists...
                        fileName = File.Exists(newFileName) ? newFileName : oldFileName;

                        // Find wrongly converted attachments
                        if (!File.Exists(fileName) && attachment.MessageID.Equals(0))
                        {
                            var file = Directory.EnumerateFiles(context.Server.MapPath(uploadFolder)).FirstOrDefault(
                                f => f.Contains("{0}.yafupload".FormatWith(attachment.FileName)));

                            fileName = file;
                        }
                    }

                    using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var buffer = new byte[input.Length];
                        input.Read(buffer, 0, buffer.Length);
                        data.Write(buffer, 0, buffer.Length);
                        input.Close();
                    }
                }
                else
                {
                    var buffer = attachment.FileData;
                    data.Write(buffer, 0, buffer.Length);
                }

                // reset position...
                data.Position = 0;

                var ms = GetAlbumOrAttachmentImageResized(
                    data,
                    previewMaxWidth,
                    previewMaxHeight,
                    previewCropped,
                    attachment.Downloads,
                    localizationFile,
                    "POSTS");

                context.Response.ContentType = "image/png";

                // output stream...
                context.Response.OutputStream.Write(ms.ToArray(), 0, ms.Length.ToType<int>());
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
                context.Response.Cache.SetLastModified(DateTime.UtcNow);
                context.Response.Cache.SetETag(eTag);

                data.Dispose();
                ms.Dispose();
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        this,
                        "URL: {0}<br />Referer URL: {1}<br />Exception: {2}".FormatWith(
                            context.Request.Url,
                            context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty,
                            x),
                        EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
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
                var basePath = "{0}://{1}".FormatWith(
                    HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.Host);

                avatarUrl = "{0}{1}".FormatWith(basePath, avatarUrl);
            }

            var maxwidth = int.Parse(context.Request.QueryString.GetFirstOrDefault("width"));
            var maxheight = int.Parse(context.Request.QueryString.GetFirstOrDefault("height"));

            var eTag = @"""{0}""".FormatWith(
                (context.Request.QueryString.GetFirstOrDefault("url") + maxheight + maxwidth).GetHashCode());

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
                    "URL: {0}<br />Referer URL: {1}<br />Exception: {2}".FormatWith(
                        avatarUrl,
                        context.Request.UrlReferrer?.AbsoluteUri ?? string.Empty,
                        exception));

                // Output the data
                context.Response.Redirect(
                    "{0}/Images/{1}".FormatWith(YafForumInfo.ForumClientFileRoot, "noavatar.gif"));
            }
        }

        #endregion
    }
}