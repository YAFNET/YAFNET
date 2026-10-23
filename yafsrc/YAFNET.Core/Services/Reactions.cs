/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Collections.Generic;
using System.Net;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
///  Reactions Class to handle the reactions (thanks) of the posts
/// </summary>
public class Reactions : IReactions, IHaveServiceLocator
{
    /// <summary>
    /// The emoji of the reactions.
    /// </summary>
    public static readonly IReadOnlyDictionary<ReactionType, string> Emojis = new Dictionary<ReactionType, string>
    {
        [ReactionType.Like] = "&#x1F44D;",
        [ReactionType.Love] = "&#x2764;&#xFE0F;",
        [ReactionType.Laugh] = "&#x1F602;",
        [ReactionType.Hooray] = "&#x1F389;",
        [ReactionType.Wow] = "&#x1F62E;"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="Reactions"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public Reactions(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Creates the reaction info of the message for the current user.
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// Returns the Reaction Info
    /// </returns>
    public ReactionInfo GetReactionInfo(int messageId)
    {
        var reactions = this.GetRepository<Thanks>().MessageGetThanksList(messageId);

        return new ReactionInfo
                   {
                       MessageID = messageId,
                       UserReaction = UserReaction(reactions),
                       ReactionsHtml = this.CreateHtml(messageId, reactions, true)
                   };
    }

    /// <summary>
    /// Creates the HTML of the reactions box of the message: the given reactions
    ///   with their counts, and the reaction picker if the current user can react.
    /// </summary>
    /// <param name="messageId">
    /// The Message Id.
    /// </param>
    /// <param name="canReact">
    /// If the current user can react to the message.
    /// </param>
    /// <returns>
    /// The reactions HTML.
    /// </returns>
    public string ReactionsHtml(int messageId, bool canReact)
    {
        return this.CreateHtml(messageId, this.GetRepository<Thanks>().MessageGetThanksList(messageId), canReact);
    }

    /// <summary>
    /// Gets the reaction of the current user.
    /// </summary>
    /// <param name="reactions">
    /// The reactions of the message.
    /// </param>
    /// <returns>
    /// The reaction, or 0 if the user has not reacted.
    /// </returns>
    private static int UserReaction(List<Tuple<Thanks, User>> reactions)
    {
        var userId = BoardContext.Current.PageUserID;

        return reactions.Find(r => r.Item1.ThanksFromUserID == userId)?.Item1.Reaction ?? 0;
    }

    /// <summary>
    /// Creates the HTML of the reactions box. The reactions are shown as buttons
    ///   (with the names of the users in the tooltip) and the picker
    ///   if the user can react, guests only see the counts.
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="reactions">
    /// The reactions of the message.
    /// </param>
    /// <param name="canReact">
    /// If the current user can react to the message.
    /// </param>
    /// <returns>
    /// The reactions HTML.
    /// </returns>
    private string CreateHtml(int messageId, List<Tuple<Thanks, User>> reactions, bool canReact)
    {
        var showNames = !BoardContext.Current.IsGuest;
        var userReaction = UserReaction(reactions);

        var html = new StringBuilder();

        html.Append(
            $"""<span id="dvReactions{messageId}" class="reactions d-inline-flex flex-wrap align-items-center gap-1">""");

        foreach (var group in reactions.Where(r => Enum.IsDefined(typeof(ReactionType), r.Item1.Reaction))
                     .GroupBy(r => (ReactionType)r.Item1.Reaction).OrderBy(g => g.Key))
        {
            // the tooltip is html (a line per user), the encoded names are encoded again for the attribute
            var title = new StringBuilder(WebUtility.HtmlEncode(this.ReactionName(group.Key)));

            if (showNames)
            {
                group.ForEach(r => title.Append("<br>").Append(this.UserTitle(r.Item1, r.Item2)));
            }

            var tooltip =
                $"data-bs-toggle=\"tooltip\" data-bs-animation=\"false\" data-bs-html=\"true\" title=\"{WebUtility.HtmlEncode(title.ToString())}\"";

            var content = $"""{Emojis[group.Key]}<span class="ms-1">{group.Count()}</span>""";

            if (canReact)
            {
                var css = group.Key == (ReactionType)userReaction ? "btn-primary" : "btn-outline-secondary";

                html.Append(
                    $"""<button type="button" class="btn btn-sm rounded-pill py-0 {css}" {tooltip} onclick="setReaction({messageId}, {(int)group.Key});">{content}</button>""");
            }
            else
            {
                html.Append(
                    $"""<span class="badge rounded-pill text-bg-light border fw-normal" {tooltip}>{content}</span>""");
            }
        }

        if (canReact)
        {
            // a dropdown toggle can't have a tooltip too, the tooltip is on the content of the button
            var reactTitle = WebUtility.HtmlEncode(this.Get<ILocalization>().GetText("BUTTON", "BUTTON_REACT_TT"));

            html.Append(
                $"""<span class="dropup"><button type="button" class="btn btn-link p-0 reaction-picker" data-bs-toggle="dropdown" aria-expanded="false" aria-label="{reactTitle}"><span class="d-inline-block px-2 py-1" data-bs-toggle="tooltip" data-bs-animation="false" data-bs-trigger="hover" title="{reactTitle}"><i class="far fa-face-smile"></i></span></button><span class="dropdown-menu reaction-picker-menu"><span class="reaction-picker-bar">""");

            Emojis.ForEach(
                emoji =>
                    {
                        var css = emoji.Key == (ReactionType)userReaction ? " active" : string.Empty;

                        html.Append(
                            $"""<button type="button" class="dropdown-item reaction-emoji{css}" data-bs-toggle="tooltip" data-bs-animation="false" title="{WebUtility.HtmlEncode(this.ReactionName(emoji.Key))}" onclick="setReaction({messageId}, {(int)emoji.Key});">{emoji.Value}</button>""");
                    });

            html.Append("</span></span></span>");
        }

        html.Append("</span>");

        return html.ToString();
    }

    /// <summary>
    /// Gets the localized name of the reaction.
    /// </summary>
    /// <param name="reaction">
    /// The reaction.
    /// </param>
    /// <returns>
    /// The reaction name.
    /// </returns>
    private string ReactionName(ReactionType reaction)
    {
        return this.Get<ILocalization>().GetText("BUTTON", $"REACTION_{reaction.ToString().ToUpperInvariant()}");
    }

    /// <summary>
    /// Gets the encoded name of the user who reacted, with the date if enabled.
    /// </summary>
    /// <param name="thanks">
    /// The reaction.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The user title.
    /// </returns>
    private string UserTitle(Thanks thanks, User user)
    {
        var name = new UnicodeEncoder().XSSEncode(user.DisplayOrUserName());

        return this.Get<BoardSettings>().ShowThanksDate
                   ? $"{name} {this.Get<ILocalization>().GetTextFormatted(
                       "ONDATE",
                       this.Get<IDateTimeService>().FormatDateShort(thanks.ThanksDate))}"
                   : name;
    }
}
