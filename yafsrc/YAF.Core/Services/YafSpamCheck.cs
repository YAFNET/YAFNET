/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Core.Services
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Web;

    using YAF.Classes;
    using YAF.Core.Services.CheckForSpam;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Yaf Spam Checking
    /// </summary>
    public class YafSpamCheck
    {
        /// <summary>
        /// Check a Post for SPAM against the BlogSpam.NET API or Akismet Service
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="postSubject">The post subject.</param>
        /// <param name="postMessage">The post message.</param>
        /// <returns>
        /// Returns if Post is SPAM or not
        /// </returns>
        public static bool IsPostSpam([NotNull]string userName, [NotNull]string postSubject, [NotNull]string postMessage)
        {
            if (YafContext.Current.Get<YafBoardSettings>().SpamServiceType.Equals(0))
            {
                return false;
            }
            

            string ipAdress = YafContext.Current.Get<HttpRequestBase>().UserHostAddress;
            bool isLocal = YafContext.Current.Get<HttpRequestBase>().IsLocal;

            string whiteList = string.Empty;

            if (isLocal)
            {
                whiteList = "whitelist=127.0.0.1";
            }

            // Use BlogSpam.NET API
            if (YafContext.Current.Get<YafBoardSettings>().SpamServiceType.Equals(1))
            {
                try
                {
                    return
                        BlogSpamNet.CommentIsSpam(
                            new BlogSpamComment
                            {
                                comment = postMessage,
                                ip = ipAdress,
                                agent = YafContext.Current.Get<HttpRequestBase>().UserAgent,
                                name = userName,
                                options = whiteList,
                            },
                            true);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            // Use Akismet API
            if (YafContext.Current.Get<YafBoardSettings>().SpamServiceType.Equals(2) && !string.IsNullOrEmpty(YafContext.Current.Get<YafBoardSettings>().AkismetApiKey))
            {
                try
                {
                    var service = new AkismetSpamClient(YafContext.Current.Get<YafBoardSettings>().AkismetApiKey, new Uri(BaseUrlBuilder.BaseUrl));

                    return
                        service.CheckCommentForSpam(
                            new Comment(IPAddress.Parse(ipAdress), YafContext.Current.Get<HttpRequestBase>().UserAgent)
                            {
                                Content = postMessage,
                                Author = userName,
                            });
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
