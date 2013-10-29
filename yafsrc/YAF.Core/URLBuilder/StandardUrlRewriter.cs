/* Yet Another Forum.net
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
    /// The standard url rewriter.
    /// </summary>
    public class StandardUrlRewriter : BaseUrlBuilder
    {
       #region Public Methods

        /// <summary>
        /// Build the url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>
        /// Returns the URL.
        /// </returns>
        public override string BuildUrl(string url)
        {
            string newUrl = "{0}{1}?{2}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName, url);

            // create scriptName
            string scriptName = "{0}{1}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName);

            // get the base script file from the config -- defaults to, well, default.aspx :)
            string scriptFile = Config.BaseScriptFile;

            if (scriptName.EndsWith(scriptFile))
            {
                var before = scriptName.Remove(scriptName.LastIndexOf(scriptFile, StringComparison.Ordinal));

                var parser = new SimpleURLParameterParser(url);

                // create "rewritten" url...
                newUrl = "{0}{1}".FormatWith(before, Config.UrlRewritingPrefix);

                var useKey = string.Empty;
                var description = string.Empty;
                var pageName = parser["g"];
                var isFeed = false;
                var handlePage = false;

                switch (parser["g"])
                {
                    case "topics":
                        useKey = "f";
                        description = UrlRewriteHelper.GetForumName(parser[useKey].ToType<int>());
                        handlePage = true;
                        break;
                    case "posts":
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
                    case "profile":
                        useKey = "u";

                        description = UrlRewriteHelper.GetProfileName(parser[useKey].ToType<int>());
                        break;
                    case "forum":
                        if (parser["c"].IsSet())
                        {
                            useKey = "c";
                            description = UrlRewriteHelper.GetCategoryName(parser[useKey].ToType<int>());
                        }

                        break;
                    case "rsstopic":
                        if (parser["pg"].IsSet())
                        {
                            description = parser["pg"].ToEnum<YafRssFeeds>().ToString().ToLower();
                        }

                        if (parser["f"].IsSet())
                        {
                            description += "_{0}".FormatWith(UrlRewriteHelper.GetForumName(parser["f"].ToType<int>()));
                        }

                        if (parser["t"].IsSet())
                        {
                            description += "_{0}".FormatWith(UrlRewriteHelper.GetTopicName(parser["t"].ToType<int>()));
                        }

                        if (parser["c"].IsSet())
                        {
                            description += "_{0}".FormatWith(UrlRewriteHelper.GetCategoryName(parser["c"].ToType<int>()));
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

                newUrl += pageName;

                if (useKey.Length > 0)
                {
                    newUrl += parser[useKey];
                }

                if (handlePage && parser["p"] != null && !isFeed)
                {
                    int page = parser["p"].ToType<int>();
                    if (page != 1)
                    {
                        newUrl += "p{0}".FormatWith(page);
                    }

                    parser.Parameters.Remove("p");
                }

                if (isFeed)
                {
                    if (parser["ft"] != null)
                    {
                        int page = parser["ft"].ToType<int>();
                        newUrl += "ft{0}".FormatWith(page);
                        parser.Parameters.Remove("ft");
                    }

                    if (parser["f"] != null)
                    {
                        int page = parser["f"].ToType<int>();
                        newUrl += "f{0}".FormatWith(page);
                        parser.Parameters.Remove("f");
                    }

                    if (parser["t"] != null)
                    {
                        int page = parser["t"].ToType<int>();
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

                    newUrl += "_{0}".FormatWith(description);
                }

                newUrl += ".aspx";

                var restURL = parser.CreateQueryString(new[] { "g", useKey });

                // append to the url if there are additional (unsupported) parameters
                if (restURL.Length > 0)
                {
                    newUrl += "?{0}".FormatWith(restURL);
                }

                // see if we can just use the default (/)
                if (newUrl.EndsWith("{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix)))
                {
                    // remove in favor of just slash...
                    newUrl =
                        newUrl.Remove(
                            newUrl.LastIndexOf(
                                "{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix), StringComparison.Ordinal),
                            "{0}forum.aspx".FormatWith(Config.UrlRewritingPrefix).Length);
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

        #endregion
    }
}