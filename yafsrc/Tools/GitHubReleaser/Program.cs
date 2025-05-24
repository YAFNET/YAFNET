using GitHubReleaser.Model;

namespace GitHubReleaser;

internal class Program
{
    private async static Task<int> Main(string[] args)
    {
        // Setup
        var argumentString = string.Join(" ", args.Skip(1));
        Console.Title = $"{nameof(GitHubReleaser)} {argumentString}";

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();

        // CommandLineParameters
        var commandLine = GetCommandline(args);

        // Execute
        try
        {
            var releaser = new Releaser(commandLine);
            await releaser.ExecuteAsync();
            Log.Information("All looks good, have fun with your release!");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during release process");
            return 100;
        }
    }

    private static CommandLineParameters GetCommandline(string[] args)
    {
        var commandLineParameters = CommandLineParameters.FromFile(args);

        // Checks
        commandLineParameters.FileForVersion = Path.GetFullPath(commandLineParameters.FileForVersion);

        if (!File.Exists(commandLineParameters.FileForVersion))
        {
            ErrorHandler.Log($"File not exists: {commandLineParameters.FileForVersion}");
        }

        var extension = Path.GetExtension(commandLineParameters.FileForVersion).ToLower();

        if (extension != ".dll" &&
            extension != ".exe")
        {
            ErrorHandler.Log($"File type not supported: {extension}");
        }

        if (commandLineParameters.ReleaseAttachmentFolder != null)
        {
            var folder = commandLineParameters.ReleaseAttachmentFolder;

            if (!Directory.Exists(folder))
            {
                ErrorHandler.Log($"Attachment folder not found: {folder}");
            }

            if (Directory.GetFiles(folder).Length == 0)
            {
                ErrorHandler.Log($"No Attachments in the folder: {folder}");
            }
        }

        LogParameter(nameof(commandLineParameters.GitHubRepo), commandLineParameters.GitHubRepo);
        LogParameter(nameof(commandLineParameters.GitHubToken), commandLineParameters.GitHubToken);
        LogParameter(nameof(commandLineParameters.FileForVersion), commandLineParameters.FileForVersion);
        LogParameter(nameof(commandLineParameters.IsChangelogFileCreationEnabled),
            commandLineParameters.IsChangelogFileCreationEnabled);
        LogParameter(nameof(commandLineParameters.IsPreRelease), commandLineParameters.IsPreRelease);
        LogParameter(nameof(commandLineParameters.IsUpdateOnly), commandLineParameters.IsUpdateOnly);
        LogParameter(nameof(commandLineParameters.IssueFilterLabel), commandLineParameters.IssueFilterLabel);
        LogParameter(nameof(commandLineParameters.IssueLabels), commandLineParameters.IssueLabels);
        LogParameter(nameof(commandLineParameters.ReleaseAttachmentFolder), commandLineParameters.ReleaseAttachmentFolder);

        return commandLineParameters;
    }

    private static void LogParameter(string name, object value)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString()))
        {
            return;
        }

        switch (value)
        {
            case List<string> list:
                {
                    var listValue = list.Aggregate(
                        string.Empty,
                        (current, item) => $"{current}{Environment.NewLine}\t\t{item}");

                    Log.Information("{Name}: {ListValue}", name, listValue);
                    break;
                }

            case Dictionary<string, string> list:
                {
                    var listValue = list.Aggregate(
                        string.Empty,
                        (current, item) => $"{current}{Environment.NewLine}\t\t{item.Key} -> {item.Value}");
                    Log.Information("{Name}: {ListValue}", name, listValue);
                    break;
                }

            default:
                Log.Information("{Name}: {Value}", name, value);
                break;
        }
    }
}