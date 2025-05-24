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

        if (!File.Exists(this.releaser.Settings.ChangeLogFile))
        {
            return changeLog;
        }

        var changeLogFull = await File.ReadAllTextAsync(this.releaser.Settings.ChangeLogFile);

        var currentVersion = "v" + this.releaser.VersionMilestone;

        changeLog = changeLogFull[(changeLogFull.IndexOf(currentVersion, StringComparison.Ordinal) + currentVersion.Length)..];

        changeLog = changeLog.Remove(changeLog.IndexOf("# ", StringComparison.Ordinal));

        return changeLog;
    }

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