

namespace GitHubReleaser.Model;

using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

internal partial class Releaser
{
    public ReleaserSettings Settings { get; set; }

    public GitHubClient Client { get; }

    public string Account { get; }

    public string Repo { get; }

    public string VersionMilestone { get; }

    public string VersionFull { get; }

    public string TagName { get; }

    public Releaser(ReleaserSettings releaserSettings)
    {
        this.Settings = releaserSettings;

        if (this.Settings.DnnFileForVersion != null)
        {
            var xDoc = XDocument.Load(this.Settings.DnnFileForVersion!);

            var versionNode = xDoc.Document
                .XPathSelectElement("./dotnetnuke/packages/package[starts-with(@name,'YetAnotherForumDotNet')]")
                .Attribute("version").Value;

            var versionRegex = VersionRegex();

            var dnnBuild = versionRegex.Replace(versionNode, "-$6");

            var fileVersion = FileVersionInfo.GetVersionInfo(this.Settings.FileForVersion!);

            var version = new Version(fileVersion.FileVersion!);
            this.VersionMilestone = version.ToString(3);

            var split = this.Settings.GitHubRepo.Split('/');
            this.Account = split[0];
            this.Repo = split[^1];

            this.VersionFull = $"{this.Repo} v{version.ToString(3)}";
            this.TagName = $"v{version.ToString(3)}{dnnBuild}";
        }
        else
        {
            var fileVersion = FileVersionInfo.GetVersionInfo(this.Settings.FileForVersion!);

            var version = new Version(fileVersion.FileVersion!);
            this.VersionMilestone = version.ToString(3);

            var split = this.Settings.GitHubRepo.Split('/');
            this.Account = split[0];
            this.Repo = split[^1];

            this.VersionFull = $"{this.Repo} v{version.ToString(3)}";
            this.TagName = $"v{version.ToString(3)}";
        }

        // GitHub
        /*ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls12; // needed https://github.com/octokit/octokit.net/issues/1756*/
        var connection = new Connection(new ProductHeaderValue(this.Repo));
        this.Client = new GitHubClient(connection);
        var tokenAuth = new Credentials(this.Settings.GitHubToken);
        this.Client.Credentials = tokenAuth;
    }

    public async Task ExecuteAsync()
    {
        // Release
        var releaseManager = new ReleaseManager(this);
        Release release;
        if (this.Settings.IsUpdateOnly)
        {
            release = await releaseManager.UpdateReleaseAsync();
        }
        else
        {
            release = await releaseManager.CreateReleaseAsync();
        }

        // Changelog
        var changelogManager = new ChangelogManager(this);
        await changelogManager.SetAsync(release);
        if (this.Settings.IsChangelogFileCreationEnabled)
        {
            await changelogManager.SetAsync();
        }
    }

    [GeneratedRegex("([0-9])([0-9]).([0-9])([0-9]).(00)([0-9]+)")]
    private static partial Regex VersionRegex();
}