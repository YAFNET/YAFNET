namespace GitHubReleaser.Model;

internal class CommandLineParameters : ReleaserSettings
{
    public static CommandLineParameters FromFile(string[] args)
    {
        if (args.Length != 1)
        {
            return null;
        }

        var configFile = args[0];

        if (!File.Exists(configFile))
        {
            Log.Error("File does not exits: {ConfigFile}", configFile);
            throw new FileNotFoundException("The file name passed with the argument 'config-file' does not exist",
                configFile);
        }

        var commandLineParameters = new CommandLineParameters();
        ReleaserSettings settings;
        var extension = Path.GetExtension(configFile);
        switch (extension)
        {
            case ".json":
                settings = JsonDeserialize(configFile);
                break;
            default:
                Log.Error("Unknown file type {Extension}", extension);
                throw new ArgumentException("The file name passed with the argument has an unknown file extension.");
        }

        commandLineParameters.MapProperties(settings);

        commandLineParameters.ConfigFile = configFile;

        return commandLineParameters;
    }

    private static ReleaserSettings JsonDeserialize(string configFile)
    {
        try
        {
            var jsonString = File.ReadAllText(configFile);
            var settings = JsonSerializer.Deserialize<ReleaserSettings>(jsonString);
            return settings;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }

    private void MapProperties(ReleaserSettings settings)
    {
        this.IsChangelogFileCreationEnabled = settings.IsChangelogFileCreationEnabled;
        this.IsUpdateOnly = settings.IsUpdateOnly;
        this.ReleaseAttachmentFolder = settings.ReleaseAttachmentFolder;
        this.IssueFilterLabel = settings.IssueFilterLabel;
        this.IssueLabels = settings.IssueLabels;
        this.IsPreRelease = settings.IsPreRelease;
        this.IsDraft = settings.IsDraft;
        this.DeleteFilesAfterUpload = settings.DeleteFilesAfterUpload;
        this.DnnFileForVersion = settings.DnnFileForVersion;
        this.FileForVersion = settings.FileForVersion;
        this.GitHubToken = Environment.ExpandEnvironmentVariables(settings.GitHubToken);
		this.GitHubRepo = settings.GitHubRepo;
        this.ChangeLogFile = settings.ChangeLogFile;
        this.Branch = settings.Branch;
    }
}