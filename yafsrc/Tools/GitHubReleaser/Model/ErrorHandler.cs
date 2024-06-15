namespace GitHubReleaser.Model;

internal class ErrorHandler
{
    public static void Log(string errorMessage)
    {
        Serilog.Log.Error(errorMessage);
        throw new Exception(errorMessage);
    }
}