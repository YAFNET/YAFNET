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
namespace YAF.Core.URLBuilder
{
    #region Using

    using System;

    using YAF.Classes;
    using YAF.Core.Helpers;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The advanced rewrite url builder.
    /// </summary>
    public class AdvancedUrlRewriter : BaseUrlBuilder
    {
        #region Public Methods

        /// <summary>
        /// Build the url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// Returns the URL.
        /// </returns>
        public override string BuildUrl(string url)
        {
            var newUrl = "{0}{1}?{2}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName, url);

            // create scriptName
            var scriptName = "{0}{1}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName);

            // get the base script file from the config -- defaults to, well, default.aspx :)
            var scriptFile = Config.BaseScriptFile;

            if (url.IsNotSet())
            {
                return newUrl;
            }

            if (scriptName.EndsWith(scriptFile))
            {
                var before = scriptName.Remove(scriptName.LastIndexOf(scriptFile, StringComparison.Ordinal));

                var parser = new SimpleURLParameterParser(url);

                // create "rewritten" url...
                newUrl = before;

                var useKey = string.Empty;
                var description = string.Empty;
                var pageName = parser["g"];
                var forumPage = ForumPages.forum;
                var getDescription = false;

                var isFeed = false;
                var handlePage = false;

                if (pageName.IsSet())
                {
                    try
                    {
                        forumPage = pageName.ToEnum<ForumPages>();
                        getDescription = true;
                    }
                    catch (Exception)
                    {
                        getDescription = false;
                    }
                }

                if (getDescription)
                {
                    switch (forumPage)
                    {
                        case ForumPages.topics:
                            useKey = "f";
                            pageName += "/";

                            // description = UrlRewriteHelper.GetForumName(parser[useKey].ToType<int>());
                            description =
                                UrlRewriteHelper.CleanStringForURL(
                                    parser["name"].IsSet()
                                        ? parser["name"]
                                        : UrlRewriteHelper.GetForumName(parser[useKey].ToType<int>()));
                            handlePage = true;
                            break;
                        case ForumPages.posts:
                            pageName += "/";
                            if (parser["t"].IsSet())
                            {
                                useKey = "t";
                                pageName += "t";
                                description = UrlRewriteHelper.GetTopicName(parser[useKey].ToType<int>());
                            }
                            else if (parser["m"].IsSet())
                            {
                                useKey = "m";
                                pageName += "m";

                                try
                                {
                                    description = UrlRewriteHelper.GetTopicNameFromMessage(parser[useKey].ToType<int>());
                                }
                                catch (Exception)
                                {
                                    description = "posts";
                                }
                            }

                            handlePage = true;
                            break;
                        case ForumPages.profile:
                            useKey = "u";
                            pageName += "/";
                            description =
                                UrlRewriteHelper.CleanStringForURL(
                                    parser["name"].IsSet()
                                        ? parser["name"]
                                        : UrlRewriteHelper.GetProfileName(parser[useKey].ToType<int>()));

                            parser.Parameters.Remove("name");
                            break;
                        case ForumPages.forum:
                            pageName = "category/";
                            if (parser["c"].IsSet())
                            {
                                useKey = "c";
                                description = UrlRewriteHelper.GetCategoryName(parser[useKey].ToType<int>());
                            }
                            else
                            {
                                pageName = string.Empty;
                            }

                            break;
                        case ForumPages.rsstopic:
                            pageName += "/";

                            if (parser["pg"].IsSet())
                            {
                                description = parser["pg"].ToEnum<YafRssFeeds>().ToString().ToLower();
                            }

                            if (parser["f"].IsSet())
                            {
                                description +=
                                    "_{0}".FormatWith(UrlRewriteHelper.GetForumName(parser["f"].ToType<int>()));
                            }

                            if (parser["t"].IsSet())
                            {
                                description +=
                                    "_{0}".FormatWith(UrlRewriteHelper.GetTopicName(parser["t"].ToType<int>()));
                            }

                            if (parser["c"].IsSet())
                            {
                                description +=
                                    "_{0}".FormatWith(UrlRewriteHelper.GetCategoryName(parser["c"].ToType<int>()));
                            }

                            if (parser["ft"].IsSet())
                            {
                                description += parser["ft"].ToType<int>() == YafSyndicationFormats.Atom.ToInt()
                                                   ? "-atom"
                                                   : "-rss";
                            }

                            handlePage = true;
                            isFeed = true;
                            break;
                    }
                }

                // special handling for admin pages
                if (parser["g"].StartsWith("admin_"))
                {
                    pageName = parser["g"].Replace("_", "/");
                }

                newUrl += pageName;

                if (useKey.Length > 0)
                {
                    newUrl += parser[useKey];
                }

                // handle pager linkse
                if (handlePage && parser["p"] != null && !isFeed)
                {
                    var page = parser["p"].ToType<int>();

                    if (page != 1)
                    {
                        description += "/page{0}".FormatWith(page);
                    }

                    parser.Parameters.Remove("p");
                }

                if (isFeed)
                {
                    if (parser["ft"] != null)
                    {
                        var page = parser["ft"].ToType<int>();
                        newUrl += "ft{0}".FormatWith(page);
                        parser.Parameters.Remove("ft");
                    }

                    if (parser["f"] != null)
                    {
                        var page = parser["f"].ToType<int>();
                        newUrl += "f{0}".FormatWith(page);
                        parser.Parameters.Remove("f");
                    }

                    if (parser["t"] != null)
                    {
                        var page = parser["t"].ToType<int>();
                        newUrl += "t{0}".FormatWith(page);
                        parser.Parameters.Remove("t");
                    }
                }

                if (parser["find"] != null)
                {
                    newUrl += "find{0}".FormatWith(parser["find"].Trim());
                    parser.Parameters.Remove("find");
                }

                if (description.Length > 0)
                {
                    if (description.EndsWith("-"))
                    {
                        description = description.Remove(description.Length - 1, 1);
                    }

                    newUrl += "-{0}".FormatWith(description);
                }

                var restURL = parser.CreateQueryString(new[] { "g", useKey, "name" });

                // append to the url if there are additional (unsupported) parameters
                if (restURL.Length > 0)
                {
                    newUrl += "?{0}".FormatWith(restURL);
                }

                if (newUrl.EndsWith("/forum"))
                {
                    // remove in favor of just slash...
                    newUrl = newUrl.Remove(newUrl.LastIndexOf("/forum", StringComparison.Ordinal), "/forum".Length);
                }

                // add anchor
                if (parser.HasAnchor)
                {
                    newUrl += "#{0}".FormatWith(parser.Anchor);
                }
            }

            // just make sure & is &amp; ...
            newUrl = newUrl.Replace("&amp;", "&").Replace("&", "&amp;");

            return newUrl;
        }

        /// <summary>
        /// Builds path for calling page with URL argument as the parameter.
        /// </summary>
        /// <param name="boardSettings">
        /// The board settings.
        /// </param>
        /// <param name="url">
        /// URL to use as a parameter.
        /// </param>
        /// <returns>
        /// URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.
        /// </returns>
        public override string BuildUrl(object boardSettings, string url)
        {
            return this.BuildUrl(url);
        }

        #endregion
    }
}