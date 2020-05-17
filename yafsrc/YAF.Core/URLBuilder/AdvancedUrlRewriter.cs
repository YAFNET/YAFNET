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
namespace YAF.Core.URLBuilder
{
    #region Using

    using System;

    using YAF.Configuration;
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
            var newUrl = $"{AppPath}{Config.ForceScriptName ?? ScriptName}?{url}";

            // create scriptName
            var scriptName = $"{AppPath}{Config.ForceScriptName ?? ScriptName}";

            // get the base script file from the config -- defaults to, well, default.aspx :)
            var scriptFile = Config.BaseScriptFile;

            if (url.IsNotSet())
            {
                return newUrl;
            }

            const string gsr = "getsearchresults";
            scriptName = scriptName.Replace(gsr, scriptFile);
            newUrl = newUrl.Replace(gsr, scriptFile);

            if (scriptName.EndsWith(scriptFile))
            {
                var before = scriptName.Remove(scriptName.LastIndexOf(scriptFile, StringComparison.Ordinal));

                var parser = new SimpleURLParameterParser(url);

                // create "rewritten" url...
                newUrl = before;

                var useKey = string.Empty;
                var description = string.Empty;
                var pageName = parser["g"];
                var forumPage = ForumPages.Board;
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
                        case ForumPages.Topics:
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
                        case ForumPages.Posts:
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
                        case ForumPages.UserProfile:
                            useKey = "u";
                            pageName += "/";
                            description =
                                UrlRewriteHelper.CleanStringForURL(
                                    parser["name"].IsSet()
                                        ? parser["name"]
                                        : UrlRewriteHelper.GetProfileName(parser[useKey].ToType<int>()));

                            parser.Parameters.Remove("name");
                            break;
                        case ForumPages.Board:
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
                        case ForumPages.RssTopic:
                            pageName += "/";

                            if (parser["feed"].IsSet())
                            {
                                description = parser["feed"].ToEnum<RssFeeds>().ToString().ToLower();
                            }

                            if (parser["f"].IsSet())
                            {
                                description += $"_{UrlRewriteHelper.GetForumName(parser["f"].ToType<int>())}";
                            }

                            if (parser["t"].IsSet())
                            {
                                description += $"_{UrlRewriteHelper.GetTopicName(parser["t"].ToType<int>())}";
                            }

                            if (parser["c"].IsSet())
                            {
                                description += $"_{UrlRewriteHelper.GetCategoryName(parser["c"].ToType<int>())}";
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

                // handle pager links
                if (handlePage && parser["p"] != null && !isFeed)
                {
                    var page = parser["p"].ToType<int>();

                    if (page != 1)
                    {
                        description += $"/page{page}";
                    }

                    parser.Parameters.Remove("p");
                }

                if (isFeed)
                {
                    if (parser["type"] != null)
                    {
                        var page = parser["type"].ToType<int>();
                        newUrl += $"type{page}";
                        parser.Parameters.Remove("type");
                    }

                    if (parser["feed"] != null)
                    {
                        var page = parser["feed"].ToType<int>();
                        newUrl += $"feed{page}";
                        parser.Parameters.Remove("feed");
                    }

                    if (parser["f"] != null)
                    {
                        var page = parser["f"].ToType<int>();
                        newUrl += $"f{page}";
                        parser.Parameters.Remove("f");
                    }

                    if (parser["t"] != null)
                    {
                        var page = parser["t"].ToType<int>();
                        newUrl += $"t{page}";
                        parser.Parameters.Remove("t");
                    }
                }

                if (parser["find"] != null)
                {
                    newUrl += $"find{parser["find"].Trim()}";
                    parser.Parameters.Remove("find");
                }

                if (description.Length > 0)
                {
                    if (description.EndsWith("-"))
                    {
                        description = description.Remove(description.Length - 1, 1);
                    }

                    newUrl += $"-{description}";
                }

                var restUrl = parser.CreateQueryString(new[] { "g", useKey, "name" });

                // append to the url if there are additional (unsupported) parameters
                if (restUrl.Length > 0)
                {
                    newUrl += $"?{restUrl}";
                }

                if (newUrl.EndsWith("/forum"))
                {
                    // remove in favor of just slash...
                    newUrl = newUrl.Remove(newUrl.LastIndexOf("/forum", StringComparison.Ordinal), "/forum".Length);
                }

                // add anchor
                if (parser.HasAnchor)
                {
                    newUrl += $"#{parser.Anchor}";
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