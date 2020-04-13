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

namespace YAF.Core.Handlers
{
    #region Using

    using System;
    using System.Web;
    using System.Xml.Serialization;

    using YAF.Core;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    using SiteMap = YAF.Types.Objects.SiteMap;

    #endregion

    /// <summary>
    /// The site map handler.
    /// </summary>
    public class SiteMapHandler : IHttpHandler, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsReusable.
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

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
            var siteMap = new SiteMap();

            var forumList = this.GetRepository<Forum>().ListAll(
                BoardContext.Current.BoardSettings.BoardID,
                UserMembershipHelper.GuestUserId);

            forumList.ForEach(
                forum =>
                    {
                        siteMap.Add(
                            new UrlLocation
                                {
                                    Url = BuildLink.GetLinkNotEscaped(
                                        ForumPages.topics,
                                        true,
                                        "f={0}",
                                        forum.Item1.ID),
                                    Priority = 0.8D,
                                    LastModified = forum.Item1.LastPosted ?? DateTime.UtcNow,
                                    ChangeFrequency = UrlLocation.ChangeFrequencies.always
                                });
                    });

            context.Response.Clear();
            var xs = new XmlSerializer(typeof(SiteMap));
            context.Response.ContentType = "text/xml";
            xs.Serialize(context.Response.Output, siteMap);
            context.Response.End();
        }

        #endregion

        #endregion
    }
}