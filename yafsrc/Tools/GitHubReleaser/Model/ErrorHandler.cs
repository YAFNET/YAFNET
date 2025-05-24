namespace GitHubReleaser.Model;

/// <summary>
/// Class ErrorHandler.
/// </summary>
internal class ErrorHandler
{
    public static void Log(string errorMessage)
    {
        Serilog.Log.Error(errorMessage);
        throw new Exception(errorMessage);
    }
}