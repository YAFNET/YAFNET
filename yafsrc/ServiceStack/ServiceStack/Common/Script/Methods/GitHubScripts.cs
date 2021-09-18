// ***********************************************************************
// <copyright file="GitHubScripts.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.IO;

namespace ServiceStack.Script
{
    /// <summary>
    /// Class GitHubPlugin.
    /// Implements the <see cref="ServiceStack.Script.IScriptPlugin" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.IScriptPlugin" />
    public class GitHubPlugin : IScriptPlugin
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(ScriptContext context)
        {
            context.ScriptMethods.Add(new GitHubScripts());
        }
    }

    /// <summary>
    /// Class GitHubScripts.
    /// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptMethods" />
    public class GitHubScripts : ScriptMethods
    {
        /// <summary>
        /// Gists the virtual files.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns>GistVirtualFiles.</returns>
        public GistVirtualFiles gistVirtualFiles(string gistId) => new GistVirtualFiles(gistId);

        /// <summary>
        /// Gists the virtual files.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>GistVirtualFiles.</returns>
        public GistVirtualFiles gistVirtualFiles(string gistId, string accessToken) =>
            new GistVirtualFiles(gistId, accessToken);

        /// <summary>
        /// Githubs the gateway.
        /// </summary>
        /// <returns>GitHubGateway.</returns>
        public GitHubGateway githubGateway() => new GitHubGateway();
        /// <summary>
        /// Githubs the gateway.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>GitHubGateway.</returns>
        public GitHubGateway githubGateway(string accessToken) => new GitHubGateway(accessToken);

        /// <summary>
        /// Githubs the source zip URL.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="orgNames">The org names.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string githubSourceZipUrl(GitHubGateway gateway, string orgNames, string name) =>
            gateway.GetSourceZipUrl(orgNames, name);

        /// <summary>
        /// Githubs the source repos.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="orgName">Name of the org.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> githubSourceRepos(GitHubGateway gateway, string orgName) =>
            Task.FromResult<object>(gateway.GetSourceReposAsync(orgName));

        /// <summary>
        /// Githubs the user and org repos.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="githubOrgOrUser">The github org or user.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> githubUserAndOrgRepos(GitHubGateway gateway, string githubOrgOrUser) =>
            Task.FromResult<object>(gateway.GetUserAndOrgReposAsync(githubOrgOrUser));

        /// <summary>
        /// Githubs the user repos.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="githubUser">The github user.</param>
        /// <returns>List&lt;GithubRepo&gt;.</returns>
        public List<GithubRepo> githubUserRepos(GitHubGateway gateway, string githubUser) =>
            gateway.GetUserRepos(githubUser);

        /// <summary>
        /// Githubs the org repos.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="githubOrg">The github org.</param>
        /// <returns>List&lt;GithubRepo&gt;.</returns>
        public List<GithubRepo> githubOrgRepos(GitHubGateway gateway, string githubOrg) =>
            gateway.GetOrgRepos(githubOrg);

        /// <summary>
        /// Githubs the create gist.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="description">The description.</param>
        /// <param name="files">The files.</param>
        /// <returns>GithubGist.</returns>
        public GithubGist githubCreateGist(GitHubGateway gateway, string description, Dictionary<string, string> files) =>
            gateway.CreateGithubGist(description: description, isPublic: true, textFiles: files);

        /// <summary>
        /// Githubs the create private gist.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="description">The description.</param>
        /// <param name="files">The files.</param>
        /// <returns>GithubGist.</returns>
        public GithubGist githubCreatePrivateGist(GitHubGateway gateway, string description, Dictionary<string, string> files) =>
            gateway.CreateGithubGist(description: description, isPublic: false, textFiles: files);

        /// <summary>
        /// Githubs the gist.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns>GithubGist.</returns>
        public GithubGist githubGist(GitHubGateway gateway, string gistId) =>
            gateway.GetGithubGist(gistId);

        /// <summary>
        /// Githubs the write gist files.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="gistFiles">The gist files.</param>
        /// <returns>IgnoreResult.</returns>
        public IgnoreResult githubWriteGistFiles(GitHubGateway gateway, string gistId, Dictionary<string, string> gistFiles)
        {
            gateway.WriteGistFiles(gistId, gistFiles);
            return IgnoreResult.Value;
        }

        /// <summary>
        /// Githubs the write gist file.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns>IgnoreResult.</returns>
        public IgnoreResult githubWriteGistFile(GitHubGateway gateway, string gistId, string filePath, string contents)
        {
            gateway.WriteGistFile(gistId, filePath, contents);
            return IgnoreResult.Value;
        }

        /// <summary>
        /// Githus the delete gist files.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>IgnoreResult.</returns>
        public IgnoreResult githuDeleteGistFiles(GitHubGateway gateway, string gistId, string filePath)
        {
            gateway.DeleteGistFiles(gistId, filePath);
            return IgnoreResult.Value;
        }

        /// <summary>
        /// Githus the delete gist files.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePaths">The file paths.</param>
        /// <returns>IgnoreResult.</returns>
        public IgnoreResult githuDeleteGistFiles(GitHubGateway gateway, string gistId, IEnumerable<string> filePaths)
        {
            gateway.DeleteGistFiles(gistId, filePaths.ToArray());
            return IgnoreResult.Value;
        }
    }
}