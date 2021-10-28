// ***********************************************************************
// <copyright file="GitHubGateway.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using ServiceStack;
    using ServiceStack.IO;
    using ServiceStack.Script;
    using ServiceStack.Text;

    /// <summary>
    /// Interface IGistGateway
    /// </summary>
    public interface IGistGateway
    {
        /// <summary>
        /// Creates the gist.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="files">The files.</param>
        /// <returns>Gist.</returns>
        Gist CreateGist(string description, bool isPublic, Dictionary<string, object> files);
        /// <summary>
        /// Creates the gist.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="textFiles">The text files.</param>
        /// <returns>Gist.</returns>
        Gist CreateGist(string description, bool isPublic, Dictionary<string, string> textFiles);
        /// <summary>
        /// Gets the gist.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns>Gist.</returns>
        Gist GetGist(string gistId);
        /// <summary>
        /// Gets the gist.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="version">The version.</param>
        /// <returns>Gist.</returns>
        Gist GetGist(string gistId, string version);
        /// <summary>
        /// Gets the gist asynchronous.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns>Task&lt;Gist&gt;.</returns>
        Task<Gist> GetGistAsync(string gistId);
        /// <summary>
        /// Gets the gist asynchronous.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;Gist&gt;.</returns>
        Task<Gist> GetGistAsync(string gistId, string version);
        /// <summary>
        /// Writes the gist files.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="files">The files.</param>
        /// <param name="description">The description.</param>
        /// <param name="deleteMissing">if set to <c>true</c> [delete missing].</param>
        void WriteGistFiles(string gistId, Dictionary<string, object> files, string description = null, bool deleteMissing = false);
        /// <summary>
        /// Writes the gist files.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="textFiles">The text files.</param>
        /// <param name="description">The description.</param>
        /// <param name="deleteMissing">if set to <c>true</c> [delete missing].</param>
        void WriteGistFiles(string gistId, Dictionary<string, string> textFiles, string description = null, bool deleteMissing = false);
        /// <summary>
        /// Creates the gist file.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        void CreateGistFile(string gistId, string filePath, string contents);
        /// <summary>
        /// Writes the gist file.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        void WriteGistFile(string gistId, string filePath, string contents);
        /// <summary>
        /// Deletes the gist files.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePaths">The file paths.</param>
        void DeleteGistFiles(string gistId, params string[] filePaths);
    }

    /// <summary>
    /// Interface IGitHubGateway
    /// Implements the <see cref="ServiceStack.IGistGateway" />
    /// </summary>
    /// <seealso cref="ServiceStack.IGistGateway" />
    public interface IGitHubGateway : IGistGateway
    {
        /// <summary>
        /// Finds the repo.
        /// </summary>
        /// <param name="orgs">The orgs.</param>
        /// <param name="name">The name.</param>
        /// <param name="useFork">if set to <c>true</c> [use fork].</param>
        /// <returns>Tuple&lt;System.String, System.String&gt;.</returns>
        Tuple<string, string> FindRepo(string[] orgs, string name, bool useFork = false);
        /// <summary>
        /// Gets the source zip URL.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>System.String.</returns>
        string GetSourceZipUrl(string user, string repo);
        /// <summary>
        /// Gets the source zip URL asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> GetSourceZipUrlAsync(string user, string repo);
        /// <summary>
        /// Gets the source repos asynchronous.
        /// </summary>
        /// <param name="orgName">Name of the org.</param>
        /// <returns>Task&lt;List&lt;GithubRepo&gt;&gt;.</returns>
        Task<List<GithubRepo>> GetSourceReposAsync(string orgName);
        /// <summary>
        /// Gets the user and org repos asynchronous.
        /// </summary>
        /// <param name="githubOrgOrUser">The github org or user.</param>
        /// <returns>Task&lt;List&lt;GithubRepo&gt;&gt;.</returns>
        Task<List<GithubRepo>> GetUserAndOrgReposAsync(string githubOrgOrUser);
        /// <summary>
        /// Gets the repo.
        /// </summary>
        /// <param name="userOrOrg">The user or org.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>GithubRepo.</returns>
        GithubRepo GetRepo(string userOrOrg, string repo);
        /// <summary>
        /// Gets the repo asynchronous.
        /// </summary>
        /// <param name="userOrOrg">The user or org.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>Task&lt;GithubRepo&gt;.</returns>
        Task<GithubRepo> GetRepoAsync(string userOrOrg, string repo);
        /// <summary>
        /// Gets the user repos.
        /// </summary>
        /// <param name="githubUser">The github user.</param>
        /// <returns>List&lt;GithubRepo&gt;.</returns>
        List<GithubRepo> GetUserRepos(string githubUser);
        /// <summary>
        /// Gets the user repos asynchronous.
        /// </summary>
        /// <param name="githubUser">The github user.</param>
        /// <returns>Task&lt;List&lt;GithubRepo&gt;&gt;.</returns>
        Task<List<GithubRepo>> GetUserReposAsync(string githubUser);
        /// <summary>
        /// Gets the org repos.
        /// </summary>
        /// <param name="githubOrg">The github org.</param>
        /// <returns>List&lt;GithubRepo&gt;.</returns>
        List<GithubRepo> GetOrgRepos(string githubOrg);
        /// <summary>
        /// Gets the org repos asynchronous.
        /// </summary>
        /// <param name="githubOrg">The github org.</param>
        /// <returns>Task&lt;List&lt;GithubRepo&gt;&gt;.</returns>
        Task<List<GithubRepo>> GetOrgReposAsync(string githubOrg);
        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>System.String.</returns>
        string GetJson(string route);
        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>T.</returns>
        T GetJson<T>(string route);
        /// <summary>
        /// Gets the json asynchronous.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> GetJsonAsync(string route);
        /// <summary>
        /// Gets the json asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> GetJsonAsync<T>(string route);
        /// <summary>
        /// Streams the json collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        IEnumerable<T> StreamJsonCollection<T>(string route);
        /// <summary>
        /// Gets the json collection asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        Task<List<T>> GetJsonCollectionAsync<T>(string route);
        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="downloadUrl">The download URL.</param>
        /// <param name="fileName">Name of the file.</param>
        void DownloadFile(string downloadUrl, string fileName);
    }

    /// <summary>
    /// Class GithubRateLimit.
    /// </summary>
    public class GithubRateLimit
    {
        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        /// <value>The limit.</value>
        public int Limit { get; set; }
        /// <summary>
        /// Gets or sets the remaining.
        /// </summary>
        /// <value>The remaining.</value>
        public int Remaining { get; set; }
        /// <summary>
        /// Gets or sets the reset.
        /// </summary>
        /// <value>The reset.</value>
        public long Reset { get; set; }
        /// <summary>
        /// Gets or sets the used.
        /// </summary>
        /// <value>The used.</value>
        public int Used { get; set; }
    }
    /// <summary>
    /// Class GithubResourcesRateLimits.
    /// </summary>
    public class GithubResourcesRateLimits
    {
        /// <summary>
        /// Gets or sets the core.
        /// </summary>
        /// <value>The core.</value>
        public GithubRateLimit Core { get; set; }
        /// <summary>
        /// Gets or sets the graphql.
        /// </summary>
        /// <value>The graphql.</value>
        public GithubRateLimit Graphql { get; set; }
        /// <summary>
        /// Gets or sets the integration manifest.
        /// </summary>
        /// <value>The integration manifest.</value>
        public GithubRateLimit Integration_Manifest { get; set; }
        /// <summary>
        /// Gets or sets the search.
        /// </summary>
        /// <value>The search.</value>
        public GithubRateLimit Search { get; set; }
    }
    /// <summary>
    /// Class GithubRateLimits.
    /// </summary>
    public class GithubRateLimits
    {
        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public GithubResourcesRateLimits Resources { get; set; }
    }

    /// <summary>
    /// Class GitHubGateway.
    /// Implements the <see cref="ServiceStack.IGistGateway" />
    /// Implements the <see cref="ServiceStack.IGitHubGateway" />
    /// </summary>
    /// <seealso cref="ServiceStack.IGistGateway" />
    /// <seealso cref="ServiceStack.IGitHubGateway" />
    public class GitHubGateway : IGistGateway, IGitHubGateway
    {
        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; set; } = nameof(GitHubGateway);
        /// <summary>
        /// The API base URL
        /// </summary>
        public const string ApiBaseUrl = "https://api.github.com/";
        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        /// <value>The base URL.</value>
        public string BaseUrl { get; set; } = ApiBaseUrl;

        /// <summary>
        /// AccessTokenSecret
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Intercept and override GitHub JSON API requests
        /// </summary>
        /// <value>The get json filter.</value>
        public Func<string, string> GetJsonFilter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubGateway"/> class.
        /// </summary>
        public GitHubGateway() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubGateway"/> class.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        public GitHubGateway(string accessToken) => AccessToken = accessToken;

        /// <summary>
        /// Get rate limits as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;GithubRateLimits&gt; representing the asynchronous operation.</returns>
        public async Task<GithubRateLimits> GetRateLimitsAsync()
            => await GetJsonAsync<GithubRateLimits>("/rate_limit").ConfigAwait();

        /// <summary>
        /// Gets the rate limits.
        /// </summary>
        /// <returns>GithubRateLimits.</returns>
        public GithubRateLimits GetRateLimits()
            => GetJson<GithubRateLimits>("/rate_limit");

        /// <summary>
        /// Unwraps the full name of the repo.
        /// </summary>
        /// <param name="orgName">Name of the org.</param>
        /// <param name="name">The name.</param>
        /// <param name="useFork">if set to <c>true</c> [use fork].</param>
        /// <returns>System.String.</returns>
        internal string UnwrapRepoFullName(string orgName, string name, bool useFork = false)
        {
            try
            {
                var repo = GetJson<GithubRepo>($"/repos/{orgName}/{name}");
                if (useFork && repo.Fork)
                {
                    if (repo.Parent != null)
                        return repo.Parent.Full_Name;
                }

                return repo.Full_Name;
            }
            catch (WebException ex)
            {
                if (ex.IsNotFound())
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Asserts the repo.
        /// </summary>
        /// <param name="orgs">The orgs.</param>
        /// <param name="name">The name.</param>
        /// <param name="useFork">if set to <c>true</c> [use fork].</param>
        /// <returns>Tuple&lt;System.String, System.String&gt;.</returns>
        /// <exception cref="System.Exception">'{name}' was not found in sources: {orgs.Join(", ")}</exception>
        public virtual Tuple<string, string> AssertRepo(string[] orgs, string name, bool useFork = false) =>
            FindRepo(orgs, name, useFork)
            ?? throw new Exception($"'{name}' was not found in sources: {orgs.Join(", ")}");

        /// <summary>
        /// Finds the repo.
        /// </summary>
        /// <param name="orgs">The orgs.</param>
        /// <param name="name">The name.</param>
        /// <param name="useFork">if set to <c>true</c> [use fork].</param>
        /// <returns>Tuple&lt;System.String, System.String&gt;.</returns>
        public virtual Tuple<string, string> FindRepo(string[] orgs, string name, bool useFork = false)
        {
            foreach (var orgName in orgs)
            {
                var repoFullName = UnwrapRepoFullName(orgName, name, useFork);
                if (repoFullName == null)
                    continue;

                var user = repoFullName.LeftPart('/');
                var repo = repoFullName.RightPart('/');
                return Tuple.Create(user, repo);
            }
            return null;
        }

        /// <summary>
        /// Gets the source zip URL.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>System.String.</returns>
        public virtual string GetSourceZipUrl(string user, string repo) =>
            GetSourceZipUrl(user, repo, GetJson($"repos/{user}/{repo}/releases"));

        /// <summary>
        /// Get source zip URL as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public virtual async Task<string> GetSourceZipUrlAsync(string user, string repo) =>
            GetSourceZipUrl(user, repo, await GetJsonAsync($"repos/{user}/{repo}/releases").ConfigAwait());

        /// <summary>
        /// Gets the source zip URL.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="repo">The repo.</param>
        /// <param name="json">The json.</param>
        /// <returns>System.String.</returns>
        private static string GetSourceZipUrl(string user, string repo, string json)
        {
            var response = JSON.parse(json);

            if (response is List<object> releases && releases.Count > 0 &&
                releases[0] is Dictionary<string, object> release &&
                release.TryGetValue("zipball_url", out var zipUrl))
            {
                return (string)zipUrl;
            }

            return $"https://github.com/{user}/{repo}/archive/master.zip";
        }

        /// <summary>
        /// Get source repos as an asynchronous operation.
        /// </summary>
        /// <param name="orgName">Name of the org.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        public virtual async Task<List<GithubRepo>> GetSourceReposAsync(string orgName)
        {
            var repos = (await GetUserAndOrgReposAsync(orgName).ConfigAwait())
                .OrderBy(x => x.Name)
                .ToList();
            return repos;
        }

        /// <summary>
        /// Get user and org repos as an asynchronous operation.
        /// </summary>
        /// <param name="githubOrgOrUser">The github org or user.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        public virtual async Task<List<GithubRepo>> GetUserAndOrgReposAsync(string githubOrgOrUser)
        {
            var map = new Dictionary<string, GithubRepo>();

            var userRepos = GetJsonCollectionAsync<List<GithubRepo>>($"users/{githubOrgOrUser}/repos");
            var orgRepos = GetJsonCollectionAsync<List<GithubRepo>>($"orgs/{githubOrgOrUser}/repos");

            try
            {
                foreach (var repos in await userRepos.ConfigAwait())
                    foreach (var repo in repos)
                        map[repo.Name] = repo;
            }
            catch (Exception e)
            {
                if (!e.IsNotFound()) throw;
            }

            try
            {
                foreach (var repos in await userRepos.ConfigAwait())
                    foreach (var repo in repos)
                        map[repo.Name] = repo;
            }
            catch (Exception e)
            {
                if (!e.IsNotFound()) throw;
            }

            return map.Values.ToList();
        }

        /// <summary>
        /// Gets the repo.
        /// </summary>
        /// <param name="userOrOrg">The user or org.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>GithubRepo.</returns>
        public virtual GithubRepo GetRepo(string userOrOrg, string repo) =>
            GetJson<GithubRepo>($"/{userOrOrg}/{repo}");

        /// <summary>
        /// Gets the repo asynchronous.
        /// </summary>
        /// <param name="userOrOrg">The user or org.</param>
        /// <param name="repo">The repo.</param>
        /// <returns>Task&lt;GithubRepo&gt;.</returns>
        public virtual Task<GithubRepo> GetRepoAsync(string userOrOrg, string repo) =>
            GetJsonAsync<GithubRepo>($"/{userOrOrg}/{repo}");

        /// <summary>
        /// Gets the user repos.
        /// </summary>
        /// <param name="githubUser">The github user.</param>
        /// <returns>List&lt;GithubRepo&gt;.</returns>
        public virtual List<GithubRepo> GetUserRepos(string githubUser) =>
            StreamJsonCollection<List<GithubRepo>>($"users/{githubUser}/repos").SelectMany(x => x).ToList();

        /// <summary>
        /// Get user repos as an asynchronous operation.
        /// </summary>
        /// <param name="githubUser">The github user.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        public virtual async Task<List<GithubRepo>> GetUserReposAsync(string githubUser) =>
            (await GetJsonCollectionAsync<List<GithubRepo>>($"users/{githubUser}/repos").ConfigAwait()).SelectMany(x => x).ToList();

        /// <summary>
        /// Gets the org repos.
        /// </summary>
        /// <param name="githubOrg">The github org.</param>
        /// <returns>List&lt;GithubRepo&gt;.</returns>
        public virtual List<GithubRepo> GetOrgRepos(string githubOrg) =>
            StreamJsonCollection<List<GithubRepo>>($"orgs/{githubOrg}/repos").SelectMany(x => x).ToList();

        /// <summary>
        /// Get org repos as an asynchronous operation.
        /// </summary>
        /// <param name="githubOrg">The github org.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        public virtual async Task<List<GithubRepo>> GetOrgReposAsync(string githubOrg) =>
            (await GetJsonCollectionAsync<List<GithubRepo>>($"orgs/{githubOrg}/repos").ConfigAwait()).SelectMany(x => x).ToList();

        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>System.String.</returns>
        public virtual string GetJson(string route)
        {
            var apiUrl = !route.IsUrl()
                ? BaseUrl.CombineWith(route)
                : route;

            if (GetJsonFilter != null)
                return GetJsonFilter(apiUrl);

            return apiUrl.GetJsonFromUrl(ApplyRequestFilters);
        }

        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>T.</returns>
        public virtual T GetJson<T>(string route) => GetJson(route).FromJson<T>();

        /// <summary>
        /// Get json as an asynchronous operation.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public virtual async Task<string> GetJsonAsync(string route)
        {
            var apiUrl = !route.IsUrl()
                ? BaseUrl.CombineWith(route)
                : route;

            if (GetJsonFilter != null)
                return GetJsonFilter(apiUrl);

            return await apiUrl.GetJsonFromUrlAsync(ApplyRequestFilters).ConfigAwait();
        }

        /// <summary>
        /// Get json as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        public virtual async Task<T> GetJsonAsync<T>(string route) =>
            (await GetJsonAsync(route).ConfigAwait()).FromJson<T>();

        /// <summary>
        /// Streams the json collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public virtual IEnumerable<T> StreamJsonCollection<T>(string route)
        {
            List<T> results;
            var nextUrl = BaseUrl.CombineWith(route);

            do
            {
                results = nextUrl.GetJsonFromUrl(ApplyRequestFilters,
                        responseFilter: res =>
                        {
                            var links = ParseLinkUrls(res.Headers["Link"]);
                            links.TryGetValue("next", out nextUrl);
                        })
                    .FromJson<List<T>>();

                foreach (var result in results)
                {
                    yield return result;
                }
            } while (results.Count > 0 && nextUrl != null);
        }

        /// <summary>
        /// Get json collection as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="route">The route.</param>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        public virtual async Task<List<T>> GetJsonCollectionAsync<T>(string route)
        {
            var to = new List<T>();
            List<T> results;
            var nextUrl = BaseUrl.CombineWith(route);

            do
            {
                results = (await nextUrl.GetJsonFromUrlAsync(ApplyRequestFilters,
                        responseFilter: res =>
                        {
                            var links = ParseLinkUrls(res.Headers["Link"]);
                            links.TryGetValue("next", out nextUrl);
                        }).ConfigAwait())
                    .FromJson<List<T>>();

                to.AddRange(results);
            } while (results.Count > 0 && nextUrl != null);

            return to;
        }

        /// <summary>
        /// Parses the link urls.
        /// </summary>
        /// <param name="linkHeader">The link header.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public virtual Dictionary<string, string> ParseLinkUrls(string linkHeader)
        {
            var map = new Dictionary<string, string>();
            var links = linkHeader;

            while (!string.IsNullOrEmpty(links))
            {
                var urlStartPos = links.IndexOf('<');
                var urlEndPos = links.IndexOf('>');

                if (urlStartPos == -1 || urlEndPos == -1)
                    break;

                var url = links.Substring(urlStartPos + 1, urlEndPos - urlStartPos - 1);
                var parts = links.Substring(urlEndPos).SplitOnFirst(',');

                var relParts = parts[0].Split(';');
                foreach (var relPart in relParts)
                {
                    var keyValueParts = relPart.SplitOnFirst('=');
                    if (keyValueParts.Length < 2)
                        continue;

                    var name = keyValueParts[0].Trim();
                    var value = keyValueParts[1].Trim().Trim('"');

                    if (name == "rel")
                    {
                        map[value] = url;
                    }
                }

                links = parts.Length > 1 ? parts[1] : null;
            }

            return map;
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="downloadUrl">The download URL.</param>
        /// <param name="fileName">Name of the file.</param>
        public virtual void DownloadFile(string downloadUrl, string fileName)
        {
            var webClient = new WebClient();
            webClient.Headers.Add(HttpHeaders.UserAgent, UserAgent);
            if (!string.IsNullOrEmpty(AccessToken))
                webClient.Headers.Add(HttpHeaders.Authorization, "token " + AccessToken);

            webClient.DownloadFile(downloadUrl, fileName);

            webClient.Dispose();
        }

        /// <summary>
        /// Gets the github gist.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns>GithubGist.</returns>
        public virtual GithubGist GetGithubGist(string gistId)
        {
            var json = GetJson($"/gists/{gistId}");
            return json.FromJson<GithubGist>();
        }

        /// <summary>
        /// Gets the github gist.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="version">The version.</param>
        /// <returns>GithubGist.</returns>
        public virtual GithubGist GetGithubGist(string gistId, string version)
        {
            var json = GetJson($"/gists/{gistId}/{version}");
            return json.FromJson<GithubGist>();
        }

        /// <summary>
        /// Gets the gist.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns>Gist.</returns>
        public virtual Gist GetGist(string gistId)
        {
            var response = GetGithubGist(gistId);
            return PopulateGist(response);
        }

        /// <summary>
        /// Gets the gist.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="version">The version.</param>
        /// <returns>Gist.</returns>
        public Gist GetGist(string gistId, string version)
        {
            var response = GetGithubGist(gistId, version);
            return PopulateGist(response);
        }

        /// <summary>
        /// Get gist as an asynchronous operation.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns>A Task&lt;Gist&gt; representing the asynchronous operation.</returns>
        public async Task<Gist> GetGistAsync(string gistId)
        {
            var response = await GetJsonAsync<GithubGist>($"/gists/{gistId}").ConfigAwait();
            return PopulateGist(response);
        }

        /// <summary>
        /// Get gist as an asynchronous operation.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="version">The version.</param>
        /// <returns>A Task&lt;Gist&gt; representing the asynchronous operation.</returns>
        public async Task<Gist> GetGistAsync(string gistId, string version)
        {
            var response = await GetJsonAsync<GithubGist>($"/gists/{gistId}/{version}").ConfigAwait();
            return PopulateGist(response);
        }

        /// <summary>
        /// Populates the gist.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>GithubGist.</returns>
        private GithubGist PopulateGist(GithubGist response)
        {
            if (response != null)
                response.UserId = response.Owner?.Login;

            return response;
        }

        /// <summary>
        /// Creates the gist.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="files">The files.</param>
        /// <returns>Gist.</returns>
        public virtual Gist CreateGist(string description, bool isPublic, Dictionary<string, object> files) =>
            CreateGithubGist(description, isPublic, files);

        /// <summary>
        /// Creates the gist.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="textFiles">The text files.</param>
        /// <returns>Gist.</returns>
        public virtual Gist CreateGist(string description, bool isPublic, Dictionary<string, string> textFiles) =>
            CreateGithubGist(description, isPublic, textFiles);

        /// <summary>
        /// Creates the github gist.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="files">The files.</param>
        /// <returns>GithubGist.</returns>
        public virtual GithubGist CreateGithubGist(string description, bool isPublic, Dictionary<string, object> files) =>
            CreateGithubGist(description, isPublic, ToTextFiles(files));

        /// <summary>
        /// Creates the github gist.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="textFiles">The text files.</param>
        /// <returns>GithubGist.</returns>
        public virtual GithubGist CreateGithubGist(string description, bool isPublic, Dictionary<string, string> textFiles)
        {
            AssertAccessToken();

            var sb = StringBuilderCache.Allocate()
                .Append("{\"description\":")
                .Append(description.ToJson())
                .Append(",\"public\":")
                .Append(isPublic ? "true" : "false")
                .Append(",\"files\":{");

            var i = 0;
            foreach (var entry in textFiles)
            {
                if (i++ > 0)
                    sb.Append(",");

                var jsonFile = entry.Key.ToJson();
                sb.Append(jsonFile)
                    .Append(":{\"content\":")
                    .Append(entry.Value.ToJson())
                    .Append("}");
            }
            sb.Append("}}");

            var json = StringBuilderCache.ReturnAndFree(sb);
            var responseJson = BaseUrl.CombineWith($"/gists")
                .PostJsonToUrl(json, requestFilter: ApplyRequestFilters);

            var response = responseJson.FromJson<GithubGist>();
            return response;
        }

        /// <summary>
        /// Determines whether [is dir sep] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is dir sep] [the specified c]; otherwise, <c>false</c>.</returns>
        public static bool IsDirSep(char c) => c == '\\' || c == '/';

        /// <summary>
        /// Sanitizes the path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        internal static string SanitizePath(string filePath)
        {
            var sanitizedPath = string.IsNullOrEmpty(filePath)
                ? null
                : IsDirSep(filePath[0]) ? filePath.Substring(1) : filePath;

            return sanitizedPath?.Replace('/', '\\');
        }

        /// <summary>
        /// Converts to base64.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.String.</returns>
        internal static string ToBase64(ReadOnlyMemory<byte> bytes) => MemoryProvider.Instance.ToBase64(bytes);

        /// <summary>
        /// Converts to base64.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.String.</returns>
        internal static string ToBase64(byte[] bytes) => Convert.ToBase64String(bytes);
        /// <summary>
        /// Converts to base64.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.String.</returns>
        internal static string ToBase64(Stream stream)
        {
            var base64 = stream is MemoryStream ms
                ? MemoryProvider.Instance.ToBase64(ms.GetBufferAsMemory())
                : Convert.ToBase64String(stream.ReadFully());
            return base64;
        }

        /// <summary>
        /// Converts to textfiles.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ToTextFiles(Dictionary<string, object> files)
        {
            string ToBase64ThenDispose(Stream stream)
            {
                using (stream)
                    return ToBase64(stream);
            }

            var gistFiles = new Dictionary<string, string>();
            foreach (var entry in files)
            {
                if (entry.Value == null)
                    continue;

                var filePath = SanitizePath(entry.Key);

                var base64 = entry.Value is string || entry.Value is ReadOnlyMemory<char>
                    ? null
                    : entry.Value is byte[] bytes
                        ? ToBase64(bytes)
                        : entry.Value is ReadOnlyMemory<byte> romBytes
                        ? ToBase64(romBytes)
                        : entry.Value is Stream stream
                            ? ToBase64ThenDispose(stream)
                            : entry.Value is IVirtualFile file &&
                              MimeTypes.IsBinary(MimeTypes.GetMimeType(file.Extension))
                                ? ToBase64(file.ReadAllBytes())
                                : null;

                if (base64 != null)
                    filePath += "|base64";

                var textContents = base64 ??
                   (entry.Value is string text
                       ? text
                       : entry.Value is ReadOnlyMemory<char> romChar
                           ? romChar.ToString()
                           : throw CreateContentNotSupportedException(entry.Value));

                gistFiles[filePath] = textContents;
            }
            return gistFiles;
        }

        /// <summary>
        /// Creates the content not supported exception.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>NotSupportedException.</returns>
        internal static NotSupportedException CreateContentNotSupportedException(object value) =>
            new($"Could not write '{value?.GetType().Name ?? "null"}' value. Only string, byte[], Stream or IVirtualFile content is supported.");

        /// <summary>
        /// Writes the gist files.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="files">The files.</param>
        /// <param name="description">The description.</param>
        /// <param name="deleteMissing">if set to <c>true</c> [delete missing].</param>
        public virtual void WriteGistFiles(string gistId, Dictionary<string, object> files, string description = null, bool deleteMissing = false) =>
            WriteGistFiles(gistId, ToTextFiles(files), description, deleteMissing);

        /// <summary>
        /// Create or Write Gist Text Files. Requires AccessToken
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="textFiles">The text files.</param>
        /// <param name="description">The description.</param>
        /// <param name="deleteMissing">if set to <c>true</c> [delete missing].</param>
        public virtual void WriteGistFiles(string gistId, Dictionary<string, string> textFiles, string description = null, bool deleteMissing = false)
        {
            AssertAccessToken();

            var i = 0;
            var sb = StringBuilderCache.Allocate().Append("{\"files\":{");
            foreach (var entry in textFiles)
            {
                if (i++ > 0)
                    sb.Append(",");

                var jsonFile = entry.Key.ToJson();
                sb.Append(jsonFile)
                    .Append(":{\"filename\":")
                    .Append(jsonFile)
                    .Append(",\"content\":")
                    .Append(entry.Value.ToJson())
                    .Append("}");
            }

            if (deleteMissing)
            {
                var gist = GetGist(gistId);
                foreach (var existingFile in gist.Files.Keys)
                {
                    if (textFiles.ContainsKey(existingFile))
                        continue;

                    if (i++ > 0)
                        sb.Append(",");

                    sb.Append(existingFile.ToJson())
                        .Append(":null");
                }
            }
            sb.Append("}");

            if (!string.IsNullOrEmpty(description))
            {
                if (i++ > 0)
                    sb.Append(",");
                sb.Append("\"description\":").Append(description.ToJson());
            }
            sb.Append("}");

            var json = StringBuilderCache.ReturnAndFree(sb);
            BaseUrl.CombineWith($"/gists/{gistId}")
                .PatchJsonToUrl(json, requestFilter: ApplyRequestFilters);
        }

        /// <summary>
        /// Create new Gist File. Requires AccessToken
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        public virtual void CreateGistFile(string gistId, string filePath, string contents)
        {
            AssertAccessToken();
            var jsonFile = filePath.ToJson();
            var sb = StringBuilderCache.Allocate().Append("{\"files\":{")
                .Append(jsonFile)
                .Append(":{")
                .Append("\"content\":")
                .Append(contents.ToJson())
                .Append("}}}");

            var json = StringBuilderCache.ReturnAndFree(sb);
            BaseUrl.CombineWith($"/gists/{gistId}")
                .PatchJsonToUrl(json, requestFilter: ApplyRequestFilters);
        }

        /// <summary>
        /// Create or Write Gist File. Requires AccessToken
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        public virtual void WriteGistFile(string gistId, string filePath, string contents)
        {
            AssertAccessToken();
            var jsonFile = filePath.ToJson();
            var sb = StringBuilderCache.Allocate().Append("{\"files\":{")
                .Append(jsonFile)
                .Append(":{\"filename\":")
                .Append(jsonFile)
                .Append(",\"content\":")
                .Append(contents.ToJson())
                .Append("}}}");

            var json = StringBuilderCache.ReturnAndFree(sb);
            BaseUrl.CombineWith($"/gists/{gistId}")
                .PatchJsonToUrl(json, requestFilter: ApplyRequestFilters);
        }

        /// <summary>
        /// Asserts the access token.
        /// </summary>
        /// <exception cref="System.NotSupportedException">An AccessToken is required to modify gist</exception>
        protected virtual void AssertAccessToken()
        {
            if (string.IsNullOrEmpty(AccessToken))
                throw new NotSupportedException("An AccessToken is required to modify gist");
        }

        /// <summary>
        /// Deletes the gist files.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="filePaths">The file paths.</param>
        public virtual void DeleteGistFiles(string gistId, params string[] filePaths)
        {
            AssertAccessToken();

            var i = 0;
            var sb = StringBuilderCache.Allocate().Append("{\"files\":{");
            foreach (var filePath in filePaths)
            {
                if (i++ > 0)
                    sb.Append(",");

                sb.Append(filePath.ToJson())
                  .Append(":null");
            }
            sb.Append("}}");

            var json = StringBuilderCache.ReturnAndFree(sb);
            BaseUrl.CombineWith($"/gists/{gistId}")
                .PatchJsonToUrl(json, requestFilter: ApplyRequestFilters);
        }

        /// <summary>
        /// Applies the request filters.
        /// </summary>
        /// <param name="req">The req.</param>
        public virtual void ApplyRequestFilters(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(AccessToken))
            {
                req.Headers["Authorization"] = "token " + AccessToken;
            }
            req.UserAgent = UserAgent;
        }
    }

    /// <summary>
    /// Class GithubRepo.
    /// </summary>
    public class GithubRepo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the homepage.
        /// </summary>
        /// <value>The homepage.</value>
        public string Homepage { get; set; }
        /// <summary>
        /// Gets or sets the watchers count.
        /// </summary>
        /// <value>The watchers count.</value>
        public int Watchers_Count { get; set; }
        /// <summary>
        /// Gets or sets the stargazers count.
        /// </summary>
        /// <value>The stargazers count.</value>
        public int Stargazers_Count { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; set; }
        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string Full_Name { get; set; }
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime Created_At { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>The updated at.</value>
        public DateTime? Updated_At { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has downloads.
        /// </summary>
        /// <value><c>true</c> if this instance has downloads; otherwise, <c>false</c>.</value>
        public bool Has_Downloads { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GithubRepo"/> is fork.
        /// </summary>
        /// <value><c>true</c> if fork; otherwise, <c>false</c>.</value>
        public bool Fork { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; } // https://api.github.com/repos/NetCoreWebApps/bare
        /// <summary>
        /// Gets or sets the HTML URL.
        /// </summary>
        /// <value>The HTML URL.</value>
        public string Html_Url { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GithubRepo"/> is private.
        /// </summary>
        /// <value><c>true</c> if private; otherwise, <c>false</c>.</value>
        public bool Private { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public GithubRepo Parent { get; set; } // only on single result, e.g: /repos/NetCoreWebApps/bare
    }

    /// <summary>
    /// Class Gist.
    /// </summary>
    public class Gist
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the HTML URL.
        /// </summary>
        /// <value>The HTML URL.</value>
        public string Html_Url { get; set; }
        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>The files.</value>
        public Dictionary<string, GistFile> Files { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Gist"/> is public.
        /// </summary>
        /// <value><c>true</c> if public; otherwise, <c>false</c>.</value>
        public bool Public { get; set; }
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime Created_At { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>The updated at.</value>
        public DateTime? Updated_At { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }
    }

    /// <summary>
    /// Class GistFile.
    /// </summary>
    public class GistFile
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language { get; set; }
        /// <summary>
        /// Gets or sets the raw URL.
        /// </summary>
        /// <value>The raw URL.</value>
        public string Raw_Url { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GistFile"/> is truncated.
        /// </summary>
        /// <value><c>true</c> if truncated; otherwise, <c>false</c>.</value>
        public bool Truncated { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }
    }

    /// <summary>
    /// Class GithubGist.
    /// Implements the <see cref="ServiceStack.Gist" />
    /// </summary>
    /// <seealso cref="ServiceStack.Gist" />
    public class GithubGist : Gist
    {
        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        /// <value>The node identifier.</value>
        public string Node_Id { get; set; }
        /// <summary>
        /// Gets or sets the git pull URL.
        /// </summary>
        /// <value>The git pull URL.</value>
        public string Git_Pull_Url { get; set; }
        /// <summary>
        /// Gets or sets the git push URL.
        /// </summary>
        /// <value>The git push URL.</value>
        public string Git_Push_Url { get; set; }
        /// <summary>
        /// Gets or sets the forks URL.
        /// </summary>
        /// <value>The forks URL.</value>
        public string Forks_Url { get; set; }
        /// <summary>
        /// Gets or sets the commits URL.
        /// </summary>
        /// <value>The commits URL.</value>
        public string Commits_Url { get; set; }
        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        public int Comments { get; set; }
        /// <summary>
        /// Gets or sets the comments URL.
        /// </summary>
        /// <value>The comments URL.</value>
        public string Comments_Url { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GithubGist"/> is truncated.
        /// </summary>
        /// <value><c>true</c> if truncated; otherwise, <c>false</c>.</value>
        public bool Truncated { get; set; }
        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public GithubUser Owner { get; set; }
        /// <summary>
        /// Gets or sets the history.
        /// </summary>
        /// <value>The history.</value>
        public GistHistory[] History { get; set; }
    }

    /// <summary>
    /// Class GistHistory.
    /// </summary>
    public class GistHistory
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public GithubUser User { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; set; }
        /// <summary>
        /// Gets or sets the committed at.
        /// </summary>
        /// <value>The committed at.</value>
        public DateTime Committed_At { get; set; }
        /// <summary>
        /// Gets or sets the change status.
        /// </summary>
        /// <value>The change status.</value>
        public GistChangeStatus Change_Status { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
    }

    /// <summary>
    /// Class GistChangeStatus.
    /// </summary>
    public class GistChangeStatus
    {
        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>The total.</value>
        public int Total { get; set; }
        /// <summary>
        /// Gets or sets the additions.
        /// </summary>
        /// <value>The additions.</value>
        public int Additions { get; set; }
        /// <summary>
        /// Gets or sets the deletions.
        /// </summary>
        /// <value>The deletions.</value>
        public int Deletions { get; set; }
    }

    /// <summary>
    /// Class GistUser.
    /// </summary>
    public class GistUser
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>The login.</value>
        public string Login { get; set; }
        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        /// <value>The avatar URL.</value>
        public string Avatar_Url { get; set; }
        /// <summary>
        /// Gets or sets the gravatar identifier.
        /// </summary>
        /// <value>The gravatar identifier.</value>
        public string Gravatar_Id { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the HTML URL.
        /// </summary>
        /// <value>The HTML URL.</value>
        public string Html_Url { get; set; }
        /// <summary>
        /// Gets or sets the gists URL.
        /// </summary>
        /// <value>The gists URL.</value>
        public string Gists_Url { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [site admin].
        /// </summary>
        /// <value><c>true</c> if [site admin]; otherwise, <c>false</c>.</value>
        public bool Site_Admin { get; set; }
    }

    /// <summary>
    /// Class GithubUser.
    /// Implements the <see cref="ServiceStack.GistUser" />
    /// </summary>
    /// <seealso cref="ServiceStack.GistUser" />
    public class GithubUser : GistUser
    {
        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        /// <value>The node identifier.</value>
        public string Node_Id { get; set; }
        /// <summary>
        /// Gets or sets the followers URL.
        /// </summary>
        /// <value>The followers URL.</value>
        public string Followers_Url { get; set; }
        /// <summary>
        /// Gets or sets the following URL.
        /// </summary>
        /// <value>The following URL.</value>
        public string Following_Url { get; set; }
        /// <summary>
        /// Gets or sets the starred URL.
        /// </summary>
        /// <value>The starred URL.</value>
        public string Starred_Url { get; set; }
        /// <summary>
        /// Gets or sets the subscriptions URL.
        /// </summary>
        /// <value>The subscriptions URL.</value>
        public string Subscriptions_Url { get; set; }
        /// <summary>
        /// Gets or sets the organizations URL.
        /// </summary>
        /// <value>The organizations URL.</value>
        public string Organizations_Url { get; set; }
        /// <summary>
        /// Gets or sets the repos URL.
        /// </summary>
        /// <value>The repos URL.</value>
        public string Repos_Url { get; set; }
        /// <summary>
        /// Gets or sets the events URL.
        /// </summary>
        /// <value>The events URL.</value>
        public string Events_Url { get; set; }
        /// <summary>
        /// Gets or sets the received events URL.
        /// </summary>
        /// <value>The received events URL.</value>
        public string Received_Events_Url { get; set; }
    }

    /// <summary>
    /// Class GithubGatewayExtensions.
    /// </summary>
    internal static class GithubGatewayExtensions
    {
        /// <summary>
        /// Determines whether the specified gist identifier is URL.
        /// </summary>
        /// <param name="gistId">The gist identifier.</param>
        /// <returns><c>true</c> if the specified gist identifier is URL; otherwise, <c>false</c>.</returns>
        public static bool IsUrl(this string gistId) => gistId.IndexOf("://", StringComparison.Ordinal) >= 0;
    }

    /// <summary>
    /// Class GistLink.
    /// </summary>
    public class GistLink
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public string User { get; set; }
        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public string To { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the gist identifier.
        /// </summary>
        /// <value>The gist identifier.</value>
        public string GistId { get; set; }

        /// <summary>
        /// Gets or sets the repo.
        /// </summary>
        /// <value>The repo.</value>
        public string Repo { get; set; }

        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        public Dictionary<string, object> Modifiers { get; set; }

        /// <summary>
        /// Converts to tagsstring.
        /// </summary>
        /// <returns>System.String.</returns>
        public string ToTagsString() => Tags == null ? "" : $"[" + string.Join(",", Tags) + "]";

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"{Name.PadRight(18, ' ')} {Description} {ToTagsString()}";

        /// <summary>
        /// Converts to listitem.
        /// </summary>
        /// <returns>System.String.</returns>
        public string ToListItem()
        {
            var sb = new StringBuilder(" - [")
                .Append(Name)
                .Append("](")
                .Append(Url)
                .Append(") {")
                .Append(!string.IsNullOrEmpty(To) ? "to:" + To.ToJson() : "")
                .Append("} `")
                .Append(Tags != null ? string.Join(",", Tags) : "")
                .Append("` ")
                .Append(Description);

            return sb.ToString();
        }

        /// <summary>
        /// Renders the links.
        /// </summary>
        /// <param name="links">The links.</param>
        /// <returns>System.String.</returns>
        public static string RenderLinks(List<GistLink> links)
        {
            var sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link.ToListItem());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parses the specified md.
        /// </summary>
        /// <param name="md">The md.</param>
        /// <returns>List&lt;GistLink&gt;.</returns>
        public static List<GistLink> Parse(string md)
        {
            var to = new List<GistLink>();

            if (!string.IsNullOrEmpty(md))
            {
                foreach (var strLine in md.ReadLines())
                {
                    var line = strLine.AsSpan();
                    if (!line.TrimStart().StartsWith("- ["))
                        continue;

                    line.SplitOnFirst('[', out _, out var startName);
                    startName.SplitOnFirst(']', out var name, out var endName);
                    endName.SplitOnFirst('(', out _, out var startUrl);
                    startUrl.SplitOnFirst(')', out var url, out var endUrl);

                    var afterModifiers = endUrl.ParseJsToken(out var token);

                    var modifiers = new Dictionary<string, object>();
                    if (token is JsObjectExpression obj)
                    {
                        foreach (var jsProperty in obj.Properties)
                        {
                            if (jsProperty.Key is JsIdentifier id)
                            {
                                modifiers[id.Name] = (jsProperty.Value as JsLiteral)?.Value;
                            }
                        }
                    }

                    var toPath = modifiers.TryGetValue("to", out var oValue)
                        ? oValue.ToString()
                        : null;

                    string tags = null;
                    afterModifiers = afterModifiers.TrimStart();
                    if (afterModifiers.StartsWith("`"))
                    {
                        afterModifiers = afterModifiers.Advance(1);
                        var pos = afterModifiers.IndexOf('`');
                        if (pos >= 0)
                        {
                            tags = afterModifiers.Substring(0, pos);
                            afterModifiers = afterModifiers.Advance(pos + 1);
                        }
                    }

                    if (name == null || url == null)
                        continue;

                    var link = new GistLink
                    {
                        Name = name.ToString(),
                        Url = url.ToString(),
                        Modifiers = modifiers,
                        To = toPath,
                        Description = afterModifiers.Trim().ToString(),
                        User = url.Substring("https://".Length).RightPart('/').LeftPart('/'),
                        Tags = tags?.Split(',').Map(x => x.Trim()).ToArray(),
                    };

                    if (TryParseGitHubUrl(link.Url, out var gistId, out var user, out var repo))
                    {
                        link.GistId = gistId;
                        if (user != null)
                        {
                            link.User = user;
                            link.Repo = repo;
                        }
                    }

                    if (link.User == "gistlyn" || link.User == "mythz")
                        link.User = "ServiceStack";

                    to.Add(link);
                }
            }

            return to;
        }

        /// <summary>
        /// Tries the parse git hub URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="gistId">The gist identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="repo">The repo.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryParseGitHubUrl(string url, out string gistId, out string user, out string repo)
        {
            gistId = user = repo = null;

            if (url.StartsWith("https://gist.github.com"))
            {
                gistId = url.LastRightPart('/');
                return true;
            }

            if (url.StartsWith("https://github.com/"))
            {
                var pathInfo = url.Substring("https://github.com/".Length);
                user = pathInfo.LeftPart('/');
                repo = pathInfo.RightPart('/').LeftPart('/');
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the specified links.
        /// </summary>
        /// <param name="links">The links.</param>
        /// <param name="gistAlias">The gist alias.</param>
        /// <returns>GistLink.</returns>
        public static GistLink Get(List<GistLink> links, string gistAlias)
        {
            var sanitizedAlias = gistAlias.Replace("-", "");
            var gistLink = links.FirstOrDefault(x => x.Name.Replace("-", "").EqualsIgnoreCase(sanitizedAlias));
            return gistLink;
        }

        /// <summary>
        /// Matcheses the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchesTag(string tagName)
        {
            if (Tags == null)
                return false;

            var searchTags = tagName.Split(',').Map(x => x.Trim());
            return searchTags.Count == 1
                ? Tags.Any(x => x.EqualsIgnoreCase(tagName))
                : Tags.Any(x => searchTags.Any(x.EqualsIgnoreCase));
        }
    }

}