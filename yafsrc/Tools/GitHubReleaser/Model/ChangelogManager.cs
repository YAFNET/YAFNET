using System.Text;

namespace GitHubReleaser.Model;

internal class ChangelogManager
{
    private readonly Releaser releaser;

    private readonly static string Alert = $"| :warning: **Pre-release**|{Environment.NewLine}| --- |";

    public ChangelogManager(Releaser releaser)
    {
        this.releaser = releaser;
    }

    public async Task SetAsync(Release release)
    {
        var updateRelease = release.ToUpdate();
        updateRelease.Body = await this.GetReleaseChangelogAsync();
        await this.releaser.Client.Repository.Release.Edit(this.releaser.Account, this.releaser.Repo, release.Id, updateRelease);
    }

    private async Task<string> GetReleaseChangelogAsync()
    {
        var changeLog = string.Empty;

        if (File.Exists(this.releaser.Settings.ChangeLogFile))
        {
            var changeLogFull = await File.ReadAllTextAsync(this.releaser.Settings.ChangeLogFile);

            var currentVersion = "v" + this.releaser.VersionMilestone;

            changeLog = changeLogFull[(changeLogFull.IndexOf(currentVersion, StringComparison.Ordinal) + currentVersion.Length)..];

            changeLog = changeLog.Remove(changeLog.IndexOf("# ", StringComparison.Ordinal));
        }

        return changeLog;
    }

    /*private async Task<string> GetReleaseChangelogAsync()
    {
        DateTimeOffset? lastReleaseCreatedDate = null;
        if (this._releaser.Settings.IsPreRelease)
        {
            var releases = await this._releaser.Client.Repository.Release.GetAll(this._releaser.Account, this._releaser.Repo);
            var lastRelease = releases.MaxBy(obj => obj.CreatedAt.DateTime);
            if (lastRelease != null)
            {
                lastReleaseCreatedDate = lastRelease.PublishedAt;
            }
        }

        var repository = await this._releaser.Client.Repository.Get(this._releaser.Account, this._releaser.Repo);
        var allIssues = await this._releaser.Client.Issue.GetAllForRepository(repository.Id,
                            new RepositoryIssueRequest
                                {
                                    State = ItemStateFilter.Closed 
                                });
        var issuesWithLabel = new List<IssueWithLabel>();
        foreach (var issue in allIssues)
        {
            if (issue.Milestone == null)
            {
                continue;
            }

            if (!issue.Milestone.Title.Equals(this._releaser.VersionMilestone))
            {
                continue;
            }

            if (this._releaser.Settings.IssueFilterLabel != null)
            {
                if (issue.Labels.Any(obj => obj.Name.ToLower().Equals(this._releaser.Settings.IssueFilterLabel.ToLower())))
                {
                    continue;
                }
            }

            if (lastReleaseCreatedDate != null)
            {
                if (issue.ClosedAt <= lastReleaseCreatedDate)
                {
                    continue;
                }
            }

            // Filter by issue label
            if (this._releaser.Settings.IssueLabels != null && this._releaser.Settings.IssueLabels.Any())
            {
                foreach (var label in issue.Labels)
                {
                    this._releaser.Settings.IssueLabels.TryGetValue(label.Name, out var labelHeader);
                    if (labelHeader != null)
                    {
                        issuesWithLabel.Add(new IssueWithLabel(labelHeader, issue));
                        break;
                    }
                }
            }
            else
            {
                issuesWithLabel.Add(new IssueWithLabel(null, issue));
            }
        }

        // Build changelog text
        var issueGroups = issuesWithLabel.GroupBy(obj => obj.Label).OrderBy(obj => obj.Key);
        var changelog = this.GetChangelogFromIssues(issueGroups);

        if (this._releaser.Settings.IsPreRelease)
        {
            changelog = Alert + Environment.NewLine + changelog;
        }

        return changelog;
    }

    private string GetChangelogFromIssues(IEnumerable<IGrouping<string, IssueWithLabel>> issueGroups)
    {
        Log.Information("Issues:");
        var sb = new StringBuilder();
        foreach (var issueGroup in issueGroups)
        {
            sb.AppendLine();
            sb.AppendLine($"#### {issueGroup.Key}:");
            Console.WriteLine($"\t{issueGroup.Key}");
            foreach (var issueWithLabel in issueGroup.OrderBy(obj => obj.Issue.Title))
            {
                sb.AppendLine($"- [{issueWithLabel.Issue.Title}]({issueWithLabel.Issue.HtmlUrl})");
                Console.WriteLine($"\t\t{issueWithLabel.Issue.Title}");
            }
        }

        var changelog = sb.ToString().Trim();
        return changelog;
    }*/

    async internal Task SetAsync()
    {
        Log.Information("Create Changelog...");

        var sb = new StringBuilder();

        var releases = await this.releaser.Client.Repository.Release.GetAll(this.releaser.Account, this.releaser.Repo);
        foreach (var release in releases.OrderByDescending(obj => obj.CreatedAt.Date))
        {
            var version = new Version(release.Name);
            var versionToDisplay = $"{version.Major}.{version.Minor}.{version.Build}";
            var dateTime = release.CreatedAt.DateTime.ToUniversalTime();
            dateTime = dateTime.AddDays(1); // Don't know why this is needed
            var dateTimeToDisplay = dateTime.ToString("yyyy-MM-dd HH:mm");

            sb.AppendLine($"## [{versionToDisplay}]({release.HtmlUrl})");
            sb.AppendLine();

            if (release.Prerelease)
            {
                sb.AppendLine($"`Build: {version.Revision} | Date (UTC): {dateTimeToDisplay} | Pre-release`");
            }
            else
            {
                sb.AppendLine($"`Build: {version.Revision} | Date (UTC): {dateTimeToDisplay}`");
            }

            sb.AppendLine();

            var changeLog = release.Body.Trim();
            changeLog = changeLog.Replace(Alert, string.Empty); // Remove alert
            var changeLogLines = changeLog.Split('\n');
            foreach (var line in changeLogLines.Where(line => !line.StartsWith('|')))
            {
                // Ignore Tables
                sb.AppendLine(line);
            }

            sb.AppendLine();
        }

        // Commit Changelog
        var changelog = sb.ToString();
        var contents = await this.releaser.Client
                           .Repository
                           .Content
                           .GetAllContents(this.releaser.Account, this.releaser.Repo);
        const string path = "CHANGELOG.md";
        var content = contents.FirstOrDefault(obj => obj.Path.Equals(path));
        if (content == null)
        {
            ErrorHandler.Log("Changelog.md not found");
        }

        await this.releaser.Client.Repository.Content.CreateFile(
            this.releaser.Account,
            this.releaser.Repo,
            path,
            new UpdateFileRequest("Changelog",
                changelog, content!.Sha));

        Console.WriteLine(changelog);
    }
}