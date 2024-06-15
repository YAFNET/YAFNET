namespace GitHubReleaser.Model;

internal class ReleaserSettings
{
    public string ConfigFile { get; set; }

    public bool IsChangelogFileCreationEnabled { get; set; }

    public bool IsUpdateOnly { get; set; }

    public string ReleaseAttachmentFolder { get; set; }

    public string IssueFilterLabel { get; set; }

    public Dictionary<string, string> IssueLabels { get; set; }

    public bool IsPreRelease { get; set; }

    public bool IsDraft { get; set; }

    public bool DeleteFilesAfterUpload { get; set; }

    public string FileForVersion { get; set; }

    public string DnnFileForVersion { get; set; }

    public string GitHubToken { get; set; }

    public string GitHubRepo { get; set; }

    public string ChangeLogFile { get; set; }

    public string Branch { get; set; }
}