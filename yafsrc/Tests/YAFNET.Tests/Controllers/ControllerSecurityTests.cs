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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Controllers;

using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using YAF.Core.BasePages;
using YAF.Core.Controllers.Modals;
using YAF.Core.Filters;
using YAF.Types.Constants;

/// <summary>
/// The attribute based protection of all controller actions (the MVC pipeline applies these, so they
/// are checked by reflection). A new action must be added to the lists below deliberately.
/// </summary>
[TestFixture]
public class ControllerSecurityTests
{
    /// <summary>
    /// The actions that may be called with GET: they only read data and change nothing
    /// (download counters aside). Everything else must be POST, so it can't be triggered by a link or an image.
    /// </summary>
    private readonly static HashSet<string> ReadOnlyGetActions =
    [
        "Albums.GetImage", "Albums.GetImagePreview", "Albums.GetAlbumCover",
        "Attachments.GetAttachment", "Attachments.GetResponseAttachment",
        "AvatarController.GetTextAvatar", "AvatarController.GetResponseLocalAvatar",
        "CustomBBCodes.GetList",
        "Feed.GetLatestPosts", "Feed.GetTopicsFeed", "Feed.GetPostsFeed",
        "LoginBox.ShowModal",
        "Stats.GetUserStats",
        "UserController.GetMentionUsers",
        "UserInfo.GetUserInfo",
        "PwaController.ServiceWorkerAsync", "PwaController.OfflineAsync", "PwaController.WebManifest"
    ];

    /// <summary>
    /// POST actions without an antiforgery token. Keep this list empty if possible.
    /// </summary>
    private readonly static HashSet<string> PostWithoutAntiforgery = [];

    private static IEnumerable<(Type Controller, MethodInfo Action)> Actions()
    {
        return typeof(ForumBaseController).Assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
            .SelectMany(
                t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName && m.GetCustomAttribute<NonActionAttribute>() is null)
                    .Select(m => (t, m)));
    }

    private static string Name((Type Controller, MethodInfo Action) action)
    {
        return $"{action.Controller.Name}.{action.Action.Name}";
    }

    private static List<string> Verbs(MethodInfo action)
    {
        return [.. action.GetCustomAttributes<HttpMethodAttribute>().SelectMany(a => a.HttpMethods)];
    }

    [Test]
    [Description("The controllers are found (guards the reflection against silently checking nothing)")]
    public void Controllers_Found_Test()
    {
        Actions().Should().HaveCountGreaterThan(50);
        Actions().Select(Name).Should().Contain(["PostsModalController.Reply", "PollController.Vote", "Attachments.GetAttachment"]);
    }

    [Test]
    [Description("Every action declares its HTTP method; GET only for the read-only actions")]
    public void Http_Methods_Test()
    {
        foreach (var action in Actions())
        {
            var name = Name(action);
            var verbs = Verbs(action.Action);

            verbs.Should().NotBeEmpty($"{name} must declare its HTTP method (without it, GET requests work too)");

            if (verbs.Contains("GET"))
            {
                ReadOnlyGetActions.Should().Contain(name, $"{name} accepts GET, only read-only actions may");
            }
        }

        ReadOnlyGetActions.Should().BeSubsetOf(Actions().Select(Name), "the list should not contain removed actions");
    }

    [Test]
    [Description("POST actions validate the antiforgery token")]
    public void Antiforgery_Test()
    {
        foreach (var action in Actions().Where(a => Verbs(a.Action).Contains("POST")))
        {
            var name = Name(action);
            var validates = action.Action.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null
                            || action.Controller.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() is not null;

            if (PostWithoutAntiforgery.Contains(name))
            {
                validates.Should().BeFalse($"{name} validates the token now, remove it from the exception list");
            }
            else
            {
                validates.Should().BeTrue($"{name} is a POST without [ValidateAntiForgeryToken]");
            }
        }
    }

    [Test]
    [Description("The admin dialogs require an admin, moving topics requires a moderator")]
    public void Admin_And_Moderator_Test()
    {
        var adminDialogs = typeof(PostsModalController).Assembly.GetTypes()
            .Where(t => t.Namespace == typeof(PostsModalController).Namespace && typeof(ControllerBase).IsAssignableFrom(t)
                        && t != typeof(PostsModalController))
            .ToList();

        adminDialogs.Should().NotBeEmpty();
        adminDialogs.Should().OnlyContain(
            t => t.GetCustomAttribute<AdminAuthorizationAttribute>() != null,
            "the dialogs in Controllers/Modals are admin dialogs");

        var moveTopic = typeof(PostsModalController).GetMethod(nameof(PostsModalController.MoveTopic));
        moveTopic.GetCustomAttribute<AuthorizationAttribute>().Should().NotBeNull();

        typeof(YAF.Core.Controllers.AccessMaskController).GetCustomAttribute<AdminAuthorizationAttribute>().Should().NotBeNull();
        typeof(YAF.Core.Controllers.Stats).GetCustomAttribute<AdminAuthorizationAttribute>().Should().NotBeNull();
    }

    [TestCase(typeof(YAF.Core.Controllers.FriendsController))]
    [TestCase(typeof(YAF.Core.Controllers.IgnoredUserController))]
    [TestCase(typeof(YAF.Core.Controllers.PollController))]
    [TestCase(typeof(YAF.Core.Controllers.PostsController))]
    [TestCase(typeof(YAF.Core.Controllers.ReputationController))]
    [Description("Controllers for logged-in users require authentication")]
    public void Authorize_Test(Type controller)
    {
        controller.GetCustomAttribute<AuthorizeAttribute>().Should().NotBeNull();
    }

    [Test]
    [Description("The authorization filters forbid users without the right")]
    public void Authorization_Filters_Test()
    {
        new AuthorizationAttribute(AuthorizationAccess.ModeratorAccess).Should().BeAssignableTo<AuthorizeAttribute>(
            "unauthenticated requests are rejected by the authorization policy");
        new AdminAuthorizationAttribute().Should().BeAssignableTo<AuthorizeAttribute>();
    }
}
