namespace GitHubReleaser.Model;

internal class ReleaseManager
{
    private readonly Releaser _releaser;

    public ReleaseManager(Releaser releaser)
    {
        this._releaser = releaser;
    }

    public async Task<Release> UpdateReleaseAsync()
    {
        var releases = await this._releaser.Client.Repository.Release.GetAll(this._releaser.Account, this._releaser.Repo); // Get() throw exception if not found
        var release = releases.FirstOrDefault(obj => obj.Name.Equals(this._releaser.VersionFull));

        if (release == null)
        {
            ErrorHandler.Log("Release to update not found");
        }

        var updateRelease = release!.ToUpdate();
        updateRelease.Draft = this._releaser.Settings.IsDraft;
        var result = await this._releaser.Client.Repository.Release.Edit(this._releaser.Account, this._releaser.Repo, release.Id, updateRelease);

        // Attachments
        if (result.Assets.Any())
        {
            foreach (var asset in result.Assets)
            {
                await this._releaser.Client.Repository.Release.DeleteAsset(this._releaser.Account, this._releaser.Repo, asset.Id);
            }
        }

        await this.UploadAttachmentsAsync(result);
        return result;
    }

    public async Task<Release> CreateReleaseAsync()
    {
        // Check existing
        var releases = await this._releaser.Client.Repository.Release.GetAll(this._releaser.Account, this._releaser.Repo); // Get() throw exception if not found
        var release = releases.FirstOrDefault(obj => obj.Name.Equals(this._releaser.VersionFull));
        if (release != null)
        {
            ErrorHandler.Log("Release already exists. Please use update.");
        }

        // Create
        Log.Information("Create release...");
        var newRelease = new NewRelease(this._releaser.TagName)
                             {
                                 Name = this._releaser.VersionFull,
                                 Prerelease = this._releaser.Settings.IsPreRelease,
                                 Draft = this._releaser.Settings.IsDraft
                             };

        if (this._releaser.Settings.Branch is not null)
        {
            newRelease.TargetCommitish = this._releaser.Settings.Branch;
        }

        release = await this._releaser.Client.Repository.Release.Create(this._releaser.Account, this._releaser.Repo, newRelease);

        // Upload: Attachments
        await this.UploadAttachmentsAsync(release);
        return release;
    }

    private async Task UploadAttachmentsAsync(Release release)
    {
        if (this._releaser.Settings.ReleaseAttachmentFolder == null)
        {
            return;
        }

        Log.Information("Upload attachments...");

        try
        {
            var files = Directory.GetFiles(this._releaser.Settings.ReleaseAttachmentFolder, "*.zip");

            foreach (var file in files)
            {
                Log.Information(file);

                var setupFile = file;

                await using var archiveContents = File.OpenRead(setupFile);

                var assetFilename = Path.GetFileName(setupFile);
                var assetUpload = new ReleaseAssetUpload
                                      {
                                          FileName = assetFilename,
                                          ContentType = "application/x-msdownload",
                                          RawData = archiveContents,
                                          Timeout = TimeSpan.FromHours(1) // Needed because there is a global timeout
                                      };
                await this._releaser.Client.Repository.Release.UploadAsset(release, assetUpload);
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception.ToString());
        }
    }
}